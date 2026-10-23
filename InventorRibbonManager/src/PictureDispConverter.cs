using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InventorRibbonManager
    {
    internal sealed class PictureDispConverter
        {
        #region Fields
        private static Guid _iPictureDispGuid = typeof(stdole.IPictureDisp).GUID;

        [DllImport("OleAut32.dll", EntryPoint = "OleCreatePictureIndirect", ExactSpelling = true, PreserveSig = false)]
        private static extern stdole.IPictureDisp OleCreatePictureIndirect([MarshalAs(UnmanagedType.AsAny)] object picdesc, ref Guid iid, [MarshalAs(UnmanagedType.Bool)] bool fOwn);

        #endregion Fields

        #region Internal Methods

        internal static stdole.IPictureDisp? GetFromPath(string path)
            {
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                { return null; }

            try
                {
                using var bitmap = new Bitmap(path);
                return ToIPictureDisp(bitmap);
                }
            catch
                {
                return null;
                }
            }

        internal static stdole.IPictureDisp ToIPictureDisp(System.Drawing.Icon icon)
            {
            return _ToIPictureDisp(icon);
            }

        internal static stdole.IPictureDisp ToIPictureDisp(System.Drawing.Bitmap bmp)
            {
            return _ToIPictureDisp(bmp);
            }

        #endregion Internal Methods

        #region private Methods

        private static stdole.IPictureDisp _ToIPictureDisp(System.Drawing.Icon icon)
            {
            PICTDESC.Icon pictIcon = new PICTDESC.Icon(icon);
            return OleCreatePictureIndirect(pictIcon, ref _iPictureDispGuid, true);
            }

        private static stdole.IPictureDisp _ToIPictureDisp(System.Drawing.Bitmap bmp)
            {
            PICTDESC.Bitmap pictBmp = new PICTDESC.Bitmap(bmp);
            return OleCreatePictureIndirect(pictBmp, ref _iPictureDispGuid, true);
            }

        #endregion private Methods

        #region Classes

        private static class PICTDESC
            {
            #region Fields
            internal const short PICTYPE_BITMAP = 1;
            internal const short PICTYPE_ICON = 3;

            #endregion Fields

            #region Classes

            [StructLayout(LayoutKind.Sequential)]
            public class Bitmap
                {
                internal int cbSizeOfStruct = Marshal.SizeOf(typeof(PICTDESC.Bitmap));
                internal int picType = PICTDESC.PICTYPE_BITMAP;
                internal IntPtr hbitmap = IntPtr.Zero;
                internal IntPtr hpal = IntPtr.Zero;
                internal int unused;

                internal Bitmap(System.Drawing.Bitmap bitmap)
                    {
                    this.hbitmap = bitmap.GetHbitmap();
                    }
                }

            [StructLayout(LayoutKind.Sequential)]
            public class Icon
                {
                internal int cbSizeOfStruct = Marshal.SizeOf(typeof(PICTDESC.Icon));
                internal int picType = PICTDESC.PICTYPE_ICON;
                internal IntPtr hicon = IntPtr.Zero;
                internal int unused1;
                internal int unused2;

                internal Icon(System.Drawing.Icon icon)
                    {
                    this.hicon = icon.ToBitmap().GetHicon();
                    }
                }

            #endregion Classes
            }

        #endregion Classes
        }
    }