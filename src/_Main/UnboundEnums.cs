namespace Unbound
{
    public class UnboundEnums
    {
        public static SlugcatStats.Name NCRTechnician;
        public static SlugcatStats.Name NCRUnbound;
        public static SSOracleBehavior.Action UnboundPebblesSlumberParty;
        public static SSOracleBehavior.SubBehavior.SubBehavID UnbSlumberParty;
        public static Conversation.ID unbKarmaPearlConv;
        public static DataPearl.AbstractDataPearl.DataPearlType unboundKarmaPearl;
        public static Conversation.ID UnboundSleepoverConversation;

        public static void RegisterValues()
        {
            NCRUnbound = new("NCRunbound", false);
            NCRTechnician = new ("NCRtech", false);
            UnboundPebblesSlumberParty = new ("UnboundPebblesSlumberParty", true);
            UnbSlumberParty = new ("UnbSlumberParty", true);
            unbKarmaPearlConv = new ("unbKarmaPearlConv", true);
            unboundKarmaPearl = new("unboundKarmaPearl", true);
            UnboundSleepoverConversation = new("UnboundSleepoverConversation", true);
        }
        public static void UnregisterValues()
        {
            Conversation.ID sleepoverconvo = UnboundSleepoverConversation;
            if (sleepoverconvo != null) { sleepoverconvo.Unregister(); }
            UnboundSleepoverConversation = null;

            DataPearl.AbstractDataPearl.DataPearlType unbpearl = unboundKarmaPearl;
            if (unbpearl != null) { unbpearl.Unregister(); }
            unboundKarmaPearl = null;

            Conversation.ID karmaconvo = unbKarmaPearlConv;
            if (karmaconvo != null) { karmaconvo.Unregister(); }
            unbKarmaPearlConv = null;

            SlugcatStats.Name smiley = NCRTechnician;
            if (smiley != null) { smiley.Unregister(); }
            NCRTechnician = null;

            SlugcatStats.Name unb = NCRUnbound;
            if (unb != null) {  unb.Unregister(); }
            NCRUnbound = null;

            SSOracleBehavior.Action partysover = UnboundPebblesSlumberParty;
            if (partysover != null) { partysover.Unregister(); }
            UnboundPebblesSlumberParty = null;

            SSOracleBehavior.SubBehavior.SubBehavID noparty = UnbSlumberParty;
            if (noparty != null) { noparty.Unregister(); }
            UnbSlumberParty = null;
        }

        // end names
    }
}
