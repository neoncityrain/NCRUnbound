using System;
using System.Linq;
using UnityEngine;

namespace Unbound
{
    internal class Revgen
    {
        public static void HookThatThang()
        {
            On.FlareBomb.Update += FlarebombStun; // stunned by flashing
            // On.Water.InitiateSprites += NormalHRWater;

            On.Player.CanBeSwallowed += NoSwallow;
            On.Player.Update += RockSwallow;
            On.Player.Regurgitate += CallForHelp;
            // sets up her crying for help using regurgitation

            On.LizardAI.IUseARelationshipTracker_UpdateDynamicRelationship += LizardPack;
            On.Player.Grabbed += LizardAnger;
            // makes yellows, reds, and cyans appreciate her

            On.Player.Update += reverbHops;
            // double jump pop
        }

        private static void LizardAnger(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            if (self != null && grasp != null && self.GetNCRunbound().IsReverb &&
                (grasp.grabber is Lizard || grasp.grabber is Vulture || grasp.grabber is BigSpider ||
                grasp.grabber is DropBug))
            {
                self.room.PlaySound(
                    ModManager.MMF ? MMFEnums.MMFSoundID.Lizard_Voice_Yellow_A : SoundID.Lizard_Voice_Pink_E,
                    self.mainBodyChunk, false, 0.8f,
                    ModManager.MMF ? UnityEngine.Random.Range(2f, 2.8f) :
                    UnityEngine.Random.Range(1.8f, 2f));
                self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 3f));

                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        self.room.abstractRoom.creatures[i].realizedCreature.Consious)
                    {
                        // if the above arent true, pass. should make the if statements easier
                        #region Yellow anger
                        if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.YellowLizard)
                        {
                            var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                            lizard.AI.excitement = 0.8f;

                            lizard.AI.yellowAI.communicating = 14;
                            lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(
                                self.mainBodyChunk.pos));
                            lizard.voice.MakeSound(LizardVoice.Emotion.PainImpact);
                            lizard.AI.runSpeed = 0.8f;
                            lizard.AI.agressionTracker.IncrementAnger(lizard.AI.tracker.RepresentationForObject(
                                grasp.grabber, true), 0.3f);
                            // indicative of snowberry itself. yellows should not be AS angry as the others, but still will
                            // help to save her
                        }
                        #endregion
                        #region Cyan anger
                        if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.CyanLizard)
                        {
                            var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                            lizard.AI.excitement = 1f;

                            lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(
                                self.mainBodyChunk.pos));
                            lizard.voice.MakeSound(LizardVoice.Emotion.BloodLust);
                            lizard.AI.runSpeed = 1f;
                            lizard.AI.agressionTracker.IncrementAnger(lizard.AI.tracker.RepresentationForObject(
                                grasp.grabber, true), 1f); // fucking Pissed.
                            // as this is indicative of unbounds reaction, cyans react more violently than reds
                            // not because flicker is less violent about defending snowberry, but because
                            // snowberry sees pol angry more than she sees flicker angry.
                        }
                        #endregion
                        #region Red anger
                        if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.RedLizard)
                        {
                            var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                            lizard.AI.excitement = 1f;

                            lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(
                                self.mainBodyChunk.pos));
                            lizard.voice.MakeSound(LizardVoice.Emotion.BloodLust);
                            lizard.AI.runSpeed = 1f;
                            lizard.AI.agressionTracker.IncrementAnger(lizard.AI.tracker.RepresentationForObject(
                                grasp.grabber, true), 0.9f);
                            // notes on cyan
                        }
                        #endregion
                    }
                }
            }
        }

        private static CreatureTemplate.Relationship LizardPack(On.LizardAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, LizardAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            if (!(self.friendTracker.giftOfferedToMe != null && self.friendTracker.giftOfferedToMe.active &&
                self.friendTracker.giftOfferedToMe.item == dRelation.trackerRep.representedCreature.realizedCreature) &&
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature &&
                dRelation.trackerRep.representedCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat &&

                dRelation.trackerRep.VisualContact && dRelation.trackerRep.representedCreature.realizedCreature != null &&

                
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsReverb)
            {
                if (self.creature.creatureTemplate.type == CreatureTemplate.Type.YellowLizard)
                {
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, 0.1f + self.LikeOfPlayer(dRelation.trackerRep));
                }
                if (self.creature.creatureTemplate.type == CreatureTemplate.Type.CyanLizard)
                {
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, 0.2f + self.LikeOfPlayer(dRelation.trackerRep));
                }
                if (self.creature.creatureTemplate.type == CreatureTemplate.Type.RedLizard)
                {
                    return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, 0.2f + self.LikeOfPlayer(dRelation.trackerRep));
                }
            }
            return orig(self, dRelation);
        }

        private static void RockSwallow(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self != null && self.room != null && self.GetNCRunbound().IsReverb && self.objectInStomach == null)
            {
                self.objectInStomach = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, 
                    self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            }
            orig(self, eu);
        }

        private static void CallForHelp(On.Player.orig_Regurgitate orig, Player self)
        {
            if (self != null && self.room != null && self.abstractCreature != null &&
                self.GetNCRunbound().IsReverb)
            {
                self.room.PlaySound(ModManager.MMF ? MMFEnums.MMFSoundID.Lizard_Voice_Yellow_A : SoundID.Lizard_Voice_Pink_E, 
                    self.mainBodyChunk, false, 0.8f,
                    ModManager.MMF ? UnityEngine.Random.Range(2f, 2.8f) :
                    UnityEngine.Random.Range(1.8f, 2f));
                self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 1f));

                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.YellowLizard && 
                        self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        self.room.abstractRoom.creatures[i].realizedCreature.deaf == 0 &&
                        self.room.abstractRoom.creatures[i].realizedCreature.Consious)
                    {
                        var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                        lizard.AI.excitement = 1f;
                        if (lizard.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.YellowLizard)
                        {
                            lizard.AI.yellowAI.communicating = 14;
                        }
                        lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(self.mainBodyChunk.pos));

                        if (lizard.AI.friendTracker.friend == null)
                        {
                            lizard.AI.LizardPlayerRelationChange(1f, self.abstractCreature);
                        }

                        lizard.voice.MakeSound(LizardVoice.Emotion.Curious);
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static bool NoSwallow(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self != null && self.room != null && testObj != null &&
                self.GetNCRunbound().IsReverb)
            {
                return false;
            }
            return orig(self, testObj);
        }

        private static void FlarebombStun(On.FlareBomb.orig_Update orig, FlareBomb self, bool eu)
        {
            orig(self, eu);
            if (self != null && !self.slatedForDeletetion && self.room != null && self.room.PlayersInRoom != null &&
                self.burning > 0f)
            {
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].realizedCreature is Player &&
                        (self.room.abstractRoom.creatures[i].realizedCreature as Player).GetNCRunbound().IsReverb &&
                        !self.room.abstractRoom.creatures[i].realizedCreature.dead &&
                        self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        (Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos,
                        self.LightIntensity * 600f) ||
                        Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos,
                        self.LightIntensity * 1600f) &&
                        self.room.VisualContact(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos)))
                    {
                        self.room.abstractRoom.creatures[i].realizedCreature.stun = 80;
                        self.room.AddObject(new CreatureSpasmer(self.room.abstractRoom.creatures[i].realizedCreature, false, 80));
                    }
                }
            }
        }

        public static void reverbHops(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self?.room != null &&
                (self.GetNCRunbound().IsReverb))
            {
                #region Setup and Nullify Unused
                if (self.GetNCRunbound().unbsmoke != null &&
                    (self.GetNCRunbound().unbsmoke.slatedForDeletetion || self.GetNCRunbound().unbsmoke.room != self.room))
                {
                    self.GetNCRunbound().unbsmoke = null;
                }
                if (self.GetNCRunbound().damagesmoke != null &&
                    (self.GetNCRunbound().damagesmoke.slatedForDeletetion || self.GetNCRunbound().damagesmoke.room != self.room))
                {
                    self.GetNCRunbound().damagesmoke = null;
                }

                if (self.GetNCRunbound().UnbCyanjumpCountdown != 0)
                {
                    self.GetNCRunbound().UnbCyanjumpCountdown--;
                }

                if (self.GetNCRunbound().UnbCyanjumpCountdown < 0)
                {
                    self.GetNCRunbound().UnbCyanjumpCountdown = 0;
                }
                // makes sure the countdown doesnt go under zero, even though it really Shouldnt

                #endregion


                if (self.GetNCRunbound().CanDoubleCyanJump && self.input[0].jmp && !self.input[1].jmp)
                {
                    // standard cyanjump!!!!
                    if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Reverb hops!"); }

                    if (!self.GetNCRunbound().holdingJumpkey)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk, false, 0.9f, 1.2f);
                        self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 1f));
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.2f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.05f, 3, false));
                    // fun effects!

                    if (self.bodyMode == Player.BodyModeIndex.ZeroG || self.room.gravity == 0f || self.gravity == 0f)
                    {
                        if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Player " + self.slugcatStats.name.ToString() + " is in zero gravity, so recharge is much faster."); }
                        // allows for quick propelling in 0g
                        float num3 = self.input[0].x;
                        float num4 = self.input[0].y;
                        while (num3 == 0f && num4 == 0f)
                        {
                            num3 = (double)UnityEngine.Random.value <= 0.33 ? 0 : (double)UnityEngine.Random.value <= 0.5 ? 1 : -1;
                            num4 = (double)UnityEngine.Random.value <= 0.33 ? 0 : (double)UnityEngine.Random.value <= 0.5 ? 1 : -1;
                        }
                        self.bodyChunks[0].vel.x = 5f * num3;
                        self.bodyChunks[0].vel.y = 5f * num4;
                        self.bodyChunks[1].vel.x = 4f * num3;
                        self.bodyChunks[1].vel.y = 4f * num4;


                        self.GetNCRunbound().UnbCyanjumpCountdown += (int)self.GetNCRunbound().CyJump1Maximum / 3;
                        // 0g
                    }
                    else // if not in 0g
                    {
                        if (self.animation == Player.AnimationIndex.Flip)
                        {
                            if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Reverb performed flipjump!"); }
                            if (self.input[0].x != 0)
                            {
                                self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 6f;
                                self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 5f;
                                self.jumpBoost = 4f;
                            }
                            if (self.input[0].x == 0 || self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.y = 8f;
                                self.bodyChunks[1].vel.y = 7f;
                                self.jumpBoost = 5f;
                            }
                            if (self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.x = 6f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 5f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 9f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 8f * self.input[0].x;
                            }
                        }
                        else if (self.animation == Player.AnimationIndex.BellySlide)
                        {
                            if (self.input[0].x != 0)
                            {
                                self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 4f;
                                self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 3f;
                                self.jumpBoost = 4f;
                            }
                            if (self.input[0].x == 0 || self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.y = 6f;
                                self.bodyChunks[1].vel.y = 5f;
                                self.jumpBoost = 5f;
                            }
                            if (self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.x = 6f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 5f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 8f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 7f * self.input[0].x;
                            }
                        }
                        else
                        {
                            // normal cyan jump
                            if (self.input[0].x != 0)
                            {
                                self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 4f;
                                self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 3f;
                                self.jumpBoost = 4f;
                            }
                            if (self.input[0].x == 0 || self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.y = 6f;
                                self.bodyChunks[1].vel.y = 5f;
                                self.jumpBoost = 6f;
                            }
                            if (self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.x = 5f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 4f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 7f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 6f * self.input[0].x;
                            }
                        }

                        self.GetNCRunbound().UnbCyanjumpCountdown += (int)self.GetNCRunbound().CyJump1Maximum;
                    }


                    #region Emit Smoke
                    if (self.GetNCRunbound().unbsmoke == null)
                    {
                        self.GetNCRunbound().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetNCRunbound().unbsmoke);
                    }
                    for (int k = 0; k < 5; k++)
                    {
                        self.GetNCRunbound().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 40f);
                    }
                    #endregion
                    #region BodyMode / Animation
                    if (self.animation == Player.AnimationIndex.Roll)
                    {
                        self.animation = Player.AnimationIndex.Flip;
                    }
                    else
                    { self.animation = Player.AnimationIndex.RocketJump; }
                    self.bodyMode = Player.BodyModeIndex.Default;
                    // fixes the bodymode index and animation
                    #endregion
                }
            }
            orig(self, eu);
        }
    }
}