using Expedition;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HUD;

namespace Unbound
{
    internal class UnbExpedition
    {
        public static void Update(On.Expedition.PearlDeliveryChallenge.orig_Update orig, PearlDeliveryChallenge self)
        {
            try
            {
                if (!(self.game == null || self.completed) && self != null &&
                (ExpeditionData.slugcatPlayer == UnboundEnums.NCRUnbound || ExpeditionData.slugcatPlayer.value == "NCRunbound"))
                {
                    #region Base.Update
                    if (self.revealCheckDelay < 100)
                    {
                        self.revealCheckDelay++;
                    }
                    if (self.hidden && !self.revealCheck && self.revealCheckDelay >= 100)
                    {
                        self.revealCheck = true;
                        self.CheckRevealable();
                    }
                    #endregion
                    if (self.iterator != 1)
                    {
                        self.iterator = 1;
                    }
                    for (int i = 0; i < self.game.Players.Count; i++)
                    {
                        if (self.game.Players[i] != null && self.game.Players[i].realizedCreature != null &&
                            self.game.Players[i].realizedCreature.room != null &&
                            self.game.Players[i].realizedCreature.room.abstractRoom.name == "SS_AI")
                        {
                            for (int j = 0; j < self.game.Players[i].realizedCreature.room.updateList.Count; j++)
                            {
                                if (self.game.Players[i].realizedCreature.room.updateList[j] is DataPearl &&
                                    ChallengeTools.ValidRegionPearl(self.region,
                                    (self.game.Players[i].realizedCreature.room.updateList[j] as DataPearl).AbstractPearl.dataPearlType) &&
                                    (self.game.Players[i].realizedCreature.room.updateList[j] as DataPearl).firstChunk.pos.x > 0f)
                                {
                                    self.CompleteChallenge();
                                }
                            }
                        }
                    }
                }
                else
                {
                    orig(self);
                }
            }
            catch (Exception e) { NCRDebug.Log("Error updating Pearlcollection for Unbound: " + e); }
        }

