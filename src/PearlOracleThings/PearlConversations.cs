using System;

namespace Unbound
{
    internal class PearlConversations
    {
        public static void Init()
        {
            On.Conversation.DataPearlToConversation += DataPearlToConversation;
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += PearlTalk;
        }

        private static void PearlTalk(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig,
            SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.id != null && self != null && self.myBehavior != null && self.myBehavior.oracle != null && self.myBehavior.player != null &&
                self.id == UnboundEnums.unbKarmaPearlConv)
            {
                if (self.myBehavior.oracle.ID == Oracle.OracleID.SL)
                {
                    // leave this, as it should probably be tweaked for moon?
                    self.PearlIntro();
                    self.LoadEventsFromFile(1431821);
                    return;
                }
                else if (self.myBehavior.oracle.ID == Oracle.OracleID.SS && self.myBehavior.player.GetNCRunbound().IsUnbound)
                {
                    if (self.myBehavior.isRepeatedDiscussion)
                    { 
                        self.PearlIntro();
                        self.LoadEventsFromFile(1431821); // change this
                    }
                    else
                    {
                        self.dialogBox.NewMessage(self.Translate("FP: . . ."), 10);
                    }
                    return;
                }
                else
                {
                    self.PearlIntro();
                    self.LoadEventsFromFile(1431821);
                    return;
                }
            }
            else
            {
                orig(self);
            }
        }

        private static Conversation.ID DataPearlToConversation(On.Conversation.orig_DataPearlToConversation orig,
            DataPearl.AbstractDataPearl.DataPearlType type)
        {
            if (type != null &&
                type == UnboundEnums.unboundKarmaPearl)
            {
                return UnboundEnums.unbKarmaPearlConv;
            }
            else return orig(type);
        }
    }
}
