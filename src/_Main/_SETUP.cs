using System.Linq;

namespace Unbound
{
    [BepInPlugin("NCR.theunbound", "unbound", "3.0.0")]

    [BepInDependency("moreslugcats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("watcher", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("pushtomeow", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("randombuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("expedition", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("DetailedIcon", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("henpemaz.rainmeadow", BepInDependency.DependencyFlags.SoftDependency)]


    public partial class UnbSetupThings : BaseUnityPlugin
    {
        public bool InitialCommit;
        public bool SecondaryCommit;
        private readonly UnbRemInterface UnbOptions;

        public void OnEnable()
        {
            On.Player.ctor += PlayerRemix;
            On.Overseer.ctor += OverseerRemix;
            // remix triggers, makes sure all values effect something else in the code

            HooksOnly.HookIn(); // redirecting to the main hooks (HooksOnly)

            On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
            On.GameSession.ctor += GameSessionOnctor;
            // clean up enums

            On.RainWorld.PostModsInit += CheckOnMods;  // check for other mods, in the cases of dms or similar
            On.RainWorld.OnModsInit += UnbExtras.WrapInit(LoadResources); // load resources, such as graphics
        }

        private void CheckOnMods(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            if (!SecondaryCommit)
            {
                SecondaryCommit = true;

                if (ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat"))
                {
                    NCRDebug.Log("DMS detected! All Unbound graphics have been disabled. (For now!)");
                }
                else
                {
                    UnbGraphics.Init(); // initialise graphics
                    UnbGraphics.GraphicsHooks(); // non-DMS exclusive hooks
                }
            }
            orig(self);
        }

        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
            // ^ the above is for loading any images within the game, usually used in _UnbGraphics.
            try
            {
                if (!InitialCommit)
                {
                    UnboundEnums.RegisterValues(); // registers all enums linked to unbound
                    MachineConnector.SetRegisteredOI("NCR.theunbound", UnbOptions); // register remix menu

                    Futile.atlasManager.LoadAtlas("atlases/icons/Kill_Slugcat_NCRunbound"); // for detailed icons
                    Futile.atlasManager.LoadAtlas("atlases/icons/Multiplayer_Death_NCRunbound"); // as above

                    InitialCommit = true;
                }
            }
            catch (Exception e)
            {
                NCRDebug.Log("Error loading resources: " + e);
                throw;
            }

        }

        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            UnboundEnums.FullUnregister(); // unregisters all existing enums when the game shuts down
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            UnboundEnums.FullUnregister(); // as above
        }

        public UnbSetupThings()
        {
            try
            {
                UnbOptions = new UnbRemInterface(this, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                NCRDebug.Log("Error with Unbound's remix interface: " + ex);
                throw;
            }
        }
    }
}