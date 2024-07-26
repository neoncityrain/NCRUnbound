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
            public bool holdingJumpkey;
            public bool didLongjump;
            public bool CanDoubleCyanJump;
            public bool CanTripleCyanJump;
            public bool DidTripleCyanJump;
            public int UnbChainjumpsCount;
            public int UnbCyanjumpCountdown;
            public UnbScales scalefrill;

            public int pebbleskilltries;


            // remix values
            public float CyJump1Maximum; // base 180
            public float CyJump2Maximum; // base 400
            public bool GraphicsDisabled;
            public bool RingsDisabled;
            public bool Unpicky;
            public bool MoreDebug;
            public bool WingscalesDisabled;
            // end remix values

            public UnboundCat()
            {

            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetNCRunbound(this Player player) => Unbound.GetValue(player, _ => new());
    }
}