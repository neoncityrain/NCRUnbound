using System;

namespace Unbound
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            public UnbJumpsmoke unbsmoke;
            public UnbJumpsmoke damagesmoke;

            public bool IsTechnician;
            public bool IsUnbound;
            public bool IsOracle;
            public bool IsNCRUnbModcat;
            public bool Reverb;
            public bool holdingJumpkey;
            public bool didLongjump;
            public bool CanDoubleCyanJump;
            public bool CanTripleCyanJump;
            public bool DidTripleCyanJump;
            public int UnbChainjumpsCount;
            public int UnbCyanjumpCountdown;
            public int pebbleskilltries;
            public float RGBCounter;
            public bool dontForceChangeEffectCol;
            public bool recheckColour;

            public bool LostTail; // for random buffs

            public Color effectColour;

            // remix values
            public float CyJump1Maximum; // base 180
            public float CyJump2Maximum; // base 400
            public bool GraphicsDisabled;
            public bool RingsDisabled;
            public bool Unpicky;
            public bool MoreDebug;
            public bool WingscalesDisabled;
            public bool RGBRings;
            // end remix values

            public UnboundCat()
            {

            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetNCRunbound(this Player player) => Unbound.GetValue(player, _ => new());

        public class RotSpear
        {
            public bool IsRotten;
            public Vector2[,] dangler;
            public SharedPhysics.TerrainCollisionData scratchTerrainCollisionData;
            public Color goldcol;

            public RotSpear()
            {

            }
        }

        private static readonly ConditionalWeakTable<AbstractSpear, RotSpear> OracleSpear = new();
        public static RotSpear GetOracleSpear(this AbstractSpear spear) => OracleSpear.GetValue(spear, _ => new());


        public class GammaSeer
        {
            public bool RGBMode;
            public float GammaRGBCounter;

            public GammaSeer()
            {

            }
        }

        private static readonly ConditionalWeakTable<Overseer, GammaSeer> Gamma = new();
        public static GammaSeer GetGamma(this Overseer overseer) => Gamma.GetValue(overseer, _ => new());



        public class GetNCRSave
        {
            public bool IsGammaInMyShelter;
            public bool sweetDream;
            public bool nightmare;

            public GetNCRSave()
            {

            }
        }

        private static readonly ConditionalWeakTable<RainWorld, GetNCRSave> rwgame = new();
        public static GetNCRSave GetNCRModSaveData(this RainWorld game) => rwgame.GetValue(game, _ => new());


        public class GetRoom
        {
            public float atmosphereFloat;

            public GetRoom()
            {

            }
        }

        private static readonly ConditionalWeakTable<Room, GetRoom> thisroom = new();
        public static GetRoom GetNCRRoom(this Room room) => thisroom.GetValue(room, _ => new());


        public class GetCoral
        {
            public bool ShiftToEffectCol;
            public Color unboundCopy;

            public GetCoral()
            {

            }
        }

        private static readonly ConditionalWeakTable<CoralBrain.CoralCircuit, GetCoral> thiscircuit = new();
        public static GetCoral GetNCRCirc(this CoralBrain.CoralCircuit circuit) => thiscircuit.GetValue(circuit, _ => new());
        // end cwts
    }
}