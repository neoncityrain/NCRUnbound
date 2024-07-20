using System;

namespace Unbound
{
    internal class SetupRoomSpecific
    {
        public static void Init()
        {
            On.Player.ctor += RoomAndPlayerSpecific;
            // for room-specific scripts / events

            On.RegionGate.customKarmaGateRequirements += CustomKarmaGates;
            // custom gate tweaks- allows for exiting MS
        }

        private static void CustomKarmaGates(On.RegionGate.orig_customKarmaGateRequirements orig, RegionGate self)
        {
            if (self != null && self.room != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.abstractRoom.name == "GATE_SL_MS")
                {
                    self.karmaRequirements[1] = RegionGate.GateRequirement.OneKarma; // karma requirement to leave submerged and enter shoreline
                }
            }
            orig(self);
        }

        private static void RoomAndPlayerSpecific(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
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
                        Debug.Log("Unbound's MS start detected, triggering intro!");
                        self.room.AddObject(new UnboundIntro());
                    }
                    else if (world.region.name == "SL" && !ModManager.MSC)
                    {
                        Debug.Log("Unbound's SL start detected! Remind me to set up a non-MSC intro :<");
                        self.objectInStomach = new DataPearl.AbstractDataPearl(self.room.world,
                            AbstractPhysicalObject.AbstractObjectType.DataPearl, null,
                            new WorldCoordinate(self.room.abstractRoom.index, -1, -1, 0), self.room.game.GetNewID(), -1, -1, null,
                            Pearl.unboundKarmaPearl);
                    }

                    if (self.room.world.overseersWorldAI.playerGuide != null && self.room.world.overseersWorldAI != null)
                    {
                        try
                        {
                            AbstractCreature gammaoverseer = self.room.world.overseersWorldAI.playerGuide;
                            gammaoverseer.ignoreCycle = true;
                            gammaoverseer.creatureTemplate.waterVision = 0;
                            gammaoverseer.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;

                            (gammaoverseer.abstractAI as OverseerAbstractAI).BringToRoomAndGuidePlayer(self.room.abstractRoom.index);
                            (self.room.world.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.likesPlayer += 1f;
                        }
                        catch (Exception e)
                        {

                            Debug.Log("Gamma missing! Error: " + e);
                        }
                    }

                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;

                    if (self.GetNCRunbound().MoreDebug)
                    {
                        Debug.Log("Unbound start detected! This SHOULD trigger regardless of the cat being actively played, and only trigger once!");
                    }
                }



                if (self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = false;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    if (self.GetNCRunbound().MoreDebug)
                    {
                        Debug.Log("Old save detected, fixing game- moon has been re-killed! Sorry, women");
                    }
                }
                if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles == true)
                {
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    if (self.GetNCRunbound().MoreDebug)
                    {
                        Debug.Log("Pebbles was killed for some reason and has been revived");
                    }
                }
            }

            // misc
            if (self != null && self.room != null &&
                self.slugcatStats.name.value == "NCRunbound")
            {
                self.GetNCRunbound().IsUnbound = true;
                self.abstractCreature.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;
            }
        }
        // end srs
    }
}
