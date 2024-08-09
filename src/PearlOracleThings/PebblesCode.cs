using System;
using JollyCoop;
using System.Linq;

namespace Unbound
{
    internal class PebblesCode
    {
        public static void Init()
        {
            // On.SSOracleBehavior.ThrowOutBehavior.Update += ThrowOutToSleepover;
            // On.SSOracleBehavior.NewAction += NewAction;

            On.SSOracleBehavior.Update += UpdateBehavior;
            On.SSOracleBehavior.SeePlayer += SeeUnbound;
            On.SSOracleBehavior.storedPearlOrbitLocation += storedPearlOrbitLocation;
            On.SSOracleBehavior.ctor += SSctor;
            On.SSOracleBehavior.InitateConversation += EnableColourmode;

            On.SSOracleBehavior.StartItemConversation += PearlConversations;
        }

        private static void PearlConversations(On.SSOracleBehavior.orig_StartItemConversation orig, SSOracleBehavior self, DataPearl item)
        {
            if (self != null && self.player != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                SLOrcacleState sloracleState = self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SLOracleState;
                self.isRepeatedDiscussion = false;

                #region Discussions that aren't important nor relevant
                if (item.AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc || 
                    item.AbstractPearl.dataPearlType.Index == -1)
                {
                    self.pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(Conversation.ID.Moon_Pearl_Misc, self, 
                        SLOracleBehaviorHasMark.MiscItemType.NA);
                }
                else if (item.AbstractPearl.dataPearlType == DataPearl.AbstractDataPearl.DataPearlType.Misc2)
                {
                    self.pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(Conversation.ID.Moon_Pearl_Misc2, 
                        self, SLOracleBehaviorHasMark.MiscItemType.NA);
                }
                else if (ModManager.MSC && item.AbstractPearl.dataPearlType == MoreSlugcatsEnums.DataPearlType.BroadcastMisc)
                {
                    self.pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(
                        MoreSlugcatsEnums.ConversationID.Moon_Pearl_BroadcastMisc, self, SLOracleBehaviorHasMark.MiscItemType.NA);
                }
                else
                {
                    if (self.pearlConversation != null)
                    {
                        self.pearlConversation.Interrupt("FP: ...", 0);
                        self.pearlConversation.Destroy();
                        self.pearlConversation = null;
                    }
                    #endregion

                    Conversation.ID id = Conversation.DataPearlToConversation(item.AbstractPearl.dataPearlType);
                    if (!sloracleState.significantPearls.Contains(item.AbstractPearl.dataPearlType))
                    {
                        sloracleState.significantPearls.Add(item.AbstractPearl.dataPearlType);
                    }

                    try 
                    {
                        if (UnboundEnums.decipheredPearlsUnboundSession != null)
                        {
                            self.isRepeatedDiscussion = (item.AbstractPearl.dataPearlType != null &&
                            UnboundEnums.decipheredPearlsUnboundSession.Contains(item.AbstractPearl.dataPearlType));
                        }
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Issue deciphering pearl:" + e);
                    }
                    try
                    {
                        MSCOnly.SetPearlDecipheredUnbound(item.AbstractPearl.dataPearlType,
                            self.oracle.room.game.rainWorld.progression.miscProgressionData);
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Issue setting pearl as deciphered:" + e);
                    }


                    self.pearlConversation = new SLOracleBehaviorHasMark.MoonConversation(id, self, SLOracleBehaviorHasMark.MiscItemType.NA);
                    sloracleState.totalPearlsBrought++;
                }
                if (!self.isRepeatedDiscussion)
                {
                    sloracleState.totalItemsBrought++;
                    sloracleState.AddItemToAlreadyTalkedAbout(item.abstractPhysicalObject.ID);
                }
                self.talkedAboutThisSession.Add(item.abstractPhysicalObject.ID);
            }
            else
            {
                orig(self, item);
            }
        }

        private static void EnableColourmode(On.SSOracleBehavior.orig_InitateConversation orig, SSOracleBehavior self, Conversation.ID convoId, SSOracleBehavior.ConversationBehavior convBehav)
        {
            orig(self, convoId, convBehav);
            if (self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                self.conversation.colorMode = true;
            }
        }

        private static Vector2 storedPearlOrbitLocation(On.SSOracleBehavior.orig_storedPearlOrbitLocation orig, SSOracleBehavior self, int index)
        {
            if (self.player.room.game.session.characterStats.name.value == "NCRunbound" ||
                self.oracle.room.game.StoryCharacter == UnboundEnums.NCRUnbound)
            {
                float num = 5f;
                float num2 = (float)index % num;
                float num3 = Mathf.Floor((float)index / num);
                float num4 = num2 * 0.5f;
                return new Vector2(615f, 100f) + new Vector2(num2 * 26f, (num3 + num4) * 18f);
            }
            else
            {
                return orig(self, index);
            }
        }

        private static void SSctor(On.SSOracleBehavior.orig_ctor orig, SSOracleBehavior self, Oracle oracle)
        {
            orig(self, oracle);
            if (self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                self.InitStoryPearlCollection();
            }
        }

