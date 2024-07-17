using System;
using RWCustom;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Unbound
{
    internal class UnbGraphics
    {
        static FAtlas unbsleevesarm;
        static FAtlas unbearhead;
        static FAtlas unbpupface;

        public static void Init()
        {
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            // unbound jumpring graphics, 1-4

            // On.JollyCoop.JollyMenu.JollyPlayerSelector.SetPortraitImage_Name_Color += JollyPlayerSelector_SetPortraitImage_Name_Color;
            // wow thats a mouthful lol. dynamic jolly pfp images. currently disabled due to coding issues

            unbsleevesarm ??= Futile.atlasManager.LoadAtlas("atlases/unbearhead");
            unbsleevesarm ??= Futile.atlasManager.LoadAtlas("atlases/unbsleevesarm");
            unbpupface ??= Futile.atlasManager.LoadAtlas("atlases/unbpupface");
            // initiating atlases
        }

        //  private static void JollyPlayerSelector_SetPortraitImage_Name_Color(On.JollyCoop.JollyMenu.JollyPlayerSelector.orig_SetPortraitImage_Name_Color orig, JollyCoop.JollyMenu.JollyPlayerSelector self, SlugcatStats.Name className, Color colorTint)
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

        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark

            if (self != null && self.player != null && self.player.room != null &&
                self.player.GetCat().IsUnbound)
            {
                string faceget = sLeaser.sprites[9]?.element?.name;
                if (unbpupface == null)
                {
                    UnityEngine.Debug.Log("Unbound Pupil sprites missing!");
                }
                else if (faceget != null && faceget.StartsWith("Face") &&
                    unbpupface._elementsByName.TryGetValue("unbpup" + faceget, out var pupils))
                {
                    sLeaser.sprites[sLeaser.sprites.Length - 9].element = pupils;
                }

                sLeaser.sprites[10].RemoveFromContainer();
                sLeaser.sprites[11].RemoveFromContainer();
                // removes the mark and the marks glow

                // PLACEMENT THINGS
                float breathaltered = 0.5f + 0.5f * Mathf.Sin(Mathf.Lerp(self.lastBreath, self.breath, timeStacker) * 3.1415927f * 2f);
                Vector2 vector = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker);
                Vector2 vector2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker);
                float num2 = Mathf.InverseLerp(0.3f, 0.5f, Mathf.Abs(Custom.DirVec(vector2, vector).y));
                float hipsrotato = Custom.AimFromOneVectorToAnother(self.head.pos, vector2);
                // initiating animation variables used in body sprites

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
                // internal circle 2

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
                // internal circle 2


                //      (body to hips position) - (camera position) - (player sleep counter * 4f) +
                //          ((1f to 1.3f at the rate of the player aerobic value) * (value for breathing)) *
                //          (1f - (0.3 to 0.5 at the rate of hips to body position drawn, inversed) + rotation of the body + value)



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
                // these add a tad more consistency in the ring placements
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


                Vector2 facev = Vector2.Lerp(self.drawPositions[0, 1], self.drawPositions[0, 0], timeStacker);
                Vector2 facev2 = Vector2.Lerp(self.drawPositions[1, 1], self.drawPositions[1, 0], timeStacker);
                Vector2 facev3 = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
                Vector2 headposition = Vector2.Lerp(self.head.lastPos, self.head.pos, timeStacker);
                float num3 = Custom.AimFromOneVectorToAnother(Vector2.Lerp(facev2, facev, 0.5f), headposition);
                Vector2 vector6 = Vector2.Lerp(self.lastLookDir, self.lookDirection, timeStacker) * 3f * (1f - self.player.sleepCurlUp);

                if (self.player.sleepCurlUp > 0f)
                {
                    sLeaser.sprites[9].scaleX = Mathf.Sign(facev.x - facev2.x);
                    sLeaser.sprites[9].rotation = num3 * (1f - self.player.sleepCurlUp);

                    vector6.y -= 2f * self.player.sleepCurlUp;
                    vector6.x -= 4f * Mathf.Sign(facev.x - facev2.x) * self.player.sleepCurlUp;
                }
                else if (self.owner.room != null && self.owner.EffectiveRoomGravity == 0f)
                {
                    sLeaser.sprites[9].rotation = num3;
                }

                else if (self.player.Consious)
                {
                    if ((self.player.bodyMode == Player.BodyModeIndex.Stand && self.player.input[0].x != 0) ||
                        self.player.bodyMode == Player.BodyModeIndex.Crawl)
                    {
                        if (self.player.bodyMode == Player.BodyModeIndex.Crawl)
                        {
                            sLeaser.sprites[9].scaleX = Mathf.Sign(facev.x - facev2.x);
                        }
                        else
                        {
                            sLeaser.sprites[9].scaleX = ((num3 < 0f) ? -1f : 1f);
                        }
                        vector6.x = 0f;
                        sLeaser.sprites[9].y += 1f;
                    }
                    else
                    {
                        if (Mathf.Abs(vector6.x) < 0.1f)
                        {
                            sLeaser.sprites[9].scaleX = ((num3 < 0f) ? -1f : 1f);
                        }
                        else
                        {
                            sLeaser.sprites[9].scaleX = Mathf.Sign(vector6.x);
                        }
                    }
                    sLeaser.sprites[9].rotation = 0f;
                }
                else
                {
                    sLeaser.sprites[9].rotation = num3;
                }
                if (ModManager.CoopAvailable && self.player.bool1)
                {
                    sLeaser.sprites[9].rotation = num3 + 0.2f;
                }

                sLeaser.sprites[9].x = facev3.x + vector6.x - camPos.x;
                sLeaser.sprites[9].y = facev3.y + vector6.y - 2f - camPos.y;


                // COLOUR THINGS ------------------------------------------------------------------------------------------------------------------------------------------------

                Color saturatedpupil = new Color(0.89f, 0.79f, 0.6f);
                if (self.useJollyColor)
                {
                    saturatedpupil = PlayerGraphics.JollyColor(self.player.playerState.playerNumber, 2);
                }
                else if (PlayerGraphics.customColors != null)
                {
                    saturatedpupil = PlayerGraphics.CustomColorSafety(2);
                }

                if ((saturatedpupil.r >= saturatedpupil.b && saturatedpupil.r >= saturatedpupil.g) ||
                    (saturatedpupil.r == saturatedpupil.b && saturatedpupil.b == saturatedpupil.g && saturatedpupil.r == saturatedpupil.g))
                {
                    saturatedpupil.r = 1f;
                }
                else if (saturatedpupil.b > saturatedpupil.r && saturatedpupil.b >= saturatedpupil.g)
                {
                    saturatedpupil.b = 1f;
                }
                else
                {
                    saturatedpupil.g = 1f;
                }
                sLeaser.sprites[sLeaser.sprites.Length - 9].color = saturatedpupil;
                

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
                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = new Color(0.87f, 0.39f, 0.33f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = new Color(0.87f, 0.39f, 0.33f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.89f, 0.79f, 0.6f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = new Color(0.87f, 0.39f, 0.33f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = new Color(0.87f, 0.39f, 0.33f, 1f);
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

                        sLeaser.sprites[sLeaser.sprites.Length - 1].color = Color.Lerp(new Color(0.87f, 0.39f, 0.33f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 2].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 3].color = Color.Lerp(new Color(0.87f, 0.39f, 0.33f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 4].color = new Color(0.89f, 0.79f, 0.6f, 1f);

                        sLeaser.sprites[sLeaser.sprites.Length - 5].color = Color.Lerp(new Color(0.87f, 0.39f, 0.33f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 6].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                        sLeaser.sprites[sLeaser.sprites.Length - 7].color = Color.Lerp(new Color(0.87f, 0.39f, 0.33f, 1f), new Color(0.89f, 0.79f, 0.6f, 1f), (self.player.GetCat().UnbCyanjumpCountdown / 100f));
                        sLeaser.sprites[sLeaser.sprites.Length - 8].color = new Color(0.89f, 0.79f, 0.6f, 1f);
                    }
                }
            }
        }

        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (self != null && self.player != null && self.player.room != null &&
                self.player.GetCat().IsUnbound)
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

                    if (i == sLeaser.sprites.Length - 1 || i == sLeaser.sprites.Length - 2 || i == sLeaser.sprites.Length - 3 ||
                        i == sLeaser.sprites.Length - 4 || i == sLeaser.sprites.Length - 5 || i == sLeaser.sprites.Length - 6 ||
                        i == sLeaser.sprites.Length - 7 || i == sLeaser.sprites.Length - 8 // || i == sLeaser.sprites.Length
                         )
                    {
                        //0-body, 1-hips, 2-tail, 3-head, 4-legs, 5-left arm, 6-right arm, 7-left hand, 8-right hand, 9-face, 10-glow, 11-pixel/mark
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[4]);
                    }
                    else if (i == sLeaser.sprites.Length - 9)
                    {
                        // pupils
                        rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[i]);
                        (sLeaser.sprites[i]).MoveInFrontOfOtherNode(sLeaser.sprites[9]);
                        // move in front of face sprite
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
            }
            else
            {
                orig(self, sLeaser, rCam, newContatiner);
            }
        }

        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);

            if (self.player.GetCat().IsUnbound)
            {
                Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 9);

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


                sLeaser.sprites[sLeaser.sprites.Length - 9] = new FSprite("Circle20", true);
                sLeaser.sprites[sLeaser.sprites.Length - 9].shader = rCam.game.rainWorld.Shaders["Basic"];
                // pupils

                // DONT FORGET TO RESIZE THE ARRAY
                self.AddToContainer(sLeaser, rCam, null);
            }
        }
        // end unbgraphics
    }
}
