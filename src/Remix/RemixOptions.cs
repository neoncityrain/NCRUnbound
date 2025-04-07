namespace Unbound
{
    public partial class UnbSetupThings
    {
        private void PlayerRemix(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
        {
            orig(self, abstractcreature, world);

            var getUnb = self.GetNCRunbound();
            getUnb.MoreDebug = UnbOptions.MoreDebugLogs.Value; // more debug

            getUnb.CyJump1Maximum = UnbOptions.CyJumpCharge.Value; // cyan jump 1
            getUnb.CyJump2Maximum = UnbOptions.CyJump2Charge.Value; // cyan jump 2

            getUnb.GraphicsDisabled = UnbOptions.UnbGraphicsDisabled.Value; // disable/enable general graphics
            getUnb.RingsDisabled = UnbOptions.UnbRingsDisabled.Value; // enable/disable rings
            getUnb.WingscalesDisabled = UnbOptions.UnbWingsDisabled.Value;  // enable/disable wings

            getUnb.Unpicky = UnbOptions.UnbUnpicky.Value; // enable/disable swallowing

            getUnb.RGBRings = UnbOptions.RGBRings.Value; // party button
        }

        public void OverseerRemix(On.Overseer.orig_ctor orig, Overseer self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            self.GetGamma().RGBMode = UnbOptions.RGBRings.Value; // party button
        }
    }
}