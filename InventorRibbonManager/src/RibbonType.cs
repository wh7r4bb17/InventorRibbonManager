using System;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Defines the Inventor ribbon environments supported by the library.
    /// </summary>
    [Flags]
    public enum RibbonType
        {
        /// <summary>No ribbon environment.</summary>
        None = 0,

        /// <summary>The zero-document environment.</summary>
        ZeroDoc = 1 << 0,

        /// <summary>The part document environment.</summary>
        Part = 1 << 1,

        /// <summary>The assembly document environment.</summary>
        Assembly = 1 << 2,

        /// <summary>The drawing document environment.</summary>
        Drawing = 1 << 3,

        /// <summary>The presentation document environment.</summary>
        Presentation = 1 << 4,

        /// <summary>All supported ribbon environments.</summary>
        All = ZeroDoc | Part | Assembly | Drawing | Presentation
        }
    }