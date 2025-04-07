namespace Unbound;
public class UnboundWingScales
{
    // thank you pocky!!
    public UnboundWingScales(PlayerGraphics pGraphics, int startSprite)
    {
        this.pGraphics = pGraphics;
        this.startSprite = startSprite;
        this.rigor = 0.5873646f;
        float num = 1.310689f;
        this.colored = true;
        this.scalesPositions = new Vector2[WingNum];
        this.scaleObjects = new ScalePart[this.scalesPositions.Length];
        this.backwardsFactors = new float[this.scalesPositions.Length];
        float num4 = 0.1759363f;

        for (int i = 0; i < WingNum; i++)
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
            this.scalesPositions[i] = new Vector2(num5, y);
            this.scaleObjects[i] = new(pGraphics);
            this.scaleObjects[i].length = Mathf.Lerp(2.5f, 15f, num * num6);
            this.backwardsFactors[i] = num4 * num7;
        }
        this.numberOfSprites = ((!this.colored) ? this.scalesPositions.Length : (this.scalesPositions.Length * 2));
        this.spritesOverlap = new SpritesOverlap[2];
        spritesOverlap[0] = SpritesOverlap.Behind;
        spritesOverlap[1] = SpritesOverlap.Behind;
    }
    public float DirectionUpdate(float dir)
    {
        float lerp = 1f;
        float dirA = 0f;
        float dirB = 0f;

        if (dir is >= -7.5f and < 135f)
        {
            dirA = -55f;
            dirB = -55f;
            lerp = 1f;
        }
        else if (dir is >= 135f and < 180f)
        {
            dirA = -55f;
            dirB = 0f;
            lerp = Mathf.InverseLerp(135f, 180f, dir);
        }
        else if (dir is 180f)
        {
            dirA = 0f;
            dirB = 0f;
            lerp = 1f;
        }

        //the other side
        else if (dir is >= -15f and < -7.5f)
        {
            dirA = 55f;
            dirB = -55f;
            lerp = Mathf.InverseLerp(-15f, 0f, dir);
        }
        else if (dir is < -15f and >= -135f)
        {
            dirA = 55f;
            dirB = 55f;
            lerp = Mathf.InverseLerp(-45f, -135f, dir);
        }
        else if (dir is < -135f and >= -180f)
        {
            dirA = 55f;
            dirB = 0f;
            lerp = Mathf.InverseLerp(-135f, -180f, dir);
        }

        var result = Mathf.Lerp(dirA, dirB, lerp);

        if (pGraphics.player.bodyMode == Player.BodyModeIndex.WallClimb) result = 55f * -pGraphics.player.flipDirection;

        return result;
    }

    public void OverlapUpdate(float dir)
    {
        if (dir is >= -67.5f and < 67.5f)
        {
            spritesOverlap[0] = SpritesOverlap.Behind;
            spritesOverlap[1] = SpritesOverlap.Behind;
        }
        else if (dir is >= 67.5f and < 112.5f)
        {
            spritesOverlap[0] = SpritesOverlap.InFront;
            spritesOverlap[1] = SpritesOverlap.Behind;
        }
        else if (dir is >= 112.5f || dir is < -112.5f)
        {
            spritesOverlap[0] = SpritesOverlap.InFront;
            spritesOverlap[1] = SpritesOverlap.InFront;
        }
        else if (dir is >= -112.5f and < -67.5f)
        {
            spritesOverlap[0] = SpritesOverlap.Behind;
            spritesOverlap[1] = SpritesOverlap.InFront;
        }
    }

    public void Update()
    {
        for (int i = 0; i < this.scaleObjects.Length; i++)
        {
            Vector2 pos = this.pGraphics.owner.bodyChunks[0].pos;
            Vector2 pos2 = this.pGraphics.owner.bodyChunks[1].pos;
            float num = 0f;
            float num2 = 90f;
            float num4 = num2 / (float)(this.scaleObjects.Length / 2);

            Vector2 a = Custom.rotateVectorDeg(Vector2.zero, (float)i * num4 - num2 / 2f + num + 90f);
            Vector2 vector = Custom.rotateVectorDeg(Custom.DegToVec(0f), (float)i * num4 - num2 / 2f + num);
            Vector2 a2 = Vector2.Lerp(vector, Custom.DirVec(pos2, pos), 0.5f);
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
    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, string baseGraphic, string overlayedGraphic)
    {
        for (int i = this.startSprite + this.scalesPositions.Length - 1; i >= this.startSprite; i--)
        {
            sLeaser.sprites[i] = new FSprite(baseGraphic, true);
            sLeaser.sprites[i].anchorY = 0.1f;
            if (this.colored)
            {
                sLeaser.sprites[i + this.scalesPositions.Length] = new FSprite(overlayedGraphic, true);
                sLeaser.sprites[i + this.scalesPositions.Length].anchorY = 0.1f;
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
            Vector2 vector = Vector2.Lerp(sLeaser.sprites[0].GetPosition(), sLeaser.sprites[1].GetPosition(), 0.25f);
            float num = 90f;
            var dir = DirectionUpdate(sLeaser.sprites[0].rotation);
            OverlapUpdate(sLeaser.sprites[0].rotation);

            if (i == this.startSprite + this.scalesPositions.Length - 1) //the first wing
            {
                sLeaser.sprites[i].MoveBehindOtherNode(sLeaser.sprites[(spritesOverlap[0] == SpritesOverlap.InFront) ? 9 : 0]);
            }
            else
            {
                sLeaser.sprites[i].MoveBehindOtherNode(sLeaser.sprites[(spritesOverlap[1] == SpritesOverlap.InFront) ? 9 : 0]);
            }

            sLeaser.sprites[i].SetPosition(vector);
            sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(vector, Vector2.Lerp(
                this.scaleObjects[i - this.startSprite].lastPos, this.scaleObjects[i - this.startSprite].pos, timeStacker)) + num;
            sLeaser.sprites[i].rotation += dir;

            if (i == this.startSprite + this.scalesPositions.Length - 1) sLeaser.sprites[i].scaleX = (dir > 0 && dir < 180) ? -1f : 1f;
            else sLeaser.sprites[i].scaleX = (dir > 0 && dir < 180) ? 1f : -1f;

            if (this.colored)
            {
                sLeaser.sprites[i + this.scalesPositions.Length].SetPosition(vector);
                sLeaser.sprites[i + this.scalesPositions.Length].MoveInFrontOfOtherNode(sLeaser.sprites[i]);
                sLeaser.sprites[i + this.scalesPositions.Length].rotation = sLeaser.sprites[i].rotation;
                sLeaser.sprites[i + this.scalesPositions.Length].scaleX = sLeaser.sprites[i].scaleX;
            }
        }
        for (int j = this.startSprite + this.scalesPositions.Length - 1; j >= this.startSprite; j--)
        {
            sLeaser.sprites[j].color = this.baseColor;
            if (this.colored)
            {
                sLeaser.sprites[j + this.scalesPositions.Length].color = Color.Lerp(
                    this.effectColor, this.baseColor, this.pGraphics.malnourished / 1.75f);
            }
        }
    }
    public void SetWingColors(Color baseCol, Color effectCol)
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

    public ScalePart[] scaleObjects;
    public float[] backwardsFactors;
    public float rigor;
    public bool colored;
    public Vector2[] scalesPositions;
    public PlayerGraphics pGraphics;
    public int numberOfSprites;
    public int startSprite;
    public RoomPalette palette;
    public SpritesOverlap[] spritesOverlap;
    public Color baseColor;
    public Color effectColor;
    public int WingNum => 2;
    public class SpritesOverlap : ExtEnum<UnboundWingScales.SpritesOverlap>
    {
        public SpritesOverlap(string value, bool register = false) : base(value, register)
        {
        }
        public static readonly SpritesOverlap Behind = new("Behind", true);
        public static readonly SpritesOverlap InFront = new("InFront", true);
    }
    public class ScalePart : BodyPart //acts like PlayerGraphics.AxolotlScale
    {
        public ScalePart(GraphicsModule cosmetics) : base(cosmetics)
        {
        }
        public override void Update()
        {
            base.Update();
            if (owner.owner.room.PointSubmerged(pos)) vel *= 0.5f;
            else vel *= 0.9f;
            lastPos = pos;
            pos += vel;
        }
        public float length; // length of wingscale. I think
    }
}
