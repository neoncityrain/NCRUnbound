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
using MonoMod.RuntimeDetour;
using System.Reflection;
using OverseerHolograms;
using UnboundMS;
using UnboundJumpsmoke;
using Menu;
using CoralBrain;

namespace TheUnbound
{
    [BepInPlugin(MOD_ID, "NCR.theunbound", "0.0.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "NCR.theunbound";
        public delegate Color orig_OverseerMainColor(global::OverseerGraphics self);
        public UnbJumpsmoke smoke;




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

            On.RegionGate.customKarmaGateRequirements += RegionGate_customKarmaGateRequirements;
            // allows for exiting MS

            // On.JollyCoop.JollyMenu.JollyPlayerSelector.SetPortraitImage_Name_Color += JollyPlayerSelector_SetPortraitImage_Name_Color;
            // wow thats a mouthful lol. dynamic jolly pfp images. currently disabled due to coding issues
        }

      //  private void JollyPlayerSelector_SetPortraitImage_Name_Color(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_SetPortraitImage_Name_Color orig, JollyCoop.JollyMenu.JollyPlayerSelector self, SlugcatStats.Name className, Color colorTint)
       // {
            //orig(self, className, colorTint);

           // MenuIllustration portrait1 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer1", new Vector2(100f, 100f) / 2f, true, true);
            //MenuIllustration portrait2 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer2", new Vector2(100f, 100f) / 2f, true, true);
            //MenuIllustration portrait3 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer3", new Vector2(100f, 100f) / 2f, true, true);
           // MenuIllustration portrait4 = new MenuIllustration(self.dialog, self, "", "recolarena-" + className.ToString() + "layer4", new Vector2(100f, 100f) / 2f, true, true);

           // portrait2.sprite.color = self.faceTintColor;
           // portrait3.sprite.color = self.uniqueTintColor;
           // portrait4.sprite.color = self.bodyTintColor;

           // self.subObjects.Add(portrait1);
           // self.subObjects.Add(portrait2);
           // self.subObjects.Add(portrait3);
           // self.subObjects.Add(portrait4);

           // portrait2.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
          //  portrait3.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
           // portrait4.sprite.alpha = (className.value == "NCRunbound") ? 1f : 0f;
       // }

        private void RegionGate_customKarmaGateRequirements(On.RegionGate.orig_customKarmaGateRequirements orig, RegionGate self)
        {
            if (self.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.room.abstractRoom.name == "GATE_SL_MS")
                {
                    int num2;
                    if (int.TryParse(self.karmaRequirements[1].value, out num2))
                    {
                        self.karmaRequirements[1] = RegionGate.GateRequirement.OneKarma;
                    }
                }
            }
            orig(self);

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
            if (self.owner.room != null && self.overseer != null &&
                self.owner.room.game.session.characterStats.name.value == "NCRunbound" && self.overseer.PlayerGuide)
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0.2f, 0.56f, 0.47f), 0.5f);
            }
            else if (self.owner.room != null && self.overseer != null)
            {
                sLeaser.sprites[self.WhiteSprite].color = Color.Lerp(self.ColorOfSegment(0.75f, timeStacker), new Color(0f, 0f, 1f), 0.5f);
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
                    (self.grasps[0].grabbed as JetFish).Consious) && self.waterFriction >= 0.7f)
                {
                    self.waterFriction -= 0.1f;
                }
                else if (self.submerged && self.waterFriction >= 0.7f &&
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
                if (smoke != null && (smoke.slatedForDeletetion || smoke.room != self.room))
                {
                    smoke = null;
                }

                if (self.GetCat().UnbCyanjumpCountdown != 0)
                {
                    self.GetCat().UnbCyanjumpCountdown--;
                }
                


                if (self.GetCat().UnbCyanjumpCountdown < 0)
                {
                    self.GetCat().UnbCyanjumpCountdown = 0;
                }
                // makes sure the countdown doesnt go under zero, even though it really Shouldnt

                if (self.GetCat().CanCyanjump1 && self.input[0].jmp && !self.input[1].jmp)
                {
                    // standard cyanjump!!!!
                    Debug.Log("Unbound Cyanjump1 Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
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

                    if (smoke == null)
                    {
                        smoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(smoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        smoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 45f);
                    }

                    self.animation = Player.AnimationIndex.RocketJump;
                    self.bodyMode = Player.BodyModeIndex.Default;


                }
                if (self.GetCat().CanCyanjump2 && self.input[0].jmp && !self.input[1].jmp && !self.GetCat().CanCyanjump1)
                {
                    // if they cant cyanjump1, BUT CAN cyanjump2, trigger

                    Debug.Log("Unbound Cyanjump2 Triggered");
                    if (!self.GetCat().PlayingSound)
                    {
                        self.room.PlaySound(SoundID.Cyan_Lizard_Powerful_Jump, self.mainBodyChunk);
                    }
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));

                   // removes the 0g log, as it should not trigger in 0g
                    if (self.input[0].x != 0)
                    {
                        self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 11f;
                        self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 10f;
                        self.jumpBoost = 12f;
                    }
                    if (self.input[0].x == 0 || self.input[0].y == 1)
                    {  
                        self.bodyChunks[0].vel.y = 14f;
                        self.bodyChunks[1].vel.y = 13f;
                        self.jumpBoost = 15f;
                    }
                    if (self.input[0].y == 1)
                    {
                        self.bodyChunks[0].vel.x = 13f * (float)self.input[0].x;
                        self.bodyChunks[1].vel.x = 11f * (float)self.input[0].x;
                    }
                    else
                    {
                        self.bodyChunks[0].vel.x = 18f * (float)self.input[0].x;
                        self.bodyChunks[1].vel.x = 15f * (float)self.input[0].x;
                    }

                    self.GetCat().UnbCyanjumpCountdown += 400;
                    // adds a LOT more to the countdown- it recharges very slowly

                    if (smoke == null)
                    {
                        smoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(smoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        smoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 50f);
                    }

                    self.animation = Player.AnimationIndex.Flip;
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
                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
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


                    if (smoke == null)
                    {
                        smoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(smoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        smoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 20f,
                            self.bodyMode == Player.BodyModeIndex.ZeroG ? false : true, 35f);
                    }
                    // smoke effects for walljumps
                }
                    
            }
        }

        private void ouuuhejumpin(On.Player.orig_MovementUpdate orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.GetCat().IsUnbound)
            {
                if (self.lowerBodyFramesOnGround > 1 || self.submerged)
                {
                    self.GetCat().didLongjump = false;
                    self.GetCat().UnbChainjumps = 0;
                    // this prevents the jump boosts from retaining, though using a grapple worm may keep it- which is fine
                    // it is kept when touching water or going onto poles
                    // this should be above longjump to keep the chainjump from not truly being considered "raising" the
                    // chainjump counter
                }

                if (self.simulateHoldJumpButton == 0 && self.GetCat().PlayingSound)
                {
                    // this checks if sound is playing so it doesnt made the worst sound known to god
                    self.GetCat().PlayingSound = false;
                    // if playingsound is true but the jump button isnt being held, set it to false. this isnt perfect obviously but shrug
                    // this assumes there is room for error in the player movements, as generally to make a terrible noise it would need to
                    // be clicked very, very rapidly
                }



                if (self.simulateHoldJumpButton > 0 && !self.GetCat().PlayingSound &&
                    self.goIntoCorridorClimb <= 0 && self.room.gravity != 0f)
                {
                    // if jump is being held and it ISNT playing sound, AKA a longjump

                    self.GetCat().UnbCyanjumpCountdown += 5;
                    // regardless, locks cyanjump for 5 frames
                    self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, self.mainBodyChunk);
                    self.jumpBoost += 3;
                    // has a decent extra boost with a standard cyanjump noise

                    self.GetCat().PlayingSound = true;
                    // so that the game doesnt make the worst noise ever

                    self.room.AddObject(new UnbJumplight(self.bodyChunks[1].pos, 0.4f, self));
                    self.room.AddObject(new ShockWave(self.firstChunk.pos, 50f, 0.07f, 3, false));
                    if (smoke == null)
                    {
                        smoke = new UnbJumpsmoke(self.room, self);
                        self.room.AddObject(smoke);
                        Debug.Log("Emitting smoke!");
                    }
                    for (int k = 0; k < 7; k++)
                    {
                        smoke.EmitSmoke(self.bodyChunks[1].pos, self.bodyChunks[1].vel +
                            Custom.DirVec(self.bodyChunks[0].pos, self.bodyChunks[1].pos) * 30f, true, 45f);
                    }
                    // emits smoke. longjumps are not possible in true 0g, so its considered a "big" jump

                    self.GetCat().didLongjump = true;
                    // sets longjump to true
                    self.GetCat().UnbChainjumps += 1;
                    // adds to the chainjump count
                    Debug.Log("Longjump detected!");
                }



                if ((self.canJump == 0 && self.lowerBodyFramesOnGround <= 0 &&
                    // cannot normally jump and had its lower body on the ground for less than/equal to 0 frames
                    self.enteringShortCut == null && !self.inShortcut && self.shortcutDelay == 0 &&
                    // not entering, inside a shortcut, or in a shortcut delay state
                    self.Consious && !self.dead && self.canWallJump == 0 && self.jumpStun == 0 &&
                    // is awake, not dead, cannot wall jump, not jumpstunned
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
                    self.bodyMode != Player.BodyModeIndex.CorridorClimb &&
                    
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.HangUnderVerticalBeam &&
                    self.animation != Player.AnimationIndex.BeamTip &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpOnBeam &&
                    self.animation != Player.AnimationIndex.GetUpToBeamTip &&
                    self.animation != Player.AnimationIndex.StandOnBeam
                    // beam stuff
                    )

                    ||

                    (self.GetCat().UnbCyanjumpCountdown <= 0 && self.canJump == 0 &&
                    self.Consious && !self.dead &&
                    !self.submerged && self.goIntoCorridorClimb > 0 &&
                    self.EffectiveRoomGravity == 0f && self.animation == Player.AnimationIndex.ZeroGSwim &&
                    self.animation != Player.AnimationIndex.VineGrab &&
                    self.animation != Player.AnimationIndex.ZeroGPoleGrab &&
                    self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam &&
                    self.animation != Player.AnimationIndex.HangFromBeam &&
                    self.animation != Player.AnimationIndex.ClimbOnBeam)
                    // checks if theyre swimming in zerog
                    )
                {
                    if (self.GetCat().UnbCyanjumpCountdown <= 0)
                    {
                        self.GetCat().CanCyanjump1 = true;
                        // if the cyan countdown is 0 or less than zero, can perform the first cyanjump
                    }
                    else
                    {
                        self.GetCat().CanCyanjump1 = false;
                    }


                    if (self.GetCat().UnbCyanjumpCountdown <= 175 && self.GetCat().UnbCyanjumpCountdown > 20 &&
                        self.GetCat().didLongjump == true &&
                        
                        self.animation != Player.AnimationIndex.GrapplingSwing &&
                        (self.grasps[0] == null || !(self.grasps[0].grabbed is TubeWorm))
                        // prevents triggering if using a grapple worm
                        )
                    {
                        self.GetCat().CanCyanjump2 = true;
                        // if ALL OF THE ABOVE are true and just performed a longjump without touching the ground
                        // also makes sure the coundown is below or at 175 so that cyjump2 isnt triggered IMMEDIATELY after cyjump1
                        // and isnt triggered immediately after itself. should never trigger in 0g
                    }
                    else
                    {
                        self.GetCat().CanCyanjump2 = false;
                    }
                }
                else
                {
                    self.GetCat().CanCyanjump1 = false;
                    self.GetCat().CanCyanjump2 = false;
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
                    Debug.Log("Stowaway state defaulting to normal");
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
            if (self.room.game.session.characterStats.name.value == "NCRunbound" && (self.room.game.IsStorySession ||
                 self.room.game.session is StoryGameSession))
            {
                if (self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived)
                {
                    Debug.Log("Old save detected, fixing game");
                    self.room.game.GetStorySession.saveState.miscWorldSaveData.moonRevived = false;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripPebbles = false;
                    // re-kills moon. sorry women.
                }
                if (!self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon)
                {
                    if (world.region.name == "MS")
                    {
                        Debug.Log("MS start detected, triggering intro");
                        self.room.AddObject(new UnboundIntro());
                    }
                    else if (world.region.name == "SL")
                    {
                        Debug.Log("SL start detected");
                    }
                    (self.room.world.game.session as StoryGameSession).saveState.miscWorldSaveData.playerGuideState.likesPlayer += 1f;
                    self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
                    Debug.Log("Unbound start detected! This SHOULD trigger regardless of the cat being actively played, and only trigger once!");
                }
                
            }
        }


        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
        }
    }
}