using BuiltinBuffs;
using System.Linq;

namespace Unbound
{
    internal class RandomBuffThings
    {
        public static void TailTracking(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (self != null && self.room != null && ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff"))
            { 
                self.GetNCRunbound().LostTail = !self.GetExPlayerData().HaveTail; 
            }
            else { self.GetNCRunbound().LostTail = false; }
        }


        public static void SetUpRGBForRB(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (!(self.player.GetNCRunbound().GraphicsDisabled && self.player.GetNCRunbound().RingsDisabled) &&
                self != null && self.player != null && self.player.room != null &&
                self.player.GetNCRunbound().IsNCRUnbModcat && !self.player.GetNCRunbound().IsOracle)
            {
                var rev = self.player.GetNCRunbound().Reverb;
                Color effectcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.24f, 0.14f, 0.05f) :
                    (rev ? new Color(0.72f, 0.6f, 0.6f) : new Color(0.87f, 0.39f, 0.33f));
                Color eyecol = self.player.GetNCRunbound().IsTechnician ? new Color(0.42f, 0.21f, 0.18f) :
                    (rev ? new Color(0.51f, 0.2f, 0.22f) : new Color(0.07f, 0.2f, 0.31f));
                Color bodycol = self.player.GetNCRunbound().IsTechnician ? new Color(0.91f, 0.8f, 0.53f) :
                    (rev ? new Color(0.95f, 0.91f, 0.91f) : new Color(0.89f, 0.79f, 0.6f));
                Color pupilcol = self.player.GetNCRunbound().IsTechnician ? new Color(0.26f, 0.09f, 0.08f) : effectcol;

                if (self.player.room.game.IsArenaSession && !self.player.GetNCRunbound().IsTechnician)
                {
                    switch (self.player.playerState.playerNumber)
                    {
                        case 0:
                            if (rCam.room.game.GetArenaGameSession.arenaSitting.gameTypeSetup.gameType != MoreSlugcatsEnums.GameTypeID.Challenge)
                            {
                                if (!rev)
                                {
                                    effectcol = new Color(0.42f, 0.31f, 0.78f);
                                    eyecol = new Color(0.22f, 0.05f, 0.09f);
                                    bodycol = new Color(0.96f, 0.95f, 0.98f);
                                }
                            }
                            break;
                        case 1:
                            if (!rev)
                            {
                                effectcol = new Color(0.11f, 0.74f, 0.58f);
                                eyecol = new Color(0.48f, 14f, 0.07f);
                                bodycol = new Color(0.97f, 0.84f, 0.45f);
                            }
                            break;
                        case 2:
                            if (!rev)
                            {
                                effectcol = new Color(0.84f, 0.08f, 0.3f);
                                eyecol = new Color(0.12f, 0.21f, 0.27f);
                                bodycol = new Color(0.98f, 0.58f, 0.38f);
                            }
                            break;
                        case 3:
                            if (!rev)
                            {
                                effectcol = new Color(0.86f, 0.23f, 0.93f);
                                eyecol = new Color(0.62f, 0.75f, 0.97f);
                                bodycol = new Color(0.06f, 0.11f, 0.24f);
                            }
                            break;
                    }
                }
                else if (self.useJollyColor)
                {
                    effectcol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                    eyecol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 1);
                    bodycol = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 0);
                }
                else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                {
                    effectcol = PlayerGraphics.CustomColorSafety(2);
                    eyecol = PlayerGraphics.CustomColorSafety(1);
                    bodycol = PlayerGraphics.CustomColorSafety(0);
                }

                if (self.player.GetNCRunbound().RGBRings)
                {
                    effectcol = new HSLColor(Mathf.Sin(self.player.GetNCRunbound().RGBCounter / 200f), 1f, 0.75f).rgb;
                    pupilcol = effectcol;
                }

                if (self.player.GetNCRunbound().effectColour == null || self.player.GetNCRunbound().effectColour != effectcol)
                {
                    self.player.GetNCRunbound().effectColour = effectcol;
                }
            }
            orig(self, sLeaser, rCam, timeStacker, camPos);
        }
        // end random buff exclusive
    }
}
