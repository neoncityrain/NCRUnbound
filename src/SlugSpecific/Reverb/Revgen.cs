using System;
using System.Linq;
using UnityEngine;

namespace Unbound
{
    internal class Revgen
    {
        public static void HookThatThang()
        {
            On.FlareBomb.Update += FlarebombStun;
            On.Water.InitiateSprites += NormalHRWater;
            On.Player.CanBeSwallowed += NoSwallow;

            On.Player.Update += RockSwallow;
            On.Player.Regurgitate += CallForHelp;

            On.LizardAI.IUseARelationshipTracker_UpdateDynamicRelationship += YellowPack;

            On.Player.Grabbed += YellowAnger;
        }

        private static void YellowAnger(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
        {
            orig(self, grasp);
            if (self != null && grasp != null && self.GetNCRunbound().Reverb &&
                (grasp.grabber is Lizard || grasp.grabber is Vulture || grasp.grabber is BigSpider || grasp.grabber is DropBug))
            {
                self.room.PlaySound(ModManager.MMF ? MMFEnums.MMFSoundID.Lizard_Voice_Yellow_A : SoundID.Lizard_Voice_Pink_E,
                    self.mainBodyChunk, false, 0.8f,
                    ModManager.MMF ? UnityEngine.Random.Range(2f, 2.8f) :
                    UnityEngine.Random.Range(1.8f, 2f));
                self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 1f));
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.YellowLizard &&
                        self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        self.room.abstractRoom.creatures[i].realizedCreature.Consious)
                    {
                        var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                        lizard.AI.excitement = 1f;

                        lizard.AI.yellowAI.communicating = 14;
                        lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(self.mainBodyChunk.pos));
                        lizard.voice.MakeSound(LizardVoice.Emotion.BloodLust);
                        lizard.AI.runSpeed = 1f;
                        lizard.AI.agressionTracker.IncrementAnger(lizard.AI.tracker.RepresentationForObject(grasp.grabber, true), 0.4f);
                    }
                }
            }
        }

        private static CreatureTemplate.Relationship YellowPack(On.LizardAI.orig_IUseARelationshipTracker_UpdateDynamicRelationship orig, LizardAI self, RelationshipTracker.DynamicRelationship dRelation)
        {
            if (!(self.friendTracker.giftOfferedToMe != null && self.friendTracker.giftOfferedToMe.active &&
                self.friendTracker.giftOfferedToMe.item == dRelation.trackerRep.representedCreature.realizedCreature) &&
                self.friendTracker.friend != dRelation.trackerRep.representedCreature.realizedCreature &&
                dRelation.trackerRep.representedCreature.creatureTemplate.type == CreatureTemplate.Type.Slugcat &&

                dRelation.trackerRep.VisualContact && dRelation.trackerRep.representedCreature.realizedCreature != null &&

                self.creature.creatureTemplate.type == CreatureTemplate.Type.YellowLizard &&
                dRelation.trackerRep.representedCreature.realizedCreature is Player &&
                (dRelation.trackerRep.representedCreature.realizedCreature as Player).GetNCRunbound().Reverb)
            {
                return new CreatureTemplate.Relationship(CreatureTemplate.Relationship.Type.Pack, 0.1f + self.LikeOfPlayer(dRelation.trackerRep));
            }
            return orig(self, dRelation);
        }

        private static void RockSwallow(On.Player.orig_Update orig, Player self, bool eu)
        {
            if (self != null && self.room != null && self.GetNCRunbound().Reverb && self.objectInStomach == null)
            {
                self.objectInStomach = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.Rock, null, 
                    self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
            }
            orig(self, eu);
        }

        private static void CallForHelp(On.Player.orig_Regurgitate orig, Player self)
        {
            if (self != null && self.room != null && self.abstractCreature != null &&
                self.GetNCRunbound().Reverb)
            {
                self.room.PlaySound(ModManager.MMF ? MMFEnums.MMFSoundID.Lizard_Voice_Yellow_A : SoundID.Lizard_Voice_Pink_E, 
                    self.mainBodyChunk, false, 0.8f,
                    ModManager.MMF ? UnityEngine.Random.Range(2f, 2.8f) :
                    UnityEngine.Random.Range(1.8f, 2f));
                self.room.InGameNoise(new InGameNoise(self.mainBodyChunk.pos, 500f, self, 1f));

                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.YellowLizard && 
                        self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        self.room.abstractRoom.creatures[i].realizedCreature.deaf == 0 &&
                        self.room.abstractRoom.creatures[i].realizedCreature.Consious)
                    {
                        var lizard = self.room.abstractRoom.creatures[i].realizedCreature as Lizard;
                        lizard.AI.excitement = 1f;
                        lizard.AI.yellowAI.communicating = 14;
                        lizard.abstractCreature.abstractAI.SetDestination(self.room.GetWorldCoordinate(self.mainBodyChunk.pos));

                        if (lizard.AI.friendTracker.friend == null)
                        {
                            lizard.AI.LizardPlayerRelationChange(1f, self.abstractCreature);
                        }

                        lizard.voice.MakeSound(LizardVoice.Emotion.Curious);
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        private static bool NoSwallow(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self != null && self.room != null && testObj != null &&
                self.GetNCRunbound().Reverb)
            {
                return false;
            }
            return orig(self, testObj);
        }

        private static void NormalHRWater(On.Water.orig_InitiateSprites orig, Water self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self != null && self.room != null && self.room.game != null && self.room.game.session != null &&
                self.room.game.session.characterStats.name.value == "NCRreverb" &&
                ModManager.MSC && self.room.world.region != null && self.room.world.region.name == "HR")
            {
                sLeaser.sprites[0].shader = self.room.game.rainWorld.Shaders["WaterSurface"];
            }
        }

        private static void FlarebombStun(On.FlareBomb.orig_Update orig, FlareBomb self, bool eu)
        {
            orig(self, eu);
            if (self != null && !self.slatedForDeletetion && self.room != null && self.room.PlayersInRoom != null &&
                self.burning > 0f)
            {
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (self.room.abstractRoom.creatures[i].realizedCreature is Player &&
                        (self.room.abstractRoom.creatures[i].realizedCreature as Player).GetNCRunbound().Reverb &&
                        !self.room.abstractRoom.creatures[i].realizedCreature.dead &&
                        self.room.abstractRoom.creatures[i].realizedCreature != null &&
                        (Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos,
                        self.LightIntensity * 600f) ||
                        Custom.DistLess(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos,
                        self.LightIntensity * 1600f) &&
                        self.room.VisualContact(self.firstChunk.pos, self.room.abstractRoom.creatures[i].realizedCreature.mainBodyChunk.pos)))
                    {
                        self.room.abstractRoom.creatures[i].realizedCreature.stun = 80;
                        self.room.AddObject(new CreatureSpasmer(self.room.abstractRoom.creatures[i].realizedCreature, false, 80));
                    }
                }
            }
        }
    }
}
