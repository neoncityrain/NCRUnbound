using VoidSea;

namespace Unbound
{
    public static class EndUnb
    {
        public static void Init()
        {
            On.VoidSea.PlayerGhosts.AddGhost += AddUnbGhost;
            On.VoidSea.VoidWorm.MainWormBehavior.Update += fuckYouUnbound;
        }

        private static void fuckYouUnbound(On.VoidSea.VoidWorm.MainWormBehavior.orig_Update orig, VoidWorm.MainWormBehavior self)
        {
            if (self?.worm?.room?.world?.game?.session?.characterStats != null &&
                self.worm.room.world.game.session.characterStats.name.value == "NCRunbound" &&
                self.voidSea?.room?.game?.FirstRealizedPlayer != null)
            {
                try
                {
                    if (self.phase == VoidWorm.MainWormBehavior.Phase.Looking && self.timeInPhase > 570)
                    {
                        self.SwitchPhase(VoidWorm.MainWormBehavior.Phase.SwimDown);
                        return;
                    }

                    if (self.phase == VoidWorm.MainWormBehavior.Phase.SwimDown)
                    {
                        for (int l = 0; l < self.voidSea.worms.Count; l++)
                        {
                            self.voidSea.worms[l].lightAlpha = Mathf.Max(0f, self.voidSea.worms[l].lightAlpha - 0.001f);
                        }
                        for (int m = 0; m < self.voidSea.elements.Count; m++)
                        {
                            if (self.voidSea.elements[m] is DistantWormLight)
                            {
                                (self.voidSea.elements[m] as DistantWormLight).alpha = Mathf.Max(0f,
                                    (self.voidSea.elements[m] as DistantWormLight).alpha - 0.001f);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Hubert being mean to Unbound: " + e);
                }
            }
            orig(self);
        }

        private static void AddUnbGhost(On.VoidSea.PlayerGhosts.orig_AddGhost orig, PlayerGhosts self)
        {
            if (self?.originalPlayer?.slugcatStats?.name?.value == "NCRunbound")
            {
                try
                {
                    Vector2 pos = self.originalPlayer.mainBodyChunk.pos + Custom.RNV() * 2000f;
                    // sets random location for the ghost

                    AbstractCreature abstractCreature = new AbstractCreature(self.voidSea.room.world,
                        StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat),
                        null, self.voidSea.room.GetWorldCoordinate(pos), new EntityID(-1, -1));

                    abstractCreature.state = new PlayerState(abstractCreature, self.originalPlayer.playerState.playerNumber,
                            SlugcatStats.Name.White, true); // force the voidsea cats to be survivor, rather than unbound
                    (abstractCreature.state as PlayerState).isGhost = true;

                    PlayerState ghoststate = abstractCreature.state as PlayerState;
                    if (ghoststate.isPup)
                    {
                        ghoststate.isPup = false; // ghosts should NEVER be pups, even if player is
                    }

                    self.voidSea.room.abstractRoom.AddEntity(abstractCreature);
                    abstractCreature.RealizeInRoom();
                    for (int i = 0; i < abstractCreature.realizedCreature.bodyChunks.Length; i++)
                    {
                        abstractCreature.realizedCreature.bodyChunks[i].restrictInRoomRange = float.MaxValue;
                    }

                    abstractCreature.realizedCreature.CollideWithTerrain = false;
                    self.ghosts.Add(new PlayerGhosts.Ghost(self, abstractCreature.realizedCreature as Player));
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Void sea error: " + e);
                }
            }
            else
            {
                orig(self);
            }
        }
    }
}
