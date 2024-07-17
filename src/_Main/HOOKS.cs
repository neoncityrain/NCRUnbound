using System;
using BepInEx;
using UnityEngine;
using RWCustom;
using MoreSlugcats;
using MonoMod.RuntimeDetour;
using System.Reflection;
using OverseerHolograms;
using Unbound;
using JollyCoop;


namespace Unbound
{
    [BepInPlugin("NCR.theunbound", "unbound", "2.1.5")]
    [BepInDependency("MoreSlugcats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MSC", BepInDependency.DependencyFlags.SoftDependency)]

    class NCRUnbound : BaseUnityPlugin
    {
        public bool IsInit;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += UnbExtras.WrapInit(LoadResources);
        }

        private void LoadResources(RainWorld rainWorld)
        {
            // Futile.atlasManager.LoadImage("");

            if (!IsInit)
            {
                IsInit = true;

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
                _SetupRoomSpecific.Init();
                // room specific
            }
        }
    }
}