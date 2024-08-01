namespace Unbound
{
    public class UnboundEnums
    {
        public static DataPearl.AbstractDataPearl.DataPearlType unboundKarmaPearl;
        public static List<DataPearl.AbstractDataPearl.DataPearlType> decipheredPearlsUnboundSession;
        // pearls
        public static SlugcatStats.Name NCRTechnician;
        public static SlugcatStats.Name NCRUnbound;
        // slugcats
        public static Conversation.ID unbKarmaPearlConv;
        public static Conversation.ID unbSlumberConv;
        public static Conversation.ID unbSSThrowFit;
        // conversations
        public static SSOracleBehavior.Action UnbSlumberParty;
        // pebbsactions
        public static SSOracleBehavior.SubBehavior.SubBehavID UnbSlumberPartySub;
        public static SSOracleBehavior.SubBehavior.SubBehavID SSMeetUnboundSub;
        // pebbssubbehaviors
        public static Oracle.OracleID NCRKTB;
        public static Oracle.OracleID NCRSTG;
        // oracles

        public static void RegisterValues()
        {
            unboundKarmaPearl = new DataPearl.AbstractDataPearl.DataPearlType("unboundKarmaPearl", true);
            // pearls
            NCRUnbound = new SlugcatStats.Name("NCRunbound", false);
            NCRTechnician = new SlugcatStats.Name("NCRtech", false);
            // slugcats
            NCRKTB = new("NCRKTB", true);
            NCRSTG = new("NCRSTG", true);
            // iterators
            unbKarmaPearlConv = new Conversation.ID("unbKarmaPearlConv", true);
            unbSlumberConv = new Conversation.ID("unbSlumberConv", true);
            unbSSThrowFit = new Conversation.ID("unbSSThrowFit", true);
            // conversations
            UnbSlumberParty = new SSOracleBehavior.Action("UnbSlumberParty", true);
            UnbSlumberPartySub = new SSOracleBehavior.SubBehavior.SubBehavID("UnbSlumberPartySub", true);
            SSMeetUnboundSub = new SSOracleBehavior.SubBehavior.SubBehavID("SSMeetUnboundSub", true);
        }

        public static void FullUnregister()
        {
            // for non-items / non-creatures
            if (unbSSThrowFit != null) { unbSSThrowFit.Unregister(); unbSSThrowFit = null; }
            if (unbSlumberConv != null) { unbSlumberConv.Unregister(); unbSlumberConv = null; }
            if (SSMeetUnboundSub != null) { SSMeetUnboundSub.Unregister(); SSMeetUnboundSub = null; }
            if (unbKarmaPearlConv != null) { unbKarmaPearlConv.Unregister(); unbKarmaPearlConv = null; }
            if (NCRTechnician != null) { NCRTechnician.Unregister(); NCRTechnician = null; }
            if (NCRUnbound != null) { NCRUnbound.Unregister(); NCRUnbound = null; }
            if (UnbSlumberParty != null) { UnbSlumberParty.Unregister(); UnbSlumberParty = null; }
            if (UnbSlumberPartySub != null) { UnbSlumberPartySub.Unregister(); UnbSlumberPartySub = null; }
            if (NCRKTB != null) { NCRKTB.Unregister(); NCRKTB = null; }
            if (NCRSTG != null) { NCRSTG.Unregister(); NCRSTG = null; }
        }

        public static void ModoffUnregister()
        {
            // for items, creatures, ect that may break the game if unregistered during actual gameplay
            if (unboundKarmaPearl != null) { unboundKarmaPearl.Unregister(); unboundKarmaPearl = null; }
            if (MSCOnly.UnbPebbles != null) { MSCOnly.UnbPebbles.Unregister(); MSCOnly.UnbPebbles = null; }
        }

        // end enums
    }
}
