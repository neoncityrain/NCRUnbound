using DressMySlugcat;
using DressMySlugcat.Hooks;
using System.Linq;

namespace Unbound
{
    internal static class DMSUnboundTime
    {
        #region FAtlases
        static FAtlas unbjumphips;
        static FAtlas unbjumpbody;
        #endregion

        public static void Init()
        {
            #region LoadAtlases
            unbjumphips ??= Futile.atlasManager.LoadAtlas("atlases/unbjumphips");
            unbjumpbody ??= Futile.atlasManager.LoadAtlas("atlases/unbjumpbody");
            // initiating atlases
            #endregion
            #region DMS Setup
            SpriteDefinitions.AvailableSprites.Add(new SpriteDefinitions.AvailableSprite
            {
                Name = "UNBOUNDRINGS1",
                Description = "Upper Rings",
                GallerySprite = "unbjumpHipsA",
                RequiredSprites = new List<string> { "unbjumpHipsA" },
                Slugcats = new List<string> { "NCRunbound", "NCRtech" }
            });
            SpriteDefinitions.AvailableSprites.Add(new SpriteDefinitions.AvailableSprite
            {
                Name = "UNBOUNDRINGS2",
                Description = "Lower Rings",
                GallerySprite = "unbjumpBodyA",
                RequiredSprites = new List<string> { "unbjumpBodyA" },
                Slugcats = new List<string> { "NCRunbound", "NCRtech" }
            });
            #endregion
        }

        public static void Coloor(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, 
            RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);

            if (self.player.room != null && self.player != null && self != null && self.player.room.game != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {
                try
                {
                    #region Colours
                    // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                    Color effectcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.24f, 0.14f, 0.05f) : new Color(0.87f, 0.39f, 0.33f);
                    Color eyecol = self.player.GetNCRunbound().IsTechnician ? new Color(0.42f, 0.21f, 0.18f) : new Color(0.07f, 0.2f, 0.31f);
                    Color bodycol = self.player.GetNCRunbound().IsTechnician ? new Color(0.91f, 0.8f, 0.53f) : new Color(0.89f, 0.79f, 0.6f);
                    Color pupilcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.26f, 0.09f, 0.08f) : effectcol;

                    if (self.player.room.game.IsArenaSession && !self.player.GetNCRunbound().IsTechnician)
                    {
                        switch (self.player.playerState.playerNumber)
                        {
                            case 0:
                                if (rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge)
                                {
                                    effectcol = new Color(0.42f, 0.31f, 0.78f);
                                    eyecol = new Color(0.22f, 0.05f, 0.09f);
                                    bodycol = new Color(0.96f, 0.95f, 0.98f);
                                }
                                break;
                            case 1:
                                effectcol = new Color(0.11f, 0.74f, 0.58f);
                                eyecol = new Color(0.48f, 14f, 0.07f);
                                bodycol = new Color(0.97f, 0.84f, 0.45f);
                                break;
                            case 2:
                                effectcol = new Color(0.84f, 0.08f, 0.3f);
                                eyecol = new Color(0.12f, 0.21f, 0.27f);
                                bodycol = new Color(0.98f, 0.58f, 0.38f);
                                break;
                            case 3:
                                effectcol = new Color(0.86f, 0.23f, 0.93f);
                                eyecol = new Color(0.62f, 0.75f, 0.97f);
                                bodycol = new Color(0.06f, 0.11f, 0.24f);
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
                    if (self.player.GetNCRunbound().effectColour == null || self.player.GetNCRunbound().effectColour != effectcol)
                    {
                        self.player.GetNCRunbound().effectColour = effectcol;
                    }

                    if (!self.player.GetNCRunbound().RingsDisabled)
                    {
                        // animated colour ------------------------------
                        if (self.player.GetNCRunbound().UnbCyanjumpCountdown == 0)
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 1].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                            sLeaser.sprites[sLeaser.sprites.Length - 2].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                            // jumprings

                            if (sLeaser.sprites[sLeaser.sprites.Length - 1].shader != rCam.game.rainWorld.Shaders["Basic"])
                            {
                                try
                                {
                                    sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                                    sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                                }
                                catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                            }
                        }
                        else if (self.player.GetNCRunbound().DidTripleCyanJump)
                        {
                            // if he did a triple jump
                            sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 120f));
                            sLeaser.sprites[sLeaser.sprites.Length - 2].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 130f));

                            if (sLeaser.sprites[sLeaser.sprites.Length - 1].shader == rCam.game.rainWorld.Shaders["Basic"])
                            {
                                try
                                {
                                    sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                    sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                }
                                catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                            }
                        }
                        else
                        {
                            sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                            sLeaser.sprites[sLeaser.sprites.Length - 2].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                        }
                        // gives his jumprings (and eyes) that nice fade effect

                        sLeaser.sprites[sLeaser.sprites.Length - 1].alpha = 1;
                        sLeaser.sprites[sLeaser.sprites.Length - 2].alpha = 1;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Error applying colours: " + e);
                }
            }
        }

        public static void DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, 
            RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (!(self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {
                #region Adding / Replacing Atlases
                // ADDING / REPLACING ATLAS THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

                // HIPS THINGS
                string hips = sLeaser.sprites[1]?.element?.name;
                if (unbjumphips == null)
                {
                    NCRDebug.Log("Unbound LOWER Jumpring sprites missing!");
                }
                else if (!self.player.GetNCRunbound().RingsDisabled && hips != null && hips.StartsWith("Hips") &&
                    unbjumphips._elementsByName.TryGetValue("unbjump" + hips, out var jumprings))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 1].element = jumprings;
                }
                // lower jumprings

                // BODY THINGS
                string bodyget = sLeaser.sprites[0]?.element?.name;
                if (unbjumpbody == null)
                {
                    NCRDebug.Log("Unbound UPPER Jumpring sprites missing!");
                }
                else if (!self.player.GetNCRunbound().RingsDisabled && bodyget != null && bodyget.StartsWith("Body") &&
                    unbjumpbody._elementsByName.TryGetValue("unbjump" + bodyget, out var jumprings2))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 2].element = jumprings2;
                }
                // upper jumprings

                PlayerGraphicsHooks.InitiateCustomGraphics(self, sLeaser, rCam);
                #endregion
                #region Mirroring
                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                UnbGraphics.MirrorSprite(sLeaser.sprites[sLeaser.sprites.Length - 2], sLeaser.sprites[0]);
                UnbGraphics.MirrorSprite(sLeaser.sprites[sLeaser.sprites.Length - 1], sLeaser.sprites[1]);
                #endregion

                // end drawsprites
            }
        }

        public static void AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self,
            RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (!(self.player.GetNCRunbound().RingsDisabled) &&
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

                    if (i == sLeaser.sprites.Length - 1 || i == sLeaser.sprites.Length - 2)
                    {
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[4]);
                        // in front of legs
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
                        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 2);
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

            if (!(self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null && sLeaser != null && rCam != null &&
                (self.player.GetNCRunbound().IsUnbound || self.player.GetNCRunbound().IsTechnician))
            {
                #region Unbound Exclusive
                try
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 1] = new FSprite("unbjumpHipsA", true);
                    sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // hips

                    sLeaser.sprites[sLeaser.sprites.Length - 2] = new FSprite("unbjumpBodyA", true);
                    sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                    // body

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
        // end dmsunbound
    }
}
