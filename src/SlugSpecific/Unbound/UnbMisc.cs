using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using DressMySlugcat;

namespace Unbound
{
    public class UnbMisc
    {
        public static void Init()
        {
            On.Player.UpdateMSC += unboundSpecialButton;
        }

        public static void unboundSpecialButton(On.Player.orig_UpdateMSC orig, Player self)
        {
            orig(self);
            if (self?.room?.game != null &&
                !self.submerged && self.Consious &&
                // not submerged, awake / non-stunned
                self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound
                )
            {
                if (ModManager.JollyCoop && self.playerState.playerNumber == 1 && self.room.game.session.characterStats.name == Watcher.WatcherEnums.SlugcatStatsName.Watcher)
                {

                }
                #region NotInWatcherAbilities
                if (ModManager.Watcher &&
                    (self.input[0].spec && !self.input[1].spec) &&
                    (!self.input[0].pckp && !self.input[0].jmp && !self.input[0].thrw) &&
                    (self.input[0].x == 0 && self.input[0].y < 0) && // must be crouching
                    self.room.abstractRoom.shelter && // only doable in shelters
                    self.GetNCRunbound().pearlBeingRead == null &&
                    (self.grasps[0].grabbed is DataPearl || self.grasps[1].grabbed is DataPearl)
                    )
                {
                    if (self.grasps[0] != null && self.grasps[0].grabbed is DataPearl)
                    {
                        self.GetNCRunbound().pearlInPaws = (self.grasps[0].grabbed as DataPearl);
                    }
                    if (self.grasps[1] != null && self.grasps[1].grabbed is DataPearl)
                    {
                        self.GetNCRunbound().pearlInPaws = (self.grasps[1].grabbed as DataPearl);
                    }
                    self.GetNCRunbound().pearlBeingRead = Watcher.PearlContent.Load(self.GetNCRunbound().pearlInPaws.AbstractPearl);
                    self.GetNCRunbound().pearlBeingRead.PlayInRoom(self.room, self.mainBodyChunk.pos);

                    // this triggers once
                }
                else if (self.GetNCRunbound().pearlBeingRead != null)
                {
                    // this is checked multiple times
                    if (!self.GetNCRunbound().pearlBeingRead.life.isFinished && !self.Stunned)
                    {
                        self.Stun(2);
                    }
                    else
                    {
                        self.GetNCRunbound().pearlBeingRead = null;
                        self.GetNCRunbound().pearlInPaws = null;
                    }
                }
            }
            // watcher-exclusive special effect

            if (self?.room?.game != null &&
                self.GetNCRunbound().CryCooldown <= 0 &&
                !self.submerged && self.Consious &&
                // not submerged, awake / non-stunned
                self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound &&
                (self.input[0].spec && !self.input[1].spec)
                )
            {
                int random = UnityEngine.Random.Range(1, 4);
                self.room.PlaySound(ModManager.MMF ?
                    (random == 1 ? MMFEnums.MMFSoundID.Lizard_Voice_Cyan_A : (
                    random == 2 ? MMFEnums.MMFSoundID.Lizard_Voice_Cyan_B :
                    MMFEnums.MMFSoundID.Lizard_Voice_Cyan_C)) :
                    SoundID.Lizard_Voice_Green_A,
                    self.mainBodyChunk, false, 0.8f,
                    ModManager.MMF ? UnityEngine.Random.Range(2f, 2.8f) :
                    UnityEngine.Random.Range(1.8f, 2f));
                self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 100f, self, 0.5f));
                self.GetNCRunbound().CryCooldown += 60;
                self.eyesClosedTime = 60;
                if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Unbound call!"); }
                self.room.AddObject(new DisciplePing(self, self.mainBodyChunk.pos, 0f, 0.2f, 0.2f, 20));

