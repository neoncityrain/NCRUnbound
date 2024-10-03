using Menu;

namespace Unbound
{
    public class UnboundEnums
    {
        public static DataPearl.AbstractDataPearl.DataPearlType unboundKarmaPearl;
        public static List<DataPearl.AbstractDataPearl.DataPearlType> decipheredPearlsUnboundSession;
        // pearls
        public static SlugcatStats.Name NCRTechnician;
        public static SlugcatStats.Name NCRUnbound;
        public static SlugcatStats.Name NCRReverb;
        public static SlugcatStats.Name NCROracle;
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
        public static Region stgRegion;
        public static Region ktbRegion;
        // regions

        public static SlideShow.SlideShowID unboundIntro;
        public static MenuScene.SceneID UNBINTRO0;
        public static MenuScene.SceneID UNBINTRO1;
        public static MenuScene.SceneID UNBINTRO2;
        public static MenuScene.SceneID UNBINTRO3;
        public static MenuScene.SceneID UNBINTRO4;
        public static MenuScene.SceneID UNBINTRO5;
        public static MenuScene.SceneID UNBINTRO6;
        public static MenuScene.SceneID UNBINTRO7;


        public static void RegisterValues()
        {
            unboundKarmaPearl = new DataPearl.AbstractDataPearl.DataPearlType("unboundKarmaPearl", true);
            // pearls
            NCRUnbound = new SlugcatStats.Name("NCRunbound", false);
            NCRTechnician = new SlugcatStats.Name("NCRtech", false);
            NCRReverb = new SlugcatStats.Name("NCRreverb", false);
            NCROracle = new SlugcatStats.Name("NCRoracle", false);
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
            // behaviours
            decipheredPearlsUnboundSession = new List<DataPearl.AbstractDataPearl.DataPearlType>();

            unboundIntro = new SlideShow.SlideShowID("NCRunboundIntro", true);
            UNBINTRO0 = new MenuScene.SceneID("NCRUnbIntro0", true);
            UNBINTRO1 = new MenuScene.SceneID("NCRUnbIntro1", true);
            UNBINTRO2 = new MenuScene.SceneID("NCRUnbIntro2", true);
            UNBINTRO3 = new MenuScene.SceneID("NCRUnbIntro3", true);
            UNBINTRO4 = new MenuScene.SceneID("NCRUnbIntro4", true);
            UNBINTRO5 = new MenuScene.SceneID("NCRUnbIntro5", true);
            UNBINTRO6 = new MenuScene.SceneID("NCRUnbIntro6", true);
            UNBINTRO7 = new MenuScene.SceneID("NCRUnbIntro7", true);
        }

        public static void FullUnregister()
        {
            // for non-items / non-creatures
            if (unbSSThrowFit != null) { unbSSThrowFit.Unregister(); unbSSThrowFit = null; }
            if (unbSlumberConv != null) { unbSlumberConv.Unregister(); unbSlumberConv = null; }
            if (SSMeetUnboundSub != null) { SSMeetUnboundSub.Unregister(); SSMeetUnboundSub = null; }
            if (unbKarmaPearlConv != null) { unbKarmaPearlConv.Unregister(); unbKarmaPearlConv = null; }
            if (UnbSlumberParty != null) { UnbSlumberParty.Unregister(); UnbSlumberParty = null; }
            if (UnbSlumberPartySub != null) { UnbSlumberPartySub.Unregister(); UnbSlumberPartySub = null; }
            if (NCRKTB != null) { NCRKTB.Unregister(); NCRKTB = null; }
            if (NCRSTG != null) { NCRSTG.Unregister(); NCRSTG = null; }

            if (unboundIntro != null) { unboundIntro.Unregister(); unboundIntro = null; }
            if (UNBINTRO0 != null) { UNBINTRO0.Unregister(); UNBINTRO0 = null; }
            if (UNBINTRO1 != null) { UNBINTRO1.Unregister(); UNBINTRO1 = null; }
            if (UNBINTRO2 != null) { UNBINTRO2.Unregister(); UNBINTRO2 = null; }
            if (UNBINTRO3 != null) { UNBINTRO3.Unregister(); UNBINTRO3 = null; }
            if (UNBINTRO4 != null) { UNBINTRO4.Unregister(); UNBINTRO4 = null; }
            if (UNBINTRO5 != null) { UNBINTRO5.Unregister(); UNBINTRO5 = null; }
            if (UNBINTRO6 != null) { UNBINTRO6.Unregister(); UNBINTRO6 = null; }
            if (UNBINTRO7 != null) { UNBINTRO7.Unregister(); UNBINTRO7 = null; }
        }

        public static void ModoffUnregister()
        {
            // for items, creatures, ect that may break the game if unregistered during actual gameplay
            if (unboundKarmaPearl != null) { unboundKarmaPearl.Unregister(); unboundKarmaPearl = null; }
            if (MSCOnly.UnbPebbles != null) { MSCOnly.UnbPebbles.Unregister(); MSCOnly.UnbPebbles = null; }
            if (stgRegion != null) { stgRegion = null; }
        }

        // end enums
    }
}
