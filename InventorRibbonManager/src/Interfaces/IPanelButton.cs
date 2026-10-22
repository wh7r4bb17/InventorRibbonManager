namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines a button that can be added to a ribbon panel.
    /// </summary>
    /// <remarks>
    /// This interface extends <see cref="IButtonDefinition"/> with panel placement information
    /// and ribbon environment targeting.
    /// </remarks>
    public interface IPanelButton : IButtonDefinition
        {
        #region Properties

        /// <summary>
        /// Gets the ribbon environments in which the button should appear.
        /// </summary>
        RibbonType Environment { get; }

        /// <summary>
        /// Gets the internal name of the target ribbon panel.
        /// </summary>
        string PanelInternalName { get; }

        /// <summary>
        /// Gets the display name of the target ribbon panel.
        /// </summary>
        string PanelName { get; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Adds the button to the specified ribbon panel for the given ribbon environment.
        /// </summary>
        /// <param name="panel">The ribbon panel to which the button should be added.</param>
        /// <param name="env">The concrete ribbon environment currently being built.</param>
        void AddToPanel(IRibbonPanel panel, RibbonType env);

        #endregion Public Methods
        }
    }