namespace Unbound
{
    internal class GammaVisuals
    {
        public delegate Color orig_OverseerMainColor(global::OverseerGraphics self);

        public static void Init()
        {
            Hook ktbmain = new Hook(typeof(global::OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance |
                BindingFlags.Public).GetGetMethod(), new Func<orig_OverseerMainColor,
                OverseerGraphics, Color>(OverseerGraphics_MainColor_get));
            // 0.29f, 0.39f, 0.47f is the main colour, 0.2f, 0.56f, 0.47f is the tendril colour, 0.13f, 0.15f, 0.18f is the eye colour
            // adjust as needed to look not like shit
            On.OverseerGraphics.ColorOfSegment += OverseerGraphics_ColorOfSegment;
            On.OverseerGraphics.DrawSprites += OverseerGraphics_DrawSprites;
            On.OverseerGraphics.DrawSprites -= Overseer_DrawspritesRemove;
            On.OverseerGraphics.InitiateSprites += OverseerGraphics_InitiateSprites;
            On.OverseerGraphics.InitiateSprites -= OverseerGraphics_RemoveSprites;
            On.CoralBrain.Mycelium.UpdateColor += Mycelium_UpdateColor;

            On.WorldLoader.GeneratePopulation += GammaID;
        }

        private static void GammaID(On.WorldLoader.orig_GeneratePopulation orig, WorldLoader self, bool fresh)
        {
            orig(self, fresh);
            if (self != null && self.world != null &&
                self.world.game.session.characterStats.name.value == "NCRunbound" &&
                (self.OverseerSpawnConditions(UnboundEnums.NCRUnbound) || self.world.region.name == "UW" ||
                (ModManager.MSC && (self.world.region.name == "LC" || self.world.region.name == "LM")) ||
                    // places that always spawn overseers, OR
                    (UnityEngine.Random.value < self.world.region.regionParams.overseersSpawnChance *
                    Mathf.InverseLerp(2f, 21f, (float)(self.game.session as StoryGameSession).saveState.cycleNumber + 10) &&
                    (self.game.session as StoryGameSession).saveState.cycleNumber != 0)))
            {
                WorldCoordinate offscreenden = new WorldCoordinate(self.world.offScreenDen.index, -1, -1, 0);
                AbstractCreature gammaguide = new AbstractCreature(self.world,
                    StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Overseer), null, offscreenden, new EntityID(-1, -7113131)); // gamma

                if (self.world.GetAbstractRoom(offscreenden).offScreenDen)
                {
                    self.world.GetAbstractRoom(offscreenden).entitiesInDens.Add(gammaguide);
                }
                else
                {
                    self.world.GetAbstractRoom(offscreenden).AddEntity(gammaguide);
                }
                int owner = 4;
                (gammaguide.abstractAI as OverseerAbstractAI).SetAsPlayerGuide(owner);

                if (self.world.region.name == "UW" || (ModManager.MSC && (self.world.region.name == "LC" || self.world.region.name == "LM")) ||
                    // places that always spawn overseers, OR
                    (UnityEngine.Random.value < self.world.region.regionParams.overseersSpawnChance *
                    Mathf.InverseLerp(2f, 21f, (float)(self.game.session as StoryGameSession).saveState.cycleNumber + 10) &&
                    (self.game.session as StoryGameSession).saveState.cycleNumber != 0)
                    // its not cycle one and the random value is LESS THAN the overseer spawn chance * cyclenumbermathstuff
                    )
                {
                    int num2 = UnityEngine.Random.Range(self.world.region.regionParams.overseersMin, self.world.region.regionParams.overseersMax);
                    for (int num3 = 0; num3 < num2; num3++)
                    {
                        self.world.offScreenDen.entitiesInDens.Add(new AbstractCreature(
                            self.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Overseer),
                            null, new WorldCoordinate(self.world.offScreenDen.index, -1, -1, 0), self.game.GetNewID()));
                    }
                }
            }
        }

        private static void Mycelium_UpdateColor(On.CoralBrain.Mycelium.orig_UpdateColor orig, CoralBrain.Mycelium self, Color newColor, float gradientStart, int spr, RoomCamera.SpriteLeaser sLeaser)
        {
            if (self != null && self.owner != null && self.owner.OwnerRoom != null &&
                self.owner.OwnerRoom.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.owner is OverseerGraphics && (self.owner as OverseerGraphics).overseer.PlayerGuide)
                {
                    self.color = newColor;
                    for (int i = 0; i < (sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length; i++)
                    {
                        float value = (float)i / (float)((sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length - 1);
                        (sLeaser.sprites[spr] as TriangleMesh).verticeColors[i] = Color.Lerp(self.color,
                            Custom.HSL2RGB(0.4888889f, 0.5f, 0.2f), Mathf.InverseLerp(gradientStart, 1f, value));
                    }
                    for (int j = 1; j < 3; j++)
                    {
                        (sLeaser.sprites[spr] as TriangleMesh).verticeColors[(sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length - j] =
                            new Color(0.2f, 0.76f, 0.57f);
                    }
                }
                else
                {
                    orig(self, newColor, gradientStart, spr, sLeaser);
                }
            }
            else
            {
                orig(self, newColor, gradientStart, spr, sLeaser);
            }
        }

        private static void OverseerGraphics_RemoveSprites(On.OverseerGraphics.orig_InitiateSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites[self.PupilSprite].color = new Color(0f, 0f, 0f, 0.5f);
        }

        private static void OverseerGraphics_InitiateSprites(On.OverseerGraphics.orig_InitiateSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.owner != null && self.overseer.room != null && self.overseer != null &&
                // making sure no values are null
                self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                sLeaser.sprites[self.PupilSprite].color = new Color(0.2f, 0.56f, 0.478f, 0.5f);

            }
            else
            {
                sLeaser.sprites[self.PupilSprite].color = new Color(0f, 0f, 0f, 0.5f);
            }
        }

        private static void OverseerGraphics_DrawSprites(On.OverseerGraphics.orig_DrawSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.owner.room != null && self.overseer != null && self != null &&
                self.owner.room.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0.2f, 0.56f, 0.47f), 0.5f);
                sLeaser.sprites[self.InnerGlowSprite].color = new Color(0.23f, 0.25f, 0.28f);
            }
            else if (self.owner.room != null && self.overseer != null)
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0f, 0f, 1f), 0.5f);
            }
        }

        private static void Overseer_DrawspritesRemove(On.OverseerGraphics.orig_DrawSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0f, 0f, 1f), 0.5f);
        }

        private static Color OverseerGraphics_ColorOfSegment(On.OverseerGraphics.orig_ColorOfSegment orig, OverseerGraphics self, float f, float timeStacker)
        {
            if (self.owner != null && self != null && self.overseer != null && self.overseer.room != null &&
                // making sure no values are null, because the game is a little bastard sometimes
                self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                return Color.Lerp(Color.Lerp(Custom.RGB2RGBA((self.MainColor + new Color(0.3f, 0.86f, 0.67f) +
                    self.earthColor * 8f) / 10f, 0.5f), Color.Lerp(self.MainColor, Color.Lerp(self.NeutralColor,
                    self.earthColor, Mathf.Pow(f, 2f)), 0.5f),
                    self.ExtensionOfSegment(f, timeStacker)), Custom.RGB2RGBA(self.MainColor, 0f),
                    Mathf.Lerp(self.overseer.lastDying, self.overseer.dying, timeStacker));
            }
            else
            {
                return orig(self, f, timeStacker);
            }
        }

        public static Color OverseerGraphics_MainColor_get(orig_OverseerMainColor orig, global::OverseerGraphics self)
        {
            if (self.owner != null && self.owner.room != null && self != null &&
                self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" &&
                self.overseer.PlayerGuide)
            {
                return new Color(0.29f, 0.59f, 0.87f);
            }
            else
            {
                return orig(self);
            }
        }
    }
}
