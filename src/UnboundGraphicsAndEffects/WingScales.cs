namespace Unbound
{
    public class UnbScales
    {
        public WingScale[] scaleObjects;
        public float[] backwardsFactors;
        public int graphic;
        public float graphicHeight;
        public float rigor;
        public float scaleX;
        public bool colored;
        public Vector2[] scalesPositions;
        public PlayerGraphics pGraphics;
        public int numberOfSprites;
        public int startSprite;
        public RoomPalette palette;
        public SpritesOverlap spritesOverlap;
        public Color baseColor;
        public Color effectColor;


        public UnbScales(PlayerGraphics pGraphics, int startSprite)
        {
            this.pGraphics = pGraphics;
            this.startSprite = startSprite;
            this.rigor = 0.5873646f;
            float num = 1.310689f;
            this.colored = true;
            this.graphic = 3;
            this.graphicHeight = Futile.atlasManager.GetElementWithName("LizardScaleA" + this.graphic.ToString()).sourcePixelSize.y;
            int scaleamount = 1;
            this.scalesPositions = new Vector2[scaleamount * 2];
            this.scaleObjects = new WingScale[this.scalesPositions.Length];
            this.backwardsFactors = new float[this.scalesPositions.Length];
            float num3 = 0.1542603f;
            float num4 = 0.1759363f;
            for (int i = 0; i < scaleamount; i++)
            {
                float y = 0.03570603f;
                float num5 = 0.659981f;
                float num6 = 0.9722961f;
                float num7 = 0.3644831f;
                if (i == 1)
                {
                    y = 0.02899241f;
                    num5 = 0.76459f;
                    num6 = 0.6056554f;
                    num7 = 0.9129724f;
                }
                if (i == 2)
                {
                    y = 0.02639332f;
                    num5 = 0.7482835f;
                    num6 = 0.7223744f;
                    num7 = 0.4567381f;
                }
                for (int j = 0; j < 2; j++)
                {
                    this.scalesPositions[i * 2 + j] = new Vector2((j != 0) ? num5 : (-num5), y);
                    this.scaleObjects[i * 2 + j] = new WingScale(pGraphics);
                    this.scaleObjects[i * 2 + j].length = Mathf.Lerp(2.5f, 15f, num * num6);
                    this.scaleObjects[i * 2 + j].width = Mathf.Lerp(0.65f, 1.2f, num3 * num);
                    this.backwardsFactors[i * 2 + j] = num4 * num7;
                }
            }
            this.numberOfSprites = ((!this.colored) ? this.scalesPositions.Length : (this.scalesPositions.Length * 2));
            this.spritesOverlap = SpritesOverlap.BehindHead;
        }

        public void Update()
        {
            for (int i = 0; i < this.scaleObjects.Length; i++)
            {
                Vector2 pos = this.pGraphics.owner.bodyChunks[0].pos;
                Vector2 pos2 = this.pGraphics.owner.bodyChunks[1].pos;
                float num = 0f;
                float num2 = 90f;
                int num3 = i % (this.scaleObjects.Length / 2);
                float num4 = num2 / (float)(this.scaleObjects.Length / 2);
                if (i >= this.scaleObjects.Length / 2)
                {
                    num = 0f;
                    pos.x += 5f;
                }
                else
                {
                    pos.x -= 5f;
                }
                Vector2 a = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num + 90f);
                float f = Custom.VecToDeg(this.pGraphics.lookDirection);
                Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)num3 * num4 - num2 / 2f + num);
                Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), Mathf.Abs(f));
                if (this.scalesPositions[i].y < 0.2f)
                {
                    a2 -= a * Mathf.Pow(Mathf.InverseLerp(0.2f, 0f, this.scalesPositions[i].y), 2f) * 2f;
                }
                a2 = Vector2.Lerp(a2, vector, Mathf.Pow(this.backwardsFactors[i], 1f)).normalized;
                Vector2 vector2 = pos + a2 * this.scaleObjects[i].length;
                if (!Custom.DistLess(this.scaleObjects[i].pos, vector2, this.scaleObjects[i].length / 2f))
                {
                    Vector2 a3 = Custom.DirVec(this.scaleObjects[i].pos, vector2);
                    float num5 = Vector2.Distance(this.scaleObjects[i].pos, vector2);
                    float num6 = this.scaleObjects[i].length / 2f;
                    this.scaleObjects[i].pos += a3 * (num5 - num6);
                    this.scaleObjects[i].vel += a3 * (num5 - num6);
                }
                this.scaleObjects[i].vel += Vector2.ClampMagnitude(vector2 - this.scaleObjects[i].pos, 10f) / Mathf.Lerp(5f, 1.5f, this.rigor);
                this.scaleObjects[i].vel *= Mathf.Lerp(1f, 0.8f, this.rigor);
                this.scaleObjects[i].ConnectToPoint(pos, this.scaleObjects[i].length, true, 0f, new Vector2(0f, 0f), 0f, 0f);
                this.scaleObjects[i].Update();
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            for (int i = this.startSprite + this.scalesPositions.Length - 1; i >= this.startSprite; i--)
            {
                sLeaser.sprites[i] = new FSprite("LizardScaleA" + this.graphic.ToString(), true);
                sLeaser.sprites[i].scaleY = this.scaleObjects[i - this.startSprite].length / this.graphicHeight;
                sLeaser.sprites[i].anchorY = -0.1f;
                if (this.colored)
                {
                    sLeaser.sprites[i + this.scalesPositions.Length] = new FSprite("LizardScaleB" + this.graphic.ToString(), true);
                    sLeaser.sprites[i + this.scalesPositions.Length].scaleY = this.scaleObjects[i - this.startSprite].length / this.graphicHeight;
                    sLeaser.sprites[i + this.scalesPositions.Length].anchorY = -0.1f;
                }
            }
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (this.pGraphics.owner == null)
            {
                return;
            }
            for (int i = this.startSprite + this.scalesPositions.Length - 1; i >= this.startSprite; i--)
            {
                Vector2 vector = new Vector2(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y + camPos.y);
                float f = 0f;
                float num = 0f;
                if (i < this.startSprite + this.scalesPositions.Length / 2)
                {
                    vector.x -= 5f;
                }
                else
                {
                    num = 180f;
                    vector.x += 5f;
                }
                sLeaser.sprites[i].x = vector.x - camPos.x;
                sLeaser.sprites[i].y = vector.y - camPos.y;
                sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(this.scaleObjects[i -
                    this.startSprite].lastPos, this.scaleObjects[i - this.startSprite].pos, timeStacker)) + num;
                sLeaser.sprites[i].scaleX = this.scaleObjects[i - this.startSprite].width * Mathf.Sign(f);
                if (this.colored)
                {
                    sLeaser.sprites[i + this.scalesPositions.Length].x = vector.x - camPos.x;
                    sLeaser.sprites[i + this.scalesPositions.Length].y = vector.y - camPos.y;
                    sLeaser.sprites[i + this.scalesPositions.Length].rotation = Custom.AimFromOneVectorToAnother(vector,
                        Vector2.Lerp(this.scaleObjects[i - this.startSprite].lastPos, this.scaleObjects[i - this.startSprite].pos, timeStacker)) + num;
                    sLeaser.sprites[i + this.scalesPositions.Length].scaleX = this.scaleObjects[i - this.startSprite].width * Mathf.Sign(f);
                    if (i < this.startSprite + this.scalesPositions.Length / 2)
                    {
                        sLeaser.sprites[i + this.scalesPositions.Length].scaleX *= -1f;
                    }
                }
                if (i < this.startSprite + this.scalesPositions.Length / 2)
                {
                    sLeaser.sprites[i].scaleX *= -1f;
                }
            }
            for (int j = this.startSprite + this.scalesPositions.Length - 1; j >= this.startSprite; j--)
            {
                sLeaser.sprites[j].color = this.baseColor;
                if (this.colored)
                {
                    sLeaser.sprites[j + this.scalesPositions.Length].color = Color.Lerp(this.effectColor, this.baseColor,
                        this.pGraphics.malnourished / 1.75f);
                }
            }
        }

        public void SetScaleColors(Color baseCol, Color effectCol)
        {
            this.baseColor = baseCol;
            if (this.pGraphics.useJollyColor)
            {
                this.effectColor = PlayerGraphics.JollyColor(this.pGraphics.player.playerState.playerNumber, 2);
                return;
            }
            if (PlayerGraphics.CustomColorsEnabled())
            {
                this.effectColor = PlayerGraphics.CustomColorSafety(2);
                return;
            }
            this.effectColor = effectCol;
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            this.palette = palette;
            for (int i = this.startSprite + this.scalesPositions.Length - 1; i >= this.startSprite; i--)
            {
                sLeaser.sprites[i].color = this.baseColor;
                if (this.colored)
                {
                    sLeaser.sprites[i + this.scalesPositions.Length].color = this.effectColor;
                }
            }
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            for (int i = this.startSprite; i < this.startSprite + this.numberOfSprites; i++)
            {
                newContatiner.AddChild(sLeaser.sprites[i]);
            }
        }

        public class SpritesOverlap : ExtEnum<PlayerGraphics.AxolotlGills.SpritesOverlap>
        {
            public SpritesOverlap(string value, bool register = false) : base(value, register)
            {
            }

            public static SpritesOverlap Behind = new SpritesOverlap("Behind", true);
            public static SpritesOverlap BehindHead = new SpritesOverlap("BehindHead", true);
            public static SpritesOverlap InFront = new SpritesOverlap("InFront", true);
        }
        // end scalebunch
    }

    public class WingScale : BodyPart
    {
        public WingScale(GraphicsModule cosmetics) : base(cosmetics)
        {
        }
        public float length;
        public float width;

        public override void Update()
        {
            base.Update();
            if (this.owner.owner.room.PointSubmerged(this.pos))
            {
                this.vel *= 0.5f;
            }
            else
            {
                this.vel *= 0.9f;
            }
            this.lastPos = this.pos;
            this.pos += this.vel;
        }
    }
    // end wingscales
}