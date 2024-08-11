using System.Linq;
using static Unbound.GammaVisuals;

namespace Unbound;
internal class HooksOnly
{
    public static void HookIn()
    {
        // UNB JUMPS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.MovementUpdate += CyanJump.SetupJumps; // base +1f increase
        On.Player.WallJump += CyanJump.SetupWalljumps; // walljumping gets faster and more efficient the more it's done
        On.Player.Update += CyanJump.UnboundCyanJumps; // cyan jump code

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

        // SETUP ROOM SPECIFIC -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.ctor += SetupRoomSpecific.Initial;// setup and intro
        On.RegionGate.customKarmaGateRequirements += SetupRoomSpecific.CustomKarmaGates; // custom gate tweaks- allows for exiting MS
        On.AntiGravity.BrokenAntiGravity.Update += SetupRoomSpecific.BrokenUpdate; // antigravity scripts
        On.AntiGravity.BrokenAntiGravity.ctor += SetupRoomSpecific.BrokenAntiGravityctor; // allow for broken gravity in ms
        On.RoomSpecificScript.AddRoomSpecificScript += SetupRoomSpecific.RoomSpecificScripts; // listing all rooms for SRS

        // GAMMA =============================================================================================
        // AI TWEAKS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Overseer.TryAddHologram += GammaAITweaks.HologramTweaks;
        On.OverseerAbstractAI.RoomAllowed += GammaAITweaks.RoomAllowed;
        On.OverseerCommunicationModule.FoodDelicousScore += GammaAITweaks.StopLeadingToFoodUnboundCantEat;
        On.OverseerAbstractAI.HowInterestingIsCreature += GammaAITweaks.InterestInUnbound;
        On.OverseerAI.Update += GammaAITweaks.GammaAIUpdate;
        On.OverseerAI.HoverScoreOfTile += GammaAITweaks.HoverScore;
        On.Overseer.Die += GammaAITweaks.DontRespawnImmediately; // gamma does not revive when killed by non-slugcat creatures

        // VISUALS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.OverseerGraphics.ColorOfSegment += GammaVisuals.GammaColouringSegments;
        On.OverseerGraphics.DrawSprites += GammaVisuals.EyeWhitesForGamma;
        On.OverseerGraphics.DrawSprites -= GammaVisuals.RemoveEyewhites;
        On.OverseerGraphics.InitiateSprites += GammaVisuals.PupilcodeForGamma;
        On.OverseerGraphics.InitiateSprites -= GammaVisuals.RemovePupilcode;
        On.CoralBrain.Mycelium.UpdateColor += GammaVisuals.GammaMycelium;
        Hook ktbmain = new Hook(typeof(global::OverseerGraphics).GetProperty("MainColor", BindingFlags.Instance |
                BindingFlags.Public).GetGetMethod(), new Func<orig_OverseerMainColor,
                OverseerGraphics, Color>(GammaVisuals.GetGammaCol));
        // TUTORIALS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        TutorialroomKill.Init(); // leave this in case it is needed later

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



        
        // MOD EXCLUSIVE =============================================================================================
        // MORE SLUGCATS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        MSCOnly.Init();

        // EXPEDITION -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Expedition.PearlDeliveryChallenge.UpdateDescription += UnbExpedition.Description;
        On.Expedition.PearlDeliveryChallenge.Update += UnbExpedition.Update;
        On.Expedition.NeuronDeliveryChallenge.ValidForThisSlugcat += UnbExpedition.Invalid;
        On.Expedition.ExpeditionGame.ExpeditionRandomStarts += UnbExpedition.UnbRandomStarts;
    }
}