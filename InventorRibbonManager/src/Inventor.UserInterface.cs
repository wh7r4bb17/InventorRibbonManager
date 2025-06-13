using System.Collections.Generic;

namespace InventorRibbonManager
    {
    public interface IRibbonTab
        {
        string Name { get; }
        string InternalName { get; }
        IEnumerable<IRibbonPanel> Panels { get; }
        IRibbonPanel AddPanel(string displayName, string internalName);
        }

    public interface IRibbonPanel
        {
        string Name { get; }
        string InternalName { get; }
        ICommandControl AddButton(IButtonDefinition buttonDef, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddButtonPopup(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddComboBox(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All);
        ICommandControl AddGallery(IEnumerable<IButtonDefinition> buttonDefinitions, bool displayAsComboBox, int Width = 200, RibbonType environment = RibbonType.All);
        ICommandControl AddPopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddSplitButton(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddSplitButtonMRU(IEnumerable<IButtonDefinition> buttonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddTogglePopup(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddButtonToSlideout(IButtonDefinition buttonDef, RibbonType environment = RibbonType.All);
        ICommandControl AddButtonPopupToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddComboBoxToSlideout(IComboBoxDefinition comboBoxDef, RibbonType environment = RibbonType.All);
        ICommandControl AddGalleryToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool displayAsComboBox, int Width = 200, RibbonType environment = RibbonType.All);
        ICommandControl AddPopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddSplitButtonToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddSplitButtonMRUToSlideout(IEnumerable<IButtonDefinition> ButtonDefinitions, bool useLargeIcon, RibbonType environment = RibbonType.All);
        ICommandControl AddTogglePopupToSlideout(IButtonDefinition displayedControl, IEnumerable<IButtonDefinition> additionalButtons, bool useLargeIcon, RibbonType environment = RibbonType.All);
        }
    }