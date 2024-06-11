using System;
using BepInEx;
using UnityEngine;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;
using RWCustom;
using MoreSlugcats;
using UnboundCat;
using Expedition;
using JollyCoop;
using System.Diagnostics.Eventing.Reader;
using MonoMod.RuntimeDetour;
using System.Reflection;
using OverseerHolograms;

namespace TheUnbound
{
    [BepInPlugin(MOD_ID, "NCR.theunbound", "0.0.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "NCR.theunbound";
        public delegate Color orig_OverseerMainColor(global::OverseerGraphics self);

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

            On.Player.Grabability += Player_Grabability;
            // prevents taking neurons from moon

            On.Player.CanBeSwallowed += Player_CanBeSwallowed;
            // cannot swallow or spit up items


            Hook ktbmain = new Hook(typeof(global::OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance |
                BindingFlags.Public).GetGetMethod(), new Func<orig_OverseerMainColor,
                OverseerGraphics, Color>(this.OverseerGraphics_MainColor_get));
            // 0.29f, 0.39f, 0.47f is the main colour, 0.2f, 0.56f, 0.47f is the tendril colour, 0.13f, 0.15f, 0.18f is the eye colour
            // adjust as needed to look not like shit
            On.OverseerGraphics.DrawSprites += OverseerGraphics_DrawSprites;
            On.OverseerGraphics.DrawSprites -= Overseer_DrawspritesRemove;
            On.OverseerGraphics.InitiateSprites += OverseerGraphics_InitiateSprites;
            On.OverseerGraphics.InitiateSprites -= OverseerGraphics_RemoveSprites;
            On.CoralBrain.Mycelium.UpdateColor += Mycelium_UpdateColor;
            On.OverseerGraphics.ColorOfSegment += OverseerGraphics_ColorOfSegment;
            On.Overseer.TryAddHologram += Overseer_TryAddHologram;
            On.OverseerAbstractAI.RoomAllowed += OverseerAbstractAI_RoomAllowed;
            //ktboverseer things

            On.RoomSpecificScript.SU_A43SuperJumpOnly.Update += SU_A43SuperJumpOnly_Update;
            On.RoomSpecificScript.SU_C04StartUp.Update += SU_C04StartUp_Update;
            On.RoomSpecificScript.SU_A23FirstCycleMessage.Update += SU_A23FirstCycleMessage_Update;
            On.RoomSpecificScript.SL_C12JetFish.Update += SL_C12JetFish_Update;
            On.RoomSpecificTextMessage.Update += RoomSpecificTextMessage_Update;
            // no tutorials, destroy them immediately
        }

        private void RoomSpecificTextMessage_Update(On.RoomSpecificTextMessage.orig_Update orig, RoomSpecificTextMessage self, bool eu)
        {
            if (self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private void SL_C12JetFish_Update(On.RoomSpecificScript.SL_C12JetFish.orig_Update orig, RoomSpecificScript.SL_C12JetFish self, bool eu)
        {
            if (self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private void SU_A23FirstCycleMessage_Update(On.RoomSpecificScript.SU_A23FirstCycleMessage.orig_Update orig, RoomSpecificScript.SU_A23FirstCycleMessage self, bool eu)
        {
            if (self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private void SU_C04StartUp_Update(On.RoomSpecificScript.SU_C04StartUp.orig_Update orig, RoomSpecificScript.SU_C04StartUp self, bool eu)
        {
            if (self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private void SU_A43SuperJumpOnly_Update(On.RoomSpecificScript.SU_A43SuperJumpOnly.orig_Update orig, RoomSpecificScript.SU_A43SuperJumpOnly self, bool eu)
        {
            if (self.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                self.Destroy();
            }
            else
            {
                orig(self, eu);
            }
        }

        private bool OverseerAbstractAI_RoomAllowed(On.OverseerAbstractAI.orig_RoomAllowed orig, OverseerAbstractAI self, int room)
        {
            if (self.world.game.session.characterStats.name.value == "NCRunbound" && self.playerGuide)
            {
                if (room < self.world.firstRoomIndex || room >= self.world.firstRoomIndex + self.world.NumberOfRooms)
                {
                    return false;
                }
                for (int i = 0; i < OverseerAbstractAI.tutorialRooms.Length; i++)
                {
                    if (self.world.GetAbstractRoom(room).name == OverseerAbstractAI.tutorialRooms[i])
                    {
                        return true;
                    }
                }
                return (self.world.region.name == "MS" ||
                    // can always show up in MS
                    !(self.world.GetAbstractRoom(room).AttractionForCreature(self.parent.creatureTemplate.type) ==
                    AbstractRoom.CreatureRoomAttraction.Forbidden) &&
                    // if the room is not forbidden
                    !self.world.GetAbstractRoom(room).scavengerOutpost && !self.world.GetAbstractRoom(room).scavengerTrader) ||
                    // if the room is not a scav outpost / scav trader
                    self.world.GetAbstractRoom(room).shelter;
                    // or if the room IS a shelter (enabling the guide to come inside the shelter with the player)
            }
            return orig(self, room);
        }

        private void Overseer_TryAddHologram(On.Overseer.orig_TryAddHologram orig, Overseer self, OverseerHolograms.OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            if (self.room.game.session.characterStats.name.value == "NCRunbound" && self.PlayerGuide)
            {
                if (self.dead)
                {
                    return;
                }
                // dont show holograms if dead
                if (self.room != null)
                {
                    if (self.room.abstractRoom.name == "SS_AI")
                    {
                        return;
                    }
                    // dont show holograms in pebbles' chamber. this is initially only for MSC- should not trigger for UB either
                }
                if (self.hologram != null)
                {
                    if (self.hologram.message == message)
                    {
                        return;
                    }
                    if (self.hologram.importance >= importance && importance != 3.4028235E+38f)
                    {
                        return;
                    }
                    self.hologram.stillRelevant = false;
                    self.hologram = null;
                    // removes unnecessary holograms
                }
                if (self.room == null)
                {
                    return;
                    // dont show holograms if not in a room
                }
                // ordinarily the tutorial holograms are here. this mod goes with the assumption that the player knows how to play,
                // so those are removed
                if (message == OverseerHologram.Message.Bats)
                {
                    self.hologram = new OverseerHologram.BatPointer(self, message, communicateWith, importance);
                }
                else if (message == OverseerHologram.Message.Shelter)
                {
                    self.hologram = new OverseerHologram.ShelterPointer(self, message, communicateWith, importance);
                }
                else if (message == OverseerHologram.Message.DangerousCreature)
                {
                    self.hologram = new OverseerHologram.CreaturePointer(self, message, communicateWith, importance);
                }
                else if (message == OverseerHologram.Message.FoodObject)
                {
                    self.hologram = new OverseerHologram.FoodPointer(self, message, communicateWith, importance);
                }
                else
                {
                    return;
                    // return if it doesnt fit the above (i.e., if its a tutorial or progression direction)
                }
                self.room.AddObject(self.hologram);
            }
            else
            {
                orig(self, message, communicateWith, importance);
            }
        }

        private void Mycelium_UpdateColor(On.CoralBrain.Mycelium.orig_UpdateColor orig, CoralBrain.Mycelium self, Color newColor, float gradientStart, int spr, RoomCamera.SpriteLeaser sLeaser)
        {
            if (self.owner.OwnerRoom.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.owner is OverseerGraphics && (self.owner as OverseerGraphics).overseer.PlayerGuide)
                {
                    self.color = newColor;
                    for (int i = 0; i < (sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length; i++)
                    {
                        float value = (float)i / (float)((sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length - 1);
                        (sLeaser.sprites[spr] as TriangleMesh).verticeColors[i] = Color.Lerp(self.color,
                            Custom.HSL2RGB(0.4888889f, 0.5f, 0.2f), Mathf.InverseLerp(gradientStart, 1f, value));
                    }
                    for (int j = 1; j < 3; j++)
                    {
                        (sLeaser.sprites[spr] as TriangleMesh).verticeColors[(sLeaser.sprites[spr] as TriangleMesh).verticeColors.Length - j] =
                            new Color(0.2f, 0.76f, 0.57f);
                    }
                }
                else
                {
                    orig(self, newColor, gradientStart, spr, sLeaser);
                }
            }
            else
            {
                orig(self, newColor, gradientStart, spr, sLeaser);
            }
        }

        private void OverseerGraphics_RemoveSprites(On.OverseerGraphics.orig_InitiateSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites[self.PupilSprite].color = new Color(0f, 0f, 0f, 0.5f);
        }

        private void OverseerGraphics_InitiateSprites(On.OverseerGraphics.orig_InitiateSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.owner != null && 
                self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                sLeaser.sprites[self.PupilSprite].color = new Color(0.2f, 0.56f, 0.478f, 0.5f);
                
            }
            else
            {
                sLeaser.sprites[self.PupilSprite].color = new Color(0f, 0f, 0f, 0.5f);
            }
        }

        private Color OverseerGraphics_ColorOfSegment(On.OverseerGraphics.orig_ColorOfSegment orig, OverseerGraphics self, float f, float timeStacker)
        {
            if (self.owner != null &&
                self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                return Color.Lerp(Color.Lerp(Custom.RGB2RGBA((self.MainColor + new Color(0.3f, 0.86f, 0.67f) +
                    self.earthColor * 8f) / 10f, 0.5f), Color.Lerp(self.MainColor, Color.Lerp(self.NeutralColor,
                    self.earthColor, Mathf.Pow(f, 2f)), 0.5f),
                    self.ExtensionOfSegment(f, timeStacker)), Custom.RGB2RGBA(self.MainColor, 0f),
                    Mathf.Lerp(self.overseer.lastDying, self.overseer.dying, timeStacker));
            }
            else
            {
                return orig(self, f, timeStacker);
            }
        }

        private void OverseerGraphics_DrawSprites(On.OverseerGraphics.orig_DrawSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.owner != null && 
                self.owner.room.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0.2f, 0.56f, 0.47f), 0.5f);
            }
            else
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0f, 0f, 1f), 0.5f);
                Color lhs = self.ColorOfSegment(self.myceliaStuckAt, timeStacker);
            }
        }

        private void Overseer_DrawspritesRemove(On.OverseerGraphics.orig_DrawSprites orig, OverseerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0f, 0f, 1f), 0.5f);

        }

        public Color OverseerGraphics_MainColor_get(Plugin.orig_OverseerMainColor orig, global::OverseerGraphics self)
        {
            if (self.owner != null && self.overseer.room.world.game.session.characterStats.name.value == "NCRunbound" &&
                self.overseer.PlayerGuide)
            {
                return new Color(0.29f, 0.59f, 0.87f);
            }
            else
            {
                return orig(self);
            }
        }


            private Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self.GetCat().IsUnbound)
            {
                if (obj is SLOracleSwarmer)
                {
                    return Player.ObjectGrabability.CantGrab;
                }
                if (obj is Leech || obj is Spider)
                {
                    return Player.ObjectGrabability.OneHand;
                }
            }
            return orig(self, obj);
        }

        private bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.GetCat().IsUnbound)
            {
                return false;
            }
            else return orig(self, testObj);
        }










        private void MoonConversation_PearlIntro(On.SLOracleBehaviorHasMark.MoonConversation.orig_PearlIntro orig, SLOracleBehaviorHasMark.MoonConversation self)
        {
            // the dialogue for moon when given a pearl, triggered prior to actually reading it
            if (self.myBehavior.player.room.game.session.characterStats.name.value == "NCRunbound")
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
            if (self.myBehavior.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                self.colorMode = true;
                Debug.Log(new string[]
                {
                self.id.ToString(),
                self.State.neuronsLeft.ToString()
                });
                if (self.id == Conversation.ID.MoonFirstPostMarkConversation)
                {
                    // moon will always have 5 neurons at this point
                    self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Ah. Hello. You can understand me, can't you?"), 0));
                            self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: What are you? You appear familiar, yet if I had my memories I would know..."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: You must be very brave to have made it all the way here. But I'm sorry to say your journey here is in vain."), 5));
                            self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: As you can see, I have nothing for you. Not even my memories."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Or did I say that already?"), 5));
                            self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: I see you don't have the gift of communication.<LINE>And yet, you watch me when I speak, as if you understand..."), 0));
                            self.events.Add(new Conversation.TextEvent(self, 15, self.Translate("BSM: Where did you come from, little creature?"), 5));
                            self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Well, it is good to have someone to talk to after all this time!<LINE>The scavengers aren't exactly good listeners. They do bring me things though, occasionally..."), 0));
                    
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
                                            "Moon recieved first neuron from Unbound game. Has neurons:",
                                            self.State.neuronsLeft.ToString()
                                        });
                                        if (self.State.neuronsLeft == 5)
                                        {
                                            self.LoadEventsFromFile(45);
                                        }
                                        else
                                        {
                                            self.LoadEventsFromFile(19);
                                            // standardized, fine to keep
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

                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: This information is illegal. Someone probably tried to send it by<LINE>a pearl somehow rather than risking being overheard on broadcast."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: The problem with breaking taboos is that the barriers are encoded<LINE>into every cell of our organic parts. And there are other taboos<LINE>strictly regulating our ability to rewrite our own genome."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: So what you need is to somehow create a small sample of living organic matter<LINE>which can procreate and act on the rest of your organic matter to re-write its genome.<LINE>The re-write has to be very specific, overriding the specific taboo you want to<LINE>circumvent but do nothing else."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, "BSM: ...", 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, "BSM: There is something about your eyes that gives me the impression that you understand<LINE>more than I've given you credit for.", 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, "BSM: I can't stop you if you were to do so, but I must request you don't give this pearl<LINE>to just anyone.", 10));

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

                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: This one is just plain text. I will read it to you."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: \"On regards of the (by spiritual splendor eternally graced) people of the Congregation of Never Dwindling Righteousness,<LINE>we Wish to congratulate (o so thankfully) this Facility on its Loyal and Relished services, and to Offer our Hopes and<LINE>Aspirations that the Fruitful and Mutually Satisfactory Cooperation may continue, for as long as the Stars stay fixed on their<LINE>Celestial Spheres and/or the Cooperation continues to be Fruitful and Mutually Satisfactory.\""), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: \"It is with Honor I, Eight Suns-Countless Leaves, of the House of Six Wagons, Count of no living blocks, Counselor of 2, Duke of 1,<LINE>Humble Secretary of the Congregation of Never Dwindling Righteousness, write this to You.\""), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Oh, are you tired already, <PlayerName>?"), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Well, you would be esctatic to know this pearl goes on for quite a while, and quite in the same way~"), 10));

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_LF_bottom)
                    {
                        self.PearlIntro();

                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: It's a Small Plate, a little text of spiritual guidance.<LINE>It's written by a monk called Four Cabinets, Eleven Hatchets.<LINE>It's old, several ages before the Void Fluid revolution."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Like most writing from this time it’s quite shrouded in analogies, but the subject is how to shed<LINE>one of the five natural urges which tie a creature to life. Namely number four, gluttony."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: It is basically an instruction on how to starve yourself<LINE>on herbal tea and gravel, but disguised as a poem."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: I wonder if you know it already?"), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: No offense intended! You don't appear very<LINE>entertained by this pearl."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: I suppose, in the life of a little beast, gluttony is a difficult thing to achieve,<LINE>and only shows in the most successful of your kind..."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Yet our creators learned to fear such a thing. It really makes you think."), 10));

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_HI)
                    {
                        self.PearlIntro();

                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: It's a production record of a mask factory, for what seems to be its last time in service.<LINE>Have you seen a bone mask, <PlayerName>?"), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: In ancient times the masks were actually about showing spiritual persuasion - covering the<LINE>face as a way to symbolically abate the self. Then of course, that was quite subverted as<LINE>excessively ornate and lavish masks became an expression of identity. Some public persons<LINE>did have problems with narrow doorways."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Now, you make a face, and I must assure you, it was as ridiculous as it sounds.<LINE>One couldn't publically say so, of course, but that doesn't negate the humor of the situation."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Originally monks in a temple would make the masks using bone plaster, and when the production was automated it would<LINE>generally remain on the same site. So that the old stones could... radiate the material with holiness, I suppose."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: This is from one such facility called Side House, which was here on Pebble's grounds. In the iterator projects many<LINE>old industrial-religious sites like this were remodeled and incorporated. I think this one was made to provide pellets<LINE>of holy ash to Pebbles, but knowing him he probably hasn't used much of it!"), 10));

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SH)
                    {
                        self.PearlIntro();
                        
                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: Oh this one is interesting. You must have found it in the memory crypts?<LINE>It has some plain text, I can read it out to you."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: \"In this vessel is the living memories of Seventeen Axes, Fifteen Spoked Wheel, of the House of Braids,<LINE>Count of 8 living blocks, Counselor of 16, Grand Master of the Twelfth Pillar of Community,<LINE>High Commander of opinion group Winged Opinions, of pure Braid heritage, voted Local Champion in the<LINE>speaking tournament of 1511.090, Mother, Father and Spouse, Spiritual Explorer and honorary member of<LINE>the Congregation of Balanced Ambiguity. Artist, Warrior, and Fashion Legend."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Seventeen Axes, Fifteen Spoked Wheel nobly decided to ascend in the beginning of  1514.008, after graciously donating all (ALL!) earthly<LINE>possessions to the local Iterator project (Unparalleled Innocence), and left these memories to be cherished by the carnal plane."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: The assorted memories and qualia include:"), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Watching dust suspended in a ray of sun (Old age).<LINE>Eating a very tasty meal (Young child)...\""), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Come now, you can't be falling asleep already? I wasn't planning to actually read it all."), 10));
                        // above slightly off

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_DS)
                    {
                        self.PearlIntro();

                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It's an old text. The verses are familiar to me, but I don't remember by whom they were written.<LINE>The language is very old and intricate."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: The first verse starts by drawing a comparison between the world and a tangled rug.<LINE>It says that the world is an unfortunate mess. Like a knot, the nature of its existence<LINE>is the fact that the parts are locking each other, none able to spring free."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Then as it goes on the world becomes a furry animal hide, I suppose... because now us living<LINE>beings are like insects crawling in the fur. And then it's a fishing net, because the more we<LINE>struggle and squirm, the more entangled we become."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: It says that only the limp body of the jellyfish cannot be captured in the net.<LINE>So we should try to be like the jellyfish, because the jellyfish doesn't try."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: This was an eternal dilemma to them - they were burdened by great ambition,<LINE>yet deeply convinced that striving in itself was an unforgivable vice.<LINE>They tried very hard to be effortless. Perhaps that's what we were to them,<LINE>someone to delegate that unrestrained effort to."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: I know I have tried very hard."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Forgive me if I am wrong, but you appear as if you have as well."), 10));

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SB_filtration)
                    {
                        self.PearlIntro();

                        self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: It's the blueprint for a Void Fluid filtration system.<LINE>Do you know what Void Fluid is?"), 0));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: You are one of the most expressive of your kind that I have met.<LINE>I can tell the answer without further prompting."), 5));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Not many creatures are as knowledgeable as you. They know of the cycle we are a part of, but<LINE>rarely have the capacity to communicate with ancient technology in order to escape."), 5));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Still, to know of Void Fluid yet be here..."), 20));
                        self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: Ah, never mind. Rambling to myself, now."), 0));
                        
                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_GW)
                    {
                        self.PearlIntro();

                        self.LoadEventsFromFile(16);
                        // this one DOES have the actual file, since its primarily just adding colour and consistancy.
                        // self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: "), 0)); < for copy-pasting

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SL_bridge)
                    {
                        self.PearlIntro();

                        self.LoadEventsFromFile(17);
                        // standardized, not edited

                        return;
                    }
                    if (self.id == Conversation.ID.Moon_Pearl_SL_moon)
                    {
                        self.PearlIntro();

                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Interesting... This one is written by me."), 10));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: It's about an iterator called Sliver of Straw. I don't remember when I wrote it...<LINE>Do you know Sliver of Straw? She's quite legendary among us."), 30));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Hm..."), 0));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: Not to anthropomorphize, but your body language suggests you understand completely."), 20));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: You are quite knowledgable for a little creature, if that is the case. How interesting!<LINE>Were any of my systems online, I would be capable of finding out more..."), 40));
                        self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("BSM: I suppose I will skip the explainations regarding who Sliver of Straw is...<LINE>This is an essay, in which I make the case that maybe she should be allowed to rest in peace now."), 10));

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

                                self.events.Add(new Conversation.TextEvent(self, 10, self.Translate("BSM: This one is an old conversation between Five Pebbles and a friend of his. I'll read it to you."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("\"1591.290 - PRIVATE<LINE>Five Pebbles, Seven Red Suns"), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: Can I tell you something? Lately..."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: I'm tired of trying and trying. And angry that they left us here.<LINE>The anger makes me even less inclined to solve their puzzle for them. Why do we do this?"), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("SRS: Yes, I'll spell this out - not because you're stupid or naive...<LINE>Also, not saying that you're not ~"), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: Please, I'm coming to you for guidance."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("SRS: Sorry, very sorry. I kid. Fact is, of course we are all aware of the evident<LINE>futility of this Big Task. It's not said out loud but if you were better at reading<LINE>between the lines there's nowhere you wouldn't see it. We're all frustrated."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: So why do we continue? We assemble work groups, we ponder,<LINE>we iterate and try. Some of us die. It's not fair."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("SRS: Because there's not any options. What else CAN we do? You're stuck in your can, and at any<LINE>moment you have no more than two alternatives: Do nothing, or work like you're supposed to."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("SRS: An analogy. You have a maze, and you have a handful of bugs. You put the bugs in the maze, and you leave.<LINE>Given infinite time, one of the bugs WILL find a way out, if they just erratically try and try.<LINE>This is why they called us Iterators."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: But we do die of old age."), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("SRS: Even more incentive! You know that nothing ever truly dies though, around and around it goes.<LINE>Granted, our tools and resources get worse over time - but that is theoretically unproblematic,<LINE>because in time even a miniscule chance will strike a positive.<LINE>All the same to them, they're not around anymore!"), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, self.Translate("FP: I struggle to accept being a bug.\""), 0));
                                self.events.Add(new Conversation.TextEvent(self, 20, "BSM: . . .", 0));
                                self.events.Add(new Conversation.TextEvent(self, 30, self.Translate("BSM: <CapPlayerName>, is there something wrong?"), 0));

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

        private void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self)
        {
            orig(self);
            if (self.GetCat().IsUnbound)
            {
                if (!self.submerged && !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious) && self.waterFriction >= 0.5f)
                {
                    self.waterFriction -= 0.1f;
                }
                else if (self.submerged && self.waterFriction >= 0.1f &&
                    !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious))
                {
                    self.waterFriction -= 0.05f;
                }
            }
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
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.89f, 0.79f, 0.6f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = new Color(0.59f, 0.14f, 0.14f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = new Color(0.89f, 0.79f, 0.6f, 1f);
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
                        
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.89f, 0.79f, 0.6f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = Color.Lerp(new Color(0.59f, 0.14f, 0.14f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = new Color(0.89f, 0.79f, 0.6f, 1f);
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
                    self.animation != Player.AnimationIndex.SurfaceSwim &&
                    self.bodyMode != Player.BodyModeIndex.Swimming &&
                    self.animation != Player.AnimationIndex.DeepSwim &&
                    self.animation != Player.AnimationIndex.AntlerClimb &&

                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.HangUnderVerticalBeam &&
                    self.animation != Player.AnimationIndex.BeamTip &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpToBeamTip &&
                    self.animation != Player.AnimationIndex.StandOnBeam)

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
                    Debug.Log("Congrats! Stowaway awoken (because life is a fucking nightmare)");
                    return true;
                    // if random number is 1, awaken stowaway
                }
                else
                {
                    Debug.Log("Stowaway remains asleep....... for now");
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
            }
            if (self.room.game.session.characterStats.name.value == "NCRunbound" && self.room.game.IsStorySession)
            {
                if (!self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    (self.room.world.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.likesPlayer += 1f;
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = true;
                    Debug.Log("Reviving Moon for Unbound's savestate and influencing PlayerGuide settings. This SHOULD trigger regardless of the cat being actively played and only once!");
                }
                
            }
        }


        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
        }
    }
}