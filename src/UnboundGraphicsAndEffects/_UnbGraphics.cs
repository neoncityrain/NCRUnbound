using Menu;
using System;

namespace Unbound
{
    internal class UnbGraphics
    {
        #region FAtlases
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
        #endregion
        #region sLeaser Sprite Variables
        static int unbSocksNum = ModManager.MSC ? 13 : 12;
        static int unbJumprings1Num = ModManager.MSC ? 14 : 13;
        static int unbFreckleNum = ModManager.MSC ? 15 : 14;
        static int unbJumprings2Num = ModManager.MSC ? 16 : 15;
        static int unbEarTips = ModManager.MSC ? 17 : 16;
        static int unbLeftMittens = ModManager.MSC ? 18 : 17;
        static int unbRightMittens = ModManager.MSC ? 19 : 18;
        static int unbLeftToes = ModManager.MSC ? 20 : 19;
        static int unbRightToes = ModManager.MSC ? 21 : 20;
        static int unbPupils = ModManager.MSC ? 22 : 21;

        static int ThisIsTheLengthOfMyMadness = 11; // update when adding more to above

        static int unbFrillStarts = ModManager.MSC ? 23 : 22;
        #endregion

        public static void Init()
        {
            On.PlayerGraphics.InitiateSprites += InitiateSprites;
            On.PlayerGraphics.AddToContainer += AddToContainer;
            On.PlayerGraphics.DrawSprites += DrawSprites;
            // unbound jumpring graphics, 1-4

            #region LoadAtlases
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
            #endregion
        }

