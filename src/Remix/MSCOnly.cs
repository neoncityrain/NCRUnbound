using System;
using Menu;
using MoreSlugcats;

namespace Unbound
{
    internal class MSCOnly
    {
        public static CollectionsMenu.PearlReadContext UnbPebbles = new ("UnbPebbles", true);

        public static void Init()
        {
            On.MoreSlugcats.CollectionsMenu.Singal += AddUnbound;
            On.MoreSlugcats.CollectionsMenu.DataPearlToFileID += PearlToFile;
        }

        private static int PearlToFile(On.MoreSlugcats.CollectionsMenu.orig_DataPearlToFileID orig, DataPearl.AbstractDataPearl.DataPearlType type)
        {
            Conversation.ID a = Conversation.DataPearlToConversation(type);
            if (a == UnboundEnums.unbKarmaPearlConv)
            {
                return 1431821;
            }
            else return orig(type);
        }

        private static void AddUnbound(On.MoreSlugcats.CollectionsMenu.orig_Singal orig, CollectionsMenu self, 
            Menu.MenuObject sender, string message)
        {
            orig(self, sender, message);
            if (message.Contains("PEARL") || message.Contains("TYPE"))
            {
                DataPearl.AbstractDataPearl.DataPearlType dataPearlType = self.usedPearlTypes[self.selectedPearlInd];
                int DataPearlToFileID = CollectionsMenu.DataPearlToFileID(dataPearlType);
                
                CollectionsMenu.PearlReadContext a = null;
                for (int m = 0; m < self.iteratorButtons.Length; m++)
                {
                    if (message.Contains("UNBOUNDPEBBLES"))
                    {
                        a = UnbPebbles;
                    }
                }
                SlugcatStats.Name saveFile = null;
                if (a == UnbPebbles)
                {
                    saveFile = UnboundEnums.NCRUnbound;
                }
                self.InitLabelsFromPearlFile(DataPearlToFileID, saveFile);
            }
        }

        public static bool SetPearlDecipheredUnbound(DataPearl.AbstractDataPearl.DataPearlType pearlType,
            PlayerProgression.MiscProgressionData self)
        {
            if (pearlType != null && self != null && ModManager.MSC)
            {
                int num = CollectionsMenu.DataPearlToFileID(pearlType);
                if (num != -1 && !Conversation.EventsFileExists(self.owner.rainWorld, num, UnboundEnums.NCRUnbound))
                {
                    return self.SetPearlDeciphered(pearlType);
                }
            }
            if (pearlType == null || (pearlType != null && UnboundEnums.decipheredPearlsUnboundSession.Contains(pearlType)))
            {
                return false;
            }
            UnboundEnums.decipheredPearlsUnboundSession.Add(pearlType);
            self.owner.SaveProgression(false, true);
            return true;
        }
        // end collections things
    }
}
