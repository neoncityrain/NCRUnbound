

namespace Unbound
{
    public static class FlickMech
    {
        public static void MakeFlickerReal()
        {
            On.LizardAI.IUseARelationshipTracker_UpdateDynamicRelationship += TweakLizardRelationship;
            On.Player.Grabability += Stronk;
        }

        private static Player.ObjectGrabability Stronk(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self != null && obj != null &&
                self.GetNCRunbound().IsOracle)
            {
                if (obj is Snail || obj is LanternMouse || obj is EggBug ||
                    // snail, mouse, eggbug
                    (obj is Cicada && !(obj as Cicada).Charging &&
                    ((obj as Cicada).cantPickUpCounter == 0 || (obj as Cicada).cantPickUpPlayer != self)) ||
                    // squidcada
                    (obj is JetFish && (obj as JetFish).grabable > 0)
                    // jetfish
                    )
                {
                    return Player.ObjectGrabability.OneHand;
                }

                if (obj is Creature && !(obj as Creature).Template.smallCreature && ((obj as Creature).dead ||
                    (SlugcatStats.SlugcatCanMaul(self.SlugCatClass) && self.dontGrabStuff < 1 && obj != self &&
                    !(obj as Creature).Consious)))
                {
                    return Player.ObjectGrabability.BigOneHand; // can grab larger creatures with one hand
                }

                if (ModManager.MSC && obj is Yeek)
                {
                    return Player.ObjectGrabability.OneHand;
                }
                if (ModManager.CoopAvailable && obj is Player)
                {
                    Player player = obj as Player;
                    if (player != null && player != self && !player.standing && !self.isSlugpup)
                    {
                        PlayerState playerState = self.playerState;
                        if (playerState == null || !playerState.isGhost)
                        {
                            return Player.ObjectGrabability.OneHand;
                        }
                    }
                }
            }
            return orig(self, obj);
        }

        private static CreatureTemplate.Relationship TweakLizardRelationship(On.LizardAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, LizardAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            if (self?.creature != null &&
                dRelation?.trackerRep?.representedCreature?.realizedCreature != null && dRelation.state != null &&
                // making sure things arent null
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                ((dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().IsOracle) &&
                // if oracle
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature
                // should still allow making friends with it
                )
            {
                if (self.creature.creatureTemplate.type == CreatureTemplate.Type.RedLizard)
                {
                    if (self.LikeOfPlayer(dRelation.trackerRep) < 0.5f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Attacks, 1f - (self.LikeOfPlayer(dRelation.trackerRep) / 2));
                    }
                    else if (self.LikeOfPlayer(dRelation.trackerRep) < 0.99f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 1f - (self.LikeOfPlayer(dRelation.trackerRep) / 2));
                        // they should be less aggressive the more rep they have with lizards
                    }
                    else // near-perfect rep or higher
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, self.LikeOfPlayer(dRelation.trackerRep));
                    }
                }
                else if (self.creature.creatureTemplate.type == CreatureTemplate.Type.BlueLizard || (ModManager.MSC &&
                    self.creature.creatureTemplate.type == DLCSharedEnums.CreatureTemplateType.ZoopLizard))
                {
                    if (self.LikeOfPlayer(dRelation.trackerRep) < 0.8f)
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Afraid, 1f - (self.LikeOfPlayer(dRelation.trackerRep)));
                        // they should be less afraid the more rep they have with lizards
                    }
                    else // high rep!
                    {
                        return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, self.LikeOfPlayer(dRelation.trackerRep));
                    }
                }
            }
            return orig(self, dRelation);
        }


        // end flicker mechanics
    }
}