        private static void DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam,
            float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {

                #region Initiating Variables
                // INITIATING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                float breathaltered = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f); // breath-altered
                Vector2 bodytohips = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker); // positions from body to hips
                Vector2 hipstobody = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker); // positions from hips to body
                // when vector and vector2 are combined, the position should be the exact center of the body
                Vector2 headposition = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker); // head position
                if (self.player.aerobicLevel > 0.5f)
                {
                    // when exhausted, maybe?
                    bodytohips += Custom.DirVec(hipstobody, bodytohips) * Mathf.Lerp(-1f, 1f, breathaltered) *
                        Mathf.InverseLerp(0.5f, 1f, self.player.aerobicLevel) * 0.5f;
                    headposition -= Custom.DirVec(hipstobody, bodytohips) * Mathf.Lerp(-1f, 1f, breathaltered) * Mathf.Pow(Mathf.InverseLerp(0.5f, 1f,
                        self.player.aerobicLevel), 1.5f) * 0.75f;
                }
                float bodyhipscenterish = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(hipstobody, bodytohips).y));
                Vector2 legspositionprobably = (hipstobody * 3f + bodytohips) / 4f;
                // body to hips position-ish. looks to be lower than normal, so probably about where the legs start
                float malnourishdependant = 1f - 0.2f * self.malnourished; // malnourished-dependent
                float tailstretch = 6f; // tail stretch value. idk why the default is 6 but shrug

                for (int i = 0; i < 4; i++)
                {
                    Vector2 tailposition = Vector2.Lerp(self.tail[i].lastPos, self.tail[i].pos, timeStacker); // determining tail position
                    Vector2 normalized = (tailposition - legspositionprobably).normalized; // switches the numbers into actual (x,y) format, should be the tail base
                    Vector2 perpendiculartonormalized = Custom.PerpendicularVector(normalized); // ?
                    float tailsizeish = Vector2.Distance(tailposition, legspositionprobably) / 5f; // the distance from tail position to body/hips/legs position
                    if (i == 0)
                    {
                        tailsizeish = 0f;
                    }
                    tailstretch = self.tail[i].StretchedRad;
                    legspositionprobably = tailposition; // sets the legs / tailbase value to be the same
                }
                float bodyhipsbasedonhead = Custom.AimFromOneVectorToAnother(Vector2.Lerp(hipstobody, bodytohips, 0.5f), headposition); // body-hips based on head
                int headishintversion = Mathf.RoundToInt(Mathf.Abs(bodyhipsbasedonhead / 360f * 34f)); // round the above to an integer
                Vector2 faceposition = Vector2.Lerp(self.lastLookDir, self.lookDirection, timeStacker) * 3f * (1f - self.player.sleepCurlUp); // face position
                if (self.player.sleepCurlUp > 0f)
                {
                    // if sneebing
                    headishintversion = 7; // ?
                    headishintversion = Custom.IntClamp((int)Mathf.Lerp(headishintversion, 4f, self.player.sleepCurlUp), 0, 8);
                    // body-hips position based on the head, but dependant on sleep pose and an integer?
                    bodyhipsbasedonhead = Mathf.Lerp(bodyhipsbasedonhead, 45f * Mathf.Sign(bodytohips.x - hipstobody.x), self.player.sleepCurlUp); // tailbase position dependant on sleep pose
                    headposition.y += 1f * self.player.sleepCurlUp; // head y position dependant on sleep pose
                    headposition.x += Mathf.Sign(bodytohips.x - hipstobody.x) * 2f * self.player.sleepCurlUp; // head x position dependant on sleep pose
                    faceposition.y -= 2f * self.player.sleepCurlUp; // face y position dependant on sleep pose
                    faceposition.x -= 4f * Mathf.Sign(bodytohips.x - hipstobody.x) * self.player.sleepCurlUp; // youll never fucking guess
                }
                else if (self.owner.room != null && self.owner.EffectiveRoomGravity == 0f)
                {
                    // if in 0g
                    headishintversion = 0; // good lerd num4 eludes me. maybe this is a head atlas value?
                }
                else if (self.player.Consious)
                {
                    if ((self.player.bodyMode == Player.BodyModeIndex.Stand && self.player.input[0].x != 0) ||
                        self.player.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            headishintversion = 7; // if crawling
                        }
                        else
                        {
                            headishintversion = 6; // if standing
                        }
                        faceposition.x = 0f; // face x position
                    }
                    else
                    {
                        Vector2 headminuships = headposition - hipstobody;
                        headminuships.x *= 1f - faceposition.magnitude / 3f; // lawd help me
                        headminuships = headminuships.normalized; // p = (p.x, p.y)
                    }
                }
                else
                {
                    faceposition *= 0f; // sets face y position to zero
                    headishintversion = 0;
                }
                if (ModManager.CoopAvailable && self.player.bool1)
                {
                    // if the player is a pup, except this doesn't entirely make sense but we aint worryin about it right now
                    headposition.y -= 1.9f;
                    bodyhipsbasedonhead = Mathf.Lerp(bodyhipsbasedonhead, 45f * Mathf.Sign(bodytohips.x - hipstobody.x), 0.7f);
                    faceposition.x -= 0.2f;
                }
                Vector2 MYLEGS = Vector2.Lerp(self.legs.lastPos, self.legs.pos, timeStacker); // legs position
                #endregion
                #region Adding / Replacing Atlases
                // ADDING / REPLACING ATLAS THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                // LEG THINGS
                string lego = sLeaser.sprites[4]?.element?.name;
                if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    unbmittenlegs == null)
                {
                    NCRDebug.Log("Unbound Socks sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    lego != null && lego.StartsWith("Legs") &&
                    unbmittenlegs._elementsByName.TryGetValue("unbmitten" + lego, out var leggy))
                {
                    sLeaser.sprites[unbSocksNum].element = leggy;
                }
                if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    unblegs == null)
                {
                    NCRDebug.Log("Unbound Leg sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    lego != null && lego.StartsWith("Legs") &&
                    unblegs._elementsByName.TryGetValue("unb" + lego, out var leggie))
                {
                    sLeaser.sprites[4].element = leggie;
                }

                // HEAD THINGS
                string head = sLeaser.sprites[3]?.element?.name;
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbearhead == null)
                {
                    NCRDebug.Log("Unbound Eartip sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbearhead._elementsByName.TryGetValue("unbear" + head, out var eartip))
                {
                    sLeaser.sprites[unbEarTips].element = eartip;
                }
                // eartips
                if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    unbhead == null)
                {
                    NCRDebug.Log("Unbound Head sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbhead._elementsByName.TryGetValue("unb" + head, out var headreplace))
                {
                    sLeaser.sprites[3].element = headreplace;
                }

                // ARM THINGS
                string larm = sLeaser.sprites[5]?.element?.name;
                string rarm = sLeaser.sprites[6]?.element?.name;
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbarm == null)
                {
                    NCRDebug.Log("Unbound Arm sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled && larm != null && larm.StartsWith("PlayerArm") &&
                    unbarm._elementsByName.TryGetValue("unb" + larm, out var leftreplace))
                {
                    sLeaser.sprites[5].element = leftreplace;
                }
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbarm != null && rarm != null && rarm.StartsWith("PlayerArm") &&
                    unbarm._elementsByName.TryGetValue("unb" + rarm, out var rightreplace))
                {
                    sLeaser.sprites[6].element = rightreplace;
                }
                // arm replacements
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbsleevesarm == null)
                {
                    NCRDebug.Log("Unbound Mitten sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled && larm != null && larm.StartsWith("PlayerArm") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + larm, out var larmreplace))
                {
                    sLeaser.sprites[unbLeftMittens].element = larmreplace;
                }
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbarm != null && rarm != null && rarm.StartsWith("PlayerArm") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + larm, out var rarmreplace))
                {
                    sLeaser.sprites[unbRightMittens].element = rarmreplace;
                }


                // HAND THINGS
                string lhand = sLeaser.sprites[7]?.element?.name;
                string rhand = sLeaser.sprites[8]?.element?.name;
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbarm != null && lhand != null && lhand.StartsWith("OnTopOf") &&
                    unbarm._elementsByName.TryGetValue("unb" + lhand, out var lhandreplace))
                {
                    sLeaser.sprites[7].element = lhandreplace;
                }
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbarm != null && rhand != null && rhand.StartsWith("OnTopOf") &&
                    unbarm._elementsByName.TryGetValue("unb" + rhand, out var rhandreplace))
                {
                    sLeaser.sprites[8].element = rhandreplace;
                }

                if (!self.player.GetNCRunbound().GraphicsDisabled && unbsleevesarm != null && lhand != null && lhand.StartsWith("OnTopOf") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + lhand, out var lsleeve))
                {
                    sLeaser.sprites[unbLeftToes].element = lsleeve;
                }
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbsleevesarm != null && rhand != null && rhand.StartsWith("OnTopOf") &&
                    unbsleevesarm._elementsByName.TryGetValue("unbsleeves" + rhand, out var rsleeve))
                {
                    sLeaser.sprites[unbRightToes].element = rsleeve;
                }

                // HIPS THINGS
                string hipwthekids = sLeaser.sprites[1]?.element?.name;
                if (!self.player.GetNCRunbound().GraphicsDisabled && unbfrecklehips == null)
                {
                    NCRDebug.Log("Unbound Freckle sprites missing!");
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    hipwthekids != null && hipwthekids.StartsWith("Hips") &&
                    unbfrecklehips._elementsByName.TryGetValue("unbfreckle" + hipwthekids, out var freck))
                {
                    sLeaser.sprites[unbFreckleNum].element = freck;
                }
                // body freckles
                if (!self.player.GetNCRunbound().RingsDisabled && unbjumphips == null)
                {
                    NCRDebug.Log("Unbound LOWER Jumpring sprites missing!");
                }
                else if (!self.player.GetNCRunbound().RingsDisabled && hipwthekids != null && hipwthekids.StartsWith("Hips") &&
                    unbjumphips._elementsByName.TryGetValue("unbjump" + hipwthekids, out var jumprings))
                {
                    sLeaser.sprites[unbJumprings1Num].element = jumprings;
                }
                // lower jumprings

                // BODY THINGS
                string bodyget = sLeaser.sprites[0]?.element?.name;
                if (!self.player.GetNCRunbound().RingsDisabled && unbjumpbody == null)
                {
                    NCRDebug.Log("Unbound UPPER Jumpring sprites missing!");
                }
                else if (!self.player.GetNCRunbound().RingsDisabled && bodyget != null && bodyget.StartsWith("Body") &&
                    unbjumpbody._elementsByName.TryGetValue("unbjump" + bodyget, out var jumprings2))
                {
                    sLeaser.sprites[unbJumprings2Num].element = jumprings2;
                }
                // upper jumprings

                // FACE THINGS
                string faceget = sLeaser.sprites[9]?.element?.name;
                if (!self.player.GetNCRunbound().RingsDisabled &&
                    unbpupface == null)
                {
                    NCRDebug.Log("Unbound Pupil sprites missing!");
                }
                else if (!self.player.GetNCRunbound().RingsDisabled &&
                    faceget != null && faceget.StartsWith("Face") &&
                    unbpupface._elementsByName.TryGetValue("unbpup" + faceget, out var pupils))
                {
                    sLeaser.sprites[unbPupils].element = pupils;
                }
                // pupils
                #endregion
                #region Vanilla Tweaks
                // VANILLA TWEAKING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                if (!self.player.GetNCRunbound().GraphicsDisabled)
                {
                    sLeaser.sprites[1].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * breathaltered - 0.05f * self.malnourished;
                    sLeaser.sprites[0].scaleX = 0.8f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * bodyhipscenterish, 0.15f,
                        self.player.sleepCurlUp);
                    // makes unbound thinner
                    sLeaser.sprites[10].alpha = 0f;
                    sLeaser.sprites[11].alpha = 0f;
                    // removes the mark and the marks glow
                    if (self.player.stun > 0)
                    {
                        sLeaser.sprites[4].isVisible = false;
                    }
                    // hides legs when stunned
                }
                #endregion
                #region Animating Things
                //  ANIMATED PLACEMENT THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                if (!self.player.GetNCRunbound().RingsDisabled)
                {
                    sLeaser.sprites[unbJumprings1Num].x = (hipstobody.x * 2f + bodytohips.x) / 3f - camPos.x;
                    sLeaser.sprites[unbJumprings1Num].y = (hipstobody.y * 2f + bodytohips.y) / 3f - camPos.y - self.player.sleepCurlUp * 3f;
                    sLeaser.sprites[unbJumprings1Num].rotation = Custom.AimFromOneVectorToAnother(bodytohips, Vector2.Lerp(self.tail[0].lastPos, self.tail[0].pos,
                        timeStacker));
                    sLeaser.sprites[unbJumprings1Num].scaleY = 1f + self.player.sleepCurlUp * 0.2f;
                    sLeaser.sprites[unbJumprings1Num].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * breathaltered - 0.05f * self.malnourished;
                    // lower jumprings, attached to hips

                    // body things
                    sLeaser.sprites[unbJumprings2Num].x = bodytohips.x - camPos.x;
                    sLeaser.sprites[unbJumprings2Num].y = bodytohips.y - camPos.y - self.player.sleepCurlUp * 4f + Mathf.Lerp(0.5f, 1f, self.player.aerobicLevel) * breathaltered *
                        (1f - bodyhipscenterish);
                    sLeaser.sprites[unbJumprings2Num].rotation = Custom.AimFromOneVectorToAnother(hipstobody, bodytohips);
                    sLeaser.sprites[unbJumprings2Num].scaleX = 0.8f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * bodyhipscenterish, 0.15f,
                        self.player.sleepCurlUp);
                    // lower jumprings, attached to body


                    // face things
                    if (self.player.sleepCurlUp > 0f)
                    {
                        sLeaser.sprites[unbPupils].scaleX = Mathf.Sign(bodytohips.x - hipstobody.x);
                        sLeaser.sprites[unbPupils].rotation = bodyhipsbasedonhead * (1f - self.player.sleepCurlUp);
                    }
                    else if (self.owner.room != null && self.owner.EffectiveRoomGravity == 0f)
                    {
                        sLeaser.sprites[unbPupils].rotation = bodyhipsbasedonhead;
                    }

                    else if (self.player.Consious)
                    {
                        if ((self.player.bodyMode == Player.BodyModeIndex.Stand && self.player.input[0].x != 0) ||
                            self.player.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                sLeaser.sprites[unbPupils].scaleX = Mathf.Sign(bodytohips.x - hipstobody.x);
                            }
                            else
                            {
                                sLeaser.sprites[unbPupils].scaleX = ((bodyhipsbasedonhead < 0f) ? -1f : 1f);
                            }
                            faceposition.x = 0f;
                            sLeaser.sprites[unbPupils].y += 1f;
                        }
                        else
                        {
                            if (Mathf.Abs(faceposition.x) < 0.1f)
                            {
                                sLeaser.sprites[unbPupils].scaleX = ((bodyhipsbasedonhead < 0f) ? -1f : 1f);
                            }
                            else
                            {
                                sLeaser.sprites[unbPupils].scaleX = Mathf.Sign(faceposition.x);
                            }
                        }
                        sLeaser.sprites[unbPupils].rotation = 0f;
                    }
                    else
                    {
                        sLeaser.sprites[unbPupils].rotation = bodyhipsbasedonhead;
                    }
                    if (ModManager.CoopAvailable && self.player.bool1)
                    {
                        sLeaser.sprites[unbPupils].rotation = bodyhipsbasedonhead + 0.2f;
                    }
                    sLeaser.sprites[unbPupils].x = headposition.x + faceposition.x - camPos.x;
                    sLeaser.sprites[unbPupils].y = headposition.y + faceposition.y - 2f - camPos.y;
                }

                if (!self.player.GetNCRunbound().WingscalesDisabled)
                {
                    // wingscales

                    //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                    Vector2 editedhead = faceposition;
                    bool facingopposite = bodyhipsbasedonhead < 0f;
                    editedhead.x += (facingopposite ? 20f : -20f);

                    sLeaser.sprites[unbFrillStarts].x = editedhead.x - camPos.x;
                    sLeaser.sprites[unbFrillStarts].y = editedhead.y - camPos.y;
                    sLeaser.sprites[unbFrillStarts].rotation = (Custom.AimFromOneVectorToAnother(editedhead,
                        Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker)) + (facingopposite ?
                        90f : -90f));
                    sLeaser.sprites[unbFrillStarts].scaleX = (facingopposite ? 1f : -1f);
                    sLeaser.sprites[unbFrillStarts].scaleY = ((Mathf.Lerp(2.5f, 15f, 0.78f)) /
                        (Futile.atlasManager.GetElementWithName("LizardScaleA3").sourcePixelSize.y));
                }

                if (!self.player.GetNCRunbound().GraphicsDisabled)
                {
                    // legs
                    sLeaser.sprites[unbSocksNum].x = MYLEGS.x - camPos.x;
                    sLeaser.sprites[unbSocksNum].y = MYLEGS.y - camPos.y;
                    sLeaser.sprites[unbSocksNum].rotation = Custom.AimFromOneVectorToAnother(self.legsDirection, new Vector2(0f, 0f));
                    sLeaser.sprites[unbSocksNum].isVisible = true;
                    if (self.player.bodyMode == Player.BodyModeIndex.Stand)
                    {
                        sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.CorridorClimb)
                    {
                        int num5 = self.player.animationFrame;
                        if (num5 > 6)
                        {
                            num5 %= 6;
                            sLeaser.sprites[unbSocksNum].scaleX = -1f;
                        }
                        else
                        {
                            sLeaser.sprites[unbSocksNum].scaleX = 1f;
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.ClimbingOnBeam)
                    {
                        if (self.player.animation == Player.AnimationIndex.StandOnBeam)
                        {
                            sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                        }
                        else if (self.player.animation == Player.AnimationIndex.ClimbOnBeam)
                        {
                            sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                            sLeaser.sprites[unbSocksNum].y = Mathf.Clamp(sLeaser.sprites[4].y, hipstobody.y - 6f - camPos.y,
                                hipstobody.y + 4f - camPos.y);
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.WallClimb)
                    {
                        sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Default)
                    {
                        if (self.player.animation == Player.AnimationIndex.LedgeGrab)
                        {
                            sLeaser.sprites[unbSocksNum].scaleX = (float)((self.player.flipDirection > 0) ? 1 : -1);
                        }
                    }
                    else if (self.player.bodyMode == Player.BodyModeIndex.Swimming)
                    {
                        if (self.player.animation == Player.AnimationIndex.DeepSwim)
                        {
                            sLeaser.sprites[unbSocksNum].isVisible = false;
                        }
                        if (self.player.stun > 0)
                        {
                            sLeaser.sprites[unbSocksNum].isVisible = false;
                        }
                    }

                    // head things
                    sLeaser.sprites[unbEarTips].x = headposition.x - camPos.x;
                    sLeaser.sprites[unbEarTips].y = headposition.y - camPos.y;
                    sLeaser.sprites[unbEarTips].rotation = bodyhipsbasedonhead;
                    sLeaser.sprites[unbEarTips].scaleX = ((bodyhipsbasedonhead < 0f) ? -1f : 1f);

                    // hips things
                    sLeaser.sprites[unbFreckleNum].x = (hipstobody.x * 2f + bodytohips.x) / 3f - camPos.x;
                    sLeaser.sprites[unbFreckleNum].y = (hipstobody.y * 2f + bodytohips.y) / 3f - camPos.y - self.player.sleepCurlUp * 3f;
                    sLeaser.sprites[unbFreckleNum].rotation = Custom.AimFromOneVectorToAnother(bodytohips,
                        Vector2.Lerp(self.tail[0].lastPos, self.tail[0].pos,
                        timeStacker));
                    sLeaser.sprites[unbFreckleNum].scaleY = 1f + self.player.sleepCurlUp * 0.2f;
                    sLeaser.sprites[unbFreckleNum].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * breathaltered - 0.05f * self.malnourished;
                    // freckles
                    


                    // arms things. keep beneath the rest to avoid strange errors
                    for (int j = 0; j < 2; j++)
                    {
                        Vector2 vector8 = Vector2.Lerp(self.hands[j].lastPos, self.hands[j].pos, timeStacker);
                        if (self.hands[j].mode != Limb.Mode.Retracted)
                        {
                            sLeaser.sprites[unbLeftMittens + j].x = vector8.x - camPos.x;
                            sLeaser.sprites[unbLeftMittens + j].y = vector8.y - camPos.y;
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
                            num6 *= Mathf.Abs(Mathf.Cos(Custom.AimFromOneVectorToAnother(hipstobody, bodytohips) / 360f * 3.1415927f * 2f));
                            Vector2 vector9;
                            vector9 = bodytohips + Custom.RotateAroundOrigo(new Vector2((-1f + 2f * (float)j) * (num6 * 0.6f), -3.5f),
                                Custom.AimFromOneVectorToAnother(hipstobody, bodytohips));
                            sLeaser.sprites[unbLeftMittens + j].rotation = Custom.AimFromOneVectorToAnother(vector8, vector9) + 90f;
                            if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                            {
                                sLeaser.sprites[unbLeftMittens + j].scaleY = ((bodytohips.x < hipstobody.x) ? -1f : 1f);
                            }
                            else if (self.player.bodyMode == Player.BodyModeIndex.WallClimb)
                            {
                                sLeaser.sprites[unbLeftMittens + j].scaleY = ((self.player.flipDirection == -1) ? -1f : 1f);
                            }
                            else
                            {
                                sLeaser.sprites[unbLeftMittens + j].scaleY = Mathf.Sign(Custom.DistanceToLine(vector8, bodytohips, hipstobody));
                            }
                            if (self.player.animation == Player.AnimationIndex.HangUnderVerticalBeam)
                            {
                                sLeaser.sprites[unbLeftMittens + j].scaleY = ((j == 0) ? 1f : -1f);
                            }
                        }
                        sLeaser.sprites[unbLeftMittens + j].isVisible = (self.hands[j].mode != Limb.Mode.Retracted &&
                            ((self.player.animation != Player.AnimationIndex.ClimbOnBeam &&
                            self.player.animation != Player.AnimationIndex.ZeroGPoleGrab) || !self.hands[j].reachedSnapPosition));
                        if ((self.player.animation == Player.AnimationIndex.ClimbOnBeam || self.player.animation == Player.AnimationIndex.HangFromBeam ||
                            self.player.animation == Player.AnimationIndex.GetUpOnBeam || self.player.animation == Player.AnimationIndex.ZeroGPoleGrab) &&
                            self.hands[j].reachedSnapPosition)
                        {
                            sLeaser.sprites[unbLeftToes + j].x = vector8.x - camPos.x;
                            sLeaser.sprites[unbLeftToes + j].y = vector8.y - camPos.y +
                                ((self.player.animation != Player.AnimationIndex.ClimbOnBeam &&
                                self.player.animation != Player.AnimationIndex.ZeroGPoleGrab) ? 3f : 0f);

                            sLeaser.sprites[unbLeftToes + j].isVisible = true;
                        }
                        else
                        {
                            sLeaser.sprites[unbLeftToes + j].isVisible = false;
                        }
                    }
                }
                #endregion
                #region Colours
                // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                Color effectcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.24f, 0.14f, 0.05f) : new Color(0.87f, 0.39f, 0.33f);
                Color eyecol = self.player.GetNCRunbound().IsTechnician ? new Color(0.42f, 0.21f, 0.18f) : new Color(0.07f, 0.2f, 0.31f);
                Color bodycol = self.player.GetNCRunbound().IsTechnician ? new Color(0.91f, 0.8f, 0.53f) : new Color(0.89f, 0.79f, 0.6f);
                Color pupilcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.26f, 0.09f, 0.08f) : effectcol;

                if (self.useJollyColor)
                {
                    effectcol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                    eyecol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 1);
                    bodycol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                }
                else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                {
                    effectcol = PlayerGraphics.CustomColorSafety(2);
                    eyecol = PlayerGraphics.CustomColorSafety(1);
                    bodycol = PlayerGraphics.CustomColorSafety(0);
                }

                if (!self.player.GetNCRunbound().GraphicsDisabled)
                {
                    sLeaser.sprites[unbFreckleNum].color = self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol; // freckles
                    sLeaser.sprites[unbEarTips].color = self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol; // head
                    sLeaser.sprites[unbLeftMittens].color = effectcol; // arm
                    sLeaser.sprites[unbRightMittens].color = effectcol; // arm
                    sLeaser.sprites[unbLeftToes].color = effectcol; // hand
                    sLeaser.sprites[unbRightToes].color = effectcol; // hand
                    sLeaser.sprites[unbSocksNum].color = effectcol; // legs
                }

                if (!self.player.GetNCRunbound().WingscalesDisabled)
                {
                    sLeaser.sprites[unbFrillStarts].color = effectcol;
                }

                if (!self.player.GetNCRunbound().RingsDisabled)
                {
                    if (self.player.GetNCRunbound().IsUnbound)
                    {
                        // colour tweaked ------------------------------
                        if ((pupilcol.r >= pupilcol.b && pupilcol.r >= pupilcol.g) ||
                            (pupilcol.r == pupilcol.b && pupilcol.b == pupilcol.g && pupilcol.r == pupilcol.g) ||
                            (pupilcol.g > 0.8 && pupilcol.r > 0.8 && pupilcol.b > 0.8) ||
                            (pupilcol.g < 0.1 && pupilcol.r < 0.1 && pupilcol.b < 0.1))
                        {
                            // itll be red the most often
                            pupilcol.r = 1f;
                        }
                        else if (pupilcol.b > pupilcol.r && pupilcol.b >= pupilcol.g)
                        {
                            pupilcol.b = 1f;
                        }
                        else
                        {
                            pupilcol.g = 1f;
                        }
                    }


                    // animated colour ------------------------------
                    if (self.player.GetNCRunbound().UnbCyanjumpCountdown == 0)
                    {
                        sLeaser.sprites[unbJumprings1Num].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                        sLeaser.sprites[unbJumprings2Num].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                        // jumprings

                        sLeaser.sprites[unbPupils].color = pupilcol;

                        if (sLeaser.sprites[unbJumprings1Num].shader != rCam.game.rainWorld.Shaders["Basic"])
                        {
                            try
                            {
                                sLeaser.sprites[unbJumprings1Num].shader = rCam.game.rainWorld.Shaders["Basic"];
                                sLeaser.sprites[unbJumprings2Num].shader = rCam.game.rainWorld.Shaders["Basic"];
                            }
                            catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                        }
                    }
                    else if (self.player.GetNCRunbound().DidTripleCyanJump)
                    {
                        // if he did a triple jump
                        sLeaser.sprites[unbJumprings1Num].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                            self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 120f));
                        sLeaser.sprites[unbJumprings2Num].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                            self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 130f));

                        sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, self.player.GetNCRunbound().IsUnbound ? eyecol : effectcol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown) / 140f);

                        if (sLeaser.sprites[unbJumprings1Num].shader == rCam.game.rainWorld.Shaders["Basic"])
                        {
                            try
                            {
                                sLeaser.sprites[unbJumprings1Num].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                sLeaser.sprites[unbJumprings2Num].shader = rCam.game.rainWorld.Shaders["Hologram"];
                            }
                            catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                        }
                    }
                    else
                    {
                        sLeaser.sprites[unbJumprings1Num].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                            (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[unbJumprings2Num].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                            (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));

                        sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f);
                    }
                    // gives his jumprings (and eyes) that nice fade effect

                }
                #endregion

                
                // end drawsprites
            }
        }

        private static void AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && rCam != null && sLeaser != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {

                try
                {
                    sLeaser.RemoveAllSpritesFromContainer();
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Error removing sprites from container: " + e);
                }

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

                    if (i == unbFrillStarts)
                    {
                        // wingscales!
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveBehindOtherNode(sLeaser.sprites[0]);
                        // move behind all other sprites
                    }
                    else if (i == unbPupils)
                    {
                        // pupils
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[9]);
                        // move in front of face sprite
                    }
                    else if (i == unbJumprings1Num || i == unbFreckleNum || i == unbJumprings2Num ||
                        i == unbSocksNum)
                    {
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[4]);
                        // in front of legs
                    }
                    else if (i == unbEarTips)
                    {
                        // eartips
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[3]);
                        // move in front of head sprite
                    }
                    else if (i == unbLeftMittens)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[5]);
                        // move in front of arm sprites
                    }
                    else if (i == unbRightMittens)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[6]);
                        // move in front of arm sprites
                    }
                    else if (i == unbLeftToes)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[7]);
                        // move in front of hand sprites
                    }
                    else if (i == unbRightToes)
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


                if (sLeaser.sprites.Length < 14)
                {
                    try
                    {
                        Array.Resize(ref sLeaser.sprites, (ModManager.MSC ? 13 : 12) + ThisIsTheLengthOfMyMadness);
                        if (self.player.GetNCRunbound().MoreDebug) { NCRDebug.Log("Array resize success!"); }
                    }
                    catch (Exception e)
                    {

                        NCRDebug.Log("Couldn't resize array: " + e);
                    }
                }

                // end
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        private static void InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);

            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && sLeaser != null && rCam != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {

                #region Unbound Exclusive
                try
                {
                    sLeaser.sprites[unbFrillStarts] = new FSprite("LizardScaleA3", true);
                    sLeaser.sprites[unbFrillStarts].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbFrillStarts].anchorY = 0.1f;
                    // frill scale one

                    sLeaser.sprites[unbSocksNum] = new FSprite("unbLegsA0", true);
                    sLeaser.sprites[unbSocksNum].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbSocksNum].anchorY = 0.25f;
                    // leggy

                    sLeaser.sprites[unbJumprings1Num] = new FSprite("unbjumpHipsA", true);
                    sLeaser.sprites[unbJumprings1Num].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbFreckleNum] = new FSprite("unbfreckleHipsA", true);
                    sLeaser.sprites[unbFreckleNum].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // hips

                    sLeaser.sprites[unbJumprings2Num] = new FSprite("unbjumpBodyA", true);
                    sLeaser.sprites[unbJumprings2Num].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // body

                    sLeaser.sprites[unbEarTips] = new FSprite("unbearHeadA0", true);
                    sLeaser.sprites[unbEarTips].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // head

                    sLeaser.sprites[unbLeftMittens] = new FSprite("unbsleevesPlayerArm0", true);
                    sLeaser.sprites[unbLeftMittens].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbLeftMittens].anchorX = 0.9f;
                    sLeaser.sprites[unbLeftMittens].scaleY = -1f;
                    sLeaser.sprites[unbRightMittens] = new FSprite("unbsleevesPlayerArm0", true);
                    sLeaser.sprites[unbRightMittens].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbRightMittens].anchorX = 0.9f;
                    sLeaser.sprites[unbLeftToes] = new FSprite("unbsleevesOnTopOfTerrainHand", true);
                    sLeaser.sprites[unbLeftToes].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbRightToes] = new FSprite("unbsleevesOnTopOfTerrainHand", true);
                    sLeaser.sprites[unbRightToes].shader = rCam.game.rainWorld.Shaders["Basic"];
                    sLeaser.sprites[unbRightToes].scaleX = -1f;
                    // mittens, including anchors and base scales

                    sLeaser.sprites[unbPupils] = new FSprite("unbpupFaceA0", true);
                    sLeaser.sprites[unbPupils].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // pupils
                    

                    // DONT FORGET TO RESIZE THE ARRAY
                    self.AddToContainer(sLeaser, rCam, null);
                }
                catch (Exception e)
                {
                    NCRDebug.Log("What the fuck Unbound!! " + e);
                }

                #endregion
                // end unbgraphics
            }
        }
    }
}
