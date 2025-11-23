using Watcher;

namespace Unbound
{
    public class watcherOnly
    {
        public static void Init()
        {
            On.Watcher.FireSpriteAI.UpdateDynamicRelationship += unboundRelationship;
        }

        private static CreatureTemplate.Relationship unboundRelationship(On.Watcher.FireSpriteAI.orig_UpdateDynamicRelationship orig, FireSpriteAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            return orig(self, dRelation);
        }
    }
}
