namespace Unbound
{
    public class SulliedAI
    {
        public static void Init()
        {
            On.Scavenger.GrabbedObjectSnatched += muenosSnatch;
        }

        private static void muenosSnatch(On.Scavenger.orig_GrabbedObjectSnatched orig, Scavenger self, PhysicalObject grabbedObject, Creature thief)
        {
            if (self?.abstractCreature != null && grabbedObject != null && thief != null &&
                self.abstractCreature.ID.number == 1319192 &&
                thief is Player && ((thief as Player).slugcatStats.name.value == "NCRunbound" ||
                (thief as Player).slugcatStats.name.value == "NCRoracle" || (thief as Player).slugcatStats.name.value == "NCRreverb")
                )
            {
                return;
                // muenos doesnt care if objects are stolen by unbound, oracle, or reverb
            }
            orig(self, grabbedObject, thief);
        }
    }
}
