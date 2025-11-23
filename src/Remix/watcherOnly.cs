using Watcher;

namespace Unbound
{
    public class watcherOnly
    {
        public static void Init()
        {
            On.Watcher.FireSpriteAI.UpdateDynamicRelationship += unboundRelationship;
            On.Watcher.PearlContent.CanUnderstandDialog += unboundUnderstand;
        }

        private static bool unboundUnderstand(On.Watcher.PearlContent.orig_CanUnderstandDialog orig, PearlContent self)
        {
            if ((Custom.rainWorld.processManager.currentMainLoop as RainWorldGame).session.characterStats.
                name.value == "NCRUnbound")
            {
                return true;
            }
            return orig(self);
        }

        private static CreatureTemplate.Relationship unboundRelationship(On.Watcher.FireSpriteAI.orig_UpdateDynamicRelationship orig, FireSpriteAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            if (self != null && dRelation != null && dRelation.trackerRep != null &&
                dRelation.trackerRep.representedCreature != null &&
                (self.DynamicRelationship(dRelation.trackerRep.representedCreature)).type ==
                CreatureTemplate.Relationship.Type.Afraid &&
                (dRelation.trackerRep.representedCreature.realizedCreature is Player) &&
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsUnbound
                )
            {
                var playerLike = self.creature.world.game.session.creatureCommunities.LikeOfPlayer(
                    self.creature.creatureTemplate.communityID, self.creature.world.RegionNumber, 
                    ((dRelation.trackerRep.representedCreature.realizedCreature as Player).
                    abstractCreature.state as PlayerState).playerNumber);
                return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Ignores, playerLike);
                // firesprites ignore unbound
            }
            return orig(self, dRelation);
        }
    }
}
