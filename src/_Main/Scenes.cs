using Menu;
using System.IO;

namespace Unbound
{
    public class Scenes
    {
        public static void Init()
        {
            On.Menu.MenuScene.BuildScene += SetUpCustomScenes;
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

        public static void BuildUnboundSleepScreen(MenuScene self)
        {
            self.sceneFolder = "Scenes" + Path.DirectorySeparatorChar.ToString() + "sleep screen - ncrunbound";
            if (!self.flatMode)
            {
                if (self.menu.manager.rainWorld.GetNCRModSaveData().IsGammaInMyShelter)
                    // gamma is logged as resting in the shelter with unbound
                {
                    NCRDebug.Log("Gamma in Unbound shelter, drawing Gamma!");

                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 6", new Vector2(672f, 236f), 3.5f,
                    MenuDepthIllustration.MenuShader.Normal)); // background symbol on shelter
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 5", new Vector2(674f, 138f), 2.8f,
                        MenuDepthIllustration.MenuShader.Normal)); // background lighter grass
                    self.depthIllustrations[self.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 4", new Vector2(719f, 127f), 2.2f,
                        MenuDepthIllustration.MenuShader.Normal)); // background darker grass
                    self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "sleep - 2 (Gamma)", new Vector2(830f, 150f), 2.0f, 
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
                    MenuDepthIllustration.MenuShader.Normal)); // unbound but good dreams
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
            else // flatmode active
            {
                self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "unbound sleep - flat",
                    new Vector2(683f, 384f), false, true));
            }
        }
    }
}
