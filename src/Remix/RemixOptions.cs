using System;

namespace Unbound
{
    public partial class NCRUnbound
    {
        private void PlayerOnctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
        {
            orig(self, abstractcreature, world);

            self.GetCat().CyJump1Maximum = UnbOptions.CyJumpCharge.Value;
            self.GetCat().CyJump2Maximum = UnbOptions.CyJump2Charge.Value;

            self.GetCat().GraphicsDisabled = UnbOptions.UnbGraphicsDisabled.Value;
            self.GetCat().RingsDisabled = UnbOptions.UnbRingsDisabled.Value;
            self.GetCat().Unpicky = UnbOptions.UnbUnpicky.Value;
        }
    }
}