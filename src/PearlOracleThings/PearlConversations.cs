using System;

namespace Unbound
{
    internal class PearlConversations
    {
        public static void Init()
        {
            On.Conversation.DataPearlToConversation += Conversation_DataPearlToConversation;
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonConversation_AddEvents;
        }

        private static void MoonConversation_AddEvents(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.id != null && self != null &&
                self.id != Conversation.ID.MoonFirstPostMarkConversation &&
                self.id != Conversation.ID.MoonSecondPostMarkConversation &&
                self.id != MoreSlugcats.MoreSlugcatsEnums.ConversationID.Moon_Gourmand_First_Conversation &&
                self.id != Conversation.ID.MoonRecieveSwarmer &&
                self.id == UnboundEnums.unbKarmaPearlConv)
            {
                self.PearlIntro();
                self.LoadEventsFromFile(1431821);
                return;
            }
            else
            {
                orig(self);
            }
        }

        private static Conversation.ID Conversation_DataPearlToConversation(On.Conversation.orig_DataPearlToConversation orig, DataPearl.AbstractDataPearl.DataPearlType type)
        {
            if (type != null &&
                type == Pearl.unboundKarmaPearl)
            {
                return UnboundEnums.unbKarmaPearlConv;
            }
            else return orig(type);
        }
    }
}
