using System;

namespace Unbound
{
    internal static class UnbCatStats
    {
        public static void Init()
        {
            On.SlugcatStats.getSlugcatName += UnbNameLogging;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += smil;
            On.SlugcatStats.AutoGrabBatflys += NoGrabby;

            On.SlugcatStats.SpearSpawnElectricRandomChance += ElectricSpear;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance += ExplosiveSpear;
            On.SlugcatStats.SpearSpawnModifier += SpawnMod;
            // changes spear chances to be between arti and hunter
        }

        private static bool NoGrabby(On.SlugcatStats.orig_AutoGrabBatflys orig, SlugcatStats.Name slugcatNum)
        {
            if (slugcatNum == UnboundEnums.NCRUnbound)
            {
                return false;
            }
            return orig(slugcatNum);
        }

        private static float SpawnMod(On.SlugcatStats.orig_SpearSpawnModifier orig, SlugcatStats.Name index, float originalSpearChance)
        {
            if (index == UnboundEnums.NCRUnbound)
            {
                return Mathf.Pow(originalSpearChance, 0.825f);
            }
            return orig(index, originalSpearChance);
        }

        private static float ExplosiveSpear(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
        {
            if (index == UnboundEnums.NCRUnbound)
            {
                return 0.012f;
            }
            return orig(index);
        }

        private static float ElectricSpear(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
        {
            if (ModManager.MSC && index == UnboundEnums.NCRUnbound)
            {
                return 0.011f;
            }
            return orig(index);
        }

        private static bool smil(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (i.value == "NCRtech" || i == UnboundEnums.NCRTechnician)
            {
                return true;
            }
            if (i.value == "NCRoracle" || i == UnboundEnums.NCROracle)
            {
                return true;
            }
            if (i.value == "NCRreverb" || i == UnboundEnums.NCRReverb)
            {
                return true;
            }
            else return orig(i);
        }

        private static string UnbNameLogging(On.SlugcatStats.orig_getSlugcatName orig, SlugcatStats.Name i)
        {
            if (i != null && i == UnboundEnums.NCRTechnician)
            {
                return "Technician";
            }
            if (i != null && i == UnboundEnums.NCRUnbound)
            {
                return "Unbound";
            }
            return orig(i);
        }
    }
}
