using System;

namespace Unbound
{
    internal class GeneralOracleThings
    {
        public static void Init()
        {
            // On.Oracle.ctor += Setup;
            // On.Oracle.OracleArm.ctor += Arm;
            // On.OracleGraphics.ApplyPalette += Palette;
        }

        private static void Palette(On.OracleGraphics.orig_ApplyPalette orig, OracleGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (IsKTB(self) || IsSTG(self))
            {
                self.SLArmBaseColA = new Color(0.52156866f, 0.52156866f, 0.5137255f);
                self.SLArmHighLightColA = new Color(0.5686275f, 0.5686275f, 0.54901963f);
                self.SLArmBaseColB = palette.texture.GetPixel(5, 1);
                self.SLArmHighLightColB = palette.texture.GetPixel(5, 2);
                for (int i = 0; i < self.armJointGraphics.Length; i++)
                {
                    self.armJointGraphics[i].ApplyPalette(sLeaser, rCam, palette);
                }
                Color color;
                if (IsKTB(self))
                {
                    color = new Color(1f, 0.4f, 0.79607844f);
                }
                else
                {
                    color = new Color(0.89f, 0.89f, 0.79f);
                }
                for (int j = 0; j < self.owner.bodyChunks.Length; j++)
                {
                    sLeaser.sprites[self.firstBodyChunkSprite + j].color = color;
                }
                sLeaser.sprites[self.neckSprite].color = color;
                sLeaser.sprites[self.HeadSprite].color = color;
                sLeaser.sprites[self.ChinSprite].color = color;
                for (int k = 0; k < 2; k++)
                {
                    if (self.armJointGraphics.Length == 0)
                    {
                        sLeaser.sprites[self.PhoneSprite(k, 0)].color = self.GenericJointBaseColor();
                        sLeaser.sprites[self.PhoneSprite(k, 1)].color = self.GenericJointHighLightColor();
                        sLeaser.sprites[self.PhoneSprite(k, 2)].color = self.GenericJointHighLightColor();
                    }
                    else
                    {
                        sLeaser.sprites[self.PhoneSprite(k, 0)].color = self.armJointGraphics[0].BaseColor(default(Vector2));
                        sLeaser.sprites[self.PhoneSprite(k, 1)].color = self.armJointGraphics[0].HighLightColor(default(Vector2));
                        sLeaser.sprites[self.PhoneSprite(k, 2)].color = self.armJointGraphics[0].HighLightColor(default(Vector2));
                    }
                    sLeaser.sprites[self.HandSprite(k, 0)].color = color;
                    if (self.gown != null)
                    {
                        for (int l = 0; l < 7; l++)
                        {
                            (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4] = self.gown.Color(0.4f);
                            (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 1] = self.gown.Color(0f);
                            (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 2] = self.gown.Color(0.4f);
                            (sLeaser.sprites[self.HandSprite(k, 1)] as TriangleMesh).verticeColors[l * 4 + 3] = self.gown.Color(0f);
                        }
                    }
                    else
                    {
                        sLeaser.sprites[self.HandSprite(k, 1)].color = color;
                    }
                    sLeaser.sprites[self.FootSprite(k, 0)].color = color;
                    sLeaser.sprites[self.FootSprite(k, 1)].color = color;
                }
                if (self.umbCord != null)
                {
                    self.umbCord.ApplyPalette(sLeaser, rCam, palette);
                    sLeaser.sprites[self.firstUmbilicalSprite].color = palette.blackColor;
                }
                else if (self.discUmbCord != null)
                {
                    self.discUmbCord.ApplyPalette(sLeaser, rCam, palette);
                }
                if (self.armBase != null)
                {
                    self.armBase.ApplyPalette(sLeaser, rCam, palette);
                }
                if (IsKTB(self))
                {
                    sLeaser.sprites[self.MoonThirdEyeSprite].color = Color.Lerp(new Color(1f, 0f, 1f), color, 0.5f);
                }
                else
                {
                    sLeaser.sprites[self.MoonThirdEyeSprite].color = Color.Lerp(new Color(1f, 0f, 1f), color, 0.3f);
                    sLeaser.sprites[self.MoonSigilSprite].color = new Color(0.12156863f, 0.28627452f, 0.48235294f);
                }
            }
            else
            {
                orig(self, sLeaser, rCam, palette);
            }
        }

        private static void Arm(On.Oracle.OracleArm.orig_ctor orig, Oracle.OracleArm self, Oracle oracle)
        {
            if (oracle.ID == UnboundEnums.NCRKTB)
            {
                self.baseMoveSoundLoop = new StaticSoundLoop(SoundID.SS_AI_Base_Move_LOOP, oracle.firstChunk.pos, oracle.room, 1f, 0.99f);
            }
            else if (oracle.ID == UnboundEnums.NCRSTG)
            {
                self.baseMoveSoundLoop = new StaticSoundLoop(SoundID.SS_AI_Base_Move_LOOP, oracle.firstChunk.pos, oracle.room, 0.99f, 1.02f);
            }
            orig(self, oracle);
        }

        private static void Setup(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysicalObject, Room room)
        {
            if (room.abstractRoom.name == "SL_STGAI" || room.abstractRoom.name == "SL_KTBAI")
            {
                self.ID = room.abstractRoom.name == "SL_KTBAI" ? UnboundEnums.NCRKTB : UnboundEnums.NCRSTG;
                for (int l = 0; l < self.bodyChunks.Length; l++)
                {
                    self.bodyChunks[l] = new BodyChunk(self, l, new Vector2(350f, 350f), 6f, 0.5f);
                }
            }
            orig(self, abstractPhysicalObject, room);
            if (self.ID == UnboundEnums.NCRSTG || self.ID == UnboundEnums.NCRKTB)
            {
                self.oracleBehavior = new SSOracleBehavior(self);
                self.myScreen = new OracleProjectionScreen(room, self.oracleBehavior);
                room.AddObject(self.myScreen);
                self.marbles = new List<PebblesPearl>();
                self.SetUpMarbles();
                room.gravity = 0f;
                for (int n = 0; n < room.updateList.Count; n++)
                {
                    if (room.updateList[n] is AntiGravity)
                    {
                        (room.updateList[n] as AntiGravity).active = false;
                        break;
                    }
                }
                self.arm = new Oracle.OracleArm(self);
            }
        }
        // end oracle things

        public static bool IsKTB(OracleGraphics self)
        {
            return self.oracle.ID == UnboundEnums.NCRKTB && self.oracle.room.world.name == "SL_KTBAI";
        }
        public static bool IsSTG(OracleGraphics self)
        {
            return self.oracle.ID == UnboundEnums.NCRSTG && self.oracle.room.world.name == "SL_STGAI";
        }
    }
}
