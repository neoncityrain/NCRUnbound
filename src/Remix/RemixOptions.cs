using System;

namespace Unbound
{
    public partial class NCRUnbound
    {
        private void PlayerOnctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
        {
            orig(self, abstractcreature, world);
            self.GetNCRunbound().MoreDebug = UnbOptions.MoreDebugLogs.Value;

            self.GetNCRunbound().CyJump1Maximum = UnbOptions.CyJumpCharge.Value;
            self.GetNCRunbound().CyJump2Maximum = UnbOptions.CyJump2Charge.Value;

            self.GetNCRunbound().GraphicsDisabled = UnbOptions.UnbGraphicsDisabled.Value;
            self.GetNCRunbound().RingsDisabled = UnbOptions.UnbRingsDisabled.Value;
            self.GetNCRunbound().Unpicky = UnbOptions.UnbUnpicky.Value;
        }
    }
}