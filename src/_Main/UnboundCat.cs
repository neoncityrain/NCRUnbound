using System.Runtime.CompilerServices;

namespace Unbound
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            public UnbJumpsmoke unbsmoke;

            public bool IsUnbound;
            public bool PlayingSound;
            public bool didLongjump;
            public bool CanCyanjump1;
            public bool CanCyanjump2;
            public int UnbChainjumps;
            public int UnbCyanjumpCountdown;

            // remix values
            public float CyJump1Maximum;
            public float CyJump2Maximum;
            public bool GraphicsDisabled;
            public bool RingsDisabled;
            public bool Unpicky;

            public UnboundCat(){
                UnbChainjumps = 0;
                UnbCyanjumpCountdown = 0;

                CyJump1Maximum = 180f;
                CyJump1Maximum = 400f;
            }
        }

        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetCat(this Player player) => Unbound.GetValue(player, _ => new());
    }
}