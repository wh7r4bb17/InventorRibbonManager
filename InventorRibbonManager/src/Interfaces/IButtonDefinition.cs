using Inventor;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines a wrapper abstraction for an Inventor button definition.
    /// </summary>
    /// <remarks>
    /// Implementations expose both the logical button metadata used by the ribbon manager
    /// and the underlying Autodesk Inventor <see cref="ButtonDefinition"/> instance.
    /// </remarks>
    public interface IButtonDefinition
        {
        #region Properties

        /// <summary>
        /// Gets the display name of the button as shown in the Inventor user interface.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the internal name that uniquely identifies the button definition.
        /// </summary>
        string InternalName { get; }

        /// <summary>
        /// Gets the underlying Autodesk Inventor <see cref="ButtonDefinition"/> instance.
        /// </summary>
        ButtonDefinition InventorButtonDefinition { get; }

        /// <summary>
        /// Gets the large icon for the button.
        /// </summary>
        /// <value>
        /// An object understood by the Inventor API as the large icon representation.
        /// </value>
        object? LargeIcon { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the button is currently in the pressed state.
        /// </summary>
        bool Pressed { get; set; }

        /// <summary>
        /// Gets the standard-sized icon for the button.
        /// </summary>
        /// <value>
        /// An object understood by the Inventor API as the standard icon representation.
        /// </value>
        object? StandardIcon { get; }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Executes the logical action associated with the button.
        /// </summary>
        void Execute();

        #endregion Public Methods
        }
    }