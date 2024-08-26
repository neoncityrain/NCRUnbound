using Expedition;
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

        public static void PebblesUnbKarmaPearl(OracleBehavior self, DataPearl pearl, SaveState saveState)
        {
            if (!saveState.theGlow) // aka, first time reading the pearl
            {
                NCRDebug.Log("Pebbles reading Unbound Karma Pearl for the first time!");
                saveState.theGlow = true;

                self.player.PyroDeath();
                return;
            }
            else
            {
                NCRDebug.Log("Pebbles reading Unbound Karma Pearl, not for the first time!");
                return;
            }
        }

        private static void PearlTalk(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig,
            SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.id != null && self != null && self.myBehavior != null && 
                self.myBehavior.oracle != null && self.myBehavior.oracle.ID != null &&
                self.myBehavior.oracle.room != null && self.myBehavior.oracle.room.world != null &&
                self.myBehavior.player != null && self.id == UnboundEnums.unbKarmaPearlConv)
            {
                try
                {
                    if (self.myBehavior.oracle.ID == Oracle.OracleID.SL)
                    {
                        // this should probably be tweaked in the future
                        self.PearlIntro();
                        self.LoadEventsFromFile(1431821);
                        return;
                    }
                    else if (self.myBehavior.oracle.ID == Oracle.OracleID.SS &&
                        self.myBehavior.oracle.room.world.game.session.characterStats.name.value == "NCRunbound")
                    {
                        if (self.myBehavior.isRepeatedDiscussion)
                        {
                            self.PearlIntro();
                            self.LoadEventsFromFile(1431821); // pearl reread dialogue
                        }
                        else
                        {
                            self.PearlIntro();
                            self.LoadEventsFromFile(1431821); // CHANGE THIS
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
                catch (Exception e)
                {
                    NCRDebug.Log("FUCK! " + e);
                }
            }
            else
            {
                try { orig(self); }
                catch (Exception e)
                {
                    NCRDebug.Log("Vanilla code for pearl reading is fucking up, because of course it would, why wouldn't it: " + e);
                }
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
