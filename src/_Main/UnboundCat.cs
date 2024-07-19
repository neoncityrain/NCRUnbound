using System;

namespace Unbound
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            public UnbJumpsmoke unbsmoke;

            public bool MoreDebug;

            public bool IsUnbound;
            public bool PlayingSound;
            public bool didLongjump;
            public bool CanCyanjump1;
            public bool CanCyanjump2;
            public int UnbChainjumps;
            public int UnbCyanjumpCountdown;
            public UnbScales scalefrill;

            // remix values
            public float CyJump1Maximum;
            public float CyJump2Maximum;
            public bool GraphicsDisabled;
            public bool RingsDisabled;
            public bool Unpicky;

            public UnboundCat(){
                MoreDebug = true;
                // SET TO FALSE WHEN RELEASING

                UnbChainjumps = 0;
                UnbCyanjumpCountdown = 0;

                CyJump1Maximum = 180f;// these are here for reading purposes, not use
                CyJump1Maximum = 400f; // these are here for reading purposes, not use
            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetCat(this Player player) => Unbound.GetValue(player, _ => new());
    }
}