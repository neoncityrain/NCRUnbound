using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;
using MoreSlugcats;
using UnboundCat;
using JollyCoop;

namespace TheUnbound
{
    [BepInPlugin(MOD_ID, "NCR.theunbound", "0.0.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "NCR.theunbound";


        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            On.Player.ctor += Player_ctor;
            // initialising

            On.MoreSlugcats.StowawayBugState.AwakeThisCycle += StowawayBugState_AwakeThisCycle;
            // rerolls if a stowaway is awake or not. it should result in a bit over a 1/3 chance that it will be awake each cycle

            On.Player.Jump += Player_Jump;
            // increases unbounds base jump by 1f

            On.Player.MovementUpdate += ouuuhejumpin;
            On.Player.WallJump += Player_WallJump;
            // sets up chain walljumps and long jumps

            On.SeedCob.HitByWeapon += SeedCob_HitByWeapon;
            // SEED COB ALLERGYYYYYY

            On.Player.Update += Player_Update;
            // sets up cyan jumps

            On.Player.UpdateAnimation += Player_UpdateAnimation;
            // swim speed code

            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            // cyan spots REALLLLLLLLLLLLLLLLLLLLLLLLLLLLLL

            On.GhostWorldPresence.SpawnGhost += GhostWorldPresence_SpawnGhost;
            // fixes being unable to encounter echos under 5 karma, since unbound has a max of 3 initially

            On.SSOracleBehavior.PebblesConversation.AddEvents += PebblesConversation_AddEvents;
            On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonConversation_AddEvents;
            On.SLOracleBehaviorHasMark.MoonConversation.PearlIntro += MoonConversation_PearlIntro;
            // oracle chats

        }

        private void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self)
        {
            orig(self);
            if (self.GetCat().IsUnbound)
            {
                if (!self.submerged && !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious) && self.waterFriction >= 0.1f)
                {
                    self.waterFriction -= 0.1f;
                }
                else if (self.submerged && self.waterFriction >= 0.05f &&
                    !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious))
                {
                    self.waterFriction -= 0.05f;
                }
            }
        }

        private void MoonConversation_PearlIntro(On.SLOracleBehaviorHasMark.MoonConversation.orig_PearlIntro orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.myBehavior.player.GetCat().IsUnbound)
            {
                if (self.myBehavior.isRepeatedDiscussion)
                {
                    self.events.Add(new Conversation.TextEvent(self, 0, self.myBehavior.AlreadyDiscussedItemString(true), 10));
                    return;
                }
                if (self.myBehavior.oracle.ID != Oracle.OracleID.SS)
                {
                    switch (self.State.totalPearlsBrought + self.State.miscPearlCounter)
                    {
                        case 0:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Ah, you would like me to read this?"), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: It's a bit dusty, but I will do my best. Hold on..."), 10));
                            return;
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Another pearl! You want me to read this one too? Just a moment..."), 10));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: And yet another one! I will read it to you."), 10));
                            return;
                        case 3:
                            if (ModManager.MSC && self.myBehavior.oracle.ID == MoreSlugcatsEnums.OracleID.DM)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Another? Let us see... to be honest, I'm as curious to see it as you are."), 10));
                                return;
                            }
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Another? You're no better than the scavengers!"), 10));
                            if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Likes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Let us see... to be honest, I'm as curious to see it as you are."), 10));
                                return;
                            }
                            break;
                        default:
                            switch (UnityEngine.Random.Range(0, 5))
                            {
                                case 0:
                                    break;
                                case 1:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: The scavengers must be jealous of you, finding all these"), 10));
                                    return;
                                case 2:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Here we go again, little archeologist. Let's read your pearl."), 10));
                                    return;
                                case 3:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: ... You're getting quite good at this you know. A little archeologist beast.<LINE>Now, let's see what it says."), 10));
                                    return;
                                default:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: And yet another one! I will read it to you."), 10));
                                    return;
                            }
                            break;
                    }
                }
                else
                {
                    switch (self.State.totalPearlsBrought + self.State.miscPearlCounter)
                    {
                        case 0:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Ah, you have found me something to read?"), 10));
                            return;
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Have you found something else for me to read?"), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Let us take a look."), 10));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I am surprised you have found so many of these."), 10));
                            return;
                        case 3:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Where do you find all of these?"), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I wonder, just how much time has passed since some of these were written."), 10));
                            return;
                        default:
                            switch (UnityEngine.Random.Range(0, 5))
                            {
                                case 0:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Let us see what you have found."), 10));
                                    return;
                                case 1:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Ah. Have you found something new?"), 10));
                                    return;
                                case 2:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: What is this?"), 10));
                                    return;
                                case 3:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Is that something new? Allow me to see."), 10));
                                    return;
                                default:
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Let us see if there is anything important written on this."), 10));
                                    break;
                            }
                            break;
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private void MoonConversation_AddEvents(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            if (self.myBehavior.player.GetCat().IsUnbound)
            {
                self.colorMode = true;
                Debug.Log(new string[]
                {
                self.id.ToString(),
                self.State.neuronsLeft.ToString()
                });
                if (self.id == Conversation.ID.MoonFirstPostMarkConversation)
                {
                    switch (Mathf.Clamp(self.State.neuronsLeft, 0, 5))
                    {
                        case 0:
                            break;
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 40, "BSM: ...", 10));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Get... get away... beast.... thing."), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Please... thiss all I have left."), 10));
                            return;
                        case 3:
                            self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: You!"), 10));
                            self.events.Add(new Conversation.TextEvent(self, 60, self.Translate("BSM: ...you ate... me. Please go away. I won't speak... to you.<LINE>I... CAN'T speak to you... because... you ate...me..."), 0));
                            return;
                        case 4:
                            self.LoadEventsFromFile(35);
                            self.LoadEventsFromFile(37);
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I'm still angry at you, but it is good to have someone to talk to after all this time.<LINE>The scavengers aren't exactly good listeners. They do bring me things though, occasionally..."), 0));
                            return;
                        case 5:
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Ah. Hello. You can understand me, can't you?"), 0));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: What are you? You appear familiar, yet I had my memories I would know..."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: You must be very brave to have made it all the way here. But I'm sorry to say your journey here is in vain."), 5));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: As you can see, I have nothing for you. Not even my memories."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Or did I say that already?"), 5));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I see you don't have the gift of communication.<LINE>And yet, you watch me when I speak as if you understand..."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 15, self.Translate("BSM: Where did you come from, little creature?"), 5));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Well, it is good to have someone to talk to after all this time!<LINE>The scavengers aren't exactly good listeners. They do bring me things though, occasionally..."), 0));
                            return;
                        default:
                            return;
                    }
                }
                else if (self.id == Conversation.ID.MoonSecondPostMarkConversation)
                {
                    switch (Mathf.Clamp(self.State.neuronsLeft, 0, 5))
                    {
                        case 0:
                            break;
                        case 1:
                            self.events.Add(new Conversation.TextEvent(self, 40, "BSM: ...", 10));
                            return;
                        case 2:
                            self.events.Add(new Conversation.TextEvent(self, 80, self.Translate("BSM: ...leave..."), 10));
                            return;
                        case 3:
                            self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: You..."), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Please don't... take... more from me... Go."), 0));
                            return;
                        case 4:
                            if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Dislikes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Oh. You."), 0));
                                return;
                            }
                            if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Likes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Hello there! You again!"), 0));
                            }
                            else
                            {
                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Hello there. You again!"), 0));
                            }
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I wonder what it is that you want?"), 0));
                            if (self.State.GetOpinion != SLOrcacleState.PlayerOpinion.Dislikes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I have had scavengers come by before. Scavengers!<LINE>And they left me alive!<LINE>But... I have told you that already, haven't I?"), 0));
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: You must excuse me if I repeat myself. My memory is bad.<LINE>I used to have a pathetic five neurons... And then you ate one.<LINE>Maybe I've told you that before as well."), 0));
                                return;
                            }
                            break;
                        case 5:
                            if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Dislikes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: You again."), 10));
                                return;
                            }
                            if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Likes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Oh, hello!"), 10));
                            }
                            else
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Oh, hello."), 10));
                            }
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I wonder what it is that you want?"), 0));
                            if (self.State.GetOpinion != SLOrcacleState.PlayerOpinion.Dislikes)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: There is nothing here. Not even my memories remain."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Even the scavengers that come here from time to time leave with nothing. But... I have told you that already, haven't I?"), 0));
                                if (self.State.GetOpinion == SLOrcacleState.PlayerOpinion.Likes)
                                {
                                    if (ModManager.MSC && self.myBehavior.CheckSlugpupsInRoom())
                                    {
                                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I do enjoy the company though. You and your family are always welcome here."), 5));
                                        return;
                                    }
                                    if (ModManager.MMF && self.myBehavior.CheckStrayCreatureInRoom() != CreatureTemplate.Type.StandardGroundCreature)
                                    {
                                        self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I do enjoy the company of you and your friend though, <PlayerName>."), 5));
                                        return;
                                    }
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I do enjoy the company though. You're welcome to stay a while, soft little thing."), 5));
                                    return;
                                    }
                                }
                            break;
                        default: return;
                    }
                }
                
                else if (self.id == Conversation.ID.MoonRecieveSwarmer)
                {
                    if (self.myBehavior is SLOracleBehaviorHasMark)
                    {
                        if (self.State.neuronsLeft - 1 > 2 && (self.myBehavior as SLOracleBehaviorHasMark).respondToNeuronFromNoSpeakMode)
                        {
                            self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: You... Strange thing. Now this?"), 10));
                            self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: I will accept your gift..."), 10));
                        }
                        switch (self.State.neuronsLeft - 1)
                        {
                            case -1:
                            case 0:
                                break;
                            case 1:
                                self.events.Add(new Conversation.TextEvent(self, 40, "BSM: ...", 10));
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: You!"), 10));
                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: ...you...killed..."), 10));
                                self.events.Add(new Conversation.TextEvent(self, 0, "BSM: ...", 10));
                                self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: ...me"), 10));
                                break;
                            case 2:
                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: ...thank you... better..."), 10));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: still, very... bad."), 10));
                                break;
                            case 3:
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Thank you... That is a little better. Thank you, creature."), 10));
                                if (!(self.myBehavior as SLOracleBehaviorHasMark).respondToNeuronFromNoSpeakMode)
                                {
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Maybe this is asking too much... But, would you bring me another one?"), 0));
                                }
                                break;
                            default:
                                if ((self.myBehavior as SLOracleBehaviorHasMark).respondToNeuronFromNoSpeakMode)
                                {
                                    self.events.Add(new Conversation.TextEvent(self, 0, self.Translate("BSM: Thank you. I do wonder what you want."), 10));
                                }
                                else
                                {
                                    if (self.State.neuronGiveConversationCounter == 0)
                                    {
                                        Debug.Log(new string[]
                                        {
                                            "Moon recieved first neuron from Unbound. Has neurons:",
                                            self.State.neuronsLeft.ToString()
                                        });
                                        if (self.State.neuronsLeft == 5)
                                        {
                                            self.LoadEventsFromFile(45);
                                        }
                                        else
                                        {
                                            self.LoadEventsFromFile(19);
                                        }
                                    }
                                    else if (self.State.neuronGiveConversationCounter == 1)
                                    {
                                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: You get these at Five Pebbles'?<LINE>Thank you so much. I'm sure he won't mind."), 10));
                                        self.events.Add(new Conversation.TextEvent(self, 10, "BSM: ...", 0));
                                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Or actually I'm sure he would, but he has so many of these~<LINE>It doesn't do him any difference.<LINE>For me though, it does! Thank you, little creature!"), 0));
                                        
                                    }
                                    else
                                    {
                                        switch (UnityEngine.Random.Range(0, 4))
                                        {
                                            case 0:
                                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Thank you, again. I feel wonderful."), 10));
                                                break;
                                            case 1:
                                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Thank you so very much!"), 10));
                                                break;
                                            case 2:
                                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: It is strange... I'm remembering myself, but also... him."), 10));
                                                break;
                                            default:
                                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Thank you... Sincerely."), 10));
                                                break;
                                        }
                                    }
                                    SLOrcacleState state = self.State;
                                    int neuronGiveConversationCounter = state.neuronGiveConversationCounter;
                                    state.neuronGiveConversationCounter = neuronGiveConversationCounter + 1;
                                }
                                break;
                        }
                        (self.myBehavior as SLOracleBehaviorHasMark).respondToNeuronFromNoSpeakMode = false;
                        return;
                    }
                }
                else
                {
                    if (self.id == Conversation.ID.Moon_Pearl_Misc)
                    {
                        self.PearlIntro();
                        self.MiscPearl(false);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_Misc2)
                    {
                        self.PearlIntro();
                        self.MiscPearl(true);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pebbles_Pearl)
                    {
                        self.PebblesPearl();
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_CC)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(7);
                        return;
                    }
                    if (!ModManager.MSC && self.id == Conversation.ID.Moon_Pearl_SI_west)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(self.GetARandomChatLog(false));
                        return;
                    }
                    if (!ModManager.MSC && self.id == Conversation.ID.Moon_Pearl_SI_top)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(self.GetARandomChatLog(true));
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_LF_west)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(10);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_LF_bottom)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(11);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_HI)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(12);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SH)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(13);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_DS)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(14);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SB_filtration)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(15);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_GW)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(16);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SL_bridge)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(17);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SL_moon)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(18);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SU)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(41);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_UW)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(42);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SB_ravine)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(43);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SL_chimney)
                    {
                        self.PearlIntro();
                        self.LoadEventsFromFile(54);
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Misc_Item)
                    {
                        if (ModManager.MMF && self.myBehavior.isRepeatedDiscussion)
                        {
                            self.events.Add(new Conversation.TextEvent(self, 0, self.myBehavior.AlreadyDiscussedItemString(false), 10));
                        }
                        if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Spear)
                        {
                            self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It's a piece of sharpened rebar... What is it you want to know?<LINE>You appear to have more dangerous fangs, but it may come in handy nonetheless."), 0));
                            return;
                        }
                        else
                        {
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.FireSpear)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It's a weapon made with fire powder. Did the scavengers give this to you?<LINE>Be very careful if you have to use it!"), 0));
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Rock)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It's a rock. Thank you, I suppose, little creature."), 0));
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.KarmaFlower)
                            {
                                self.LoadEventsFromFile(25);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.WaterNut)
                            {
                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It's a plant. Most would find it delicious, but judging by your physical build,<LINE>perhaps plants aren't something you should be attempting to eat."), 0));
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.DangleFruit)
                            {
                                self.LoadEventsFromFile(26);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.FlareBomb)
                            {
                                self.LoadEventsFromFile(27);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.VultureMask)
                            {
                                self.LoadEventsFromFile(28);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.PuffBall)
                            {
                                self.LoadEventsFromFile(29);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.JellyFish)
                            {
                                self.LoadEventsFromFile(30);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Lantern)
                            {
                                self.LoadEventsFromFile(31);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.Mushroom)
                            {
                                self.LoadEventsFromFile(32);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.FirecrackerPlant)
                            {
                                self.LoadEventsFromFile(33);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.SlimeMold)
                            {
                                self.LoadEventsFromFile(34);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.ScavBomb)
                            {
                                self.LoadEventsFromFile(44);
                                return;
                            }
                            if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.OverseerRemains)
                            {
                                self.LoadEventsFromFile(52);
                                return;
                            }
                            else
                            {
                                if (self.describeItem == SLOracleBehaviorHasMark.MiscItemType.BubbleGrass)
                                {
                                    self.LoadEventsFromFile(53);
                                    return;
                                }
                                if (ModManager.MSC)
                                {
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.SingularityGrenade)
                                    {
                                        self.LoadEventsFromFile(127);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.ElectricSpear)
                                    {
                                        self.LoadEventsFromFile(112);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.InspectorEye)
                                    {
                                        self.LoadEventsFromFile(113);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.GooieDuck)
                                    {
                                        self.LoadEventsFromFile(114);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.NeedleEgg)
                                    {
                                        self.LoadEventsFromFile(116);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.LillyPuck)
                                    {
                                        self.LoadEventsFromFile(117);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.GlowWeed)
                                    {
                                        self.LoadEventsFromFile(118);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.DandelionPeach)
                                    {
                                        self.LoadEventsFromFile(122);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.MoonCloak)
                                    {
                                        self.LoadEventsFromFile(123);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.EliteMask)
                                    {
                                        self.LoadEventsFromFile(136);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.KingMask)
                                    {
                                        self.LoadEventsFromFile(137);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.FireEgg)
                                    {
                                        self.LoadEventsFromFile(164);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.SpearmasterSpear)
                                    {
                                        self.LoadEventsFromFile(166);
                                        return;
                                    }
                                    if (self.describeItem == MoreSlugcatsEnums.MiscItemType.Seed)
                                    {
                                        self.LoadEventsFromFile(167);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ModManager.MSC)
                        {
                            if (self.id == Conversation.ID.Moon_Pearl_SI_west)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(20);
                                return;
                            }
                            if (self.id == Conversation.ID.Moon_Pearl_SI_top)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(21);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_SI_chat3)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(22);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_SI_chat4)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(23);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_SI_chat5)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(24);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_SU_filt)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(101);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_DM)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(102);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_LC)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(103);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_OE)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(104);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_MS)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(105);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_RM)
                            {
                                if (self.currentSaveFile != MoreSlugcatsEnums.SlugcatStatsName.Saint)
                                {
                                    self.PearlIntro();
                                }
                                self.LoadEventsFromFile(106);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_LC_second)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(121);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_VS)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(128);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_PearlBleaching)
                            {
                                self.LoadEventsFromFile(129);
                                return;
                            }
                            if (self.id == MoreSlugcatsEnums.ConversationID.Moon_Pearl_BroadcastMisc)
                            {
                                self.PearlIntro();
                                self.LoadEventsFromFile(132, true, (!(self.myBehavior is SLOracleBehaviorHasMark) ||
                                    (self.myBehavior as SLOracleBehaviorHasMark).holdingObject == null) ?
                                    UnityEngine.Random.Range(0, 100000) :
                                    (self.myBehavior as SLOracleBehaviorHasMark).holdingObject.abstractPhysicalObject.ID.RandomSeed);
                                self.State.miscPearlCounter++;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private void PebblesConversation_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation self)
        {
            if (self.id == Conversation.ID.Pebbles_White && self.owner.player.GetCat().IsUnbound)
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


        private bool GhostWorldPresence_SpawnGhost(On.GhostWorldPresence.orig_SpawnGhost orig, GhostWorldPresence.GhostID ghostID, int karma, int karmaCap, int ghostPreviouslyEncountered, bool playingAsRed)
        {
            if(Custom.rainWorld.progression.PlayingAsSlugcat.value == "NCRunbound" &&
                !(ModManager.Expedition && Custom.rainWorld.ExpeditionMode && Custom.rainWorld.progression.currentSaveState.cycleNumber == 0)
                && !Custom.rainWorld.safariMode && karmaCap < 4 && ghostPreviouslyEncountered < 0f)
            {
                // unbound under karma cap 5, allowing echos anyway
                return karma >= karmaCap;
                // ASSUMING theyre at max karma out of their possible karma.
            }
            else return orig(ghostID, karma, karmaCap, ghostPreviouslyEncountered, playingAsRed);
        }

        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (self.player.GetCat().IsUnbound)
            {
                sLeaser.sprites[10].RemoveFromContainer();
                sLeaser.sprites[11].RemoveFromContainer();
                // removes the mark and the marks glow



                float breathaltered = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f);
                Vector2 vector = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker);
                Vector2 vector2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker);
                float num2 = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(vector2, vector).y));

                float hipsrotato = Custom.AimFromOneVectorToAnother(self.head.pos, vector2);

                // where is ymir when i need them i cannot do math stuff for the life of me
                //initiating animation variables used in body sprites;

                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleX = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 1].scaleY = 0.3f;
                sLeaser.sprites[sLeaser.sprites.Length - 1].rotation = hipsrotato;
                // coloured ring 1
                sLeaser.sprites[sLeaser.sprites.Length - 2].scaleX = 0.1f;
                sLeaser.sprites[sLeaser.sprites.Length - 2].scaleY = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 2].rotation = hipsrotato;
                // internal circle 1
                sLeaser.sprites[sLeaser.sprites.Length - 3].scaleX = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 3].scaleY = 0.3f;
                sLeaser.sprites[sLeaser.sprites.Length - 3].rotation = hipsrotato;
                // coloured ring 2
                sLeaser.sprites[sLeaser.sprites.Length - 4].scaleX = 0.1f;
                sLeaser.sprites[sLeaser.sprites.Length - 4].scaleY = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 4].rotation = hipsrotato;
                //d internal circle 2

                // upper rings
                sLeaser.sprites[sLeaser.sprites.Length - 5].scaleX = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 5].scaleY = 0.3f;
                sLeaser.sprites[sLeaser.sprites.Length - 5].rotation = hipsrotato;
                // coloured ring 1
                sLeaser.sprites[sLeaser.sprites.Length - 6].scaleX = 0.1f;
                sLeaser.sprites[sLeaser.sprites.Length - 6].scaleY = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 6].rotation = hipsrotato;
                // internal circle 1
                sLeaser.sprites[sLeaser.sprites.Length - 7].scaleX = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 7].scaleY = 0.3f;
                sLeaser.sprites[sLeaser.sprites.Length - 7].rotation = hipsrotato;
                // coloured ring 2
                sLeaser.sprites[sLeaser.sprites.Length - 8].scaleX = 0.1f;
                sLeaser.sprites[sLeaser.sprites.Length - 8].scaleY = 0.2f + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.15f, self.malnourished), 0.05f, breathaltered) * num2, 0.15f, self.player.sleepCurlUp);
                sLeaser.sprites[sLeaser.sprites.Length - 8].rotation = hipsrotato;
                //d internal circle 2


                //      (body to hips position) - (camera position) - (player sleep counter * 4f) +
                //          ((1f to 1.3f at the rate of the player aerobic value) * (value for breathing)) *
                //          (1f - (0.3 to 0.5 at the rate of hips to body position drawn, inversed) + rotation of the body + value)
                // this took way longer than youd believe
                //I can tell. dw.

                if (hipsrotato >= 260)
                {
                    hipsrotato -= 260;
                }
                else if (hipsrotato >= 180)
                {
                    hipsrotato -= 180;
                }
                else if (hipsrotato >= 90)
                {
                    hipsrotato -= 90;
                }
                
                if (hipsrotato <= -90)
                {
                    hipsrotato = -(hipsrotato + 90);
                }
                else if (hipsrotato <= -180)
                {
                    hipsrotato = -(hipsrotato + 180);
                }
                else if (hipsrotato <= -260)
                {
                    hipsrotato = -(hipsrotato + 260);
                }

                // lower rings!

                sLeaser.sprites[sLeaser.sprites.Length - 1].x = vector2.x - camPos.x - hipsrotato / 12 + 2f;
                    sLeaser.sprites[sLeaser.sprites.Length - 1].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f - num2) + hipsrotato / 12f;
                    // ring 1
                    sLeaser.sprites[sLeaser.sprites.Length - 2].x = vector2.x - camPos.x - hipsrotato / 12 + 1.5f;
                    sLeaser.sprites[sLeaser.sprites.Length - 2].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f - num2) + hipsrotato / 12f;
                // circle 1
                    sLeaser.sprites[sLeaser.sprites.Length - 3].x = vector2.x - camPos.x + hipsrotato / 12 - 2f;
                    sLeaser.sprites[sLeaser.sprites.Length - 3].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f - num2) + hipsrotato / 12f;
                    // ring 2
                    sLeaser.sprites[sLeaser.sprites.Length - 4].x = vector2.x - camPos.x + hipsrotato / 12 - 1.5f;
                    sLeaser.sprites[sLeaser.sprites.Length - 4].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                        Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f - num2) + hipsrotato / 12f;
                // circle 2

                // upper rings
                sLeaser.sprites[sLeaser.sprites.Length - 5].x = vector.x - camPos.x - hipsrotato / 12 + 2f;
                sLeaser.sprites[sLeaser.sprites.Length - 5].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                    Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f + num2) + hipsrotato / 12 + 5f;
                // ring 1
                sLeaser.sprites[sLeaser.sprites.Length - 6].x = vector.x - camPos.x - hipsrotato / 12 + 1.5f;
                sLeaser.sprites[sLeaser.sprites.Length - 6].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                    Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f + num2) + hipsrotato / 12 + 5f;
                // circle 1
                sLeaser.sprites[sLeaser.sprites.Length - 7].x = vector.x - camPos.x + hipsrotato / 12 - 2f;
                sLeaser.sprites[sLeaser.sprites.Length - 7].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                    Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f + num2) + hipsrotato / 12 + 5f;
                // ring 2
                sLeaser.sprites[sLeaser.sprites.Length - 8].x = vector.x - camPos.x + hipsrotato / 12 - 1.5f;
                sLeaser.sprites[sLeaser.sprites.Length - 8].y = vector2.y - camPos.y + ((self.player.sleepCurlUp) * 4f) +
                    Mathf.Lerp(1f, 1.3f, self.player.aerobicLevel) * breathaltered * (1f + num2) + hipsrotato / 12 + 5f;
                // circle 2



                // the below works fine for now
                if (self.player.GetCat().UnbCyanjumpCountdown == 0)
                {
                    
                    if (self.useJollyColor && ModManager.JollyCoop)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                    }
                    else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.CustomColorSafety(0);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = PlayerGraphics.CustomColorSafety(2);
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = PlayerGraphics.CustomColorSafety(0);

                    }
                    else
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.81f, 0.8f, 0.8f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                    }
                }
                else
                {
                    if (self.useJollyColor && ModManager.JollyCoop)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2), PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.Lerp(PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2), PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0), (self.player.GetCat().UnbCyanjumpCountdown / 100f)); ;
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = Color.Lerp(PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2), PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0), (self.player.GetCat().UnbCyanjumpCountdown / 100f)); ;
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = Color.Lerp(PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2), PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0), (self.player.GetCat().UnbCyanjumpCountdown / 100f)); ;
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                    }
                    else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                    {
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), PlayerGraphics.CustomColorSafety(0), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), PlayerGraphics.CustomColorSafety(0), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = PlayerGraphics.CustomColorSafety(0);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), PlayerGraphics.CustomColorSafety(0), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = PlayerGraphics.CustomColorSafety(0);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), PlayerGraphics.CustomColorSafety(0), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = PlayerGraphics.CustomColorSafety(0);
                    }
                    else
                    {
                        
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.81f, 0.8f, 0.8f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.81f, 0.8f, 0.8f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.81f, 0.8f, 0.8f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.81f, 0.8f, 0.8f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.81f, 0.8f, 0.8f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = new Color(0.81f, 0.8f, 0.8f, 1f);
                    }
                }



            }
        }

        private void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            
            if (self.player.GetCat().IsUnbound)
            {
                sLeaser.RemoveAllSpritesFromContainer();
                if (newContatiner == null)
                {
                    newContatiner = rCam.ReturnFContainer("Midground");
                }
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    if (ModManager.MSC && i == self.gownIndex)
                    {
                        newContatiner = rCam.ReturnFContainer("Items");
                        newContatiner.AddChild(sLeaser.sprites[i]);
                    }
                    else if (ModManager.MSC)
                    {
                        if (i == sLeaser.sprites.Length - 1 || i == sLeaser.sprites.Length - 2)
                        {
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 1]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 2]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 3]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 4]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 5]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 6]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 7]);
                            rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[sLeaser.sprites.Length - 8]);
                        }
                        else if ((i <= 6 || i >= 9) && i <= 9)
                        {
                            newContatiner.AddChild(sLeaser.sprites[i]);
                        }
                        else
                        {
                            rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                        }
                    }
                    else if ((i > 6 && i < 9) || i > 9)
                    {
                        rCam.ReturnFContainer("Foreground").AddChild(sLeaser.sprites[i]);
                    }
                    else
                    {
                        newContatiner.AddChild(sLeaser.sprites[i]);
                    }
                }
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        

        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            
            if (self.player.GetCat().IsUnbound)
            {
                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 8);

                // lower cyanspots
                sLeaser.sprites[sLeaser.sprites.Length - 1] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 1].shader = rCam.game.rainWorld.Shaders["Basic"];
                //coloured spot, aka the coloured ring
                sLeaser.sprites[sLeaser.sprites.Length - 2] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 2].shader = rCam.game.rainWorld.Shaders["Basic"];
                //inside spot
                sLeaser.sprites[sLeaser.sprites.Length - 3] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 3].shader = rCam.game.rainWorld.Shaders["Basic"];
                // TWO!
                sLeaser.sprites[sLeaser.sprites.Length - 4] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 4].shader = rCam.game.rainWorld.Shaders["Basic"];
                // RAAAAH

                // upper cyanspots
                sLeaser.sprites[sLeaser.sprites.Length - 5] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 5].shader = rCam.game.rainWorld.Shaders["Basic"];
                //coloured spot, aka the coloured ring
                sLeaser.sprites[sLeaser.sprites.Length - 6] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 6].shader = rCam.game.rainWorld.Shaders["Basic"];
                //inside spot
                sLeaser.sprites[sLeaser.sprites.Length - 7] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 7].shader = rCam.game.rainWorld.Shaders["Basic"];
                // TWO!
                sLeaser.sprites[sLeaser.sprites.Length - 8] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 8].shader = rCam.game.rainWorld.Shaders["Basic"];
                // twwwwo

                self.AddToContainer(sLeaser, rCam, null);
            }
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self.GetCat().IsUnbound)
            {
                if (self.GetCat().UnbCyanjumpCountdown != 0)
                {
                    self.GetCat().UnbCyanjumpCountdown--;
                }
                


                if (self.GetCat().UnbCyanjumpCountdown < 0)
                {
                    self.GetCat().UnbCyanjumpCountdown = 0;
                }
                // makes sure the countdown doesnt go under zero, even though it really Shouldnt

                if (self.GetCat().CanCyanjump && self.input[0].jmp && !self.input[1].jmp)
                {
                    // standard cyanjump!!!!
                    Debug.Log("Unbound Cyanjump Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // fun effects!


                    if (self.bodyMode == Player.BodyModeIndex.ZeroG || self.room.gravity == 0f || self.gravity == 0f)
                    {
                        // allows for quick propelling in 0g
                        float num3 = (float)self.input[0].x;
                        float num4 = (float)self.input[0].y;
                        while (num3 == 0f && num4 == 0f)
                        {
                            num3 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                            num4 = (float)(((double)UnityEngine.Random.value <= 0.33) ? 0 : (((double)UnityEngine.Random.value <= 0.5) ? 1 : -1));
                        }
                        self.bodyChunks[0].vel.x = 9f * num3;
                        self.bodyChunks[0].vel.y = 9f * num4;
                        self.bodyChunks[1].vel.x = 8f * num3;
                        self.bodyChunks[1].vel.y = 8f * num4;


                        self.GetCat().UnbCyanjumpCountdown = 60;
                    }
                    else
                    {
                        if (self.input[0].x != 0)
                        {
                            self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 8f;
                            self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 7f;
                            self.jumpBoost = 6f;
                        }
                        if (self.input[0].x == 0 || self.input[0].y == 1)
                        {
                            self.bodyChunks[0].vel.y = 11f;
                            self.bodyChunks[1].vel.y = 10f;
                            self.jumpBoost = 8f;
                        }
                        if (self.input[0].y == 1)
                        {
                            self.bodyChunks[0].vel.x = 10f * (float)self.input[0].x;
                            self.bodyChunks[1].vel.x = 8f * (float)self.input[0].x;
                        }
                        else
                        {
                            self.bodyChunks[0].vel.x = 15f * (float)self.input[0].x;
                            self.bodyChunks[1].vel.x = 13f * (float)self.input[0].x;
                        }
                        self.GetCat().UnbCyanjumpCountdown = 180;
                    }


                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyMode = Player.BodyModeIndex.Default;
                }
            }
            orig(self, eu);
        }

        private void SeedCob_HitByWeapon(On.SeedCob.orig_HitByWeapon orig, SeedCob self, Weapon weapon)
        {
            if (weapon == null || self.room == null || self.room.roomSettings == null)
            {
                return;
            }
            if (self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.roomSettings.DangerType == MoreSlugcatsEnums.RoomRainDangerType.Blizzard && weapon.firstChunk.vel.magnitude < 20f)
                {
                    if (UnityEngine.Random.Range(0.5f, 0.8f) < self.freezingCounter)
                    {
                        self.spawnUtilityFoods();
                    }
                    return;
                }
                if (weapon.thrownBy != null && weapon.thrownBy is Player && ((weapon.thrownBy as Player).slugcatStats.name == MoreSlugcatsEnums.SlugcatStatsName.Spear || (weapon.thrownBy as Player).SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint))
                {
                    return;
                }
                self.spawnUtilityFoods();
                return;
            }
            else
            {
                orig(self, weapon);
            }
        }

        private void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            if (self.GetCat().IsUnbound)
            {
                self.GetCat().UnbChainjumps += 1;
                if (self.GetCat().UnbChainjumps > 1)
                {
                    // only triggers if unbchainjumps is greater than 1, preventing a chainjump from triggering when bouncing off a wall normally
                    self.GetCat().UnbCyanjumpCountdown += 10;


                    self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    // grants a cyan-like distortion effect and sparks

                    if (self.GetCat().UnbChainjumps < 7)
                    {
                        self.jumpBoost += self.GetCat().UnbChainjumps;
                        // jump boost raises with chained jumps
                    }
                    else
                    {
                        self.jumpBoost += 7f;
                        // prevents jump boosts from going above 7. this should reset upon hitting the ground
                    }
                }
                    
            }
        }

        private void ouuuhejumpin(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);
            
            if (self.GetCat().IsUnbound)
            {



                if (self.simulateHoldJumpButton == 0 && self.GetCat().PlayingSound)
                {
                    // this checks if sound is playing so it doesnt made the worst sound known to god
                    self.GetCat().PlayingSound = false;
                    // if playingsound is true but the jump button isnt being held, set it to false. this isnt perfect obviously but shrug
                }



                if (self.simulateHoldJumpButton > 0 && !self.GetCat().PlayingSound &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if jump is being held and it ISNT playing sound
                    System.Random rd = new System.Random();
                    int rand_num = rd.Next(1, 6);
                    // gets a random value from 1 to 6. this should include 1 and 6

                    self.GetCat().UnbCyanjumpCountdown += 1;
                    // regardless, locks cyanjump for 1 frame

                    if (rand_num < 3)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Small_Jump, self.mainBodyChunk);
                        self.jumpBoost += 2;
                        // 1-2, has a small boost with a little cyanjump noise
                    }
                    else if (rand_num < 6)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                        self.jumpBoost += 3;
                        // 3-5, has a decent boost with a standard cyanjump noise
                    }
                    else
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Powerful_Jump, self.mainBodyChunk);
                        self.jumpBoost += 5;
                        // 6, has a large boost with a strong cyanjump noise
                    }


                    self.GetCat().PlayingSound = true;
                    // so that the game doesnt make the worst noise ever

                    self.room.AddObject(new UnbJumplight(self.firstChunk.pos, 0.4f));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    self.room.AddObject(new Spark(self.firstChunk.pos, Custom.RNV(), Color.red, null, 4, 8));
                    // red cyanliz effect

                    self.GetCat().UnbChainjumps += 1;
                    // adds to the chainjump count
                }
                if (self.lowerBodyFramesOnGround > 0 || self.submerged)
                {
                    self.GetCat().CanCyanjump = false;
                    // makes it so unbound DEFINITELY cant cyan jump as long as theyre on the ground
                    self.GetCat().UnbChainjumps = 0;
                    // resets values, including the chainjump value. this prevents the jump boost from retaining, though using a grapple worm may keep it- which is fine
                }










                


                if ((self.GetCat().UnbCyanjumpCountdown == 0 && self.canJump == 0 &&
                    // countdown has hit or become less than zero, cannot normally jump
                    self.Consious && !self.dead &&
                    // is awake and not dead
                    !self.submerged && self.goIntoCorridorClimb <= 0 &&
                    // is not underwater and not climbing through a pipe
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.CorridorTurn &&
                    self.bodyMode != Player.BodyModeIndex.Crawl &&
                    self.bodyMode != Player.BodyModeIndex.WallClimb &&
                    self.animation != Player.AnimationIndex.LedgeCrawl &&
                    self.animation != Player.AnimationIndex.BellySlide &&
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.SurfaceSwim &&
                    self.bodyMode != Player.BodyModeIndex.Swimming &&
                    self.animation != Player.AnimationIndex.DeepSwim &&
                    self.animation != Player.AnimationIndex.AntlerClimb)

                    ||

                    (self.GetCat().UnbCyanjumpCountdown <= 0 && self.canJump == 0 &&
                    self.Consious && !self.dead &&
                    !self.submerged && self.goIntoCorridorClimb > 0 &&
                    self.EffectiveRoomGravity == 0f && self.animation == Player.AnimationIndex.ZeroGSwim &&
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                    // checks if theyre swimming in zerog. if so, can cyanjump
                    )
                {
                    self.GetCat().CanCyanjump = true;
                }
                else
                {
                    self.GetCat().CanCyanjump = false;
                }



                
            }
        }

        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (self.GetCat().IsUnbound)
            {
                self.jumpBoost += 1f;
                // has a jump boost of +1 compared to surv
            }
        }

        private bool StowawayBugState_AwakeThisCycle(On.MoreSlugcats.StowawayBugState.orig_AwakeThisCycle orig, MoreSlugcats.StowawayBugState self, int cycle)
        {
            if (self.creature.world.game.session.characterStats.name.value == "NCRunbound")
            {
                Debug.Log("Unbound world, rerolling stowawake (because life is a fucking nightmare)");
                System.Random rd = new System.Random();
                int rand_num = rd.Next(1, 3);
                if (rand_num == 1)
                {
                    return true;
                    // if random number is 1, awaken stowaway
                }
                else
                {
                    return orig(self, cycle);
                    // if the random number isnt 1, refer to the original code
                }
            }
            else return orig(self, cycle);
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.slugcatStats.name.value == "NCRunbound")
            {
                self.GetCat().IsUnbound = true;

                if (ModManager.JollyCoop && self.IsJollyPlayer && self.GetCat().unbPlayerNumber != -1)
                {
                    self.playerState.playerNumber = self.GetCat().unbPlayerNumber;
                    // records the player number
                }

                if (self.room.game.IsStorySession && !self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = true;
                }
            }
        }


        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
        }
    }
}