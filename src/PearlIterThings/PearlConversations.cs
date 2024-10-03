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

        private static void PearlTalk(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig,
            SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.id == UnboundEnums.unbKarmaPearlConv)
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
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Why did you take this pearl, only to give it back?"), 0));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Make up your mind, beast."), 0));
                        }
                        else
                        {
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: I told you, I don't want your-"), 0));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Wait, what is this? Where did you get this?"), 30));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: On first glance, it appears to be a simple ciphered letter from someone with the tag \'KTB.\'"), 50));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: But do not be mistaken. This pearl contains a virus targeted toward Iterators."), 40));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: I'm already dying, I do not need this on my plate as well!"), 20));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Wait... Part of it has already been burned away. Who did you give this to first?<LINE>Whoever it was, they've already read it, and the pearl has already met its mark."), 100));
                            
                            if (self != null) // this should never be null, so it should always trigger the below dialogue for now.
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: I feel uncomfortable with this thing in my chamber, but perhaps I can reverse engineer it somehow<LINE>and create a cure for myself. At least slow down the decay."), 120));
                                
                                self.events.Add(new Conversation.TextEvent(self, 0, "FP: .  .  .", 10));
                                self.events.Add(new Conversation.TextEvent(self, 0, "FP: .  .  .  .  .  .", 50));

                                self.events.Add(new Conversation.TextEvent(self, 0, "FP: Oh, you're still here.", 20));
                                self.events.Add(new Conversation.TextEvent(self, 0, "FP: Please leave now.", 50));
                            }
                            else
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Take this and be gone. You are nothing but trouble!"), 0));
                            }
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
