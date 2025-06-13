using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class TogglePopup : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;
        private readonly IPictureDisp _icon16;
        private readonly IPictureDisp _icon32;

        private IButtonDefinition _mainButton;
        private List<IButtonDefinition> _options;

        #endregion Fields

        #region Properties
        public string DisplayName => "TogglePopup";
        public string InternalName => "TogglePopup.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "TogglePopup";
        public string PanelInternalName => "TogglePopup_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public TogglePopup(Application app)
            {
            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);

            var MainBtn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
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

            _options = Enumerable.Range(1, 3)
                .Select(i =>
                {
                    var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                        DisplayName: $"Toggle Option {i}",
                                        InternalName: $"Toggle.Option{i}.Internal",
                                        Classification: CommandTypesEnum.kShapeEditCmdType,
                                        ClientId: StandardAddInServer._guid,
                                        DescriptionText: $"Toggle Option button {i}",
                                        ToolTipText: $"Toggle Option {i}",
                                        StandardIcon: null,
                                        LargeIcon: null,
                                        ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                    btn.OnExecute += (ctx) =>
                            {
                                btn.Pressed = !btn.Pressed;   // Toggle the pressed state  for UI
                                System.Windows.Forms.MessageBox.Show($"Toggle Option {i} clicked!");
                            };

                    return new CustomButtonDefinition(btn);
                }
                        )
                .Cast<IButtonDefinition>()
                .ToList();
            }

        #endregion Constructor

        #region Methods
        public void Execute() { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddTogglePopup(_mainButton, _options, true, env);
            }

        #endregion Methods
        }

    public class SlideoutTogglePopup : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;
        private readonly IPictureDisp _icon16;
        private readonly IPictureDisp _icon32;

        private IButtonDefinition _mainButton;
        private List<IButtonDefinition> _options;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutTogglePopup";
        public string InternalName => "SlideoutTogglePopup.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "TogglePopup";
        public string PanelInternalName => "TogglePopup_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SlideoutTogglePopup(Application app)
            {
            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);

            var MainBtn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
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

            _options = Enumerable.Range(1, 3)
                .Select(i =>
                {
                    var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                        DisplayName: $"Toggle Option {i}",
                                        InternalName: $"SlideoutToggle.Option{i}.Internal",
                                        Classification: CommandTypesEnum.kShapeEditCmdType,
                                        ClientId: StandardAddInServer._guid,
                                        DescriptionText: $"Toggle Option button {i}",
                                        ToolTipText: $"Toggle Option {i}",
                                        StandardIcon: null,
                                        LargeIcon: null,
                                        ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                    btn.OnExecute += (ctx) =>
                    {
                        btn.Pressed = !btn.Pressed; // Toggle the pressed state for UI
                        System.Windows.Forms.MessageBox.Show($"Toggle Option {i} clicked!");
                    };

                    return new CustomButtonDefinition(btn);
                }
                        )
                .Cast<IButtonDefinition>()
                .ToList();
            }

        #endregion Constructor

        #region Methods
        public void Execute() { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddTogglePopupToSlideout(_mainButton, _options, true, env);
            }

        #endregion Methods
        }
    }