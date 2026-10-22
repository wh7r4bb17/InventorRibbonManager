using Inventor;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines a wrapper abstraction for an Inventor combo box definition.
    /// </summary>
    /// <remarks>
    /// The wrapped <see cref="ComboBoxDefinition"/> represents a command whose user interface
    /// is displayed as a combo box in Inventor.
    /// </remarks>
    public interface IComboBoxDefinition
        {
        #region Properties

        /// <summary>
        /// Gets the internal name that uniquely identifies the combo box definition.
        /// </summary>
        string InternalName { get; }

        /// <summary>
        /// Gets the underlying Autodesk Inventor <see cref="ComboBoxDefinition"/> instance.
        /// </summary>
        ComboBoxDefinition InventorComboBoxDefinition { get; }

        #endregion Properties
        }
    }