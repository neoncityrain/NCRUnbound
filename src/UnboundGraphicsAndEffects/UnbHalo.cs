using System;

namespace Unbound
{
    public class UnbHalo
    {
        public OracleGraphics owner;
        public int firstSprite;
        public int totalSprites;
        public int firstBitSprite;
        public Connection[] connections;
        public float connectionsFireChance;
        public MemoryBit[][] bits;
        public float[,] ringRotations;
        public float expand;
        public float lastExpand;
        public float getToExpand;
        public float push;
        public float lastPush;
        public float getToPush;
        public float white;
        public float lastWhite;
        public float getToWhite;

        public UnbHalo(OracleGraphics owner, int firstSprite)
        {
            this.owner = owner;
            this.firstSprite = firstSprite;
            this.totalSprites = 2;
            this.connections = new Connection[20];
            this.totalSprites += this.connections.Length;
            for (int i = 0; i < this.connections.Length; i++)
            {
                this.connections[i] = new Connection(this, new Vector2(owner.owner.room.PixelWidth / 2f, owner.owner.room.PixelHeight / 2f) + Custom.RNV() * Mathf.Lerp(300f, 500f, UnityEngine.Random.value));
            }
            this.connectionsFireChance = Mathf.Pow(UnityEngine.Random.value, 3f);
            this.firstBitSprite = firstSprite + this.totalSprites;
            this.bits = new MemoryBit[3][];
            this.bits[0] = new MemoryBit[10];
            this.bits[1] = new MemoryBit[30];
            this.bits[2] = new MemoryBit[60];
            for (int j = 0; j < this.bits.Length; j++)
            {
                for (int k = 0; k < this.bits[j].Length; k++)
                {
                    this.bits[j][k] = new MemoryBit(this, new IntVector2(j, k));
                }
            }
            this.totalSprites += 100;
            this.ringRotations = new float[10, 5];
            this.expand = 1f;
            this.getToExpand = 1f;
        }

        public void Update()
        {
            for (int i = 0; i < this.connections.Length; i++)
            {
                this.connections[i].lastLightUp = this.connections[i].lightUp;
                this.connections[i].lightUp *= 0.9f;
                if (UnityEngine.Random.value < this.connectionsFireChance / 40f && this.owner.oracle.Consious)
                {
                    this.connections[i].lightUp = 1f;
                    this.owner.owner.room.PlaySound(SoundID.SS_AI_Halo_Connection_Light_Up, 0f,
                        ModManager.MSC ? (1f * (1f - this.owner.oracle.noiseSuppress)) : 1f, 1f);
                }
            }
            if (UnityEngine.Random.value < 0.016666668f)
            {
                this.connectionsFireChance = Mathf.Pow(UnityEngine.Random.value, 3f);
            }
            if (ModManager.MSC && this.owner.oracle.suppressConnectionFires)
            {
                this.connectionsFireChance = 0f;
            }
            for (int j = 0; j < this.ringRotations.GetLength(0); j++)
            {
                this.ringRotations[j, 1] = this.ringRotations[j, 0];
                if (this.ringRotations[j, 0] != this.ringRotations[j, 3])
                {
                    this.ringRotations[j, 4] += 1f / Mathf.Lerp(20f, Mathf.Abs(this.ringRotations[j, 2] - this.ringRotations[j, 3]), 0.5f);
                    this.ringRotations[j, 0] = Mathf.Lerp(this.ringRotations[j, 2], this.ringRotations[j, 3], Custom.SCurve(this.ringRotations[j, 4], 0.5f));
                    if (this.ringRotations[j, 4] > 1f)
                    {
                        this.ringRotations[j, 4] = 0f;
                        this.ringRotations[j, 2] = this.ringRotations[j, 3];
                        this.ringRotations[j, 0] = this.ringRotations[j, 3];
                    }
                }
                else if (UnityEngine.Random.value < 0.033333335f)
                {
                    this.ringRotations[j, 3] = this.ringRotations[j, 0] + ((UnityEngine.Random.value < 0.5f) ? -1f : 1f) *
                        Mathf.Lerp(15f, 150f, UnityEngine.Random.value);
                }
            }
            for (int k = 0; k < this.bits.Length; k++)
            {
                for (int l = 0; l < this.bits[k].Length; l++)
                {
                    this.bits[k][l].Update();
                }
            }
            if (UnityEngine.Random.value < 0.016666668f && this.bits.Length != 0)
            {
                int num = UnityEngine.Random.Range(0, this.bits.Length);
                for (int m = 0; m < this.bits[num].Length; m++)
                {
                    this.bits[num][m].SetToMax();
                }
            }
            this.lastExpand = this.expand;
            this.lastPush = this.push;
            this.lastWhite = this.white;
            this.expand = Custom.LerpAndTick(this.expand, this.getToExpand, 0.05f, 0.0125f);
            this.push = Custom.LerpAndTick(this.push, this.getToPush, 0.02f, 0.025f);
            this.white = Custom.LerpAndTick(this.white, this.getToWhite, 0.07f, 0.022727273f);
            bool flag = false;
            if (UnityEngine.Random.value < 0.00625f)
            {
                if (UnityEngine.Random.value < 0.125f)
                {
                    flag = (this.getToWhite < 1f);
                    this.getToWhite = 1f;
                }
                else
                {
                    this.getToWhite = 0f;
                }
            }
            if (UnityEngine.Random.value < 0.00625f || flag)
            {
                this.getToExpand = ((UnityEngine.Random.value < 0.5f && !flag) ? 1f : Mathf.Lerp(0.8f, 2f, Mathf.Pow(UnityEngine.Random.value, 1.5f)));
            }
            if (UnityEngine.Random.value < 0.00625f || flag)
            {
                this.getToPush = ((UnityEngine.Random.value < 0.5f && !flag) ? 0f : ((float)(-1 +
                    UnityEngine.Random.Range(0, UnityEngine.Random.Range(1, 6)))));
            }
        }

