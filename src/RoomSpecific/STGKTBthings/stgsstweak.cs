using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbound
{
    public class stgsstweak
    {
        public static void Init()
        {
            On.SSOracleBehavior.NewAction += newAction;
        }

        private static void newAction(On.SSOracleBehavior.orig_NewAction orig, SSOracleBehavior self, SSOracleBehavior.Action nextAction)
        {
            if (self?.oracle?.ID != null && self.oracle.ID == UnboundEnums.NCRSTG)
            {

            }
            orig(self, nextAction);
        }
    }
}
