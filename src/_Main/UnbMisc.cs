using System;
using RWCustom;
using UnityEngine;

namespace Unbound
{
    internal class UnbMisc
    {
        public static void Init()
        {
            On.MoreSlugcats.StowawayBugState.AwakeThisCycle += StowawayBugState_AwakeThisCycle;
            // rerolls if a stowaway is awake or not. it should result in a bit over a 1/3 chance that it will be awake each cycle

            On.Player.Jump += Player_Jump;
            // increases unbounds base jump by 1f

            On.SeedCob.HitByWeapon += SeedCob_HitByWeapon;
            // SEED COB ALLERGYYYYYY

            On.Player.UpdateAnimation += Player_UpdateAnimation;
            // swim speed code

            On.GhostWorldPresence.SpawnGhost += GhostWorldPresence_SpawnGhost;
            // fixes being unable to encounter echos under 5 karma, since unbound has a max of 3 initially

            On.Player.Grabability += Player_Grabability;
            // prevents taking neurons from moon

            On.Player.CanBeSwallowed += Player_CanBeSwallowed;
            // cannot swallow items

            On.LizardAI.IUseARelationshipTracker_UpdateDynamicRelationship += LizardAI_IUseARelationshipTracker_UpdateDynamicRelationship;
            // cyans consider unbound to be a cyan / are territorial rather than aggressive as long as hes alive
            // keeps them a bit more aggro than they are to one another BUT its not eating him so shrug

            On.Centipede.Shock += Centipede_Shock;
            // centishock resistance
        }

        private static void Centipede_Shock(On.Centipede.orig_Shock orig, Centipede self, PhysicalObject shockObj)
        {
            if (self != null && self.room != null && shockObj != null &&
                shockObj is Creature && (shockObj is Player && (shockObj as Player).GetCat().IsUnbound))
            {
                self.room.PlaySound(SoundID.Centipede_Shock, self.mainBodyChunk.pos);
                if (self.graphicsModule != null)
                {
                    (self.graphicsModule as CentipedeGraphics).lightFlash = 1f;
                    for (int i = 0; i < (int)Mathf.Lerp(4f, 8f, self.size); i++)
                    {
                        self.room.AddObject(new Spark(self.HeadChunk.pos, Custom.RNV() * Mathf.Lerp(4f, 14f, UnityEngine.Random.value),
                            new Color(0.9f, 0.7f, 1f), null, 8, 14));
                    }
                }
                for (int j = 0; j < self.bodyChunks.Length; j++)
                {
                    self.bodyChunks[j].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    self.bodyChunks[j].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                for (int k = 0; k < shockObj.bodyChunks.Length; k++)
                {
                    shockObj.bodyChunks[k].vel += Custom.RNV() * 6f * UnityEngine.Random.value;
                    shockObj.bodyChunks[k].pos += Custom.RNV() * 6f * UnityEngine.Random.value;
                }
                if (shockObj is Creature)
                {
                    if (self.Small)
                    {
                        (shockObj as Creature).Stun(60);
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, false, (shockObj as Creature).stun));
                        (shockObj as Creature).LoseAllGrasps();
                    }
                    else if (self.Red)
                    {
                        (shockObj as Creature).Die();
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, 200));
                        (shockObj as Creature).LoseAllGrasps();
                    }
                    else if (shockObj.TotalMass < self.TotalMass)
                    {
                        (shockObj as Player).playerState.permanentDamageTracking += self.size;
                        if ((shockObj as Player).playerState.permanentDamageTracking >= 1)
                        {
                            (shockObj as Player).Die();
                        }

                        (shockObj as Creature).Stun((int)Custom.LerpMap(shockObj.TotalMass, 0f, self.TotalMass * 2f, 300f, 30f));
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, (shockObj as Creature).stun));

