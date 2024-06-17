using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using Smoke;
using UnityEngine;

namespace Unbound
{
    public class UnbJumpsmoke : SmokeSystem
    {
        public Player player;

        public UnbJumpsmoke(Room room, Player player) : base(SmokeType.CyanLizardSmoke, room, 2, 0f)
        {
            this.player = player;
        }

        public override SmokeSystemParticle CreateParticle()
        {
            return new UnbSmokeSprite();
        }

        public void EmitSmoke(Vector2 pos, Vector2 vel, bool big, float maxlifeTime)
        {
            UnbSmokeSprite sprites = this.AddParticle(pos, vel, maxlifeTime * Mathf.Lerp(0.3f, 1f, UnityEngine.Random.value)) as UnbSmokeSprite;
            if (sprites != null)
            {
                sprites.big = big;
                sprites.player = player;
            }
        }

        public class UnbSmokeSprite : SpriteSmoke
        {
            public Color fadeColor;
            public float moveDir;
            public int counter;
            public bool big;
            public Player player;

            public override void Reset(SmokeSystem newOwner, Vector2 pos, Vector2 vel, float newLifeTime)
            {
                base.Reset(newOwner, pos, vel, newLifeTime);
                this.rad = Mathf.Lerp(28f, 46f, UnityEngine.Random.value);
                this.moveDir = UnityEngine.Random.value * 360f;
                this.counter = 0;
            }

            public override void Update(bool eu)
            {
                base.Update(eu);
                if (this.resting)
                {
                    return;
                }
                this.vel *= 0.7f + 0.3f / Mathf.Pow(this.vel.magnitude, 0.5f);
                this.moveDir += Mathf.Lerp(-1f, 1f, UnityEngine.Random.value) * 50f;
                if (this.room.PointSubmerged(this.pos))
                {
                    this.pos.y = this.room.FloatWaterLevel(this.pos.x);
                }
                this.counter++;
                if (this.room.GetTile(this.pos).Solid && !this.room.GetTile(this.lastPos).Solid)
                {
                    IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(this.room, this.room.GetTilePosition(this.lastPos), this.room.GetTilePosition(this.pos));
                    FloatRect floatRect = Custom.RectCollision(this.pos, this.lastPos, this.room.TileRect(intVector.Value).Grow(2f));
                    this.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);
                    if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                    {
                        this.vel.x = Mathf.Abs(this.vel.x);
                        return;
                    }
                    if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                    {
                        this.vel.x = -Mathf.Abs(this.vel.x);
                        return;
                    }
                    if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                    {
                        this.vel.y = Mathf.Abs(this.vel.y);
                        return;
                    }
                    if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                    {
                        this.vel.y = -Mathf.Abs(this.vel.y);
                    }
                }
            }

            public override float Rad(int type, float useLife, float useStretched, float timeStacker)
            {
                float num;
                if (type != 0)
                {
                    if (type != 1)
                    {
                        num = Mathf.Lerp(4f, this.rad, Mathf.Pow(1f - useLife, 0.2f));
                    }
                    else
                    {
                        num = 1.5f * Mathf.Lerp(2f, this.rad, Mathf.Pow(1f - useLife, 0.2f));
                    }
                }
                else
                {
                    num = Mathf.Lerp(4f, this.rad, Mathf.Pow(1f - useLife, 0.2f) + useStretched);
                }
                if (this.big)
                {
                    num *= 1f + Mathf.InverseLerp(0f, 10f, (float)this.counter + timeStacker);
                }
                else
                {
                    num *= 0.2f;
                }
                return num;
            }

            public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
            {
                base.InitiateSprites(sLeaser, rCam);
                for (int i = 0; i < 2; i++)
                {
                    sLeaser.sprites[i].shader = this.room.game.rainWorld.Shaders["FireSmoke"];
                }
            }

            public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
            {
                base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
                if (this.resting)
                {
                    return;
                }
                float num = Mathf.Lerp(this.lastLife, this.life, timeStacker);

                Color color;

                

                if (this.big)
                {
                    if (ModManager.JollyCoop)
                    {
                        color = Color.Lerp(PlayerGraphics.JollyColor(player.playerState.playerNumber, 2), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num));
                    }
                    else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                    {
                        color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num));

                    }
                    else
                    {
                        color = Color.Lerp(new Color(0.8f, 0.1f, 0.1f), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num));
                    }
                }
                else
                {
                    if (ModManager.JollyCoop)
                    {
                        color = Color.Lerp(PlayerGraphics.JollyColor(player.playerState.playerNumber, 2), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num) * 0.5f);
                    }
                    else if (PlayerGraphics.customColors != null && !ModManager.JollyCoop)
                    {
                        color = Color.Lerp(PlayerGraphics.CustomColorSafety(2), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num) * 0.5f);

                    }
                    else
                    {
                        color = Color.Lerp(new Color(0.8f, 0.1f, 0.1f), this.fadeColor, Mathf.InverseLerp(1f, 0.25f, num) * 0.5f);
                    }
                }
                sLeaser.sprites[0].color = color;
                sLeaser.sprites[1].color = color;
                sLeaser.sprites[0].alpha = Mathf.Pow(num, 0.25f) * (1f - this.stretched) * (this.big ? (1f - 0.2f * Mathf.InverseLerp(0f, 10f, (float)this.counter + timeStacker)) : 1f);
                sLeaser.sprites[1].alpha = (0.3f + Mathf.Pow(Mathf.Sin(num * 3.1415927f), 0.7f) * 0.65f * (1f - this.stretched)) * (this.big ? (1f - 0.2f * Mathf.InverseLerp(0f, 10f, (float)this.counter + timeStacker)) : 1f);
            }
            public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
            {
                this.fadeColor = Color.Lerp(palette.blackColor, palette.fogColor, 0.6f);
            }
        }
    }
}
