using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbound
{
    public class KTBIntro : UpdatableAndDeletable
    {
        public Player player;
        public FadeOut fadeOut;
        public float afterFadeTime;
        public bool triggered;

        public KTBIntro(Room room)
        {
            this.room = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            try
            {
                if (this.player == null && this.room?.game?.Players != null && this.room.game.Players.Count > 0 &&
                this.room.game.FirstAlivePlayer?.realizedCreature != null &&
                this.room.game.FirstAlivePlayer.realizedCreature.room == this.room)
                {
                    this.player = this.room.game.FirstAlivePlayer.realizedCreature as Player;
                }
            }
            catch (Exception e) 
            {
                NCRDebug.Log("Error finding Unbound player: " + e);
            }
            
            try
            {
                if (this.player != null && this.player.mainBodyChunk.pos.y < 834f)
                {
                    if (this.fadeOut == null)
                    {
                        this.fadeOut = new FadeOut(this.room, Color.black, 60f, false);
                        this.room.AddObject(this.fadeOut);
                    }
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error fading out: " + e);
            }


            try
            {
                if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.triggered)
                {
                    this.afterFadeTime += 1f;
                    if (this.afterFadeTime > 120f)
                    {
                        this.triggered = true;
                        this.room.world.game.globalRain.ResetRain();

                        if (ModManager.MSC)
                        {
                            NCRDebug.Log("MSC Unbound save! Transferring player to MS_UNBSTART...");
                            RainWorldGame.ForceSaveNewDenLocation(this.room.game, "MS_UNBSTART", true);
                        }
                        else
                        {
                            NCRDebug.Log("Vanilla Unbound save! Transferring player to SL_S11...");
                            RainWorldGame.ForceSaveNewDenLocation(this.room.game, "SL_S11", true);
                        }

                        IL_killUnbound:
                        this.player.Die();
                        if (!this.player.dead)
                        {
                            NCRDebug.Log("Unbound failed to die! Re-attempting...");
                            goto IL_killUnbound;
                        }
                        this.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = false;
                        // to 're-trigger' the intro cutscene

                        if (ModManager.ActiveMods.Any(mod => mod.id == "fake_achievements"))
                        {
                            AchievementsManager.ShowAchievement("unbtheend");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error ending fade: " + e);
            }
        }
    }
}
