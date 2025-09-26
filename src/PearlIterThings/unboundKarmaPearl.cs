using UnityEngine;

namespace Unbound
{
    internal class unboundKarmaPearl
    {
        public static void Init()
        {
            On.DataPearl.UniquePearlMainColor += karmaPearlUniqueColour;
            On.DataPearl.UniquePearlHighLightColor += karmaPearlUniqueHighlight;
            On.DataPearl.ApplyPalette += applyKarmapearlColour;
            On.DataPearl.DrawSprites += DrawKarmaSymbol;

            On.DataPearl.PearlIsNotMisc += NotMisc;
        }

        private static void DrawKarmaSymbol(On.DataPearl.orig_DrawSprites orig, DataPearl self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self?.AbstractPearl?.dataPearlType != null &&
                !self.slatedForDeletetion && self.room == rCam.room &&
                self.AbstractPearl.dataPearlType == UnboundEnums.unboundKarmaPearl)
            {
                if (sLeaser.sprites[1].scale != 0.1f)
                {
                    sLeaser.sprites[1].element = Futile.atlasManager.GetElementWithName("smallKarma4");
                    sLeaser.sprites[1].scale = 0.1f;

                    sLeaser.sprites[0].scale = 1.1f;
                }
                
            }
        }

        private static bool NotMisc(On.DataPearl.orig_PearlIsNotMisc orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType != null && pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return true;
            }
            else
            {
                return orig(pearlType);
            }
        }

        private static Color? karmaPearlUniqueHighlight(On.DataPearl.orig_UniquePearlHighLightColor orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType != null && pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return new Color(0.1f, 0f, 0.2f);
            }
            else return orig(pearlType);
        }

        private static Color karmaPearlUniqueColour(On.DataPearl.orig_UniquePearlMainColor orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType != null && pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return new Color(0.45f, 0.15f, 0.55f);
            }
            else return orig(pearlType);
        }

        private static void applyKarmapearlColour(On.DataPearl.orig_ApplyPalette orig, DataPearl self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self?.abstractPhysicalObject != null &&
                (self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
            {
                if (rCam.room.game.IsStorySession &&
                    SlugcatStats.AtOrBeforeTimeline(rCam.room.game.TimelinePoint, SlugcatStats.Timeline.Spear))
                {
                    self.color = new Color(0.45f, 0.15f, 0.55f);
                    self.highlightColor = new Color(0.6f, 0.3f, 0.7f);
                }
                else
                {
                    self.color = DataPearl.UniquePearlMainColor((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType);
                    self.highlightColor = DataPearl.UniquePearlHighLightColor((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType);
                }
                self.darkness = rCam.room.Darkness(self.firstChunk.pos);
            }
            else
            {
                orig(self, sLeaser, rCam, palette);
            }
        }
        // end pearl
    }
}
