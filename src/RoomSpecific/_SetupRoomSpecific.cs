using System;

namespace Unbound
{
    internal class SetupRoomSpecific
    {

        public static void Init()
        {
            On.Player.ctor += Initial;
            // setup and intro
            On.RegionGate.customKarmaGateRequirements += CustomKarmaGates;
            // custom gate tweaks- allows for exiting MS

            On.AntiGravity.BrokenAntiGravity.Update += BrokenUpdate;
            On.AntiGravity.BrokenAntiGravity.ctor += BrokenAntiGravityctor;
            // antigravity scripts

            On.RoomSpecificScript.AddRoomSpecificScript += RoomSpecificScripts;

            On.ZapCoil.InitiateSprites += RedZap;
            On.ZapCoilLight.ctor += RedLight;
            On.ZapCoil.DrawSprites += DrawRedzap;
        }

        private static void DrawRedzap(On.ZapCoil.orig_DrawSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (self != null && self.room != null && self.room.world != null && !self.slatedForDeletetion && self.room == rCam.room &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER" || self.room.world.name == "STG")
                {
                    float num = Mathf.Lerp(self.lastTurnedOn, self.turnedOn, timeStacker);
                    sLeaser.sprites[0].alpha = num;
                    Vector2 a = new Vector2((float)self.rect.left * 20f, (float)self.rect.bottom * 20f);
                    Vector2 a2 = new Vector2((float)(self.rect.right + 1) * 20f, (float)(self.rect.top + 1) * 20f);
                    Vector2 a3 = new Vector2((float)self.rect.left * 20f, (float)(self.rect.top + 1) * 20f);
                    Vector2 a4 = new Vector2((float)(self.rect.right + 1) * 20f, (float)self.rect.bottom * 20f);
                    float num2 = 120f * num;
                    float num3 = 30f;
                    float num4 = Mathf.Lerp(self.flicker[0, 1], self.flicker[0, 0], timeStacker);
                    float num5 = Mathf.Lerp(self.flicker[1, 1], self.flicker[1, 0], timeStacker);
                    if (self.horizontalAlignment)
                    {
                        a.x -= num3;
                        a3.x -= num3;
                        a2.x += num3;
                        a4.x += num3;
                        a.y -= num2 * num4;
                        a4.y -= num2 * num5;
                        a3.y += num2 * num4;
                        a2.y += num2 * num5;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a3 + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a4 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a2 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a4 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a2 - camPos);
                    }
                    else
                    {
                        a.x -= num2 * num4;
                        a3.x -= num2 * num5;
                        a2.x += num2 * num5;
                        a4.x += num2 * num4;
                        a.y -= num3;
                        a4.y -= num3;
                        a3.y += num3;
                        a2.y += num3;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a2 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a3 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a2 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a4 + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a4 - camPos);
                    }
                    sLeaser.sprites[0].color = new Color(1f, Mathf.InverseLerp(0f, 0.5f, self.zapLit) * num, Mathf.InverseLerp(0f, 0.5f, self.zapLit) * num);
                }
                else
                {
                    orig(self, sLeaser, rCam, timeStacker, camPos);
                }
            }
            else
            {
                orig(self, sLeaser, rCam, timeStacker, camPos);
            }
        }

        private static void RedLight(On.ZapCoilLight.orig_ctor orig, ZapCoilLight self, Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER" || self.room.world.name == "STG")
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(1f, 0f, 0f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, (float)lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                }
                else if (self.room.world.name == "MS")
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(0f, 0f, 1f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, (float)lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                }
                else
                {
                    orig(self, placedInRoom, placedObject, lightData);
                }
            }
            else
            {
                orig(self, placedInRoom, placedObject, lightData);
            }
        }

        private static void RedZap(On.ZapCoil.orig_InitiateSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER")
                {
                    TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[6];
                    for (int i = 0; i < 6; i++)
                    {
                        array[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
                    }
                    TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
                    float num = 0.4f;
                    triangleMesh.UVvertices[0] = new Vector2(0f, 0f);
                    triangleMesh.UVvertices[1] = new Vector2(1f, 0f);
                    triangleMesh.UVvertices[2] = new Vector2(0f, num);
                    triangleMesh.UVvertices[3] = new Vector2(1f, num);
                    triangleMesh.UVvertices[4] = new Vector2(0f, 1f - num);
                    triangleMesh.UVvertices[5] = new Vector2(1f, 1f - num);
                    triangleMesh.UVvertices[6] = new Vector2(0f, 1f);
                    triangleMesh.UVvertices[7] = new Vector2(1f, 1f);
                    sLeaser.sprites = new FSprite[1];
                    sLeaser.sprites[0] = triangleMesh;
                    sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
                    sLeaser.sprites[0].color = new Color(1f, 0f, 0f);
                    self.AddToContainer(sLeaser, rCam, null);
                }
                else
                {
                    orig(self, sLeaser, rCam);
                }
            }
            else
            {
                orig(self, sLeaser, rCam);
            }
        }

        private static void RoomSpecificScripts(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
        {
            if (room != null && room.game != null && room.game.session is StoryGameSession && room.world != null &&
                room.game.session.characterStats.name.value == "NCRunbound")
            {
                orig(room);
                string name = room.abstractRoom.name;

                if (name == "deathPit")
                {
                    room.AddObject(new RoomSpecificScript.DeathPit(room));
                }

                #region Five Pebbles
                if (room.world.name == "SS")
                {
                    if (name == "SS_E08") // five pebbles gravity gradient room
                    {
                        room.AddObject(new RoomSpecificScript.SS_E08GradientGravity(room));
                    }
                }
                #endregion
                #region Subterranean
                else if (room.world.name == "SB")
                {
                    if (name == "SB_A14") // sub ten karma symbol
                    {
                        room.AddObject(new RoomSpecificScript.SB_A14KarmaIncrease(room));
                    }
                }
                #endregion
                #region Shoreline
                else if (room.world.name == "SL")
                {
                    if (name == "SL_WALL06") //
                    {
                        // room.AddObject(new UnbSLWall06(room, rsplayer));
                    }
                }
                #endregion
                #region Submerged Superstructure
                else if (room.world.name == "MS")
                {
                    
                }
                #endregion
            }
            else
            {
                orig(room);
            }
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

        private static void Initial(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self != null && self.room != null && self.room.game != null && self.abstractCreature != null && world != null &&
                world.region != null && world.region.name != null && self.room.game.GetStorySession != null && abstractCreature != null &&

                self.room.game.session.characterStats.name.value == "NCRunbound" && self.room.game.session is StoryGameSession)
            {
                AbstractCreature gammaoverseer = self.room.world.overseersWorldAI.playerGuide;
                try
                {
                    if (!self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon)
                    {
                        if (world.region.name == "MS" && ModManager.MSC)
                        {
                            NCRDebug.Log("Unbound's MS start detected, triggering intro!");
                            self.room.AddObject(new UnboundIntro());
                        }
                        else if (world.region.name == "SL" && !ModManager.MSC)
                        {
                            NCRDebug.Log("Unbound's SL start detected! Remind me to set up a non-MSC intro :<");
                            self.objectInStomach = new DataPearl.AbstractDataPearl(self.room.world,
                                AbstractPhysicalObject.AbstractObjectType.DataPearl, null,
                                new WorldCoordinate(self.room.abstractRoom.index, -1, -1, 0), self.room.game.GetNewID(), -1, -1, null,
                                UnboundEnums.unboundKarmaPearl);
                        }

                        if (self.room.world.overseersWorldAI.playerGuide != null && self.room.world.overseersWorldAI != null)
                        {
                            try
                            {
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
                    NCRDebug.Log("The intro is fucking up!" + e);
                }
                // THINGS FOR GAME SETUP BELOW ------------------------------------------------------------------------------------------------------------


                if (gammaoverseer.ignoreCycle != true)
                {
                    gammaoverseer.ignoreCycle = true;
                    gammaoverseer.creatureTemplate.waterVision = 0;
                    gammaoverseer.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;

                    (gammaoverseer.abstractAI as OverseerAbstractAI).BringToRoomAndGuidePlayer(self.room.abstractRoom.index);
                }

                if (self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = false;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    if (self.GetNCRunbound().MoreDebug)
                    {
                        NCRDebug.Log("Old save detected, fixing game- moon has been re-killed! Sorry, women");
                    }
                }
                if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles == true)
                {
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    if (self.GetNCRunbound().MoreDebug)
                    {
                        NCRDebug.Log("Pebbles was killed for some reason and has been revived");
                    }
                }
                // end unbound worldstate things
            }

            // misc
            if (self != null && self.room != null && self.abstractCreature != null &&
                self.slugcatStats.name.value == "NCRunbound")
            {
                try
                {
                    self.GetNCRunbound().IsUnbound = true;

                    if (self.abstractCreature.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] != 1.5f)
                    {
                        self.abstractCreature.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;
                    }

                    if (self.room.game.session.characterStats.name.value == "NCRunbound" && self.room.game.session is StoryGameSession &&
                        self.glowing == false)
                    {
                        self.glowing = true;
                    }
                }
                catch (Exception e)
                {
                    NCRDebug.Log("The character setup for Unbound is fucking up!" + e);
                }
            }
            else if (self != null && self.room != null &&
                self.slugcatStats.name.value == "NCRtech")
            {
                self.GetNCRunbound().IsTechnician = true;
                self.abstractCreature.creatureTemplate.damageRestistances[(int)Creature.DamageType.Electric, 0] = 1.5f;
            }
        }

        private static void BrokenAntiGravityctor(On.AntiGravity.BrokenAntiGravity.orig_ctor orig, AntiGravity.BrokenAntiGravity self, int cycleMin, int cycleMax, RainWorldGame game)
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

        private static void BrokenUpdate(On.AntiGravity.BrokenAntiGravity.orig_Update orig, AntiGravity.BrokenAntiGravity self)
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
