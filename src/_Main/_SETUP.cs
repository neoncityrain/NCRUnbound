using System.Linq;

namespace Unbound
{
    [BepInPlugin("NCR.theunbound", "unbound", "2.3.0")]

    [BepInDependency("moreslugcats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("pushtomeow", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("fakeachievements", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Pupbase", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("randombuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("expedition", BepInDependency.DependencyFlags.SoftDependency)]


    public partial class UnbSetupThings : BaseUnityPlugin
    {
        public bool InitialCommit;
        public bool SecondaryCommit;
        private readonly UnbRemInterface UnbOptions;

        public void OnEnable()
        {
            On.Player.ctor += PlayerOnctor;
            // remix triggers

            HooksOnly.HookIn();

            On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
            On.GameSession.ctor += GameSessionOnctor;
            // cleanup

            On.RainWorld.OnModsInit += UnbExtras.WrapInit(LoadResources);
        }

        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
            try
            {
                if (!InitialCommit)
                {
                    UnboundEnums.RegisterValues();


                    if (!(ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff") &&
                        !(ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat"))))
                    {
                        NCRDebug.Log("DMS and Random Buffs not enabled, proceeding to load normal Unbound graphics");
                        UnbGraphics.Init();

                        On.PlayerGraphics.InitiateSprites += UnbGraphics.InitiateSprites;
                        On.PlayerGraphics.AddToContainer += UnbGraphics.AddToContainer;
                        On.PlayerGraphics.DrawSprites += UnbGraphics.DrawSprites;
                    }
                    else if ((ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat")) &&
                        !(ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff")))
                    {
                        NCRDebug.Log("DMS enabled, proceeding to load DMS Unbound graphics");
                        DMSUnboundTime.Init();

                        On.PlayerGraphics.InitiateSprites += DMSUnboundTime.InitiateSprites;
                        On.PlayerGraphics.AddToContainer += DMSUnboundTime.AddToContainer;
                        On.PlayerGraphics.DrawSprites += DMSUnboundTime.DrawSprites;
                        On.PlayerGraphics.ApplyPalette += DMSUnboundTime.Coloor;
                    }
                    else
                    {
                        NCRDebug.Log("Random Buffs enabled, disabling Unbound graphics");
                        On.Player.ctor += RandomBuffThings.TailTracking;
                    }


                    MachineConnector.SetRegisteredOI("NCR.theunbound", UnbOptions);
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
            UnboundEnums.FullUnregister();
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            UnboundEnums.FullUnregister();
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
                throw;
            }
        }
    }
}