        private static void NewAction(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action nextAction)
        {
            if (self != null && self.player != null && self.player.room != null && !self.player.room.game.rainWorld.safariMode &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                SSOracleBehavior.SubBehavior.SubBehavID subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.General;

                if (nextAction == UnboundEnums.UnbSlumberParty &&
                    ((self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad >= 1 &&
                    nextAction != SSOracleBehavior.Action.MeetWhite_Shocked) || self.oracle.room.game.rainWorld.ExpeditionMode))
                {
                    subBehavID = UnboundEnums.UnbSlumberPartySub;
                }
                else if ((nextAction == SSOracleBehavior.Action.MeetWhite_Curious ||
                    nextAction == SSOracleBehavior.Action.MeetWhite_Images ||
                    nextAction == SSOracleBehavior.Action.MeetWhite_SecondCurious ||
                    nextAction == SSOracleBehavior.Action.MeetWhite_Shocked ||
                    nextAction == SSOracleBehavior.Action.MeetWhite_Talking ||
                    nextAction == SSOracleBehavior.Action.MeetWhite_Texting ||
                    (nextAction == UnboundEnums.UnbSlumberParty &&
                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad < 1)) &&
                    !self.oracle.room.game.rainWorld.ExpeditionMode)
                {
                    try { subBehavID = UnboundEnums.SSMeetUnboundSub; }
                    catch (Exception e) { NCRDebug.Log("Error meeting Unbound: " + e); }
                }
                else if (nextAction == SSOracleBehavior.Action.ThrowOut_KillOnSight ||
                    nextAction == SSOracleBehavior.Action.ThrowOut_SecondThrowOut ||
                    nextAction == SSOracleBehavior.Action.ThrowOut_ThrowOut ||
                    nextAction == SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut)
                {
                    subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut;
                }
                else if (ModManager.MSC && (nextAction == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_SecondImages ||
                    nextAction == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_ThirdCurious ||
                    nextAction == MoreSlugcatsEnums.SSOracleBehaviorAction.MeetWhite_StartDialog))
                {
                    try { subBehavID = UnboundEnums.SSMeetUnboundSub; }
                    catch (Exception e) { NCRDebug.Log("Error meeting Unbound: " + e); }
                }
                else if (ModManager.MSC && nextAction == MoreSlugcatsEnums.SSOracleBehaviorAction.ThrowOut_Singularity)
                {
                    subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut;
                }
                else
                {
                    subBehavID = SSOracleBehavior.SubBehavior.SubBehavID.General;
                }

                self.currSubBehavior.NewAction(self.action, nextAction);

                if (subBehavID != SSOracleBehavior.SubBehavior.SubBehavID.General && subBehavID != self.currSubBehavior.ID)
                {
                    if (self.player.GetNCRunbound().MoreDebug) { NCRDebug.Log("Next subBehavior: " + subBehavID); }
                    SSOracleBehavior.SubBehavior subBehavior = null;
                    for (int i = 0; i < self.allSubBehaviors.Count; i++)
                    {
                        if (self.allSubBehaviors[i].ID == subBehavID)
                        {
                            subBehavior = self.allSubBehaviors[i];
                            break;
                        }
                    }
                    if (subBehavior == null)
                    {
                        self.LockShortcuts();
                        if (subBehavID == UnboundEnums.UnbSlumberPartySub &&
                            self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad >= 1 &&
                            nextAction != SSOracleBehavior.Action.MeetWhite_Shocked)
                        {
                            subBehavior = new UnboundPebblesSleepover(self);
                            self.UnlockShortcuts();
                        }
                        else if (subBehavID == SSOracleBehavior.SubBehavior.SubBehavID.MeetWhite ||
                            subBehavID == UnboundEnums.SSMeetUnboundSub ||
                            (nextAction == UnboundEnums.UnbSlumberParty &&
                            self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad < 1))
                        {
                            subBehavior = new SSOracleBehavior.SSOracleMeetWhite(self);
                        }
                        else if (subBehavID == SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut)
                        {
                            subBehavior = new SSOracleBehavior.ThrowOutBehavior(self);
                        }
                        self.allSubBehaviors.Add(subBehavior);
                    }
                    subBehavior.Activate(self.action, nextAction);
                    self.currSubBehavior.Deactivate();
                    self.currSubBehavior = subBehavior;
                }
                self.inActionCounter = 0;
                self.action = nextAction;
            }
            else
            {
                orig(self, nextAction);
            }
        }

