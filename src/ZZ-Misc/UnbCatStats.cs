using System;
using JollyCoop;

namespace Unbound
{
    internal static class UnbCatStats
    {
        public static void Init()
        {
            On.SlugcatStats.getSlugcatName += UnbNameLogging;
            //On.SlugcatStats.HiddenOrUnplayableSlugcat += lockedCats;
            On.SlugcatStats.AutoGrabBatflys += NoGrabby;

            On.SlugcatStats.SpearSpawnElectricRandomChance += ElectricSpear;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance += ExplosiveSpear;
            On.SlugcatStats.SpearSpawnModifier += SpawnMod;
            // changes spear chances to be between arti and hunter
        }

        private static bool NoGrabby(On.SlugcatStats.orig_AutoGrabBatflys orig, SlugcatStats.Name slugcatNum)
        {
            if (slugcatNum != null &&
                (slugcatNum == UnboundEnums.NCRUnbound || slugcatNum.value == "NCRunbound"))
            {
                return false;
            }
            return orig(slugcatNum);
        }

        private static float SpawnMod(On.SlugcatStats.orig_SpearSpawnModifier orig, SlugcatStats.Name index, float originalSpearChance)
        {
            if (index != null)
            {
                if (index == UnboundEnums.NCRUnbound || index.value == "NCRunbound")
                {
                    return Mathf.Pow(originalSpearChance, 0.825f);
                }
                if (index == UnboundEnums.NCRTechnician || index.value == "NCRtech")
                {
                    return Mathf.Pow(originalSpearChance, 0.9f);
                }
            }
            
            return orig(index, originalSpearChance);
        }

        private static float ExplosiveSpear(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
        {
            if (index != null)
            {
                if (index == UnboundEnums.NCRUnbound || index.value == "NCRunbound")
                {
                    return 0.011f;
                }
                if (index == UnboundEnums.NCRTechnician || index.value == "NCRtech")
                {
                    return 0.013f;
                }
            }
            
            return orig(index);
        }

        private static float ElectricSpear(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
        {
            if (index != null)
            {
                if (ModManager.MSC && (index == UnboundEnums.NCRUnbound || index.value == "NCRunbound"))
                {
                    return 0.011f;
                }
                else if (ModManager.MSC && (index == UnboundEnums.NCRTechnician || index.value == "NCRtech"))
                {
                    return 0.09f;
                }
            }
            
            return orig(index);
        }

        private static bool lockedCats(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (i != null)
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
            }
            
            return orig(i);
        }

        private static string UnbNameLogging(On.SlugcatStats.orig_getSlugcatName orig, SlugcatStats.Name i)
        {
            if (i != null && (i == UnboundEnums.NCRTechnician || i.value == "NCRtech"))
            {
                return "Technician";
            }
            if (i != null && (i == UnboundEnums.NCRUnbound || i.value == "NCRunbound"))
            {
                return "Unbound";
            }
            return orig(i);
        }
    }
}
