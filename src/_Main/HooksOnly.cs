using System.Linq;
using static Unbound.GammaVisuals;

namespace Unbound;
internal class HooksOnly
{
    public static void HookIn()
    {
        // UNB MOVEMENT -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.MovementUpdate += UnbMovement.SetupJumps; // base +1f increase
        On.Player.WallJump += UnbMovement.SetupWalljumps; // walljumping gets faster and more efficient the more it's done
        On.Player.Update += UnbMovement.UnboundCyanJumps; // cyan jump code
        //On.Player.TerrainImpact += UnbMovement.UnbImpact;

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
        On.Lizard.DamageAttack += UnbMisc.BiteUnb; // cyan bites do damage rather than autostun
        On.PlayerGraphics.Update += UnbMisc.UpdateTheGlow; // glow can change colour
        On.SSOracleSwarmer.DrawSprites += UnbMisc.NeuronColourShift; // when in a room with a neuron, rings change to be the neuron colour
        On.JellyFish.Collide += UnbMisc.shockMeLess; // reduces stun time upon jellyfish hit

        // SETUP ROOM SPECIFIC -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Player.ctor += SetupRoomSpecific.UnboundFirstBootup;// setup and intro
        On.RegionGate.customKarmaGateRequirements += SetupRoomSpecific.CustomKarmaGates; // custom gate tweaks- allows for exiting MS
        On.AntiGravity.BrokenAntiGravity.Update += SetupRoomSpecific.BrokenGravUpdate; // antigravity scripts
        On.AntiGravity.BrokenAntiGravity.ctor += SetupRoomSpecific.BrokenAntiGravityctor; // allow for broken gravity in ms
        On.AntiGravity.BrokenAntiGravity.Update += SetupRoomSpecific.UpdateGravity; // as above
        On.Player.ctor += SetupRoomSpecific.MaintainRoomSpecific; // continues to seek room specific code
        On.RoomSpecificScript.AddRoomSpecificScript += SetupRoomSpecific.KTBIntroSetup; // ktb room intro
        // On.Player.Update += SetupRoomSpecific.IsGammaInMyShelter;

        // GAMMA =============================================================================================
        // AI TWEAKS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        On.Overseer.TryAddHologram += GammaAITweaks.HologramTweaks; // changes the holograms used by gamma + priority
        On.OverseerAbstractAI.RoomAllowed += GammaAITweaks.RoomAllowed; // allows gamma into shelters
        On.OverseerCommunicationModule.FoodDelicousScore += GammaAITweaks.StopLeadingToFoodUnboundCantEat; // notes unbounds diet, rather than leading him to anything
        On.OverseerAbstractAI.HowInterestingIsCreature += GammaAITweaks.InterestInUnbound; // unbound is always interesting
        // On.OverseerAI.Update += GammaAITweaks.GammaAIUpdate; // update gamma ai
        On.OverseerAI.HoverScoreOfTile += GammaAITweaks.HoverScore; // changes gammas hoverscore on tiles
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
        On.Overseer.Update += RBGUpdate;

        // TUTORIALS -----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // TutorialroomKill.Init(); // leave this in case it is needed later

        // BLOODMOON =============================================================================================
        Tmsbb.Init();

        // ECT =============================================================================================
        Scenes.Init();
        // gamma
        Pearl.Init();
        PearlConversations.Init();
        PebblesDialogue.Init();
        PebblesCode.Init();
        GeneralOracleThings.Init();
        // iterators
        STGKTB.Init();
        // room specific
        Revgen.HookThatThang();
        FlickMech.MakeFlickerReal();
        FlickSpearMechanics.Init();
        UnbCatStats.Init();
        EndUnb.Init();
        Techgen.Init();
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