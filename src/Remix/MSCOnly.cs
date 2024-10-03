using System;
using Menu;
using MoreSlugcats;

namespace Unbound
{
    internal class MSCOnly
    {
        public static CollectionsMenu.PearlReadContext UnbPebbles = new("UnbPebbles", true);

        public static void Init()
        {
            On.MoreSlugcats.CollectionsMenu.Singal += AddUnbound;
            On.MoreSlugcats.CollectionsMenu.DataPearlToFileID += PearlToFile;


            On.MoreSlugcats.StowawayBugState.AwakeThisCycle += AwakenMyPsionicWarriors;
            // rerolls if a stowaway is awake or not. it should result in a bit over a 1/3 chance that it will be awake each cycle

            On.SeedCob.HitByWeapon += SeedAllergy;
            // SEED COB ALLERGYYYYYY

        }

        private static bool AwakenMyPsionicWarriors(On.MoreSlugcats.StowawayBugState.orig_AwakeThisCycle orig,
            StowawayBugState self, int cycle)
        {
            if (self != null && self.creature != null && self.creature.Room != null &&
                self.creature.world.game.session.characterStats.name.value == "NCRunbound" && ModManager.MSC)
            {
                NCRDebug.Log("Unbound world, rerolling stowawake (because life is a fucking nightmare)");
                System.Random rd = new System.Random();
                int rand_num = rd.Next(1, 3);
                if (rand_num == 1)
                {
                    NCRDebug.Log("Congrats! Stowaway awoken (because life is a fucking nightmare)");
                    return true;
                    // if random number is 1, awaken stowaway
                }
                else
                {
                    NCRDebug.Log("Stowaway state defaulting to normal");
                    return orig(self, cycle);
                    // if the random number isnt 1, refer to the original code
                }
            }
            else return orig(self, cycle);
        }

        private static void SeedAllergy(On.SeedCob.orig_HitByWeapon orig, SeedCob self, Weapon weapon)
        {
            if (self != null &&
                !(weapon == null || self.room == null || self.room.roomSettings == null) &&
                self.room.game.session.characterStats.name.value == "NCRunbound" && ModManager.MSC)
            {
                if (self.room.roomSettings.DangerType == MoreSlugcatsEnums.RoomRainDangerType.Blizzard &&
                    weapon.firstChunk.vel.magnitude < 20f)
                {
                    if (UnityEngine.Random.Range(0.5f, 0.8f) < self.freezingCounter)
                    {
                        self.spawnUtilityFoods();
                    }
                    return;
                }
                if (weapon.thrownBy != null && weapon.thrownBy is Player && ((weapon.thrownBy as Player).slugcatStats.name ==
                    MoreSlugcatsEnums.SlugcatStatsName.Spear || (weapon.thrownBy as Player).SlugCatClass ==
                    MoreSlugcatsEnums.SlugcatStatsName.Saint))
                {
                    return;
                }
                self.spawnUtilityFoods();
                return;
            }
            else
            {
                orig(self, weapon);
            }
        }

        private static int PearlToFile(On.MoreSlugcats.CollectionsMenu.orig_DataPearlToFileID orig, DataPearl.AbstractDataPearl.DataPearlType type)
        {
            Conversation.ID a = Conversation.DataPearlToConversation(type);
            if (a == UnboundEnums.unbKarmaPearlConv)
            {
                return 1431821;
            }
            else return orig(type);
        }

        private static void AddUnbound(On.MoreSlugcats.CollectionsMenu.orig_Singal orig, CollectionsMenu self,
            MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message.Contains("PEARL") || message.Contains("TYPE"))
            {
                DataPearl.AbstractDataPearl.DataPearlType dataPearlType = self.usedPearlTypes[self.selectedPearlInd];
                int DataPearlToFileID = CollectionsMenu.DataPearlToFileID(dataPearlType);

                CollectionsMenu.PearlReadContext a = null;
                for (int m = 0; m < self.iteratorButtons.Length; m++)
                {
                    if (message.Contains("UNBOUNDPEBBLES"))
                    {
                        a = UnbPebbles;
                    }
                }
                SlugcatStats.Name saveFile = null;
                if (a == UnbPebbles)
                {
                    saveFile = UnboundEnums.NCRUnbound;
                }
                self.InitLabelsFromPearlFile(DataPearlToFileID, saveFile);
            }
        }

        public static bool SetPearlDecipheredUnbound(DataPearl.AbstractDataPearl.DataPearlType pearlType,
            PlayerProgression.MiscProgressionData self)
        {
            try { 
                if (pearlType != null && self != null && ModManager.MSC)
                {
                    int num = CollectionsMenu.DataPearlToFileID(pearlType);
                    if (num != -1 && !Conversation.EventsFileExists(self.owner.rainWorld, num, UnboundEnums.NCRUnbound))
                    {
                        return self.SetPearlDeciphered(pearlType);
                    }
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error setting pearl as deciphered via normal route: " + e);
            }

            try
            {
                if (pearlType == null || pearlType != null && UnboundEnums.decipheredPearlsUnboundSession.Contains(pearlType))
                {
                    return false;
                }
                UnboundEnums.decipheredPearlsUnboundSession.Add(pearlType);
                self.owner.SaveProgression(false, true);
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error setting pearl as deciphered via Unbound Enums: " + e);
            }
            return true;
        }
        // end collections things
    }
}
