using UnityEngine;

namespace Unbound
{
    internal class Pearl
    {
        public static void Init()
        {
            On.DataPearl.UniquePearlMainColor += DataPearl_UniquePearlMainColor;
            On.DataPearl.UniquePearlHighLightColor += DataPearl_UniquePearlHighLightColor;
            On.DataPearl.ApplyPalette += DataPearl_ApplyPalette;
            On.Player.StomachGlowLightColor += Player_StomachGlowLightColor;

            On.DataPearl.PearlIsNotMisc += NotMisc;
            On.MoreSlugcats.PersistentObjectTracker.getRepData += RepData;
        }

        private static string RepData(On.MoreSlugcats.PersistentObjectTracker.orig_getRepData orig, PersistentObjectTracker self,
            AbstractPhysicalObject abstractObj)
        {
            if (abstractObj is DataPearl.AbstractDataPearl &&
                (abstractObj as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
            {
                return "unboundKarmaPearl";
            }
            else
            {
                return orig(self, abstractObj);
            }
        }

        private static bool NotMisc(On.DataPearl.orig_PearlIsNotMisc orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return true;
            }
            else
            {
                return orig(pearlType);
            }
        }

        private static Color? DataPearl_UniquePearlHighLightColor(On.DataPearl.orig_UniquePearlHighLightColor orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return new Color(0.2f, 0f, 0.3f);
            }
            else return orig(pearlType);
        }

        private static Color DataPearl_UniquePearlMainColor(On.DataPearl.orig_UniquePearlMainColor orig, DataPearl.AbstractDataPearl.DataPearlType pearlType)
        {
            if (pearlType == UnboundEnums.unboundKarmaPearl)
            {
                return new Color(0.4f, 0.1f, 0.5f);
            }
            else return orig(pearlType);
        }

        private static void DataPearl_ApplyPalette(On.DataPearl.orig_ApplyPalette orig, DataPearl self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            if ((self.abstractPhysicalObject as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
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

        private static Color? Player_StomachGlowLightColor(On.Player.orig_StomachGlowLightColor orig, Player self)
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
            return orig(self);
        }
        // end pearl
    }
}
