using Menu.Remix.MixedUI;

namespace Unbound
{
public class UnbRemInterface : OptionInterface
{
    private readonly ManualLogSource Logger;

    public UnbRemInterface(NCRUnbound modInstance, ManualLogSource loggerSource)
    {
            Logger = loggerSource;
            CyJumpCharge = this.config.Bind<float>("CyJumpCharge", 180f, new ConfigAcceptableRange<float>(5f, 500f));
            CyJump2Charge = this.config.Bind<float>("CyJump2Charge", 400f, new ConfigAcceptableRange<float>(5f, 1000f));
            UnbGraphicsDisabled = this.config.Bind<bool>("UnbGraphicsDisabled", false);
            UnbRingsDisabled = this.config.Bind<bool>("UnbRingsDisabled", false);
            UnbUnpicky = this.config.Bind<bool>("UnbUnpicky", false);
            MoreDebugLogs = this.config.Bind<bool>("MoreDebugLogs", false);
        }

        public readonly Configurable<float> CyJumpCharge;
        public readonly Configurable<float> CyJump2Charge;
        public readonly Configurable<bool> UnbGraphicsDisabled;
        public readonly Configurable<bool> UnbRingsDisabled;
        public readonly Configurable<bool> UnbUnpicky;
        public readonly Configurable<bool> MoreDebugLogs;
        private UIelement[] UIArrPlayerOptions;


    public override void Initialize()
    {
        var opTab = new OpTab(this, "Options");
        this.Tabs = new[]
        {
            opTab
        };

        UIArrPlayerOptions = new UIelement[]
        {
            new OpLabel(10f, 550f, "Options", true),

            // row 1

            new OpLabel(10f, 520f, "Cyan Doublejump Recharge"),
            new OpUpdown(CyJumpCharge, new Vector2(30f,490f), 100f, 1),

            new OpLabel(200f, 520f, "Disable Misc Graphics"),
            new OpCheckBox(UnbGraphicsDisabled, new Vector2(230f,490f)),

            new OpLabel(360f, 520f, "Disable Cyan Rings"),
            new OpCheckBox(UnbRingsDisabled, new Vector2(390f,490f)),

            // row 2

            new OpLabel(10f, 450f, "Cyan Triplejump Recharge"),
            new OpUpdown(CyJump2Charge, new Vector2(30f,420f), 100f, 1),

            new OpLabel(200f, 450f, "Allow Object Swallowing"),
            new OpCheckBox(UnbUnpicky, new Vector2(230f,420f)),

            new OpLabel(360f, 450f, "More Debug Logs"),
            new OpCheckBox(MoreDebugLogs, new Vector2(390f,420f))
        };
        opTab.AddItems(UIArrPlayerOptions);
    }

    public override void Update()
    {
        if (((OpUpdown)UIArrPlayerOptions[2]).GetValueFloat() > 10)
        {
            ((OpLabel)UIArrPlayerOptions[3]).Show();
        }
        else
        {
            ((OpLabel)UIArrPlayerOptions[3]).Hide();
        }
    }

}}