using System;

namespace InventorRibbonManager
    {
    [Flags]
    public enum RibbonType
        {
        None = 0,
        ZeroDoc = 1 << 0,
        Part = 1 << 1,
        Assembly = 1 << 2,
        Drawing = 1 << 3,
        Presentation = 1 << 4,
        All = ZeroDoc | Part | Assembly | Drawing | Presentation
        }
    }