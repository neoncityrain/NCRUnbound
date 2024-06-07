using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;


namespace UnboundCat
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            // Define your variables to store here!
            public bool IsUnbound;
            public bool PlayingSound;
            public bool CanCyanjump;
            public int UnbChainjumps;
            public int UnbCyanjumpCountdown;
            public int unbPlayerNumber;

            public UnboundCat(){
                UnbChainjumps = 0;
                UnbCyanjumpCountdown = 0;
                CanCyanjump = false;
                unbPlayerNumber = -1;
            }
        }

        // This part lets you access the stored stuff by simply doing "self.GetCat()" in Plugin.cs or everywhere else!
        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetCat(this Player player) => Unbound.GetValue(player, _ => new());
    }
}