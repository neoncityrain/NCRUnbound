using RWCustom;
using UnityEngine;

namespace Unbound
{
    internal class CyanJump
    {
        public static void Init()
        {
            On.Player.MovementUpdate += ouuuhejumpin;
            On.Player.WallJump += Player_WallJump;
            On.Player.Update += Player_Update;
        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self != null && self.room != null &&
                self.GetCat().IsUnbound)
            {
                if (self.GetCat().unbsmoke != null && (self.GetCat().unbsmoke.slatedForDeletetion || self.GetCat().unbsmoke.room != self.room))
                {
                    self.GetCat().unbsmoke = null;
                }

                if (self.GetCat().UnbCyanjumpCountdown != 0)
                {
                    self.GetCat().UnbCyanjumpCountdown--;
                }



                if (self.GetCat().UnbCyanjumpCountdown < 0)
                {
                    self.GetCat().UnbCyanjumpCountdown = 0;
                }
                // makes sure the countdown doesnt go under zero, even though it really Shouldnt

                if (self.GetCat().CanCyanjump1 && self.input[0].jmp && !self.input[1].jmp)
                {
                    // standard cyanjump!!!!
                    Debug.Log("Unbound Cyanjump1 Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // fun effects!

                    if (self.bodyMode == Player.BodyModeIndex.ZeroG || self.room.gravity == 0f || self.gravity == 0f)
                    {
                        // allows for quick propelling in 0g
                        float num3 = (float)self.input[0].x;
                        float num4 = (float)self.input[0].y;
                        while (num3 == 0f && num4 == 0f)
                        {
                            num3 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                            num4 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                        }
                        self.bodyChunks[0].vel.x = 9f * num3;
                        self.bodyChunks[0].vel.y = 9f * num4;
                        self.bodyChunks[1].vel.x = 8f * num3;
                        self.bodyChunks[1].vel.y = 8f * num4;


                        self.GetCat().UnbCyanjumpCountdown += (int)self.GetCat().CyJump1Maximum / 3;
                        // 0g
                    }
                    else
                    {
                        if (self.input[0].x != 0)
                        {
                            self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 8f;
                            self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 7f;
                            self.jumpBoost = 6f;
                        }
                        if (self.input[0].x == 0 || self.input[0].y == 1)
                        {
                            self.bodyChunks[0].vel.y = 11f;
                            self.bodyChunks[1].vel.y = 10f;
                            self.jumpBoost = 8f;
                        }
                        if (self.input[0].y == 1)
                        {
                            self.bodyChunks[0].vel.x = 10f * (float)self.input[0].x;
                            self.bodyChunks[1].vel.x = 8f * (float)self.input[0].x;
                        }
                        else
                        {
                            self.bodyChunks[0].vel.x = 15f * (float)self.input[0].x;
                            self.bodyChunks[1].vel.x = 13f * (float)self.input[0].x;
                        }
                        self.GetCat().UnbCyanjumpCountdown += (int)self.GetCat().CyJump1Maximum;
                    }

                    if (self.GetCat().unbsmoke == null)
                    {
                        self.GetCat().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetCat().unbsmoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetCat().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 45f);
                    }

                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyMode = Player.BodyModeIndex.Default;


                }
                if (self.GetCat().CanCyanjump2 && self.input[0].jmp && !self.input[1].jmp && !self.GetCat().CanCyanjump1)
                {
                    // if they cant cyanjump1, BUT CAN cyanjump2, trigger

                    Debug.Log("Unbound Cyanjump2 Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Powerful_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));

                    // removes the 0g log, as it should not trigger in 0g
                    if (self.input[0].x != 0)
                    {
                        self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 11f;
                        self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 10f;
                        self.jumpBoost = 12f;
                    }
                    if (self.input[0].x == 0 || self.input[0].y == 1)
                    {
                        self.bodyChunks[0].vel.y = 14f;
                        self.bodyChunks[1].vel.y = 13f;
                        self.jumpBoost = 15f;
                    }
                    if (self.input[0].y == 1)
                    {
                        self.bodyChunks[0].vel.x = 13f * (float)self.input[0].x;
                        self.bodyChunks[1].vel.x = 11f * (float)self.input[0].x;
                    }
                    else
                    {
                        self.bodyChunks[0].vel.x = 18f * (float)self.input[0].x;
                        self.bodyChunks[1].vel.x = 15f * (float)self.input[0].x;
                    }

                    self.GetCat().UnbCyanjumpCountdown += (int)self.GetCat().CyJump2Maximum;
                    // adds a LOT more to the countdown- it recharges very slowly

                    if (self.GetCat().unbsmoke == null)
                    {
                        self.GetCat().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetCat().unbsmoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetCat().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 50f);
                    }

                    self.animation = Player.AnimationIndex.Flip;
                    self.bodyMode = Player.BodyModeIndex.Default;
                }
            }
            orig(self, eu);
        }

