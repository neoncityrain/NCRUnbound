using System;

namespace Unbound
{
    internal class STGKTB
    {
        public static void Init()
        {
            On.ZapCoil.InitiateSprites += RedZap;
            On.ZapCoilLight.ctor += RedLight;
            On.ZapCoil.DrawSprites += DrawRedzap;
            On.Region.RegionColor += Colour;
        }

        private static Color Colour(On.Region.orig_RegionColor orig, string regionName)
        {
            if (Region.EquivalentRegion(regionName, "STG"))
            {
                return new Color(0.87f, 0.39f, 0.33f, 1f);
            }
            if (Region.EquivalentRegion(regionName, "KTB"))
            {
                return new Color(0.29f, 0.39f, 0.47f, 1f);
            }
            return orig(regionName);
        }

        private static void DrawRedzap(On.ZapCoil.orig_DrawSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (self != null && self.room != null && self.room.world != null && !self.slatedForDeletetion && self.room == rCam.room &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER" || self.room.world.name == "STG")
                {
                    float num = Mathf.Lerp(self.lastTurnedOn, self.turnedOn, timeStacker);
                    sLeaser.sprites[0].alpha = num;
                    Vector2 a = new Vector2((float)self.rect.left * 20f, (float)self.rect.bottom * 20f);
                    Vector2 a2 = new Vector2((float)(self.rect.right + 1) * 20f, (float)(self.rect.top + 1) * 20f);
                    Vector2 a3 = new Vector2((float)self.rect.left * 20f, (float)(self.rect.top + 1) * 20f);
                    Vector2 a4 = new Vector2((float)(self.rect.right + 1) * 20f, (float)self.rect.bottom * 20f);
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
                }
                else
                {
                    orig(self, sLeaser, rCam, timeStacker, camPos);
                }
            }
            else
            {
                orig(self, sLeaser, rCam, timeStacker, camPos);
            }
        }

        private static void RedLight(On.ZapCoilLight.orig_ctor orig, ZapCoilLight self, Room placedInRoom, PlacedObject placedObject, PlacedObject.LightFixtureData lightData)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER" || self.room.world.name == "STG")
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(1f, 0f, 0f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, (float)lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                }
                else if (self.room.world.name == "MS")
                {
                    self.lightSource = new LightSource(placedObject.pos, false, new Color(0f, 0f, 1f), self);
                    placedInRoom.AddObject(self.lightSource);
                    self.lightSource.setRad = new float?(Mathf.Lerp(100f, 2000f, (float)lightData.randomSeed / 100f));
                    self.lightSource.setAlpha = new float?(1f);
                    self.lightSource.affectedByPaletteDarkness = 0.5f;
                }
                else
                {
                    orig(self, placedInRoom, placedObject, lightData);
                }
            }
            else
            {
                orig(self, placedInRoom, placedObject, lightData);
            }
        }

        private static void RedZap(On.ZapCoil.orig_InitiateSprites orig, ZapCoil self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            if (self != null && self.room != null && self.room.world != null &&
                self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string name = self.room.abstractRoom.name;

                if (name == "MS_STGSTARCHAMBER" || self.room.world.name == "STG")
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
                }
                else
                {
                    orig(self, sLeaser, rCam);
                }
            }
            else
            {
                orig(self, sLeaser, rCam);
            }
        }

        // end stgktb
    }
}
