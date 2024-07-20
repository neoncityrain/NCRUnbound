using System;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Unbound
{
    [BepInPlugin("NCR.theunbound", "unbound", "2.2.1")]
    [BepInDependency("MoreSlugcats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MSC", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("pushtomeow", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("fakeachievements", BepInDependency.DependencyFlags.SoftDependency)]

    public partial class NCRUnbound : BaseUnityPlugin
    {
        public bool IsInit;
        private UnbRemInterface UnbOptions;
        
        public void OnEnable()
        {
            On.Player.ctor += PlayerOnctor;
            // remix triggers

            On.RainWorld.OnModsInit += UnbExtras.WrapInit(LoadResources);
        }

        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");
            try
            {
                if (IsInit) return;

                UnbMisc.Init();
                UnbGraphics.Init();
                // _Main
                CyanJump.Init();
                // cyanjumps
                GammaAI.Init();
                GammaVisuals.Init();
                TutorialroomKill.Init();
                // gamma
                Pearl.Init();
                PearlConversations.Init();
                PebblesConversations.Init();
                // iterators
                SetupRoomSpecific.Init();
                // room specific

                On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                On.GameSession.ctor += GameSessionOnctor;
                // cleanup

                MachineConnector.SetRegisteredOI("NCR.theunbound", UnbOptions);
                IsInit = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }

        }

        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            ClearMemory();
        }

        private void ClearMemory()
        {
            // clear collections here
            
        }


        public NCRUnbound()
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