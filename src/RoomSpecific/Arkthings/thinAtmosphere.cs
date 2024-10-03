using EffExt;
using System.Linq;

namespace Unbound;


internal class thinAtmosphere
{
    public static void Init()
    {
        On.AirBreatherCreature.Update += atmosphereAir;
        On.Player.LungUpdate += atmosphereLungUpdate;
        On.PlayerGraphics.Update += noBreathingInLowAtmos;
        On.Creature.Update += freezingSky;

        EffectDefinitionBuilder atmosphereEffect = new EffectDefinitionBuilder("Thin Atmosphere");

        atmosphereEffect
                .AddFloatField("floatfield", 0f, 1f, 0.001f, 0.1f, "Strength")
                .SetUADFactory((room, data, firstTimeRealized) => new thinAtmosphereData(data))
                .SetCategory("NCR's Effects")
                .Register();
    }

    internal class thinAtmosphereData : UpdatableAndDeletable
    {

        public EffectExtraData EffectData { get; }

        public thinAtmosphereData(EffectExtraData effectData)
        {
            EffectData = effectData;
        }

        public override void Update(bool eu)
        {
            room.AddObject(new drawnAtmosphere(this, room));
        }
    }

    // start low atmosphere object
    private class drawnAtmosphere : UpdatableAndDeletable, IDrawable
    {
        private readonly thinAtmosphereData owner;
        public Room affectedRoom;

