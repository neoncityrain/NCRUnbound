using MoreSlugcats;

namespace Unbound;

public class UnbSLWall06 : UpdatableAndDeletable
{
    public AbstractCreature abstractcat;
    public Creature technicianCat;
    public int timer;
    public bool setUp;
    public Player player;

    public UnbSLWall06(Room room, Player player)
    {
        this.room = room;
        this.player = player;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (this.room != null && this.room.game.FirstRealizedPlayer != null && this.room.abstractRoom != null &&
            this.room.game.AllPlayersRealized)
        {
            if (!this.setUp || this.technicianCat == null) { Init(); } // setting up initial values


            else if (this.setUp) // if set up
            {
                try {
                    if (this.timer == 0 && this.technicianCat.mainBodyChunk.pos != new Vector2(30f, 34f) &&
                        this.room == player.room)
                    {
                        this.abstractcat.Realize();
                        this.technicianCat.mainBodyChunk.HardSetPosition(new Vector2(30f, 34f));
                        NCRDebug.Log("Tech position set!");
                    }
                }
                catch (Exception e) { NCRDebug.Log("Error setting tech position: " + e); }


                NCRDebug.Log("Running timer...");
                if (this.room == player.room) { this.timer++; }
                // proceed timer while in room



                if ((this.technicianCat.dead || this.room != player.room) && this.timer < 300 &&
                        this.timer > 5) { this.timer = 300; }
                    // if tech is dead or the room isnt the given room (SL_WALL06), but the timer HAS started, end the timer immediately


                if (this.timer >= 300) // if the event was set up and events ran, but timer is over 300
                {
                    
                    this.Destroy();
                }
            }

            // end update
        }
    }

    public void Init()
    {
        if (!setUp)
        {
            try
            {
                // start init
                try
                {
                    NCRDebug.Log("Initiating...");
                    this.timer = 0;

                    this.abstractcat = new AbstractCreature(this.room.world,
                        StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat),
                        null, this.room.ToWorldCoordinate(new Vector2(30f, 34f)),
                        this.room.game.GetNewID());

                    this.abstractcat.ID.setAltSeed(-1913912525); // 19 13 9 12 5 25 : smiley

                    this.abstractcat.state = new PlayerState(this.abstractcat, 4, UnboundEnums.NCRTechnician, false);

                    this.technicianCat = this.abstractcat.realizedCreature;
                }
                catch (Exception e) { NCRDebug.Log("Error setting up abstractcat: " + e); }


                
                try { this.technicianCat = this.technicianCat as Player;}
                catch (Exception e) { NCRDebug.Log("Error techniciancat to be player: " + e); }

                try { this.room.abstractRoom.AddEntity(this.abstractcat); }
                catch (Exception e) { NCRDebug.Log("Error realizing: " + e); }


                this.setUp = true;
            }
            catch (Exception e) { NCRDebug.Log("Error in initiating: " + e); this.Destroy(); }
            // end setup
        }
    }
}