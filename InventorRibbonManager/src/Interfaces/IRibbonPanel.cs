using System.Collections.Generic;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines an abstraction for an Inventor ribbon panel.
    /// </summary>
    /// <remarks>
    /// A ribbon panel can host standard controls and slideout controls, such as buttons,
    /// combo boxes, galleries, popups, split buttons, and toggle popups.
    /// </remarks>
    public interface IRibbonPanel
        {
        #region Properties

        /// <summary>
        /// Gets the internal name that uniquely identifies the ribbon panel.
        /// </summary>
        string InternalName { get; }

        /// <summary>
        /// Gets the display name of the ribbon panel.
        /// </summary>
        string Name { get; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Adds a standard button control to the ribbon panel.
        /// </summary>
        /// <param name="buttonDef">The button definition to add.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddButton(IButtonDefinition buttonDef, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a button popup control to the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddButtonPopup(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a button popup control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddButtonPopupToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a standard button control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinition">The button definition to add.</param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddButtonToSlideout(IButtonDefinition buttonDefinition, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a combo box control to the ribbon panel.
        /// </summary>
        /// <param name="comboBoxDef">The combo box definition to add.</param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddComboBox(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a combo box control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="comboBoxDef">The combo box definition to add.</param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddComboBoxToSlideout(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a gallery control to the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the gallery.</param>
        /// <param name="displayAsComboBox">
        /// <see langword="true"/> to display the gallery as a combo box; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="width">The width of the gallery in pixels.</param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddGallery(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int width = 200, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a gallery control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the gallery.</param>
        /// <param name="displayAsComboBox">
        /// <see langword="true"/> to display the gallery as a combo box; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="width">The width of the gallery in pixels.</param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddGalleryToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int width = 200, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a popup control to the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown within the popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddPopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a popup control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown within the popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddPopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a split button control to the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown in the split button dropdown.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddSplitButton(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a most-recently-used split button control to the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the MRU split button.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddSplitButtonMRU(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a most-recently-used split button control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="buttonDefinitions">The button definitions that populate the MRU split button.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddSplitButtonMRUToSlideout(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a split button control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown in the split button dropdown.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddSplitButtonToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a toggle popup control to the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown within the toggle popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddTogglePopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        /// <summary>
        /// Adds a toggle popup control to the slideout area of the ribbon panel.
        /// </summary>
        /// <param name="displayedControl">The button definition shown as the primary visible control.</param>
        /// <param name="additionalButtons">The additional button definitions shown within the toggle popup.</param>
        /// <param name="useLargeIcon">
        /// <see langword="true"/> to use the large icon representation; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="environment">
        /// The ribbon environment for which the control is created. The default is <see cref="RibbonType.All"/>.
        /// </param>
        /// <returns>
        /// The created command control.
        /// </returns>
        ICommandControl AddTogglePopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);

        #endregion Public Methods
        }
    }