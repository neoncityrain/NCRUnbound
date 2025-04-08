using JollyCoop;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Unbound
{
    internal class Techgen
    {
        public static void Init()
        {
            On.JollyCoop.JollyMenu.JollySlidingMenu.NextClass += DontSkipSmiley;
            On.JollyCoop.JollyCustom.SlugClassMenu += AllowSmiley;
            On.MultiplayerUnlocks.ClassUnlocked += UnlockSmiley;
            On.Menu.MultiplayerMenu.NextClass += SmileyAllowedNextclass;
        }

        private static SlugcatStats.Name SmileyAllowedNextclass(On.Menu.MultiplayerMenu.orig_NextClass orig, Menu.MultiplayerMenu self, SlugcatStats.Name curClass)
        {
            SlugcatStats.Name name;
            if (curClass == null)
            {
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(0), false);
            }
            else
            {
                if (curClass.Index >= ExtEnum<SlugcatStats.Name>.values.Count - 1 || curClass.Index == -1)
                {
                    return null;
                }
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(curClass.Index + 1), false);
            }

            if (curClass != null && curClass != null && name != null &&
                name.value == "NCRtech")
            {
                try
                {
                    return name;
                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);
                    NCRDebug.Log(e);
                }
            }
            return orig(self, curClass);
        }

        private static bool UnlockSmiley(On.MultiplayerUnlocks.orig_ClassUnlocked orig, MultiplayerUnlocks self, SlugcatStats.Name classID)
        {
            if (classID != null && classID.value == "NCRtech")
            {
                return true;
            }
            return orig(self, classID);
        }

        private static SlugcatStats.Name AllowSmiley(On.JollyCoop.JollyCustom.orig_SlugClassMenu orig, int playerNumber, SlugcatStats.Name fallBack)
        {
            if (fallBack != null && JollyCustom.JollyOptions(playerNumber) != null &&
                JollyCustom.JollyOptions(playerNumber).playerClass != null &&
                JollyCustom.JollyOptions(playerNumber).playerClass.value == "NCRtech")
            {
                try
                {
                    return JollyCustom.JollyOptions(playerNumber).playerClass;
                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);
                    NCRDebug.Log("Error setting Technician as a Jollycat: " + e);
                }
            }
            return orig(playerNumber, fallBack);
        }

        private static SlugcatStats.Name DontSkipSmiley(On.JollyCoop.JollyMenu.JollySlidingMenu.orig_NextClass orig, JollyCoop.JollyMenu.JollySlidingMenu self, SlugcatStats.Name curClass)
        {
            SlugcatStats.Name name;
            if (curClass == null)
            {
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(0), false);
            }
            else
            {
                if (curClass.Index >= ExtEnum<SlugcatStats.Name>.values.Count - 1 || curClass.Index == -1)
                {
                    return self.NextClass(null);
                }
                name = new SlugcatStats.Name(ExtEnum<SlugcatStats.Name>.values.GetEntry(curClass.Index + 1), false);
            }

            if (curClass != null && curClass != null && name != null &&
                name.value == "NCRtech"
                )
            {
                try
                {
                    return name;
                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);
                    NCRDebug.Log(e);
                }
            }
            return orig(self, curClass);
        }
    }
}
