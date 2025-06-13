using Inventor;
using InventorRibbonManager;
using System.Collections.Generic;
using System.Linq;
using static InventorRibbonManager.InventorRibbonBuilder;

namespace SampleAddin
    {
    public class SplitButtonMRU : IPanelButton
        {
        #region Fields
        private readonly List<IButtonDefinition> _mruButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SplitButtonMRU";
        public string InternalName => "SplitButtonMRU.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "SplitMRU";
        public string PanelInternalName => "SplitMRU_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SplitButtonMRU(Inventor.Application app)
            {
            _mruButtons = Enumerable.Range(1, 4)
                .Select(i =>
            {
                string Icon32x32 = IconHelper.GetIconPath(i, 32);
                object _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
                var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                    DisplayName: $"MRU Option {i}",
                                    InternalName: $"MRU.Option{i}.Internal",
                                    Classification: CommandTypesEnum.kShapeEditCmdType,
                                    ClientId: StandardAddInServer._guid,
                                    DescriptionText: $"MRU Option button {i}",
                                    ToolTipText: $"MRU Option {i}",
                                    StandardIcon: null,
                                    LargeIcon: _icon32,
                                    ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                btn.OnExecute += (ctx) =>
                                System.Windows.Forms.MessageBox.Show($"Split Option {i} clicked!");


                return new CustomButtonDefinition(btn);
            })
                .Cast<IButtonDefinition>()
                .ToList();
            }

        #endregion Constructor

        #region Methods
        public void Execute() { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddSplitButtonMRU(_mruButtons, true, env);
            }

        #endregion Methods
        }

    public class SlideoutSplitButtonMRU : IPanelButton
        {
        #region Fields
        private readonly List<IButtonDefinition> _mruButtons;

        #endregion Fields

        #region Properties
        public string DisplayName => "SlideoutSplitMRU";
        public string InternalName => "SlideoutSplitMRU.Internal";
        public object StandardIcon => null;
        public object LargeIcon => null;
        public RibbonType Environment => RibbonType.All;
        public string PanelName => "SplitMRU";
        public string PanelInternalName => "SplitMRU_Internal";
        public bool Pressed { get; set; }

        public ButtonDefinition InventorButtonDefinition => null;

        #endregion Properties

        #region Constructor
        public SlideoutSplitButtonMRU(Inventor.Application app)
            {
            _mruButtons = Enumerable.Range(1, 3)
                .Select(i =>
                {
                    string Icon32x32 = IconHelper.GetIconPath(i, 32);
                    object _icon32 = (IPictureDisp)PictureDispConverter.GetFromPath(Icon32x32);
                    var btn = app.CommandManager.ControlDefinitions.AddButtonDefinition(
                                                    DisplayName: $"MRU Option {i}",
                                                    InternalName: $"SlideoutMRU.Option{i}.Internal",
                                                    Classification: CommandTypesEnum.kShapeEditCmdType,
                                                    ClientId: StandardAddInServer._guid,
                                                    DescriptionText: $"MRU Option button {i}",
                                                    ToolTipText: $"MRU Option {i}",
                                                    StandardIcon: null,
                                                    LargeIcon: _icon32,
                                                    ButtonDisplay: ButtonDisplayEnum.kDisplayTextInLearningMode);
                    btn.OnExecute += (ctx) =>
                                                System.Windows.Forms.MessageBox.Show($"MRU Option {i} clicked!");


                    return new CustomButtonDefinition(btn);
                })
                .Cast<IButtonDefinition>()
                .ToList();
            }

        #endregion Constructor

        #region Methods
        public void Execute() { }

        public void AddToPanel(IRibbonPanel panel, RibbonType env)
            {
            panel.AddSplitButtonMRUToSlideout(_mruButtons, true, env);
            }

        #endregion Methods
        }
    }