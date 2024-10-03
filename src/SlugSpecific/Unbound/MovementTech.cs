namespace Unbound
{
    internal class UnbMovement
    {
        public static void UnboundCyanJumps(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound)
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

                    if (self.GetNCRunbound().DidTripleCyanJump &&
                        Math.Pow(2, self.GetNCRunbound().UnbCyanjumpCountdown) > 5)
                    {
                        // only spawn smoke if triple jumped, and (should?) get exponentially less likely

                        if (UnityEngine.Random.value < 0.1f)
                        {
                            if (self.GetNCRunbound().damagesmoke == null)
                            {
                                self.GetNCRunbound().damagesmoke = new UnbJumpsmoke(self.room, self);
                                self.room.AddObject(self.GetNCRunbound().damagesmoke);
                            }
                            self.GetNCRunbound().damagesmoke.EmitSmoke(self.firstChunk.pos, Custom.RNV(), false, 20f);
                            if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Damaged smoke triggered from overcharge!"); }
                        }
                        if (UnityEngine.Random.value < 0.02f)
                        {
                            self.room.AddObject(new Spark(self.mainBodyChunk.pos, Custom.RNV(), self.GetNCRunbound().effectColour, null, 4, 8));
                        }
                    }
                }
                else if (self.GetNCRunbound().DidTripleCyanJump)
                {
                    self.GetNCRunbound().DidTripleCyanJump = false;
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
                    if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Unbound wants to cyan jump!"); }

                    if (!self.GetNCRunbound().holdingJumpkey)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                        self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 1f));
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
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
                        self.bodyChunks[0].vel.x = 9f * num3;
                        self.bodyChunks[0].vel.y = 9f * num4;
                        self.bodyChunks[1].vel.x = 8f * num3;
                        self.bodyChunks[1].vel.y = 8f * num4;


                        self.GetNCRunbound().UnbCyanjumpCountdown += (int)self.GetNCRunbound().CyJump1Maximum / 3;
                        // 0g
                    }
                    else
                    {
                        if (self.animation == Player.AnimationIndex.Flip)
                        {
                            if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Unbound performed cyan flipjump!"); }
                            if (self.input[0].x != 0)
                            {
                                self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 10f;
                                self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 9f;
                                self.jumpBoost = 9f;
                            }
                            if (self.input[0].x == 0 || self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.y = 12f;
                                self.bodyChunks[1].vel.y = 11f;
                                self.jumpBoost = 10f;
                            }
                            if (self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.x = 11f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 9f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 15.5f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 13.5f * self.input[0].x;
                            }
                        }
                        else if (self.animation == Player.AnimationIndex.BellySlide)
                        {
                            if (self.input[0].x != 0)
                            {
                                self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 5f;
                                self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 4f;
                                self.jumpBoost = 5f;
                            }
                            if (self.input[0].x == 0 || self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.y = 10f;
                                self.bodyChunks[1].vel.y = 9f;
                                self.jumpBoost = 6f;
                            }
                            if (self.input[0].y == 1)
                            {
                                self.bodyChunks[0].vel.x = 13f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 12f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 16f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 14f * self.input[0].x;
                            }
                        }
                        else
                        {
                            // normal cyan jump
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
                                self.bodyChunks[0].vel.x = 10f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 8f * self.input[0].x;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.x = 15f * self.input[0].x;
                                self.bodyChunks[1].vel.x = 13f * self.input[0].x;
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
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetNCRunbound().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 45f);
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
                    // fixes the bodymode index and animation. without these, he will ascend endlessly... this is not a joke.
                    #endregion
                }
                #region Triple Jump
                if (self.GetNCRunbound().CanTripleCyanJump && self.input[0].jmp && !self.input[1].jmp &&
                    !self.GetNCRunbound().CanDoubleCyanJump)
                {
                    // if they cant cyanjump1, BUT CAN cyanjump2, trigger!

                    if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Unbound Cyanjump2 Triggered"); }
                    self.GetNCRunbound().DidTripleCyanJump = true;
                    if (!self.GetNCRunbound().holdingJumpkey)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Powerful_Jump, self.mainBodyChunk);
                        self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 2000f, self, 1f));
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));

                    // removed the 0g log, as it should not trigger in 0g
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
                        self.bodyChunks[0].vel.x = 13f * self.input[0].x;
                        self.bodyChunks[1].vel.x = 11f * self.input[0].x;
                    }
                    else
                    {
                        self.bodyChunks[0].vel.x = 18f * self.input[0].x;
                        self.bodyChunks[1].vel.x = 15f * self.input[0].x;
                    }

                    self.GetNCRunbound().UnbCyanjumpCountdown += (int)self.GetNCRunbound().CyJump2Maximum;
                    // adds a LOT more to the countdown- it recharges very slowly

                    #region Emit Smoke
                    if (self.GetNCRunbound().unbsmoke == null)
                    {
                        self.GetNCRunbound().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetNCRunbound().unbsmoke);
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetNCRunbound().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 50f);
                    }
                    #endregion

                    self.animation = Player.AnimationIndex.Flip;
                    self.bodyMode = Player.BodyModeIndex.Default;
                }
                #endregion
            }
            orig(self, eu);
        }

        public static void SetupWalljumps(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            if (!self.GetNCRunbound().LostTail &&
                self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound)
            {
                self.GetNCRunbound().UnbChainjumpsCount += 1;
                if (self.GetNCRunbound().UnbChainjumpsCount > 1 && !self.GetNCRunbound().DidTripleCyanJump)
                {
                    // only triggers if unbchainjumps is greater than 1, preventing a chainjump from triggering when bouncing off a wall normally
                    // while this only triggers if he HASNT done a triple jump, this should still 
                    self.GetNCRunbound().UnbCyanjumpCountdown += 10;
                    // add to the countdown to prevent immediate re-triggering of the cyan jump and make walljumping smoother

                    for (int i = 0; i < 6; i++)
                    {
                        self.room.AddObject(new Spark(self.mainBodyChunk.pos, Custom.RNV(), self.GetNCRunbound().effectColour, null, 4, 8));
                    }
                    self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                    self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 150f, self, 0.5f));
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // grants a cyan-like distortion effect and sparks

                    if (self.GetNCRunbound().UnbChainjumpsCount < 15)
                    {
                        self.jumpBoost += self.GetNCRunbound().UnbChainjumpsCount;
                        // jump boost raises with chained jumps
                    }
                    else
                    {
                        self.jumpBoost += 7f;
                        // prevents jump boosts from going above 7. this should reset upon hitting the ground
                    }


                    if (self.GetNCRunbound().unbsmoke == null)
                    {
                        self.GetNCRunbound().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetNCRunbound().unbsmoke);
                        if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Emitting smoke!"); }
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetNCRunbound().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 20f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 35f);
                    }
                    // smoke effects for walljumps
                }
            }
        }

        public static void SetupJumps(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self != null && self.room != null &&
                self.GetNCRunbound().IsUnbound)
            {
                #region Init Variables
                if (self.lowerBodyFramesOnGround > 1 || self.submerged)
                {
                    if (self.GetNCRunbound().didLongjump) { self.GetNCRunbound().didLongjump = false; }
                    if (self.GetNCRunbound().UnbChainjumpsCount != 0) { self.GetNCRunbound().UnbChainjumpsCount = 0; }
                    // this prevents the jump boosts from retaining, though using a grapple worm may keep it- which is fine
                    // it is kept when touching water or going onto poles
                    // this should be above longjump to keep the chainjump from not truly being considered "raising" the
                    // chainjump counter
                }

                if (self.simulateHoldJumpButton == 0 && self.GetNCRunbound().holdingJumpkey)
                {
                    self.GetNCRunbound().holdingJumpkey = false;
                }
                #endregion
                #region Long Jump
                // LONG JUMP ==========================================================================
                if (!self.GetNCRunbound().LostTail && !self.GetNCRunbound().DidTripleCyanJump &&
                    self.simulateHoldJumpButton > 0 && !self.GetNCRunbound().holdingJumpkey &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if jump is being held and it ISNT playing sound, AKA a longjump
                    // cannot do this jump if he did a triple jump, as he is "overcharged"
                    // also prevents him from doing so when tailless

                    self.GetNCRunbound().UnbCyanjumpCountdown += 5;
                    // regardless, locks cyanjump for 5 frames
                    self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 250f, self, 1f));
                    self.jumpBoost += 5;
                    // has a decent extra boost with a standard cyanjump noise

                    self.GetNCRunbound().holdingJumpkey = true;
                    // so that the game doesnt make the worst noise ever

                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    if (self.GetNCRunbound().unbsmoke == null)
                    {
                        self.GetNCRunbound().unbsmoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(self.GetNCRunbound().unbsmoke);
                        if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Emitting smoke!"); }
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        self.GetNCRunbound().unbsmoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 45f);
                    }
                    // emits smoke. longjumps are not possible in true 0g, so its considered a "big" jump

                    self.GetNCRunbound().didLongjump = true;
                    // sets longjump to true
                    self.GetNCRunbound().UnbChainjumpsCount += 1;
                    // adds to the chainjump count
                    if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Longjump detected!"); }
                }
                else if (self.simulateHoldJumpButton > 0 && !self.GetNCRunbound().holdingJumpkey &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if hes lost his tail or after a triple jump, he still gets a jump boost
                    self.jumpBoost += 3;

                    self.GetNCRunbound().holdingJumpkey = true;
                    self.GetNCRunbound().didLongjump = true;
                    // sets longjump to true
                    self.GetNCRunbound().UnbChainjumpsCount += 1;
                    // adds to the chainjump count

                    if (self.GetNCRunbound().MoreDebug) { NCRDebug.Log("Longjump detected! However, Unbound was " +
                        (self.GetNCRunbound().LostTail ? "missing a tail..." : "overcharged...")); }
                }
                #endregion
                #region Can He Jump?
                if (self.canJump == 0 && self.lowerBodyFramesOnGround <= 0 &&
                    // cannot normally jump and had its lower body on the ground for less than/equal to 0 frames
                    self.enteringShortCut == null && !self.inShortcut && self.shortcutDelay == 0 &&
                    // not entering, inside a shortcut, or in a shortcut delay state
                    self.Consious && !self.dead && self.canWallJump == 0 && self.jumpStun == 0 &&
                    // is awake, not dead, cannot wall jump, not jumpstunned
                    !self.submerged && self.goIntoCorridorClimb <= 0 &&
                    // is not underwater and not climbing through a pipe
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.CorridorTurn &&
                    self.animation != Player.AnimationIndex.LedgeCrawl &&
                    self.animation != Player.AnimationIndex.SurfaceSwim &&
                    self.animation != Player.AnimationIndex.DeepSwim &&
                    self.animation != Player.AnimationIndex.AntlerClimb &&
                    // animation indexes
                    self.bodyMode != Player.BodyModeIndex.CorridorClimb &&
                    self.bodyMode != Player.BodyModeIndex.Crawl &&
                    self.bodyMode != Player.BodyModeIndex.WallClimb &&
                    self.bodyMode != Player.BodyModeIndex.Swimming &&
                    // body mode indexes
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam &&
                    self.animation != Player.AnimationIndex.ZeroGPoleGrab &&
                    self.animation != Player.AnimationIndex.GetUpOnBeam &&
                    self.animation != Player.AnimationIndex.StandOnBeam &&
                    self.animation != Player.AnimationIndex.AntlerClimb &&
                    self.animation != Player.AnimationIndex.HangUnderVerticalBeam &&
                    self.animation != Player.AnimationIndex.BeamTip &&
                    self.animation != Player.AnimationIndex.GetUpToBeamTip)
                {
                    if (self.GetNCRunbound().UnbCyanjumpCountdown <= 0)
                    {
                        self.GetNCRunbound().CanDoubleCyanJump = true;
                        // if the cyan countdown is 0 or less than zero, can perform the first cyanjump
                    }
                    else
                    {
                        self.GetNCRunbound().CanDoubleCyanJump = false;
                    }

                    if (self.GetNCRunbound().UnbCyanjumpCountdown <= 175 && self.GetNCRunbound().UnbCyanjumpCountdown > 20 &&
                        self.GetNCRunbound().didLongjump == true &&

                        self.animation != Player.AnimationIndex.GrapplingSwing &&
                        (self.grasps[0] == null || self.grasps[0].grabbed is not TubeWorm) &&
                        // prevents triggering if using a grapple worm

                        (self.EffectiveRoomGravity >= 0.2f || self.gravity >= 0.2f) &&
                        // prevents usage in 0g

                        !self.GetNCRunbound().LostTail
                        // can only triple jump if he has a tail (random buffs)
                        )
                    {
                        self.GetNCRunbound().CanTripleCyanJump = true;
                        // if ALL OF THE ABOVE are true and just performed a longjump without touching the ground
                        // also makes sure the coundown is below or at 175 so that cyjump2 isnt triggered IMMEDIATELY after cyjump1
                        // and isnt triggered immediately after itself. should never trigger in 0g
                    }
                    else
                    {
                        self.GetNCRunbound().CanTripleCyanJump = false;
                    }
                }
                else
                {
                    self.GetNCRunbound().CanDoubleCyanJump = false;
                    self.GetNCRunbound().CanTripleCyanJump = false;
                }
                #endregion
            }
        }

        // end cyanjump
    }
}
