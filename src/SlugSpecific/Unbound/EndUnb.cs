using VoidSea;

namespace Unbound
{
    public static class EndUnb
    {
        public static void Init()
        {
            On.VoidSea.PlayerGhosts.AddGhost += AddUnbGhost;
        }

        private static void AddUnbGhost(On.VoidSea.PlayerGhosts.orig_AddGhost orig, PlayerGhosts self)
        {
            if (self.originalPlayer.slugcatStats.name.value == "NCRunbound")
            {
                Vector2 pos = self.originalPlayer.mainBodyChunk.pos + Custom.RNV() * 2000f;
                AbstractCreature abstractCreature = new AbstractCreature(self.voidSea.room.world, 
                    StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat), 
                    null, self.voidSea.room.GetWorldCoordinate(pos), new EntityID(-1, -1));
                abstractCreature.state = new PlayerState(abstractCreature, self.originalPlayer.playerState.playerNumber,
                    UnboundEnums.NCRTechnician, true);
                self.voidSea.room.abstractRoom.AddEntity(abstractCreature);
                abstractCreature.RealizeInRoom();
                for (int i = 0; i < abstractCreature.realizedCreature.bodyChunks.Length; i++)
                {
                    abstractCreature.realizedCreature.bodyChunks[i].restrictInRoomRange = float.MaxValue;
                }
                abstractCreature.realizedCreature.CollideWithTerrain = false;
                self.ghosts.Add(new PlayerGhosts.Ghost(self, abstractCreature.realizedCreature as Player));
            }
            else
            {
                orig(self);
            }
        }
    }
}
