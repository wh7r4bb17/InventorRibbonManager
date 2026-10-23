using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class Gallery : IPanelButton
        {
        #region Fields
        private List<IButtonDefinition> _galleryButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "Gallery";
        public RibbonType Environment => RibbonType.All;
        public string InternalName => "Gallery.Internal";
        public ButtonDefinition InventorButtonDefinition => null;
        public object LargeIcon => null;
        public string PanelInternalName => "Gallery_Internal";
        public string PanelName => "Gallery";
        public bool Pressed { get; set; }
        public object StandardIcon => null;

        #endregion Properties

        #region Constructor

        public Gallery(Application app)
            {
            _galleryButtons = [.. Enumerable.Range(1, 5)
                .Select(i =>
            {
                string Icon32x32 = IconHelper.GetIconPath(i, 32);
                object _icon32 = InventorIconConverter.FromFile(Icon32x32);
                var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                    DisplayName: $"Gallery Item {i}",
                    InternalName: $"GalleryItem{i}.Internal",
                    Classification: CommandTypesEnum.kShapeEditCmdType,
                    ClientId: StandardAddInServer._guid,
                    DescriptionText: $"Item {i}",
                    ToolTipText: $"Gallery Item {i}",
                    StandardIcon: null,
                    LargeIcon: _icon32,
                    ButtonDisplay: ButtonDisplayEnum.kNoTextWithIcon);

                btn.OnExecute += (ctx) => System.Windows.Forms.MessageBox.Show($"Gallery Item {i} clicked!");

                return new CustomButtonDefinition(btn);
            })
                .Cast<IButtonDefinition>()];
            }

        #endregion Constructor

        #region Methods

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddGallery(_galleryButtons, false, 300, env);
            }

        public void Execute()
            { }

        #endregion Methods
        }

    public class SlideoutGallery : IPanelButton
        {
        #region Fields
        private List<IButtonDefinition> _galleryButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutGallery";
        public RibbonType Environment => RibbonType.All;
        public string InternalName => "SlideoutGallery.Internal";
        public ButtonDefinition InventorButtonDefinition => null;
        public object LargeIcon => null;
        public string PanelInternalName => "Gallery_Internal";
        public string PanelName => "Gallery";
        public bool Pressed { get; set; }
        public object StandardIcon => null;

        #endregion Properties

        #region Constructor

        public SlideoutGallery(Application app)
            {
            _galleryButtons = [.. Enumerable.Range(1, 3)
                .Select(i =>
            {
                string Icon32x32 = IconHelper.GetIconPath(i, 32);
                object _icon32 = InventorIconConverter.FromFile(Icon32x32);
                var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                    DisplayName: $"Gallery {i}",
                    InternalName: $"Gallery{i}.Internal",
                    Classification: CommandTypesEnum.kShapeEditCmdType,
                    ClientId: StandardAddInServer._guid,
                    DescriptionText: $"Gallery Item {i}",
                    ToolTipText: $"Gallery {i}",
                    StandardIcon: null,
                    LargeIcon: _icon32,
                    ButtonDisplay: ButtonDisplayEnum.kNoTextWithIcon);

                btn.OnExecute += (ctx) => System.Windows.Forms.MessageBox.Show($"Gallery {i} clicked!");

                return new CustomButtonDefinition(btn);
            })
                .Cast<IButtonDefinition>()];
            }

        #endregion Constructor

        #region Methods

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddGalleryToSlideout(_galleryButtons, true, 300, env);
            }

        public void Execute()
            { }

        #endregion Methods
        }
    }