namespace Unbound
{
    public class UnboundEnums
    {
        public static DataPearl.AbstractDataPearl.DataPearlType unboundKarmaPearl;
        // pearls
        public static SlugcatStats.Name NCRTechnician;
        public static SlugcatStats.Name NCRUnbound;
        // slugcats
        public static Conversation.ID unbKarmaPearlConv;
        // conversations
        public static SSOracleBehavior.Action UnbSlumberParty;
        // pebbsactions
        public static SSOracleBehavior.SubBehavior.SubBehavID UnbSlumberPartySub;
        public static SSOracleBehavior.SubBehavior.SubBehavID SSMeetUnboundSub;
        // pebbssubbehaviors

        public static void RegisterValues()
        {
            unboundKarmaPearl = new DataPearl.AbstractDataPearl.DataPearlType("unboundKarmaPearl", true);
            // pearls
            NCRUnbound = new SlugcatStats.Name("NCRunbound", false);
            NCRTechnician = new SlugcatStats.Name("NCRtech", false);
            // slugcats
            unbKarmaPearlConv = new Conversation.ID("unbKarmaPearlConv", true);
            // conversations
            UnbSlumberParty = new SSOracleBehavior.Action("UnbSlumberParty", true);
            UnbSlumberPartySub = new SSOracleBehavior.SubBehavior.SubBehavID("UnbSlumberPartySub", true);
            SSMeetUnboundSub = new SSOracleBehavior.SubBehavior.SubBehavID("SSMeetUnboundSub", true);
        }

        public static void UnregisterValues()
        {
            if (SSMeetUnboundSub != null) { SSMeetUnboundSub.Unregister(); SSMeetUnboundSub = null; }
            if (unbKarmaPearlConv != null) { unbKarmaPearlConv.Unregister(); unbKarmaPearlConv = null; }
            if (NCRTechnician != null) { NCRTechnician.Unregister(); NCRTechnician = null; }
            if (NCRUnbound != null) { NCRUnbound.Unregister(); NCRUnbound = null; }
            if (UnbSlumberParty != null) { UnbSlumberParty.Unregister(); UnbSlumberParty = null; }
            if (UnbSlumberPartySub != null) { UnbSlumberPartySub.Unregister(); UnbSlumberPartySub = null; }
        }

        // end names
    }
}
