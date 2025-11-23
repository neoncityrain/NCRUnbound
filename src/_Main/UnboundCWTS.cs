using System;

namespace Unbound
{
    public static class UnboundCWT
    {
        #region Player.GetUnb
        public class UnboundCat
        {
            #region Graphics
            public UnbJumpsmoke unbsmoke;
            public UnbJumpsmoke damagesmoke;
            // public UnboundWingScales wingscales;
            // the objects added by this mod. jumpsmoke and damaged smoke are considered two different
            // things, despite being functionally the same, allowing both effects to be present at the same time.
            public int TailPatternInt;
            public int UnboundJumpringStartSprite;
            public int GeneralGraphicStartSprite;
            public int UnboundWingscaleStartSprite;
            // since the sprites and spritenumbers are dynamic, this sets the sprites to certain variables
            // that are decided when sprites are generated.

            public Color effectColour; // unbounds current third colour (effect)
            public bool dontForceChangeEffectCol; // lock and prevent the above effect colour from changing
            public bool recheckColour; // "dirty" the colour, forcing it to re-check what it is supposed to be
            public float RGBCounter; // for party mode, to determine what colour is the current one
            #endregion
            #region Modcat Check
            public bool IsNCRUnbModcat;
            // modcat from this mod
            public bool IsTechnician;
            public bool IsUnbound;
            public bool IsOracle;
            public bool IsReverb;
            // self-explanitory. i hope
            #endregion
            #region Movement Checks
            public bool holdingJumpkey;
            // to prevent the sounds from scrambling / bursting ears. unsure if it actually works as intended,
            // so it probably should undergo more testing?
            public bool didLongjump;
            // to check if unbound / tech longjumped, in use for the triple-jump burst
            public bool CanDoubleCyanJump;
            public bool CanTripleCyanJump;
            // self explanitory
            public bool DidTripleCyanJump;
            // prevents triple jumping a bunch in succession. probably
            public int UnbChainjumpsCount;
            // wall-jumping causes a counter to raise up to a certain amount, this tracks that.
            public int UnbCyanjumpCountdown;
            // the current cooldown count
            #endregion
            #region StoryBeats
            public int pebbleskilltries;
            // how many times pebbles tried to kill the player. as this number is allowed to reset,
            // its fine as a CWT that wont save permanently
            #endregion
            #region Remix Values
            public float CyJump1Maximum; // base 180
            public float CyJump2Maximum; // base 400
            public bool Unpicky; // unbound cant normally swallow non-food items. this can change that.
            public bool MoreDebug; // adds more debug logs for... mostly MY use
            public bool RGBRings; // aka party mode. has colours in a rainbow

            public bool GraphicsDisabled; // are ALL graphics disabled?
            public bool RingsDisabled; // are rings disabled?
            public bool WingscalesDisabled; // are wingscales disabled?
            public bool TailDisabled; // are tail graphics disabled?
            #endregion
            #region Pearl Things
            public Watcher.PearlContent pearlBeingRead;
            public DataPearl pearlInPaws;
            #endregion

            public bool LostTail; // for random buffs
            public int CryCooldown; // to prevent reverb from crying constantly

            public UnboundCat()
            {
                TailDisabled = true; // currently keeping this as true, as the tail graphics arent ready.
                // because they are hurting me. help.
                WingscalesDisabled = true; // also disabling wingscales, as theyre being redone.
            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetNCRunbound(this Player player) => Unbound.GetValue(player, _ => new());
        #endregion

        #region AbstractSpear.GetOracleSpear
        public class RotSpear
        {
            public bool IsRotten; // makes sure the spear SHOULD be an oracle spear
            #region Graphics
            public Vector2[,] dangler; // rot-like dangler effect
            public SharedPhysics.TerrainCollisionData scratchTerrainCollisionData; // ?
            public Color goldcol; // the effect colour that should be the gold of the rot
            #endregion

            public RotSpear()
            {

            }
        }

        private static readonly ConditionalWeakTable<AbstractSpear, RotSpear> OracleSpear = new();
        public static RotSpear GetOracleSpear(this AbstractSpear spear) => OracleSpear.GetValue(spear, _ => new());
        #endregion

        #region Overseer.GetGamma
        public class GammaSeer
        {
            public bool RGBMode; // is rgb on?
            public float GammaRGBCounter; // the counter that rgb should be at, if it is on

            public GammaSeer()
            {

            }
        }

        private static readonly ConditionalWeakTable<Overseer, GammaSeer> Gamma = new();
        public static GammaSeer GetGamma(this Overseer overseer) => Gamma.GetValue(overseer, _ => new());
        #endregion

        #region CoralBrain.CoralCircuit.GetNCRCirc
        public class GetCoral
        {
            public bool ShiftToEffectCol; // change the colour of the circuit to the effect colour currently logged
            public Color unboundCopy; // the colour intended to copy unbound

            public GetCoral()
            {

            }
        }

        private static readonly ConditionalWeakTable<CoralBrain.CoralCircuit, GetCoral> thiscircuit = new();
        public static GetCoral GetNCRCirc(this CoralBrain.CoralCircuit circuit) => thiscircuit.GetValue(circuit, _ => new());
        #endregion
        // end cwts
    }
}