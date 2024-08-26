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
    }
}