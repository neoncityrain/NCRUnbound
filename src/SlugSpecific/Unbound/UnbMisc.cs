﻿using System;
using SlugBase;

namespace Unbound
{
    public class UnbMisc
    {
        public static void NeuronColourShift(On.SSOracleSwarmer.orig_DrawSprites orig, SSOracleSwarmer self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self != null && self.room != null && self.room.game != null)
            {
                foreach (AbstractCreature otherCreature in self.room.abstractRoom.creatures)
                {
                    if (otherCreature.realizedCreature != null && otherCreature.realizedCreature is Player &&
                        otherCreature.realizedCreature.room != null &&
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
            if (self != null && self.player.room != null && self.player.mainBodyChunk != null &&
                self.player.GetNCRunbound().effectColour != null && self.player.room.game.session != null &&
                !self.player.room.game.IsArenaSession && self.player.room.game.IsStorySession &&
                self.player.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon &&
                self.player.GetNCRunbound().IsUnbound && !self.player.DreamState)
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
            if (self != null && grasp != null && grasp.grabber != null && grasp.grabber is Player &&
                (!ModManager.MSC || !(grasp.grabber as Player).isNPC) && (grasp.grabber as Player).GetNCRunbound().IsUnbound)
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
            if (chunk != null && chunk.owner is Creature && (chunk.owner is Player) &&
                (chunk.owner as Player).GetNCRunbound().IsUnbound &&
                self.AI.DynamicRelationship((chunk.owner as Creature).abstractCreature).type == CreatureTemplate.Relationship.Type.AgressiveRival)
            {
                (chunk.owner as Player).playerState.permanentDamageTracking = (dmgFac / 10f);
            }
            orig(self, chunk, dmgFac);
        }

        public static void DamageTracking(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self != null && self.room != null && self.abstractCreature != null &&
                self.GetNCRunbound().IsNCRUnbModcat == true && self.GetNCRunbound().IsOracle == false &&
                self.GetNCRunbound().RGBRings)
            {
                self.GetNCRunbound().RGBCounter++;
            }
            orig(self, eu);
            if (self != null && self.room != null && self.abstractCreature != null &&
                self.GetNCRunbound().IsUnbound && self.Wounded)
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
            if (self != null && self.room != null && !self.slatedForDeletetion &&
                self.room.world.game.session.characterStats.name.value == "NCRunbound")
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
                                                (self.room.physicalObjects[i][j] as Player).GetNCRunbound().IsUnbound)
                                            {
                                                (self.room.physicalObjects[i][j] as Player).Stun(200);
                                                (self.room.physicalObjects[i][j] as Player).room.AddObject(new
                                                    CreatureSpasmer(self.room.physicalObjects[i][j] as Player, true, 200));
                                                (self.room.physicalObjects[i][j] as Player).playerState.permanentDamageTracking += 0.95f;

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
            if (self != null && self.room != null && shockObj != null &&
                shockObj is Creature && shockObj is Player && (shockObj as Player).GetNCRunbound().IsUnbound)
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
            if (self != null && dRelation != null && self.creature != null &&
                dRelation.trackerRep.representedCreature.realizedCreature != null && dRelation.state != null &&
                // making sure things arent null
                self.creature.creatureTemplate.type == CreatureTemplate.Type.CyanLizard &&
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsUnbound &&
                // if cyan, if unbound
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature
                // should still allow making friends with it
                )
            {
                if (self.LikeOfPlayer(dRelation.trackerRep) > 0.98f)
                {
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, self.LikeOfPlayer(dRelation.trackerRep));
                }
                else if (self.LikeOfPlayer(dRelation.trackerRep) < -0.98f)
                {
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, -(self.LikeOfPlayer(dRelation.trackerRep)));
                }
                return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 1f + (self.LikeOfPlayer(dRelation.trackerRep)));
            }
            return orig(self, dRelation);
        }

        public static Player.ObjectGrabability NoGrab(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self != null && self.room != null && obj != null &&
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
            if (!self.GetNCRunbound().Unpicky &&
                self != null && self.room != null && testObj != null &&
                self.GetNCRunbound().IsUnbound)
            {
                if (testObj is DataPearl && (testObj as DataPearl).AbstractPearl.dataPearlType == UnboundEnums.unboundKarmaPearl)
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
            if (self != null && self.room != null && (self.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.VoidSea) == null ||
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
                Custom.rainWorld.progression.PlayingAsSlugcat.value == "NCRunbound" &&
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
            if (self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound)
            {
                self.jumpBoost += 1f;
                // has a jump boost of +1 compared to surv
            }
        }

        // end unbmisc
    }
}
