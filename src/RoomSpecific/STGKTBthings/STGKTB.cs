using System;
using static Unbound.GammaVisuals;

namespace Unbound
{
    internal class STGKTB
    {
        public delegate Color orig_OwneriteratorColor(Inspector self);
        // in general: 0.29f, 0.39f, 0.47f for KTB, 0.87f, 0.39f, 0.33f for STG. adjust as needed
        // et tu, liberum?

        public static void Init()
        {
            On.ZapCoil.InitiateSprites += RedZap;
            On.ZapCoilLight.ctor += RedLight;
            On.ZapCoil.DrawSprites += DrawRedzap;
            On.ZapCoil.ZapFlash.InitiateSprites += FlashCol;
            On.ZapCoil.ZapFlash.Update += UpdateGlowCol;

            Hook inspectorColourChange = new Hook(typeof(Inspector).GetProperty("TrueColor", BindingFlags.Instance |
                BindingFlags.Public).GetGetMethod(), new Func<orig_OwneriteratorColor,
                Inspector, Color>(InspektaCheka));
            On.MoreSlugcats.Inspector.InitiateGraphicsModule += initiateInspectorOwner;

            On.Oracle.ctor += setOracle;
            On.OracleGraphics.SkinColor += setUnboracleSkinColours;
            On.OracleGraphics.Gown.Color += unboracleGownColours;
            On.OracleGraphics.InitiateSprites += tweakCols;
            On.Room.ReadyForAI += readyForUnboracles;
            On.Oracle.OracleArm.ctor += ArmTweaks;
            On.Oracle.OracleArm.Update += UpdateArm;
        }

        private static void initiateInspectorOwner(On.MoreSlugcats.Inspector.orig_InitiateGraphicsModule orig, Inspector self)
        {
            if (self != null && self.room != null && self.room.abstractRoom != null &&
                self.ownerIterator == -1 && self.room.game.IsStorySession && self.room.world?.region != null &&
                self.room.world.region.name == "KTB")
            {
                if (self.room.abstractRoom.name.StartsWith("KTB_STG"))
                {
                    self.ownerIterator = -19207;
                }
                else
                {
                    self.ownerIterator = -11202;
                }
            }
            orig(self);
        }

        public static Color InspektaCheka(orig_OwneriteratorColor orig, Inspector self)
        {
            if (self.ownerIterator == -19207) // stg
            {
                return new Color(0.87f, 0.39f, 0.33f);
            }
            if (self.ownerIterator == -11202) // ktb
            {
                return new Color(0.29f, 0.39f, 0.47f);
            }
            return orig(self);
        }

