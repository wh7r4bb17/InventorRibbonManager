using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;

//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-RibbonTab
//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-RibbonPanel
//https://help.autodesk.com/view/INVNTOR/2025/ENU/?guid=GUID-CommandControls


namespace InventorRibbonManager
    {
    public class InventorRibbonBuilder : IDisposable
        {
        #region Fields
        private Inventor.Application _app;
        internal static string _guid;

        #endregion Fields

        #region Constructor
        public InventorRibbonBuilder(Inventor.Application app, string guid)
            {
            _app = app;
            _guid = guid;
            }

        #endregion Constructor

        #region Dispose

        public void Dispose()
            {
            _app = null;
            }

        #endregion Dispose

        #region Methods
        public IRibbonTab CreateOrGetTab(RibbonType ribbonType, string name, string internalName)
            {
            Ribbon ribbon = GetRibbon(ribbonType);
            RibbonTab tab = null;

            foreach (RibbonTab _tab in ribbon.RibbonTabs)
                {
                if (_tab.InternalName == internalName)
                    {
                    tab = _tab;
                    break;
                    }
                }

            if (tab == null)
                { tab = ribbon.RibbonTabs.Add(name, internalName, _guid); }

            return new RibbonTabWrapper(tab);
            }

        public void RegisterPanelForRibbons(
            string tabName,
            string tabInternalName,
            string panelDisplayName,
            string panelInternalName,
            RibbonType ribbonTypes,
            Action<IRibbonPanel, RibbonType> configurePanel)
            {
            foreach (RibbonType type in (RibbonType[])Enum.GetValues(typeof(RibbonType)))
                {
                if (type == RibbonType.None || !ribbonTypes.HasFlag(type) || type == RibbonType.All)
                    { continue; }

                var tab = CreateOrGetTab(type, tabName, tabInternalName);
                var panel = tab.AddPanel(panelDisplayName, panelInternalName);
                configurePanel?.Invoke(panel, type);
                }
            }

        public void RegisterPanelForRibbons(
            string tabName,
            string tabInternalName,
            string panelDisplayName,
            string panelInternalName,
            Action<IRibbonPanel, RibbonType> configurePanel)
            {
            foreach (RibbonType type in (RibbonType[])Enum.GetValues(typeof(RibbonType)))
                {
                if (type == RibbonType.None || type == RibbonType.All)
                    { continue; }

                var tab = CreateOrGetTab(type, tabName, tabInternalName);
                var panel = tab.AddPanel(panelDisplayName, panelInternalName);
                configurePanel?.Invoke(panel, type);
                }
            }

        public static void CreatePanelsAndButtonsForTab(InventorRibbonBuilder ribbonBuilder, string tabName, string tabInternalName, IEnumerable<IPanelButton> buttons)
            {
            var registered = new HashSet<(string ButtonInternalName, RibbonType Env)>();
            RibbonType[] allEnvs = Enum.GetValues(typeof(RibbonType))
                .Cast<RibbonType>()
                .Where(e => e != RibbonType.None && e != RibbonType.All)
                .ToArray();

            foreach (RibbonType env in allEnvs)
                {
                List<IPanelButton> buttonsForEnv = buttons
                    .Where(btn => (btn.Environment & env) != 0
                                 && !registered.Contains((btn.InternalName, env)))
                    .ToList();
                foreach (var panelGroup in buttonsForEnv.GroupBy(b => (b.PanelName, b.PanelInternalName)))
                    {
                    ribbonBuilder.RegisterPanelForRibbons(
                        tabName,
                        tabInternalName,
                        panelGroup.Key.PanelName,
                        panelGroup.Key.PanelInternalName,
                        (panel, currentEnv) =>
                        {
                            if (currentEnv != env)
                                { return; }
                            foreach (IPanelButton btn in panelGroup)
                                {
                                if (!registered.Add((btn.InternalName, env)))
                                    { continue; }
                                btn.AddToPanel(panel, env);
                                }
                        }
                    );
                    }
                }

            foreach (RibbonType env in new[] { RibbonType.All })
                {
                List<IPanelButton> buttonsForEnv = buttons
                    .Where(btn =>
                        btn.Environment == RibbonType.All &&
                        !allEnvs.Any(singleEnv => (btn.Environment & singleEnv) != 0 &&
                        registered.Contains((btn.InternalName, singleEnv))) &&
                        !registered.Contains((btn.InternalName, env))
                    )
                    .ToList();
                foreach (var panelGroup in buttonsForEnv.GroupBy(b => (b.PanelName, b.PanelInternalName)))
                    {
                    ribbonBuilder.RegisterPanelForRibbons(
                        tabName,
                        tabInternalName,
                        panelGroup.Key.PanelName,
                        panelGroup.Key.PanelInternalName,
                        (panel, currentEnv) =>
                        {
                            if (currentEnv != env)
                                { return; }
                            foreach (IPanelButton btn in panelGroup)
                                {
                                if (!registered.Add((btn.InternalName, env)))
                                    { continue; }
                                btn.AddToPanel(panel, env);
                                }
                        }
                    );
                    }
                }
            }

        #endregion Methods

        #region private Methods
        private Ribbon GetRibbon(RibbonType ribbonType)
            {
            Ribbons ribbons = _app.UserInterfaceManager.Ribbons;
            string ribbonName = GetRibbonNameFromRibbonType(ribbonType);
            return ribbons[ribbonName];
            }

        private string GetRibbonNameFromRibbonType(RibbonType ribbonType)
            {
            if (ribbonType.HasFlag(RibbonType.Part))
                { return "Part"; }
            if (ribbonType.HasFlag(RibbonType.Assembly))
                { return "Assembly"; }
            if (ribbonType.HasFlag(RibbonType.Drawing))
                { return "Drawing"; }
            if (ribbonType.HasFlag(RibbonType.Presentation))
                { return "Presentation"; }
            return "ZeroDoc";
            }

        #endregion private Methods

        public class RibbonTabWrapper : IRibbonTab
            {
            #region Fields
            private readonly RibbonTab _tab;

            #endregion Fields

            #region Properties
            public string Name => _tab.DisplayName;

            public string InternalName => _tab.InternalName;

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
            public RibbonTabWrapper(RibbonTab tab)
                {
                _tab = tab;
                }

            #endregion Constructor

            #region Methods
            public IRibbonPanel AddPanel(string displayName, string internalName)
                {
                foreach (RibbonPanel panel in _tab.RibbonPanels)
                    {
                    if (panel.InternalName == internalName)
                        { return new RibbonPanelWrapper(panel); }
                    }

                RibbonPanel newPanel = _tab.RibbonPanels.Add(displayName, internalName, InventorRibbonBuilder._guid);
                return new RibbonPanelWrapper(newPanel);
                }

            #endregion Methods
            }

        public class RibbonPanelWrapper : IRibbonPanel
            {
            #region Fields
            private readonly RibbonPanel _panel;

            #endregion Fields

            #region Properties
            public string Name => _panel.DisplayName;
            public string InternalName => _panel.InternalName;

            #endregion Properties

            #region Constructor
            public RibbonPanelWrapper(RibbonPanel panel)
                {
                _panel = panel;
                }

            #endregion Constructor

            #region Methods
            public ICommandControl AddButton(IButtonDefinition buttonDef, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                try
                    {
                    if (_panel.CommandControls[buttonDef.InternalName] is CommandControl existing)
                        {
                        return new CommandControlWrapper(existing, environment);
                        }
                    }
                catch (System.Runtime.InteropServices.COMException)
                    {
                    }
                catch (ArgumentException Argex)
                    {
                    }
                catch (Exception ex)
                    {
                    }

                try
                    {
                    CommandControl newCtrl = _panel.CommandControls.AddButton(buttonDef.InventorButtonDefinition, useLargeIcon);
                    return new CommandControlWrapper(newCtrl, environment);
                    }
                catch (Exception ex)
                    {
                    return null;
                    }

                }

            public ICommandControl AddButtonToSlideout(IButtonDefinition buttonDef, RibbonType environment = RibbonType.All)
                {
                CommandControl ctrl = _panel.SlideoutControls.AddButton(buttonDef.InventorButtonDefinition, true);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddButtonPopup(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in ButtonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddButtonPopup(defs, useLargeIcon, true);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddButtonPopupToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in ButtonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddButtonPopup(defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddComboBox(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All)
                {
                CommandControl ctrl = _panel.CommandControls.AddComboBox(comboBoxDef.InventorComboBoxDefinition);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddComboBoxToSlideout(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All)
                {
                CommandControl ctrl = _panel.SlideoutControls.AddComboBox(comboBoxDef.InventorComboBoxDefinition);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddGallery(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int Width = 200, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in buttonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddGallery(defs, displayAsComboBox, Width);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddGalleryToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool displayAsComboBox, int Width = 200, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in ButtonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddGallery(defs, displayAsComboBox, Width);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddPopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                if (additionalButtons == null)
                    { return null; }
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddPopup(displayedControl.InventorButtonDefinition, defs, useLargeIcon, true);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddPopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddPopup(displayedControl.InventorButtonDefinition, defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddSplitButton(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddSplitButton(displayedControl.InventorButtonDefinition, defs, useLargeIcon, true);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddSplitButtonToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddSplitButton(displayedControl.InventorButtonDefinition, defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddSplitButtonMRU(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in buttonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddSplitButtonMRU(defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddSplitButtonMRUToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in ButtonDefinitions)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddSplitButtonMRU(defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddTogglePopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.CommandControls.AddTogglePopup(displayedControl.InventorButtonDefinition, defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            public ICommandControl AddTogglePopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All)
                {
                ObjectCollection defs = _panel.Parent.Application.TransientObjects.CreateObjectCollection();
                foreach (IButtonDefinition def in additionalButtons)
                    {
                    defs.Add(def.InventorButtonDefinition);
                    }
                CommandControl ctrl = _panel.SlideoutControls.AddTogglePopup(displayedControl.InventorButtonDefinition, defs, useLargeIcon);
                return new CommandControlWrapper(ctrl, environment);
                }

            #endregion Methods
            }

        public class CommandControlWrapper : ICommandControl
            {
            #region Fields
            private readonly CommandControl _control;

            #endregion Fields

            #region Properties
            public RibbonType Environment { get; }
            public string InternalName => _control.InternalName;
            public object InventorControl => _control;
            #endregion Properties

            #region Constructor
            public CommandControlWrapper(CommandControl control, RibbonType environment)
                {
                _control = control;
                Environment = environment;
                }

            #endregion Constructor
            }

        public class CustomButtonDefinition : IButtonDefinition, IDisposable
            {
            #region Fields
            private readonly Action _executeAction;

            #endregion Fields

            #region Properties
            public string DisplayName => InventorButtonDefinition.DisplayName;
            public string InternalName => InventorButtonDefinition.InternalName;
            public object StandardIcon { get; }
            public object LargeIcon { get; }
            public ButtonDefinition InventorButtonDefinition { get; }
            public RibbonType Environment { get; }
            public bool Pressed { get; set; }

            #endregion Properties

            #region Constructor
            public CustomButtonDefinition(ButtonDefinition definition, RibbonType environment = RibbonType.All)
                {
                InventorButtonDefinition = definition;
                Environment = environment;
                InventorButtonDefinition.OnExecute += OnExecute;
                }

            public CustomButtonDefinition(ButtonDefinition definition, Action onExecute, RibbonType environment = RibbonType.All)
                {
                InventorButtonDefinition = definition;
                _executeAction = onExecute;
                Environment = environment;
                InventorButtonDefinition.OnExecute += OnExecute;
                }

            #endregion Constructor

            #region Dispose
            public void Dispose()
                {
                if (InventorButtonDefinition != null)
                    { InventorButtonDefinition.OnExecute -= OnExecute; }
                }

            #endregion Dispose

            #region Methods
            public void Execute()
                {
                _executeAction?.Invoke();
                }

            #endregion Methods

            #region private Methods
            private void OnExecute(NameValueMap Context)
                {
                _executeAction?.Invoke();
                }

            #endregion private Methods
            }

        public class ComboBoxDefinitionWrapper : IComboBoxDefinition
            {
            #region Properties
            public string InternalName => InventorComboBoxDefinition.InternalName;
            public ComboBoxDefinition InventorComboBoxDefinition { get; }

            #endregion Properties

            #region Constructor
            public ComboBoxDefinitionWrapper(ComboBoxDefinition comboBoxDefinition)
                {
                InventorComboBoxDefinition = comboBoxDefinition;
                }

            #endregion Constructor
            }
        }
    }