using Inventor;
using InventorRibbonManager;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class ComboBox : IPanelButton
        {
        #region Fields
        private readonly IComboBoxDefinition _comboBoxDef;
        private int _lastListIndex = -1;

        #endregion Fields

        #region Properties
        public string DisplayName => "ComboBox";
        public string InternalName => "ComboBox.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "ComboBox";
        public string PanelInternalName => "ComboBox_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public ComboBox(Application app)
            {
            ComboBoxDefinition comboBoxDef = app.CommandManager.ControlDefinitions.AddComboBoxDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                DropDownWidth: 100,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "ComboBox for testing",
                ToolTipText: DisplayName
            );

            comboBoxDef.AddItem("Option 1", 1);
            comboBoxDef.AddItem("Option 2", 2);
            comboBoxDef.ListIndex = 1;

            // Event (optional)
            comboBoxDef.OnSelect += ComboBox_OnSelect;

            _comboBoxDef = new ComboBoxDefinitionWrapper(comboBoxDef);
            }

        #endregion Constructor

        #region Methods
        public void Execute()
            { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddComboBox(_comboBoxDef, env);
            }

        #endregion Methods

        #region private Methods
        private void ComboBox_OnSelect(NameValueMap context)
            {
            if (!StandardAddInServer.addInLoaded)
                { return; }
            ComboBoxDefinition comboBox = _comboBoxDef.InventorComboBoxDefinition;

            if (comboBox.ListIndex != _lastListIndex)
                {
                _lastListIndex = comboBox.ListIndex;
                System.Windows.Forms.MessageBox.Show($"ComboBox selection changed: {comboBox.ListIndex}");
                }
            }

        #endregion private Methods
        }

    public class SlideoutComboBox : IPanelButton
        {
        #region Fields
        private readonly IComboBoxDefinition _comboBoxDef;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutComboBox";
        public string InternalName => "SlideoutComboBox.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "ComboBox";
        public string PanelInternalName => "ComboBox_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SlideoutComboBox(Application app)
            {
            var def = app.CommandManager.ControlDefinitions.AddComboBoxDefinition(
                DisplayName: DisplayName,
                InternalName: InternalName,
                Classification: CommandTypesEnum.kShapeEditCmdType,
                DropDownWidth: 100,
                ClientId: StandardAddInServer._guid,
                DescriptionText: "Slideout ComboBox",
                ToolTipText: DisplayName);

            def.AddItem("Slide 1", 1);
            def.AddItem("Slide 2", 2);
            def.ListIndex = 1;

            _comboBoxDef = new ComboBoxDefinitionWrapper(def);
            }

        #endregion Constructor

        #region Methods
        public void Execute() { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddComboBoxToSlideout(_comboBoxDef, env);
            }

        #endregion Methods
        }
    }