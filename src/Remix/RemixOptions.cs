using System;

namespace Unbound
{
    public partial class UnbSetupThings
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

            self.GetNCRunbound().RGBRings = UnbOptions.RGBRings.Value;
        }

        public void RemixSet(On.Overseer.orig_ctor orig, Overseer self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            self.GetGamma().RGBMode = UnbOptions.RGBRings.Value;
        }
    }
}