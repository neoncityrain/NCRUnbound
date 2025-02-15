﻿using System;
using System.Linq;

namespace Unbound
{
    internal static class UnbGraphics
    {
        #region FAtlases
        static FAtlas unbsleevesarm;
        static FAtlas unbearhead;
        static FAtlas unbpupface;
        static FAtlas unbjumphips;
        static FAtlas unbjumpbody;
        static FAtlas unbfrecklehips;
        static FAtlas unbhead;
        static FAtlas rehead;
        static FAtlas reear;
        static FAtlas unbarm;
        static FAtlas unbmittenlegs;
        static FAtlas unblegs;
        #endregion
        #region sLeaser Sprite Variables
        static int unbSocksNum = ModManager.MSC ? 13 : 12;
        public static int unbJumprings1Num = ModManager.MSC ? 14 : 13;
        static int unbFreckleNum = ModManager.MSC ? 15 : 14;
        public static int unbJumprings2Num = ModManager.MSC ? 16 : 15;
        static int unbEarTips = ModManager.MSC ? 17 : 16;
        static int unbLeftMittens = ModManager.MSC ? 18 : 17;
        static int unbRightMittens = ModManager.MSC ? 19 : 18;
        static int unbLeftToes = ModManager.MSC ? 20 : 19;
        static int unbRightToes = ModManager.MSC ? 21 : 20;
        static int unbPupils = ModManager.MSC ? 22 : 21;

        static int ThisIsTheLengthOfMyMadness = 10; // update when adding more to above
        #endregion


        public static void MirrorSprite(this FSprite addon, FSprite original)
        {
            addon.SetPosition(original.GetPosition());
            addon.rotation = original.rotation;
            addon.scaleX = original.scaleX;
            addon.scaleY = original.scaleY;
            addon.alpha = original.alpha;
            addon.anchorX = original.anchorX;
            addon.anchorY = original.anchorY;

            if (original == null)
            {
                addon = null;
            }
            if ((original.isVisible && !addon.isVisible) || (addon.isVisible && !original.isVisible))
            {
                addon.isVisible = original.isVisible;
            }
        }

