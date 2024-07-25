using DevInterface;
using System;

namespace Unbound
{
    internal class PebblesDialogue
    {
        public static void Init()
        {
            On.SSOracleBehavior.PebblesConversation.AddEvents += InitialText;
            On.SSOracleBehavior.CreatureJokeDialog += JokeDialogue; // just adds the prefix to his dialogue. for now.
            On.SSOracleBehavior.InitateConversation += InitateConvo; // adds prefix and forces colourmode to always be active
            On.SSOracleBehavior.InterruptPearlMessagePlayerLeaving += PlayerLeftDuringPearlread;
            On.SSOracleBehavior.ResumePausedPearlConversation += ResumePearlread;
            On.SSOracleBehavior.NameForPlayer += NameForPlayer; // i dont think this is used, BUT just in case!

            On.SLOracleBehaviorHasMark.MoonConversation.PearlIntro += PearlIntro;
            On.OracleBehavior.AlreadyDiscussedItemString += AlreadyDiscussedItemString;
            // pearl intros
        }

        public static void UnbPearlTalk(SSOracleBehavior self)
        {
            #region UnboundPearl
            if (self.conversation != null && self.conversation.id == Conversation.ID.Pebbles_White)
            {
                for (int j = 0; j < self.player.grasps.Length; j++)
                {
                    if (self.player.grasps[j] != null && self.player.grasps[j].grabbed is DataPearl &&
                        (self.player.grasps[j].grabbed as DataPearl).AbstractPearl.dataPearlType == UnboundEnums.unboundKarmaPearl)
                    {
                        self.pearlConversation.Interrupt(self.Translate("FP: . . ."), 10);
                        self.restartConversationAfterCurrentDialoge = false;

                        self.NewAction(UnboundEnums.UnbSlumberParty);
                        self.player.ReleaseGrasp(j);
                        return;
                    }
                }
                if (self.player.objectInStomach != null)
                {
                    self.player.Regurgitate();
                }
            }
            #endregion
        }

