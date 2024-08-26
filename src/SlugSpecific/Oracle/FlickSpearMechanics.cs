namespace Unbound
{
    internal class FlickSpearMechanics
    {
        public static void Init()
        {
            On.Player.CanBeSwallowed += CanEatSpear;
            On.Player.SwallowObject += FlickSpitupSpear;
            // causes flicker to successfully spawn a rotspear

            On.Spear.DrawSprites += DrawRotsprites;
            On.Spear.Update += UpdateRotspear;
            On.Spear.PlaceInRoom += PlaceInWorld;
            On.Weapon.NewRoom += RotSpearNewRoom;
            On.Spear.InitiateSprites += InitRotSpearSprites;
            // visuals
        }

        private static void InitRotSpearSprites(On.Spear.orig_InitiateSprites orig, Spear self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.room.world != null && self.abstractPhysicalObject != null &&
                self != null && sLeaser != null && rCam != null && self.room != null && self.abstractSpear != null &&
                self.abstractSpear.GetOracleSpear().IsRotten)
            {
                try
                {
                    if (self.stuckIns != null)
                    {
                        rCam.ReturnFContainer("HUD").AddChild(self.stuckIns.label);
                    }
                    sLeaser.sprites = new FSprite[4];

                    sLeaser.sprites[3] = new FSprite("JetFishEyeB", true);
                    // inside of bulb

                    sLeaser.sprites[0] = new FSprite("DangleFruit0B", true);
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Error making base Rotspear sprites: " + e);
                }

                try
                {
                    UnityEngine.Random.State state = UnityEngine.Random.state;
                    UnityEngine.Random.InitState(self.abstractPhysicalObject.ID.RandomSeed);
                    self.abstractSpear.GetOracleSpear().dangler = new Vector2[UnityEngine.Random.Range(4, UnityEngine.Random.Range(3, 5)), 6];
                    UnityEngine.Random.state = state;
                    // initiating random state for the dangler length
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Error setting Rotspear random state: " + e);
                }

                try {
                    sLeaser.sprites[2] = TriangleMesh.MakeLongMesh(self.abstractSpear.GetOracleSpear().dangler.GetLength(0), false, false);
                    sLeaser.sprites[2].shader = rCam.game.rainWorld.Shaders["JaggedCircle"];
                    sLeaser.sprites[2].alpha = rCam.game.SeededRandom(self.abstractPhysicalObject.ID.RandomSeed);
                    // dangler

                    sLeaser.sprites[1] = new FSprite(ModManager.MSC ? "FireBugSpear" : "SmallSpear", true);
                    // spear itself, should stay at bottom
                }
                catch (Exception e) 
                {
                    NCRDebug.Log("Error setting Rotspear long mesh: " + e);
                }

                try 
                {
                    self.AddToContainer(sLeaser, rCam, null);
                }
                catch (Exception e)
                {
                    NCRDebug.Log("Error adding sprites to container: " + e);
                }
            }
        }

        private static void RotSpearNewRoom(On.Weapon.orig_NewRoom orig, Weapon self, Room newRoom)
        {
            orig(self, newRoom);
            if (self != null && self.abstractPhysicalObject != null && newRoom != null &&
                self is Spear && (self as Spear).abstractSpear != null && (self as Spear).abstractSpear.GetOracleSpear().IsRotten)
            {
                ResetDanglers(self as Spear);
            }
        }

        public static void ResetDanglers(Spear self)
        {
            Vector2 vecasTwo = Vector3.Slerp(self.lastRotation, self.rotation, 1f) * 15f;
            Vector2 vector = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, 1f) + vecasTwo;
            for (int i = 0; i < self.abstractSpear.GetOracleSpear().dangler.GetLength(0); i++)
            {
                self.abstractSpear.GetOracleSpear().dangler[i, 0] = vector;
                self.abstractSpear.GetOracleSpear().dangler[i, 1] = vector;
                self.abstractSpear.GetOracleSpear().dangler[i, 2] *= 0f;
            }
        }

        private static void PlaceInWorld(On.Spear.orig_PlaceInRoom orig, Spear self, Room placeRoom)
        {
            orig(self, placeRoom);
            if (self.abstractSpear.GetOracleSpear().IsRotten)
            {
                ResetDanglers(self);
            }
        }

        private static void UpdateRotspear(On.Spear.orig_Update orig, Spear self, bool eu)
        {
            orig(self, eu);
            if (self != null && self.abstractSpear != null && self.abstractSpear.GetOracleSpear().IsRotten) 
            {
                Vector2[,] dangler = self.abstractSpear.GetOracleSpear().dangler;
                for (int i = 0; i < dangler.GetLength(0); i++)
                {
                    float t = (float)i / (float)(dangler.GetLength(0) - 1);
                    dangler[i, 1] = dangler[i, 0];
                    dangler[i, 0] += dangler[i, 2];
                    dangler[i, 2] -= self.rotation * Mathf.InverseLerp(1f, 0f, (float)i) * 0.8f;
                    dangler[i, 4] = dangler[i, 3];
                    dangler[i, 3] = (dangler[i, 3] + dangler[i, 5] *
                        Custom.LerpMap(Vector2.Distance(dangler[i, 0], dangler[i, 1]), 1f, 18f, 0.05f, 0.3f)).normalized;
                    dangler[i, 5] = (dangler[i, 5] + Custom.RNV() * UnityEngine.Random.value *
                        Mathf.Pow(Mathf.InverseLerp(1f, 18f, Vector2.Distance(dangler[i, 0], dangler[i, 1])), 0.3f)).normalized;
                    if (self.room.PointSubmerged(dangler[i, 0]))
                    {
                        dangler[i, 2] *= Custom.LerpMap(dangler[i, 2].magnitude, 1f, 10f, 1f, 0.5f, Mathf.Lerp(1.4f, 0.4f, t));
                        dangler[i, 2].y += 0.05f;
                        dangler[i, 2] += Custom.RNV() * 0.1f;
                    }
                    else
                    {
                        dangler[i, 2] *= Custom.LerpMap(Vector2.Distance(dangler[i, 0], dangler[i, 1]), 1f, 6f, 0.999f, 0.7f, Mathf.Lerp(1.5f, 0.5f, t));
                        dangler[i, 2].y -= self.room.gravity * Custom.LerpMap(Vector2.Distance(dangler[i, 0], dangler[i, 1]), 1f, 6f, 0.6f, 0f);
                        if (i % 3 == 2 || i == dangler.GetLength(0) - 1)
                        {
                            SharedPhysics.TerrainCollisionData terrainCollisionData =
                                self.abstractSpear.GetOracleSpear().scratchTerrainCollisionData.Set(dangler[i, 0], dangler[i, 1],
                                dangler[i, 2], 1f, new IntVector2(0, 0), false);
                            terrainCollisionData = SharedPhysics.HorizontalCollision(self.room, terrainCollisionData);
                            terrainCollisionData = SharedPhysics.VerticalCollision(self.room, terrainCollisionData);
                            terrainCollisionData = SharedPhysics.SlopesVertically(self.room, terrainCollisionData);
                            dangler[i, 0] = terrainCollisionData.pos;
                            dangler[i, 2] = terrainCollisionData.vel;
                            if (terrainCollisionData.contactPoint.x != 0)
                            {
                                dangler[i, 2].y *= 0.6f;
                            }
                            if (terrainCollisionData.contactPoint.y != 0)
                            {
                                dangler[i, 2].x *= 0.6f;
                            }
                        }
                    }
                }
                for (int j = 0; j < dangler.GetLength(0); j++)
                {
                    if (j > 0)
                    {
                        Vector2 normalized = (dangler[j, 0] - dangler[j - 1, 0]).normalized;
                        float distanceBetweenPoints = Vector2.Distance(dangler[j, 0], dangler[j - 1, 0]);
                        float d = (distanceBetweenPoints > 7f) ? 0.5f : 0.25f;
                        dangler[j, 0] += normalized * (7f - distanceBetweenPoints) * d;
                        dangler[j, 2] += normalized * (7f - distanceBetweenPoints) * d;
                        dangler[j - 1, 0] -= normalized * (7f - distanceBetweenPoints) * d;
                        dangler[j - 1, 2] -= normalized * (7f - distanceBetweenPoints) * d;
                        if (j > 1)
                        {
                            normalized = (dangler[j, 0] - dangler[j - 2, 0]).normalized;
                            dangler[j, 2] += normalized * 0.2f;
                            dangler[j - 2, 2] -= normalized * 0.2f;
                        }
                        if (j < dangler.GetLength(0) - 1)
                        {
                            dangler[j, 3] = Vector3.Slerp(dangler[j, 3], (dangler[j - 1, 3] * 2f + dangler[j + 1, 3]) / 3f, 0.1f);
                            dangler[j, 5] = Vector3.Slerp(dangler[j, 5], (dangler[j - 1, 5] * 2f + dangler[j + 1, 5]) / 3f,
                                Custom.LerpMap(Vector2.Distance(dangler[j, 1], dangler[j, 0]), 1f, 8f, 0.05f, 0.5f));
                        }
                    }
                    else
                    {
                        Vector2 vecasTwo = Vector3.Slerp(self.lastRotation, self.rotation, 1f) * 15f;
                        dangler[j, 0] = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, 1f) + vecasTwo;
                        dangler[j, 2] *= 0f;
                    }
                }
            }
        }

        private static void DrawRotsprites(On.Spear.orig_DrawSprites orig, Spear self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self != null && self.room != null && self.abstractSpear != null &&
                self.abstractSpear.GetOracleSpear().IsRotten)
            {
                Vector2 firsttoLastChunk = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, timeStacker);
                Vector3 v = Vector3.Slerp(self.lastRotation, self.rotation, timeStacker);
                if (self.vibrate > 0)
                {
                    firsttoLastChunk += Custom.DegToVec(UnityEngine.Random.value * 360f) * 2f * UnityEngine.Random.value;
                }

                sLeaser.sprites[3].x = firsttoLastChunk.x - camPos.x;
                sLeaser.sprites[3].y = firsttoLastChunk.y - camPos.y;
                sLeaser.sprites[3].anchorY = Mathf.Lerp(self.lastPivotAtTip ? 0.85f : 0.5f, self.pivotAtTip ? 0.85f : 0.5f, timeStacker);
                sLeaser.sprites[3].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), v);

                for (int i = 0; i <= 1; i++)
                {
                    sLeaser.sprites[i].x = firsttoLastChunk.x - camPos.x;
                    sLeaser.sprites[i].y = firsttoLastChunk.y - camPos.y;
                    sLeaser.sprites[i].anchorY = Mathf.Lerp(self.lastPivotAtTip ? 0.85f : 0.5f, self.pivotAtTip ? 0.85f : 0.5f, timeStacker);
                    sLeaser.sprites[i].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), v);
                }

                sLeaser.sprites[1].color = self.color;
                sLeaser.sprites[2].color = self.color;

                self.abstractSpear.GetOracleSpear().goldcol = Color.Lerp(RainWorld.SaturatedGold, rCam.currentPalette.blackColor, 0.1f + 0.8f *
                    rCam.currentPalette.darkness);

                sLeaser.sprites[3].color = self.abstractSpear.GetOracleSpear().goldcol;
                sLeaser.sprites[0].anchorY -= 0.4f;
                sLeaser.sprites[3].anchorY -= 0.45f;

                if (self.blink > 0)
                {
                    if (self.blink > 1 && UnityEngine.Random.value < 0.5f)
                    {
                        sLeaser.sprites[1].color = new Color(1f, 1f, 1f);
                    }
                    else
                    {
                        sLeaser.sprites[1].color = self.color;
                    }
                }
                else if (sLeaser.sprites[1].color != self.color)
                {
                    sLeaser.sprites[1].color = self.color;
                }
                if (self.mode == Weapon.Mode.Free && self.firstChunk.ContactPoint.y < 0)
                {
                    sLeaser.sprites[0].anchorY += 0.2f;
                }

                float num = 0f;


                Vector2 vecasTwo = Vector3.Slerp(self.lastRotation, self.rotation, timeStacker) * 15f;
                Vector2 a = Vector2.Lerp(self.firstChunk.lastPos, self.firstChunk.pos, timeStacker) + vecasTwo;
                for (int i = 0; i < self.abstractSpear.GetOracleSpear().dangler.GetLength(0); i++)
                {
                    float f = (float)i / (float)(self.abstractSpear.GetOracleSpear().dangler.GetLength(0) - 1);
                    Vector2 vector = Vector2.Lerp(self.abstractSpear.GetOracleSpear().dangler[i, 1], self.abstractSpear.GetOracleSpear().dangler[i, 0], 
                        timeStacker);
                    float num2 = (2f + 2f * Mathf.Sin(Mathf.Pow(f, 2f) * 3.1415927f)) * Vector3.Slerp(self.abstractSpear.GetOracleSpear().dangler[i, 4], 
                        self.abstractSpear.GetOracleSpear().dangler[i, 3], timeStacker).x;
                    Vector2 normalized = (a - vector).normalized;
                    Vector2 a2 = Custom.PerpendicularVector(normalized);
                    float d = Vector2.Distance(a, vector) / 5f;
                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4, a - normalized * d - a2 * (num2 + num) * 0.5f - camPos);
                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 1, a - normalized * d + a2 * (num2 + num) * 0.5f - camPos);
                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 2, vector + normalized * d - a2 * num2 - camPos);
                    (sLeaser.sprites[2] as TriangleMesh).MoveVertice(i * 4 + 3, vector + normalized * d + a2 * num2 - camPos);
                    a = vector;
                    num = num2;
                }
            }
        }

        private static void FlickSpitupSpear(On.Player.orig_SwallowObject orig, Player self, int grasp)
        {
            orig(self, grasp);
            if (self != null && self.GetNCRunbound().IsOracle &&
                self.objectInStomach is AbstractSpear) 
            {
                NCRDebug.Log("Oracle swallow spear!");
                self.SubtractFood(2);
                self.objectInStomach = null;

                AbstractSpear abstractSpear = new AbstractSpear(self.room.world, 
                    null, self.abstractCreature.pos, self.room.game.GetNewID(), false);
                abstractSpear.GetOracleSpear().IsRotten = true;
                self.room.abstractRoom.AddEntity(abstractSpear);
                abstractSpear.RealizeInRoom();
                if (self.FreeHand() != -1)
                {
                    self.SlugcatGrab(abstractSpear.realizedObject, self.FreeHand());
                }
            }
        }

        private static bool CanEatSpear(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.GetNCRunbound().IsOracle)
            {
                return (orig(self, testObj) || testObj is Spear && (self.FoodInStomach > 2));
            }
            return orig(self, testObj);
        }
    }
}
