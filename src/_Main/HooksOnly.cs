using System.Linq;

namespace Unbound;
internal class HooksOnly
{
    public static void HookIn()
    {
        // UNB JUMPS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.MovementUpdate += CyanJump.SetupJumps; // base +1f increase
        On.Player.WallJump += CyanJump.SetupWalljumps;
        On.Player.Update += CyanJump.UnboundCyanJumps;

        // UNB MISC -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.Jump += UnbMisc.MadHopsBro; // increases unbounds base jump by 1f
        On.Player.UpdateAnimation += UnbMisc.SwimspeedTweak; // swim speed
        On.GhostWorldPresence.SpawnGhost += UnbMisc.KarmaUnderThreeGhost; // fixes being unable to encounter echos under 5 karma
        On.Player.Grabability += UnbMisc.NoGrab; // prevents grabbing gamma
        On.Player.CanBeSwallowed += UnbMisc.PickyBastard; // cannot swallow items
        On.Centipede.Shock += UnbMisc.ShockResistant; // centishock resistance
        On.ZapCoil.Update += UnbMisc.unbZapped; // zapcoils dont immediately kill
        On.Player.Update += UnbMisc.DamageTracking; // misc damage tracking
        On.OracleSwarmer.BitByPlayer += UnbMisc.noGlow; // glow is not handled via oracle swarmers
        On.LizardAI.IUseARelationshipTracker_UpdateDynamicRelationship += UnbMisc.TreatedAsCyan; // cyan relationship to unbound
        



        GammaAITweaks.Init();
        GammaVisuals.Init();
        TutorialroomKill.Init();
        // gamma
        Pearl.Init();
        PearlConversations.Init();
        PebblesDialogue.Init();
        PebblesCode.Init();
        GeneralOracleThings.Init();
        // iterators
        // STGKTB.Init(); (KEEP DISABLED FOR NOW)
        // room specific
        UnbCatStats.Init();
        // slugcatstats exclusive things



        // SETUP ROOM SPECIFIC -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.ctor += SetupRoomSpecific.Initial;// setup and intro
        On.RegionGate.customKarmaGateRequirements += SetupRoomSpecific.CustomKarmaGates; // custom gate tweaks- allows for exiting MS
        On.AntiGravity.BrokenAntiGravity.Update += SetupRoomSpecific.BrokenUpdate; // antigravity scripts
        On.AntiGravity.BrokenAntiGravity.ctor += SetupRoomSpecific.BrokenAntiGravityctor; // allow for broken gravity in ms
        On.RoomSpecificScript.AddRoomSpecificScript += SetupRoomSpecific.RoomSpecificScripts; // listing all rooms for SRS

        // MORE SLUGCATS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (ModManager.MSC)
        {
            MSCOnly.Init();
        }

        // EXPEDITION -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (ModManager.Expedition)
        {
            On.Expedition.PearlDeliveryChallenge.UpdateDescription += UnbExpedition.Description;
            On.Expedition.PearlDeliveryChallenge.Update += UnbExpedition.Update;
            On.Expedition.NeuronDeliveryChallenge.ValidForThisSlugcat += UnbExpedition.Invalid;
            On.Expedition.ExpeditionGame.ExpeditionRandomStarts += UnbExpedition.UnbRandomStarts;
        }

        // RANDOM BUFF -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        if (ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff"))
        {
            NCRDebug.Log("Random Buffs detected! Not applying Unbound's regular graphics in order to prevent errors. Please disable RB if you want to play using his normal graphics.");
            On.Player.ctor += RandomBuffThings.TailTracking;
        }
        // DRESS MY SLUGCAT -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        else if (ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat"))
        {
            NCRDebug.Log("Unbound is proceeding with DMS graphics hooking");
            DMSUnboundTime.Init();
        }
        // REGULAR UNBOUND GRAPHICS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        else
        {
            On.PlayerGraphics.InitiateSprites += UnbGraphics.InitiateSprites;
            On.PlayerGraphics.AddToContainer += UnbGraphics.AddToContainer;
            On.PlayerGraphics.DrawSprites += UnbGraphics.DrawSprites;
            // STANDARD unbound graphics
        }
    }
}