        public static string UnbRandomStarts(On.Expedition.ExpeditionGame.orig_ExpeditionRandomStarts orig, RainWorld rainWorld, SlugcatStats.Name slug)
        {
            if (slug == UnboundEnums.NCRUnbound || slug.value == "NCRunbound")
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
                List<string> list2 = SlugcatStats.SlugcatStoryRegions(slug);
                if (File.Exists(AssetManager.ResolveFilePath("randomstarts.txt")))
                {
                    string[] array = File.ReadAllLines(AssetManager.ResolveFilePath("randomstarts.txt"));
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (!array[i].StartsWith("//") && array[i].Length > 0)
                        {
                            string text = Regex.Split(array[i], "_")[0];
                            if (!(ExpeditionGame.lastRandomRegion == text))
                            {
                                if (!dictionary2.ContainsKey(text))
                                {
                                    dictionary2.Add(text, new List<string>());
                                }
                                if (list2.Contains(text))
                                {
                                    dictionary2[text].Add(array[i]);
                                }
                                else if (ModManager.MSC)
                                {
                                    if (text == "OE" && ExpeditionGame.unlockedExpeditionSlugcats.Contains(MoreSlugcatsEnums.SlugcatStatsName.Gourmand))
                                    {
                                        dictionary2[text].Add(array[i]);
                                    }
                                    if (text == "LC" && ExpeditionGame.unlockedExpeditionSlugcats.Contains(MoreSlugcatsEnums.SlugcatStatsName.Artificer))
                                    {
                                        dictionary2[text].Add(array[i]);
                                    }
                                    if (text == "MS" && ExpeditionGame.unlockedExpeditionSlugcats.Contains(MoreSlugcatsEnums.SlugcatStatsName.Rivulet))
                                    {
                                        dictionary2[text].Add(array[i]);
                                    }
                                }
                                if (dictionary2[text].Contains(array[i]) && !dictionary.ContainsKey(text))
                                {
                                    dictionary.Add(text, ExpeditionGame.GetRegionWeight(text));
                                }
                            }
                        }
                    }


                    System.Random random = new System.Random();
                    int maxValue = dictionary.Values.Sum();
                    int randomIndex = random.Next(0, maxValue);
                    string key = dictionary.First(delegate (KeyValuePair<string, int> x)
                    {
                        randomIndex -= x.Value;
                        return randomIndex < 0;
                    }).Key;
                    ExpeditionGame.lastRandomRegion = key;
                    int num = (from list in dictionary2.Values
                               select list.Count).Sum();
                    string text2 = dictionary2[key].ElementAt(UnityEngine.Random.Range(0, dictionary2[key].Count - 1));


                    int roomrand = UnityEngine.Random.Range(1, 100);

                    

                    if (roomrand == 1) { text2 = "SU_A12"; }
                    else if (roomrand == 2) { text2 = "SU_A40"; }
                    else if (roomrand == 3) { text2 = "SU_S05"; }
                    else if (roomrand == 4 && ModManager.MSC) { text2 = "MS_CORE"; }
                    else if (roomrand == 5 && ModManager.MSC) { text2 = "MS_AIR01"; }
                    else if (roomrand == 6 && ModManager.MSC) { text2 = "MS_bitteraerie6"; }
                    else if (roomrand == 7 && ModManager.MSC) { text2 = "MS_bitteraerie5"; }
                    else if (roomrand == 8 && ModManager.MSC) { text2 = "MS_DMU04"; }
                    else if (roomrand == 9) { text2 = "SL_LMBRIDE01"; }
                    else if (roomrand == 10) { text2 = "SL_LMC01"; }
                    else if (roomrand == 11 && ModManager.MSC) { text2 = "SL_WALL06"; }

                    // NO MSC
                    else if (roomrand == 4 && !ModManager.MSC) { text2 = "SS_LAB13"; }
                    else if (roomrand == 5 && !ModManager.MSC) { text2 = "SS_S05"; }
                    else if (roomrand == 6 && !ModManager.MSC) { text2 = "LF_A03"; }
                    else if (roomrand == 7 && !ModManager.MSC) { text2 = "LF_A17"; }
                    else if (roomrand == 8 && !ModManager.MSC) { text2 = "UW_A05"; }
                    else if (roomrand == 11 && !ModManager.MSC) { text2 = "SL_LMS06"; }


                    return text2;
                }
            }
            return (orig(rainWorld, slug));
        }

        public static bool Invalid(On.Expedition.NeuronDeliveryChallenge.orig_ValidForThisSlugcat orig, NeuronDeliveryChallenge self,
            SlugcatStats.Name slugcat)
        {
            if (slugcat == UnboundEnums.NCRUnbound || slugcat.value == "NCRunbound")
            {
                return false;
            }

            return orig(self, slugcat);
        }

        public static void Description(On.Expedition.PearlDeliveryChallenge.orig_UpdateDescription orig, PearlDeliveryChallenge self)
        {
            try
            {
                if (self != null &&
                    (ExpeditionData.slugcatPlayer == UnboundEnums.NCRUnbound || ExpeditionData.slugcatPlayer.value == "NCRunbound"))
                {
                    self.description = ChallengeTools.IGT.Translate("<region> pearl delivered to Five Pebbles").Replace("<region>",
                        ChallengeTools.IGT.Translate(Region.GetRegionFullName(self.region, ExpeditionData.slugcatPlayer)));

                    #region Base.Update
                    if (self.hidden && !self.revealed)
                    {
                        self.description = ChallengeTools.IGT.Translate("HIDDEN");
                        return;
                    }
                    if (self.game != null && self.game.cameras != null && self.game.cameras[0].hud != null)
                    {
                        for (int i = 0; i < self.game.cameras[0].hud.parts.Count; i++)
                        {
                            if (self.game.cameras[0].hud.parts[i] is ExpeditionHUD)
                            {
                                (self.game.cameras[0].hud.parts[i] as ExpeditionHUD).pendingUpdates = true;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    orig(self);
                }
            }
            catch (Exception e) { NCRDebug.Log("Error tweaking description for Pearl Delivery challenge: " + e); }
        }
    }
}