        private static void UpdateArm(On.Oracle.OracleArm.orig_Update orig, Oracle.OracleArm self)
        {
            if (self.oracle.room.world.name == "STG")
            {
                if (self.oracle.Consious)
                {
                    float num = 1f;
                    if (ModManager.MSC)
                    {
                        if (self.oracle.dazed > 240f)
                        {
                            num = 0f;
                        }
                        else
                        {
                            num = 1f - self.oracle.dazed / 240f;
                        }
                    }
                    for (int i = 0; i < self.oracle.bodyChunks.Length; i++)
                    {
                        self.oracle.bodyChunks[i].vel *= 0.4f;
                    }
                    self.oracle.bodyChunks[0].vel += Vector2.ClampMagnitude(self.oracle.oracleBehavior.OracleGetToPos -
                        self.oracle.bodyChunks[0].pos, 100f) / 100f * 6.2f * num;
                    for (int j = 1; j < self.oracle.bodyChunks.Length; j++)
                    {
                        self.oracle.bodyChunks[j].vel += Vector2.ClampMagnitude(self.oracle.oracleBehavior.OracleGetToPos -
                            self.oracle.oracleBehavior.GetToDir * self.oracle.bodyChunkConnections[0].distance -
                            self.oracle.bodyChunks[0].pos, 100f) / 100f * 3.2f * num;
                    }
                }
                Vector2 baseGetToPos = self.oracle.oracleBehavior.BaseGetToPos;
                Vector2 vector;
                vector = new Vector2(Mathf.Clamp(baseGetToPos.x, self.cornerPositions[0].x, self.cornerPositions[1].x), self.cornerPositions[0].y);
                float num2 = Vector2.Distance(vector, baseGetToPos);
                float num3 = Mathf.InverseLerp(self.cornerPositions[0].x, self.cornerPositions[1].x, baseGetToPos.x);
                for (int k = 1; k < 4; k++)
                {
                    Vector2 vector2;
                    if (k % 2 == 0)
                    {
                        vector2 = new Vector2(Mathf.Clamp(baseGetToPos.x, self.cornerPositions[0].x, self.cornerPositions[1].x),
                            self.cornerPositions[k].y);
                    }
                    else
                    {
                        vector2 = new Vector2(self.cornerPositions[k].x, Mathf.Clamp(baseGetToPos.y, self.cornerPositions[2].y,
                            self.cornerPositions[0].y));
                    }
                    float num4 = Vector2.Distance(vector2, baseGetToPos);
                    if (num4 < num2)
                    {
                        vector = vector2;
                        num2 = num4;
                        if (k == 1)
                        {
                            num3 = (float)k + Mathf.InverseLerp(self.cornerPositions[0].y, self.cornerPositions[2].y, baseGetToPos.y);
                        }
                        else if (k == 2)
                        {
                            num3 = (float)k + Mathf.InverseLerp(self.cornerPositions[1].x, self.cornerPositions[0].x, baseGetToPos.x);
                        }
                        else if (k == 3)
                        {
                            num3 = (float)k + Mathf.InverseLerp(self.cornerPositions[2].y, self.cornerPositions[0].y, baseGetToPos.y);
                        }
                    }
                }
                self.baseMoving = (Vector2.Distance(self.BasePos(1f), vector) > (self.baseMoving ? 50f : 350f) &&
                    self.oracle.oracleBehavior.consistentBasePosCounter > 30);
                self.lastFramePos = self.framePos;
                if (self.baseMoving)
                {
                    self.framePos = Mathf.MoveTowardsAngle(self.framePos * 90f, num3 * 90f, 1f) / 90f;
                    if (self.baseMoveSoundLoop != null)
                    {
                        self.baseMoveSoundLoop.volume = Mathf.Min(self.baseMoveSoundLoop.volume + 0.1f, 1f);
                        self.baseMoveSoundLoop.pitch = Mathf.Min(self.baseMoveSoundLoop.pitch + 0.025f, 1f);
                    }
                }
                else if (self.baseMoveSoundLoop != null)
                {
                    self.baseMoveSoundLoop.volume = Mathf.Max(self.baseMoveSoundLoop.volume - 0.1f, 0f);
                    self.baseMoveSoundLoop.pitch = Mathf.Max(self.baseMoveSoundLoop.pitch - 0.025f, 0.5f);
                }
                if (self.baseMoveSoundLoop != null)
                {
                    self.baseMoveSoundLoop.pos = self.BasePos(1f);
                    self.baseMoveSoundLoop.Update();
                    if (ModManager.MSC)
                    {
                        self.baseMoveSoundLoop.volume *= 1f - self.oracle.noiseSuppress;
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void ArmTweaks(On.Oracle.OracleArm.orig_ctor orig, Oracle.OracleArm self, Oracle oracle)
        {
            orig(self, oracle);
            if (oracle?.room?.world?.name == "KTB")
            {
                self.baseMoveSoundLoop = new StaticSoundLoop(SoundID.SS_AI_Base_Move_LOOP, oracle.firstChunk.pos,
                    oracle.room, 0.9f, oracle.ID == UnboundEnums.NCRSTG ? 1.02f : 0.86f);
            }
        }

        private static void readyForUnboracles(On.Room.orig_ReadyForAI orig, Room self)
        {
            orig(self);

            try
            {
                if (self?.game != null && self.game.IsStorySession && self.abstractRoom != null &&
                self.abstractRoom.name == "KTB_STGai")
                {
                    Oracle oracle = new Oracle(new AbstractPhysicalObject(self.world,
                        AbstractPhysicalObject.AbstractObjectType.Oracle, null, new WorldCoordinate(
                            self.abstractRoom.index, 15, 15, -1), self.game.GetNewID()), self);
                    self.AddObject(oracle);
                    self.waitToEnterAfterFullyLoaded = Math.Max(self.waitToEnterAfterFullyLoaded, 80);
                }
            }
            catch (Exception e) { NCRDebug.LogException(e); }
        }

        private static void tweakCols(On.OracleGraphics.orig_InitiateSprites orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self?.oracle?.ID == UnboundEnums.NCRSTG)
            {
                for (int l = 0; l < 2; l++)
                {
                    sLeaser.sprites[self.EyeSprite(l)].color = new Color(1f, 0f, 0f); // bright red. shrimple
                }
            }
            else if (self?.oracle?.ID == UnboundEnums.NCRKTB)
            {
                for (int l = 0; l < 2; l++)
                {
                    sLeaser.sprites[self.EyeSprite(l)].color = new Color(0.2f, 0.56f, 0.478f); // gamma green
                }
            }
        }

        private static Color unboracleGownColours(On.OracleGraphics.Gown.orig_Color orig, OracleGraphics.Gown self, float f)
        {
            if (self?.owner?.oracle?.ID == UnboundEnums.NCRSTG)
            {
                return new Color(0.89f, 0.8f, 0.8f); // pale pink
            }
            else if (self?.owner?.oracle?.ID == UnboundEnums.NCRKTB)
            {
                return new Color(0.29f, 0.59f, 0.87f); // gamma-blue
            }
            else if (self?.owner?.oracle?.ID == UnboundEnums.NCRARK)
            {

            }
            return orig(self, f);
        }

        private static Color setUnboracleSkinColours(On.OracleGraphics.orig_SkinColor orig, OracleGraphics self)
        {
            if (self?.oracle?.ID == UnboundEnums.NCRSTG)
            {
                return new Color(0.25f, 0.24f, 0.24f); // neutral gray
            }
            else if (self?.oracle?.ID == UnboundEnums.NCRKTB)
            {
                return new Color(0.21f, 0.24f, 0.26f); // cold gray
            }
            else if (self?.oracle?.ID == UnboundEnums.NCRARK)
            {

            }
            return (orig(self));
        }

        private static void setOracle(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysicalObject, Room room)
        {
            orig(self, abstractPhysicalObject, room);
            if (self != null && room != null &&
                (self.room?.world?.region?.name == "KTB" || self.room?.world?.region?.name == "АЯ"))
            {
                // unbound oracles specifically.
                if ((room.abstractRoom.name.StartsWith("KTB_STG") || room.abstractRoom.name == "KTB_STGai"))
                {
                    try
                    {
                        self.ID = UnboundEnums.NCRSTG;

                        self.myScreen = new OracleProjectionScreen(room, self.oracleBehavior);
                        room.AddObject(self.myScreen);
                        self.marbles = new List<PebblesPearl>();
                        self.SetUpMarbles();

                        for (int l = 0; l < self.bodyChunks.Length; l++)
                        {
                            self.bodyChunks[l] = new BodyChunk(self, l, new Vector2(350f, 350f), 6f, 0.5f);
                        }
                    }
                    catch (Exception e)
                    {
                        UnbHelperCode.GamebreakingError(e);
                        NCRDebug.Log("Error setting ID to proper iterator: " + e);
                    }
                    
                    self.oracleBehavior = new SSOracleBehavior(self);
                }
                else if ((room.abstractRoom.name.StartsWith("KTB") && room.abstractRoom.name == "KTB_ai"))
                {
                    self.ID = UnboundEnums.NCRKTB;
                    //self.oracleBehavior = new KTBoracle(self);
                }
                else // for ark. world region should be "АЯ"
                {
                    self.ID = UnboundEnums.NCRARK;
                    //self.oracleBehavior = new ARKoracle(self);
                }
            }
        }

        private static void UpdateGlowCol(On.ZapCoil.ZapFlash.orig_Update orig, ZapCoil.ZapFlash self, bool eu)
        {
            orig(self, eu);
            if (self != null && self.room != null && self.room.world != null && !self.slatedForDeletetion &&
                self.room.abstractRoom.name.StartsWith("KTB"))
            {
                if (self.room.abstractRoom.name.StartsWith("KTB_STG"))
                {
                    self.lightsource.color = new Color(1f, 0f, 0f);
                }
                else
                {
                    self.lightsource.color = new Color(0.7f, 0.7f, 0.7f);
                }
            }
        }

        private static void FlashCol(On.ZapCoil.ZapFlash.orig_InitiateSprites orig, ZapCoil.ZapFlash self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self != null && self.room != null && self.room.world != null && !self.slatedForDeletetion &&
                self.room == rCam.room && self.room.abstractRoom.name.StartsWith("KTB"))
            {
                if (self.room.abstractRoom.name.StartsWith("KTB_STG"))
                {
                    sLeaser.sprites[0].color = new Color(1f, 0f, 0f);
                }
                else
                {
                    sLeaser.sprites[0].color = new Color(0.8f, 0.8f, 0.8f);
                }
            }
        }

        private static void DrawRedzap(On.ZapCoil.orig_DrawSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (self != null && self.room != null && self.room.world != null && !self.slatedForDeletetion &&
                self.room == rCam.room && self.room.abstractRoom.name.StartsWith("KTB"))
            {
                if (self.room.abstractRoom.name.StartsWith("KTB_STG"))
                {
                    float num = Mathf.Lerp(self.lastTurnedOn, self.turnedOn, timeStacker);
                    sLeaser.sprites[0].alpha = num;
                    Vector2 a = new Vector2(self.rect.left * 20f, self.rect.bottom * 20f);
                    Vector2 a2 = new Vector2((self.rect.right + 1) * 20f, (self.rect.top + 1) * 20f);
                    Vector2 a3 = new Vector2(self.rect.left * 20f, (self.rect.top + 1) * 20f);
                    Vector2 a4 = new Vector2((self.rect.right + 1) * 20f, self.rect.bottom * 20f);
                    float num2 = 120f * num;
                    float num3 = 30f;
                    float num4 = Mathf.Lerp(self.flicker[0, 1], self.flicker[0, 0], timeStacker);
                    float num5 = Mathf.Lerp(self.flicker[1, 1], self.flicker[1, 0], timeStacker);
                    if (self.horizontalAlignment)
                    {
                        a.x -= num3;
                        a3.x -= num3;
                        a2.x += num3;
                        a4.x += num3;
                        a.y -= num2 * num4;
                        a4.y -= num2 * num5;
                        a3.y += num2 * num4;
                        a2.y += num2 * num5;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a3 + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a4 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a2 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a4 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a2 - camPos);
                    }
                    else
                    {
                        a.x -= num2 * num4;
                        a3.x -= num2 * num5;
                        a2.x += num2 * num5;
                        a4.x += num2 * num4;
                        a.y -= num3;
                        a4.y -= num3;
                        a3.y += num3;
                        a2.y += num3;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a2 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a3 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a2 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a4 + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a4 - camPos);
                    }
                    sLeaser.sprites[0].color = new Color(1f, Mathf.InverseLerp(0f, 0.5f, self.zapLit) * num, Mathf.InverseLerp(0f, 0.5f, self.zapLit) * num);
                    return;
                }
                else
                {
                    float num = Mathf.Lerp(self.lastTurnedOn, self.turnedOn, timeStacker);
                    sLeaser.sprites[0].alpha = num;
                    Vector2 a = new Vector2(self.rect.left * 20f, self.rect.bottom * 20f);
                    Vector2 a2 = new Vector2((self.rect.right + 1) * 20f, (self.rect.top + 1) * 20f);
                    Vector2 a3 = new Vector2(self.rect.left * 20f, (self.rect.top + 1) * 20f);
                    Vector2 a4 = new Vector2((self.rect.right + 1) * 20f, self.rect.bottom * 20f);
                    float num2 = 120f * num;
                    float num3 = 30f;
                    float num4 = Mathf.Lerp(self.flicker[0, 1], self.flicker[0, 0], timeStacker);
                    float num5 = Mathf.Lerp(self.flicker[1, 1], self.flicker[1, 0], timeStacker);
                    if (self.horizontalAlignment)
                    {
                        a.x -= num3;
                        a3.x -= num3;
                        a2.x += num3;
                        a4.x += num3;
                        a.y -= num2 * num4;
                        a4.y -= num2 * num5;
                        a3.y += num2 * num4;
                        a2.y += num2 * num5;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a3 + new Vector2(num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a4 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a2 + new Vector2(-num3, 0f) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a4 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a2 - camPos);
                    }
                    else
                    {
                        a.x -= num2 * num4;
                        a3.x -= num2 * num5;
                        a2.x += num2 * num5;
                        a4.x += num2 * num4;
                        a.y -= num3;
                        a4.y -= num3;
                        a3.y += num3;
                        a2.y += num3;
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(0, a3 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(1, a2 - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(2, a3 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(3, a2 + new Vector2(0f, -num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(4, a + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(5, a4 + new Vector2(0f, num3) - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(6, a - camPos);
                        (sLeaser.sprites[0] as TriangleMesh).MoveVertice(7, a4 - camPos);
                    }
                    sLeaser.sprites[0].color = new Color(Mathf.InverseLerp(0f, 0.5f, self.zapLit) * num,
                        Mathf.InverseLerp(0.75f, 1f, self.zapLit) * num, 1f);
                    return;
                }
            }
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }

        private static void RedLight(On.ZapCoilLight.orig_ctor orig, ZapCoilLight self, Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name.StartsWith("KTB_STG"))
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(1f, 0f, 0f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                    return;
                }
                else if (name.StartsWith("KTB_"))
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(0f, 0.75f, 1f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                    return;
                }
                else if (self.room.world.name == "MS")
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(0f, 0f, 0.9f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                    return;
                }
                // if not the above, should call orig
            }
            orig(self, placedInRoom, placedObject, lightData);
        }

        private static void RedZap(On.ZapCoil.orig_InitiateSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name.StartsWith("KTB_STG"))
                {
                    TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[6];
                    for (int i = 0; i < 6; i++)
                    {
                        array[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
                    }
                    TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
                    float num = 0.4f;
                    triangleMesh.UVvertices[0] = new Vector2(0f, 0f);
                    triangleMesh.UVvertices[1] = new Vector2(1f, 0f);
                    triangleMesh.UVvertices[2] = new Vector2(0f, num);
                    triangleMesh.UVvertices[3] = new Vector2(1f, num);
                    triangleMesh.UVvertices[4] = new Vector2(0f, 1f - num);
                    triangleMesh.UVvertices[5] = new Vector2(1f, 1f - num);
                    triangleMesh.UVvertices[6] = new Vector2(0f, 1f);
                    triangleMesh.UVvertices[7] = new Vector2(1f, 1f);
                    sLeaser.sprites = new FSprite[1];
                    sLeaser.sprites[0] = triangleMesh;
                    sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
                    sLeaser.sprites[0].color = new Color(1f, 0f, 0f);
                    self.AddToContainer(sLeaser, rCam, null);
                    return;
                }
                if (name.StartsWith("KTB_"))
                {
                    TriangleMesh.Triangle[] array = new TriangleMesh.Triangle[6];
                    for (int i = 0; i < 6; i++)
                    {
                        array[i] = new TriangleMesh.Triangle(i, i + 1, i + 2);
                    }
                    TriangleMesh triangleMesh = new TriangleMesh("Futile_White", array, false, false);
                    float num = 0.4f;
                    triangleMesh.UVvertices[0] = new Vector2(0f, 0f);
                    triangleMesh.UVvertices[1] = new Vector2(1f, 0f);
                    triangleMesh.UVvertices[2] = new Vector2(0f, num);
                    triangleMesh.UVvertices[3] = new Vector2(1f, num);
                    triangleMesh.UVvertices[4] = new Vector2(0f, 1f - num);
                    triangleMesh.UVvertices[5] = new Vector2(1f, 1f - num);
                    triangleMesh.UVvertices[6] = new Vector2(0f, 1f);
                    triangleMesh.UVvertices[7] = new Vector2(1f, 1f);
                    sLeaser.sprites = new FSprite[1];
                    sLeaser.sprites[0] = triangleMesh;
                    sLeaser.sprites[0].shader = rCam.room.game.rainWorld.Shaders["FlareBomb"];
                    sLeaser.sprites[0].color = new Color(0f, 0.75f, 1f);
                    self.AddToContainer(sLeaser, rCam, null);
                    return;
                }
            }
            orig(self, sLeaser, rCam);
        }

        // end stgktb
    }
}
