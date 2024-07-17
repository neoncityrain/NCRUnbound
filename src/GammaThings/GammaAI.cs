using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverseerHolograms;
using UnityEngine;

namespace Unbound
{
    internal class GammaAI
    {
        public static void Init()
        {
            On.Overseer.TryAddHologram += Overseer_TryAddHologram;
            On.OverseerAbstractAI.RoomAllowed += OverseerAbstractAI_RoomAllowed;
            On.OverseerCommunicationModule.FoodDelicousScore += OverseerCommunicationModule_FoodDelicousScore;
        }

        private static float OverseerCommunicationModule_FoodDelicousScore(On.OverseerCommunicationModule.orig_FoodDelicousScore orig, OverseerCommunicationModule self, AbstractPhysicalObject foodObject, Player player)
        {
            if (self.overseerAI.overseer.room.world.game.session.characterStats.name.value == "NCRunbound")
            {
                if (foodObject == null || foodObject.realizedObject == null || foodObject.Room != player.abstractCreature.Room ||
                    foodObject.slatedForDeletion)
                {
                    return 0f;
                }
                if (foodObject.type != AbstractPhysicalObject.AbstractObjectType.DangleFruit &&
                    foodObject.type != AbstractPhysicalObject.AbstractObjectType.JellyFish &&
                    foodObject.type != AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer)
                {
                    return 0f;
                }
                float num = Mathf.InverseLerp(1100f, 400f, Vector2.Distance(foodObject.realizedObject.firstChunk.pos, player.DangerPos));
                if (num == 0f)
                {
                    return 0f;
                }
                if (self.GuideState.itemTypes.Contains(foodObject.type))
                {
                    if (num <= 0.2f || !self.room.ViewedByAnyCamera(foodObject.realizedObject.firstChunk.pos, 0f))
                    {
                        return 0f;
                    }
                    num = 0.3f;
                }
                for (int i = 0; i < self.objectsAlreadyTalkedAbout.Count; i++)
                {
                    if (self.objectsAlreadyTalkedAbout[i] == foodObject.ID)
                    {
                        return 0f;
                    }
                }
                if (foodObject == self.mostDeliciousFoodInRoom &&
                    self.currentConcern == OverseerCommunicationModule.PlayerConcern.FoodItemInRoom)
                {
                    num *= 1.1f;
                }
                return num * Mathf.Lerp(self.GeneralPlayerFoodNeed(player), 0.6f, 0.5f);
            }
            else return orig(self, foodObject, player);
        }

        private static bool OverseerAbstractAI_RoomAllowed(On.OverseerAbstractAI.orig_RoomAllowed orig, OverseerAbstractAI self, int room)
        {
            if (self.world.game.session.characterStats.name.value == "NCRunbound" && self.playerGuide)
            {
                if (room < self.world.firstRoomIndex || room >= self.world.firstRoomIndex + self.world.NumberOfRooms)
                {
                    return false;
                }
                for (int i = 0; i < OverseerAbstractAI.tutorialRooms.Length; i++)
                {
                    if (self.world.GetAbstractRoom(room).name == OverseerAbstractAI.tutorialRooms[i])
                    {
                        return true;
                    }
                }
                return (self.world.region.name == "MS" || self.world.GetAbstractRoom(room).gate ||
                    // can always show up in MS, can enter gate rooms
                    !(self.world.GetAbstractRoom(room).AttractionForCreature(self.parent.creatureTemplate.type) ==
                    AbstractRoom.CreatureRoomAttraction.Forbidden) ||
                    // if the room is not forbidden
                    !self.world.GetAbstractRoom(room).scavengerOutpost || !self.world.GetAbstractRoom(room).scavengerTrader) ||
                    // if the room is not a scav outpost / scav trader
                    self.world.GetAbstractRoom(room).shelter;
                // or if the room IS a shelter (enabling the guide to come inside the shelter with the player)
            }
            return orig(self, room);
        }

        private static void Overseer_TryAddHologram(On.Overseer.orig_TryAddHologram orig, Overseer self, OverseerHolograms.OverseerHologram.Message message, Creature communicateWith, float importance)
        {
            if (self.room.game.session.characterStats.name.value == "NCRunbound" && self.PlayerGuide)
            {
                if (self.dead)
                {
                    return;
                }
                // dont show holograms if dead
                if (self.room != null)
                {
                    if (self.room.abstractRoom.name == "SS_AI")
                    {
                        return;
                    }
                    // dont show holograms in pebbles' chamber. this is initially only for MSC- should not trigger for UB either
                }
                if (self.hologram != null)
                {
                    if (self.hologram.message == message)
                    {
                        return;
                    }
                    if (self.hologram.importance >= importance && importance != 3.4028235E+38f)
                    {
                        return;
                    }
                    self.hologram.stillRelevant = false;
                    self.hologram = null;
                    // removes unnecessary holograms
                }
                if (self.room == null)
                {
                    return;
                    // dont show holograms if not in a room
                }
                // ordinarily the tutorial holograms are here. this mod goes with the assumption that the player knows how to play,
                // so those are removed
                if (message == OverseerHologram.Message.Angry)
                {
                    self.hologram = new AngryHologram(self, message, communicateWith, importance);
                }
                // this is moved to the top, as it is the highest priority.
                else if (message == OverseerHologram.Message.DangerousCreature)
                {
                    self.hologram = new OverseerHologram.CreaturePointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.Shelter)
                {
                    self.hologram = new OverseerHologram.ShelterPointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.Bats)
                {
                    self.hologram = new OverseerHologram.BatPointer(self, message, communicateWith, importance);
                }

                else if (message == OverseerHologram.Message.FoodObject)
                {
                    self.hologram = new OverseerHologram.FoodPointer(self, message, communicateWith, importance);
                }

                else
                {
                    return;
                    // return if it doesnt fit the above (i.e., if its a tutorial or progression direction)
                }
                self.room.AddObject(self.hologram);
            }
            else
            {
                orig(self, message, communicateWith, importance);
            }
        }
    }
}
