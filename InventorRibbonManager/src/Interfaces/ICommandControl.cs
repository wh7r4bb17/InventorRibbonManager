namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines a wrapper abstraction for a ribbon command control.
    /// </summary>
    /// <remarks>
    /// Implementations represent concrete controls that were added to the Inventor ribbon UI,
    /// together with their associated ribbon environment.
    /// </remarks>
    public interface ICommandControl
        {
        #region Properties

        /// <summary>
        /// Gets the ribbon environment in which the control exists.
        /// </summary>
        RibbonType Environment { get; }

        /// <summary>
        /// Gets the internal name that uniquely identifies the command control.
        /// </summary>
        string InternalName { get; }

        /// <summary>
        /// Gets the underlying Inventor control object.
        /// </summary>
        /// <value>
        /// The concrete Autodesk Inventor control instance, such as a button control,
        /// combo box control, or another supported ribbon control type.
        /// </value>
        object InventorControl { get; }

        #endregion Properties
        }
    }