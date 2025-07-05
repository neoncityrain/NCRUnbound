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
                if (player == null && room?.game?.Players != null && room.game.Players.Count > 0 &&
                room.game.FirstAlivePlayer?.realizedCreature != null &&
                room.game.FirstAlivePlayer.realizedCreature.room == room)
                {
                    player = room.game.FirstAlivePlayer.realizedCreature as Player;
                }
            }
            catch (Exception e) 
            {
                NCRDebug.Log("Error finding Unbound player: " + e);
            }
            
            try
            {
                if (player != null && player.mainBodyChunk.pos.y < 834f)
                {
                    if (fadeOut == null)
                    {
                        fadeOut = new FadeOut(room, Color.black, 60f, false);
                        room.AddObject(fadeOut);
                    }
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error fading out: " + e);
            }


            try
            {
                if (fadeOut != null && fadeOut.IsDoneFading() && !triggered)
                {
                    afterFadeTime += 1f;
                    if (afterFadeTime > 120f)
                    {
                        triggered = true;
                        room.world.game.globalRain.ResetRain();

                        if (ModManager.MSC)
                        {
                            NCRDebug.Log("MSC Unbound save! Transferring player to MS_UNBSTART...");
                            RainWorldGame.ForceSaveNewDenLocation(room.game, "MS_UNBSTART", true);
                        }
                        else
                        {
                            NCRDebug.Log("Vanilla Unbound save! Transferring player to SL_S11...");
                            RainWorldGame.ForceSaveNewDenLocation(room.game, "SL_S11", true);
                        }

                        IL_killUnbound:
                        player.Die();
                        if (!player.dead)
                        {
                            NCRDebug.Log("Unbound failed to die! Re-attempting...");
                            goto IL_killUnbound;
                        }
                        room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = false;
                        // to 're-trigger' the intro cutscene
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
