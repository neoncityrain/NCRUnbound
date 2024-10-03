using Menu;
using System.IO;

namespace Unbound
{
    public class Scenes
    {
        public static void Init()
        {
            On.Menu.MenuScene.BuildScene += SetUpCustomScenes;
            On.Menu.SlideShow.ctor += SlideShow_ctor;
        }

        private static void SlideShow_ctor(On.Menu.SlideShow.orig_ctor orig, SlideShow self, ProcessManager manager, SlideShow.SlideShowID slideShowID)
        {
            orig(self, manager, slideShowID);
            if (slideShowID == UnboundEnums.unboundIntro)
            {
                if (manager.musicPlayer != null)
                {
                    self.waitForMusic = "RW_Intro_Theme";
                    self.stall = true;
                    manager.musicPlayer.MenuRequestsSong(self.waitForMusic, 1.5f, 40f);
                }
                // break after every "empty" for better readability
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, 0f, 0f, 0f));

                self.playList.Add(new SlideShow.Scene(UnboundEnums.UNBINTRO0,
                    self.ConvertTime(0, 0, 20), self.ConvertTime(0, 3, 26), self.ConvertTime(0, 8, 6)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, self.ConvertTime(0, 9, 6), 0f, 0f));

                self.playList.Add(new SlideShow.Scene(UnboundEnums.UNBINTRO1,
                    self.ConvertTime(0, 9, 19), self.ConvertTime(0, 10, 19), self.ConvertTime(0, 16, 2)));
                self.playList.Add(new SlideShow.Scene(UnboundEnums.UNBINTRO2,
                    self.ConvertTime(0, 17, 21), self.ConvertTime(0, 18, 10), self.ConvertTime(0, 24, 3)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, self.ConvertTime(0, 34, 6), 0f, 0f));

                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_5_Hunting,
                    self.ConvertTime(0, 35, 50), self.ConvertTime(0, 36, 54), self.ConvertTime(0, 42, 15)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_6_7_Rain_Drop, self.ConvertTime(0, 43, 0), self.ConvertTime(0, 44, 0), self.ConvertTime(0, 49, 29)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_8_Climbing, self.ConvertTime(0, 50, 19), self.ConvertTime(0, 51, 9), self.ConvertTime(0, 55, 21)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Empty, self.ConvertTime(0, 56, 24), 0f, 0f));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_9_Rainy_Climb, self.ConvertTime(0, 57, 2), self.ConvertTime(0, 57, 80), self.ConvertTime(1, 1, 1)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_10_Fall, self.ConvertTime(1, 1, 1), self.ConvertTime(1, 1, 1), self.ConvertTime(1, 4, 0)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_10_5_Separation, self.ConvertTime(1, 4, 29), self.ConvertTime(1, 5, 18), self.ConvertTime(1, 11, 10)));
                self.playList.Add(new SlideShow.Scene(MenuScene.SceneID.Intro_14_Title, self.ConvertTime(1, 27, 24), self.ConvertTime(1, 31, 34), self.ConvertTime(1, 33, 60)));
                for (int i = 1; i < self.playList.Count; i++)
                {
                    self.playList[i].startAt += 0.6f;
                    self.playList[i].fadeInDoneAt += 0.6f;
                    self.playList[i].fadeOutStartAt += 0.6f;
                }
                self.processAfterSlideShow = ProcessManager.ProcessID.Game;
            }
        }

        private static void SetUpCustomScenes(On.Menu.MenuScene.orig_BuildScene orig, MenuScene self)
        {
            SlugcatStats.Name slugcatBeingPlayed = SlugcatStats.Name.White;
            if (self.menu.manager.currentMainLoop is RainWorldGame)
            {
                slugcatBeingPlayed = (self.menu.manager.currentMainLoop as RainWorldGame).StoryCharacter;
            }
            else
            {
                slugcatBeingPlayed = self.menu.manager.rainWorld.progression.PlayingAsSlugcat;
            }

            if (self != null && self.sceneID != null && self.sceneFolder != "" &&
                !(self.sceneID == MenuScene.SceneID.Empty) &&
                (slugcatBeingPlayed == UnboundEnums.NCRUnbound || slugcatBeingPlayed.value == "NCRunbound"))
            {
                if (self is InteractiveMenuScene)
                {
                    (self as InteractiveMenuScene).idleDepths = new List<float>();
                }
                Vector2 vector = new Vector2(0f, 0f);

                if (self.sceneID == MenuScene.SceneID.SleepScreen)
                {
                    NCRDebug.Log("Unbound save! Building his sleep screen. Is Gamma in shelter? " +
                    self.menu.manager.rainWorld.GetNCRModSaveData().IsGammaInMyShelter.ToString());
                    BuildUnboundSleepScreen(self);
                }
                else if (self.sceneID == UnboundEnums.UNBINTRO0 || self.sceneID == UnboundEnums.UNBINTRO1 ||
                    self.sceneID == UnboundEnums.UNBINTRO2 || self.sceneID == UnboundEnums.UNBINTRO3 ||
                    self.sceneID == UnboundEnums.UNBINTRO4 || self.sceneID == UnboundEnums.UNBINTRO5 ||
                    self.sceneID == UnboundEnums.UNBINTRO6 || self.sceneID == UnboundEnums.UNBINTRO7)
                {
                    BuildUnbIntro(self);
                }
                else
                {
                    orig(self); // backup, to prevent crashes when ascending / dying / ect
                }
            }
            else
            {
                orig(self);
            }
        }

        public static void BuildUnbIntro(MenuScene self)
        {
            self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar.ToString() + "unbintro";
            if (self.sceneID == UnboundEnums.UNBINTRO0)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "0", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO1)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "1", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO2)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "2", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO3)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "3", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO4)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "4", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO5)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "5", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO6)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "6", new Vector2(0f, 0f), false, true));
            }
            else if (self.sceneID == UnboundEnums.UNBINTRO7)
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "7", new Vector2(0f, 0f), false, true));
            }
        }

        public static void BuildUnboundSleepScreen(MenuScene self)
        {
            self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar.ToString() + "sleep screen - ncrunbound";
            if (!self.flatMode)
            {
                if (self.menu.manager.rainWorld.GetNCRModSaveData().IsGammaInMyShelter) // gamma is logged as resting in the shelter with unbound
                {
                    NCRDebug.Log("Gamma in Unbound shelter, drawing Gamma!");

                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 6", new Vector2(672f, 236f), 3.5f,
                    MenuDepthIllustration.MenuShader.Normal)); // background symbol on shelter
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 5", new Vector2(674f, 138f), 2.8f,
                        MenuDepthIllustration.MenuShader.Normal)); // background lighter grass
                    self.depthIllustrations[self.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 4", new Vector2(719f, 127f), 2.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // background darker grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 2 (gamma)", new Vector2(830f, 150f), 2.0f, 
                        MenuDepthIllustration.MenuShader.Normal)); // gamma
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 3 (Unbound)", new Vector2(817f, 112f), 1.7f,
                    MenuDepthIllustration.MenuShader.Normal)); // unbound
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 2", new Vector2(817f, 95f), 1.6f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground foreleg grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 1", new Vector2(486f, -54f), 1.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground dark grass

                    (self as InteractiveMenuScene).idleDepths.Add(3.3f); // shelter symbol
                    (self as InteractiveMenuScene).idleDepths.Add(2.7f); // far grass
                    (self as InteractiveMenuScene).idleDepths.Add(2.2f); // close grass
                    (self as InteractiveMenuScene).idleDepths.Add(2.0f); // gamma
                    (self as InteractiveMenuScene).idleDepths.Add(1.7f); // unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.6f); // grass around the front of unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.2f); // dark foregrass
                    return;
                }
                else if (self.menu.manager.rainWorld.GetNCRModSaveData().sweetDream)
                {
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 6", new Vector2(672f, 236f), 3.5f,
                    MenuDepthIllustration.MenuShader.Normal)); // background symbol on shelter
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 5", new Vector2(674f, 138f), 2.8f,
                        MenuDepthIllustration.MenuShader.Normal)); // background lighter grass
                    self.depthIllustrations[self.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 4", new Vector2(719f, 127f), 2.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // background darker grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "unbounddreamsleep1", new Vector2(817f, 112f), 1.7f,
                    MenuDepthIllustration.MenuShader.Normal)); // unbound
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 2", new Vector2(817f, 95f), 1.6f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground foreleg grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 1", new Vector2(486f, -54f), 1.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground dark grass

                    (self as InteractiveMenuScene).idleDepths.Add(3.3f); // shelter symbol
                    (self as InteractiveMenuScene).idleDepths.Add(2.7f); // far grass
                    (self as InteractiveMenuScene).idleDepths.Add(2.2f); // close grass
                    (self as InteractiveMenuScene).idleDepths.Add(2.0f); // gamma
                    (self as InteractiveMenuScene).idleDepths.Add(1.7f); // unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.6f); // grass around the front of unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.2f); // dark foregrass
                    return;
                }
                else
                {
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 6", new Vector2(672f, 236f), 3.5f,
                    MenuDepthIllustration.MenuShader.Normal)); // background symbol on shelter
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 5", new Vector2(674f, 138f), 2.8f,
                        MenuDepthIllustration.MenuShader.Normal)); // background lighter grass
                    self.depthIllustrations[self.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 4", new Vector2(719f, 127f), 2.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // background darker grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 3 (Unbound)", new Vector2(817f, 112f), 1.7f,
                    MenuDepthIllustration.MenuShader.Normal)); // the boy himself
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 2", new Vector2(817f, 95f), 1.6f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground foreleg grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 1", new Vector2(486f, -54f), 1.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // foreground dark grass

                    (self as InteractiveMenuScene).idleDepths.Add(3.3f); // shelter symbol
                    (self as InteractiveMenuScene).idleDepths.Add(2.7f); // far grass
                    (self as InteractiveMenuScene).idleDepths.Add(2.2f); // close grass
                    (self as InteractiveMenuScene).idleDepths.Add(1.7f); // unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.6f); // grass around the front of unbound
                    (self as InteractiveMenuScene).idleDepths.Add(1.2f); // dark foregrass
                    return;
                }
            }
            self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "unbound sleep - flat", new Vector2(683f, 384f), false, true));
        }
    }
}