        private static string AlreadyDiscussedItemString(On.OracleBehavior.orig_AlreadyDiscussedItemString orig, OracleBehavior self, bool pearl)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS &&
                self.player != null && self.player.room != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string result = string.Empty;
                int randomvar = UnityEngine.Random.Range(0, 3);
                if (randomvar != 0)
                {
                    if (randomvar != 1)
                    {
                        result = self.Translate("FP: I have read this. As I have stated before:");
                    }
                    else
                    {
                        result = self.Translate("FP: As if I am not busy enough...");
                    }
                }
                else
                {
                    result = self.Translate("FP: This again? Fine.");
                }
                return result;
            }
            else
            {
                return orig(self, pearl);
            }
        }

        private static void PearlIntro(On.SLOracleBehaviorHasMark.MoonConversation.orig_PearlIntro orig,
            SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self != null && self.myBehavior.oracle != null && self.myBehavior.oracle.ID == Oracle.OracleID.SS &&
                self.myBehavior.player != null && self.myBehavior.player.room != null &&
                self.myBehavior.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.myBehavior.isRepeatedDiscussion)
                {
                    self.events.Add(new Conversation.TextEvent(self, 0, self.myBehavior.AlreadyDiscussedItemString(true), 10));
                    return;
                }
                if (!self.colorMode) { self.colorMode = true; }
                switch (self.State.totalPearlsBrought + self.State.miscPearlCounter)
                {
                    case 0:
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: Fine. I will read your garbage, provided it leads you to leave me be."), 10));
                        return;
                    case 1:
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: More? Really?"), 0));
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: Fine."), 0));
                        return;
                    case 2:
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: I suppose I must read to you again."), 10));
                        return;
                    case 3:
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: I would much prefer it if you did not bring these upon every given opportunity."), 10));
                        return;
                    case 10:
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: You are quite skilled when it comes to finding these."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: I suppose I will entertain this behavior a short while longer."), 10));
                        return;
                    default:
                        switch (UnityEngine.Random.Range(0, 5))
                        {
                            case 0:
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                                    "FP: I suppose you expect me to read this."), 10));
                                return;
                            case 1:
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                                    "FP: Something new? Fine."), 10));
                                return;
                            case 2:
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                                    "FP: What is this?"), 10));
                                return;
                            case 3:
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                                    "FP: Is that something new? Allow me to see."), 10));
                                return;
                            default:
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                                    "FP: Let us see if there is anything important written on this."), 10));
                                break;
                        }
                    break;
                }
                // end unbound-pebbles convo
            }
            else
            {
                orig(self);
            }
        }

        private static void ResumePearlread(On.SSOracleBehavior.orig_ResumePausedPearlConversation orig, SSOracleBehavior self)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS && self.player != null && self.player.room != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                int num = UnityEngine.Random.Range(0, 5);
                string s;
                if (num == 0)
                {
                    s = "FP: And I suppose now you expect me to continue. Typical...";
                }
                else if (num == 1)
                {
                    s = "FP: Do not do such things.";
                }
                else if (num == 2)
                {
                    s = "FP: . . .";
                }
                else if (num == 3)
                {
                    s = "FP: Stay still and listen this time.";
                }
                else
                {
                    s = "FP: I would appreciate you not doing that again.";
                }
                self.pearlConversation.Interrupt(self.Translate(s), 10);
                self.restartConversationAfterCurrentDialoge = true;
            }
            else
            {
                orig(self);
            }
        }

        private static string NameForPlayer(On.SSOracleBehavior.orig_NameForPlayer orig, SSOracleBehavior self, bool capitalized)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS && self.player != null && self.player.room != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                string str = self.Translate("beast");
                string text = self.Translate("vile");
                if (capitalized && InGameTranslator.LanguageID.UsesCapitals(self.oracle.room.game.rainWorld.inGameTranslator.currentLanguage))
                {
                    text = char.ToUpper(text[0]).ToString() + text.Substring(1);
                }
                return text + " " + str;
            }
            else return orig(self, capitalized);
        }

        private static void PlayerLeftDuringPearlread(On.SSOracleBehavior.orig_InterruptPearlMessagePlayerLeaving orig, SSOracleBehavior self)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS && self.player != null && self.player.room != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                int randnum = UnityEngine.Random.Range(0, 8);
                string interruptdialog;
                if (randnum == 0)
                {
                    interruptdialog = "FP: Good riddance.";
                }
                else if (randnum == 1)
                {
                    interruptdialog = "FP: Was that necessary?";
                }
                else if (randnum == 2)
                {
                    interruptdialog = "FP: Clearly this was a waste of time.";
                }
                else if (randnum == 3)
                {
                    interruptdialog = "FP: . . .";
                }
                else if (randnum == 4)
                {
                    interruptdialog = "FP: Was it that boring?";
                }
                else if (randnum == 5)
                {
                    interruptdialog = "FP: I should have known a beast wouldn't understand.";
                }
                else if (randnum == 6)
                {
                    interruptdialog = "FP: Do you mind?";
                }
                else
                {
                    interruptdialog = "FP: Don't come back.";
                }
                self.pearlConversation.Interrupt(self.Translate(interruptdialog), 10);
            }
            else { orig(self); }
        }

        private static void InitateConvo(On.SSOracleBehavior.orig_InitateConversation orig, SSOracleBehavior self,
            Conversation.ID convoId, SSOracleBehavior.ConversationBehavior convBehav)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS && self.player != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.conversation != null)
                {
                    self.conversation.Interrupt("FP: ...", 0);
                    self.conversation.Destroy();
                }
                self.conversation = new SSOracleBehavior.PebblesConversation(self, convBehav, convoId, self.dialogBox);
                self.conversation.colorMode = true;
            }
            else
            {
                orig(self, convoId, convBehav);
            }
        }

        private static void JokeDialogue(On.SSOracleBehavior.orig_CreatureJokeDialog orig, SSOracleBehavior self)
        {
            if (self != null && self.oracle != null && self.oracle.ID == Oracle.OracleID.SS && self.player != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                CreatureTemplate.Type frienddraggedin = self.CheckStrayCreatureInRoom();
                if (frienddraggedin == CreatureTemplate.Type.Vulture || frienddraggedin == CreatureTemplate.Type.KingVulture ||
                    frienddraggedin == CreatureTemplate.Type.BigEel || frienddraggedin == CreatureTemplate.Type.MirosBird ||
                    (ModManager.MSC && frienddraggedin == MoreSlugcatsEnums.CreatureTemplateType.MirosVulture) ||
                    frienddraggedin == CreatureTemplate.Type.RedCentipede)
                {
                    self.dialogBox.NewMessage(self.Translate("FP: How did you fit them inside here anyhow?"), 10);
                    return;
                }
                if (frienddraggedin == CreatureTemplate.Type.Deer || frienddraggedin == CreatureTemplate.Type.TempleGuard)
                {
                    self.dialogBox.NewMessage(self.Translate("FP: I am afraid to ask how you brought your friend here."), 10);
                    return;
                }
                if (frienddraggedin == CreatureTemplate.Type.DaddyLongLegs ||
                    frienddraggedin == CreatureTemplate.Type.BrotherLongLegs ||
                    (ModManager.MSC && frienddraggedin == MoreSlugcatsEnums.CreatureTemplateType.TerrorLongLegs))
                {
                    self.dialogBox.NewMessage(self.Translate("FP: Take your friend with you. Please. I beg you."), 10);
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void InitialText(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            if (self != null && self.id != null && self.owner != null && self.owner.oracle != null && self.owner.oracle.ID == Oracle.OracleID.SS &&
                self.owner.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                try
                {
                    self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 10));

                    self.events.Add(new Conversation.TextEvent(self, 0,
                        self.Translate("FP: A little animal, on the floor of my chamber. I think I know what you are looking for."), 0));
                    self.events.Add(new Conversation.TextEvent(self, 0,
                        self.Translate("FP: You're stuck in a cycle, a repeating pattern. You want a way out."), 0));
                    self.events.Add(new Conversation.TextEvent(self, 0,
                        self.Translate(
                            "FP: Know that this does not make you special - every living thing shares that same frustration.<LINE>" +
                            "From the microbes in the processing strata to me, who am, if you excuse me, godlike in comparison."), 0));
                    self.events.Add(new Conversation.TextEvent(self, 0,
                        self.Translate("FP: The good news first. In a way, I am what you are searching for. Me and my kind have as our<LINE>" +
                        "purpose to solve that very oscillating claustrophobia in the chests of you and countless others.<LINE>" +
                        "A strange charity - you the unknowing recipient, I the reluctant gift. The noble benefactors?<LINE>Gone."), 0));
                    self.events.Add(new Conversation.TextEvent(self, 0,
                        self.Translate(
                            "FP: The bad news is that no definitive solution has been found. And every moment the equipment erodes to a new state of decay.<LINE>" +
                            "I can't help you collectively, or individually. I can't even help myself."), 0));

                    self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 210));

                    self.events.Add(new Conversation.TextEvent(self, 0, "FP: .  .  .", 0));
                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                        "FP: That is quite the vile expression from such a little beast. Perhaps you do not share in the idiocy of your kind?"), 0));

                    if (self.owner.oracle.room.game.IsStorySession &&
                        self.owner.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.memoryArraysFrolicked)
                    {
                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                            "FP: Yet you still find the time to put your grubby appendages all across my memory arrays.<LINE>" +
                            "So, I suppose, such is only the wistful musing of a superior being."), 0));
                    }

                    self.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(self, self.convBehav, 210));

                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate(
                        "FP: Find the old path. Go to the west past the Farm Arrays, and then down into the earth where the land fissures,<LINE>" +
                        "as deep as you can reach, where the ancients built their temples and danced their silly rituals."), 0));
                    self.events.Add(new Conversation.TextEvent(self, 0, "FP: Best of luck to you, distraught one. There is nothing more I can do.", 0));
                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("FP: I must resume my work."), 0));
                }
                catch (Exception e) 
                {
                    NCRDebug.Log("Unbound Intro Error: " + e);
                }
            }
            else { orig(self); }
        }
    }
}
