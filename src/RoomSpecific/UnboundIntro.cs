using MoreSlugcats;

namespace Unbound
{
    internal class UnboundIntro : UpdatableAndDeletable
    {
        int unboundstarttimer;

        public UnboundIntro()
        {
        }

        public void CameraSetup()
        {
            room.game.cameras[0].MoveCamera(2);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.BigEel, false);
            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.Leech, false);
            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.SeaLeech, false);
            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, false);
            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.Vulture, false);
            room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.KingVulture, false);

            if (room.game.AllPlayersRealized)
            {

                for (int i = 0; i < room.game.Players.Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        (room.game.Players[i].realizedCreature as Player).bodyChunks[j].HardSetPosition(room.MiddleOfTile(142, 72));
                        (room.game.Players[i].realizedCreature as Player).standing = false;
                    }
                }


                if (unboundstarttimer == 0)
                {
                    CameraSetup();
                    (room.game.Players[0].realizedCreature as Player).objectInStomach =
                        new DataPearl.AbstractDataPearl(room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl,
                        null, new WorldCoordinate(room.abstractRoom.index, -1, -1, 0), room.game.GetNewID(), -1, -1, null,
                        Pearl.unboundKarmaPearl);
                }
                if (unboundstarttimer < 290)
                {
                    unboundstarttimer++;
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        (room.game.Players[i].realizedCreature as Player).airInLungs = 0.01f;
                        (room.game.Players[i].realizedCreature as Player).stun = 100;
                    }
                }

                if (unboundstarttimer == 290)
                {
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        (room.game.Players[i].realizedCreature as Player).stun = 0;
                    }

                    Debug.Log("Start of game initiated, yippee!");
                    Destroy();
                }
            }
            else
            {
                Debug.Log("Player not realized! Sorry for dropping you into water to drown, I twied :(");
            }
        }
    }
}