﻿using System;

namespace Unbound
{
    public class UnbJumplight : CosmeticSprite
    {
        public Player player;
        public float life;
        public float lastLife;
        public float lifeTime;
        public float intensity;

        public UnbJumplight(Vector2 pos, float intensity, Player player)
        {
            life = 1f;
            lastLife = 1f;
            this.pos = pos;
            lastPos = pos;
            lifeTime = Mathf.Lerp(4f, 22f, Mathf.Pow(intensity, 2f));
            this.player = player;
        }

        public override void Update(bool eu)
        {
            lastLife = life;
            life -= 1f / lifeTime;
            if (lastLife <= 0f)
            {
                Destroy();
            }
            base.Update(eu);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[3];
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i] = new FSprite("Futile_White", true);
            }
            sLeaser.sprites[0].shader = rCam.game.rainWorld.Shaders["LightSource"];
            sLeaser.sprites[1].shader = rCam.game.rainWorld.Shaders["FlatLight"];
            sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["FlatLight"];
            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(lastPos, pos, timeStacker);
            float num = Mathf.Lerp(lastLife, life, timeStacker);
            for (int i = 0; i < sLeaser.sprites.Length; i++)
            {
                sLeaser.sprites[i].x = vector.x - camPos.x;
                sLeaser.sprites[i].y = vector.y - camPos.y;
            }
            float num2 = Mathf.Lerp(20f + 10f * intensity, 40f + 30f * intensity, num) +
                Mathf.Lerp(50f, 90f, intensity) * Mathf.Sin(Mathf.Pow(num, 2f) * 3.1415927f);
            sLeaser.sprites[0].scale = num2 * 2f / 8f;
            sLeaser.sprites[0].alpha = Mathf.Pow(Mathf.InverseLerp(0f, 0.5f, num), 0.5f);
            sLeaser.sprites[1].scale = num2 / 8f;
            sLeaser.sprites[1].alpha = Mathf.Pow(num, 2f) * (0.4f + 0.4f * intensity);

            sLeaser.sprites[0].color = player.GetNCRunbound().effectColour;
            sLeaser.sprites[1].color = player.GetNCRunbound().effectColour;

            sLeaser.sprites[2].scale = num2 * Mathf.Lerp(0.4f, 0.8f, UnityEngine.Random.value) / 8f;
            sLeaser.sprites[2].alpha = Mathf.Pow(Mathf.InverseLerp(0.25f, 1f, num), 3f) * intensity;
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
        }

        public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
        }
        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            if (newContatiner == null)
            {
                newContatiner = rCam.ReturnFContainer("Bloom");
            }
            foreach (FSprite fsprite in sLeaser.sprites)
            {
                fsprite.RemoveFromContainer();
                newContatiner.AddChild(fsprite);
            }
        }
    }
}