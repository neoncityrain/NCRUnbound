namespace Unbound
{
    internal static class UnbGraphics
    {
        #region FAtlases
        static FAtlas UnboundTailPattern;
        // tail
        static FAtlas UnboundWingfade;
        static FAtlas UnboundWingbase;
        // wingscales
        static FAtlas UnboundJumpHips;
        static FAtlas UnboundJumpBody;
        // jumprings
        static FAtlas UnboundPawMittens;
        static FAtlas UnboundEartips;
        static FAtlas UnboundPupils;
        static FAtlas UnboundFreckleHips;
        static FAtlas UnboundArms;
        static FAtlas UnboundLegMittens;
        static FAtlas UnboundLegs;
        static FAtlas UnboundHead;
        static FAtlas ReverbHead;
        static FAtlas ReverbEartips;
        // misc graphics
        #endregion

        public static void GraphicsHooks()
        {
            On.PlayerGraphics.ApplyPalette += ApplyUnboundRingPalette;
            On.PlayerGraphics.InitiateSprites += InitiateUnboundGraphics;

            On.PlayerGraphics.DrawSprites += DrawUnboundGraphics;
            On.PlayerGraphics.AddToContainer += AddUnboundGraphicsToContainer;

            On.PlayerGraphics.ApplyPalette += ApplyWingscalePalette;
            On.PlayerGraphics.Update += WingscaleUpdate;

            On.PlayerGraphics.ctor += TailThangs;
        }

        private static void ApplyUnboundRingPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);

            if (self?.player?.room?.game != null &&
                (self.player.GetNCRunbound().IsTechnician || (self.player.GetNCRunbound().IsUnbound && !self.player.playerState.isGhost)))
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
                        var jumpringOne = self.player.GetNCRunbound().UnboundJumpringStartSprite;
                        var jumpringTwo = self.player.GetNCRunbound().UnboundJumpringStartSprite + 1;
                        // animated colour ------------------------------
                        if (self.player.GetNCRunbound().UnbCyanjumpCountdown == 0)
                        {
                            sLeaser.sprites[jumpringOne].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                            sLeaser.sprites[jumpringTwo].color = self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol;
                            // jumprings

                            if (sLeaser.sprites[jumpringOne].shader != rCam.game.rainWorld.Shaders["Basic"])
                            {
                                try
                                {
                                    sLeaser.sprites[jumpringOne].shader = rCam.game.rainWorld.Shaders["Basic"];
                                    sLeaser.sprites[jumpringTwo].shader = rCam.game.rainWorld.Shaders["Basic"];
                                }
                                catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                            }
                        }
                        else if (self.player.GetNCRunbound().DidTripleCyanJump)
                        {
                            // if he did a triple jump
                            sLeaser.sprites[jumpringOne].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 120f));
                            sLeaser.sprites[jumpringTwo].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol,
                                self.player.GetNCRunbound().IsUnbound ? eyecol : pupilcol, (self.player.GetNCRunbound().UnbCyanjumpCountdown / 130f));

                            if (sLeaser.sprites[jumpringOne].shader == rCam.game.rainWorld.Shaders["Basic"])
                            {
                                try
                                {
                                    sLeaser.sprites[jumpringOne].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                    sLeaser.sprites[jumpringTwo].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                }
                                catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                            }
                        }
                        else
                        {
                            sLeaser.sprites[jumpringOne].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                            sLeaser.sprites[jumpringTwo].color = Color.Lerp(self.player.GetNCRunbound().IsUnbound ? effectcol : eyecol, bodycol,
                                (self.player.GetNCRunbound().UnbCyanjumpCountdown / 100f));
                        }
                        // gives his jumprings (and eyes) that nice fade effect

                        sLeaser.sprites[jumpringOne].alpha = 1;
                        sLeaser.sprites[jumpringTwo].alpha = 1;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);

                    NCRDebug.Log("Error applying colours: " + e);
                    NCRDebug.LogException(e);
                }
            }
        }

        private static void WingscaleUpdate(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
        {
            orig(self);
            try
            {
                if (self?.player != null &&
                self.player.GetNCRunbound().wingscales != null)
                {
                    self.player.GetNCRunbound().wingscales.Update();
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Unbound Wingscale update error: " + e);
            }
        }

        private static void ApplyWingscalePalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            try
            {
                orig(self, sLeaser, rCam, palette);
            }
            catch (Exception e)
            {
                NCRDebug.Log("Orig palette error: " + e);
            }

            try
            {
                if (self?.player?.room?.game != null &&
                self.player.GetNCRunbound().wingscales != null &&
                ((self.player.GetNCRunbound().IsUnbound && !self.player.playerState.isGhost) || self.player.GetNCRunbound().IsTechnician)
                )
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
                                if (rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType !=
                                    MoreSlugcatsEnums.GameTypeID.Challenge)
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
                    #endregion

                    self.player.GetNCRunbound().wingscales.SetWingColors(bodycol, effectcol);
                    self.player.GetNCRunbound().wingscales.ApplyPalette(sLeaser, rCam, palette);
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error setting Unbound wing colours: " + e);
            }
        }

        private static void InitiateUnboundGraphics(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (self?.player?.room != null && sLeaser != null && rCam != null &&
                !(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled &&
                self.player.GetNCRunbound().WingscalesDisabled) &&
                // if NOT all graphics are disabled
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle &&
                // modcat who isnt oracle (ie unbound, reverb, technician)
                (!self.player.GetNCRunbound().IsUnbound || (self.player.GetNCRunbound().IsUnbound && !self.player.playerState.isGhost))
                // either not unbound, or IS unbound but ISNT a ghost
                )
            {
                // bluhhhh im gonna throw upppppppppppp. what the fuck ever
                var getUnb = self.player.GetNCRunbound();

                bool ringsDisabled = getUnb.RingsDisabled;
                bool wingscalesDisabled = getUnb.WingscalesDisabled;
                bool generalGraphicsDisabled = getUnb.GraphicsDisabled;

                try
                {
                    try
                    {
                        int spriteNumber;
                        spriteNumber = 13 + (!wingscalesDisabled ? getUnb.wingscales.numberOfSprites : 0);
                        self.gownIndex = spriteNumber - 1;

                        if (!ringsDisabled)
                        {
                            getUnb.UnboundJumpringStartSprite = spriteNumber;
                            spriteNumber += 2; // jumpring additions. this will always be two.
                        }
                        if (!generalGraphicsDisabled)
                        {
                            getUnb.GeneralGraphicStartSprite = spriteNumber;
                            spriteNumber += 6; // general graphic additions
                        }
                        if (!getUnb.TailDisabled)
                        {
                            spriteNumber += 1; // tail graphic
                            getUnb.TailPatternInt = spriteNumber;
                        }

                        self.firstMudSprite = spriteNumber;
                        spriteNumber = spriteNumber + self.mudSpriteCount;
                        // mud things



                        sLeaser.sprites = new FSprite[spriteNumber];
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Error setting Unbound sLeaser sprites: " + e);
                    }

                    // 0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                    sLeaser.sprites[0] = new FSprite("BodyA", true);
                    sLeaser.sprites[0].anchorY = 0.7894737f;
                    if (self.RenderAsPup)
                    {
                        sLeaser.sprites[0].scaleY = 0.5f;
                    }
                    sLeaser.sprites[1] = new FSprite("HipsA", true);

                    #region tail mesh
                    TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
                    {
                        new TriangleMesh.Triangle(0, 1, 2), new TriangleMesh.Triangle(1, 2, 3),
                        new TriangleMesh.Triangle(4, 5, 6), new TriangleMesh.Triangle(5, 6, 7),
                        new TriangleMesh.Triangle(8, 9, 10),new TriangleMesh.Triangle(9, 10, 11),
                        new TriangleMesh.Triangle(12, 13, 14), new TriangleMesh.Triangle(2, 3, 4),
                        new TriangleMesh.Triangle(3, 4, 5), new TriangleMesh.Triangle(6, 7, 8),
                        new TriangleMesh.Triangle(7, 8, 9), new TriangleMesh.Triangle(10, 11, 12),
                        new TriangleMesh.Triangle(11, 12, 13)
                    };
                    TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
                    sLeaser.sprites[2] = triangleMesh;
                    // tail mesh
                    #endregion

                    sLeaser.sprites[3] = new FSprite("HeadA0", true);
                    sLeaser.sprites[4] = new FSprite("LegsA0", true);
                    sLeaser.sprites[4].anchorY = 0.25f;
                    sLeaser.sprites[5] = new FSprite("PlayerArm0", true);
                    sLeaser.sprites[5].anchorX = 0.9f;
                    sLeaser.sprites[5].scaleY = -1f;
                    sLeaser.sprites[6] = new FSprite("PlayerArm0", true);
                    sLeaser.sprites[6].anchorX = 0.9f;
                    sLeaser.sprites[7] = new FSprite("OnTopOfTerrainHand", true);
                    sLeaser.sprites[8] = new FSprite("OnTopOfTerrainHand", true);
                    sLeaser.sprites[8].scaleX = -1f;
                    sLeaser.sprites[9] = new FSprite("FaceA0", true);

                    if (!wingscalesDisabled)
                    {
                        getUnb.wingscales.InitiateSprites(sLeaser, rCam, "UnboundWingbase", "UnboundWingfade");
                    }

                    try
                    {
                        if (!ringsDisabled)
                        {
                            sLeaser.sprites[getUnb.UnboundJumpringStartSprite + 1] = new FSprite("unbjumpHipsA", true);
                            sLeaser.sprites[getUnb.UnboundJumpringStartSprite + 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                            sLeaser.sprites[getUnb.UnboundJumpringStartSprite] = new FSprite("unbjumpBodyA", true);
                            sLeaser.sprites[getUnb.UnboundJumpringStartSprite].shader = rCam.game.rainWorld.Shaders["Basic"];
                        }
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Error with jumpring initiation: " + e);
                    }

                    try
                    {
                        if (!generalGraphicsDisabled)
                        {
                            var start = getUnb.GeneralGraphicStartSprite;
                            // 0-socks, 1-freckles, 2-ears, 3-leftarm, 4-rightarm, 5-pupils
                            sLeaser.sprites[start] = new FSprite("unbLegsA0", true);
                            sLeaser.sprites[start].shader = rCam.game.rainWorld.Shaders["Basic"];
                            sLeaser.sprites[start].anchorY = 0.25f;
                            // leg socks
                            sLeaser.sprites[start + 1] = new FSprite("unbfreckleHipsA", true);
                            sLeaser.sprites[start + 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                            // hip freckles
                            sLeaser.sprites[start + 2] = new FSprite("unbearHeadA0", true);
                            sLeaser.sprites[start + 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                            // eartips
                            sLeaser.sprites[start + 3] = new FSprite("unbsleevesPlayerArm0", true);
                            sLeaser.sprites[start + 3].shader = rCam.game.rainWorld.Shaders["Basic"];
                            sLeaser.sprites[start + 3].anchorX = 0.9f;
                            sLeaser.sprites[start + 3].scaleY = -1f;
                            sLeaser.sprites[start + 4] = new FSprite("unbsleevesPlayerArm0", true);
                            sLeaser.sprites[start + 4].shader = rCam.game.rainWorld.Shaders["Basic"];
                            sLeaser.sprites[start + 4].anchorX = 0.9f;
                            // mittens
                            sLeaser.sprites[start + 5] = new FSprite("unbpupFaceA0", true);
                            sLeaser.sprites[start + 5].shader = rCam.game.rainWorld.Shaders["Basic"];
                            // pupils
                        }
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Error with general graphic additions initiation: " + e);
                    }

                    if (!getUnb.TailDisabled)
                    {
                        try
                        {
                            TriangleMesh.Triangle[] pat = new TriangleMesh.Triangle[]
                            {
                                    new TriangleMesh.Triangle(0, 1, 2), new TriangleMesh.Triangle(1, 2, 3),
                                    new TriangleMesh.Triangle(4, 5, 6), new TriangleMesh.Triangle(5, 6, 7),
                                    new TriangleMesh.Triangle(8, 9, 10),new TriangleMesh.Triangle(9, 10, 11),
                                    new TriangleMesh.Triangle(12, 13, 14), new TriangleMesh.Triangle(2, 3, 4),
                                    new TriangleMesh.Triangle(3, 4, 5), new TriangleMesh.Triangle(6, 7, 8),
                                    new TriangleMesh.Triangle(7, 8, 9), new TriangleMesh.Triangle(10, 11, 12),
                                    new TriangleMesh.Triangle(11, 12, 13)
                            };
                            TriangleMesh patternMesh = new TriangleMesh("unbtail", pat, false, false);

                            sLeaser.sprites[getUnb.TailPatternInt] = patternMesh;
                            // tail pattern
                        }
                        catch (Exception e)
                        {
                            NCRDebug.Log("Unbound tailmesh error: " + e);
                        }
                    }


                    sLeaser.sprites[11] = new FSprite("pixel", true);
                    sLeaser.sprites[11].scale = 5f;
                    sLeaser.sprites[10] = new FSprite("Futile_White", true);
                    sLeaser.sprites[10].shader = rCam.game.rainWorld.Shaders["FlatLight"];

                    if (ModManager.MSC)
                    {
                        PlayerGraphics.Gown gown = self.gown;
                        if (gown != null)
                        {
                            gown.InitiateSprite(self.gownIndex, sLeaser, rCam);
                            // 16
                        }
                    }

                    MudUtils.MakeMudSprites(sLeaser, rCam, self.firstMudSprite, new int[] { 0, 1, 2, 3, 4, 5, 6 });
                    // 17(?)-24


                    self.AddToContainer(sLeaser, rCam, null);

                    // end unbgraphics

                    #region base.initsprites
                    if (self.DEBUGLABELS != null && self.DEBUGLABELS.Length != 0)
                    {
                        foreach (DebugLabel debugLabel in self.DEBUGLABELS)
                        {
                            rCam.ReturnFContainer("HUD").AddChild(debugLabel.label);
                        }
                    }
                    #endregion

                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);

                    NCRDebug.Log("Error initiating sprites for Unbound: " + e);
                    if (sLeaser?.sprites != null)
                    {
                        NCRDebug.Log("Player has " + sLeaser.sprites.Length.ToString() + " sprites.");
                    }
                    NCRDebug.Log("Player base sprite number is 24, with all DLC.");
                    NCRDebug.Log("The game expects Unbound to have this number of sprites: " +
                        (13 + (wingscalesDisabled ? 0 : getUnb.wingscales.numberOfSprites) +
                        (getUnb.GraphicsDisabled ? 0 : 6) +
                        (getUnb.RingsDisabled ? 0 : 2) +
                        (getUnb.TailDisabled ? 0 : 1) +
                        self.mudSpriteCount
                        ));

                    NCRDebug.Log("Base graphics number is: " + (ModManager.MSC ? 13 : 12));
                    if (!ringsDisabled) { NCRDebug.Log("Jumpring start sprite is: " + getUnb.UnboundJumpringStartSprite.ToString()); }
                    if (!wingscalesDisabled) { NCRDebug.Log("Wingscale start sprite is: " +
                        getUnb.wingscales.startSprite.ToString());
                        NCRDebug.Log("Wingscale number of sprites: " +
                            getUnb.wingscales.numberOfSprites.ToString());
                        NCRDebug.Log("Wingscale end sprite is: " + (getUnb.wingscales.startSprite +
                            getUnb.wingscales.numberOfSprites));
                    }
                    if (ModManager.MSC) { NCRDebug.Log("Gown sprite is: " + self.gownIndex); }
                    if (ModManager.Watcher) { NCRDebug.Log("Mud sprite is: " + self.firstMudSprite +
                        " to " + (self.firstMudSprite + self.mudSpriteCount)); }
                }
            }
            else
            {
                orig(self, sLeaser, rCam);
            }
        }

        private static void AddUnboundGraphicsToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (self != null && self.player != null && self.player.room != null && rCam != null && sLeaser != null &&
                // standard nullchecks as usual
                !(self.player.GetNCRunbound().RingsDisabled && self.player.GetNCRunbound().WingscalesDisabled &&
                self.player.GetNCRunbound().GraphicsDisabled) &&
                // not all graphics are disabled
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle &&
                // not oracle
                (!self.player.GetNCRunbound().IsUnbound || (self.player.GetNCRunbound().IsUnbound && !self.player.playerState.isGhost))
                // isnt playerghost
                )
            {
                var getUnb = self.player.GetNCRunbound(); 

                try
                {
                    sLeaser.RemoveAllSpritesFromContainer();
                }
                catch (Exception e)
                {
                    UnbHelperCode.GamebreakingError(e);

                    NCRDebug.Log("Error removing Unbound sprites from container: " + e);
                    if (sLeaser?.sprites != null)
                    {
                        NCRDebug.Log("Player has " + sLeaser.sprites.Length.ToString() + " sprites.");
                    }
                    NCRDebug.Log("Player base sprite number is 24, with all DLC.");
                    NCRDebug.Log("The game expects Unbound to have this number of sprites: " +
                        (13 + (getUnb.WingscalesDisabled ? 0 : getUnb.wingscales.numberOfSprites) +
                        (getUnb.GraphicsDisabled ? 0 : 7) +
                        (getUnb.RingsDisabled ? 0 : 2) +
                        self.mudSpriteCount
                        ));

                    NCRDebug.Log("Base graphics number is: " + (ModManager.MSC ? 13 : 12));
                    if (!getUnb.RingsDisabled) { NCRDebug.Log("Jumpring start sprite is: " +
                        getUnb.UnboundJumpringStartSprite.ToString()); }
                    if (!getUnb.WingscalesDisabled)
                    {
                        NCRDebug.Log("Wingscale start sprite is: " +
                        getUnb.wingscales.startSprite.ToString());
                        NCRDebug.Log("Wingscale number of sprites: " +
                            getUnb.wingscales.numberOfSprites.ToString());
                        NCRDebug.Log("Wingscale end sprite is: " + (getUnb.wingscales.startSprite +
                            getUnb.wingscales.numberOfSprites));
                    }
                    if (ModManager.MSC) { NCRDebug.Log("Gown sprite is: " + self.gownIndex); }
                    if (ModManager.Watcher)
                    {
                        NCRDebug.Log("Mud sprite is: " + self.firstMudSprite +
                        " to " + (self.firstMudSprite + self.mudSpriteCount));
                    }
                    NCRDebug.Log("This usually happens due to another mod adding to Unbound's graphics in some way.");
                    NCRDebug.Log("Please check if another mod adds to the container of every slugcat.");
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
                    // 0-socks, 1-freckles, 2-ears, 3-leftarm, 4-rightarm, 5-pupils
                    if (i == getUnb.TailPatternInt && !getUnb.TailDisabled)
                    {
                        // tail pattern
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[3]);
                        // move behind head sprite
                    }
                    else if (i == getUnb.GeneralGraphicStartSprite + 5 && !getUnb.GraphicsDisabled)
                    {
                        // pupils
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[9]);
                        // move in front of face sprite
                    }
                    else if ((!getUnb.RingsDisabled &&
                        (i == getUnb.UnboundJumpringStartSprite || i == getUnb.UnboundJumpringStartSprite + 1)) || // jumprings
                        (!getUnb.GraphicsDisabled &&
                        (i == getUnb.GeneralGraphicStartSprite || i == getUnb.GeneralGraphicStartSprite + 1)) // freckles and socks
                        )
                    {
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[4]);
                        // in front of legs
                    }
                    else if (!getUnb.GraphicsDisabled && i == getUnb.GeneralGraphicStartSprite + 2)
                    {
                        // eartips
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[3]);
                        // move in front of head sprite
                    }
                    else if (!getUnb.GraphicsDisabled && i == getUnb.GeneralGraphicStartSprite + 3)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[5]);
                        // move in front of arm sprites
                    }
                    else if (!getUnb.GraphicsDisabled && i == getUnb.GeneralGraphicStartSprite + 4)
                    {
                        // arm sleeves
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[6]);
                        // move in front of arm sprites
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

                if (!getUnb.WingscalesDisabled)
                {
                    try
                    {
                        self.player.GetNCRunbound().wingscales.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Midground"));
                    }
                    catch (Exception e)
                    {
                        UnbHelperCode.GamebreakingError(e);

                        NCRDebug.Log("Error adding Unbound Wingscales to container: " + e);
                        NCRDebug.LogException(e);
                    }
                }
                // end
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        private static void DrawUnboundGraphics(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (!(self.player.GetNCRunbound().RingsDisabled && self.player.GetNCRunbound().GraphicsDisabled &&
                self.player.GetNCRunbound().WingscalesDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat &&

                (!self.player.GetNCRunbound().IsUnbound || (self.player.GetNCRunbound().IsUnbound && !self.player.playerState.isGhost))
                )
            {
                var unbGet = self.player.GetNCRunbound();
                var graphicsStart = unbGet.GeneralGraphicStartSprite;

                #region Initiating Variables
                // INITIATING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                float breathaltered = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f); // breath-altered
                Vector2 bodytohips = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker); // positions from body to hips
                Vector2 hipstobody = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker); // positions from hips to body
                // when vector and vector2 are combined, the position should be the exact center of the body
                if (self.player.aerobicLevel > 0.5f)
                {
                    // if exhausted / doing a lot of physical activity
                    bodytohips += Custom.DirVec(hipstobody, bodytohips) * Mathf.Lerp(-1f, 1f, breathaltered) *
                        Mathf.InverseLerp(0.5f, 1f, self.player.aerobicLevel) * 0.5f;
                }
                bool isReverb = unbGet.IsReverb; // check if reverb is being played or not
                float bodyhipscenterish = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(hipstobody, bodytohips).y));
                #endregion

                #region Jumpring Atlases
                //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                if (!unbGet.IsOracle && !unbGet.RingsDisabled)
                {
                    // HIPS THINGS
                    string hips = sLeaser.sprites[1]?.element?.name;
                    if (UnboundJumpHips == null)
                    {
                        NCRDebug.Log("Unbound LOWER Jumpring sprites missing!");
                    }
                    else if (hips != null && hips.StartsWith("Hips") &&
                        UnboundJumpHips._elementsByName.TryGetValue("unbjump" + hips, out var jumprings))
                    {
                        sLeaser.sprites[unbGet.UnboundJumpringStartSprite].element = jumprings;
                    }
                    // lower jumprings

                    // BODY THINGS
                    string bodyget = sLeaser.sprites[0]?.element?.name;
                    if (UnboundJumpBody == null)
                    {
                        NCRDebug.Log("Unbound UPPER Jumpring sprites missing!");
                    }
                    else if (bodyget != null && bodyget.StartsWith("Body") &&
                        UnboundJumpBody._elementsByName.TryGetValue("unbjump" + bodyget, out var jumprings2))
                    {
                        sLeaser.sprites[unbGet.UnboundJumpringStartSprite + 1].element = jumprings2;
                    }
                    // upper jumprings
                }
                #endregion
                #region Wingscales
                if (!unbGet.WingscalesDisabled &&
                    (unbGet.IsTechnician || unbGet.IsUnbound))
                {
                    try
                    {
                        unbGet.wingscales.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                    }
                    catch (Exception e)
                    {
                        UnbHelperCode.GamebreakingError(e);

                        NCRDebug.Log("Error drawing Wingscale sprites for Unbound: " + e);
                        NCRDebug.LogException(e);
                    }
                }
                #endregion
                #region Tail
                if (!unbGet.TailDisabled)
                {
                    
                }
                #endregion
                #region Misc Graphics
                if (!unbGet.GraphicsDisabled)
                {
                    //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                    // 0-socks, 1-freckles, 2-ears, 3-leftarm, 4-rightarm, 5-pupils

                    // LEG THINGS
                    string legSprites = sLeaser.sprites[4]?.element?.name;
                    if (UnboundLegMittens == null)
                    {
                        NCRDebug.Log("Unbound Socks sprites missing!");
                    }
                    else if (legSprites != null && legSprites.StartsWith("Legs") &&
                        UnboundLegMittens._elementsByName.TryGetValue("unbmitten" + legSprites, out var unbSocks))
                    {
                        sLeaser.sprites[graphicsStart].element = unbSocks;
                    }
                    if (UnboundLegs == null)
                    {
                        NCRDebug.Log("Unbound Leg sprites missing!");
                    }
                    else if (legSprites != null && legSprites.StartsWith("Legs") &&
                        UnboundLegs._elementsByName.TryGetValue("unb" + legSprites, out var unbLegs))
                    {
                        sLeaser.sprites[4].element = unbLegs;
                    }

                    // HEAD THINGS
                    string headSprites = sLeaser.sprites[3]?.element?.name;
                    if (UnboundEartips == null || ReverbEartips == null)
                    {
                        NCRDebug.Log("Unbound Eartip sprites missing!");
                    }
                    else if (!isReverb &&
                        headSprites != null && headSprites.StartsWith("Head") &&
                        UnboundEartips._elementsByName.TryGetValue("unbear" + headSprites, out var unbEartip))
                    {
                        sLeaser.sprites[graphicsStart + 2].element = unbEartip;
                    }
                    else if (!self.player.GetNCRunbound().GraphicsDisabled &&
                        headSprites != null && headSprites.StartsWith("Head") &&
                        ReverbEartips._elementsByName.TryGetValue("revear" + headSprites, out var revEartip))
                    {
                        sLeaser.sprites[graphicsStart + 2].element = revEartip;
                    }
                    // eartips
                    if (!self.player.GetNCRunbound().GraphicsDisabled &&
                        (UnboundHead == null || ReverbHead == null))
                    {
                        NCRDebug.Log("Unbound Head sprites missing!");
                    }
                    else if (!isReverb &&
                        headSprites != null && headSprites.StartsWith("Head") &&
                        UnboundHead._elementsByName.TryGetValue("unb" + headSprites, out var unbHead))
                    {
                        sLeaser.sprites[3].element = unbHead;
                    }
                    else if (headSprites != null && headSprites.StartsWith("Head") &&
                        ReverbHead._elementsByName.TryGetValue("rev" + headSprites, out var revHead))
                    {
                        sLeaser.sprites[3].element = revHead;
                    }

                    // ARM THINGS
                    string leftArm = sLeaser.sprites[5]?.element?.name;
                    string rightArm = sLeaser.sprites[6]?.element?.name;
                    if (UnboundArms == null)
                    {
                        NCRDebug.Log("Unbound Arm sprites missing!");
                    }
                    else if (leftArm != null &&
                        leftArm.StartsWith("PlayerArm") &&
                        UnboundArms._elementsByName.TryGetValue("unb" + leftArm, out var leftreplace))
                    {
                        sLeaser.sprites[5].element = leftreplace;
                    }
                    if (UnboundArms != null && rightArm != null &&
                        rightArm.StartsWith("PlayerArm") &&
                        UnboundArms._elementsByName.TryGetValue("unb" + rightArm, out var rightreplace))
                    {
                        sLeaser.sprites[6].element = rightreplace;
                    }
                    // arm replacements
                    if (UnboundPawMittens == null)
                    {
                        NCRDebug.Log("Unbound Mitten sprites missing!");
                    }
                    else if (leftArm != null && leftArm.StartsWith("PlayerArm") &&
                        UnboundPawMittens._elementsByName.TryGetValue("unbsleeves" + leftArm, out var larmreplace))
                    {
                        sLeaser.sprites[graphicsStart + 3].element = larmreplace;
                    }
                    if (UnboundArms != null && rightArm != null && rightArm.StartsWith("PlayerArm") &&
                        UnboundPawMittens._elementsByName.TryGetValue("unbsleeves" + leftArm, out var rarmreplace))
                    {
                        sLeaser.sprites[graphicsStart + 4].element = rarmreplace;
                    }

                    // HIPS THINGS
                    string hipSprites = sLeaser.sprites[1]?.element?.name;
                    if (UnboundFreckleHips == null)
                    {
                        NCRDebug.Log("Unbound Freckle sprites missing!");
                    }
                    else if (hipSprites != null && hipSprites.StartsWith("Hips") &&
                        UnboundFreckleHips._elementsByName.TryGetValue("unbfreckle" + hipSprites, out var unbFreckles))
                    {
                        sLeaser.sprites[graphicsStart + 1].element = unbFreckles;
                    }
                    // body freckles

                    // FACE THINGS
                    string faceSprites = sLeaser.sprites[9]?.element?.name;
                    if (!self.player.GetNCRunbound().RingsDisabled &&
                        UnboundPupils == null)
                    {
                        NCRDebug.Log("Unbound Pupil sprites missing!");
                    }
                    else if (!self.player.GetNCRunbound().RingsDisabled &&
                        faceSprites != null && faceSprites.StartsWith("Face") &&
                        UnboundPupils._elementsByName.TryGetValue("unbpup" + faceSprites, out var unbPupils))
                    {
                        sLeaser.sprites[graphicsStart + 5].element = unbPupils;
                    }
                    // pupils
                }
                #endregion

                #region Vanilla Tweaks
                if (!unbGet.GraphicsDisabled)
                {
                    // VANILLA TWEAKING THINGS --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                    if (unbGet.IsOracle)
                    {
                        //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                        sLeaser.sprites[1].scaleX = 1.5f + self.player.sleepCurlUp * 0.2f + 0.05f * breathaltered - 0.05f * self.malnourished;
                        sLeaser.sprites[0].scaleX = 1.3f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) *
                            bodyhipscenterish, 0.15f, self.player.sleepCurlUp);
                        // makes oracle fatter. love and light on planet rain world

                        sLeaser.sprites[1].scaleY = 1.1f + self.player.sleepCurlUp * 0.2f;
                    }
                    if (unbGet.IsUnbound)
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
                    if (unbGet.IsReverb)
                    {
                        sLeaser.sprites[0].scale *= 0.9f;
                        sLeaser.sprites[1].scale *= 0.9f;
                    }
                    // makes reverb a lil smaller
                }
                
                #endregion
                #region Mirroring
                // 0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                // this should go beneath tweaks, to avoid any misc weirdness
                if (!unbGet.RingsDisabled)
                {
                    MirrorSprite(sLeaser.sprites[unbGet.UnboundJumpringStartSprite], sLeaser.sprites[0]);
                    MirrorSprite(sLeaser.sprites[unbGet.UnboundJumpringStartSprite + 1], sLeaser.sprites[1]);
                }
                if (!unbGet.GraphicsDisabled)
                {
                    // 0-socks, 1-freckles, 2-ears, 3-leftarm, 4-rightarm, 5-pupils
                    MirrorSprite(sLeaser.sprites[graphicsStart], sLeaser.sprites[4]);
                    MirrorSprite(sLeaser.sprites[graphicsStart + 1], sLeaser.sprites[1]);
                    MirrorSprite(sLeaser.sprites[graphicsStart + 2], sLeaser.sprites[3]);
                    MirrorSprite(sLeaser.sprites[graphicsStart + 3], sLeaser.sprites[5]);
                    MirrorSprite(sLeaser.sprites[graphicsStart + 4], sLeaser.sprites[6]);
                    if (!isReverb) { MirrorSprite(sLeaser.sprites[graphicsStart + 5], sLeaser.sprites[9]); }
                }
                #endregion

                #region Colours
                // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                var isTechnician = unbGet.IsTechnician;
                Color effectcol = isTechnician ? new Color(0.24f, 0.14f, 0.05f) :
                    (isReverb ? new Color(0.72f, 0.6f, 0.6f) : new Color(0.87f, 0.39f, 0.33f));
                Color eyecol = isTechnician ? new Color(0.42f, 0.21f, 0.18f) :
                    (isReverb ? new Color(0.51f, 0.2f, 0.22f) : new Color(0.07f, 0.2f, 0.31f));
                Color bodycol = isTechnician ? new Color(0.91f, 0.8f, 0.53f) :
                    (isReverb ? new Color(0.95f, 0.91f, 0.91f) : new Color(0.89f, 0.79f, 0.6f));
                Color pupilcol = isTechnician ? new Color(0.1f, 0.04f, 0.03f) :
                    (unbGet.IsReverb ? new Color(0.95f, 0.9f, 0.5f) : new Color(1f, 0f, 0f));

                if (self.player.room.game.IsArenaSession && !isTechnician)
                {
                    // if in a challenge / arena
                    switch (self.player.playerState.playerNumber)
                    {
                        case 0:
                            if (rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType !=
                                MoreSlugcatsEnums.GameTypeID.Challenge)
                            {
                                if (!isReverb)
                                {
                                    effectcol = new Color(0.42f, 0.31f, 0.78f);
                                    eyecol = new Color(0.22f, 0.05f, 0.09f);
                                    bodycol = new Color(0.96f, 0.95f, 0.98f);
                                    pupilcol = new Color(0.18f, 0.11f, 0.78f);
                                }
                            }
                            break;
                        case 1:
                            if (!isReverb)
                            {
                                effectcol = new Color(0.11f, 0.74f, 0.58f);
                                eyecol = new Color(0.48f, 14f, 0.07f);
                                bodycol = new Color(0.97f, 0.84f, 0.45f);
                                pupilcol = new Color(0.56f, 0.29f, 0.92f);
                            }
                            break;
                        case 2:
                            if (!isReverb)
                            {
                                effectcol = new Color(0.84f, 0.08f, 0.3f);
                                eyecol = new Color(0.12f, 0.21f, 0.27f);
                                bodycol = new Color(0.98f, 0.58f, 0.38f);
                                pupilcol = new Color(0.36f, 0.95f, 0.72f);
                            }
                            break;
                        case 3:
                            if (!isReverb)
                            {
                                effectcol = new Color(0.86f, 0.23f, 0.93f);
                                eyecol = new Color(0.62f, 0.75f, 0.97f);
                                bodycol = new Color(0.06f, 0.11f, 0.24f);
                                pupilcol = new Color(0.94f, 0.02f, 0.14f);
                            }
                            break;
                    }
                }
                else if (self.useJollyColor)
                {
                    effectcol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                    eyecol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 1);
                    bodycol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                    pupilcol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 3);
                }
                else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                {
                    effectcol = PlayerGraphics.CustomColorSafety(2);
                    eyecol = PlayerGraphics.CustomColorSafety(1);
                    bodycol = PlayerGraphics.CustomColorSafety(0);
                    pupilcol = PlayerGraphics.CustomColorSafety(3);
                }

                if (unbGet.RGBRings)
                {
                    effectcol = new HSLColor(Mathf.Sin(unbGet.RGBCounter / 200f), 1f, 0.75f).rgb;
                    pupilcol = effectcol;
                }

                if (((unbGet.effectColour == null || unbGet.effectColour != effectcol) &&
                    !unbGet.dontForceChangeEffectCol) || unbGet.recheckColour)
                {
                    unbGet.effectColour = effectcol;
                    if (unbGet.recheckColour) { unbGet.recheckColour = false; }
                }
                else
                {
                    effectcol = unbGet.effectColour;
                }

                if (!unbGet.IsOracle)
                {
                    if (!unbGet.RingsDisabled)
                    {
                        var jumpringOne = unbGet.UnboundJumpringStartSprite;
                        var jumpringTwo = unbGet.UnboundJumpringStartSprite + 1;
                        if (!isReverb)
                        {
                            // animated colour ------------------------------
                            if (unbGet.UnbCyanjumpCountdown == 0)
                            {
                                sLeaser.sprites[jumpringOne].color = unbGet.IsTechnician ? eyecol : effectcol;
                                sLeaser.sprites[jumpringTwo].color = unbGet.IsTechnician ? eyecol : effectcol;

                                if (sLeaser.sprites[jumpringOne].shader != rCam.game.rainWorld.Shaders["Basic"])
                                {
                                    try
                                    {
                                        sLeaser.sprites[jumpringOne].shader = rCam.game.rainWorld.Shaders["Basic"];
                                        sLeaser.sprites[jumpringTwo].shader = rCam.game.rainWorld.Shaders["Basic"];
                                    }
                                    catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                                }
                            }
                            else if (unbGet.DidTripleCyanJump)
                            {
                                if (isTechnician)
                                {
                                    sLeaser.sprites[jumpringOne].color = Color.Lerp(eyecol, pupilcol,
                                        (unbGet.UnbCyanjumpCountdown / 120f));
                                    sLeaser.sprites[jumpringTwo].color = Color.Lerp(eyecol, pupilcol,
                                        (unbGet.UnbCyanjumpCountdown / 130f));
                                }
                                else
                                {
                                    sLeaser.sprites[jumpringOne].color = Color.Lerp(effectcol, eyecol,
                                    (unbGet.UnbCyanjumpCountdown / 120f));
                                    sLeaser.sprites[jumpringTwo].color = Color.Lerp(effectcol, eyecol,
                                        (unbGet.UnbCyanjumpCountdown / 130f));
                                }

                                if (sLeaser.sprites[jumpringOne].shader == rCam.game.rainWorld.Shaders["Basic"])
                                {
                                    try
                                    {
                                        sLeaser.sprites[jumpringOne].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                        sLeaser.sprites[jumpringTwo].shader = rCam.game.rainWorld.Shaders["Hologram"];
                                    }
                                    catch (Exception e) { NCRDebug.Log("Shader error: " + e); }
                                }
                            }
                            else
                            {
                                if (isTechnician)
                                {
                                    sLeaser.sprites[jumpringOne].color = Color.Lerp(eyecol, bodycol,
                                        (unbGet.UnbCyanjumpCountdown / 100f));
                                    sLeaser.sprites[jumpringTwo].color = Color.Lerp(eyecol, bodycol,
                                        (unbGet.UnbCyanjumpCountdown / 100f));
                                    // changes color based on the cyan countdown
                                }
                                else
                                {
                                    sLeaser.sprites[jumpringOne].color = Color.Lerp(effectcol, bodycol,
                                        (unbGet.UnbCyanjumpCountdown / 100f));
                                    sLeaser.sprites[jumpringTwo].color = Color.Lerp(effectcol, bodycol,
                                        (unbGet.UnbCyanjumpCountdown / 100f));
                                }
                            }
                            // gives his jumprings (and eyes) that nice fade effect

                        }
                        else
                        {
                            // for rev only

                            if (unbGet.RevCryCooldown <= 0)
                            {
                                sLeaser.sprites[jumpringOne].color = effectcol;
                                sLeaser.sprites[jumpringTwo].color = effectcol;
                            }
                            else
                            {
                                sLeaser.sprites[jumpringOne].color = Color.Lerp(effectcol, pupilcol,
                                        (unbGet.RevCryCooldown / 100f));
                                sLeaser.sprites[jumpringTwo].color = Color.Lerp(effectcol, pupilcol,
                                    (unbGet.RevCryCooldown / 100f));
                            }
                        }
                    }
                    if (!unbGet.GraphicsDisabled)
                    {
                        var unbPupils = graphicsStart + 5;

                        // 0-socks, 1-freckles, 2-ears, 3-leftarm, 4-rightarm, 5-pupils
                        sLeaser.sprites[graphicsStart + 1].color = isTechnician ? eyecol : effectcol; // freckles
                        sLeaser.sprites[graphicsStart + 2].color = isTechnician ? eyecol : effectcol; // ears
                        sLeaser.sprites[graphicsStart + 3].color = effectcol; // arm
                        sLeaser.sprites[graphicsStart + 4].color = effectcol; // arm
                        sLeaser.sprites[graphicsStart].color = effectcol; // legs

                        // animated colour ------------------------------
                        if (unbGet.UnbCyanjumpCountdown == 0)
                        {
                            sLeaser.sprites[unbPupils].color = pupilcol;
                        }
                        else if (unbGet.DidTripleCyanJump)
                        {
                            // if he did a triple jump
                            sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, unbGet.IsUnbound ? eyecol : effectcol,
                                (unbGet.UnbCyanjumpCountdown) / 140f);
                        }
                        else
                        {
                            sLeaser.sprites[unbPupils].color = Color.Lerp(pupilcol, unbGet.IsUnbound ? effectcol : eyecol,
                                unbGet.UnbCyanjumpCountdown / 100f);
                        }

                        if (isReverb)
                        {
                            // simply hides the pupils, to avoid issues
                            sLeaser.sprites[unbPupils].alpha = 0f;
                        }
                    }
                    if (!unbGet.TailDisabled)
                    {
                        sLeaser.sprites[unbGet.TailPatternInt].color = effectcol; // tail
                    }
                }
                #endregion
                // end drawsprites
            }
        }

        public static void TailThangs(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self?.owner != null && self.player?.room != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && self.tail != null &&
                (self.player.slugcatStats.name.value != "NCRunbound" ||
                // either not unbound
                (!self.player.playerState.isGhost && self.player.slugcatStats.name.value == "NCRunbound"))
                // or NOT a ghost who IS unbound
                )
            {
                if (!self.player.GetNCRunbound().GraphicsDisabled)
                {
                    if (self.player.GetNCRunbound().IsReverb)
                    {
                        // owner, rad, connectionrad, connectedsegment, surfacefriction, airfriction, affectprevious, pullinpreviousposition
                        self.tail[0] = new TailSegment(self, 8f, 2f, null, 0.85f, 0.98f, 1f, true);
                        self.tail[1] = new TailSegment(self, 6f, 3.5f, self.tail[0], 0.85f, 0.95f, 0.5f, true);
                        self.tail[2] = new TailSegment(self, 4f, 3.5f, self.tail[1], 0.85f, 0.95f, 0.5f, true);
                        self.tail[3] = new TailSegment(self, 2f, 3.5f, self.tail[2], 0.85f, 0.93f, 0.5f, true);
                    }
                    else if (self.player.GetNCRunbound().IsOracle)
                    {
                        // owner, radius, connectionrad, connectedsegment, surfacefriction, airfriction, affectprevious, pullinpreviousposition
                        self.tail = new TailSegment[5];
                        self.tail[0] = new TailSegment(self, 8f, 2f, null, 0.8f, 1f, 1f, true);
                        self.tail[1] = new TailSegment(self, 7f, 4f, self.tail[0], 0.75f, 1f, 0.7f, true);
                        self.tail[2] = new TailSegment(self, 5f, 6f, self.tail[1], 0.75f, 0.98f, 0.6f, true);
                        self.tail[3] = new TailSegment(self, 3f, 7f, self.tail[2], 0.75f, 0.95f, 0.5f, true);
                        self.tail[4] = new TailSegment(self, 1.5f, 8f, self.tail[3], 0.70f, 0.9f, 0.1f, true);
                    }
                    else
                    {
                        // if unbound or technician

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
                if (!self.player.GetNCRunbound().WingscalesDisabled)
                {
                    try
                    {
                        // wingscales set
                        var wingscaleStart = ModManager.MSC ? 12 : 11;
                        // as msc adds the gown to the player container, the number at which wingscales start should change
                        self.player.GetNCRunbound().wingscales = new UnboundWingScales(self, wingscaleStart);
                    }
                    catch (Exception e)
                    {
                        UnbHelperCode.GamebreakingError(e);

                        NCRDebug.Log("Error setting wingscale sprites as existing: " + e);
                        NCRDebug.LogException(e);
                    }
                }
            }
        }

        public static void MirrorSprite(this FSprite addon, FSprite original)
        {
            try
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
            catch (Exception e)
            {
                UnbHelperCode.GamebreakingError(e);

                NCRDebug.Log("Error mirroring sprites: " + e);
            }
        }

        public static void Init()
        {
            #region LoadAtlases
            UnboundPawMittens ??= Futile.atlasManager.LoadAtlas("atlases/unbsleevesarm");
            UnboundArms ??= Futile.atlasManager.LoadAtlas("atlases/unbarm");
            UnboundPupils ??= Futile.atlasManager.LoadAtlas("atlases/unbpupface");
            UnboundFreckleHips ??= Futile.atlasManager.LoadAtlas("atlases/unbfrecklehips");
            UnboundJumpHips ??= Futile.atlasManager.LoadAtlas("atlases/unbjumphips");
            UnboundJumpBody ??= Futile.atlasManager.LoadAtlas("atlases/unbjumpbody");
            UnboundEartips ??= Futile.atlasManager.LoadAtlas("atlases/unbearhead");
            ReverbHead ??= Futile.atlasManager.LoadAtlas("atlases/revhead");
            ReverbEartips ??= Futile.atlasManager.LoadAtlas("atlases/revearhead");
            UnboundHead ??= Futile.atlasManager.LoadAtlas("atlases/unbhead");
            UnboundLegs ??= Futile.atlasManager.LoadAtlas("atlases/unblegs");
            UnboundLegMittens ??= Futile.atlasManager.LoadAtlas("atlases/unbmittenlegs");

            UnboundWingfade ??= Futile.atlasManager.LoadAtlas("atlases/unboundwingfade");
            UnboundWingbase ??= Futile.atlasManager.LoadAtlas("atlases/unboundwingbase");

            UnboundTailPattern ??= Futile.atlasManager.LoadAtlas("atlases/unbtail");
            // initiating atlases
            #endregion
        }

    }
}
