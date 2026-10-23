using Inventor;
using InventorRibbonManager;


namespace SampleAddin
    {
    public class Button : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;
        private readonly object _icon16;
        private readonly object _icon32;

        #endregion Fields

        #region Properties
        public string DisplayName => "SampleButton";
        public string InternalName => "SampleButton.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public ButtonDefinition InventorButtonDefinition { get; private set; }
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "Button";
        public string PanelInternalName => "Button_Internal";
        public static bool IsToggle => false;
        public bool Pressed { get; set; }

        #endregion Properties

        #region Constructor
        public Button(Application app)
            {
            _icon16 = InventorIconConverter.FromFile(Icon16x16);
            _icon32 = InventorIconConverter.FromFile(Icon32x32);  

            InventorButtonDefinition = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Visible in all Environments",
                ToolTipText: DisplayName,
                StandardIcon: StandardIcon,
                LargeIcon: LargeIcon,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            InventorButtonDefinition.OnExecute += (ctx) => Execute();
            InventorButtonDefinition.ProgressiveToolTip.IsProgressive = true;
            InventorButtonDefinition.ProgressiveToolTip.Description = "This is a sample button that shows a message box when clicked.";
            InventorButtonDefinition.ProgressiveToolTip.ExpandedDescription = "This button is part of the sample add-in and demonstrates how to create a simple button in the Inventor Ribbon.";
            InventorButtonDefinition.ProgressiveToolTip.Title = "Sample Button Tooltip";
            InventorButtonDefinition.ProgressiveToolTip.Image = _icon32;
            }

        #endregion Constructor

        #region Methods
        public void Execute()
            {
            System.Windows.Forms.MessageBox.Show("Sample Button Clicked!");
            }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddButton(this, true, env);
            }

        #endregion Methods
        }

    public class SlideoutButton : IPanelButton
        {
        #region Fields
        private static string Icon16x16 => IconHelper.Icon1_16x16;
        private static string Icon32x32 => IconHelper.Icon1_32x32;
        private readonly object _icon16;
        private readonly object _icon32;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutButton";
        public string InternalName => "SlideoutButton.Internal";
        public object StandardIcon => _icon16;
        public object LargeIcon => _icon32;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "Button";
        public string PanelInternalName => "Button_Internal";
        public bool Pressed { get; set; }
        public ButtonDefinition InventorButtonDefinition { get; private set; }
        #endregion Properties

        #region Constructor
        public SlideoutButton(Application app)
            {
            _icon16 = InventorIconConverter.FromFile(Icon16x16);
            _icon32 = InventorIconConverter.FromFile(Icon32x32); 

            InventorButtonDefinition = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Button in Slideout",
                ToolTipText: DisplayName,
                StandardIcon: StandardIcon,
                LargeIcon: LargeIcon,
                ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);

            InventorButtonDefinition.OnExecute += (ctx) => Execute();
            }

        #endregion Constructor

        #region Methods
        public void Execute()
            {
            System.Windows.Forms.MessageBox.Show("Slideout Button Clicked!");
            }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddButtonToSlideout(this, env);
            }

        #endregion Methods
        }
    }