        public drawnAtmosphere(thinAtmosphereData owner, Room room)
        {
            this.owner = owner;
            this.affectedRoom = room;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            float strength = this.owner.EffectData.GetFloat("floatfield");

            if (room.GetNCRRoom().atmosphereFloat != strength)
            {
                room.GetNCRRoom().atmosphereFloat = strength;
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1];

            sLeaser.sprites[0] = new FSprite("FoodCircleA")
            {
                color = Color.red,
                scale = 2f,
                alpha = 0f
            };
            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            sLeaser.sprites[0].SetPosition(-1000, -1000);
            if (!sLeaser.deleteMeNextFrame && (base.slatedForDeletetion || this.room != rCam.room))
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            //
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContatiner)
        {
            foreach (var sprite in sLeaser.sprites) sprite?.RemoveFromContainer();
            newContatiner ??= rCam.ReturnFContainer("Foreground");
            sLeaser.sprites[0].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[0]);
        }
    }





    private static void freezingSky(On.Creature.orig_Update orig, Creature self, bool eu)
    {
        orig(self, eu);
        if (!self.dead && self != null && self.room != null && self.abstractCreature.creatureTemplate.type != CreatureTemplate.Type.Overseer &&
            self.newToRoomInvinsibility <= 0 && !self.abstractCreature.world.game.setupValues.invincibility)
        {
            self.HypothermiaGain = 0f;

            if (self.room.GetNCRRoom().atmosphereFloat > 0f)
            {
                foreach (IProvideWarmth provideWarmth in self.room.blizzardHeatSources)
                {
                    float num = Vector2.Distance(self.firstChunk.pos, provideWarmth.Position());
                    if (self.abstractCreature.Hypothermia > 0.001f && provideWarmth.loadedRoom == self.room && num < provideWarmth.range)
                    {
                        float num2 = Mathf.InverseLerp(provideWarmth.range, provideWarmth.range * 0.2f, num);
                        self.abstractCreature.Hypothermia -= Mathf.Lerp(provideWarmth.warmth * num2, 0f, self.HypothermiaExposure);
                        if (self.abstractCreature.Hypothermia < 0f)
                        {
                            self.abstractCreature.Hypothermia = 0f;
                        }
                    }
                }
                if (!self.dead)
                {
                    self.HypothermiaGain = (Mathf.Lerp(0f, RainWorldGame.DefaultHeatSourceWarmth * 0.08f,
                        Mathf.InverseLerp(0.05f, 0.8f, self.room.GetNCRRoom().atmosphereFloat)) / 1000f);

                    if (!self.abstractCreature.HypothermiaImmune)
                    {
                        self.HypothermiaGain += (Mathf.Lerp(0f, 0.9f, self.room.GetNCRRoom().atmosphereFloat) / 30f);
                        self.HypothermiaGain += (Mathf.Lerp(-self.room.gravity, 0.4f, Mathf.InverseLerp(0f, 0.5f, 1f)) / 45f);
                    }

                    if (self.Submersion >= 0.15f)
                    {
                        self.HypothermiaExposure = 1f;
                    }


                    self.HypothermiaGain += self.Submersion / 7000f;
                    self.HypothermiaGain *= (Mathf.InverseLerp(50f, -10f, self.TotalMass)) / 5f;
                }
                else // if dead
                {
                    self.HypothermiaExposure = 1f;
                    self.HypothermiaGain = Mathf.Lerp(0f, 4E-05f, Mathf.InverseLerp(0.8f, 1f, self.room.world.rainCycle.CycleProgression));
                    self.HypothermiaGain += self.Submersion / 6000f;
                    self.HypothermiaGain += Mathf.InverseLerp(50f, -10f, self.TotalMass) / 1000f;
                }

                if (self.Hypothermia > 1.5f)
                {
                    self.HypothermiaGain *= 2.3f;
                }
                else if (self.Hypothermia > 0.8f)
                {
                    self.HypothermiaGain *= 1f;
                }
                if (self.abstractCreature.HypothermiaImmune)
                {
                    self.HypothermiaGain /= 75f;
                }
                self.HypothermiaGain = Mathf.Clamp(self.HypothermiaGain, -1f, 0.0055f);
                self.Hypothermia += self.HypothermiaGain;

                if (self.Hypothermia >= 0.8f && self.Consious && self.room != null && !self.room.abstractRoom.shelter)
                {
                    if (self.HypothermiaGain > 0.0003f)
                    {
                        if (self.HypothermiaStunDelayCounter < 0)
                        {
                            int stunValue = (int)Mathf.Lerp(5f, 60f, Mathf.Pow(self.Hypothermia / 2f, 8f));

                            self.HypothermiaStunDelayCounter = (int)UnityEngine.Random.Range(300f -
                                self.Hypothermia * 120f, 500f - self.Hypothermia * 100f);
                            self.Stun(stunValue);
                        }
                    }
                    else
                    {
                        self.HypothermiaStunDelayCounter = UnityEngine.Random.Range(200, 500);
                    }
                }
                if (self.Hypothermia >= 1f && (float)self.stun > 50f && !self.dead)
                {
                    self.Die();
                    return;
                }
                if (self.room != null && !self.room.abstractRoom.shelter)
                {
                    self.HypothermiaStunDelayCounter--;
                }
            }
            
        }
    }

    private static void noBreathingInLowAtmos(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
        if (!self.player.dead && self != null && self.player != null && self.player.room != null &&
            self.player.room.GetNCRRoom().atmosphereFloat > 0f)
        {
            self.breath -= self.player.room.GetNCRRoom().atmosphereFloat / Mathf.Lerp(60f, 15f, Mathf.Pow(self.player.aerobicLevel, 1.5f));
        }
    }

    private static void atmosphereLungUpdate(On.Player.orig_LungUpdate orig, Player self)
    {
        if (self != null && !self.dead && self.room != null && self.Submersion < 1f &&
            self.room.GetNCRRoom().atmosphereFloat > 0f)
        {
            self.airInLungs = Mathf.Min(self.airInLungs, 1f - self.rainDeath);

            if (self.firstChunk.submersion <= 0.9f && !self.room.game.setupValues.invincibility && !self.chatlog)
            {
                float num = self.airInLungs;
                if (!self.monkAscension)
                {
                    self.airInLungs -= (self.room.GetNCRRoom().atmosphereFloat / (40f * (self.lungsExhausted ? 4.5f : 9f) *
                        ((self.input[0].y == 1 && self.input[0].x == 0 && self.airInLungs < 0.33333334f) ? 1.5f : 1f) *
                        ((float)self.room.game.setupValues.lungs / 100f)) * self.slugcatStats.lungsFac) / 2f;
                }
                if (self.airInLungs <= 0f)
                {
                    self.airInLungs = 0f;
                    self.Stun(10);
                    self.drown += 0.00833333f;
                    if (self.drown >= 1f)
                    {
                        self.Die();
                    }
                }
                else if (self.airInLungs < 0.3333333f)
                {
                    if (self.slowMovementStun < 1)
                    {
                        self.slowMovementStun = 1;
                    }
                }
            }
            else
            {
                if (self.airInLungs < 0.33333334f)
                {
                    self.lungsExhausted = true;
                }
                if (!self.lungsExhausted && self.airInLungs > 0.9f)
                {
                    self.airInLungs = 1f;
                }
                if (self.airInLungs <= 0f)
                {
                    self.airInLungs = 0f;
                }
                self.airInLungs += 1f / (float)(self.lungsExhausted ? 240 : 60);
                if (self.airInLungs >= 1f)
                {
                    self.airInLungs = 1f;
                    self.lungsExhausted = false;
                    self.drown = 0f;
                }
                self.submerged = false;
            }
            if (self.lungsExhausted)
            {
                if (self.slowMovementStun < 5)
                {
                    self.slowMovementStun = 5;
                }
                if (self.drown > 0f && self.slowMovementStun < 10)
                {
                    self.slowMovementStun = 10;
                }
            }
        }
        else
        {
            orig(self);
        }
    }

    private static void atmosphereAir(On.AirBreatherCreature.orig_Update orig, AirBreatherCreature self, bool eu)
    {
        if (self != null && !self.dead && self.room != null && self.Submersion < 1f &&
            self.room.GetNCRRoom().atmosphereFloat > 0f)
        {
            self.lungs = (Mathf.Max(-1f, self.lungs - self.room.GetNCRRoom().atmosphereFloat / self.Template.lungCapacity) / 1.5f);
            if (self.lungs < 0.25f)
            {
                if (UnityEngine.Random.value < 0.02f) // slightly lower chance than water
                {
                    self.LoseAllGrasps();
                }
                for (int i = 0; i < self.bodyChunks.Length; i++)
                {
                    self.bodyChunks[i].vel += Custom.RNV() * self.bodyChunks[i].rad * 0.4f *
                        UnityEngine.Random.value * Mathf.Sin(Mathf.InverseLerp(0.3f, -0.3f, self.lungs) * 3.1415927f) +
                        Custom.DegToVec(Mathf.Lerp(-30f, 30f, UnityEngine.Random.value)) * UnityEngine.Random.value *
                        ((i == self.mainBodyChunkIndex) ? 0.4f : 0.2f) * Mathf.Pow(Mathf.Sin(Mathf.InverseLerp(0.3f, -0.3f, self.lungs) *
                        3.1415927f), 2f);
                }
                if (self.lungs <= 0f && UnityEngine.Random.value < 0.1f)
                {
                    self.Stun(UnityEngine.Random.Range(0, 18));
                }
                if (self.lungs < -0.5f && UnityEngine.Random.value < 1f / Custom.LerpMap(self.lungs, -0.5f, -1f, 90f, 30f))
                {
                    self.Die();
                }
            }
        }
        orig(self, eu);
    }
    // end effect
}
