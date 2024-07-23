namespace Unbound
{
    public class UnbName
    {
        public static SlugcatStats.Name NCRTechnician;
        public static SlugcatStats.Name NCRUnbound;

        public static void RegisterValues()
        {
            NCRTechnician = new SlugcatStats.Name("NCRtech", false);
            NCRUnbound = new SlugcatStats.Name("NCRunbound", false);
        }
        public static void UnregisterValues()
        {
            SlugcatStats.Name smiley = NCRTechnician;
            if (smiley != null)
            {
                smiley.Unregister();
            }
            NCRTechnician = null;
        }

        // end names
    }
}
