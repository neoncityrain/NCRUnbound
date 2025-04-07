namespace Unbound
{
    internal static class UnbCatStats
    {
        public static void Init()
        {
            On.SlugcatStats.HiddenOrUnplayableSlugcat += lockedCats;
            // hide certain slurts
            On.Player.ThrownSpear += SpearthrowTweaks;
            // unbound spearthrow variables

            On.SlugcatStats.AutoGrabBatflys += NoGrabby;
            On.SlugcatStats.getSlugcatName += UnbNameLogging;
            On.SlugcatStats.SlugcatCanMaul += AllowForMauling;
            On.SlugcatStats.SlugcatFoodMeter += UnbFoodMeter;
            On.SlugcatStats.SlugcatStartingKarma += UnbStartKarma;
            // the "standard" slugbase variables

            On.SlugcatStats.SpearSpawnModifier_Timeline_float += UnbSpearSpawn;
            On.SlugcatStats.SpearSpawnExplosiveRandomChance_Timeline += Explosive;
            On.SlugcatStats.SpearSpawnElectricRandomChance_Timeline += Electric;
            // spear modifiers
        }

        private static float Electric(On.SlugcatStats.orig_SpearSpawnElectricRandomChance_Timeline orig, SlugcatStats.Timeline index)
        {
            if (index != null)
            {
                if (ModManager.MSC && (index == UnboundEnums.UnboundTimeline || index.value == "NCRunbound"))
                {
                    return 0.011f;
                }
                else if (ModManager.MSC && (index.value == "NCRtech"))
                {
                    return 0.09f;
                }
            }

            return orig(index);
        }

        private static float Explosive(On.SlugcatStats.orig_SpearSpawnExplosiveRandomChance_Timeline orig, SlugcatStats.Timeline index)
        {
            if (index != null)
            {
                if (index == UnboundEnums.UnboundTimeline || index.value == "NCRunbound")
                {
                    return 0.011f;
                }
                if (index.value == "NCRtech")
                {
                    return 0.013f;
                }
            }

            return orig(index);
        }

        private static float UnbSpearSpawn(On.SlugcatStats.orig_SpearSpawnModifier_Timeline_float orig, SlugcatStats.Timeline index, float originalSpearChance)
        {
            if (index != null)
            {
                if (index == UnboundEnums.UnboundTimeline || index.value == "NCRunbound")
                {
                    return Mathf.Pow(originalSpearChance, 0.825f);
                }
                if (index.value == "NCRoracle")
                {
                    return Mathf.Pow(originalSpearChance, 0.83f);
                }
                if (index.value == "NCRtech")
                {
                    return Mathf.Pow(originalSpearChance, 0.9f);
                }
            }

            return orig(index, originalSpearChance);
        }

        private static int UnbStartKarma(On.SlugcatStats.orig_SlugcatStartingKarma orig, SlugcatStats.Name slugcatNum)
        {
            if (slugcatNum == UnboundEnums.NCRUnbound || slugcatNum.value == "NCRunbound")
            {
                return 2; // the friend karma.
            }
            return orig(slugcatNum);
        }

        private static IntVector2 UnbFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            if (slugcat == UnboundEnums.NCRUnbound || slugcat.value == "NCRunbound")
            {
                return new IntVector2(7, 6);
                // in order, goes "max, min"
                // ideally, this number would change over time...
            }
            return orig(slugcat);
        }

        private static bool AllowForMauling(On.SlugcatStats.orig_SlugcatCanMaul orig, SlugcatStats.Name slugcatNum)
        {
            if (slugcatNum == UnboundEnums.NCRUnbound || slugcatNum.value == "NCRunbound")
            {
                return true;
            }
            return orig(slugcatNum);
        }

        private static void SpearthrowTweaks(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            #region my cringefail son
            if (self?.slugcatStats != null &&
                self.slugcatStats.name.value == "NCRunbound")
            {
                if (self.animation == Player.AnimationIndex.RocketJump && !self.GetNCRunbound().DidTripleCyanJump)
                {
                    // should work during a standard rocket jump as well

                    spear.throwModeFrames = 20; // throws longer
                    spear.spearDamageBonus = 0.7f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
                    BodyChunk spearChunk = spear.firstChunk;
                    spearChunk.vel.x = spearChunk.vel.x * 0.8f;

                    self.animation = Player.AnimationIndex.Flip;
                }
                else if (self.GetNCRunbound().didLongjump && !self.GetNCRunbound().DidTripleCyanJump &&
                    self.canJump == 0
                    )
                {
                    // the double jump code takes priority over this
                    spear.throwModeFrames = 20; // throws longer
                    spear.spearDamageBonus = 0.7f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);

                    self.animation = Player.AnimationIndex.Flip;
                }
                else if (self.animation == Player.AnimationIndex.Flip && self.GetNCRunbound().DidTripleCyanJump &&
                    // if in the flip animation and did a triple jump
                    self.bodyMode != Player.BodyModeIndex.ZeroG &&
                    // to prevent any weird 0g glitches i may have mysteriously missed
                    self.GetNCRunbound().didLongjump
                    // to prevent simply backflipping and getting the boost
                    )
                {
                    if (ModManager.MMF && MMF.cfgUpwardsSpearThrow.Value && spear.setRotation.Value.y == 1f)
                    {
                        // if spear is thrown upwards somehow, uuuh.......... sure ? why not.
                        BodyChunk firstChunk2 = spear.firstChunk;
                        firstChunk2.vel.y = firstChunk2.vel.y * 0.87f;
                    }
                    else
                    {
                        spear.throwModeFrames = 22; // throws even longer
                        spear.spearDamageBonus += 0.3f * Mathf.Pow(UnityEngine.Random.value, 4f);
                        // greater than survival throw
                        BodyChunk spearChunk = spear.firstChunk;
                        spearChunk.vel.y = spearChunk.vel.y * 1.1f; // go faster boy!


                        self.room.AddObject(new UnbJumplight(spearChunk.pos, 0.4f, self));
                        self.room.AddObject(new ShockWave(spearChunk.pos, 50f, 0.07f, 3, false));
                        self.room.PlaySound(SoundID.Cyan_Lizard_Medium_Jump, spearChunk);
                        self.room.InGameNoise(new InGameNoise(spearChunk.pos, 500f, self, 3f));

                        self.animation = Player.AnimationIndex.RocketJump;
                        // set him to rocket jump instead, as the animation index after triplejump is a flip.
                        // this shoooould prevent anybody trying to repeatedly throw spears in the air
                        self.GetNCRunbound().UnbCyanjumpCountdown += (int)self.GetNCRunbound().CyJump1Maximum / 3;
                    }
                }
                else if (self.animation == Player.AnimationIndex.RocketJump && self.GetNCRunbound().DidTripleCyanJump)
                {
                    // as above, for the folks trying to cheese for the boost-

                    spear.throwModeFrames = 5; // good fucking luck
                }
                else
                {
                    if (self.canJump != 0) // if not jumping / is on the ground
                    {
                        spear.throwModeFrames = 17; // shorter than monk, but not by much
                        spear.spearDamageBonus = 0.4f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 3f); // does less damage than monk
                        BodyChunk spearChunk = spear.firstChunk;
                        spearChunk.vel.x = spearChunk.vel.x * 0.75f; // less velocity than monk
                    }
                    else
                    {
                        spear.throwModeFrames = 18; // standard monk distance
                        spear.spearDamageBonus = 0.5f + 0.3f * Mathf.Pow(UnityEngine.Random.value, 3f);
                        // does less damage than monk, but LESS badly than above, as he is in the air
                        BodyChunk spearChunk = spear.firstChunk;
                        spearChunk.vel.x = spearChunk.vel.x * 0.76f; // also less velocity than monk, but again- not as bad
                    }
                }
            }
            #endregion
            else
            {
                orig(self, spear);
            }
        }

        private static bool NoGrabby(On.SlugcatStats.orig_AutoGrabBatflys orig, SlugcatStats.Name slugcatNum)
        {
            if (slugcatNum != null &&
                (slugcatNum == UnboundEnums.NCRUnbound || slugcatNum.value == "NCRunbound"))
            {
                return false;
            }
            return orig(slugcatNum);
        }

        private static bool lockedCats(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (i != null)
            {
                if (i.value == "NCRtech" || i == UnboundEnums.NCRTechnician)
                {
                    return true;
                }
                if (i.value == "NCRoracle" || i == UnboundEnums.NCROracle)
                {
                    return true;
                }
                if (i.value == "NCRreverb" || i == UnboundEnums.NCRReverb)
                {
                    return true;
                }
            }
            
            return orig(i);
        }

        private static string UnbNameLogging(On.SlugcatStats.orig_getSlugcatName orig, SlugcatStats.Name i)
        {
            if (i != null && (i == UnboundEnums.NCRTechnician || i.value == "NCRtech"))
            {
                return "Technician";
            }
            if (i != null && (i == UnboundEnums.NCRUnbound || i.value == "NCRunbound"))
            {
                return "Unbound";
            }
            return orig(i);
        }
    }
}