        public static void DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, 
            RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle)
            {

                #region Initiating Variables
                // INITIATING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                float breathaltered = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f); // breath-altered
                Vector2 bodytohips = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker); // positions from body to hips
                Vector2 hipstobody = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker); // positions from hips to body
                // when vector and vector2 are combined, the position should be the exact center of the body
                if (self.player.aerobicLevel > 0.5f)
                {
                    // when exhausted, maybe?
                    bodytohips += Custom.DirVec(hipstobody, bodytohips) * Mathf.Lerp(-1f, 1f, breathaltered) *
                        Mathf.InverseLerp(0.5f, 1f, self.player.aerobicLevel) * 0.5f;
                }
                bool rev = self.player.GetNCRunbound().Reverb;
                float bodyhipscenterish = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(hipstobody, bodytohips).y));
                #endregion
                #region Adding / Replacing Atlases
                // ADDING / REPLACING ATLAS THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                if (self.player.room.GetNCRRoom().atmosphereFloat > 0f && (self.player.airInLungs < 0.2f || self.player.Hypothermia > 0.8f) &&
                    self.player.Consious)
                {
                    sLeaser.sprites[9].element = Futile.atlasManager.GetElementWithName("FaceStunned");
                }

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
                if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    (unbearhead == null || reear == null))
                {
                    NCRDebug.Log("Unbound Eartip sprites missing!");
                }
                else if (!rev && !self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbearhead._elementsByName.TryGetValue("unbear" + head, out var eartip))
                {
                    sLeaser.sprites[unbEarTips].element = eartip;
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    reear._elementsByName.TryGetValue("revear" + head, out var earsss))
                {
                    sLeaser.sprites[unbEarTips].element = earsss;
                }
                // eartips
                if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    (unbhead == null || rehead == null))
                {
                    NCRDebug.Log("Unbound Head sprites missing!");
                }
                else if (!rev && !self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    unbhead._elementsByName.TryGetValue("unb" + head, out var headreplace))
                {
                    sLeaser.sprites[3].element = headreplace;
                }
                else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                    head != null && head.StartsWith("Head") &&
                    rehead._elementsByName.TryGetValue("rev" + head, out var hea))
                {
                    sLeaser.sprites[3].element = hea;
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

                if (!self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().IsUnbound)
                {
                    //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                    sLeaser.sprites[1].scaleX = 0.8f + self.player.sleepCurlUp * 0.2f + 0.05f * breathaltered - 0.05f * self.malnourished;
                    sLeaser.sprites[0].scaleX = 0.8f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * 
                        bodyhipscenterish, 0.15f, self.player.sleepCurlUp);
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
                if (!self.player.GetNCRunbound().GraphicsDisabled && rev)
                {
                    sLeaser.sprites[0].scale *= 0.9f;
                    sLeaser.sprites[1].scale *= 0.9f;
                }
                #endregion
                #region Mirroring
                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                MirrorSprite(sLeaser.sprites[unbJumprings2Num], sLeaser.sprites[0]);
                MirrorSprite(sLeaser.sprites[unbJumprings1Num], sLeaser.sprites[1]);
                MirrorSprite(sLeaser.sprites[unbFreckleNum], sLeaser.sprites[1]);
                MirrorSprite(sLeaser.sprites[unbEarTips], sLeaser.sprites[3]);
                MirrorSprite(sLeaser.sprites[unbSocksNum], sLeaser.sprites[4]);
                MirrorSprite(sLeaser.sprites[unbLeftMittens], sLeaser.sprites[5]);
                MirrorSprite(sLeaser.sprites[unbRightMittens], sLeaser.sprites[6]);
                MirrorSprite(sLeaser.sprites[unbLeftToes], sLeaser.sprites[7]);
                MirrorSprite(sLeaser.sprites[unbRightToes], sLeaser.sprites[8]);
                if (!rev) { MirrorSprite(sLeaser.sprites[unbPupils], sLeaser.sprites[9]); }
                #endregion
                #region Colours
                // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                Color effectcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.24f, 0.14f, 0.05f) :
                    (rev ? new Color(0.72f, 0.6f, 0.6f) : new Color(0.87f, 0.39f, 0.33f));
                Color eyecol = self.player.GetNCRunbound().IsTechnician ? new Color(0.42f, 0.21f, 0.18f) : 
                    (rev ? new Color(0.51f, 0.2f, 0.22f) : new Color(0.07f, 0.2f, 0.31f));
                Color bodycol = self.player.GetNCRunbound().IsTechnician ? new Color(0.91f, 0.8f, 0.53f) : 
                    (rev ? new Color(0.95f, 0.91f, 0.91f) : new Color(0.89f, 0.79f, 0.6f));
                Color pupilcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.26f, 0.09f, 0.08f) : effectcol;

                if (self.player.room.game.IsArenaSession && !self.player.GetNCRunbound().IsTechnician)
                {
                    switch (self.player.playerState.playerNumber)
                    {
                        case 0:
                            if (rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge)
                            {
                                if (!rev)
                                {
                                    effectcol = new Color(0.42f, 0.31f, 0.78f);
                                    eyecol = new Color(0.22f, 0.05f, 0.09f);
                                    bodycol = new Color(0.96f, 0.95f, 0.98f);
                                }
                            }
                            break;
                        case 1:
                            if (!rev)
                            {
                                effectcol = new Color(0.11f, 0.74f, 0.58f);
                                eyecol = new Color(0.48f, 14f, 0.07f);
                                bodycol = new Color(0.97f, 0.84f, 0.45f);
                            }
                            break;
                        case 2:
                            if (!rev)
                                {
                                effectcol = new Color(0.84f, 0.08f, 0.3f);
                                eyecol = new Color(0.12f, 0.21f, 0.27f);
                                bodycol = new Color(0.98f, 0.58f, 0.38f);
                            }
                            break;
                        case 3:
                            if (!rev)
                            {
                                effectcol = new Color(0.86f, 0.23f, 0.93f);
                                eyecol = new Color(0.62f, 0.75f, 0.97f);
                                bodycol = new Color(0.06f, 0.11f, 0.24f);
                            }
                            break;
                    }
                }
                else if (self.useJollyColor)
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

                if (self.player.GetNCRunbound().RGBRings)
                { 
                    effectcol = new HSLColor(Mathf.Sin(self.player.GetNCRunbound().RGBCounter / 200f), 1f, 0.75f).rgb;
                    pupilcol = effectcol;
                }

                if (((self.player.GetNCRunbound().effectColour == null || self.player.GetNCRunbound().effectColour != effectcol) &&
                    !self.player.GetNCRunbound().dontForceChangeEffectCol) || self.player.GetNCRunbound().recheckColour)
                {
                    self.player.GetNCRunbound().effectColour = effectcol;
                    if (self.player.GetNCRunbound().recheckColour) { self.player.GetNCRunbound().recheckColour = false; }
                }
                else
                {
                    effectcol = self.player.GetNCRunbound().effectColour;
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

                    // animated colour ------------------------------
                    if (self.player.GetNCRunbound().UnbCyanjumpCountdown == 0)
                    {
                        sLeaser.sprites[unbPupils].color = pupilcol;
                    }
                    else if (self.player.GetNCRunbound().DidTripleCyanJump)
                    {
                        // if he did a triple jump
                        sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, self.player.GetNCRunbound().IsUnbound ? eyecol : effectcol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown) / 140f);
                    }
                    else
                    {
                        sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f);
                    }
                    if (rev)
                    {
                        sLeaser.sprites[unbPupils].alpha = 0f;
                    }
                }

                if (!self.player.GetNCRunbound().WingscalesDisabled)
                {
                    // sLeaser.sprites[unbFrillStarts].color = effectcol;
                }


                if (!self.player.GetNCRunbound().RingsDisabled && !rev)
                {
                    // animated colour ------------------------------
                    if (self.player.GetNCRunbound().UnbCyanjumpCountdown == 0)
                    {
                        sLeaser.sprites[unbJumprings1Num].color = self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol;
                        sLeaser.sprites[unbJumprings2Num].color = self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol;
                        // jumprings

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
                        sLeaser.sprites[unbJumprings1Num].color = Color.Lerp(self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol,
                            self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 120f));
                        sLeaser.sprites[unbJumprings2Num].color = Color.Lerp(self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol,
                            self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 130f));

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
                        sLeaser.sprites[unbJumprings1Num].color = Color.Lerp(self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol, bodycol,
                            (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[unbJumprings2Num].color = Color.Lerp(self.player.GetNCRunbound().IsTechnician ? eyecol : effectcol, bodycol,
                            (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                    }
                    // gives his jumprings (and eyes) that nice fade effect

                }
                else if (!self.player.GetNCRunbound().RingsDisabled)
                {
                    // for rev only

                    sLeaser.sprites[unbJumprings1Num].color = effectcol;
                    sLeaser.sprites[unbJumprings2Num].color = effectcol;
                }
                #endregion

                // end drawsprites
            }
        }

        public static void AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && rCam != null && sLeaser != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle)
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

                    if (i == unbPupils)
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
                        if (i == 4)
                        {
                            sLeaser.sprites[4].MoveBehindOtherNode(sLeaser.sprites[0]);
                        }
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
                        if (ThisIsTheLengthOfMyMadness != 10) { ThisIsTheLengthOfMyMadness = 10; }
                        if (self.player.GetNCRunbound().RingsDisabled && !self.player.GetNCRunbound().GraphicsDisabled)
                        {
                            ThisIsTheLengthOfMyMadness -= 2;
                        }
                        else if (!self.player.GetNCRunbound().RingsDisabled && self.player.GetNCRunbound().GraphicsDisabled)
                        {
                            ThisIsTheLengthOfMyMadness -= 8;
                        }

                        if (ThisIsTheLengthOfMyMadness > 10) { ThisIsTheLengthOfMyMadness = 10; }
                        else if (ThisIsTheLengthOfMyMadness < 0) 
                        { 
                            ThisIsTheLengthOfMyMadness = 0;
                            self.player.GetNCRunbound().GraphicsDisabled = true;
                            self.player.GetNCRunbound().RingsDisabled = true;
                            NCRDebug.Log("ERROR WITH GRAPHICS FOUND, DISABLING THEM");
                        }

                        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + ThisIsTheLengthOfMyMadness);
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

        public static void InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, 
            RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);

            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && sLeaser != null && rCam != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle)
            {

                #region Unbound Exclusive
                try
                {
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

        public static void TailThangs(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self != null && self.owner != null && self.player != null && self.player.room != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && self.tail != null)
            {
                if (self.player.GetNCRunbound().Reverb)
                {
                    // owner, rad, connectionrad, connectedsegment, surfacefriction, airfriction, affectprevious, pullinpreviousposition
                    self.tail[0] = new TailSegment(self, 8f, 2f, null, 0.85f, 0.98f, 1f, true);
                    self.tail[1] = new TailSegment(self, 6f, 3.5f, self.tail[0], 0.85f, 0.95f, 0.5f, true);
                    self.tail[2] = new TailSegment(self, 4f, 3.5f, self.tail[1], 0.85f, 0.95f, 0.5f, true);
                    self.tail[3] = new TailSegment(self, 2f, 3.5f, self.tail[2], 0.85f, 0.93f, 0.5f, true);
                }
                else if (self.player.GetNCRunbound().IsOracle)
                {
                    // owner, rad, connectionrad, connectedsegment, surfacefriction, airfriction, affectprevious, pullinpreviousposition
                    self.tail = new TailSegment[5];
                    self.tail[0] = new TailSegment(self, 6f, 5f, null, 0.85f, 1f, 1f, true);
                    self.tail[1] = new TailSegment(self, 4f, 8f, self.tail[0], 0.85f, 1f, 0.7f, true);
                    self.tail[2] = new TailSegment(self, 2.5f, 8f, self.tail[1], 0.85f, 1f, 0.6f, true);
                    self.tail[3] = new TailSegment(self, 1f, 8f, self.tail[2], 0.85f, 1f, 0.5f, true);
                    self.tail[4] = new TailSegment(self, 1f, 6f, self.tail[3], 0.80f, 0.4f, 0.8f, true);
                }
                else
                {
                    // owner, rad, connectionrad, connectedsegment, surfacefriction, airfriction, affectprevious, pullinpreviousposition
                    // affectprevious is reversed, so higher numbers affect less... i think?
                    if (self.player.playerState.isPup)
                    {
                        self.tail[0] = new TailSegment(self, 4f, 2f, null, 0.8f, 1f, 1f, true);
                        self.tail[1] = new TailSegment(self, 6f, 3.5f, self.tail[0], 0.75f, 1f, 0.7f, true);
                        self.tail[2] = new TailSegment(self, 4f, 4f, self.tail[1], 0.75f, 0.97f, 0.5f, true);
                        self.tail[3] = new TailSegment(self, 2f, 4f, self.tail[2], 0.7f, 0.9f, 0.4f, true);
                    }
                    else
                    {
                        self.tail[0] = new TailSegment(self, 6f, 2.5f, null, 0.85f, 1f, 1f, true);
                        self.tail[1] = new TailSegment(self, 7f, 7f, self.tail[0], 0.8f, 1f, 0.7f, true);
                        self.tail[2] = new TailSegment(self, 5f, 6f, self.tail[1], 0.8f, 0.99f, 0.5f, true);
                        self.tail[3] = new TailSegment(self, 3f, 6f, self.tail[2], 0.75f, 0.97f, 0.4f, true);
                    }
                }
            }
        }

        public static void Init()
        {
            #region LoadAtlases
            unbsleevesarm ??= Futile.atlasManager.LoadAtlas("atlases/unbsleevesarm");
            unbarm ??= Futile.atlasManager.LoadAtlas("atlases/unbarm");
            unbpupface ??= Futile.atlasManager.LoadAtlas("atlases/unbpupface");
            unbfrecklehips ??= Futile.atlasManager.LoadAtlas("atlases/unbfrecklehips");
            unbjumphips ??= Futile.atlasManager.LoadAtlas("atlases/unbjumphips");
            unbjumpbody ??= Futile.atlasManager.LoadAtlas("atlases/unbjumpbody");
            unbearhead ??= Futile.atlasManager.LoadAtlas("atlases/unbearhead");
            rehead ??= Futile.atlasManager.LoadAtlas("atlases/revhead");
            reear ??= Futile.atlasManager.LoadAtlas("atlases/revearhead");
            unbhead ??= Futile.atlasManager.LoadAtlas("atlases/unbhead");
            unblegs ??= Futile.atlasManager.LoadAtlas("atlases/unblegs");
            unbmittenlegs ??= Futile.atlasManager.LoadAtlas("atlases/unbmittenlegs");
            // initiating atlases
            #endregion
        }

    }
}
