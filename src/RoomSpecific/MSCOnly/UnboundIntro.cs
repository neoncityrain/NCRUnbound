

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
                NCRDebug.Log("Could not ban creatures from the cutscene room! Error code: " + e);
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
                        UnboundEnums.unboundKarmaPearl);
                        // first player only
                    }
                    catch (Exception e)
                    {
                        NCRDebug.Log("Could not spawn unbound pearl! Error code: " + e);
                    }
                }

                if (unboundstarttimer < 150)
                {
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        try
                        {
                            (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(34, 36));
                            (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(35, 36));
                        }
                        catch (Exception e)
                        {
                            NCRDebug.Log("Error triggered: " + e);
                        }
                    }
                }
                if (unboundstarttimer == 150)
                {
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        try
                        {
                            (room.game.Players[i].realizedCreature as Player).bodyChunks[0].HardSetPosition(room.MiddleOfTile(31, 35));
                            (room.game.Players[i].realizedCreature as Player).bodyChunks[1].HardSetPosition(room.MiddleOfTile(32, 35));
                        }
                        catch (Exception e)
                        {
                            NCRDebug.Log("Error triggered: " + e);
                        }
                    }
                }

                if (unboundstarttimer < 290)
                {
                    unboundstarttimer++;
                    for (int i = 0; i < room.game.Players.Count; i++)
                    {
                        (room.game.Players[i].realizedCreature as Player).aerobicLevel = 1f;
                        (room.game.Players[i].realizedCreature as Player).exhausted = true;
                        (room.game.Players[i].realizedCreature as Player).lungsExhausted = true;
                        (room.game.Players[i].realizedCreature as Player).airInLungs = 0.005f;
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

                    NCRDebug.Log("Start of game initiated, yippee!");
                    Destroy();
                    // then die <3
                }
            }
            else
            {
                NCRDebug.Log("Player not realized in Unbound intro...");
            }
        }
    }
}