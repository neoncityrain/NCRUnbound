using Expedition;

namespace Unbound
{
    internal class UnbExpedition
    {
        public static void Init()
        {
            On.Expedition.PearlDeliveryChallenge.UpdateDescription += Description;
            On.Expedition.PearlDeliveryChallenge.Update += Update;
            On.Expedition.NeuronDeliveryChallenge.ValidForThisSlugcat += Invalid;
        }

        private static bool Invalid(On.Expedition.NeuronDeliveryChallenge.orig_ValidForThisSlugcat orig, NeuronDeliveryChallenge self, SlugcatStats.Name slugcat)
        {
            if (slugcat == UnboundEnums.NCRUnbound)
            {
                return false;
            }
            else
            {
                return orig(self, slugcat);
            }
        }

        private static void Update(On.Expedition.PearlDeliveryChallenge.orig_Update orig, PearlDeliveryChallenge self)
        {
            if (!(self.game == null || self.completed) &&
                ExpeditionData.slugcatPlayer == UnboundEnums.NCRUnbound)
            {
                #region Base.Update
                if (self.revealCheckDelay < 100)
                {
                    self.revealCheckDelay++;
                }
                if (self.hidden && !self.revealCheck && self.revealCheckDelay >= 100)
                {
                    self.revealCheck = true;
                    self.CheckRevealable();
                }
                #endregion
                if (self.iterator != 1)
                {
                    self.iterator = 1;
                }
                for (int i = 0; i < self.game.Players.Count; i++)
                {
                    if (self.game.Players[i] != null && self.game.Players[i].realizedCreature != null &&
                        self.game.Players[i].realizedCreature.room != null &&
                        self.game.Players[i].realizedCreature.room.abstractRoom.name == "SS_AI")
                    {
                        for (int j = 0; j < self.game.Players[i].realizedCreature.room.updateList.Count; j++)
                        {
                            if (self.game.Players[i].realizedCreature.room.updateList[j] is DataPearl &&
                                ChallengeTools.ValidRegionPearl(self.region,
                                (self.game.Players[i].realizedCreature.room.updateList[j] as DataPearl).AbstractPearl.dataPearlType) &&
                                (self.game.Players[i].realizedCreature.room.updateList[j] as DataPearl).firstChunk.pos.x > 0f)
                            {
                                self.CompleteChallenge();
                            }
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void Description(On.Expedition.PearlDeliveryChallenge.orig_UpdateDescription orig, PearlDeliveryChallenge self)
        {
            if (ExpeditionData.slugcatPlayer == UnboundEnums.NCRUnbound)
            {
                self.description = ChallengeTools.IGT.Translate("<region> pearl delivered to Five Pebbles").Replace("<region>",
                    ChallengeTools.IGT.Translate(Region.GetRegionFullName(self.region, ExpeditionData.slugcatPlayer)));
                self.UpdateDescription();
            }
            else
            {
                orig(self);
            }
        }
    }
}
