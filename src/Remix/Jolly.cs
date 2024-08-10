using JollyCoop;

namespace Unbound
{
    internal static class UnbJolly
    {
        public static void Init()
        {
            On.JollyCoop.JollyMenu.JollyPlayerSelector.GetPupButtonOffName += PupButton;
            On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.HasUniqueSprite += UnbUnique;
        }

        private static bool UnbUnique(On.JollyCoop.JollyMenu.SymbolButtonTogglePupButton.orig_HasUniqueSprite orig, JollyCoop.JollyMenu.SymbolButtonTogglePupButton self)
        {
            return (self.symbolNameOff.Contains("unb") && !self.isToggled) || orig(self);
        }

        private static string PupButton(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_GetPupButtonOffName orig, JollyCoop.JollyMenu.JollyPlayerSelector self)
        {
            SlugcatStats.Name playerClass = self.JollyOptions(self.index).playerClass;
            if (playerClass != null && playerClass.value.Equals("NCRunbound"))
            {
                return "unb_pup_off";
            }
            return orig(self);
        }
    }
}
