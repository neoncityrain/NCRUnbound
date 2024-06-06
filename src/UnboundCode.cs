using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;
using MoreSlugcats;
using UnboundCat;
using System.Xml.Schema;

namespace TheUnbound
{
    [BepInPlugin(MOD_ID, "NCR.theunbound", "0.0.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "NCR.theunbound";


        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            On.Player.ctor += Player_ctor;
            // initialising

            On.MoreSlugcats.StowawayBugState.AwakeThisCycle += StowawayBugState_AwakeThisCycle;
            // rerolls if a stowaway is awake or not. it should result in a bit over a 1/3 chance that it will be awake each cycle

            On.Player.Jump += Player_Jump;
            // increases unbounds base jump by 1f

            On.Player.MovementUpdate += ouuuhejumpin;
            On.Player.WallJump += Player_WallJump;
            // sets up chain walljumps and long jumps

            On.SeedCob.HitByWeapon += SeedCob_HitByWeapon;
            // SEED COB ALLERGYYYYYY

            On.Player.Update += Player_Update;
            // sets up cyan jumps

            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
        }

































        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (self.player.GetCat().IsUnbound)
            {
                float num = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f);
                Vector2 vector = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker);
                Vector2 vector2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker);
                float num2 = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(vector2, vector).y));
                // where is ymir when i need them i cannot do math stuff for the life of me
                //initiating animation variables used in body sprites


                float circlesrota = sLeaser.sprites[sLeaser.sprites.Length - 1].rotation;
                // rotation value

                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = 0.23f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleY = 0.35f;
                sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                // coloured ring 1

                sLeaser.sprites[sLeaser.sprites.Length - 2].scaleX = 0.14f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 2].scaleY = 0.25f;
                sLeaser.sprites[sLeaser.sprites.Length - 2].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                // internal circle 1

                sLeaser.sprites[sLeaser.sprites.Length - 3].scaleX = 0.23f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 3].scaleY = 0.35f;
                sLeaser.sprites[sLeaser.sprites.Length - 3].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                // coloured ring 2

                sLeaser.sprites[sLeaser.sprites.Length - 4].scaleX = 0.14f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 4].scaleY = 0.25f;
                sLeaser.sprites[sLeaser.sprites.Length - 4].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                //d internal circle 2





                if (self.player.flipDirection > 0)
                {
                    // if facing the players left... probably

                    //      (body to hips position) - (camera position) - (player sleep counter * 4f) +
                    //          ((1f to 1.3f at the rate of the player aerobic value) * (value for breathing)) *
                    //          (1f - (0.3 to 0.5 at the rate of hips to body position drawn, inversed) + rotation of the body + value)
                    // this took way longer than youd believe
                    //I can tell. dw.

                    sLeaser.sprites[sLeaser.sprites.Length - 1].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) - 3f + (circlesrota / 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 1].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f)
                        + Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num * (1f - num2) - (circlesrota * 2f) - 10f;
                    // ring 1

                    sLeaser.sprites[sLeaser.sprites.Length - 2].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) - 4f + (circlesrota / 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 2].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        (Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num) * (1f - num2) - (circlesrota * 2f) - 10f;
                    // circle 1



                    // the other side
                    sLeaser.sprites[sLeaser.sprites.Length - 3].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) + 8.2f + (circlesrota * 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 3].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num * (1f - num2) + (circlesrota / 2f) - 10f;
                    // ring 2

                    sLeaser.sprites[sLeaser.sprites.Length - 4].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) + 9f + (circlesrota * 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 4].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        (Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num) * (1f - num2) + (circlesrota / 2f) - 10f;
                    // circle 2
                }
                else
                {
                    // if facing the players right

                    sLeaser.sprites[sLeaser.sprites.Length - 1].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) - 1f - (circlesrota * 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 1].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num * (1f - num2) + (circlesrota * 2f) - 10f;
                    // ring 1

                    sLeaser.sprites[sLeaser.sprites.Length - 2].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) - 2f - (circlesrota * 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 2].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num * (1f - num2) + (circlesrota * 2f) - 10f;
                    // circle 1



                    // the other side
                    sLeaser.sprites[sLeaser.sprites.Length - 3].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) + 6.2f - (circlesrota / 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 3].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num * (1f - num2) - (circlesrota * 2f) - 10f;
                    // ring 2

                    sLeaser.sprites[sLeaser.sprites.Length - 4].x = vector.x - camPos.x - (self.player.sleepCurlUp * 4f) + 7f - (circlesrota / 2f);
                    sLeaser.sprites[sLeaser.sprites.Length - 4].y = vector.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        (Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * num) * (1f - num2) - (circlesrota * 2f) - 10f;
                    // circle 2
                }



                // the below works fine for now
                if (self.player.GetCat().UnbCyanjumpCountdown == 0)
                {
                    if (PlayerGraphics.customColors != null)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.CustomColorSafety(0);

                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.CustomColorSafety(0);

                    }
                    else
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.81f, 0.8f, 0.8f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.blue;
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                    }
                }
                else
                {
                    if (PlayerGraphics.customColors != null)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.CustomColorSafety(0);

                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.CustomColorSafety(0);
                    }
                    else
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.81f, 0.8f, 0.8f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                    }
                }



            }
        }























        private void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            
            if (self.player.GetCat().IsUnbound)
            {
                sLeaser.RemoveAllSpritesFromContainer();
                if (newContatiner == null)
                {
                    newContatiner = rCam.ReturnFContainer("Midground");
                }
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    if (ModManager.MSC && i == self.gownIndex)
                    {
                        newContatiner = rCam.ReturnFContainer("Items");
                        newContatiner.AddChild(sLeaser.sprites[i]);
                    }
                    else if (ModManager.MSC)
                    {
                        if (i == sLeaser.sprites.Length - 1 || i == sLeaser.sprites.Length - 2)
                        {
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 1]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 2]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 3]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 4]);
                        }
                        else if ((i <= 6 || i >= 9) && i <= 9)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else
                        {
                            rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                        }
                    }
                    else if ((i > 6 && i < 9) || i > 9)
                    {
                        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                    }
                    else
                    {
                        newContatiner.AddChild(sLeaser.sprites[i]);
                    }
                }
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        

        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            
            if (self.player.GetCat().IsUnbound)
            {
                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 4);
                sLeaser.sprites[sLeaser.sprites.Length - 1] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                //coloured spot, aka the coloured ring


                sLeaser.sprites[sLeaser.sprites.Length - 2] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                //inside spot


                sLeaser.sprites[sLeaser.sprites.Length - 3] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 3].shader = rCam.game.rainWorld.Shaders["Basic"];
                // TWO!


                sLeaser.sprites[sLeaser.sprites.Length - 4] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 4].shader = rCam.game.rainWorld.Shaders["Basic"];
                // RAAAAH

                self.AddToContainer(sLeaser, rCam, null);
            }
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.GetCat().IsUnbound)
            {
                if (self.GetCat().UnbCyanjumpCountdown != 0)
                {
                    self.GetCat().UnbCyanjumpCountdown--;
                }
                


                if (self.GetCat().UnbCyanjumpCountdown < 0)
                {
                    self.GetCat().UnbCyanjumpCountdown = 0;
                }
                // makes sure the countdown doesnt go under zero, even though it really Shouldnt

                if (self.GetCat().CanCyanjump && self.input[0].jmp && !self.input[1].jmp)
                {
                    Debug.Log("Unbound Cyanjump Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // fun effects!


                    if (self.bodyMode == Player.BodyModeIndex.ZeroG || self.room.gravity == 0f || self.gravity == 0f)
                    {
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


                        self.GetCat().UnbCyanjumpCountdown = 100;
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
                        self.GetCat().UnbCyanjumpCountdown = 200;
                    }


                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyMode = Player.BodyModeIndex.Default;
                }
            }
            orig(self, eu);
        }

        private void SeedCob_HitByWeapon(On.SeedCob.orig_HitByWeapon orig, SeedCob self, Weapon weapon)
        {
            if (weapon == null || self.room == null || self.room.roomSettings == null)
            {
                return;
            }
            if (self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.roomSettings.DangerType == MoreSlugcatsEnums.RoomRainDangerType.Blizzard && weapon.firstChunk.vel.magnitude < 20f)
                {
                    if (UnityEngine.Random.Range(0.5f, 0.8f) < self.freezingCounter)
                    {
                        self.spawnUtilityFoods();
                    }
                    return;
                }
                if (weapon.thrownBy != null && weapon.thrownBy is Player && ((weapon.thrownBy as Player).slugcatStats.name == MoreSlugcatsEnums.SlugcatStatsName.Spear || (weapon.thrownBy as Player).SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                {
                    return;
                }
                self.spawnUtilityFoods();
                return;
            }
            else
            {
                orig(self, weapon);
            }
        }

        private void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            if (self.GetCat().IsUnbound)
            {
                self.GetCat().UnbChainjumps += 1;
                if (self.GetCat().UnbChainjumps > 1)
                {
                    // only triggers if unbchainjumps is greater than 1, preventing a chainjump from triggering when bouncing off a wall normally
                    
                    self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
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
                }
                    
            }
        }

        private void ouuuhejumpin(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            
            if (self.GetCat().IsUnbound)
            {



                if (self.simulateHoldJumpButton == 0 && self.GetCat().PlayingSound)
                {
                    // this checks if sound is playing so it doesnt made the worst sound known to god
                    self.GetCat().PlayingSound = false;
                    // if playingsound is true but the jump button isnt being held, set it to false. this isnt perfect obviously but shrug
                }



                if (self.simulateHoldJumpButton > 0 && !self.GetCat().PlayingSound &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if jump is being held and it ISNT playing sound
                    System.Random rd = new System.Random();
                    int rand_num = rd.Next(1, 6);
                    // gets a random value from 1 to 6. this should include 1 and 6

                    if (rand_num < 3)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                        self.jumpBoost += 2;
                        // 1-2, has a small boost with a little cyanjump noise
                    }
                    else if (rand_num < 6)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                        self.jumpBoost += 3;
                        // 3-5, has a decent boost with a standard cyanjump noise
                    }
                    else
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Powerful_Jump, self.mainBodyChunk);
                        self.jumpBoost += 5;
                        // 6, has a large boost with a strong cyanjump noise
                    }


                    self.GetCat().PlayingSound = true;
                    // so that the game doesnt make the worst noise ever

                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
                    // red cyanliz effect

                    self.GetCat().UnbChainjumps += 1;
                    // adds to the chainjump count
                }
                if (self.lowerBodyFramesOnGround > 0 || self.submerged)
                {
                    self.GetCat().CanCyanjump = false;
                    // makes it so unbound DEFINITELY cant cyan jump as long as theyre on the ground
                    self.GetCat().UnbChainjumps = 0;
                    // resets values, including the chainjump value. this prevents the jump boost from retaining, though using a grapple worm may keep it- which is fine
                }










                


                if ((self.GetCat().UnbCyanjumpCountdown == 0 && self.canJump == 0 &&
                    // countdown has hit or become less than zero, cannot normally jump
                    self.Consious && !self.dead &&
                    // is awake and not dead
                    !self.submerged && self.goIntoCorridorClimb <= 0 &&
                    // is not underwater and not climbing through a pipe
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.CorridorTurn &&
                    self.bodyMode != Player.BodyModeIndex.Crawl &&
                    self.bodyMode != Player.BodyModeIndex.WallClimb &&
                    self.animation != Player.AnimationIndex.LedgeCrawl &&
                    self.animation != Player.AnimationIndex.BellySlide &&
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.SurfaceSwim &&
                    self.bodyMode != Player.BodyModeIndex.Swimming &&
                    self.animation != Player.AnimationIndex.DeepSwim &&
                    self.animation != Player.AnimationIndex.AntlerClimb)

                    ||

                    (self.GetCat().UnbCyanjumpCountdown <= 0 && self.canJump == 0 &&
                    self.Consious && !self.dead &&
                    !self.submerged && self.goIntoCorridorClimb > 0 &&
                    self.EffectiveRoomGravity == 0f && self.animation == Player.AnimationIndex.ZeroGSwim
                    && self.animation != Player.AnimationIndex.VineGrab)
                    // checks if theyre swimming in zerog. if so, can cyanjump
                    )
                {
                    self.GetCat().CanCyanjump = true;
                }
                else
                {
                    self.GetCat().CanCyanjump = false;
                }



                
            }
        }

        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (self.GetCat().IsUnbound)
            {
                self.jumpBoost += 1f;
                // has a jump boost of +1 compared to surv
            }
        }

        private bool StowawayBugState_AwakeThisCycle(On.MoreSlugcats.StowawayBugState.orig_AwakeThisCycle orig, MoreSlugcats.StowawayBugState self, int cycle)
        {
            if (self.creature.world.game.session.characterStats.name.value == "NCRunbound")
            {
                Debug.Log("Unbound world, rerolling stowawake (because life is a fucking nightmare)");
                System.Random rd = new System.Random();
                int rand_num = rd.Next(1, 3);
                if (rand_num == 1)
                {
                    return true;
                    // if random number is 1, awaken stowaway
                }
                else
                {
                    return orig(self, cycle);
                    // if the random number isnt 1, refer to the original code
                }
            }
            else return orig(self, cycle);
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.slugcatStats.name.value == "NCRunbound")
            {
                self.GetCat().IsUnbound = true;
                
            }
            if (self.room.game.session is StoryGameSession && self.room.game.session.characterStats.name.value == "NCRunbound" &&
                !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripMoon &&
                !(self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripPebbles)
            {
                // checks if moon and pebbles are dead. if they arent, kill them
                (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripPebbles = true;
                (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.ripMoon = true;
            }
        }


        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
        }
    }
}