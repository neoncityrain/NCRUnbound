using BuiltinBuffs;
using System.Linq;

namespace Unbound
{
    internal class RandomBuffThings
    {
        public static void TailTracking(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (self != null && self.room != null && ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff"))
            { 
                self.GetNCRunbound().LostTail = !self.GetExPlayerData().HaveTail; 
            }
            else { self.GetNCRunbound().LostTail = false; }
        }
    }
}