                if (self?.room.world.overseersWorldAI != null &&
                    self.room.world.overseersWorldAI.playerGuide != null &&
                    self.room.world.overseersWorldAI.playerGuide.realizedCreature != null)
                {
                    AbstractCreature gammaoverseer = self.room.world.overseersWorldAI.playerGuide;
                    if (!gammaoverseer.realizedCreature.dead)
                    {
                        (gammaoverseer.abstractAI as OverseerAbstractAI).BringToRoomAndGuidePlayer(
                            self.room.abstractRoom.index);
                    }
                }
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    var critter = self.room.abstractRoom.creatures[i];
                    if (critter.realizedCreature != null && critter.realizedCreature.deaf == 0 &&
                        critter.realizedCreature.Consious)
                    {
                        if (ModManager.Watcher &&
                        (critter.creatureTemplate.type == Watcher.WatcherEnums.CreatureTemplateType.FireSprite))
                        {
                            (critter.abstractAI.RealAI as Watcher.FireSpriteAI).pathFinder.SetDestination(self.coord);
                        }
                    }
                }
            }
            #endregion
        }

        public static void CycleTick(On.CreatureCommunities.orig_CycleTick orig, CreatureCommunities self, int cycle, SlugcatStats.Name saveStateNumber)
        {
            if (saveStateNumber != null && saveStateNumber.value == "NCRunbound")
            {
                NCRDebug.Log("Unbound save cycletick!");
                if (cycle > 10 && self.scavengerShyness > 0f)
                {
                    self.scavengerShyness = Mathf.Max(0f, self.scavengerShyness - 0.02f);
                    // functionally, scavs are slower to stop being shy
                }

                for (int l = 0; l < self.playerOpinions.GetLength(0); l++)
                {
                    for (int m = 0; m < self.playerOpinions.GetLength(1); m++)
                    {
                        for (int n = 0; n < self.playerOpinions.GetLength(2); n++)
                        {
                            if (self.playerOpinions[l, m, n] < 0.85f)
                            {
                                self.playerOpinions[l, m, n] = Mathf.Min(0.25f, self.playerOpinions[l, m, n] - 0.001f);
                            }
                        }
                    }
                }
                return;
            }
            if (saveStateNumber != null && saveStateNumber.value == "NCRoracle")
            {
                if (self.scavengerShyness > 0f) { self.scavengerShyness = 0f; }
                return;
            }
            orig(self, cycle, saveStateNumber);
        }

        public static void shockMeLess(On.JellyFish.orig_Collide orig, JellyFish self, PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            if (self != null && otherObject != null &&
                otherObject is Creature && otherObject != self.thrownBy && self.Electric &&
                otherObject is Player && ((otherObject as Player).slugcatStats.name.value == "NCRunbound" ||
                (otherObject as Player).slugcatStats.name.value == "NCRtech"))
            {
                bool isTech = (otherObject as Player).slugcatStats.name.value == "NCRtech";
                (otherObject as Creature).Violence(self.firstChunk, new Vector2?(Custom.DirVec(self.firstChunk.pos,
                    otherObject.bodyChunks[otherChunk].pos) * 5f), otherObject.bodyChunks[otherChunk], null,
                    Creature.DamageType.Electric, 0.1f, isTech ? 30f : 70f);
                self.room.AddObject(new CreatureSpasmer(otherObject as Creature, false, (otherObject as Creature).stun));
                self.room.PlaySound(SoundID.Jelly_Fish_Tentacle_Stun, self.firstChunk.pos);
                self.room.AddObject(new Explosion.ExplosionLight(self.firstChunk.pos, 200f, 1f, 4, new Color(0.7f, 1f, 1f)));
                if (self.electricCounter > 5)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 vector = Custom.DegToVec(360f * UnityEngine.Random.value);
                        self.room.AddObject(new MouseSpark(self.firstChunk.pos + vector * 9f, self.firstChunk.vel + vector * 36f *
                            UnityEngine.Random.value, 20f, new Color(0.7f, 1f, 1f)));
                    }
                }
                self.electricCounter = Math.Min(self.electricCounter, 5);
            }
            else
            {
                orig(self, otherObject, myChunk, otherChunk);
            }
        }

        public static void NeuronColourShift(On.SSOracleSwarmer.orig_DrawSprites orig, SSOracleSwarmer self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self?.room?.game != null)
            {
                foreach (AbstractCreature otherCreature in self.room.abstractRoom.creatures)
                {
                    if (otherCreature?.realizedCreature?.room != null &&
                        otherCreature.realizedCreature is Player &&
                        (otherCreature.realizedCreature as Player).GetNCRunbound().IsUnbound)
                    {
                        if (otherCreature.realizedCreature.room != self.room)
                        {
                            if ((otherCreature.realizedCreature as Player).GetNCRunbound().dontForceChangeEffectCol)
                            {
                                (otherCreature.realizedCreature as Player).GetNCRunbound().dontForceChangeEffectCol = false;
                                (otherCreature.realizedCreature as Player).GetNCRunbound().recheckColour = true;
                            }
                            return;
                        }
                        if (!(otherCreature.realizedCreature as Player).GetNCRunbound().dontForceChangeEffectCol)
                        {
                            (otherCreature.realizedCreature as Player).GetNCRunbound().dontForceChangeEffectCol = true;
                        }
                        if ((otherCreature.realizedCreature as Player).GetNCRunbound().RGBRings)
                        {
                            Color color;
                            color = (otherCreature.realizedCreature as Player).GetNCRunbound().effectColour;
                            sLeaser.sprites[4].color = (otherCreature.realizedCreature as Player).GetNCRunbound().effectColour;
                            for (int i = 0; i < 4; i++)
                            {
                                sLeaser.sprites[i].color = color;
                            }
                        }
                        else
                        {
                            (otherCreature.realizedCreature as Player).GetNCRunbound().effectColour = sLeaser.sprites[2].color;
                        }
                    }
                }
            }
        }

        public static void UpdateTheGlow(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            if (self?.player?.room?.game?.session != null && self.player.mainBodyChunk != null &&
                self.player.GetNCRunbound().effectColour != null &&

                !self.player.room.game.IsArenaSession && self.player.room.game.IsStorySession &&
                self.player.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician) &&
                !self.player.DreamState &&
                 self.player.room.world.name != "KTB"
                )
            {
                if (self.lightSource != null)
                {
                    self.lightSource.color = Color.Lerp(new Color(1f, 1f, 1f), self.player.GetNCRunbound().effectColour, 0.75f);
                    self.lightSource.stayAlive = true;
                    self.lightSource.setPos = new Vector2?(self.player.mainBodyChunk.pos);
                    if (self.lightSource.slatedForDeletetion)
                    {
                        self.lightSource = null;
                    }
                }
                else
                {
                    self.lightSource = new LightSource(self.player.mainBodyChunk.pos, false, Color.Lerp(new Color(1f, 1f, 1f),
                    self.player.GetNCRunbound().effectColour, 0.75f), self.player);
                    self.lightSource.requireUpKeep = true;
                    self.lightSource.setRad = new float?(250f);
                    self.lightSource.setAlpha = new float?(0.97f);
                    self.player.room.AddObject(self.lightSource);
                }
            }
            orig(self);
        }

        public static void noGlow(On.OracleSwarmer.orig_BitByPlayer orig, OracleSwarmer self, Creature.Grasp grasp, bool eu)
        {
            if (self != null && grasp?.grabber != null && grasp.grabber is Player &&
                (!ModManager.MSC || !(grasp.grabber as Player).isNPC) &&
                ((grasp.grabber as Player).GetNCRunbound().IsUnbound || (grasp.grabber as Player).GetNCRunbound().IsTechnician))
            {
                self.bites--;
                self.room.PlaySound(self.bites == 0 ? SoundID.Slugcat_Eat_Swarmer : SoundID.Slugcat_Bite_Swarmer, self.firstChunk.pos);
                self.firstChunk.MoveFromOutsideMyUpdate(eu, grasp.grabber.mainBodyChunk.pos);
                if (self.bites < 1)
                {
                    (grasp.grabber as Player).ObjectEaten(self);
                    grasp.Release();
                    self.Destroy();
                }
            }
            else
            {
                orig(self, grasp, eu);
            }
        }

        public static void BiteUnb(On.Lizard.orig_DamageAttack orig, Lizard self, BodyChunk chunk, float dmgFac)
        {
            if (chunk?.owner != null && self?.AI != null &&
                chunk.owner is Creature && (chunk.owner is Player) &&
                (chunk.owner as Player).GetNCRunbound().IsUnbound &&
                self.AI.DynamicRelationship((chunk.owner as Creature).abstractCreature).type == CreatureTemplate.Relationship.Type.AgressiveRival)
            {
                (chunk.owner as Player).playerState.permanentDamageTracking = (dmgFac / 10f);
            }
            orig(self, chunk, dmgFac);
        }

        public static void DamageTracking(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self?.room != null && self.abstractCreature != null &&
                self.GetNCRunbound().RGBRings)
            {
                self.GetNCRunbound().RGBCounter++;
            }
            orig(self, eu);
            if (self?.room != null && self.abstractCreature != null &&
                (self.GetNCRunbound().IsUnbound || self.GetNCRunbound().IsTechnician) && self.Wounded)
            {
                if (UnityEngine.Random.value < Mathf.Lerp(0.004f, 0.02f, (float)(self.State as PlayerState).permanentDamageTracking))
                {
                    if (self.GetNCRunbound().damagesmoke == null)
                    {
                        self.GetNCRunbound().damagesmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetNCRunbound().damagesmoke);
                    }
                    self.GetNCRunbound().damagesmoke.EmitSmoke(self.firstChunk.pos, Custom.RNV(), true, 30f);
                }
                self.Blink(100);
            }
        }

        public static void unbZapped(On.ZapCoil.orig_Update orig, ZapCoil self, bool eu)
        {
            if (self?.room != null && !self.slatedForDeletetion &&
                (self.room.world.game.session.characterStats.name.value == "NCRunbound" ||
                self.room.world.game.session.characterStats.name.value == "NCRtech"))
            {
                #region PreUnb
                self.evenUpdate = eu;
                self.soundLoop.Update();
                self.disruptedLoop.Update();
                if (self.turnedOn > 0.5f)
                {
                    for (int i = 0; i < self.room.physicalObjects.Length; i++)
                    {
                        for (int j = 0; j < self.room.physicalObjects[i].Count; j++)
                        {
                            for (int k = 0; k < self.room.physicalObjects[i][j].bodyChunks.Length; k++)
                            {
                                if (self.horizontalAlignment && self.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.y != 0 ||
                                    !self.horizontalAlignment && self.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.x != 0)
                                {
                                    Vector2 a = self.room.physicalObjects[i][j].bodyChunks[k].ContactPoint.ToVector2();
                                    Vector2 v = self.room.physicalObjects[i][j].bodyChunks[k].pos + a *
                                        (self.room.physicalObjects[i][j].bodyChunks[k].rad + 30f);
                                    if (self.GetFloatRect.Vector2Inside(v))
                                    {
                                        self.TriggerZap(self.room.physicalObjects[i][j].bodyChunks[k].pos + a *
                                            self.room.physicalObjects[i][j].bodyChunks[k].rad, self.room.physicalObjects[i][j].bodyChunks[k].rad);
                                        self.room.physicalObjects[i][j].bodyChunks[k].vel -= (a * 6f + Custom.RNV() *
                                            UnityEngine.Random.value) / self.room.physicalObjects[i][j].bodyChunks[k].mass;
                                        if (self.room.physicalObjects[i][j] is Creature)
                                        {
                                            #endregion
                                            if (self.room.physicalObjects[i][j] is Player &&
                                                ((self.room.physicalObjects[i][j] as Player).GetNCRunbound().IsUnbound ||
                                                (self.room.physicalObjects[i][j] as Player).GetNCRunbound().IsTechnician))
                                            {
                                                (self.room.physicalObjects[i][j] as Player).Stun(200);
                                                (self.room.physicalObjects[i][j] as Player).room.AddObject(new
                                                    CreatureSpasmer(self.room.physicalObjects[i][j] as Player, true, 200));
                                                (self.room.physicalObjects[i][j] as Player).playerState.permanentDamageTracking +=
                                                    (self.room.physicalObjects[i][j] as Player).GetNCRunbound().IsUnbound ? 0.95f : 0.4f;

                                                if ((self.room.physicalObjects[i][j] as Player).playerState.permanentDamageTracking >= 1)
                                                {
                                                    (self.room.physicalObjects[i][j] as Player).Die();
                                                }

                                                self.room.physicalObjects[i][j].room.AddObject(new ShockWave((self.room.physicalObjects[i][j] as Player).firstChunk.pos,
                                                    (self.room.physicalObjects[i][j] as Player).dead ? UnityEngine.Random.Range(30, 140) : UnityEngine.Random.Range(20, 80),
                                                    0.08f, 7, false));

                                                self.room.physicalObjects[i][j].room.PlaySound(SoundID.Overseer_Death,
                                                    (self.room.physicalObjects[i][j] as Player).mainBodyChunk.pos,
                                                    (self.room.physicalObjects[i][j] as Player).dead ? 0.6f : 0.4f, // volume
                                                    (self.room.physicalObjects[i][j] as Player).dead ? 0.8f : 1f //pitch
                                                    );
                                            }
                                            else
                                            {
                                                (self.room.physicalObjects[i][j] as Creature).Die();
                                            }
                                            #region PostUnb
                                        }
                                        if (ModManager.MSC && self.room.physicalObjects[i][j] is ElectricSpear)
                                        {
                                            (self.room.physicalObjects[i][j] as ElectricSpear).Recharge();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                self.lastTurnedOn = self.turnedOn;
                if (UnityEngine.Random.value < 0.005f)
                {
                    self.disruption = Mathf.Max(self.disruption, UnityEngine.Random.value);
                }
                self.disruption = Mathf.Max(0f, self.disruption - 1f / Mathf.Lerp(70f, 300f, UnityEngine.Random.value));
                self.smoothDisruption = Mathf.Lerp(self.smoothDisruption, self.disruption, 0.2f);
                float num = Mathf.InverseLerp(0.1f, 1f, self.smoothDisruption);
                self.soundLoop.Volume = (1f - num) * self.turnedOn;
                self.disruptedLoop.Volume = num * Mathf.Pow(self.turnedOn, 0.2f);

                for (int l = 0; l < self.flicker.GetLength(0); l++)
                {
                    self.flicker[l, 1] = self.flicker[l, 0];
                    self.flicker[l, 3] = Mathf.Clamp(self.flicker[l, 3] + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) / 10f, 0f, 1f);
                    self.flicker[l, 2] += 1f / Mathf.Lerp(70f, 20f, self.flicker[l, 3]);
                    self.flicker[l, 0] = Mathf.Clamp(0.5f + self.smoothDisruption * (Mathf.Lerp(0.2f, 0.1f, self.flicker[l, 3]) * Mathf.Sin(6.2831855f *
                        self.flicker[l, 2]) + Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) / 20f), 0f, 1f);
                }

                if (UnityEngine.Random.value < self.disruption && UnityEngine.Random.value < 0.0025f)
                {
                    self.turnedOffCounter = UnityEngine.Random.Range(10, 100);
                }
                if (!self.powered)
                {
                    self.turnedOn = Mathf.Max(0f, self.turnedOn - 0.1f);
                }
                if (self.turnedOffCounter > 0)
                {
                    self.turnedOffCounter--;
                    if (UnityEngine.Random.value < 0.5f || UnityEngine.Random.value > self.disruption || !self.powered)
                    {
                        self.turnedOn = 0f;
                    }
                    else
                    {
                        self.turnedOn = UnityEngine.Random.value;
                    }

                    if (self.powered)
                    {
                        self.turnedOn = Mathf.Lerp(self.turnedOn, 1f, self.zapLit * UnityEngine.Random.value);
                    }

                    self.smoothDisruption = 1f;
                }
                else if (self.powered)
                {
                    self.turnedOn = Mathf.Min(self.turnedOn + UnityEngine.Random.value / 30f, 1f);
                }
                self.zapLit = Mathf.Max(0f, self.zapLit - 0.1f);
                if (self.room.fullyLoaded)
                {
                    self.disruption = Mathf.Max(self.disruption, self.room.gravity);
                }
                if (self.room.roomSettings.GetEffectAmount(RoomSettings.RoomEffect.Type.BrokenZeroG) > 0f)
                {
                    bool brokenGravityTurnedOff = self.room.world.rainCycle.brokenAntiGrav.to == 1f &&
                        self.room.world.rainCycle.brokenAntiGrav.progress == 1f;
                    if (!brokenGravityTurnedOff)
                    {
                        self.disruption = 1f;
                        if (self.powered && UnityEngine.Random.value < 0.2f)
                        {
                            self.powered = false;
                        }
                    }
                    if (brokenGravityTurnedOff && !self.powered && UnityEngine.Random.value < 0.025f)
                    {
                        self.powered = true;
                    }
                }
            }
            else { orig(self, eu); }
            #endregion
        }

        public static void ShockResistant(On.Centipede.orig_Shock orig, Centipede self, PhysicalObject shockObj)
        {
            if (self?.room != null && shockObj != null &&
                shockObj is Creature && shockObj is Player && ((shockObj as Player).GetNCRunbound().IsUnbound ||
                (shockObj as Player).GetNCRunbound().IsTechnician))
            {
                self.room.PlaySound(SoundID.Centipede_Shock, self.mainBodyChunk.pos);
                if (self.graphicsModule != null)
                {
                    (self.graphicsModule as CentipedeGraphics).lightFlash = 1f;
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, self.size); i++)
                    {
                        self.room.AddObject(new Spark(self.HeadChunk.pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value),
                            new Color(0.9f, 0.7f, 1f), null, 8, 14));
                    }
                }
                for (int j = 0; j < self.bodyChunks.Length; j++)
                {
                    self.bodyChunks[j].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    self.bodyChunks[j].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                for (int k = 0; k < shockObj.bodyChunks.Length; k++)
                {
                    shockObj.bodyChunks[k].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    shockObj.bodyChunks[k].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                if (shockObj is Creature)
                {
                    if (self.Small)
                    {
                        (shockObj as Creature).Stun(60);
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, false, (shockObj as Creature).stun));
                        (shockObj as Creature).LoseAllGrasps();
                    }
                    else if (self.Red)
                    {
                        (shockObj as Creature).Die();
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, 200));
                        (shockObj as Creature).LoseAllGrasps();
                    }
                    else if (shockObj.TotalMass < self.TotalMass)
                    {
                        (shockObj as Player).playerState.permanentDamageTracking += self.size;
                        if ((shockObj as Player).playerState.permanentDamageTracking >= 1)
                        {
                            (shockObj as Player).Die();
                        }

                        (shockObj as Creature).Stun((int)Custom.LerpMap(shockObj.TotalMass, 0f, self.TotalMass * 2f, 300f, 30f));
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, (shockObj as Creature).stun));

                        self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
                        self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
                        self.Stun((shockObj as Creature).stun + 3);
                        self.LoseAllGrasps();
                    }
                    else
                    {
                        (shockObj as Player).playerState.permanentDamageTracking += self.TotalMass - shockObj.TotalMass;
                        if ((shockObj as Player).playerState.permanentDamageTracking >= 1)
                        {
                            (shockObj as Player).Die();
                        }

                        (shockObj as Creature).Stun((int)Custom.LerpMap(shockObj.TotalMass, 0f, self.TotalMass * 2f, 300f, 30f));
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, (shockObj as Creature).stun));

                        self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
                        self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
                        self.Stun((shockObj as Creature).stun + 3);
                        self.LoseAllGrasps();
                    }
                }
                if (shockObj.Submersion > 0f)
                {
                    self.room.AddObject(new UnderwaterShock(self.room, self,
                        self.HeadChunk.pos, 14, Mathf.Lerp(ModManager.MMF ? 0f : 200f, 1200f, self.size),
                        0.2f + 1.9f * self.size, self, new Color(0.9f, 0.7f, 1f)));
                }
            }
            else
            {
                orig(self, shockObj);
            }
        }

        public static CreatureTemplate.Relationship TreatedAsCyan(On.LizardAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, LizardAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            // cyans consider unbound to be a cyan / are territorial rather than aggressive as long as hes alive
            // keeps them a bit more aggro than they are to one another BUT its not eating him so shrug
            if (self?.creature != null &&
                dRelation?.trackerRep?.representedCreature?.realizedCreature != null && dRelation.state != null &&
                // making sure things arent null
                self.creature.creatureTemplate.type == CreatureTemplate.Type.CyanLizard &&
                (((self.creature.realizedCreature as Lizard).rotModule == null) ||
                (self.creature.state as LizardState).rotType == LizardState.RotType.None ||
                (self.creature.state as LizardState).rotType == LizardState.RotType.Slight) &&
                // if cyan and not (too!) rotted. heavily rotted cyans still target him as normal
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                ((dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsUnbound ||
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsTechnician) &&
                // if unbound OR tech
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature
                // should still allow making friends with it
                )
            {
                if ((dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsTechnician)
                {
                    // technician
                    if (self.LikeOfPlayer(dRelation.trackerRep) > 0.8f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, self.LikeOfPlayer(dRelation.trackerRep));
                    }
                    else if (self.LikeOfPlayer(dRelation.trackerRep) < -0.95f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, -(self.LikeOfPlayer(dRelation.trackerRep)));
                    }
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 1f - (self.LikeOfPlayer(dRelation.trackerRep)));
                }
                else
                {
                    // is tha loser !!!!!!!!
                    if (self.LikeOfPlayer(dRelation.trackerRep) > 0.90f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, self.LikeOfPlayer(dRelation.trackerRep));
                    }
                    else if (self.LikeOfPlayer(dRelation.trackerRep) < -0.95f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, -(self.LikeOfPlayer(dRelation.trackerRep)));
                    }
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 1f - (self.LikeOfPlayer(dRelation.trackerRep)));
                }
            }
            return orig(self, dRelation);
        }

        public static Player.ObjectGrabability NoGrab(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self?.room != null && obj != null &&
                self.GetNCRunbound().IsUnbound)
            {
                if (obj is Creature && !(obj as Creature).dead && obj is Overseer && (obj as Overseer).PlayerGuide &&
                    self.room.game.session.characterStats.name.value == "NCRunbound")
                {
                    return Player.ObjectGrabability.CantGrab;
                    // cant grab gamma
                }
            }
            return orig(self, obj);
        }

        public static bool PickyBastard(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self?.room != null && testObj != null &&
                !self.GetNCRunbound().Unpicky &&
                self.GetNCRunbound().IsUnbound)
            {
                if (testObj is DataPearl &&
                    ((testObj as DataPearl).AbstractPearl.dataPearlType == UnboundEnums.unboundKarmaPearl ||
                    (testObj as DataPearl).AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Spearmasterpearl ||
                    (testObj as DataPearl).AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Red_stomach ||
                    (testObj as DataPearl).AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.Rivulet_stomach
                    ))
                {
                    return true;
                }
                return false;
            }
            else return orig(self, testObj);
        }

        public static void SwimspeedTweak(On.Player.orig_UpdateAnimation orig, Player self)
        {
            // swimming code
            orig(self);
            if (self?.room != null && (self.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.VoidSea) == null ||
                self.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.VoidSea) != null &&
                self.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.VoidSea).amount > 0f) &&
                self.GetNCRunbound().IsUnbound)
            {
                if (!self.submerged && !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious) && self.waterFriction >= 0.7f)
                {
                    self.waterFriction -= 0.05f;
                }
                else if (self.submerged && self.waterFriction >= 0.7f &&
                    !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious))
                {
                    self.waterFriction -= 0.025f;
                }
            }
        }

        public static bool KarmaUnderThreeGhost(On.GhostWorldPresence.orig_SpawnGhost orig, GhostWorldPresence.GhostID ghostID, int karma, int karmaCap, int ghostPreviouslyEncountered, bool playingAsRed)
        {
            if (ghostID != null &&
                (Custom.rainWorld.progression.PlayingAsSlugcat.value == "NCRunbound" ||
                Custom.rainWorld.progression.PlayingAsSlugcat.value == "NCRtech") &&
                !(ModManager.Expedition && Custom.rainWorld.ExpeditionMode && Custom.rainWorld.progression.currentSaveState.cycleNumber == 0)
                && !Custom.rainWorld.safariMode && karmaCap < 4 && ghostPreviouslyEncountered < 0f)
            {
                // unbound under karma cap 5, allowing echos anyway
                return karma >= karmaCap;
                // ...ASSUMING theyre at max karma out of their possible karma.
            }
            else return orig(ghostID, karma, karmaCap, ghostPreviouslyEncountered, playingAsRed);
        }

        public static void MadHopsBro(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (self?.room != null &&
                (self.GetNCRunbound().IsUnbound || self.GetNCRunbound().IsTechnician))
            {
                self.jumpBoost += 1f;
                // has a jump boost of +1 compared to surv
            }
        }

        // end unbmisc
    }
}
