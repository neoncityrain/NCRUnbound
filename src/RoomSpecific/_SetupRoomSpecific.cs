using System;

namespace Unbound
{
    internal class _SetupRoomSpecific
    {
        public static void Init()
        {
            On.Player.ctor += Player_ctor;
            // for room-specific scripts / events

            On.RegionGate.customKarmaGateRequirements += RegionGate_customKarmaGateRequirements;
            // custom gate tweaks- allows for exiting MS
        }

        private static void RegionGate_customKarmaGateRequirements(On.RegionGate.orig_customKarmaGateRequirements orig, RegionGate self)
        {
            if (self != null && self.room != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.abstractRoom.name == "GATE_SL_MS")
                {
                    int num2;
                    if (int.TryParse(self.karmaRequirements[1].value, out num2))
                    {
                        self.karmaRequirements[1] = RegionGate.GateRequirement.OneKarma;
                    }
                }
            }
            orig(self);
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self != null && self.room != null && self.room.game != null && self.abstractCreature != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound" && self.room.game.session is StoryGameSession)
            {







                // THINGS FOR GAME SETUP BELOW ------------------------------------------------------------------------------------------------------------
                if (!self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon)
                {
                    if (world.region.name == "MS" && ModManager.MSC)
                    {
                        UnityEngine.Debug.Log("MS start detected, triggering intro!");
                        self.room.AddObject(new UnboundIntro());
                    }
                    else if (world.region.name == "SL" && !ModManager.MSC)
                    {
                        UnityEngine.Debug.Log("SL start detected");
                        self.objectInStomach = new DataPearl.AbstractDataPearl(self.room.world,
                            AbstractPhysicalObject.AbstractObjectType.DataPearl, null,
                            new WorldCoordinate(self.room.abstractRoom.index, -1, -1, 0), self.room.game.GetNewID(), -1, -1, null,
                            Pearl.unboundKarmaPearl);
                    }

                    if (self.room.world.overseersWorldAI.playerGuide != null && self.room.world.overseersWorldAI != null)
                    {
                        AbstractCreature gammaoverseer = self.room.world.overseersWorldAI.playerGuide;
                        gammaoverseer.ignoreCycle = true;
                        gammaoverseer.creatureTemplate.waterVision = 0;
                        gammaoverseer.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;

                        (gammaoverseer.abstractAI as OverseerAbstractAI).BringToRoomAndGuidePlayer(self.room.abstractRoom.index);
                        (self.room.world.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.likesPlayer += 1f;
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Gamma missing for some reason, not calling it to room");
                    }

                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    UnityEngine.Debug.Log("Unbound start detected! This SHOULD trigger regardless of the cat being actively played, and only trigger once!");
                }



                if (self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    UnityEngine.Debug.Log("Old save detected, fixing game");
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = false;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    // re-kills moon. sorry women.
                }
                if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles == true)
                {
                    UnityEngine.Debug.Log("Pebbles dead for some reason, attempting to fix game");
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                }
            }

            if (self != null && self.room != null &&
                self.slugcatStats.name.value == "NCRunbound")
            {
                self.GetCat().IsUnbound = true;
                self.abstractCreature.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;
            }
        }
        // end srs
    }
}
