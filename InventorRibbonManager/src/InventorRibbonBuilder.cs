using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;

//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-RibbonTab
//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-RibbonPanel
//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-CommandControls

namespace InventorRibbonManager
    {
    /// <summary>
    /// Creates and manages Autodesk Inventor ribbon tabs and panels for an add-in.
    /// </summary>
    public class InventorRibbonBuilder : IDisposable
        {
        #region Fields
        private readonly string _clientId;
        private Inventor.Application? _app;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InventorRibbonBuilder"/> class.
        /// </summary>
        /// <param name="app">The active Inventor application instance.</param>
        /// <param name="guid">The add-in client identifier used to create ribbon elements.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="app"/> or <paramref name="guid"/> is null.
        /// </exception>
        public InventorRibbonBuilder(Inventor.Application app, string guid)
            {
            _app = app ?? throw new ArgumentNullException(nameof(app));
            _clientId = !string.IsNullOrWhiteSpace(guid) ? guid : throw new ArgumentException("The add-in client identifier must not be null or empty.", nameof(guid));
            }

        #endregion Constructor

        #region Dispose

        public void Dispose()
            {
            _app = null;
            }

        #endregion Dispose

        #region Methods

        /// <summary>
        /// Creates all ribbon panels and registers all buttons for the specified tab in the requested ribbon environments.
        /// </summary>
        /// <param name="ribbonBuilder">The ribbon builder used to create tabs and panels.</param>
        /// <param name="tabName">The display name of the ribbon tab.</param>
        /// <param name="tabInternalName">The internal name of the ribbon tab.</param>
        /// <param name="buttons">The button definitions to place on the tab.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="ribbonBuilder"/> or <paramref name="buttons"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="tabName"/> or <paramref name="tabInternalName"/> is null or empty.
        /// </exception>
        public static void CreatePanelsAndButtonsForTab(InventorRibbonBuilder ribbonBuilder, string tabName, string tabInternalName, IEnumerable<IPanelButton> buttons)
            {
            if (ribbonBuilder == null)
                { throw new ArgumentNullException(nameof(ribbonBuilder)); }

            if (buttons == null)
                { throw new ArgumentNullException(nameof(buttons)); }

            if (string.IsNullOrWhiteSpace(tabName))
                { throw new ArgumentException("The tab name must not be null or empty.", nameof(tabName)); }

            if (string.IsNullOrWhiteSpace(tabInternalName))
                { throw new ArgumentException("The internal tab name must not be null or empty.", nameof(tabInternalName)); }

            List<IPanelButton> buttonList = buttons.ToList();

            // Prevent duplicate registration of the same button in the same ribbon environment.
            HashSet<(string ButtonInternalName, RibbonType Environment)> registered = new HashSet<(string ButtonInternalName, RibbonType Environment)>();

            // Build the UI once per concrete Inventor ribbon environment.
            foreach (RibbonType env in GetSingleRibbonTypes())
                {
                List<IPanelButton> buttonsForEnvironment = buttonList
                    .Where(button => button.Environment.HasFlag(env))
                    .ToList();

                if (buttonsForEnvironment.Count == 0)
                    { continue; }

                // Group buttons by target panel so each panel is created only once per environment.
                foreach (var panelGroup in buttonsForEnvironment.GroupBy(b => (b.PanelName, b.PanelInternalName)))
                    {
                    if (string.IsNullOrWhiteSpace(panelGroup.Key.PanelName))
                        { throw new ArgumentException("A button definition contains an empty panel name.", nameof(buttons)); }

                    if (string.IsNullOrWhiteSpace(panelGroup.Key.PanelInternalName))
                        { throw new ArgumentException("A button definition contains an empty internal panel name.", nameof(buttons)); }

                    // Register the panel only for the current environment.
                    ribbonBuilder.RegisterPanelForRibbons(tabName, tabInternalName, panelGroup.Key.PanelName,
                        panelGroup.Key.PanelInternalName, env, (panel, currentEnvironment) =>
                        {
                            foreach (IPanelButton button in panelGroup)
                                {
                                // Skip duplicate button registrations for the same environment.
                                if (!registered.Add((button.InternalName, currentEnvironment)))
                                    { continue; }

                                button.AddToPanel(panel, currentEnvironment);
                                }
                        });
                    }
                }
            }

        /// <summary>
        /// Creates a ribbon tab if it does not already exist, or returns the existing tab.
        /// </summary>
        /// <param name="ribbonType">The target ribbon environment.</param>
        /// <param name="name">The display name of the tab.</param>
        /// <param name="internalName">The internal name of the tab.</param>
        /// <returns>An <see cref="IRibbonTab"/> instance for the requested ribbon tab.</returns>
        public IRibbonTab CreateOrGetTab(RibbonType ribbonType, string name, string internalName)
            {
            if (string.IsNullOrWhiteSpace(name))
                { throw new ArgumentException("The tab name must not be null or empty.", nameof(name)); }

            if (string.IsNullOrWhiteSpace(internalName))
                { throw new ArgumentException("The internal tab name must not be null or empty.", nameof(internalName)); }

            Ribbon ribbon = GetRibbon(ribbonType);

            foreach (RibbonTab tab in ribbon.RibbonTabs)
                {
                if (string.Equals(tab.InternalName, internalName, StringComparison.Ordinal))
                    { return new RibbonTabWrapper(tab, _clientId); }
                }

            RibbonTab newTab = ribbon.RibbonTabs.Add(name, internalName, _clientId);
            return new RibbonTabWrapper(newTab, _clientId);
            }

        /// <summary>
        /// Creates or gets a ribbon panel in all specified ribbon environments and invokes an optional callback
        /// to configure the panel for each environment.
        /// </summary>
        /// <param name="tabName">The display name of the ribbon tab.</param>
        /// <param name="tabInternalName">The internal name of the ribbon tab.</param>
        /// <param name="panelDisplayName">The display name of the ribbon panel.</param>
        /// <param name="panelInternalName">The internal name of the ribbon panel.</param>
        /// <param name="ribbonTypes">
        /// A flag combination of ribbon environments in which the panel should be created,
        /// for example <see cref="RibbonType.Part"/> | <see cref="RibbonType.Assembly"/>.
        /// </param>
        /// <param name="configurePanel">
        /// An optional callback that is invoked for each created or existing panel together with its ribbon environment.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="tabName"/>, <paramref name="tabInternalName"/>,
        /// <paramref name="panelDisplayName"/>, or <paramref name="panelInternalName"/> is null or empty.
        /// </exception>
        public void RegisterPanelForRibbons(string tabName, string tabInternalName, string panelDisplayName, string panelInternalName, RibbonType ribbonTypes, Action<IRibbonPanel, RibbonType> configurePanel)
            {
            if (string.IsNullOrWhiteSpace(tabName))
                { throw new ArgumentException("The tab name must not be null or empty.", nameof(tabName)); }

            if (string.IsNullOrWhiteSpace(tabInternalName))
                { throw new ArgumentException("The internal tab name must not be null or empty.", nameof(tabInternalName)); }

            if (string.IsNullOrWhiteSpace(panelDisplayName))
                { throw new ArgumentException("The panel display name must not be null or empty.", nameof(panelDisplayName)); }

            if (string.IsNullOrWhiteSpace(panelInternalName))
                { throw new ArgumentException("The internal panel name must not be null or empty.", nameof(panelInternalName)); }

            foreach (RibbonType type in GetSingleRibbonTypes())
                {
                if (!ribbonTypes.HasFlag(type))
                    { continue; }

                IRibbonTab tab = CreateOrGetTab(type, tabName, tabInternalName);
                IRibbonPanel panel = tab.AddPanel(panelDisplayName, panelInternalName);
                configurePanel?.Invoke(panel, type);
                }
            }

        /// <summary>
        /// Creates or gets a ribbon panel in all supported ribbon environments and invokes an optional callback
        /// to configure the panel for each environment.
        /// </summary>
        /// <param name="tabName">The display name of the ribbon tab.</param>
        /// <param name="tabInternalName">The internal name of the ribbon tab.</param>
        /// <param name="panelDisplayName">The display name of the ribbon panel.</param>
        /// <param name="panelInternalName">The internal name of the ribbon panel.</param>
        /// <param name="configurePanel">
        /// An optional callback that is invoked for each created or existing panel together with its ribbon environment.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="tabName"/>, <paramref name="tabInternalName"/>,
        /// <paramref name="panelDisplayName"/>, or <paramref name="panelInternalName"/> is null or empty.
        /// </exception>
        public void RegisterPanelForRibbons(string tabName, string tabInternalName, string panelDisplayName, string panelInternalName, Action<IRibbonPanel, RibbonType> configurePanel)
            {
            RegisterPanelForRibbons(tabName, tabInternalName, panelDisplayName, panelInternalName, RibbonType.All, configurePanel);
            }

        #endregion Methods

        #region private Methods

        /// <summary>
        /// Gets the Autodesk Inventor internal ribbon name for a single ribbon environment.
        /// </summary>
        /// <param name="ribbonType">The ribbon environment.</param>
        /// <returns>The internal Inventor ribbon name.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="ribbonType"/> is not a single supported ribbon environment.
        /// </exception>
        private static string GetRibbonNameFromRibbonType(RibbonType ribbonType)
            {
            if (!IsSingleRibbonType(ribbonType) || ribbonType == RibbonType.None || ribbonType == RibbonType.All)
                { throw new ArgumentException("The ribbon type must be a single supported ribbon environment.", nameof(ribbonType)); }

            return ribbonType switch
                {
                    RibbonType.ZeroDoc => "ZeroDoc",
                    RibbonType.Part => "Part",
                    RibbonType.Assembly => "Assembly",
                    RibbonType.Drawing => "Drawing",
                    RibbonType.Presentation => "Presentation",
                    _ => throw new ArgumentOutOfRangeException(nameof(ribbonType), ribbonType, "Unsupported ribbon type."),
                    };
            }

        private static IEnumerable<RibbonType> GetSingleRibbonTypes()
            {
            yield return RibbonType.ZeroDoc;
            yield return RibbonType.Part;
            yield return RibbonType.Assembly;
            yield return RibbonType.Drawing;
            yield return RibbonType.Presentation;
            }

        /// <summary>
        /// Determines whether the specified ribbon type contains exactly one ribbon environment flag.
        /// </summary>
        /// <param name="ribbonType">The ribbon type to test.</param>
        /// <returns>
        /// <see langword="true"/> if the value contains exactly one ribbon environment flag;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        private static bool IsSingleRibbonType(RibbonType ribbonType)
            {
            int value = (int)ribbonType;
            return value != 0 && (value & (value - 1)) == 0;
            }

        /// <summary>
        /// Gets the Autodesk Inventor ribbon for the specified ribbon environment.
        /// </summary>
        /// <param name="ribbonType">The ribbon environment.</param>
        /// <returns>The matching Inventor <see cref="Ribbon"/>.</returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the current <see cref="InventorRibbonBuilder"/> instance has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="ribbonType"/> is not a single supported ribbon environment.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the requested ribbon cannot be found in the current Inventor user interface.
        /// </exception>
        private Ribbon GetRibbon(RibbonType ribbonType)
            {
            if (_app == null)
                { throw new ObjectDisposedException(nameof(InventorRibbonBuilder)); }

            string ribbonName = GetRibbonNameFromRibbonType(ribbonType);
            Ribbons ribbons = _app.UserInterfaceManager.Ribbons;

            try
                {
                return ribbons[ribbonName];
                }
            catch (Exception ex)
                {
                throw new InvalidOperationException($"The Inventor ribbon '{ribbonName}' for ribbon type '{ribbonType}' could not be found.", ex);
                }
            }

        #endregion private Methods

        #region Classes

        /// <inheritdoc />
        public class ComboBoxDefinitionWrapper : IComboBoxDefinition
            {
            #region Properties

            /// <inheritdoc />
            public string InternalName => InventorComboBoxDefinition.InternalName;

            /// <inheritdoc />
            public ComboBoxDefinition InventorComboBoxDefinition { get; }

            #endregion Properties

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="ComboBoxDefinitionWrapper"/> class.
            /// </summary>
            /// <param name="comboBoxDefinition">The Inventor combo box definition to wrap.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="comboBoxDefinition"/> is <see langword="null"/>.
            /// </exception>
            public ComboBoxDefinitionWrapper(ComboBoxDefinition comboBoxDefinition)
                {
                InventorComboBoxDefinition = comboBoxDefinition ?? throw new ArgumentNullException(nameof(comboBoxDefinition));
                }

            #endregion Constructor
            }

        /// <inheritdoc />
        public class CommandControlWrapper : ICommandControl
            {
            #region Fields
            private readonly CommandControl _control;

            #endregion Fields

            #region Properties

            /// <inheritdoc />
            public RibbonType Environment { get; }

            /// <inheritdoc />
            public string InternalName => _control.InternalName;

            /// <inheritdoc />
            public object InventorControl => _control;

            #endregion Properties

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandControlWrapper"/> class.
            /// </summary>
            /// <param name="control">The Inventor command control to wrap.</param>
            /// <param name="environment">The ribbon environment in which the command control exists.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="control"/> is <see langword="null"/>.
            /// </exception>
            public CommandControlWrapper(CommandControl control, RibbonType environment)
                {
                _control = control ?? throw new ArgumentNullException(nameof(control));
                Environment = environment;
                }

            #endregion Constructor
            }

        /// <inheritdoc />
        public sealed class CustomButtonDefinition : IButtonDefinition, IDisposable
            {
            #region Fields
            private readonly Action? _executeAction;

            private bool _disposed;

            #endregion Fields

            #region Properties

            /// <inheritdoc />
            public string DisplayName => InventorButtonDefinition.DisplayName;

            /// <inheritdoc />
            public RibbonType Environment { get; }

            /// <inheritdoc />
            public string InternalName => InventorButtonDefinition.InternalName;

            /// <summary>
            /// Gets the wrapped Inventor <see cref="ButtonDefinition"/>.
            /// </summary>
            public ButtonDefinition InventorButtonDefinition { get; }

            /// <inheritdoc />
            public object? LargeIcon { get; }

            /// <inheritdoc />
            public bool Pressed { get; set; }

            /// <inheritdoc />
            public object? StandardIcon { get; }

            #endregion Properties

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomButtonDefinition"/> class.
            /// </summary>
            /// <param name="definition">The underlying Inventor button definition.</param>
            /// <param name="environment">The ribbon environment in which the button is intended to be used.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="definition"/> is <see langword="null"/>.
            /// </exception>
            public CustomButtonDefinition(ButtonDefinition definition, RibbonType environment = RibbonType.All)
                : this(definition, null, environment)
                { }

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomButtonDefinition"/> class.
            /// </summary>
            /// <param name="definition">The underlying Inventor button definition.</param>
            /// <param name="onExecute">The action to invoke when the button is executed.</param>
            /// <param name="environment">The ribbon environment in which the button is intended to be used.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="definition"/> is <see langword="null"/>.
            /// </exception>
            public CustomButtonDefinition(ButtonDefinition definition, Action? onExecute, RibbonType environment = RibbonType.All)
                {
                InventorButtonDefinition = definition ?? throw new ArgumentNullException(nameof(definition));
                _executeAction = onExecute;
                Environment = environment;

                InventorButtonDefinition.OnExecute += OnExecute;
                }

            #endregion Constructor

            #region Dispose

            /// <summary>
            /// Unsubscribes the registered execute handler.
            /// </summary>
            public void Dispose()
                {
                if (_disposed)
                    { return; }
                InventorButtonDefinition.OnExecute -= OnExecute;
                _disposed = true;

                GC.SuppressFinalize(this);
                }

            #endregion Dispose

            #region Methods

            /// <inheritdoc />
            public void Execute()
                {
                _executeAction?.Invoke();
                }

            #endregion Methods

            #region private Methods

            private void OnExecute(NameValueMap context)
                {
                _executeAction?.Invoke();
                }

            #endregion private Methods
            }

        /// <inheritdoc />
        public sealed class RibbonPanelWrapper : IRibbonPanel
            {
            #region Fields
            private readonly RibbonPanel _panel;

            #endregion Fields

            #region Properties

            /// <inheritdoc />
            public string InternalName => _panel.InternalName;

            /// <inheritdoc />
            public string Name => _panel.DisplayName;

            #endregion Properties

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="RibbonPanelWrapper"/> class.
            /// </summary>
            /// <param name="panel">The Inventor ribbon panel to wrap.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="panel"/> is <see langword="null"/>.
            /// </exception>
            public RibbonPanelWrapper(RibbonPanel panel)
                {
                _panel = panel ?? throw new ArgumentNullException(nameof(panel));
                }

            #endregion Constructor

            #region Methods

            /// <inheritdoc />
            public ICommandControl AddButton(IButtonDefinition buttonDef, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (buttonDef == null)
                    { throw new ArgumentNullException(nameof(buttonDef)); }

                try
                    {
                    if (_panel.CommandControls[buttonDef.InternalName] is CommandControl existing)
                        { return new CommandControlWrapper(existing, environment); }
                    }
                catch (System.Runtime.InteropServices.COMException)
                    {
                    // The control does not exist yet. Create it below.
                    }
                catch (ArgumentException)
                    {
                    // The control does not exist yet. Create it below.
                    }

                CommandControl newControl = _panel.CommandControls.AddButton(buttonDef.InventorButtonDefinition, useLargeIcon);
                return new CommandControlWrapper(newControl, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddButtonPopup(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl ctrl = _panel.CommandControls.AddButtonPopup(definitions, useLargeIcon, true);

                return new CommandControlWrapper(ctrl, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddButtonPopupToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl control = _panel.SlideoutControls.AddButtonPopup(definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddButtonToSlideout(IButtonDefinition buttonDef, RibbonType environment = RibbonType.All)
                {
                if (buttonDef == null)
                    { throw new ArgumentNullException(nameof(buttonDef)); }

                CommandControl control = _panel.SlideoutControls.AddButton(buttonDef.InventorButtonDefinition, true);
                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddComboBox(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All)
                {
                if (comboBoxDef == null)
                    { throw new ArgumentNullException(nameof(comboBoxDef)); }

                CommandControl control = _panel.CommandControls.AddComboBox(comboBoxDef.InventorComboBoxDefinition);
                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddComboBoxToSlideout(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All)
                {
                if (comboBoxDef == null)
                    { throw new ArgumentNullException(nameof(comboBoxDef)); }

                CommandControl control = _panel.SlideoutControls.AddComboBox(comboBoxDef.InventorComboBoxDefinition);
                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddGallery(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int width = 200, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl control = _panel.CommandControls.AddGallery(definitions, displayAsComboBox, width);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddGalleryToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int width = 200, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl control = _panel.SlideoutControls.AddGallery(definitions, displayAsComboBox, width);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddPopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.CommandControls.AddPopup(displayedControl.InventorButtonDefinition, definitions, useLargeIcon, true);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddPopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.SlideoutControls.AddPopup(displayedControl.InventorButtonDefinition, definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddSplitButton(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.CommandControls.AddSplitButton(displayedControl.InventorButtonDefinition, definitions, useLargeIcon, true);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddSplitButtonMRU(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl control = _panel.CommandControls.AddSplitButtonMRU(definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddSplitButtonMRUToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (buttonDefinitions == null)
                    { throw new ArgumentNullException(nameof(buttonDefinitions)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(buttonDefinitions);
                CommandControl control = _panel.SlideoutControls.AddSplitButtonMRU(definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddSplitButtonToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.SlideoutControls.AddSplitButton(displayedControl.InventorButtonDefinition, definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddTogglePopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.CommandControls.AddTogglePopup(displayedControl.InventorButtonDefinition, definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            /// <inheritdoc />
            public ICommandControl AddTogglePopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                if (displayedControl == null)
                    { throw new ArgumentNullException(nameof(displayedControl)); }

                if (additionalButtons == null)
                    { throw new ArgumentNullException(nameof(additionalButtons)); }

                ObjectCollection definitions = CreateButtonDefinitionCollection(additionalButtons);
                CommandControl control = _panel.SlideoutControls.AddTogglePopup(displayedControl.InventorButtonDefinition, definitions, useLargeIcon);

                return new CommandControlWrapper(control, environment);
                }

            #endregion Methods

            #region Private Methods

            private ObjectCollection CreateButtonDefinitionCollection(IEnumerable<IButtonDefinition> buttonDefinitions)
                {
                ObjectCollection definitions = _panel.Parent.Application.TransientObjects.CreateObjectCollection();

                foreach (IButtonDefinition buttonDefinition in buttonDefinitions)
                    {
                    if (buttonDefinition == null)
                        { throw new ArgumentException("The collection contains a null button definition.", nameof(buttonDefinitions)); }

                    definitions.Add(buttonDefinition.InventorButtonDefinition);
                    }
                return definitions;
                }

            #endregion Private Methods
            }

        /// <inheritdoc />
        public sealed class RibbonTabWrapper : IRibbonTab
            {
            #region Fields
            private readonly string _clientId;
            private readonly RibbonTab _tab;

            #endregion Fields

            #region Properties

            /// <inheritdoc />
            public string InternalName => _tab.InternalName;

            /// <inheritdoc />
            public string Name => _tab.DisplayName;

            /// <inheritdoc />
            public IEnumerable<IRibbonPanel> Panels
                {
                get
                    {
                    foreach (RibbonPanel panel in _tab.RibbonPanels)
                        { yield return new RibbonPanelWrapper(panel); }
                    }
                }

            #endregion Properties

            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="RibbonTabWrapper"/> class.
            /// </summary>
            /// <param name="tab">The underlying Inventor ribbon tab.</param>
            /// <param name="clientId">The add-in client identifier.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="tab"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when <paramref name="clientId"/> is empty or consists only of white-space characters.
            /// </exception>
            public RibbonTabWrapper(RibbonTab tab, string clientId)
                {
                _tab = tab ?? throw new ArgumentNullException(nameof(tab));

                if (string.IsNullOrWhiteSpace(clientId))
                    { throw new ArgumentException("The client identifier must not be null, empty, or white-space.", nameof(clientId)); }

                _clientId = clientId;
                }

            #endregion Constructor

            #region Methods

            /// <inheritdoc />
            public IRibbonPanel AddPanel(string displayName, string internalName)
                {
                if (string.IsNullOrWhiteSpace(displayName))
                    { throw new ArgumentException("The panel display name must not be null or empty.", nameof(displayName)); }

                if (string.IsNullOrWhiteSpace(internalName))
                    { throw new ArgumentException("The internal panel name must not be null or empty.", nameof(internalName)); }

                foreach (RibbonPanel panel in _tab.RibbonPanels)
                    {
                    if (string.Equals(panel.InternalName, internalName, StringComparison.Ordinal))
                        { return new RibbonPanelWrapper(panel); }
                    }

                RibbonPanel newPanel = _tab.RibbonPanels.Add(displayName, internalName, _clientId);
                return new RibbonPanelWrapper(newPanel);
                }

            #endregion Methods
            }

        #endregion Classes
        }
    }