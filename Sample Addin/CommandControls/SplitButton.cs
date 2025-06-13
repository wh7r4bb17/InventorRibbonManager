using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class SplitButton : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;

        private readonly IPictureDisp _icon16;
        private readonly IPictureDisp _icon32;
        private IButtonDefinition _mainButton;
        private List<IButtonDefinition> _otherButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SplitButton";
        public string InternalName => "SplitButton.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "SplitButton";
        public string PanelInternalName => "SplitButton_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SplitButton(Inventor.Application app)
            {
            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);

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

            MainBtn.OnExecute += (ctx) => Execute();

            _mainButton = new CustomButtonDefinition(MainBtn);

            _otherButtons = Enumerable.Range(1, 3)
              .Select(i =>
              {
                  string Icon32x32 = IconHelper.GetIconPath(i, 32);
                  object _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
                  var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                      DisplayName: $"Split Option {i}",
                                      InternalName: $"Split.Option{i}.Internal",
                                      Classification: CommandTypesEnum.kShapeEditCmdType,
                                      ClientId: StandardAddInServer._guid,
                                      DescriptionText: $"Split Option button {i}",
                                      ToolTipText: $"Split Option {i}",
                                      StandardIcon: null,
                                      LargeIcon: _icon32,
                                      ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                  btn.OnExecute += (ctx) =>
                                  System.Windows.Forms.MessageBox.Show($"Split Option {i} clicked!");

                  return new CustomButtonDefinition(btn);
              }
                      )
              .Cast<IButtonDefinition>()
              .ToList();
            }

        #endregion Constructor

        #region Methods
        public void Execute()
            {
            System.Windows.Forms.MessageBox.Show($"Split Button clicked!");
            }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddSplitButton(_mainButton, _otherButtons, true, env);
            }

        #endregion Methods
        }

    public class SlideoutSplitButton : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;

        private readonly IPictureDisp _icon16;
        private readonly IPictureDisp _icon32;
        private IButtonDefinition _mainButton;
        private List<IButtonDefinition> _otherButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutSplitButton";
        public string InternalName => "SlideoutSplitButton.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "SplitButton";
        public string PanelInternalName => "SplitButton_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SlideoutSplitButton(Inventor.Application app)
            {
            _icon16 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon16x16);
            _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);

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

            MainBtn.OnExecute += (ctx) => Execute();

            _mainButton = new CustomButtonDefinition(MainBtn);

            _otherButtons = Enumerable.Range(1, 3)
              .Select(i =>
              {
                  string Icon32x32 = IconHelper.GetIconPath(i, 32);
                  object _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
                  var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                      DisplayName: $"Split Option {i}",
                                      InternalName: $"SlideoutSplit.Option{i}.Internal",
                                      Classification: CommandTypesEnum.kShapeEditCmdType,
                                      ClientId: StandardAddInServer._guid,
                                      DescriptionText: $"Split Option button {i}",
                                      ToolTipText: $"Split Option {i}",
                                      StandardIcon: null,
                                      LargeIcon: _icon32,
                                      ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                  btn.OnExecute += (ctx) =>
                                  System.Windows.Forms.MessageBox.Show($"Split Option {i} clicked!");

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
            panel.AddSplitButtonToSlideout(_mainButton, _otherButtons, true, env);
            }

        #endregion Methods
        }
    }