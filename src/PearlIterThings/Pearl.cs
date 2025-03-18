using UnityEngine;

namespace Unbound
{
    internal class Pearl
    {
        public static void Init()
        {
            On.DataPearl.UniquePearlMainColor += karmaPearlUniqueColour;
            On.DataPearl.UniquePearlHighLightColor += karmaPearlUniqueHighlight;
            On.DataPearl.ApplyPalette += applyKarmapearlColour;
            On.Player.StomachGlowLightColor += karmapearlGlow;

            On.DataPearl.PearlIsNotMisc += NotMisc;
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
                return new Color(0.2f, 0f, 0.3f);
            }
            else return orig(pearlType);
        }

        private static Color karmaPearlUniqueColour(On.DataPearl.orig_UniquePearlMainColor orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType != null && pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return new Color(0.4f, 0.1f, 0.5f);
            }
            else return orig(pearlType);
        }

        private static void applyKarmapearlColour(On.DataPearl.orig_ApplyPalette orig, DataPearl self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if (self?.abstractPhysicalObject != null &&
                (self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
            {
                self.color = DataPearl.UniquePearlMainColor((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType);
                self.highlightColor = DataPearl.UniquePearlHighLightColor((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType);
                self.darkness = rCam.room.Darkness(self.firstChunk.pos);
            }
            else
            {
                orig(self, sLeaser, rCam, palette);
            }
        }

        private static Color? karmapearlGlow(On.Player.orig_StomachGlowLightColor orig, Player self)
        {
            if (self != null)
            {
                AbstractPhysicalObject stomachObject;
                if (self.AI == null)
                {
                    stomachObject = self.objectInStomach;
                }
                else
                {
                    stomachObject = (self.State as PlayerNPCState).StomachObject;
                }

                if (stomachObject != null)
                {
                    if (self.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.DataPearl &&
                        (self.objectInStomach as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
                    {
                        return new Color?(new Color(0.8f, 0.1f, 0.9f, 0.25f));
                    }
                }
            }
            
            return orig(self);
        }
        // end pearl
    }
}