        public void ChangeAllRadi()
        {
            this.getToExpand = Mathf.Lerp(0.8f, 2f, Mathf.Pow(UnityEngine.Random.value, 1.5f));
            this.getToPush = (float)(-1 + UnityEngine.Random.Range(0, UnityEngine.Random.Range(1, 6)));
        }

        public float Radius(float ring, float timeStacker)
        {
            return (3f + ring + Mathf.Lerp(this.lastPush, this.push, timeStacker) - 0.5f * this.owner.averageVoice) * Mathf.Lerp(this.lastExpand, this.expand, timeStacker) * 10f;
        }

        public float Rotation(int ring, float timeStacker)
        {
            return Mathf.Lerp(this.ringRotations[ring, 1], this.ringRotations[ring, 0], timeStacker);
        }

        public Vector2 Center(float timeStacker)
        {
            Vector2 vector = Vector2.Lerp(this.owner.head.lastPos, this.owner.head.pos, timeStacker);
            return vector + Custom.DirVec(Vector2.Lerp(this.owner.owner.firstChunk.lastPos, this.owner.owner.firstChunk.pos, timeStacker), vector) * 20f;
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            for (int i = 0; i < 2; i++)
            {
                sLeaser.sprites[this.firstSprite + i] = new FSprite("Futile_White", true);
                sLeaser.sprites[this.firstSprite + i].shader = rCam.game.rainWorld.Shaders["VectorCircle"];
                sLeaser.sprites[this.firstSprite + i].color = new Color(0f, 0f, 0f);
            }
            for (int j = 0; j < this.connections.Length; j++)
            {
                sLeaser.sprites[this.firstSprite + 2 + j] = TriangleMesh.MakeLongMesh(20, false, false);
                sLeaser.sprites[this.firstSprite + 2 + j].color = new Color(0f, 0f, 0f);
            }
            for (int k = 0; k < 100; k++)
            {
                sLeaser.sprites[this.firstBitSprite + k] = new FSprite("pixel", true);
                sLeaser.sprites[this.firstBitSprite + k].scaleX = 4f;
                sLeaser.sprites[this.firstBitSprite + k].color = new Color(0f, 0f, 0f);
            }
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            if (sLeaser.sprites[this.firstSprite].isVisible != this.owner.oracle.Consious)
            {
                for (int i = 0; i < 2 + this.connections.Length; i++)
                {
                    sLeaser.sprites[this.firstSprite + i].isVisible = this.owner.oracle.Consious;
                }
                for (int j = 0; j < 100; j++)
                {
                    sLeaser.sprites[this.firstBitSprite + j].isVisible = this.owner.oracle.Consious;
                }
            }
            Vector2 vector = this.Center(timeStacker);
            for (int k = 0; k < 2; k++)
            {
                sLeaser.sprites[this.firstSprite + k].x = vector.x - camPos.x;
                sLeaser.sprites[this.firstSprite + k].y = vector.y - camPos.y;
                sLeaser.sprites[this.firstSprite + k].scale = this.Radius((float)k, timeStacker) / 8f;
            }
            sLeaser.sprites[this.firstSprite].alpha = Mathf.Lerp(3f / this.Radius(0f, timeStacker), 1f, Mathf.Lerp(this.lastWhite, this.white, timeStacker));
            sLeaser.sprites[this.firstSprite + 1].alpha = 3f / this.Radius(1f, timeStacker);
            for (int l = 0; l < this.connections.Length; l++)
            {
                if (this.connections[l].lastLightUp > 0.05f || this.connections[l].lightUp > 0.05f)
                {
                    Vector2 vector2 = this.connections[l].stuckAt;
                    float d = 2f * Mathf.Lerp(this.connections[l].lastLightUp, this.connections[l].lightUp, timeStacker);
                    for (int m = 0; m < 20; m++)
                    {
                        float f = (float)m / 19f;
                        Vector2 a = Custom.DirVec(vector, this.connections[l].stuckAt);
                        Vector2 vector3 = Custom.Bezier(this.connections[l].stuckAt, this.connections[l].handle, vector + a * this.Radius(2f, timeStacker), vector + a * 400f, f);
                        Vector2 vector4 = Custom.DirVec(vector2, vector3);
                        Vector2 a2 = Custom.PerpendicularVector(vector4);
                        float d2 = Vector2.Distance(vector2, vector3);
                        (sLeaser.sprites[this.firstSprite + 2 + l] as TriangleMesh).MoveVertice(m * 4, vector3 - vector4 * d2 * 0.3f - a2 * d - camPos);
                        (sLeaser.sprites[this.firstSprite + 2 + l] as TriangleMesh).MoveVertice(m * 4 + 1, vector3 - vector4 * d2 * 0.3f + a2 * d - camPos);
                        (sLeaser.sprites[this.firstSprite + 2 + l] as TriangleMesh).MoveVertice(m * 4 + 2, vector3 - a2 * d - camPos);
                        (sLeaser.sprites[this.firstSprite + 2 + l] as TriangleMesh).MoveVertice(m * 4 + 3, vector3 + a2 * d - camPos);
                        vector2 = vector3;
                    }
                }
            }
            int num = this.firstBitSprite;
            for (int n = 0; n < this.bits.Length; n++)
            {
                for (int num2 = 0; num2 < this.bits[n].Length; num2++)
                {
                    float num3 = (float)num2 / (float)this.bits[n].Length * 360f + this.Rotation(n, timeStacker);
                    Vector2 vector5 = vector + Custom.DegToVec(num3) * this.Radius((float)n + 0.5f, timeStacker);
                    sLeaser.sprites[num].scaleY = 8f * this.bits[n][num2].Fill(timeStacker);
                    sLeaser.sprites[num].x = vector5.x - camPos.x;
                    sLeaser.sprites[num].y = vector5.y - camPos.y;
                    sLeaser.sprites[num].rotation = num3;
                    num++;
                }
            }
        }


