using System;

namespace Unbound
{
    public class UnboundPebblesSleepover : SSOracleBehavior.ConversationBehavior
    {
        public bool holdPlayer;

        public UnboundPebblesSleepover(SSOracleBehavior owner) :
            base(owner, UnboundEnums.UnbSlumberPartySub, UnboundEnums.unbSlumberConv)
        {
            #region Setup Things
            if (!base.oracle.room.game.GetStorySession.saveState.deathPersistentSaveData.theMark)
            {
                // this should be replaced by another savedata value, as unbound will always have the mark
                return;
            }
            if (this.owner.conversation != null)
            {
                this.owner.conversation.Destroy();
                this.owner.conversation = null;
                return;
            }
            this.owner.TurnOffSSMusic(true);
            if (this.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts == 1) 
            {
                // AKA, the first time unbound meets pebbles
                this.holdPlayer = true;

                this.movementBehavior = SSOracleBehavior.MovementBehavior.KeepDistance;
                this.dialogBox.NewMessage(this.Translate("FP: . . ."), 20);
                this.dialogBox.NewMessage(this.Translate(
                    "FP: Is this some sort of joke to you?"), 20);
                this.dialogBox.NewMessage(this.Translate(
                    "FP: You are but an animal, yet you stand here, resisting my every attempt to push you out."), 40);
                this.dialogBox.NewMessage(this.Translate(
                    "FP: There was one like you, once. One who is now long since gone.<LINE>" +
                    "As all things eventually."), 50);
                this.dialogBox.NewMessage(this.Translate(
                    "FP: You are no replacement, as I have no use for you."), 20);
            }
            if (this.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 1)
            {
                owner.getToWorking = 1f;
            }
            #endregion

            if (!base.oracle.room.game.rainWorld.ExpeditionMode)
            {
                NCRDebug.Log("SLUMBER PARTY!");
                
                #region Intro Text
                if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 1)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Forget this. Giving you attention will only encourage you to stay."), 10);
                }
                else if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 2)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: What is your purpose in coming here? Are you here to mock me?"), 0);
                }
                else if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 3)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Do you mind? I am busy."), 0);
                }
                else if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 8)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: I will not rid myself of you, will I...?"), 0);
                }
                else if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 9)
                {
                    base.dialogBox.NewMessage(base.Translate("FP: . . ."), 0);
                }
                else if (base.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 10)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: I have been nothing but cold to you. And yet, here you are."), 0);
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: What goes through your miniscule synapses?"), 0);
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Is it only because I cannot kill you? Or is there another reason?"), 10);
                }
                // end conversation-number-dependent dialogue, start random dialogue
                else if (UnityEngine.Random.value < 0.1f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Your presence is not welcome. Please leave."), 0);
                }
                else if (UnityEngine.Random.value < 0.3f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: What is it this time?"), 0);
                }
                else if (UnityEngine.Random.value < 0.3f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: There are plenty of better things we both could be doing. And yet here you are."), 0);
                }
                else if (UnityEngine.Random.value < 0.3f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: I am quite busy. Get on with it."), 0);
                }
                else if (UnityEngine.Random.value < 0.5f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Do you mind? I am busy."), 0);
                }
                else if (UnityEngine.Random.value < 0.1f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: I suppose your presence is not entirely unwelcome at this time."), 0);
                }
                else
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: .  .  ."), 0);
                }

                return;
                #endregion
            }
            if (ModManager.Expedition && base.oracle.room.game.rainWorld.ExpeditionMode)
            {
                #region Expedition Intro Text
                if (UnityEngine.Random.value < 0.3f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Yes? What is it?"), 0);
                }
                else if (UnityEngine.Random.value < 0.5f)
                {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: Are you not busy? Are we both not busy, beast?"), 0);
                }
                else {
                    base.dialogBox.NewMessage(base.Translate(
                        "FP: . . ."), 0);
                }
                return;
                #endregion
            }
        }

        public override void Activate(SSOracleBehavior.Action oldAction, SSOracleBehavior.Action newAction)
        {
            base.Activate(oldAction, newAction);
        }

        public override void NewAction(SSOracleBehavior.Action oldAction, SSOracleBehavior.Action newAction)
        {
            base.NewAction(oldAction, newAction);
            if (newAction == SSOracleBehavior.Action.ThrowOut_KillOnSight && this.owner.conversation != null)
            {
                this.owner.conversation.Destroy();
                this.owner.conversation = null;
            }
        }

        public override void Update()
        {
            base.Update();
            if (base.player == null || base.oracle == null || this == null || this.owner == null)
            {
                return;
            }

            if (this.holdPlayer && (this.dialogBox == null || this.dialogBox.slatedForDeletion || !this.dialogBox.ShowingAMessage))
            {
                this.owner.UnlockShortcuts();
                this.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts = 2;
                this.holdPlayer = false;
                this.owner.getToWorking = 1f;

                this.movementBehavior = SSOracleBehavior.MovementBehavior.Idle;
            }

            if (this.owner.conversation != null && !this.owner.conversation.colorMode) 
            {
                this.owner.conversation.colorMode = true;
            }

            if (this.holdPlayer && base.player.room == base.oracle.room)
            {
                base.player.mainBodyChunk.vel *= Custom.LerpMap((float)base.inActionCounter, 0f, 30f, 1f, 0.95f);
                base.player.bodyChunks[1].vel *= Custom.LerpMap((float)base.inActionCounter, 0f, 30f, 1f, 0.95f);
                base.player.mainBodyChunk.vel += Custom.DirVec(base.player.mainBodyChunk.pos, this.holdPlayerPos) *
                    Mathf.Lerp(0.5f, Custom.LerpMap(Vector2.Distance(base.player.mainBodyChunk.pos, this.holdPlayerPos), 30f, 150f, 2.5f, 7f),
                    base.oracle.room.gravity) * Mathf.InverseLerp(0f, 10f, (float)base.inActionCounter) * Mathf.InverseLerp(0f, 30f,
                    Vector2.Distance(base.player.mainBodyChunk.pos, this.holdPlayerPos));
                this.player.Stun(15);
            }

            this.owner.SetNewDestination(base.oracle.firstChunk.pos);

            if (base.action == SSOracleBehavior.Action.General_GiveMark)
            {
                return;
            }
        }

        public Vector2 holdPlayerPos { get { return new Vector2(483f, 360f + Mathf.Sin((float)base.inActionCounter / 70f * 3.1415927f * 2f) * 4f); } }
        // end pebblessleepover
    }
}
