using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

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
            this.room.game.cameras[0].MoveCamera(2);
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.BigEel, false);
            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.Leech, false);
            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.SeaLeech, false);
            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", MoreSlugcatsEnums.CreatureTemplateType.JungleLeech, false);
            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.Vulture, false);
            this.room.world.ToggleCreatureAccessFromCutscene("MS_FARSIDE", CreatureTemplate.Type.KingVulture, false);

            if (room.game.AllPlayersRealized)
            {
                
                for (int i = 0; i < this.room.game.Players.Count; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        (this.room.game.Players[i].realizedCreature as Player).bodyChunks[j].HardSetPosition(this.room.MiddleOfTile(142, 72));
                        (this.room.game.Players[i].realizedCreature as Player).standing = false;
                    }
                }


                if (unboundstarttimer == 0)
                {
                    CameraSetup();
                }
                if (unboundstarttimer < 300)
                {
                    this.unboundstarttimer++;
                    for (int i = 0; i < this.room.game.Players.Count; i++)
                    {
                        (this.room.game.Players[i].realizedCreature as Player).airInLungs = 0.01f;
                        (this.room.game.Players[i].realizedCreature as Player).stun = 100;
                    }
                }

                if (unboundstarttimer == 300)
                {
                    for (int i = 0; i < this.room.game.Players.Count; i++)
                    {
                        (this.room.game.Players[i].realizedCreature as Player).stun = 0;
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