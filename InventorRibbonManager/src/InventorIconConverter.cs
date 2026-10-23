using System.Drawing;

namespace InventorRibbonManager
    {
    /// <summary>
    /// Contains the standard and large icons used by an Autodesk Inventor ribbon button.
    /// </summary>
    /// <remarks>
    /// This type groups both button icon variants into a single return value for
    /// convenience when creating ribbon controls.
    /// </remarks>
    public readonly struct InventorButtonIcons
        {
        #region Properties

        /// <summary>
        /// Gets the large icon.
        /// </summary>
        /// <value>
        /// A COM-compatible picture object for the large button icon,
        /// or <see langword="null"/> if no icon is available.
        /// </value>
        public object? LargeIcon { get; }

        /// <summary>
        /// Gets the standard-sized icon.
        /// </summary>
        /// <value>
        /// A COM-compatible picture object for the standard-sized button icon,
        /// or <see langword="null"/> if no icon is available.
        /// </value>
        public object? StandardIcon { get; }

        #endregion Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InventorButtonIcons"/> struct.
        /// </summary>
        /// <param name="standardIcon">
        /// The standard-sized icon.
        /// </param>
        /// <param name="largeIcon">
        /// The large icon.
        /// </param>
        public InventorButtonIcons(object? standardIcon, object? largeIcon)
            {
            StandardIcon = standardIcon;
            LargeIcon = largeIcon;
            }

        #endregion Public Constructors
        }

    /// <summary>
    /// Provides helper methods for converting .NET icons and bitmaps into
    /// Autodesk Inventor-compatible ribbon button icons.
    /// </summary>
    /// <remarks>
    /// The returned objects are intended for use with properties such as
    /// <c>StandardIcon</c> and <c>LargeIcon</c> on ribbon button definitions.
    /// </remarks>
    public static class InventorIconConverter
        {
        #region Public Methods

        /// <summary>
        /// Converts a <see cref="Bitmap"/> into an Inventor-compatible icon object.
        /// </summary>
        /// <param name="bitmap">
        /// The source bitmap.
        /// </param>
        /// <returns>
        /// A COM-compatible picture object for use with Inventor ribbon controls,
        /// or <see langword="null"/> if <paramref name="bitmap"/> is <see langword="null"/>.
        /// </returns>
        public static object? FromBitmap(Bitmap? bitmap)
            {
            return bitmap == null ? null : PictureDispConverter.ToIPictureDisp(bitmap);
            }

        /// <summary>
        /// Converts the standard and large button icons from bitmap instances.
        /// </summary>
        /// <param name="standardIcon">
        /// The bitmap for the standard-sized icon, typically a 16x16 image.
        /// </param>
        /// <param name="largeIcon">
        /// The bitmap for the large icon, typically a 32x32 image.
        /// </param>
        /// <returns>
        /// An <see cref="InventorButtonIcons"/> value containing the converted
        /// standard and large icons. Each icon is <see langword="null"/> if the
        /// corresponding bitmap is <see langword="null"/>.
        /// </returns>
        public static InventorButtonIcons FromBitmaps(Bitmap? standardIcon, Bitmap? largeIcon)
            {
            return new InventorButtonIcons(FromBitmap(standardIcon), FromBitmap(largeIcon));
            }

        /// <summary>
        /// Loads an image from a file and converts it into an Inventor-compatible icon object.
        /// </summary>
        /// <param name="path">
        /// The absolute or relative file path to the image.
        /// </param>
        /// <returns>
        /// A COM-compatible picture object for use with Inventor ribbon controls,
        /// or <see langword="null"/> if <paramref name="path"/> is <see langword="null"/>,
        /// empty, whitespace, invalid, or the file cannot be loaded.
        /// </returns>
        public static object? FromFile(string? path)
            {
            if (string.IsNullOrWhiteSpace(path))
                { return null; }

            return PictureDispConverter.GetFromPath(path!);
            }

        /// <summary>
        /// Loads the standard and large button icons from image files.
        /// </summary>
        /// <param name="standardIconPath">
        /// The file path to the standard-sized icon, typically a 16x16 image.
        /// </param>
        /// <param name="largeIconPath">
        /// The file path to the large icon, typically a 32x32 image.
        /// </param>
        /// <returns>
        /// An <see cref="InventorButtonIcons"/> value containing the converted
        /// standard and large icons. Each icon is <see langword="null"/> if the
        /// corresponding file path is <see langword="null"/>, empty, whitespace,
        /// invalid, or the file cannot be loaded.
        /// </returns>
        public static InventorButtonIcons FromFiles(string? standardIconPath, string? largeIconPath)
            {
            return new InventorButtonIcons(FromFile(standardIconPath), FromFile(largeIconPath));
            }

        /// <summary>
        /// Converts an <see cref="Icon"/> into an Inventor-compatible icon object.
        /// </summary>
        /// <param name="icon">
        /// The source icon.
        /// </param>
        /// <returns>
        /// A COM-compatible picture object for use with Inventor ribbon controls,
        /// or <see langword="null"/> if <paramref name="icon"/> is <see langword="null"/>.
        /// </returns>
        public static object? FromIcon(Icon? icon)
            {
            return icon == null ? null : PictureDispConverter.ToIPictureDisp(icon);
            }

        /// <summary>
        /// Converts the standard and large button icons from <see cref="Icon"/> instances.
        /// </summary>
        /// <param name="standardIcon">
        /// The icon for the standard-sized button image, typically a 16x16 icon.
        /// </param>
        /// <param name="largeIcon">
        /// The icon for the large button image, typically a 32x32 icon.
        /// </param>
        /// <returns>
        /// An <see cref="InventorButtonIcons"/> value containing the converted
        /// standard and large icons. Each icon is <see langword="null"/> if the
        /// corresponding icon is <see langword="null"/>.
        /// </returns>
        public static InventorButtonIcons FromIcons(Icon? standardIcon, Icon? largeIcon)
            {
            return new InventorButtonIcons(FromIcon(standardIcon), FromIcon(largeIcon));
            }

        #endregion Public Methods
        }
    }