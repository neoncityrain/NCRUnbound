using Music;
using System.IO;
using System.Linq;

namespace Unbound
{
    internal class SetupRoomSpecific
    {
        public static void IsGammaInMyShelter(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            try
            {
                if (self != null &&
                self.room != null && self.room.game != null && self.abstractCreature != null &&
                self.room.abstractRoom != null && self.room.game != null && self.room.game.session != null &&
                self.room.abstractRoom.creatures != null && self.room.abstractRoom.creatures.Count > 0 &&
                self.room.abstractRoom.shelter && self.room.game.session.characterStats != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
                {
                    List<AbstractCreature> overseersInRoom = new List<AbstractCreature>();
                    if (self.room.abstractRoom.creatures.Count > 1)
                    {
                        for (int j = 0; j < self.room.abstractRoom.creatures.Count; j++)
                        {
                            if (self.room.abstractRoom.creatures[j].creatureTemplate.type == CreatureTemplate.Type.Overseer &&
                                self.room.abstractRoom.creatures[j].Room == self.room.abstractRoom &&
                                self.room.abstractRoom.creatures[j].creatureTemplate.type != CreatureTemplate.Type.Slugcat)
                            {
                                overseersInRoom.Add(self.room.abstractRoom.creatures[j]);
                            }
                        }
                    }

                    if (overseersInRoom.Count == 1 && !self.room.world.game.rainWorld.GetNCRModSaveData().IsGammaInMyShelter)
                    {
                        if (self.GetNCRunbound().MoreDebug)
                        {
                            NCRDebug.Log("Gamma in shelter Unbound is in!");
                        }
                        self.room.world.game.rainWorld.GetNCRModSaveData().IsGammaInMyShelter = true;
                    }

                    if (overseersInRoom.Count != 1 && self.room.world.game.rainWorld.GetNCRModSaveData().IsGammaInMyShelter)
                    {
                        if (self.GetNCRunbound().MoreDebug)
                        {
                            NCRDebug.Log("Gamma no longer in shelter Unbound is in!");
                        }
                        self.room.world.game.rainWorld.GetNCRModSaveData().IsGammaInMyShelter = false;
                    }

                    overseersInRoom.Clear();
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Hi this is Unbound I love throwing tantrums: " + e);
            }
        }

        public static void MaintainRoomSpecific(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self != null && self.room != null && self.room.game != null && self.abstractCreature != null &&
                world != null && world.region != null && world.region.name != null && 
                abstractCreature != null && self.room.game.session.characterStats.name.value == "NCRunbound" &&
                self.room.game.session is not ArenaGameSession && (!ModManager.MSC || !self.room.game.rainWorld.safariMode)
                )
            {
                try
                {
                    if (self.room.world != null && self.room.world.overseersWorldAI != null && self.room.world.overseersWorldAI.playerGuide != null)
                    {
                        AbstractCreature gammaoverseer = self.room.world.overseersWorldAI.playerGuide;
                        if (gammaoverseer != null && gammaoverseer.ID != null &&
                            gammaoverseer.ID.number != -7113131)
                        {
                            if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Gamma ID tweaked!"); }
                            gammaoverseer.ID.number = -7113131; // sets gamma to always have this id
                            // "7 1 13 13 1", aka "gamma", negative because the positive counterpart has only two mycelia and that looked weird
                            if (gammaoverseer.ignoreCycle != true)
                            {
                                gammaoverseer.ignoreCycle = true;
                                gammaoverseer.creatureTemplate.waterVision = -1f;
                                gammaoverseer.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;
                                gammaoverseer.creatureTemplate.bodySize = 0.7f;

                                (gammaoverseer.abstractAI as OverseerAbstractAI).BringToRoomAndGuidePlayer(self.room.abstractRoom.index);
                                if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Gamma settings incorrect, fixed them and called Gamma!"); }
                            }
                        }
                    }

                    if (self.room.game.GetStorySession != null && self.room.game.GetStorySession.saveState != null &&
                        self.room.game.GetStorySession.saveState.miscWorldSaveData != null)
                    {
                        if (self.room.game.GetStorySession != null && self.room.game.GetStorySession.saveState != null &&
                        self.room.game.GetStorySession.saveState.miscWorldSaveData != null &&
                        self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                        {
                            self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = false;
                            self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                            self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                            if (self.GetNCRunbound().MoreDebug)
                            {
                                NCRDebug.Log("Old save detected, fixing game- moon has been re-killed! Sorry, women");
                            }
                        }



                        if (self.room.game.GetStorySession.saveState.cycleNumber != 0 &&
                        self.room.game.AllPlayersRealized && !self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon &&
                        self.room.game.Players != null && self.room.game.Players.Count > 0)
                        {
                            NCRDebug.Log("Unbound's death persistent save data fucked up! Attempting to fix it...");
                            self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                        }

                        if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles == true)
                        {
                            self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                            if (self.GetNCRunbound().MoreDebug)
                            {
                                NCRDebug.Log("Pebbles was killed for some reason and has been revived");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Unbound your room specific shit's being an ass again: " + e);
                }
                // end unbound worldstate things
            }

            // misc
            try
            {
                if (self != null && self.room != null && self.abstractCreature != null &&
                    self.slugcatStats != null && self.slugcatStats.name != null &&
                    self.slugcatStats.name.value == "NCRunbound")
                {
                    self.GetNCRunbound().IsUnbound = true;
                    self.GetNCRunbound().IsNCRUnbModcat = true;
                    if (self.slugcatStats.name != UnboundEnums.NCRUnbound) { self.slugcatStats.name = UnboundEnums.NCRUnbound; }
                }
                if (self != null && self.room != null && self.abstractCreature != null &&
                    self.slugcatStats != null && self.slugcatStats.name != null &&
                self.slugcatStats.name.value == "NCRoracle")
                {
                    self.GetNCRunbound().IsOracle = true;
                    self.GetNCRunbound().IsNCRUnbModcat = true;
                    if (self.slugcatStats.name != UnboundEnums.NCROracle) { self.slugcatStats.name = UnboundEnums.NCROracle; }
                }
                if (self != null && self.room != null && self.abstractCreature != null &&
                    self.slugcatStats != null && self.slugcatStats.name != null &&
                self.slugcatStats.name.value == "NCRreverb")
                {
                    if (!self.playerState.isPup)
                    {
                        self.setPupStatus(true);
                    }
                    self.GetNCRunbound().Reverb = true;
                    self.GetNCRunbound().IsNCRUnbModcat = true;
                    if (self.slugcatStats.name != UnboundEnums.NCRReverb) { self.slugcatStats.name = UnboundEnums.NCRReverb; }
                }
                if (self != null && self.room != null && self.abstractCreature != null &&
                    self.slugcatStats != null && self.slugcatStats.name != null &&
                    self.slugcatStats.name.value == "NCRtech")
                {
                    self.GetNCRunbound().IsTechnician = true;
                    self.GetNCRunbound().IsNCRUnbModcat = true;
                    if (self.slugcatStats.name != UnboundEnums.NCRTechnician) { self.slugcatStats.name = UnboundEnums.NCRTechnician; }
                }

            }
            catch (Exception e)
            {
                NCRDebug.Log("The character setup for Unbound is fucking up: " + e);
            }
        }

        public static void CustomKarmaGates(On.RegionGate.orig_customKarmaGateRequirements orig, RegionGate self)
        {
            if (self != null && self.room != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.abstractRoom.name == "GATE_SL_MS")
                {
                    self.karmaRequirements[1] = RegionGate.GateRequirement.OneKarma; // karma requirement to leave submerged and enter shoreline
                }
                else if (self.room.abstractRoom.name == "GATE_SB_OE")
                {
                    self.karmaRequirements[0] = RegionGate.GateRequirement.OneKarma; // karma requirement to leave outer expanse and enter subter
                }
            }
            orig(self);
        }

        public static void UnboundFirstBootup(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            try
            {
                if (self != null && self.room != null && self.room.game != null && self.abstractCreature != null &&
                world != null && world.region != null && world.region.name != null &&
                abstractCreature != null && self.room.game.session.characterStats.name.value == "NCRunbound" &&
                self.room.game.session is not ArenaGameSession && (!ModManager.MSC || !self.room.game.rainWorld.safariMode) &&
                self.room.game.GetStorySession != null && self.room.game.GetStorySession.saveState != null &&
                self.room.game.GetStorySession.saveState.cycleNumber == 0 &&
                self.room.game.AllPlayersRealized && !self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon &&
                self.room.game.Players.Count > 0
                )
            {
                    if (!ModManager.Expedition || !Custom.rainWorld.ExpeditionMode)
                    {
                        if (world.region.name == "MS" && ModManager.MSC)
                        {
                            NCRDebug.Log("Unbound's MS start detected, triggering intro!");
                            self.room.AddObject(new UnboundIntro());
                        }
                        else if (world.region.name == "SL")
                        {
                            NCRDebug.Log("Unbound's SL start detected! Remind me to set up a non-MSC intro :<");
                            self.objectInStomach = new DataPearl.AbstractDataPearl(self.room.world,
                                AbstractPhysicalObject.AbstractObjectType.DataPearl, null,
                                new WorldCoordinate(self.room.abstractRoom.index, -1, -1, 0), self.room.game.GetNewID(), -1, -1, null,
                                UnboundEnums.unboundKarmaPearl);
                        }
                    }
                    else
                    {
                        if (self.room.abstractRoom.name == "SU_A12")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(32, 21));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(32, 22));
                        }
                        if (self.room.abstractRoom.name == "SU_A40")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(47, 30));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(46, 30));
                        }
                        if (self.room.abstractRoom.name == "SS_LAB14")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(24, 17));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(25, 18));
                        }
                        if (self.room.abstractRoom.name == "LF_A03")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(15, 29));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(14, 29));
                        }
                        if (self.room.abstractRoom.name == "LF_A17")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(15, 14));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(14, 14));
                        }
                        if (self.room.abstractRoom.name == "UW_A05")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(35, 27));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(36, 27));
                        }
                        if (self.room.abstractRoom.name == "SL_LMS06")
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(14, 26));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(13, 26));
                        }

                        if (self.room.abstractRoom.name == "MS_CORE" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(23, 11));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(23, 12));
                        }
                        if (self.room.abstractRoom.name == "MS_AIR01" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(34, 13));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(33, 13));
                        }
                        if (self.room.abstractRoom.name == "MS_bitteraerie6" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(148, 32));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(147, 32));
                        }
                        if (self.room.abstractRoom.name == "MS_bitteraerie5" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(76, 36));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(75, 36));
                        }
                        if (self.room.abstractRoom.name == "MS_DMU04" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(32, 35));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(33, 35));
                        }
                        if (self.room.abstractRoom.name == "SL_WALL06" && ModManager.MSC)
                        {
                            self.bodyChunks[0].HardSetPosition(self.room.MiddleOfTile(35, 45));
                            self.bodyChunks[1].HardSetPosition(self.room.MiddleOfTile(36, 45));
                        }
                    }

                    if (self.room.world.overseersWorldAI.playerGuide != null && self.room.world.overseersWorldAI != null &&
                        self.room.world.overseersWorldAI.playerGuide.creatureTemplate != null)
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
                            NCRDebug.Log("Gamma missing! Error: " + e);
                        }
                    }

                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    if (self.GetNCRunbound().MoreDebug)
                    {
                        NCRDebug.Log("Unbound start detected! This SHOULD trigger regardless of the cat being actively played, and only trigger once!");
                    }
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("The intro is fucking up: " + e);
            }
        }

        public static void BrokenAntiGravityctor(On.AntiGravity.BrokenAntiGravity.orig_ctor orig, AntiGravity.BrokenAntiGravity self, int cycleMin, int cycleMax, RainWorldGame game)
        {
            orig(self, cycleMin, cycleMax, game);
            if (self != null && self.game != null && self.game.world != null &&
                self.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.game.world.name == "MS" || self.game.world.name == "SL")
                {
                    if (self.game.IsStorySession)
                    {
                        self.on = (UnityEngine.Random.value < 0.4f - (float)(self.game.GetStorySession.saveState.totTime / 6000) -
                        // this should make it LESS likely to be on the longer the playtime is
                        ((self.game.world.name == "SL") ? 0.3f : 0f)
                        );
                    }
                }
                self.from = (self.on ? 1f : 0f);
                self.to = (self.on ? 1f : 0f);
                self.lights = self.to;
            }
        }

        public static void BrokenGravUpdate(On.AntiGravity.BrokenAntiGravity.orig_Update orig, AntiGravity.BrokenAntiGravity self)
        {
            if (self != null && self.game != null && self.game.world != null &&
                self.game.session.characterStats.name.value == "NCRunbound")
            {
                self.counter--;

                if (self.counter < 1)
                {
                    self.on = !self.on;
                    self.counter = UnityEngine.Random.Range(self.cycleMin * 40, self.cycleMax * 40);
                    self.from = self.to;
                    self.to = (self.on ? 1f : 0f);
                    self.progress = 0f;
                    for (int i = 0; i < self.game.cameras.Length; i++)
                    {
                        if (self.game.cameras[i].room != null &&
                            self.game.cameras[i].room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
                        {
                            self.game.cameras[i].room.PlaySound(self.on ? SoundID.Broken_Anti_Gravity_Switch_On : SoundID.Broken_Anti_Gravity_Switch_Off,
                                0f, // pan
                                self.game.cameras[i].room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG), // volume
                                0.96f // pitch
                                );
                        }
                    }
                }

                if (self.progress < 1f)
                {
                    self.progress = Mathf.Min(1f, self.progress + 0.008333334f);
                    if (UnityEngine.Random.value < 0.125f)
                    {
                        self.lightsGetTo = Mathf.Lerp(self.from, self.to, Mathf.Pow(UnityEngine.Random.value *
                            Mathf.Pow(Mathf.InverseLerp(0f, 0.5f, self.progress), 0.5f), Custom.LerpMap(self.progress, 0f, 0.6f, 1f, 0f)));
                    }
                }
                else
                {
                    self.lightsGetTo = self.to;
                }

                self.lights = Custom.LerpAndTick(self.lights, self.lightsGetTo, 0.15f, 0.00083333335f);

                if (self.progress > 0f && self.progress < 1f)
                {
                    for (int j = 0; j < self.game.cameras.Length; j++)
                    {
                        if (self.game.cameras[j].room != null &&
                            self.game.cameras[j].room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
                        {
                            self.game.cameras[j].room.ScreenMovement(null, new Vector2(0f, 0f),
                                self.game.cameras[j].room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) *
                                0.5f * Mathf.Sin(self.progress * 3.1415927f));
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }
        }
        // end srs
    }
}
