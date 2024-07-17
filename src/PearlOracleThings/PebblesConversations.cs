using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbound
{
    internal class PebblesConversations
    {
        public static void Init()
        {
            On.SSOracleBehavior.PebblesConversation.AddEvents += PebblesConversation_AddEvents;
            // On.SSOracleBehavior.ThrowOutBehavior.Update
            // ^ for later
        }

        private static void PebblesConversation_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            if (self.id == Conversation.ID.Pebbles_White && self.owner.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                self.colorMode = true;


                self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 10));

                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: A little animal, on the floor of my chamber. I think I know what you are looking for."), 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: You're stuck in a cycle, a repeating pattern. You want a way out."), 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Know that this does not make you special - every living thing shares that same frustration.<LINE>From the microbes in the processing strata to me, who am, if you excuse me, godlike in comparison."), 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: The good news first. In a way, I am what you are searching for. Me and my kind have as our<LINE>purpose to solve that very oscillating claustrophobia in the chests of you and countless others.<LINE>A strange charity - you the unknowing recipient, I the reluctant gift. The noble benefactors?<LINE>Gone."), 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: The bad news is that no definitive solution has been found. And every moment the equipment erodes to a new state of decay.<LINE>I can't help you collectively, or individually. I can't even help myself."), 0));

                self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 210));

                self.events.Add(new Conversation.TextEvent(self, 0, "FP: .  .  .", 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: That is quite the vile expression from such a little beast. Perhaps you do not share in the idiocy of your kind?"), 0));

                if (self.owner.oracle.room.game.IsStorySession &&
                    self.owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked)
                {
                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Yet you still find the time to put your grubby appendages all across my memory arrays.<LINE>So, I suppose, such is only the wistful musing of a superior being."), 0));
                }

                self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 210));

                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: Find the old path. Go to the west past the Farm Arrays, and then down into the earth where the land fissures,<LINE>as deep as you can reach, where the ancients built their temples and danced their silly rituals."), 0));
                self.events.Add(new Conversation.TextEvent(self, 0, "FP: Best of luck to you, distraught one. There is nothing more I can do.", 0));
                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: I must resume my work."), 0));
            }
            else orig(self);
        }
    }
}
