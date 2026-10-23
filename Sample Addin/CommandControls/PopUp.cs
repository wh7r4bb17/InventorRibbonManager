using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class Popup : IPanelButton
        {
        #region Fields
        private readonly object _icon16;
        private readonly object _icon32;
        private readonly IButtonDefinition _mainButton;
        private readonly List<IButtonDefinition> _otherButtons;
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;

        #endregion Fields

        #region Properties
        public string DisplayName => "Popup";
        public RibbonType Environment => RibbonType.All;
        public string InternalName => "Popup.Internal";
        public ButtonDefinition InventorButtonDefinition => null;
        public object LargeIcon => _icon32;
        public string PanelInternalName => "Popup_Internal";
        public string PanelName => "Popup";
        public bool Pressed { get; set; }
        public object StandardIcon => _icon16;

        #endregion Properties

        #region Constructor

        public Popup(Application app)
            {
            _icon16 = InventorIconConverter.FromFile(Icon16x16);
            _icon32 = InventorIconConverter.FromFile(Icon32x32);

            ButtonDefinition MainBtn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Visible in all Environments",
                ToolTipText: "Popup Main",
                StandardIcon: _icon16,
                LargeIcon: _icon32,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            _mainButton = new CustomButtonDefinition(MainBtn);
            _otherButtons = [.. Enumerable.Range(1, 3)
                .Select(i =>
                {
                    string Icon32x32 = IconHelper.GetIconPath(i, 32);
                    object _icon32 = InventorIconConverter.FromFile(Icon32x32);
                    var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                        DisplayName: $"Popup Option {i}",
                                        InternalName: $"Popup.Option{i}.Internal",
                                        Classification: CommandTypesEnum.kShapeEditCmdType,
                                        ClientId: StandardAddInServer._guid,
                                        DescriptionText: $"Popup Option button {i}",
                                        ToolTipText: $"Popup Option {i}",
                                        StandardIcon: null,
                                        LargeIcon: _icon32,
                                        ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                    btn.OnExecute += (ctx) =>
                                    System.Windows.Forms.MessageBox.Show($"Popup Option {i} clicked!");

                    return new CustomButtonDefinition(btn);
                }
                        )
                .Cast<IButtonDefinition>()];
            }

        #endregion Constructor

        #region Methods

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddPopup(_mainButton, _otherButtons, true, env);
            }

        public void Execute()
            { }

        #endregion Methods
        }

    public class SlideoutPopup : IPanelButton
        {
        #region Fields
        private readonly List<IButtonDefinition> _additionalButtons;
        private readonly object _icon16;
        private readonly object _icon32;
        private readonly IButtonDefinition _mainButton;
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutPopup";
        public RibbonType Environment => RibbonType.All;
        public string InternalName => "SlideoutPopup.Internal";
        public ButtonDefinition InventorButtonDefinition => null;
        public object LargeIcon => null;
        public string PanelInternalName => "Popup_Internal";
        public string PanelName => "Popup";
        public bool Pressed { get; set; }
        public object StandardIcon => null;

        #endregion Properties

        #region Constructor

        public SlideoutPopup(Application app)
            {
            _icon16 = InventorIconConverter.FromFile(Icon16x16);
            _icon32 = InventorIconConverter.FromFile(Icon32x32);

            ButtonDefinition MainBtn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Visible in all Environments",
                ToolTipText: "Popup Main",
                StandardIcon: _icon16,
                LargeIcon: _icon32,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            _mainButton = new CustomButtonDefinition(MainBtn);
            _additionalButtons = [.. Enumerable.Range(1, 3)
                 .Select(i =>
                 {
                     string Icon32x32 = IconHelper.GetIconPath(i, 32);
                     object _icon32 = InventorIconConverter.FromFile(Icon32x32);
                     var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                         DisplayName: $"Popup Option {i}",
                                         InternalName: $"SlideoutPopup.Option{i}.Internal",
                                         Classification: CommandTypesEnum.kShapeEditCmdType,
                                         ClientId: StandardAddInServer._guid,
                                         DescriptionText: $"Popup Option button {i}",
                                         ToolTipText: $"Popup Option {i}",
                                         StandardIcon: null,
                                         LargeIcon: _icon32,
                                         ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                     btn.OnExecute += (ctx) =>
                                     System.Windows.Forms.MessageBox.Show($"Popup Option {i} clicked!");

                     return new CustomButtonDefinition(btn);
                 }
                         )
                 .Cast<IButtonDefinition>()];
            }

        #endregion Constructor

        #region Methods

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddPopupToSlideout(_mainButton, _additionalButtons, false, env);
            }

        public void Execute()
            { }

        #endregion Methods
        }
    }