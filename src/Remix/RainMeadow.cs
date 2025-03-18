using RainMeadow;

namespace Unbound.Remix
{
    internal class unbRainMeadow
    {
        public static void Init()
        {
            On.SlugcatStats.ctor += setFoodOnline;

            _ = new Hook(
                typeof(StoryGameMode).GetMethod(nameof(StoryGameMode.LoadWorldAs), BindingFlags.Instance | BindingFlags.Public),
                typeof(unbRainMeadow).GetMethod(nameof(loadWorldAsUnbound), BindingFlags.Static | BindingFlags.NonPublic)
                );
        }

        private static SlugcatStats.Name loadWorldAsUnbound(Func<StoryGameMode, RainWorldGame, SlugcatStats.Name> orig,
            StoryGameMode self, RainWorldGame game)
        {
            if (game?.StoryCharacter != null &&
                (game.StoryCharacter == UnboundEnums.NCRUnbound || game.StoryCharacter.value == "NCRunbound"))
            {
                NCRDebug.Log("NCR Unbound world detected!");
                return UnboundEnums.NCRUnbound;
            }

            return orig(self, game);
        }

        private static void setFoodOnline(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);
            if (slugcat != null && (slugcat == UnboundEnums.NCRUnbound || slugcat.value == "NCRunbound") &&
                RainMeadow.RainMeadow.isStoryMode(out var storyGameMode))
            {
                NCRDebug.Log("Unbound Meadow detected! Fixed Slugstats for Rain Meadow");
                self.maxFood = SlugcatStats.SlugcatFoodMeter(storyGameMode.currentCampaign).x;
                self.foodToHibernate = SlugcatStats.SlugcatFoodMeter(storyGameMode.currentCampaign).y;
            }
        }
    }
}