        private static void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            if (self != null && self.room != null &&
                self.GetCat().IsUnbound)
            {
                self.GetCat().UnbChainjumps += 1;
                if (self.GetCat().UnbChainjumps > 1)
                {
                    // only triggers if unbchainjumps is greater than 1, preventing a chainjump from triggering when bouncing off a wall normally
                    self.GetCat().UnbCyanjumpCountdown += 10;


                    self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // grants a cyan-like distortion effect and sparks

                    if (self.GetCat().UnbChainjumps < 7)
                    {
                        self.jumpBoost += self.GetCat().UnbChainjumps;
                        // jump boost raises with chained jumps
                    }
                    else
                    {
                        self.jumpBoost += 7f;
                        // prevents jump boosts from going above 7. this should reset upon hitting the ground
                    }


                    if (self.GetCat().unbsmoke == null)
                    {
                        self.GetCat().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetCat().unbsmoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetCat().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 20f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 35f);
                    }
                    // smoke effects for walljumps
                }

            }
        }

        private static void ouuuhejumpin(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self != null && self.room != null &&
                self.GetCat().IsUnbound)
            {
                if (self.lowerBodyFramesOnGround > 1 || self.submerged)
                {
                    self.GetCat().didLongjump = false;
                    self.GetCat().UnbChainjumps = 0;
                    // this prevents the jump boosts from retaining, though using a grapple worm may keep it- which is fine
                    // it is kept when touching water or going onto poles
                    // this should be above longjump to keep the chainjump from not truly being considered "raising" the
                    // chainjump counter
                }

                if (self.simulateHoldJumpButton == 0 && self.GetCat().PlayingSound)
                {
                    // this checks if sound is playing so it doesnt made the worst sound known to god
                    self.GetCat().PlayingSound = false;
                    // if playingsound is true but the jump button isnt being held, set it to false. this isnt perfect obviously but shrug
                    // this assumes there is room for error in the player movements, as generally to make a terrible noise it would need to
                    // be clicked very, very rapidly
                }



                if (self.simulateHoldJumpButton > 0 && !self.GetCat().PlayingSound &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if jump is being held and it ISNT playing sound, AKA a longjump

                    self.GetCat().UnbCyanjumpCountdown += 5;
                    // regardless, locks cyanjump for 5 frames
                    self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    self.jumpBoost += 3;
                    // has a decent extra boost with a standard cyanjump noise

                    self.GetCat().PlayingSound = true;
                    // so that the game doesnt make the worst noise ever

                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    if (self.GetCat().unbsmoke == null)
                    {
                        self.GetCat().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetCat().unbsmoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetCat().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 45f);
                    }
                    // emits smoke. longjumps are not possible in true 0g, so its considered a "big" jump

                    self.GetCat().didLongjump = true;
                    // sets longjump to true
                    self.GetCat().UnbChainjumps += 1;
                    // adds to the chainjump count
                    Debug.Log("Longjump detected!");
                }



                if ((self.canJump == 0 && self.lowerBodyFramesOnGround <= 0 &&
                    // cannot normally jump and had its lower body on the ground for less than/equal to 0 frames
                    self.enteringShortCut == null && !self.inShortcut && self.shortcutDelay == 0 &&
                    // not entering, inside a shortcut, or in a shortcut delay state
                    self.Consious && !self.dead && self.canWallJump == 0 && self.jumpStun == 0 &&
                    // is awake, not dead, cannot wall jump, not jumpstunned
                    !self.submerged && self.goIntoCorridorClimb <= 0 &&
                    // is not underwater and not climbing through a pipe
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.CorridorTurn &&
                    self.bodyMode != Player.BodyModeIndex.Crawl &&
                    self.bodyMode != Player.BodyModeIndex.WallClimb &&
                    self.animation != Player.AnimationIndex.LedgeCrawl &&
                    self.animation != Player.AnimationIndex.BellySlide &&
                    self.animation != Player.AnimationIndex.SurfaceSwim &&
                    self.bodyMode != Player.BodyModeIndex.Swimming &&
                    self.animation != Player.AnimationIndex.DeepSwim &&
                    self.animation != Player.AnimationIndex.AntlerClimb &&
                    self.bodyMode != Player.BodyModeIndex.CorridorClimb &&

                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.HangUnderVerticalBeam &&
                    self.animation != Player.AnimationIndex.BeamTip &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpToBeamTip &&
                    self.animation != Player.AnimationIndex.StandOnBeam
                    // beam stuff
                    )

                    ||

                    (self.GetCat().UnbCyanjumpCountdown <= 0 && self.canJump == 0 &&
                    self.Consious && !self.dead &&
                    !self.submerged && self.goIntoCorridorClimb > 0 &&
                    self.EffectiveRoomGravity == 0f && self.animation == Player.AnimationIndex.ZeroGSwim &&
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.ZeroGPoleGrab &&
                    self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam &&
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam)
                    // checks if theyre swimming in zerog
                    )
                {
                    if (self.GetCat().UnbCyanjumpCountdown <= 0)
                    {
                        self.GetCat().CanCyanjump1 = true;
                        // if the cyan countdown is 0 or less than zero, can perform the first cyanjump
                    }
                    else
                    {
                        self.GetCat().CanCyanjump1 = false;
                    }


                    if (self.GetCat().UnbCyanjumpCountdown <= 175 && self.GetCat().UnbCyanjumpCountdown > 20 &&
                        self.GetCat().didLongjump == true &&

                        self.animation != Player.AnimationIndex.GrapplingSwing &&
                        (self.grasps[0] == null || !(self.grasps[0].grabbed is TubeWorm)) &&
                        // prevents triggering if using a grapple worm

                        self.EffectiveRoomGravity != 0f
                    // prevents usage in 0g
                    )
                    {
                        self.GetCat().CanCyanjump2 = true;
                        // if ALL OF THE ABOVE are true and just performed a longjump without touching the ground
                        // also makes sure the coundown is below or at 175 so that cyjump2 isnt triggered IMMEDIATELY after cyjump1
                        // and isnt triggered immediately after itself. should never trigger in 0g
                    }
                    else
                    {
                        self.GetCat().CanCyanjump2 = false;
                    }
                }
                else
                {
                    self.GetCat().CanCyanjump1 = false;
                    self.GetCat().CanCyanjump2 = false;
                }
            }
        }

        // end cyanjump
    }
}