        private static void SeeUnbound(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self)
        {
            if (self != null && self.player != null && self.player.room != null && !self.player.room.game.rainWorld.safariMode &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.timeSinceSeenPlayer < 0)
                {
                    self.timeSinceSeenPlayer = 0;
                }

                #region Jolly Setup
                if (ModManager.CoopAvailable && self.timeSinceSeenPlayer < 5)
                {
                    Player player = null;
                    int num = 0;
                    foreach (Player altplayer in from x in self.oracle.room.game.NonPermaDeadPlayers
                                               where x.Room != self.oracle.room.abstractRoom
                                               select x.realizedCreature as Player into x
                                               orderby x.slugOnBack != null
                                               select x)
                    {
                        if (altplayer.slugOnBack != null)
                        {
                            altplayer.slugOnBack.DropSlug();
                        }
                        try
                        {
                            int node;
                            if (altplayer.room.abstractRoom.name == "SS_D07")
                            {
                                node = 1;
                            }
                            else
                            {
                                node = 0;
                            }
                            WorldCoordinate worldCoordinate = self.oracle.room.LocalCoordinateOfNode(node);
                            JollyCustom.MovePlayerWithItems(altplayer, altplayer.room, self.oracle.room.abstractRoom.name, worldCoordinate);
                            Vector2 down = Vector2.down;
                            for (int i = 0; i < altplayer.bodyChunks.Length; i++)
                            {
                                altplayer.bodyChunks[i].HardSetPosition(self.oracle.room.MiddleOfTile(worldCoordinate) - down * (-0.5f + (float)i) * 5f);
                                altplayer.bodyChunks[i].vel = down * 2f;
                            }
                            num++;
                        }
                        catch (Exception ex)
                        {
                            string str = "Failed to move player ";
                            Exception ex2 = ex;
                            JollyCustom.Log(str + ((ex2 != null) ? ex2.ToString() : null), true);
                        }

                        if (player == null)
                        {
                            ExtEnum<AbstractPhysicalObject.AbstractObjectType> a;
                            if (altplayer == null)
                            {
                                a = null;
                            }
                            else
                            {
                                AbstractPhysicalObject objectInStomach = altplayer.objectInStomach;
                                a = ((objectInStomach != null) ? objectInStomach.type : null);

                                if (a != null && (a == AbstractPhysicalObject.AbstractObjectType.DataPearl ||
                                    objectInStomach is DataPearl.AbstractDataPearl) &&
                                    (objectInStomach as DataPearl.AbstractDataPearl).dataPearlType == UnboundEnums.unboundKarmaPearl)
                                {
                                    player = altplayer;
                                    JollyCustom.Log(string.Format("Found player with Unbound Pearl in stomach, focusing ...{0}", altplayer), false);
                                }
                            }
                        }
                    }
                    if (player != null)
                    {
                        self.player = player;
                    }
                }
                #endregion

                if (self.action != SSOracleBehavior.Action.MeetWhite_Shocked && self.action != UnboundEnums.UnbSlumberParty
                    && self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad >= 1)
                {
                    if (self.player.GetNCRunbound().MoreDebug)
                    {
                        NCRDebug.Log("Pebbles seeing Unbound again!");
                    }
                    // self.NewAction(UnboundEnums.UnbSlumberParty);
                }
                else if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad < 1)
                {
                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad = 0;
                    if (self.player.GetNCRunbound().MoreDebug && self.action != SSOracleBehavior.Action.MeetWhite_Shocked)
                    { NCRDebug.Log("Pebbles meet Unbound!"); }
                    self.NewAction(SSOracleBehavior.Action.MeetWhite_Shocked);
                    self.SlugcatEnterRoomReaction();
                }
                if (self.player.GetNCRunbound().MoreDebug)
                {
                    NCRDebug.Log("Conversations with Pebbles: " +
                            self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad.ToString());
                }

