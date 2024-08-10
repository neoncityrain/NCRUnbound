using OverseerHolograms;
using Expedition;

namespace Unbound
{
    internal class GammaAITweaks
    {
        public static void Init()
        {
            On.Overseer.TryAddHologram += HologramTweaks;
            On.OverseerAbstractAI.RoomAllowed += RoomAllowed;
            On.OverseerCommunicationModule.FoodDelicousScore += StopLeadingToFoodUnboundCantEat;
            On.OverseerAbstractAI.HowInterestingIsCreature += InterestInUnbound;

            On.OverseerAI.Update += GammaAIUpdate;
        }

        private static void GammaAIUpdate(On.OverseerAI.orig_Update orig, OverseerAI self)
        {
            if (self != null && self.overseer != null &&
                self.overseer.PlayerGuide &&
                self.overseer.room.game.session.characterStats.name.value == "NCRunbound")
            {
                NCRDebug.Log("OOO GAMMA TIME");

                #region Base.Update
                self.timeInRoom++;
                for (int i = 0; i < self.modules.Count; i++)
                {
                    self.modules[i].Update();
                }
                if (ModManager.Expedition && self.creature.world.game.rainWorld.ExpeditionMode && 
                    ExpeditionGame.activeUnlocks.Contains("bur-hunted") && self.creature.world.rainCycle.CycleProgression > 0.05f &&
                    self.tracker != null && self.creature.world.game.Players != null)
                {
                    int j = 0;
                    while (j < self.creature.world.game.Players.Count)
                    {
                        if (self.creature.world.game.Players[j].realizedCreature != null && !(self.creature.world.game.Players[j].realizedCreature as Player).dead)
                        {
                            if (self.creature.Room != self.creature.world.game.Players[j].Room)
                            {
                                self.tracker.SeeCreature(self.creature.world.game.Players[j]);
                                return;
                            }
                            break;
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
                #endregion

                self.slowLookAt = Vector2.Lerp(Custom.MoveTowards(self.slowLookAt, self.lookAt, 60f), self.lookAt, 0.02f);
                if (UnityEngine.Random.value < 0.015f) // slightly more likely than usual to swap casual interests
                {
                    self.casualInterestBonus = Mathf.Pow(UnityEngine.Random.value, 3f) * 2f * ((UnityEngine.Random.value < 0.5f) ? -1f : 1f);
                }
                if (self.overseer.hologram != null)
                {
                    self.lookAt = self.overseer.hologram.lookAt;
                }
                else if (self.overseer.SandboxOverseer && !self.overseer.editCursor.menuMode)
                {
                    self.lookAt = self.overseer.editCursor.pos;
                }
                else
                {
                    bool watchingCreature = self.casualInterestCreature != null && self.casualInterestCreature.realizedCreature != null && 
                        self.casualInterestCreature.pos.room == self.overseer.room.abstractRoom.index && self.tutorialBehavior == null;

                    if (watchingCreature)
                    {
                        if (self.targetCreature != null && self.targetCreature.realizedCreature != null && self.targetCreature.realizedCreature.room == 
                            self.overseer.room)
                        {
                            watchingCreature = (self.RealizedCreatureInterest(self.casualInterestCreature.realizedCreature) + self.casualInterestBonus > 
                                self.RealizedCreatureInterest(self.targetCreature.realizedCreature));
                        }
                        else
                        {
                            watchingCreature = (self.RealizedCreatureInterest(self.casualInterestCreature.realizedCreature) + self.casualInterestBonus > 0f);
                        }
                    }
                    if (self.lookAtFlyingWeapon != null)
                    {
                        self.lookAt = self.lookAtFlyingWeapon.firstChunk.pos;
                        if (self.lookAtFlyingWeapon.slatedForDeletetion || self.lookAtFlyingWeapon.mode != Weapon.Mode.Thrown)
                        {
                            self.lookAtFlyingWeapon = null;
                        }
                    }
                    else if (watchingCreature)
                    {
                        self.lookAt = self.casualInterestCreature.realizedCreature.DangerPos;
                        self.LensUpdate(self.casualInterestCreature.realizedCreature);
                    }
                    else if (self.targetCreature != null && self.targetCreature.realizedCreature != null && 
                        self.targetCreature.realizedCreature.room == self.overseer.room)
                    {
                        self.lookAt = self.targetCreature.realizedCreature.DangerPos;
                        self.LensUpdate(self.targetCreature.realizedCreature);
                    }
                    else
                    {
                        self.targetStationary = Mathf.Max(0f, self.targetStationary - 0.008333334f);
                        Vector2 testPos;
                        if (UnityEngine.Random.value < 0.1f)
                        {
                            testPos = self.overseer.mainBodyChunk.pos + Custom.RNV() * UnityEngine.Random.value * 600f;
                        }
                        else
                        {
                            testPos = self.lookAt + Custom.RNV() * Mathf.Pow(UnityEngine.Random.value, 3f) * 600f;
                        }
                        if (self.LookAtAirPosScore(testPos) > self.LookAtAirPosScore(self.lookAt))
                        {
                            self.lookAt = testPos;
                            self.lookAtSameAirPosCounter = UnityEngine.Random.Range(30, 130);
                        }
                        else
                        {
                            self.lookAtSameAirPosCounter--;
                            if (self.lookAtSameAirPosCounter < 1)
                            {
                                self.lastLookAtAirPositions.Insert(0, self.lookAt);
                                if (self.lastLookAtAirPositions.Count > 10)
                                {
                                    self.lastLookAtAirPositions.RemoveAt(self.lastLookAtAirPositions.Count - 1);
                                }
                                self.lookAtSameAirPosCounter = UnityEngine.Random.Range(30, 130);
                            }
                        }
                    }
                }
                if (UnityEngine.Random.value < 0.027f)
                {
                    self.lookAtAdd = Custom.RNV() * UnityEngine.Random.value;
                }
                self.UpdateZipMatrix();
                self.UpdateTempHoverPosition();
                if (self.overseer.mode == Overseer.Mode.Watching || self.overseer.mode == Overseer.Mode.Projecting)
                {
                    if (self.overseer.room.abstractRoom.creatures.Count == 0)
                    {
                        return;
                    }
                    AbstractCreature abstractCreature = self.overseer.room.abstractRoom.creatures[UnityEngine.Random.Range(0, 
                        self.overseer.room.abstractRoom.creatures.Count)];
                    if (abstractCreature.realizedCreature != null)
                    {
                        if (abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Overseer)
                        {
                            if (!abstractCreature.creatureTemplate.smallCreature &&
                                // not a small thang
                                ((abstractCreature.realizedCreature is Player &&
                                !(abstractCreature.realizedCreature as Player).GetNCRunbound().IsUnbound) ||
                                // if its IS a player, but it ISNT unbound
                                !(abstractCreature.realizedCreature is Player)) &&
                                // or if its not a player
                                !abstractCreature.realizedCreature.dead && 
                                // not dead
                                Custom.DistLess(self.overseer.rootPos, abstractCreature.realizedCreature.DangerPos, self.scaredDistance - 10f))
                                // is inside the fear distance
                            {
                                self.casualInterestCreature = abstractCreature;
                                self.overseer.afterWithdrawMode = Overseer.Mode.SittingInWall;
                                self.overseer.SwitchModes(Overseer.Mode.Withdrawing);
                            }
                            else if (abstractCreature.realizedCreature is Player && (abstractCreature.realizedCreature as Player).GetNCRunbound().IsUnbound  &&
                                !abstractCreature.realizedCreature.dead)
                            {
                                self.casualInterestCreature = abstractCreature;
                            }
                            else if (self.targetCreature != abstractCreature && (self.casualInterestCreature == null || 
                                self.RealizedCreatureInterest(abstractCreature.realizedCreature) > 
                                self.RealizedCreatureInterest(self.casualInterestCreature.realizedCreature) + 0.1f || 
                                self.targetCreature == self.casualInterestCreature) && 
                                self.overseer.room.VisualContact(self.overseer.mainBodyChunk.pos, abstractCreature.realizedCreature.mainBodyChunk.pos))
                            {
                                self.casualInterestCreature = abstractCreature;
                            }
                        }
                        else if (abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Scavenger && 
                            (self.creature.abstractAI as OverseerAbstractAI).goToPlayer)
                        {
                            (self.creature.abstractAI as OverseerAbstractAI).PlayerGuideGoAway(UnityEngine.Random.Range(200, 1200));
                            NCRDebug.Log("player guide leaving because scavs in room");
                        }
                        else if (self.overseer.mode != Overseer.Mode.Projecting && self.overseer.conversationDelay == 0)
                        {
                            Overseer overseer = abstractCreature.realizedCreature as Overseer;

                            if (Custom.DistLess(self.overseer.rootPos, overseer.rootPos, 70f * self.overseer.size + 70f + overseer.size) && 
                                overseer.mode == Overseer.Mode.Watching && overseer.conversationPartner == null && 
                                overseer.conversationDelay == 0 && self.overseer.lastConversationPartner != overseer)
                            {
                                self.overseer.conversationPartner = overseer;
                                overseer.conversationPartner = self.overseer;
                                self.overseer.SwitchModes(Overseer.Mode.Conversing);
                                overseer.SwitchModes(Overseer.Mode.Conversing);
                                self.overseer.conversationDelay = UnityEngine.Random.Range(30, 190);
                                overseer.conversationDelay = UnityEngine.Random.Range(30, 190);
                            }
                        }
                    }
                }
                else if (self.overseer.mode == Overseer.Mode.SittingInWall)
                {
                    bool flag2 = false;
                    int num = 0;
                    while (num < self.overseer.room.abstractRoom.creatures.Count && !flag2)
                    {
                        if (self.overseer.room.abstractRoom.creatures[num].realizedCreature != null && 
                            self.overseer.room.abstractRoom.creatures[num].creatureTemplate.type != CreatureTemplate.Type.Overseer && 
                            !self.overseer.room.abstractRoom.creatures[num].creatureTemplate.smallCreature && 
                            !self.overseer.room.abstractRoom.creatures[num].realizedCreature.dead && 
                            Custom.DistLess(self.overseer.rootPos, self.overseer.room.abstractRoom.creatures[num].realizedCreature.DangerPos, 200f))
                        {
                            flag2 = true;
                        }
                        num++;
                    }
                    if (!flag2)
                    {
                        self.overseer.SwitchModes(Overseer.Mode.Emerging);
                    }
                }
                else if (self.overseer.mode == Overseer.Mode.Conversing)
                {
                    if (self.overseer.conversationPartner == null || self.overseer.conversationPartner.room != self.overseer.room || 
                        self.overseer.conversationPartner.mode != Overseer.Mode.Conversing || 
                        self.overseer.conversationPartner.conversationPartner != self.overseer)
                    {
                        self.overseer.SwitchModes(Overseer.Mode.Watching);
                    }
                    else
                    {
                        self.lookAt = self.overseer.conversationPartner.mainBodyChunk.pos;
                    }
                }
                self.creature.abstractAI.AbstractBehavior(1);
            }
            else
            {
                orig(self);
            }
        }

        private static float InterestInUnbound(On.OverseerAbstractAI.orig_HowInterestingIsCreature orig, OverseerAbstractAI self, 
            AbstractCreature testCrit)
        {
            if (self != null && self.world != null && !self.world.game.IsArenaSession && self.isPlayerGuide &&
                testCrit != null &&
                self.world.game.session.characterStats.name.value == "NCRunbound")
            {
                if (testCrit.creatureTemplate.smallCreature)
                {
                    return 0.05f;
                }
                float num;
                if (testCrit.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                {
                    if (testCrit.realizedCreature != null)
                    {
                        if (testCrit.realizedCreature is Player && (testCrit.realizedCreature as Player).GetNCRunbound().IsUnbound)
                        {
                            if (testCrit.realizedCreature.dead)
                            { 
                                num = 10f;
                            }
                            else
                            { 
                                num = Custom.LerpMap((float)(testCrit.realizedCreature as Player).Karma, 1f, 5f, 0.55f, 2.5f, 1.2f);
                            }
                        }
                        else
                        {
                            num = Custom.LerpMap((float)(testCrit.realizedCreature as Player).Karma, 0f, 4f, 0.15f, 2f, 1.2f);
                        }
                    }
                    else
                    {
                        num = 0.15f;
                    }
                    num = Mathf.Max(1.5f, num);
                    if (ModManager.MMF && self.goToPlayer)
                    {
                        num = 1.5f;
                    }
                }

                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.Fly)
                {
                    num = 0.001f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.CicadaA ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.CicadaB)
                {
                    num = 0.09f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.GarbageWorm)
                {
                    num = 0.1f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.GreenLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.PinkLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.Salamander)
                {
                    num = 0.2f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.Vulture ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.LanternMouse ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.Centipede)
                {
                    num = 0.25f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.BlackLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.BlueLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.KingVulture)
                {
                    num = 0.4f;
                }
                else if ((ModManager.MSC &&
                    (testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.EelLizard ||
                    testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SpitLizard)))
                {
                    num = 0.5f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.BigEel)
                {
                    num = 0.65f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.WhiteLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.YellowLizard ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.MirosBird ||
                    (ModManager.MSC && testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.TrainLizard))
                {
                    num = 0.7f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.DaddyLongLegs ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.BrotherLongLegs)
                {
                    num = 0.8f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.RedLizard ||
                    (ModManager.MSC && (testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.StowawayBug ||
                    testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture)))
                {
                    num = 0.85f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.Overseer ||
                    testCrit.creatureTemplate.type == CreatureTemplate.Type.RedCentipede)
                {
                    num = 0.9f;
                }
                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.Scavenger)
                {
                    num = 1f;
                }
                else if (ModManager.MSC && testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerElite ||
                    testCrit.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.ScavengerKing)
                {
                    num = 2f;
                }

                else if (testCrit.creatureTemplate.type == CreatureTemplate.Type.LizardTemplate)
                {
                    // this should make it so any misc lizards are at 0.5
                    num = 0.5f;
                }
                else
                {
                    num = 0.15f;
                }
                if (testCrit.state.dead)
                {
                    num /= 5f;
                }
                num *= testCrit.Room.AttractionValueForCreature(self.parent.creatureTemplate.type);
                return num * Mathf.Lerp(0.5f, 1.5f, self.world.game.SeededRandom(self.parent.ID.RandomSeed + testCrit.ID.RandomSeed));
            }
            else
            {
                return orig(self, testCrit);
            }
        }

        private static float StopLeadingToFoodUnboundCantEat(On.OverseerCommunicationModule.orig_FoodDelicousScore orig, OverseerCommunicationModule self, AbstractPhysicalObject foodObject, Player player)
        {
            if (self != null && self.overseerAI != null && self.overseerAI.overseer != null && self.overseerAI.overseer.room != null &&
                self.overseerAI.overseer.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                if (foodObject == null || foodObject.realizedObject == null || foodObject.Room != player.abstractCreature.Room ||
                    foodObject.slatedForDeletion)
                {
                    return 0f;
                }
                if (foodObject.type != AbstractPhysicalObject.AbstractObjectType.DangleFruit &&
                    foodObject.type != AbstractPhysicalObject.AbstractObjectType.JellyFish &&
                    foodObject.type != AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer)
                {
                    return 0f;
                }
                float num = Mathf.InverseLerp(1100f, 400f, Vector2.Distance(foodObject.realizedObject.firstChunk.pos, player.DangerPos));
                if (num == 0f)
                {
                    return 0f;
                }
                if (self.GuideState.itemTypes.Contains(foodObject.type))
                {
                    if (num <= 0.2f || !self.room.ViewedByAnyCamera(foodObject.realizedObject.firstChunk.pos, 0f))
                    {
                        return 0f;
                    }
                    num = 0.3f;
                }
                for (int i = 0; i < self.objectsAlreadyTalkedAbout.Count; i++)
                {
                    if (self.objectsAlreadyTalkedAbout[i] == foodObject.ID)
                    {
                        return 0f;
                    }
                }
                if (foodObject == self.mostDeliciousFoodInRoom &&
                    self.currentConcern == OverseerCommunicationModule.PlayerConcern.FoodItemInRoom)
                {
                    num *= 1.1f;
                }
                return num * Mathf.Lerp(self.GeneralPlayerFoodNeed(player), 0.6f, 0.5f);
            }
            else return orig(self, foodObject, player);
        }

        private static bool RoomAllowed(On.OverseerAbstractAI.orig_RoomAllowed orig, OverseerAbstractAI self, int room)
        {
            if (self != null && self.world != null && self.RelevantPlayer != null &&
                self.world.game.session.characterStats.name.value == "NCRunbound" && self.playerGuide)
            {
                if (room < self.world.firstRoomIndex || room >= self.world.firstRoomIndex + self.world.NumberOfRooms)
                {
                    return false;
                }
                for (int i = 0; i < OverseerAbstractAI.tutorialRooms.Length; i++)
                {
                    if (self.world.GetAbstractRoom(room).name == OverseerAbstractAI.tutorialRooms[i])
                    {
                        return true;
                    }
                }
                return (self.world.region.name == "MS" || self.world.GetAbstractRoom(room).gate ||
                    // can always show up in MS, can enter gate rooms
                    !(self.world.GetAbstractRoom(room).AttractionForCreature(self.parent.creatureTemplate.type) ==
                    AbstractRoom.CreatureRoomAttraction.Forbidden) ||
                    // if the room is not forbidden
                    !self.world.GetAbstractRoom(room).scavengerOutpost || !self.world.GetAbstractRoom(room).scavengerTrader) ||
                    // if the room is not a scav outpost / scav trader
                    self.world.GetAbstractRoom(room).shelter;
                // or if the room IS a shelter (enabling the guide to come inside the shelter with the player)
            }
            return orig(self, room);
        }

        private static void HologramTweaks(On.Overseer.orig_TryAddHologram orig, Overseer self, OverseerHolograms.OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            if (self != null && self.room != null && !self.dead &&
                self.room.game.session.characterStats.name.value == "NCRunbound" && self.PlayerGuide)
            {
                if (self.room != null)
                {
                    if (self.room.abstractRoom.name == "SS_AI")
                    {
                        return;
                    }
                    // dont show holograms in pebbles' chamber. this is initially only for MSC- should not trigger for UB either,
                    // at least for now
                }
                if (self.hologram != null)
                {
                    if (self.hologram.message == message)
                    {
                        return;
                    }
                    if (self.hologram.importance >= importance && importance != 3.4028235E+38f)
                    {
                        return;
                    }
                    self.hologram.stillRelevant = false;
                    self.hologram = null;
                    // removes unnecessary holograms
                }
                if (self.room == null)
                {
                    return;
                    // dont show holograms if not in a room
                }
                // ordinarily the tutorial holograms are here. this mod goes with the assumption that the player knows how to play,
                // so those are removed
                if (message == OverseerHologram.Message.Angry)
                {
                    self.hologram = new AngryHologram(self, message, communicateWith, importance);
                }
                // this is moved to the top, as it is the highest priority.
                else if (message == OverseerHologram.Message.DangerousCreature)
                {
                    self.hologram = new OverseerHologram.CreaturePointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.Shelter)
                {
                    self.hologram = new OverseerHologram.ShelterPointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.Bats)
                {
                    self.hologram = new OverseerHologram.BatPointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.FoodObject)
                {
                    self.hologram = new OverseerHologram.FoodPointer(self, message, communicateWith, importance);
                }

                else
                {
                    return;
                    // return if it doesnt fit the above (i.e., if its a tutorial or progression direction)
                }
                self.room.AddObject(self.hologram);
            }
            else
            {
                orig(self, message, communicateWith, importance);
            }
        }
    }
}
