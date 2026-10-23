using Inventor;
using InventorRibbonManager;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public static class IconHelper
        {
        #region Properties
        public static string Icon1_16x16 => GetIconPath(@"Properties\1-16x16.png");

        public static string Icon1_32x32 => GetIconPath(@"Properties\1-32x32.png");

        public static string Icon2_16x16 => GetIconPath(@"Properties\2-16x16.png");

        public static string Icon2_32x32 => GetIconPath(@"Properties\2-32x32.png");

        #endregion Properties

        #region Public Methods

        public static string GetIconPath(int nummer, int size)
            {
            var assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = $"{nummer}-{size}x{size}.png";
            return System.IO.Path.Combine(assemblyPath, "Properties", fileName);
            }

        #endregion Public Methods

        #region Private Methods

        private static string GetIconPath(string relativePath)
            {
            var assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return System.IO.Path.Combine(assemblyPath, relativePath);
            }

        #endregion Private Methods
        }

    [Guid("4B4088C4-EB94-442B-9D77-61DBB5E1781D")]
    public class StandardAddInServer : ApplicationAddInServer
        {
        #region Fields
        internal static string _guid;
        internal static bool addInLoaded = false;
        private Inventor.Application _inventorApp;

        #endregion Fields

        #region Properties
        public object Automation => null;

        #endregion Properties

        #region Public Methods

        public void Activate(ApplicationAddInSite addInSiteObject, bool firstTime)
            {
            _guid = ((GuidAttribute)System.Attribute.GetCustomAttribute(GetType(), typeof(GuidAttribute))).Value;
            _inventorApp = addInSiteObject.Application;

            try
                {
                InventorRibbonBuilder ribbonBuilder = new(_inventorApp, _guid);

                List<IPanelButton> buttons =
                    [
                new Button(_inventorApp),
                new ButtonPopup(_inventorApp),
                new ComboBox(_inventorApp),
                new Gallery(_inventorApp),
                new Popup(_inventorApp),
                new SplitButton(_inventorApp),
                new SplitButtonMRU(_inventorApp),
                new TogglePopup(_inventorApp),
                new SlideoutButton(_inventorApp),
                new SlideoutPopup(_inventorApp),
                new SlideoutComboBox(_inventorApp),
                new SlideoutGallery(_inventorApp),
                new SlideoutSplitButton(_inventorApp),
                new SlideoutSplitButtonMRU(_inventorApp),
                new SlideoutTogglePopup(_inventorApp),
                new SlideoutButtonPopup(_inventorApp)
                ];

                CreatePanelsAndButtonsForTab(ribbonBuilder, "Demo-Tab", "Demo-Tab.Internal", buttons);

                addInLoaded = true;
                }
            catch (Exception ex)
                {
                MessageBox.Show($"Error loading add-in: {ex.Message}\n{ex.StackTrace}", "Error");
                }
            }

        public void Deactivate()
            {
            addInLoaded = false;
            Marshal.ReleaseComObject(_inventorApp);
            }

        public void ExecuteCommand(int commandID)
            { }

        #endregion Public Methods
        }
    }