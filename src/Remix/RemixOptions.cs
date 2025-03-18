namespace Unbound
{
    public partial class UnbSetupThings
    {
        private void PlayerRemix(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
        {
            orig(self, abstractcreature, world);
            self.GetNCRunbound().MoreDebug = UnbOptions.MoreDebugLogs.Value; // more debug

            self.GetNCRunbound().CyJump1Maximum = UnbOptions.CyJumpCharge.Value; // cyan jump 1
            self.GetNCRunbound().CyJump2Maximum = UnbOptions.CyJump2Charge.Value; // cyan jump 2

            self.GetNCRunbound().GraphicsDisabled = UnbOptions.UnbGraphicsDisabled.Value; // disable/enable general graphics
            self.GetNCRunbound().RingsDisabled = UnbOptions.UnbRingsDisabled.Value; // enable/disable rings
            self.GetNCRunbound().Unpicky = UnbOptions.UnbUnpicky.Value; // enable/disable swallowing

            self.GetNCRunbound().RGBRings = UnbOptions.RGBRings.Value; // party button
        }

        public void OverseerRemix(On.Overseer.orig_ctor orig, Overseer self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            self.GetGamma().RGBMode = UnbOptions.RGBRings.Value; // party button
        }
    }
}