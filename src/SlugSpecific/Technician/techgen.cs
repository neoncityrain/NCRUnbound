using JollyCoop;

namespace Unbound
{
    internal class Techgen
    {
        public static void Init()
        {
            On.Menu.MultiplayerMenu.NextClass += allowSmiley;
            On.JollyCoop.JollyCustom.SlugClassMenu += jollyAllowSmiley;
        }

        private static SlugcatStats.Name jollyAllowSmiley(On.JollyCoop.JollyCustom.orig_SlugClassMenu orig, int playerNumber, SlugcatStats.Name fallBack)
        {
            SlugcatStats.Name name = JollyCustom.JollyOptions(playerNumber).playerClass;
            if (name != null && SlugcatStats.HiddenOrUnplayableSlugcat(name) &&
                (name.value == "NCRtech"))
            {
                return name;
            }
            return orig(playerNumber, fallBack);
        }

        private static SlugcatStats.Name allowSmiley(On.Menu.MultiplayerMenu.orig_NextClass orig, Menu.MultiplayerMenu self, SlugcatStats.Name curClass)
        {
            SlugcatStats.Name name;
            if (curClass == null)
            {
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(0), false);
                // get slugcat zero if theres no entry
            }
            else
            {
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(curClass.Index + 1), false);
                // get next cat in line
            }
            if (name != null && SlugcatStats.HiddenOrUnplayableSlugcat(name) &&
                (name.value == "NCRtech"))
            {
                return name;
            }
            return orig(self, curClass);
        }
    }
}
