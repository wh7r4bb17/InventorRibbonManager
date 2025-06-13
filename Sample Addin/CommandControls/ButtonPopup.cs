using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class ButtonPopup : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;
        private static string Icon16x16_2 => IconHelper.Icon2_16x16;
        private static string Icon32x32_2 => IconHelper.Icon2_32x32;
        private readonly IPictureDisp _icon16;
        private readonly IPictureDisp _icon32;
        private readonly List<IButtonDefinition> _buttons;
        #endregion Fields

        #region Properties
        public string DisplayName => "ButtonPopup";
        public string InternalName => "ButtonPopup.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public ButtonDefinition InventorButtonDefinition { get; }
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "ButtonPopup";
        public string PanelInternalName => "ButtonPopup_Internal";
        public bool IsToggle => false;
        public bool Pressed { get; set; }

        #endregion Properties

        #region Constructor
        public ButtonPopup(Application app)
            {
            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
            _buttons = new List<IButtonDefinition>();

            InventorButtonDefinition = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Visible in all Environments",
                ToolTipText: DisplayName,
                StandardIcon: _icon16,
                LargeIcon: _icon32,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            InventorButtonDefinition.OnExecute += (ctx) => Execute();
            _buttons.Add(this);

            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16_2);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32_2);

            ButtonDefinition secondBtn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: "ButtonPopup_1",
                InternalName: "ButtonPopup_1.Internal",
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Visible in all Environments",
                ToolTipText: "ButtonPopup_1",
                StandardIcon: _icon16,
                LargeIcon: _icon32,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            secondBtn.OnExecute += (ctx) => Execute2();

            _buttons.Add(new CustomButtonDefinition(secondBtn));
            }
        #endregion Constructor

        #region Methods
        public void Execute()
            {
            System.Windows.Forms.MessageBox.Show("First Button in Popup clicked!");
            }

        public static void Execute2()
            {
            System.Windows.Forms.MessageBox.Show("Second Button in Popup clicked!");
            }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddButtonPopup(_buttons, true, env);
            }
        #endregion Methods
        }

    public class SlideoutButtonPopup : IPanelButton
        {
        #region Fields
        private readonly List<IButtonDefinition> _buttons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutButtonPopup";
        public string InternalName => "SlideoutButtonPopup.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "ButtonPopup";
        public string PanelInternalName => "ButtonPopup_Internal";
        public bool Pressed { get; set; }
        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SlideoutButtonPopup(Application app)
            {
            _buttons = Enumerable.Range(1, 3)
                .Select(i =>
                            {
                                string Icon16x16 = IconHelper.GetIconPath(i, 16);
                                string Icon32x32 = IconHelper.GetIconPath(i, 32);
                                object _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
                                object _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
                                var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                        DisplayName: $"SlideoutBtn {i}",
                                        InternalName: $"SlideoutBtn{i}.Internal",
                                        Classification: CommandTypesEnum.kShapeEditCmdType,
                                        ClientId: StandardAddInServer._guid,
                                        DescriptionText: $"Slideout button {i}",
                                        ToolTipText: $"SlideoutBtn {i}",
                                        StandardIcon: _icon16,
                                        LargeIcon: _icon32,
                                        ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

                                btn.OnExecute += (ctx) =>
                                    System.Windows.Forms.MessageBox.Show($"Slideout Popup {i} clicked!");

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
            panel.AddButtonPopupToSlideout(_buttons, true, env);
            }

        #endregion Methods
        }
    }