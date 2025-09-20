using System.Linq;

namespace Unbound
{
    public class UnbHelperCode
    {

        public static void GamebreakingError(Exception e)
        {
            #region UnityEngine.Log
            NCRDebug.LogException(e);

            NCRDebug.Log("Unbound has run into a potentially gamebreaking error. Error code: " + e);
            #region CheckDLC
            if ((ModManager.MSC || ModManager.Watcher) && !(ModManager.Watcher && ModManager.MSC))
            {NCRDebug.Log("DLC active: " + (ModManager.Watcher ? "Watcher" : "MSC"));}
            else if (ModManager.Watcher && ModManager.MSC)
            {NCRDebug.Log("All DLC is active.");}
            else
            {NCRDebug.Log("No DLC active.");}
            #endregion
            #region Check specific mods
            if (ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "dressmyslugcat"))
            {
                NCRDebug.Log("Dress my Slugcat is an active mod.");
            }
            if (ModManager.ActiveMods.Any((ModManager.Mod mod) => mod.id == "randombuff"))
            {
                NCRDebug.Log("Random Buff is an active mod.");
            }
            #endregion
            NCRDebug.Log("If disabling other mods does not fix this error, please report it to the Github or Steam Workshop.");
            NCRDebug.Log("Do not re-run the game prior to reporting, as all logs regarding the error will be lost.");
            NCRDebug.Log("The more information you give, the better. Please be descriptive in your issue.");
            NCRDebug.Log("Github issue tracker: https://github.com/neoncityrain/NCRUnbound/issues");
            NCRDebug.Log("Steam issue tracker: https://steamcommunity.com/workshop/filedetails/discussion/3262661679/4343239957177356958/");
            NCRDebug.Log("You may also reach out to me on Discord at NeonCityRain. I am in the Rain World server.");

            NCRDebug.Log("Standard error dialogue for the problem will now run. This will help to pinpoint the exact location that's running into problems, so please do not leave it out if reporting the error.");
            #endregion
        }
    }
}
