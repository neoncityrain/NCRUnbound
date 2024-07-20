using MoreSlugcats;

namespace Unbound
{
    internal class UnboundIntro : UpdatableAndDeletable
    {
        int unboundstarttimer;

        public UnboundIntro()
        {

        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            try
            {
                room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.BigEel, false);
                room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.Vulture, false);
                room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.KingVulture, false);
            }
            catch (Exception e)
            {
                Debug.Log("Could not ban creatures from the cutscene room! Error code: " + e);
            }
            

            if (room.game.AllPlayersRealized)
            {
                if (unboundstarttimer == 1)
                {
                    try
                    {
                        (room.game.Players[0].realizedCreature as Player).objectInStomach =
                        new DataPearl.AbstractDataPearl(room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl,
                        null, new WorldCoordinate(room.abstractRoom.index, 1, 1, 0), room.game.GetNewID(), -1, -1, null,
                        Pearl.unboundKarmaPearl);
                        // first player only
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Could not spawn unbound pearl! Error code: " + e);
                    }
                    

                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        try
                        {
                            if (i == 0)
                            {
                                // player 1
                                (room.game.Players[i].realizedCreature as Player).standing = false;
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(141, 72));
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(142, 72));
                                // this should force the player to be facing the left. but sometimes it doesnt so idfk
                            }
                            else if (i == 1)
                            {
                                // player 2
                                (room.game.Players[i].realizedCreature as Player).standing = false;
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(134, 73));
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(133, 73));
                            }
                            else if (i == 2)
                            {
                                // player 3
                                (room.game.Players[i].realizedCreature as Player).standing = false;
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(128, 74));
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(129, 74));
                            }
                            else
                            {
                                Debug.Log("Ya got a lot of players there, eh? I'm flattered! Have fun, gang :]");
                                (room.game.Players[i].realizedCreature as Player).standing = false;
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(112, 72));
                                (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(113, 72));
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log("Error triggered: " + e);
                        }
                    }
                }

                if (unboundstarttimer < 290)
                {
                    unboundstarttimer++;
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        (room.game.Players[i].realizedCreature as Player).airInLungs = 0.01f;
                        (room.game.Players[i].realizedCreature as Player).stun = 100;
                        // forces all players to remain stunned and without any breath until the start timer ends
                    }
                }

                if (unboundstarttimer == 290)
                {
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        (room.game.Players[i].realizedCreature as Player).stun = 0;
                        // frees players from stunlock
                    }

                    Debug.Log("Start of game initiated, yippee!");
                    Destroy();
                    // then die <3
                }
            }
            else
            {
                Debug.Log("Player not realized! Sorry for dropping you into water to drown, I twied :(");
            }
        }
    }
}