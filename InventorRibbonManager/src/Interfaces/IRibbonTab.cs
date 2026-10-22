using System.Collections.Generic;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines an abstraction for an Inventor ribbon tab.
    /// </summary>
    /// <remarks>
    /// A ribbon tab groups one or more ribbon panels within a specific Inventor ribbon environment.
    /// </remarks>
    public interface IRibbonTab
        {
        #region Properties

        /// <summary>
        /// Gets the internal name that uniquely identifies the ribbon tab.
        /// </summary>
        string InternalName { get; }

        /// <summary>
        /// Gets the display name of the ribbon tab.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the ribbon panels that belong to this ribbon tab.
        /// </summary>
        IEnumerable<IRibbonPanel> Panels { get; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Creates a ribbon panel if it does not already exist, or returns the existing panel.
        /// </summary>
        /// <param name="displayName">The display name of the panel.</param>
        /// <param name="internalName">The internal name of the panel.</param>
        /// <returns>
        /// The created or existing ribbon panel.
        /// </returns>
        IRibbonPanel AddPanel(string displayName, string internalName);

        #endregion Public Methods
        }
    }