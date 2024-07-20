using System;

namespace Unbound
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            public UnbJumpsmoke unbsmoke;


            public bool IsUnbound;
            public bool holdingJumpkey;
            public bool didLongjump;
            public bool CanDoubleCyanJump;
            public bool CanTripleCyanJump;
            public int UnbChainjumpsCount;
            public int UnbCyanjumpCountdown;
            public UnbScales scalefrill;


            // remix values
            public float CyJump1Maximum; // the maximum amount of time a double cyan jump takes to recharge, used in remix
            public float CyJump2Maximum; // the maximum amount of time a triple cyan jump takes to recharge, used in remix
            public bool GraphicsDisabled;
            public bool RingsDisabled;
            public bool Unpicky;
            public bool MoreDebug;
            // end remix values

            public UnboundCat()
            {
                CyJump1Maximum = 180f;// these are here for reading purposes, not use
                CyJump1Maximum = 400f; // these are here for reading purposes, not use
            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetNCRunbound(this Player player) => Unbound.GetValue(player, _ => new());
    }
}