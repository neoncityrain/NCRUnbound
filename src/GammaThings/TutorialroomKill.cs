using System;

namespace Unbound
{
    internal class TutorialroomKill
    {
        public static void Init()
        {
            On.RoomSpecificScript.SU_A43SuperJumpOnly.Update += SU_A43SuperJumpOnly_Update;
            On.RoomSpecificScript.SU_C04StartUp.Update += SU_C04StartUp_Update;
            On.RoomSpecificScript.SU_A23FirstCycleMessage.Update += SU_A23FirstCycleMessage_Update;
            On.RoomSpecificScript.SL_C12JetFish.Update += SL_C12JetFish_Update;
            On.RoomSpecificTextMessage.Update += RoomSpecificTextMessage_Update;
            // gamma shut up about tutorials challenge
        }


        private static void RoomSpecificTextMessage_Update(On.RoomSpecificTextMessage.orig_Update orig, RoomSpecificTextMessage self, bool eu)
        {
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void SL_C12JetFish_Update(On.RoomSpecificScript.SL_C12JetFish.orig_Update orig, RoomSpecificScript.SL_C12JetFish self, bool eu)
        {
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void SU_A23FirstCycleMessage_Update(On.RoomSpecificScript.SU_A23FirstCycleMessage.orig_Update orig, RoomSpecificScript.SU_A23FirstCycleMessage self, bool eu)
        {
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void SU_C04StartUp_Update(On.RoomSpecificScript.SU_C04StartUp.orig_Update orig, RoomSpecificScript.SU_C04StartUp self, bool eu)
        {
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void SU_A43SuperJumpOnly_Update(On.RoomSpecificScript.SU_A43SuperJumpOnly.orig_Update orig, RoomSpecificScript.SU_A43SuperJumpOnly self, bool eu)
        {
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }
    }
}
