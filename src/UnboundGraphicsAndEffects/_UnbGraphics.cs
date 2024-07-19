using IL.LizardCosmetics;
using System;

namespace Unbound
{
    internal class UnbGraphics
    {
        static FAtlas unbsleevesarm;
        static FAtlas unbearhead;
        static FAtlas unbpupface;
        static FAtlas unbjumphips;
        static FAtlas unbjumpbody;
        static FAtlas unbfrecklehips;
        static FAtlas unbhead;
        static FAtlas unbarm;
        static FAtlas unbmittenlegs;
        static FAtlas unblegs;

        public static void Init()
        {
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            // unbound jumpring graphics, 1-4

            // On.JollyCoop.JollyMenu.JollyPlayerSelector.SetPortraitImage_Name_Color += JollyPlayerSelector_SetPortraitImage_Name_Color;
            // wow thats a mouthful lol. dynamic jolly pfp images. currently disabled due to coding issues

            unbsleevesarm ??= Futile.atlasManager.LoadAtlas("atlases/unbsleevesarm");
            unbarm ??= Futile.atlasManager.LoadAtlas("atlases/unbarm");

            unbpupface ??= Futile.atlasManager.LoadAtlas("atlases/unbpupface");

            unbfrecklehips ??= Futile.atlasManager.LoadAtlas("atlases/unbfrecklehips");
            unbjumphips ??= Futile.atlasManager.LoadAtlas("atlases/unbjumphips");
            unbjumpbody ??= Futile.atlasManager.LoadAtlas("atlases/unbjumpbody");

            unbearhead ??= Futile.atlasManager.LoadAtlas("atlases/unbearhead");
            unbhead ??= Futile.atlasManager.LoadAtlas("atlases/unbhead");

            unblegs ??= Futile.atlasManager.LoadAtlas("atlases/unblegs");
            unbmittenlegs ??= Futile.atlasManager.LoadAtlas("atlases/unbmittenlegs");
            // initiating atlases

            On.PlayerGraphics.ctor += PlayerGraphics_ctor;
            On.PlayerGraphics.ApplyPalette += PlayerGraphics_ApplyPalette;
            // cyan frills
        }

        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self.player.GetCat().IsUnbound)
            {
                // self.player.GetCat().scalefrill = new UnbScales(self, 13);
            }
        }

        private static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);

            if (self.player.GetCat().IsUnbound)
            {
                Color color = PlayerGraphics.SlugcatColor(self.CharacterForColor);
                Color color2 = new Color(color.r, color.g, color.b);
                if (self.malnourished > 0f)
                {
                    float num = self.player.Malnourished ? self.malnourished : Mathf.Max(0f, self.malnourished - 0.005f);
                    color2 = Color.Lerp(color2, Color.gray, 0.4f * num);
                }
                color2 = self.HypothermiaColorBlend(color2);
                // initiate colour values

                if (self.player.GetCat().scalefrill != null)
                {
                    Color effectCol = new Color(0.87f, 0.39f, 0.33f);
                    if (!rCam.room.game.setupValues.arenaDefaultColors && !ModManager.CoopAvailable)
                    {
                        switch (self.player.playerState.playerNumber)
                        {
                            case 0:
                                if (rCam.room.game.IsArenaSession)
                                {
                                    effectCol = new Color(0.25f, 0.65f, 0.82f);
                                }
                                break;
                            case 1:
                                effectCol = new Color(0.31f, 0.73f, 0.26f);
                                break;
                            case 2:
                                effectCol = new Color(0.6f, 0.16f, 0.6f);
                                break;
                            case 3:
                                effectCol = new Color(0.96f, 0.75f, 0.95f);
                                break;
                        }
                    }
                    self.player.GetCat().scalefrill.SetScaleColors(color2, effectCol);
                    self.player.GetCat().scalefrill.ApplyPalette(sLeaser, rCam, palette);
                }
            }
            // end applypalette
        }

        //  private static void JollyPlayerSelector_SetPortraitImage_Name_Color(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_SetPortraitImage_Name_Color orig, JollyCoop.JollyMenu.JollyPlayerSelector self, SlugcatStats.Name className, Color colorTint)
        // {
        //orig(self, className, colorTint);

        // MenuIllustration portrait1 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer1", new Vector2(100f, 100f) / 2f, true, true);
        //MenuIllustration portrait2 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer2", new Vector2(100f, 100f) / 2f, true, true);
        //MenuIllustration portrait3 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer3", new Vector2(100f, 100f) / 2f, true, true);
        // MenuIllustration portrait4 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer4", new Vector2(100f, 100f) / 2f, true, true);

        // portrait2.sprite.color = self.faceTintColor;
        // portrait3.sprite.color = self.uniqueTintColor;
        // portrait4.sprite.color = self.bodyTintColor;

        // self.subObjects.Add(portrait1);
        // self.subObjects.Add(portrait2);
        // self.subObjects.Add(portrait3);
        // self.subObjects.Add(portrait4);

        // portrait2.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
        //  portrait3.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
        // portrait4.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
        // }

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (!(self.player.GetCat().GraphicsDisabled && self.player.GetCat().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                self.player.GetCat().IsUnbound)
            {
                // INITIATING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                float num = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f); // breath-altered
                Vector2 vector = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker); // positions from body to hips
                Vector2 vector2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker); // positions from hips to body
                // when vector and vector2 are combined, the position should be the exact center of the body
                Vector2 vector3 = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker); // head position
                if (self.player.aerobicLevel > 0.5f)
                {
                    // when exhausted, maybe?
                    vector += Custom.DirVec(vector2, vector) * Mathf.Lerp(-1f, 1f, num) *
                        Mathf.InverseLerp(0.5f, 1f, self.player.aerobicLevel) * 0.5f;
                    vector3 -= Custom.DirVec(vector2, vector) * Mathf.Lerp(-1f, 1f, num) * Mathf.Pow(Mathf.InverseLerp(0.5f, 1f,
                        self.player.aerobicLevel), 1.5f) * 0.75f;
                }
                float num2 = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(vector2, vector).y));
                // ill be honest im lost
                Vector2 vector4 = (vector2 * 3f + vector) / 4f;
                // body to hips position-ish. looks to be lower than normal, so probably about where the legs start
                float d = 1f - 0.2f * self.malnourished; // malnourished-dependent
                float d2 = 6f; // tail stretch value. idk why the default is 6 but shrug

                for (int i = 0; i < 4; i++)
                {
                    Vector2 vector5 = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker); // determining tail position
                    Vector2 normalized = (vector5 - vector4).normalized; // switches the numbers into actual (x,y) format, should be the tail base
                    Vector2 a = Custom.PerpendicularVector(normalized); // ?
                    float d3 = Vector2.Distance(vector5, vector4) / 5f; // the distance from tail position to body/hips/legs position
                    if (i == 0)
                    {
                        d3 = 0f;
                    }

                    // TAIL MOVEMENT
                    (sLeaser.sprites[sLeaser.sprites.Length - 11] as TriangleMesh).MoveVertice(i * 4, vector4 - a * d2 * d + normalized * d3 - camPos);
                    (sLeaser.sprites[sLeaser.sprites.Length - 11] as TriangleMesh).MoveVertice(i * 4 + 1, vector4 + a * d2 * d + normalized * d3 - camPos);
                    if (i < 3)
                    {
                        (sLeaser.sprites[sLeaser.sprites.Length - 11] as TriangleMesh).MoveVertice(i * 4 + 2, vector5 - a *
                            self.tail[i].StretchedRad * d - normalized * d3 - camPos);
                        (sLeaser.sprites[sLeaser.sprites.Length - 11] as TriangleMesh).MoveVertice(i * 4 + 3, vector5 + a *
                            self.tail[i].StretchedRad * d - normalized * d3 - camPos);
                    }
                    else
                    {
                        (sLeaser.sprites[sLeaser.sprites.Length - 11] as TriangleMesh).MoveVertice(i * 4 + 2, vector5 - camPos);
                    }
                    // END TAIL MOVEMENT

                    d2 = self.tail[i].StretchedRad;
                    vector4 = vector5; // sets the legs / tailbase value to be the same
                }
                float num3 = Custom.AimFromOneVectorToAnother(Vector2.Lerp(vector2, vector, 0.5f), vector3); // body-hips based on head
                int num4 = Mathf.RoundToInt(Mathf.Abs(num3 / 360f * 34f)); // round the above to an integer
                Vector2 vector6 = Vector2.Lerp(self.lastLookDir, self.lookDirection, timeStacker) * 3f * (1f - self.player.sleepCurlUp); // face position
                if (self.player.sleepCurlUp > 0f)
                {
                    // if sneebing
                    num4 = 7; // ?
                    num4 = Custom.IntClamp((int)Mathf.Lerp(num4, 4f, self.player.sleepCurlUp), 0, 8);
                    // body-hips position based on the head, but dependant on sleep pose and an integer?
                    num3 = Mathf.Lerp(num3, 45f * Mathf.Sign(vector.x - vector2.x), self.player.sleepCurlUp); // tailbase position dependant on sleep pose
                    vector3.y += 1f * self.player.sleepCurlUp; // head y position dependant on sleep pose
                    vector3.x += Mathf.Sign(vector.x - vector2.x) * 2f * self.player.sleepCurlUp; // head x position dependant on sleep pose
                    vector6.y -= 2f * self.player.sleepCurlUp; // face y position dependant on sleep pose
                    vector6.x -= 4f * Mathf.Sign(vector.x - vector2.x) * self.player.sleepCurlUp; // youll never fucking guess
                }
                else if (self.owner.room != null && self.owner.EffectiveRoomGravity == 0f)
                {
                    // if in 0g
                    num4 = 0; // good lerd num4 eludes me. maybe this is a head atlas value?
                }
                else if (self.player.Consious)
                {
                    if ((self.player.bodyMode == Player.BodyModeIndex.Stand && self.player.input[0].x != 0) ||
                        self.player.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            num4 = 7; // if crawling
                        }
                        else
                        {
                            num4 = 6; // if standing
                        }
                        vector6.x = 0f; // face x position
                    }
                    else
                    {
                        Vector2 p = vector3 - vector2; // hips - body
                        p.x *= 1f - vector6.magnitude / 3f; // lawd help me
                        p = p.normalized; // p = (p.x, p.y)
                    }
                }
                else
                {
                    vector6 *= 0f; // sets face y position to zero
                    num4 = 0;
                }
                if (ModManager.CoopAvailable && self.player.bool1)
                {
                    // if the player is a pup, except this doesn't entirely make sense but we aint worryin about it right now
                    vector3.y -= 1.9f;
                    num3 = Mathf.Lerp(num3, 45f * Mathf.Sign(vector.x - vector2.x), 0.7f);
                    vector6.x -= 0.2f;
                }
                Vector2 vector7 = Vector2.Lerp(self.legs.lastPos, self.legs.pos, timeStacker); // legs position


                // ADDING / REPLACING ATLAS THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                // LEG THINGS
                string lego = sLeaser.sprites[4]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled &&
                    unbmittenlegs == null)
                {
                    UnityEngine.Debug.Log("Unbound Socks sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    lego != null && lego.StartsWith("Legs") &&
                    unbmittenlegs._elementsByName.TryGetValue("unbmitten" + lego, out var leggy))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 10].element = leggy;
                }
                if (!self.player.GetCat().GraphicsDisabled &&
                    unblegs == null)
                {
                    UnityEngine.Debug.Log("Unbound Leg sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    lego != null && lego.StartsWith("Legs") &&
                    unblegs._elementsByName.TryGetValue("unb" + lego, out var leggie))
                {
                    sLeaser.sprites[4].element = leggie;
                }

                // HEAD THINGS
                string head = sLeaser.sprites[3]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled && unbearhead == null)
                {
                    UnityEngine.Debug.Log("Unbound Eartip sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbearhead._elementsByName.TryGetValue("unbear" + head, out var eartip))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 5].element = eartip;
                }
                // eartips
                if (!self.player.GetCat().GraphicsDisabled &&
                    unbhead == null)
                {
                    UnityEngine.Debug.Log("Unbound Head sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbhead._elementsByName.TryGetValue("unb" + head, out var headreplace))
                {
                    sLeaser.sprites[3].element = headreplace;
                }

                // ARM THINGS
                string larm = sLeaser.sprites[5]?.element?.name;
                string rarm = sLeaser.sprites[6]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled && unbarm == null)
                {
                    UnityEngine.Debug.Log("Unbound Arm sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled && larm != null && larm.StartsWith("PlayerArm") &&
                    unbarm._elementsByName.TryGetValue("unb" + larm, out var leftreplace))
                {
                    sLeaser.sprites[5].element = leftreplace;
                }
                if (!self.player.GetCat().GraphicsDisabled && unbarm != null && rarm != null && rarm.StartsWith("PlayerArm") &&
                    unbarm._elementsByName.TryGetValue("unb" + rarm, out var rightreplace))
                {
                    sLeaser.sprites[6].element = rightreplace;
                }
                // arm replacements
                if (!self.player.GetCat().GraphicsDisabled && unbsleevesarm == null)
                {
                    UnityEngine.Debug.Log("Unbound Mitten sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled && larm != null && larm.StartsWith("PlayerArm") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + larm, out var larmreplace))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 6].element = larmreplace;
                }
                if (!self.player.GetCat().GraphicsDisabled && unbarm != null && rarm != null && rarm.StartsWith("PlayerArm") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + larm, out var rarmreplace))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 7].element = rarmreplace;
                }


                // HAND THINGS
                string lhand = sLeaser.sprites[7]?.element?.name;
                string rhand = sLeaser.sprites[8]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled && unbarm != null && lhand != null && lhand.StartsWith("OnTopOf") &&
                    unbarm._elementsByName.TryGetValue("unb" + lhand, out var lhandreplace))
                {
                    sLeaser.sprites[7].element = lhandreplace;
                }
                if (!self.player.GetCat().GraphicsDisabled && unbarm != null && rhand != null && rhand.StartsWith("OnTopOf") &&
                    unbarm._elementsByName.TryGetValue("unb" + rhand, out var rhandreplace))
                {
                    sLeaser.sprites[8].element = rhandreplace;
                }

                if (!self.player.GetCat().GraphicsDisabled && unbsleevesarm != null && lhand != null && lhand.StartsWith("OnTopOf") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + lhand, out var lsleeve))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 8].element = lsleeve;
                }
                if (!self.player.GetCat().GraphicsDisabled && unbsleevesarm != null && rhand != null && rhand.StartsWith("OnTopOf") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + rhand, out var rsleeve))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 9].element = rsleeve;
                }

                // HIPS THINGS
                string hipwthekids = sLeaser.sprites[1]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled && unbfrecklehips == null)
                {
                    UnityEngine.Debug.Log("Unbound Freckle sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    hipwthekids != null && hipwthekids.StartsWith("Hips") &&
                    unbfrecklehips._elementsByName.TryGetValue("unbfreckle" + hipwthekids, out var freck))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 3].element = freck;
                }
                // body freckles
                if (!self.player.GetCat().RingsDisabled && unbjumphips == null)
                {
                    UnityEngine.Debug.Log("Unbound LOWER Jumpring sprites missing!");
                }
                else if (!self.player.GetCat().RingsDisabled && hipwthekids != null && hipwthekids.StartsWith("Hips") &&
                    unbjumphips._elementsByName.TryGetValue("unbjump" + hipwthekids, out var jumprings))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 2].element = jumprings;
                }
                // lower jumprings

                // BODY THINGS
                string bodyget = sLeaser.sprites[0]?.element?.name;
                if (!self.player.GetCat().RingsDisabled && unbjumpbody == null)
                {
                    UnityEngine.Debug.Log("Unbound UPPER Jumpring sprites missing!");
                }
                else if (!self.player.GetCat().RingsDisabled && bodyget != null && bodyget.StartsWith("Body") &&
                    unbjumpbody._elementsByName.TryGetValue("unbjump" + bodyget, out var jumprings2))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 4].element = jumprings2;
                }
                // upper jumprings

                // FACE THINGS
                string faceget = sLeaser.sprites[9]?.element?.name;
                if (!self.player.GetCat().GraphicsDisabled &&
                    unbpupface == null)
                {
                    UnityEngine.Debug.Log("Unbound Pupil sprites missing!");
                }
                else if (!self.player.GetCat().GraphicsDisabled &&
                    faceget != null && faceget.StartsWith("Face") &&
                    unbpupface._elementsByName.TryGetValue("unbpup" + faceget, out var pupils))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 1].element = pupils;
                }
                // pupils

                // VANILLA TWEAKING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                
                if (!self.player.GetCat().GraphicsDisabled)
                {
                    sLeaser.sprites[1].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * num - 0.05f * self.malnourished;
                    sLeaser.sprites[0].scaleX = 0.8f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f,
                        self.player.sleepCurlUp);
                    // makes unbound thinner
                    sLeaser.sprites[10].RemoveFromContainer();
                    sLeaser.sprites[11].RemoveFromContainer();
                    // removes the mark and the marks glow
                    if (self.player.stun > 0)
                    {
                        sLeaser.sprites[4].isVisible = false;
                    }
                    // hides legs when stunned
                }

                //  ANIMATED PLACEMENT THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                if (!self.player.GetCat().RingsDisabled)
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 2].x = (vector2.x * 2f + vector.x) / 3f - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 2].y = (vector2.y * 2f + vector.y) / 3f - camPos.y - self.player.sleepCurlUp * 3f;
                    sLeaser.sprites[sLeaser.sprites.Length - 2].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(self.tail[0].lastPos, self.tail[0].pos,
                        timeStacker));
                    sLeaser.sprites[sLeaser.sprites.Length - 2].scaleY = 1f + self.player.sleepCurlUp * 0.2f;
                    sLeaser.sprites[sLeaser.sprites.Length - 2].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * num - 0.05f * self.malnourished;
                    // lower jumprings, attached to hips

                    // body things
                    sLeaser.sprites[sLeaser.sprites.Length - 4].x = vector.x - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 4].y = vector.y - camPos.y - self.player.sleepCurlUp * 4f + Mathf.Lerp(0.5f, 1f, self.player.aerobicLevel) * num *
                        (1f - num2);
                    sLeaser.sprites[sLeaser.sprites.Length - 4].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                    sLeaser.sprites[sLeaser.sprites.Length - 4].scaleX = 0.8f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f,
                        self.player.sleepCurlUp);
                    // lower jumprings, attached to body
                }
                if (!self.player.GetCat().GraphicsDisabled)
                {
                    // legs
                    sLeaser.sprites[sLeaser.sprites.Length - 10].x = vector7.x - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 10].y = vector7.y - camPos.y;
                    sLeaser.sprites[sLeaser.sprites.Length - 10].rotation = Custom.AimFromOneVectorToAnother(self.legsDirection, new Vector2(0f, 0f));
                    sLeaser.sprites[sLeaser.sprites.Length - 10].isVisible = true;
                    if (self.player.bodyMode == Player.BodyModeIndex.Stand)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.CorridorClimb)
                    {
                        int num5 = self.player.animationFrame;
                        if (num5 > 6)
                        {
                            num5 %= 6;
                            sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = -1f;
                        }
                        else
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = 1f;
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
                    {
                        if (self.player.animation == Player.AnimationIndex.StandOnBeam)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                        }
                        else if (self.player.animation == Player.AnimationIndex.ClimbOnBeam)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                            sLeaser.sprites[sLeaser.sprites.Length - 10].y = Mathf.Clamp(sLeaser.sprites[4].y, vector2.y - 6f - camPos.y, vector2.y + 4f - camPos.y);
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.WallClimb)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Default)
                    {
                        if (self.player.animation == Player.AnimationIndex.LedgeGrab)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Swimming)
                    {
                        if (self.player.animation == Player.AnimationIndex.DeepSwim)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].isVisible = false;
                        }
                        if (self.player.stun > 0)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 10].isVisible = false;
                        }
                    }

                    // head things
                    sLeaser.sprites[sLeaser.sprites.Length - 5].x = vector3.x - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 5].y = vector3.y - camPos.y;
                    sLeaser.sprites[sLeaser.sprites.Length - 5].rotation = num3;
                    sLeaser.sprites[sLeaser.sprites.Length - 5].scaleX = ((num3 < 0f) ? -1f : 1f);

                    // hips things
                    sLeaser.sprites[sLeaser.sprites.Length - 3].x = (vector2.x * 2f + vector.x) / 3f - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 3].y = (vector2.y * 2f + vector.y) / 3f - camPos.y - self.player.sleepCurlUp * 3f;
                    sLeaser.sprites[sLeaser.sprites.Length - 3].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(self.tail[0].lastPos, self.tail[0].pos,
                        timeStacker));
                    sLeaser.sprites[sLeaser.sprites.Length - 3].scaleY = 1f + self.player.sleepCurlUp * 0.2f;
                    sLeaser.sprites[sLeaser.sprites.Length - 3].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * num - 0.05f * self.malnourished;
                    // freckles
                    

                    // face things
                    if (self.player.sleepCurlUp > 0f)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = Mathf.Sign(vector.x - vector2.x);
                        sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = num3 * (1f - self.player.sleepCurlUp);
                    }
                    else if (self.owner.room != null && self.owner.EffectiveRoomGravity == 0f)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = num3;
                    }

                    else if (self.player.Consious)
                    {
                        if ((self.player.bodyMode == Player.BodyModeIndex.Stand && self.player.input[0].x != 0) ||
                            self.player.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = Mathf.Sign(vector.x - vector2.x);
                            }
                            else
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = ((num3 < 0f) ? -1f : 1f);
                            }
                            vector6.x = 0f;
                            sLeaser.sprites[sLeaser.sprites.Length - 1].y += 1f;
                        }
                        else
                        {
                            if (Mathf.Abs(vector6.x) < 0.1f)
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = ((num3 < 0f) ? -1f : 1f);
                            }
                            else
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = Mathf.Sign(vector6.x);
                            }
                        }
                        sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = 0f;
                    }
                    else
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = num3;
                    }
                    if (ModManager.CoopAvailable && self.player.bool1)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = num3 + 0.2f;
                    }
                    sLeaser.sprites[sLeaser.sprites.Length - 1].x = vector3.x + vector6.x - camPos.x;
                    sLeaser.sprites[sLeaser.sprites.Length - 1].y = vector3.y + vector6.y - 2f - camPos.y;

                    // arms things. keep beneath the rest to avoid strange errors
                    for (int j = 0; j < 2; j++)
                    {
                        Vector2 vector8 = Vector2.Lerp(self.hands[j].lastPos, self.hands[j].pos, timeStacker);
                        if (self.hands[j].mode != Limb.Mode.Retracted)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 6 - j].x = vector8.x - camPos.x;
                            sLeaser.sprites[sLeaser.sprites.Length - 6 - j].y = vector8.y - camPos.y;
                            float num6 = 4.5f / ((float)self.hands[j].retractCounter + 1f);
                            if ((self.player.animation == Player.AnimationIndex.StandOnBeam || self.player.animation ==
                                Player.AnimationIndex.BeamTip) && self.disbalanceAmount <= 40f &&
                                self.hands[j].mode == Limb.Mode.HuntRelativePosition)
                            {
                                num6 *= self.disbalanceAmount / 40f;
                            }
                            if (self.player.animation == Player.AnimationIndex.HangFromBeam)
                            {
                                num6 *= 0.5f;
                            }
                            num6 *= Mathf.Abs(Mathf.Cos(Custom.AimFromOneVectorToAnother(vector2, vector) / 360f * 3.1415927f * 2f));
                            Vector2 vector9;
                            vector9 = vector + Custom.RotateAroundOrigo(new Vector2((-1f + 2f * (float)j) * (num6 * 0.6f), -3.5f),
                                Custom.AimFromOneVectorToAnother(vector2, vector));
                            sLeaser.sprites[sLeaser.sprites.Length - 6 - j].rotation = Custom.AimFromOneVectorToAnother(vector8, vector9) + 90f;
                            if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 6 - j].scaleY = ((vector.x < vector2.x) ? -1f : 1f);
                            }
                            else if (self.player.bodyMode == Player.BodyModeIndex.WallClimb)
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 6 - j].scaleY = ((self.player.flipDirection == -1) ? -1f : 1f);
                            }
                            else
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 6 - j].scaleY = Mathf.Sign(Custom.DistanceToLine(vector8, vector, vector2));
                            }
                            if (self.player.animation == Player.AnimationIndex.HangUnderVerticalBeam)
                            {
                                sLeaser.sprites[sLeaser.sprites.Length - 6 - j].scaleY = ((j == 0) ? 1f : -1f);
                            }
                        }
                        sLeaser.sprites[sLeaser.sprites.Length - 6 - j].isVisible = (self.hands[j].mode != Limb.Mode.Retracted &&
                            ((self.player.animation != Player.AnimationIndex.ClimbOnBeam &&
                            self.player.animation != Player.AnimationIndex.ZeroGPoleGrab) || !self.hands[j].reachedSnapPosition));
                        if ((self.player.animation == Player.AnimationIndex.ClimbOnBeam || self.player.animation == Player.AnimationIndex.HangFromBeam ||
                            self.player.animation == Player.AnimationIndex.GetUpOnBeam || self.player.animation == Player.AnimationIndex.ZeroGPoleGrab) &&
                            self.hands[j].reachedSnapPosition)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 8 - j].x = vector8.x - camPos.x;
                            sLeaser.sprites[sLeaser.sprites.Length - 8 - j].y = vector8.y - camPos.y +
                                ((self.player.animation != Player.AnimationIndex.ClimbOnBeam &&
                                self.player.animation != Player.AnimationIndex.ZeroGPoleGrab) ? 3f : 0f);

                            sLeaser.sprites[sLeaser.sprites.Length - 8 - j].isVisible = true;
                        }
                        else
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 8 - j].isVisible = false;
                        }
                    }
                }
                   
                // VANILLA PLACEMENTS FOR REFERENCE ------------------------------------------------------------------------------------------------------------------------------------------------

                // BODY ------------------------------------------
                //sLeaser.sprites[0].x = vector.x - camPos.x;
                //sLeaser.sprites[0].y = vector.y - camPos.y - self.player.sleepCurlUp * 4f + Mathf.Lerp(0.5f, 1f, self.player.aerobicLevel) * num *
                //    (1f - num2);
                //sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                //sLeaser.sprites[0].scaleX = 1f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, num) * num2, 0.15f,
                //    self.player.sleepCurlUp);
                // HIPS ------------------------------------------
                // the 1f on scalex is what to edit for fatter or thinner slugcats.
                // note that these sprites are considered upside down and therefore referencing from them may cause errors.
                //sLeaser.sprites[1].x = (vector2.x * 2f + vector.x) / 3f - camPos.x;
                //sLeaser.sprites[1].y = (vector2.y * 2f + vector.y) / 3f - camPos.y - self.player.sleepCurlUp * 3f;
                //sLeaser.sprites[1].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(self.tail[0].lastPos, self.tail[0].pos,
                //    timeStacker));
                //sLeaser.sprites[1].scaleY = 1f + self.player.sleepCurlUp * 0.2f;
                //sLeaser.sprites[1].scaleX = 1f + self.player.sleepCurlUp * 0.2f + 0.05f * num - 0.05f * self.malnourished;




                // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                Color effectcol = new Color(0.87f, 0.39f, 0.33f);
                Color eyecol = new Color(0.7f, 0.2f, 0.31f);
                Color bodycol = new Color(0.89f, 0.79f, 0.6f);

                if (self.useJollyColor)
                {
                    effectcol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                    eyecol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 1);
                    bodycol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                }
                else if (PlayerGraphics.customColors != null)
                {
                    effectcol = PlayerGraphics.CustomColorSafety(2);
                    eyecol = PlayerGraphics.CustomColorSafety(1);
                    bodycol = PlayerGraphics.CustomColorSafety(0);
                }

                if (!self.player.GetCat().GraphicsDisabled)
                {

                    // applying colour ------------------------------
                    sLeaser.sprites[sLeaser.sprites.Length - 3].color = effectcol; // freckles
                    sLeaser.sprites[sLeaser.sprites.Length - 5].color = effectcol; // head
                    sLeaser.sprites[sLeaser.sprites.Length - 6].color = effectcol; // arm
                    sLeaser.sprites[sLeaser.sprites.Length - 7].color = effectcol; // arm
                    sLeaser.sprites[sLeaser.sprites.Length - 8].color = effectcol; // hand
                    sLeaser.sprites[sLeaser.sprites.Length - 9].color = effectcol; // hand
                    sLeaser.sprites[sLeaser.sprites.Length - 10].color = effectcol; // legs
                }
                
                if (!self.player.GetCat().RingsDisabled)
                {
                    Color saturatedpupil = effectcol;

                    // colour tweaked ------------------------------
                    if ((saturatedpupil.r >= saturatedpupil.b && saturatedpupil.r >= saturatedpupil.g) ||
                        (saturatedpupil.r == saturatedpupil.b && saturatedpupil.b == saturatedpupil.g && saturatedpupil.r == saturatedpupil.g) ||
                        (saturatedpupil.g > 0.8 && saturatedpupil.r > 0.8 && saturatedpupil.b > 0.8) ||
                        (saturatedpupil.g < 0.1 && saturatedpupil.r < 0.1 && saturatedpupil.b < 0.1))
                    {
                        // itll be red the most often
                        saturatedpupil.r = 1f;
                    }
                    else if (saturatedpupil.b > saturatedpupil.r && saturatedpupil.b >= saturatedpupil.g)
                    {
                        saturatedpupil.b = 1f;
                    }
                    else
                    {
                        saturatedpupil.g = 1f;
                    }


                    // animated colour ------------------------------
                    if (self.player.GetCat().UnbCyanjumpCountdown == 0)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = effectcol;
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = effectcol;
                        // jumprings

                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = saturatedpupil;
                        // his pupils normally become the effect colour, but super saturated
                    }
                    else
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = Color.Lerp(effectcol, bodycol,
                            (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = Color.Lerp(effectcol, bodycol,
                            (self.player.GetCat().UnbCyanjumpCountdown / 100f));

                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(saturatedpupil, eyecol,
                            (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                    }
                    // gives his jumprings that nice fade effect
                }


                self.player.GetCat().scalefrill.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                // end drawsprites
            }
        }

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (!(self.player.GetCat().GraphicsDisabled && self.player.GetCat().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && sLeaser != null && rCam != null &&
                self.player.GetCat().IsUnbound)
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
                    //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                    if (i == sLeaser.sprites.Length - 1)
                    {
                        // pupils
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[9]);
                        // move in front of face sprite
                    }
                    else if (i == sLeaser.sprites.Length - 2 || i == sLeaser.sprites.Length - 3 || i == sLeaser.sprites.Length - 4 ||
                        i == sLeaser.sprites.Length - 10)
                    {
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[4]);
                        // in front of legs
                    }
                    else if (i == sLeaser.sprites.Length - 5)
                    {
                        // eartips
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[3]);
                        // move in front of head sprite
                    }
                    else if (i == sLeaser.sprites.Length - 6)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[5]);
                        // move in front of arm sprites
                    }
                    else if (i == sLeaser.sprites.Length - 7)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[6]);
                        // move in front of arm sprites
                    }
                    else if (i == sLeaser.sprites.Length - 8)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[7]);
                        // move in front of hand sprites
                    }
                    else if (i == sLeaser.sprites.Length - 9)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[8]);
                        // move in front of hand sprites
                    }

                    // VANILLA ---------------------------------------------------------------------
                    else if ((i <= 6 || i >= 9) && i <= 9)
                    {
                        newContatiner.AddChild(sLeaser.sprites[i]);
                    }
                    else
                    {
                        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                    }
                }

                try
                {
                    // self.player.GetCat().scalefrill.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Midground"));
                }
                catch (Exception e)
                {
                    Debug.Log("Error trying to add scalefrills: " + e);
                    Debug.Log("Spritelength is " + sLeaser.sprites.Length);
                    Debug.Log("Spritelength should be around " + (24 + self.player.GetCat().scalefrill.numberOfSprites) + " or so");
                }
                // end
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);

            if (!(self.player.GetCat().GraphicsDisabled && self.player.GetCat().RingsDisabled) &&
                self != null && self.player != null &&
                self.player.GetCat().IsUnbound)
            {
                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 11
                    //+ self.player.GetCat().scalefrill.numberOfSprites
                    );

                sLeaser.sprites[sLeaser.sprites.Length - 10] = new FSprite("unbLegsA0", true);
                sLeaser.sprites[sLeaser.sprites.Length - 10].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 10].anchorY = 0.25f;
                // leggy

                sLeaser.sprites[sLeaser.sprites.Length - 2] = new FSprite("unbjumpHipsA", true);
                sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 3] = new FSprite("unbfreckleHipsA", true);
                sLeaser.sprites[sLeaser.sprites.Length - 3].shader = rCam.game.rainWorld.Shaders["Basic"];
                // hips

                sLeaser.sprites[sLeaser.sprites.Length - 4] = new FSprite("unbjumpBodyA", true);
                sLeaser.sprites[sLeaser.sprites.Length - 4].shader = rCam.game.rainWorld.Shaders["Basic"];
                // body

                sLeaser.sprites[sLeaser.sprites.Length - 5] = new FSprite("unbearHeadA0", true);
                sLeaser.sprites[sLeaser.sprites.Length - 5].shader = rCam.game.rainWorld.Shaders["Basic"];
                // head

                sLeaser.sprites[sLeaser.sprites.Length - 6] = new FSprite("unbsleevesPlayerArm0", true);
                sLeaser.sprites[sLeaser.sprites.Length - 6].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 6].anchorX = 0.9f;
                sLeaser.sprites[sLeaser.sprites.Length - 6].scaleY = -1f;
                sLeaser.sprites[sLeaser.sprites.Length - 7] = new FSprite("unbsleevesPlayerArm0", true);
                sLeaser.sprites[sLeaser.sprites.Length - 7].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 7].anchorX = 0.9f;
                sLeaser.sprites[sLeaser.sprites.Length - 8] = new FSprite("unbsleevesOnTopOfTerrainHand", true);
                sLeaser.sprites[sLeaser.sprites.Length - 8].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 9] = new FSprite("unbsleevesOnTopOfTerrainHand", true);
                sLeaser.sprites[sLeaser.sprites.Length - 9].shader = rCam.game.rainWorld.Shaders["Basic"];
                sLeaser.sprites[sLeaser.sprites.Length - 9].scaleX = -1f;
                // mittens, including anchors and base scales

                sLeaser.sprites[sLeaser.sprites.Length - 1] = new FSprite("unbpupFaceA0", true);
                sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                // pupils

                //try { sLeaser.sprites = new FSprite[sLeaser.sprites.Length - 11 - self.player.GetCat().scalefrill.numberOfSprites]; Debug.Log("Scalefrill index: " + (sLeaser.sprites.Length - 11 - self.player.GetCat().scalefrill.numberOfSprites));
                //    self.player.GetCat().scalefrill.InitiateSprites(sLeaser, rCam); }
                //catch (Exception e) { Debug.Log("Scalefrills failed to apply: " + e); Debug.Log("Scalefrill index: " + (sLeaser.sprites.Length - 11 - self.player.GetCat().scalefrill.numberOfSprites)); }

                Debug.Log("Number of sprites in array: " + (sLeaser.sprites.Length));
                // DONT FORGET TO RESIZE THE ARRAY
                self.AddToContainer(sLeaser, rCam, null);
            }
        }
        // end unbgraphics
    }
}