                if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad != 0)
                {
                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
                }
            }
            else
            {
                orig(self);
            }
        }

        private static void UpdateBehavior(On.SSOracleBehavior.orig_Update orig, SSOracleBehavior self, bool eu)
        {
            if (self != null && self.player != null && self.player.room != null && !self.player.room.game.rainWorld.safariMode &&
                self.oracle.ID == Oracle.OracleID.SS && self.oracle != null && 
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                if (self.player.dead && self.currSubBehavior.ID != SSOracleBehavior.SubBehavior.SubBehavID.ThrowOut)
                {
                    // kicks out dead player
                    self.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                }

                if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad > 1 &&
                    self.inspectPearl == null && // so it never triggers
                    self.inspectPearl != null)
                {
                    self.movementBehavior = SSOracleBehavior.MovementBehavior.Meditate;
                    if (self.inspectPearl.grabbedBy.Count > 0)
                    {
                        for (int i = 0; i < self.inspectPearl.grabbedBy.Count; i++)
                        {
                            Creature playergrabber = self.inspectPearl.grabbedBy[i].grabber;
                            if (playergrabber != null)
                            {
                                for (int j = 0; j < playergrabber.grasps.Length; j++)
                                {
                                    if (playergrabber.grasps[j].grabbed != null && playergrabber.grasps[j].grabbed == self.inspectPearl)
                                    {
                                        playergrabber.ReleaseGrasp(j);
                                    }
                                }
                            }
                        }
                    }

                    Vector2 oracletopearl = self.oracle.firstChunk.pos - self.inspectPearl.firstChunk.pos;
                    float oraclepearldis = Custom.Dist(self.oracle.firstChunk.pos, self.inspectPearl.firstChunk.pos);

                    self.inspectPearl.firstChunk.vel += Vector2.ClampMagnitude(oracletopearl, 40f) / 40f *
                        Mathf.Clamp(2f - oraclepearldis / 200f * 2f, 0.5f, 2f);

                    if (self.inspectPearl.firstChunk.vel.magnitude < 1f && oraclepearldis < 16f)
                    {
                        self.inspectPearl.firstChunk.vel = Custom.RNV() * 8f;
                    }
                    if (self.inspectPearl.firstChunk.vel.magnitude > 8f)
                    {
                        self.inspectPearl.firstChunk.vel /= 2f;
                    }

                    if (oraclepearldis < 100f && self.pearlConversation == null && self.conversation == null)
                    {
                        self.StartItemConversation(self.inspectPearl);
                    }
                }
                self.UpdateStoryPearlCollection();

                if (self.timeSinceSeenPlayer >= 0)
                {
                    self.timeSinceSeenPlayer++;
                }

                if (self.pearlPickupReaction && self.timeSinceSeenPlayer > 300 && self.oracle.room.game.IsStorySession &&
                    self.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark &&
                    (!(self.currSubBehavior is SSOracleBehavior.ThrowOutBehavior) ||
                    self.action == SSOracleBehavior.Action.ThrowOut_Polite_ThrowOut))
                {
                    bool PebbsPearl = false;
                    for (int k = 0; k < self.player.grasps.Length; k++)
                    {
                        if (self.player.grasps[k] != null && self.player.grasps[k].grabbed is PebblesPearl)
                        {
                            PebbsPearl = true;
                            break;
                        }
                    }

                    if (PebbsPearl && !self.lastPearlPickedUp &&
                        self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad < 1 &&
                        (self.conversation == null || (self.conversation.age > 300 && !self.conversation.paused)))
                    {
                        if (self.conversation != null)
                        {
                            self.conversation.paused = true;
                            self.restartConversationAfterCurrentDialoge = true;
                        }
                        self.dialogBox.Interrupt(self.Translate(
                            "FP: Yes, help yourself. They are not edible."), 10);
                        self.pearlPickupReaction = false;
                    }
                    self.lastPearlPickedUp = PebbsPearl;
                }

                if (self.conversation != null)
                {
                    if (self.restartConversationAfterCurrentDialoge && self.conversation.paused &&
                        self.action != SSOracleBehavior.Action.General_GiveMark && self.dialogBox.messages.Count == 0 &&
                        (!ModManager.MSC || self.player.room == self.oracle.room))
                    {
                        self.conversation.paused = false;
                        self.restartConversationAfterCurrentDialoge = false;
                        self.conversation.RestartCurrent();
                    }
                }
                else if (self.pearlConversation != null)
                {
                    if (self.pearlConversation.slatedForDeletion)
                    {
                        self.pearlConversation = null;
                        if (self.inspectPearl != null)
                        {
                            self.inspectPearl.firstChunk.vel = Custom.DirVec(self.inspectPearl.firstChunk.pos, self.player.mainBodyChunk.pos) * 3f;
                            self.readDataPearlOrbits.Add(self.inspectPearl.AbstractPearl);
                            self.inspectPearl = null;
                        }
                    }
                    else
                    {
                        self.pearlConversation.Update();
                        if (self.player.room != self.oracle.room)
                        {
                            if (self.player.room != null && !self.pearlConversation.paused)
                            {
                                self.pearlConversation.paused = true;
                                self.InterruptPearlMessagePlayerLeaving();
                            }
                        }
                        else if (self.pearlConversation.paused && !self.restartConversationAfterCurrentDialoge)
                        {
                            self.ResumePausedPearlConversation();
                        }
                        if (self.pearlConversation.paused && self.restartConversationAfterCurrentDialoge && self.dialogBox.messages.Count == 0)
                        {
                            self.pearlConversation.paused = false;
                            self.restartConversationAfterCurrentDialoge = false;
                            self.pearlConversation.RestartCurrent();
                        }
                    }
                }
                else
                {
                    self.restartConversationAfterCurrentDialoge = false;
                }

                #region Base.Update
                if (self.voice != null)
                {
                    self.voice.alive = true;
                    if (self.voice.slatedForDeletetion)
                    {
                        self.voice = null;
                    }
                }
                self.FindPlayer();
                #endregion

                for (int l = 0; l < self.oracle.room.game.cameras.Length; l++)
                {
                    if (self.oracle.room.game.cameras[l].room == self.oracle.room)
                    {
                        self.oracle.room.game.cameras[l].virtualMicrophone.volumeGroups[2] = 1f - self.oracle.room.gravity;
                    }
                    else
                    {
                        self.oracle.room.game.cameras[l].virtualMicrophone.volumeGroups[2] = 1f;
                    }
                }
                if (!self.oracle.Consious)
                {
                    return;
                }
                self.unconciousTick = 0f;
                self.currSubBehavior.Update();
                if (self.oracle.slatedForDeletetion)
                {
                    return;
                }
                if (self.conversation != null)
                {
                    self.conversation.Update();
                }

                if (!self.currSubBehavior.CurrentlyCommunicating && self.pearlConversation == null)
                {
                    self.pathProgression = Mathf.Min(1f, self.pathProgression + 1f / Mathf.Lerp(40f + self.pathProgression * 80f,
                        Vector2.Distance(self.lastPos, self.nextPos) / 5f, 0.5f));
                }
                
                self.currentGetTo = Custom.Bezier(self.lastPos, self.ClampVectorInRoom(self.lastPos + self.lastPosHandle),
                    self.nextPos, self.ClampVectorInRoom(self.nextPos + self.nextPosHandle), self.pathProgression);
                self.floatyMovement = false;
                self.investigateAngle += self.invstAngSpeed;
                self.inActionCounter++;

                if (self.player.room == self.oracle.room)
                {
                    self.playerOutOfRoomCounter = 0;
                }
                else
                {
                    self.killFac = 0f;
                    self.playerOutOfRoomCounter++;
                }

                if (self.pathProgression >= 1f && self.consistentBasePosCounter > 100 && !self.oracle.arm.baseMoving)
                {
                    self.allStillCounter++;
                }
                else
                {
                    self.allStillCounter = 0;
                }

                self.lastKillFac = self.killFac;
                self.lastKillFacOverseer = self.killFacOverseer;

                if (self.action == SSOracleBehavior.Action.General_Idle)
                {
                    if (self.movementBehavior != SSOracleBehavior.MovementBehavior.Idle &&
                        self.movementBehavior != SSOracleBehavior.MovementBehavior.Meditate)
                    {
                        self.movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
                    }
                    self.throwOutCounter = 0;

                    if (self.player.room == self.oracle.room)
                    {
                        self.discoverCounter++;
                        if (self.oracle.room.GetTilePosition(self.player.mainBodyChunk.pos).y < 32 && (self.discoverCounter > 220 ||
                            Custom.DistLess(self.player.mainBodyChunk.pos, self.oracle.firstChunk.pos, 150f) ||
                            !Custom.DistLess(self.player.mainBodyChunk.pos,
                            self.oracle.room.MiddleOfTile(self.oracle.room.ShortcutLeadingToNode(1).StartTile), 150f)))
                        {
                            self.SeePlayer();
                        }
                    }
                }
                else if (self.action == SSOracleBehavior.Action.General_GiveMark)
                {
                    self.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    if ((self.inActionCounter > 30 && self.inActionCounter < 300))
                    {
                        if (self.inActionCounter < 300)
                        {
                            if (ModManager.CoopAvailable)
                            {
                                self.StunCoopPlayers(20);
                            }
                            else
                            {
                                self.player.Stun(20);
                            }
                        }
                        Vector2 pebblesToUnbound = Vector2.ClampMagnitude(self.oracle.room.MiddleOfTile(24, 14) -
                            self.player.mainBodyChunk.pos, 40f) / 40f * 2.8f * Mathf.InverseLerp(30f, 160f, (float)self.inActionCounter);
                        if (ModManager.CoopAvailable)
                        {
                            using (List<Player>.Enumerator enumerator = self.PlayersInRoom.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    Player player = enumerator.Current;
                                    player.mainBodyChunk.vel += pebblesToUnbound;
                                }
                                goto IL_skipmove;
                            }
                        }
                        self.player.mainBodyChunk.vel += pebblesToUnbound;
                    }


                    IL_skipmove:
                    if (self.inActionCounter == 30)
                    {
                        self.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Telekenisis, 0f, 1f, 1f);
                    }
                    if (self.inActionCounter == 300)
                    {
                        if (!ModManager.MSC)
                        {
                            self.player.mainBodyChunk.vel += Custom.RNV() * 10f;
                            self.player.bodyChunks[1].vel += Custom.RNV() * 10f;
                        }
                        else
                        {
                            if (ModManager.CoopAvailable)
                            {
                                self.StunCoopPlayers(40);
                            }
                            else
                            {
                                self.player.Stun(40);
                            }
                        }

                        if (ModManager.CoopAvailable)
                        {
                            using (List<Player>.Enumerator playersinRoom = self.PlayersInRoom.GetEnumerator())
                            {
                                while (playersinRoom.MoveNext())
                                {
                                    Player currentGrabbedPlayer = playersinRoom.Current;
                                    for (int num3 = 0; num3 < 20; num3++)
                                    {
                                        self.oracle.room.AddObject(new Spark(currentGrabbedPlayer.mainBodyChunk.pos, Custom.RNV() *
                                            UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                                    }
                                }
                                goto IL_vineboom;
                            }
                        }
                        for (int num4 = 0; num4 < 20; num4++)
                        {
                            self.oracle.room.AddObject(new Spark(self.player.mainBodyChunk.pos, Custom.RNV() * UnityEngine.Random.value * 40f,
                                new Color(1f, 1f, 1f), null, 30, 120));
                        }

                    IL_vineboom:
                        self.oracle.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, 0f, 1f, 1f);
                    }

                    if (self.inActionCounter >= 500)
                    {
                        self.NewAction(self.afterGiveMarkAction);
                        if (self.conversation != null)
                        {
                            self.conversation.paused = false;
                        }
                    }
                }

                self.Move();
                if (self.working != self.getToWorking)
                {
                    self.working = Custom.LerpAndTick(self.working, self.getToWorking, 0.05f, 0.033333335f);
                }
                for (int oracleRoomCam = 0; oracleRoomCam < self.oracle.room.game.cameras.Length; oracleRoomCam++)
                {
                    if (self.oracle.room.game.cameras[oracleRoomCam].room == self.oracle.room &&
                        !self.oracle.room.game.cameras[oracleRoomCam].AboutToSwitchRoom &&
                        self.oracle.room.game.cameras[oracleRoomCam].paletteBlend != self.working)
                    {
                        self.oracle.room.game.cameras[oracleRoomCam].ChangeBothPalettes(25, 26, self.working);
                    }
                }

                if (self.player.room == self.oracle.room && self.oracle.room.physicalObjects != null)
                {
                    List<PhysicalObject>[] physicalObjects = self.oracle.room.physicalObjects;
                    for (int ObjLength = 0; ObjLength < physicalObjects.Length; ObjLength++)
                    {
                        for (int ObjCount = 0; ObjCount < physicalObjects[ObjLength].Count; ObjCount++)
                        {
                            PhysicalObject physicalObject = physicalObjects[ObjLength][ObjCount];

                            if (!self.talkedAboutThisSession.Contains(physicalObject.abstractPhysicalObject.ID) &&
                                (self.action == UnboundEnums.UnbSlumberParty ||
                                self.action == SSOracleBehavior.Action.General_Idle) &&
                                self.currSubBehavior is UnboundPebblesSleepover &&
                                self.inspectPearl == null && (self.conversation == null ||
                                self.conversation.id == UnboundEnums.unbSlumberConv) &&
                                !(self.currSubBehavior is SSOracleBehavior.ThrowOutBehavior) &&
                                physicalObject is DataPearl && (physicalObject as DataPearl).grabbedBy.Count == 0 &&
                                ((physicalObject as DataPearl).AbstractPearl.dataPearlType != 
                                DataPearl.AbstractDataPearl.DataPearlType.PebblesPearl))
                            {
                                self.inspectPearl = (physicalObject as DataPearl);

                                if (self.player.GetNCRunbound().MoreDebug)
                                { NCRDebug.Log("Inspecting pearl: " + self.inspectPearl.AbstractPearl.dataPearlType); }
                                break;
                            }
                            else
                            {
                                self.inspectPearl = null;
                            }
                        }
                    }
                }
                if (self.currSubBehavior.LowGravity >= 0f)
                {
                    self.oracle.room.gravity = self.currSubBehavior.LowGravity;
                    return;
                }
                if (!self.currSubBehavior.Gravity)
                {
                    self.oracle.room.gravity = Custom.LerpAndTick(self.oracle.room.gravity, 0f, 0.05f, 0.02f);
                    return;
                }

                if (self.action != SSOracleBehavior.Action.ThrowOut_KillOnSight)
                {
                    self.oracle.room.gravity = 1f - self.working;
                }
            }
            else
            {
                orig(self, eu);
            }
        }

        private static void ThrowOutToSleepover(On.SSOracleBehavior.ThrowOutBehavior.orig_Update orig, SSOracleBehavior.ThrowOutBehavior self)
        {
            if (self != null && self.player != null && self.player.room != null &&
                self.player.room.game.session.characterStats.name.value == "NCRunbound")
            {
                self.owner.LockShortcuts();
                if (self.owner.conversation != null && self.owner.conversation.colorMode == false)
                {
                    self.owner.conversation.colorMode = true;
                }
                #region SetupandSuch
                if (self.player.room == self.oracle.room || (ModManager.MSC && self.oracle.room.abstractRoom.creatures.Count > 0))
                {

                    if (self.owner.action != SSOracleBehavior.Action.ThrowOut_KillOnSight && self.owner.throwOutCounter < 900)
                    {
                        Vector2 vector = self.oracle.room.MiddleOfTile(28, 33);
                        using (List<AbstractCreature>.Enumerator enumerator = self.oracle.room.abstractRoom.creatures.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                AbstractCreature creatureBeingPushed = enumerator.Current;
                                if (creatureBeingPushed.realizedCreature != null)
                                {
                                    if (!self.oracle.room.aimap.getAItile(creatureBeingPushed.realizedCreature.mainBodyChunk.pos).narrowSpace ||
                                        creatureBeingPushed.realizedCreature != self.player)
                                    {
                                        creatureBeingPushed.realizedCreature.mainBodyChunk.vel +=
                                            Custom.DirVec(creatureBeingPushed.realizedCreature.mainBodyChunk.pos, vector) * 0.2f *
                                            (1f - self.oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, (float)self.inActionCounter);
                                    }
                                    else if (creatureBeingPushed.realizedCreature != null && creatureBeingPushed.realizedCreature != self.player &&
                                        creatureBeingPushed.realizedCreature.enteringShortCut == null &&
                                        creatureBeingPushed.pos == self.owner.oracle.room.ToWorldCoordinate(vector))
                                    {
                                        creatureBeingPushed.realizedCreature.enteringShortCut = new
                                            IntVector2?(self.owner.oracle.room.ToWorldCoordinate(vector).Tile);
                                        if (creatureBeingPushed.abstractAI.RealAI != null)
                                        {
                                            creatureBeingPushed.abstractAI.RealAI.SetDestination(self.owner.oracle.room.ToWorldCoordinate(vector));
                                        }
                                    }
                                }
                            }
                            goto IL_ThrowOut;
                        }
                    }

                    else if (self.telekinThrowOut)
                    {
                        Vector2 vector2 = self.oracle.room.MiddleOfTile(28, 33);
                        foreach (AbstractCreature otherCreature in self.oracle.room.abstractRoom.creatures)
                        {
                            if (otherCreature.realizedCreature != null)
                            {
                                if (!self.oracle.room.aimap.getAItile(otherCreature.realizedCreature.mainBodyChunk.pos).narrowSpace ||
                                    otherCreature.realizedCreature != self.player)
                                {
                                    otherCreature.realizedCreature.mainBodyChunk.vel +=
                                        Custom.DirVec(self.player.mainBodyChunk.pos, self.oracle.room.MiddleOfTile(28, 32)) * 0.2f *
                                        (1f - self.oracle.room.gravity) * Mathf.InverseLerp(220f, 280f, (float)self.inActionCounter);
                                }
                                else if (otherCreature.realizedCreature != self.player && otherCreature.realizedCreature.enteringShortCut == null &&
                                    otherCreature.pos == self.owner.oracle.room.ToWorldCoordinate(vector2))
                                {
                                    otherCreature.realizedCreature.enteringShortCut = new IntVector2?(self.owner.oracle.room.ToWorldCoordinate(vector2).Tile);
                                    if (otherCreature.abstractAI.RealAI != null)
                                    {
                                        otherCreature.abstractAI.RealAI.SetDestination(self.owner.oracle.room.ToWorldCoordinate(vector2));
                                    }
                                }
                            }
                        }
                    }
                }
#endregion

                IL_ThrowOut:
                if (self.action == SSOracleBehavior.Action.ThrowOut_ThrowOut)
                {
                    if (self.player.room == self.oracle.room)
                    {
                        self.owner.throwOutCounter++;
                    }
                    self.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    self.telekinThrowOut = (self.inActionCounter > 220);
                    
                    if (self.owner.throwOutCounter == 700)
                    {
                        self.dialogBox.Interrupt(self.Translate("FP: That's all. You'll have to go now."), 0);
                    }
                    else if (self.owner.throwOutCounter == 980)
                    {
                        self.dialogBox.Interrupt(self.Translate("FP: LEAVE."), 0);
                    }
                    else if (self.owner.throwOutCounter == 1530)
                    {
                        self.dialogBox.Interrupt(self.Translate("FP: Little creature. This is your last warning."), 0);
                    }
                    else if (self.owner.throwOutCounter > 1780)
                    {
                        self.owner.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
                    }

                    if ((self.owner.playerOutOfRoomCounter > 100 && self.owner.throwOutCounter > 400) || self.owner.throwOutCounter > 3200)
                    {
                        self.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                        self.owner.getToWorking = 1f;
                        return;
                    }
                }
                else if (self.action == SSOracleBehavior.Action.ThrowOut_KillOnSight)
                {
                    if (ModManager.MMF)
                    {
                        if (self.player.room == self.oracle.room)
                        {
                            self.owner.throwOutCounter++;
                        }
                        if (self.owner.throwOutCounter == 10)
                        {
                            self.dialogBox.Interrupt(self.Translate("FP: ..."), 200);
                        }
                    }
                    if (!ModManager.MMF || self.owner.throwOutCounter >= 200)
                    {
                        if ((!self.player.dead || self.owner.killFac > 0.5f) && self.player.room == self.oracle.room)
                        {
                            self.owner.killFac += 0.025f;
                            if (self.owner.killFac >= 1f)
                            {
                                self.player.mainBodyChunk.vel += Custom.RNV() * 12f;
                                for (int k = 0; k < 20; k++)
                                {
                                    self.oracle.room.AddObject(new Spark(self.player.mainBodyChunk.pos, Custom.RNV() *
                                        UnityEngine.Random.value * 40f, new Color(1f, 1f, 1f), null, 30, 120));
                                }
                                self.owner.killFac = 0f;
                                self.owner.player.GetNCRunbound().pebbleskilltries++;
                                if (self.player.GetNCRunbound().MoreDebug) { NCRDebug.Log("Pebbles attempting to kill Unbound"); }
                                self.player.room.PlaySound(SoundID.Snail_Pop, self.player.mainBodyChunk, false, 1f, 1.5f + UnityEngine.Random.value);

                                if (self.owner.player.GetNCRunbound().pebbleskilltries == 3)
                                {
                                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad = 1;
                                    self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts = 1;
                                    if (self.player.GetNCRunbound().MoreDebug) { NCRDebug.Log("Pebbles failed to kill Unbound 3 times!"); }
                                    self.owner.SlugcatEnterRoomReaction();

                                    self.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            self.owner.killFac *= 0.8f;
                            self.owner.getToWorking = 1f;
                            self.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                            if (self.player.room != self.oracle.room && self.oracle.oracleBehavior.PlayersInRoom.Count <= 0)
                            {
                                self.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                                return;
                            }
                            if (!ModManager.CoopAvailable)
                            {
                                self.player.mainBodyChunk.vel += Custom.DirVec(self.player.mainBodyChunk.pos, self.oracle.room.MiddleOfTile(28, 32)) * 0.6f * (1f - self.oracle.room.gravity);
                                if (self.oracle.room.GetTilePosition(self.player.mainBodyChunk.pos) == new IntVector2(28, 32) && self.player.enteringShortCut == null)
                                {
                                    self.player.enteringShortCut = new IntVector2?(self.oracle.room.ShortcutLeadingToNode(1).StartTile);
                                    return;
                                }
                                return;
                            }
                            else
                            {
                                using (List<Player>.Enumerator enumerator2 = self.oracle.oracleBehavior.PlayersInRoom.GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        Player player = enumerator2.Current;
                                        player.mainBodyChunk.vel += Custom.DirVec(player.mainBodyChunk.pos, self.oracle.room.MiddleOfTile(28, 32)) *
                                            0.6f * (1f - self.oracle.room.gravity);
                                        if (self.oracle.room.GetTilePosition(player.mainBodyChunk.pos) == new IntVector2(28, 32) &&
                                            player.enteringShortCut == null)
                                        {
                                            player.enteringShortCut = new IntVector2?(self.oracle.room.ShortcutLeadingToNode(1).StartTile);
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
                #region Singularity Bomb
                else if (ModManager.MSC && self.action == MoreSlugcatsEnums.SSOracleBehaviorAction.ThrowOut_Singularity)
                {
                    if (self.inActionCounter == 10)
                    {
                        if ((self.oracle.oracleBehavior as SSOracleBehavior).conversation != null)
                        {
                            (self.oracle.oracleBehavior as SSOracleBehavior).conversation.Destroy();
                            (self.oracle.oracleBehavior as SSOracleBehavior).conversation = null;
                        }
                        self.dialogBox.Interrupt(self.Translate("FP: . . . !"), 0);
                    }
                    self.owner.getToWorking = 1f;
                    if (self.player.room != self.oracle.room && !self.player.inShortcut)
                    {
                        if (self.player.grasps[0] != null && self.player.grasps[0].grabbed is SingularityBomb)
                        {
                            (self.player.grasps[0].grabbed as SingularityBomb).Thrown(self.player, self.player.firstChunk.pos, null, new IntVector2(0, -1), 1f, true);
                            (self.player.grasps[0].grabbed as SingularityBomb).ignited = true;
                            (self.player.grasps[0].grabbed as SingularityBomb).activateSucktion = true;
                            (self.player.grasps[0].grabbed as SingularityBomb).counter = 50f;
                            (self.player.grasps[0].grabbed as SingularityBomb).floatLocation = self.player.firstChunk.pos;
                            (self.player.grasps[0].grabbed as SingularityBomb).firstChunk.pos = self.player.firstChunk.pos;
                        }
                        if (self.player.grasps[1] != null && self.player.grasps[1].grabbed is SingularityBomb)
                        {
                            (self.player.grasps[1].grabbed as SingularityBomb).Thrown(self.player, self.player.firstChunk.pos, null, new IntVector2(0, -1), 1f, true);
                            (self.player.grasps[1].grabbed as SingularityBomb).ignited = true;
                            (self.player.grasps[1].grabbed as SingularityBomb).activateSucktion = true;
                            (self.player.grasps[1].grabbed as SingularityBomb).counter = 50f;
                            (self.player.grasps[1].grabbed as SingularityBomb).floatLocation = self.player.firstChunk.pos;
                            (self.player.grasps[1].grabbed as SingularityBomb).firstChunk.pos = self.player.firstChunk.pos;
                        }
                        self.player.Stun(200);
                        self.owner.NewAction(SSOracleBehavior.Action.General_Idle);
                        return;
                    }
                    self.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                    if (self.oracle.ID == Oracle.OracleID.SS)
                    {
                        self.player.mainBodyChunk.vel += Custom.DirVec(self.player.mainBodyChunk.pos, self.oracle.room.MiddleOfTile(28, 32)) * 1.3f;
                        self.player.mainBodyChunk.pos = Vector2.Lerp(self.player.mainBodyChunk.pos, self.oracle.room.MiddleOfTile(28, 32), 0.08f);
                        if (self.player.enteringShortCut == null && self.player.mainBodyChunk.pos.x < 560f && self.player.mainBodyChunk.pos.y > 630f)
                        {
                            self.player.mainBodyChunk.pos.y = 630f;
                        }
                        if ((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity != null)
                        {
                            (self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.activateSucktion = false;
                            (self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.vel += Custom.DirVec((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, self.player.mainBodyChunk.pos) * 1.3f;
                            (self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos = Vector2.Lerp((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, self.player.mainBodyChunk.pos, 0.1f);
                            if (Vector2.Distance((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity.firstChunk.pos, self.player.mainBodyChunk.pos) < 10f)
                            {
                                if (self.player.grasps[0] == null)
                                {
                                    self.player.SlugcatGrab((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 0);
                                }
                                if (self.player.grasps[1] == null)
                                {
                                    self.player.SlugcatGrab((self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity, 1);
                                }
                            }
                        }
                        if (self.oracle.room.GetTilePosition(self.player.mainBodyChunk.pos) == new IntVector2(28, 32) && self.player.enteringShortCut == null)
                        {
                            bool flag = false;
                            if (self.player.grasps[0] != null && self.player.grasps[0].grabbed == (self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                            {
                                flag = true;
                            }
                            if (self.player.grasps[1] != null && self.player.grasps[1].grabbed == (self.oracle.oracleBehavior as SSOracleBehavior).dangerousSingularity)
                            {
                                flag = true;
                            }
                            if (flag)
                            {
                                self.player.enteringShortCut = new IntVector2?(self.oracle.room.ShortcutLeadingToNode(1).StartTile);
                                return;
                            }
                            self.player.ReleaseGrasp(0);
                            self.player.ReleaseGrasp(1);
                        }
                    }
                    else
                    {
                        orig(self);
                    }
                }
                #endregion
            }
            else
            {
                orig(self);
            }
        }
    }
}