                        self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
                        self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
                        self.Stun((shockObj as Creature).stun + 3);
                        self.LoseAllGrasps();
                    }
                    else
                    {
                        (shockObj as Player).playerState.permanentDamageTracking += self.TotalMass - shockObj.TotalMass;
                        if ((shockObj as Player).playerState.permanentDamageTracking >= 1)
                        {
                            (shockObj as Player).Die();
                        }

                        (shockObj as Creature).Stun((int)Custom.LerpMap(shockObj.TotalMass, 0f, self.TotalMass * 2f, 300f, 30f));
                        self.room.AddObject(new CreatureSpasmer(shockObj as Creature, true, (shockObj as Creature).stun));

                        self.shockGiveUpCounter = Math.Max(self.shockGiveUpCounter, 30);
                        self.AI.annoyingCollisions = Math.Min(self.AI.annoyingCollisions / 2, 150);
                        self.Stun((shockObj as Creature).stun + 3);
                        self.LoseAllGrasps();
                    }
                }
                if (shockObj.Submersion > 0f)
                {
                    self.room.AddObject(new UnderwaterShock(self.room, self,
                        self.HeadChunk.pos, 14, Mathf.Lerp(ModManager.MMF ? 0f : 200f, 1200f, self.size),
                        0.2f + 1.9f * self.size, self, new Color(0.9f, 0.7f, 1f)));
                }
            }
            else
            {
                orig(self, shockObj);
            }
        }

        private static CreatureTemplate.Relationship LizardAI_IUseARelationshipTracker_UpdateDynamicRelationship(On.LizardAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, LizardAI self, RelationshipTracker.DynamicRelationship dRelation)
        {

            if (self != null && dRelation != null && self.creature != null &&
                dRelation.trackerRep.representedCreature.realizedCreature != null && dRelation.state != null &&
                // making sure things arent null
                self.creature.creatureTemplate.type == CreatureTemplate.Type.CyanLizard &&
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetCat().IsUnbound &&
                // if cyan, if unbound
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature
                // should still allow making friends with it
                )
            {
                return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.AgressiveRival, 0.15f);
            }
            return orig(self, dRelation);
        }

        private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (self != null && self.room != null && obj != null &&
                self.GetCat().IsUnbound)
            {
                if (obj is SLOracleSwarmer)
                {
                    return Player.ObjectGrabability.CantGrab;
                }
            }
            return orig(self, obj);
        }

        private static bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (!self.GetCat().Unpicky &&
                self != null && self.room != null && testObj != null &&
                self.GetCat().IsUnbound)
            {
                return false;
            }
            else return orig(self, testObj);
        }

        private static void Player_UpdateAnimation(On.Player.orig_UpdateAnimation orig, Player self)
        {
            // swimming code
            orig(self);
            if (self != null && self.room != null &&
                self.GetCat().IsUnbound)
            {
                if (!self.submerged && !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious) && self.waterFriction >= 0.7f)
                {
                    self.waterFriction -= 0.1f;
                }
                else if (self.submerged && self.waterFriction >= 0.7f &&
                    !(self.grasps[0] != null && self.grasps[0].grabbed is JetFish &&
                    (self.grasps[0].grabbed as JetFish).Consious))
                {
                    self.waterFriction -= 0.05f;
                }
            }
        }

        private static bool GhostWorldPresence_SpawnGhost(On.GhostWorldPresence.orig_SpawnGhost orig, GhostWorldPresence.GhostID ghostID, int karma, int karmaCap, int ghostPreviouslyEncountered, bool playingAsRed)
        {
            if (ghostID != null &&
                Custom.rainWorld.progression.PlayingAsSlugcat.value == "NCRunbound" &&
                !(ModManager.Expedition && Custom.rainWorld.ExpeditionMode && Custom.rainWorld.progression.currentSaveState.cycleNumber == 0)
                && !Custom.rainWorld.safariMode && karmaCap < 4 && ghostPreviouslyEncountered < 0f)
            {
                // unbound under karma cap 5, allowing echos anyway
                return karma >= karmaCap;
                // ...ASSUMING theyre at max karma out of their possible karma.
            }
            else return orig(ghostID, karma, karmaCap, ghostPreviouslyEncountered, playingAsRed);
        }

        private static void SeedCob_HitByWeapon(On.SeedCob.orig_HitByWeapon orig, SeedCob self, Weapon weapon)
        {
            if (self != null &&
                !(weapon == null || self.room == null || self.room.roomSettings == null) &&
                self.room.game.session.characterStats.name.value == "NCRunbound" && ModManager.MSC)
            {
                if (self.room.roomSettings.DangerType == MoreSlugcats.MoreSlugcatsEnums.RoomRainDangerType.Blizzard &&
                    weapon.firstChunk.vel.magnitude < 20f)
                {
                    if (UnityEngine.Random.Range(0.5f, 0.8f) < self.freezingCounter)
                    {
                        self.spawnUtilityFoods();
                    }
                    return;
                }
                if (weapon.thrownBy != null && weapon.thrownBy is Player && ((weapon.thrownBy as Player).slugcatStats.name ==
                    MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Spear || (weapon.thrownBy as Player).SlugCatClass ==
                    MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Saint))
                {
                    return;
                }
                self.spawnUtilityFoods();
                return;
            }
            else
            {
                orig(self, weapon);
            }
        }

        private static void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            if (self != null && self.room != null &&
                self.GetCat().IsUnbound)
            {
                self.jumpBoost += 1f;
                // has a jump boost of +1 compared to surv
            }
        }

        private static bool StowawayBugState_AwakeThisCycle(On.MoreSlugcats.StowawayBugState.orig_AwakeThisCycle orig,
            MoreSlugcats.StowawayBugState self, int cycle)
        {
            if (self != null && self.creature != null && self.creature.Room != null &&
                self.creature.world.game.session.characterStats.name.value == "NCRunbound" && ModManager.MSC)
            {
                Debug.Log("Unbound world, rerolling stowawake (because life is a fucking nightmare)");
                System.Random rd = new System.Random();
                int rand_num = rd.Next(1, 3);
                if (rand_num == 1)
                {
                    Debug.Log("Congrats! Stowaway awoken (because life is a fucking nightmare)");
                    return true;
                    // if random number is 1, awaken stowaway
                }
                else
                {
                    Debug.Log("Stowaway state defaulting to normal");
                    return orig(self, cycle);
                    // if the random number isnt 1, refer to the original code
                }
            }
            else return orig(self, cycle);
        }
    }
}