        public class Connection
        {
            public UnbHalo halo;
            public Vector2 stuckAt;
            public Vector2 handle;
            public float lightUp;
            public float lastLightUp;

            public Connection(UnbHalo halo, Vector2 stuckAt)
            {
                this.halo = halo;
                Vector2 vector = stuckAt;
                vector.x = Mathf.Clamp(vector.x, halo.owner.oracle.arm.cornerPositions[0].x, halo.owner.oracle.arm.cornerPositions[1].x);
                vector.y = Mathf.Clamp(vector.y, halo.owner.oracle.arm.cornerPositions[2].y, halo.owner.oracle.arm.cornerPositions[1].y);
                this.stuckAt = Vector2.Lerp(stuckAt, vector, 0.5f);
                this.handle = stuckAt + Custom.RNV() * Mathf.Lerp(400f, 700f, UnityEngine.Random.value);
            }
        }

        public class MemoryBit
        {
            public UnbHalo halo;
            public IntVector2 position;
            public float filled;
            public float lastFilled;
            public float getToFilled;
            public float fillSpeed;
            public int blinkCounter;

            public float Fill(float timeStacker)
            {
                if (this.blinkCounter % 4 > 1 && this.filled == this.getToFilled)
                {
                    return 0f;
                }
                return Mathf.Lerp(this.lastFilled, this.filled, timeStacker);
            }

            public MemoryBit(UnbHalo halo, IntVector2 position)
            {
                this.halo = halo;
                this.position = position;
                this.filled = UnityEngine.Random.value;
                this.lastFilled = this.filled;
                this.getToFilled = this.filled;
                this.fillSpeed = 0f;
            }

            public void SetToMax()
            {
                this.getToFilled = 1f;
                this.fillSpeed = Mathf.Lerp(this.fillSpeed, 0.25f, 0.25f);
                this.blinkCounter = 20;
            }

            public void Update()
            {
                this.lastFilled = this.filled;
                if (this.filled != this.getToFilled)
                {
                    this.filled = Custom.LerpAndTick(this.filled, this.getToFilled, 0.03f, this.fillSpeed);
                    return;
                }
                if (this.blinkCounter > 0)
                {
                    this.blinkCounter--;
                    return;
                }
                if (UnityEngine.Random.value < 0.016666668f)
                {
                    this.getToFilled = UnityEngine.Random.value;
                    this.fillSpeed = 1f / Mathf.Lerp(2f, 80f, UnityEngine.Random.value);
                }
            }
            
        }

        // end halo
    }
}
