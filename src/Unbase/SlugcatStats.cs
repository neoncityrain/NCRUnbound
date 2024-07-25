using System;
using Menu;

namespace Unbound
{
    public class UnboundSlugcatStats
    {
        public static void Init()
        {
            On.SlugcatStats.SpearSpawnElectricRandomChance += ElectricSpear;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance += ExplosiveSpear;
        }

        private static float ExplosiveSpear(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance orig, SlugcatStats.Name index)
        {
            if (index == UnboundEnums.NCRUnbound)
            {
                return 0.01f;
            }
            else return orig(index);
        }

        private static float ElectricSpear(On.SlugcatStats.orig_SpearSpawnElectricRandomChance orig, SlugcatStats.Name index)
        {
            if (index == UnboundEnums.NCRUnbound)
            {
                return 0.045f;
            }
            else return orig(index);
        }
    }
}
