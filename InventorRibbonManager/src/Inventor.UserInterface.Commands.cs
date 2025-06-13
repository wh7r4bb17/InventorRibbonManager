using Inventor;

namespace InventorRibbonManager
    {
    public interface IButtonDefinition
        {
        string DisplayName { get; }
        string InternalName { get; }
        object StandardIcon { get; }
        object LargeIcon { get; }
        void Execute();
        ButtonDefinition InventorButtonDefinition { get; }
        bool Pressed { get; set; }
        }

    public interface IPanelButton : IButtonDefinition
        {
        string PanelName { get; }
        string PanelInternalName { get; }
        RibbonType Environment { get; }
        void AddToPanel(IRibbonPanel panel, RibbonType env);
        }

    public interface IComboBoxDefinition
        {
        string InternalName { get; }
        ComboBoxDefinition InventorComboBoxDefinition { get; }
        }

    public interface ICommandControl
        {
        string InternalName { get; }
        object InventorControl { get; }
        RibbonType Environment { get; }
        }
    }