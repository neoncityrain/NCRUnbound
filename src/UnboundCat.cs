using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;


namespace Unbound
{
    public static class UnboundCWT
    {
        public class UnboundCat
        {
            // Define your variables to store here!
            public bool IsUnbound;
            public bool PlayingSound;
            public bool didLongjump;

            public bool CanCyanjump1;
            public bool CanCyanjump2;

            public int UnbChainjumps;
            public int UnbCyanjumpCountdown;

            public UnboundCat(){
                UnbChainjumps = 0;
                UnbCyanjumpCountdown = 0;
                CanCyanjump1 = false;
                CanCyanjump2 = false;
                didLongjump = false;
            }
        }

        // This part lets you access the stored stuff by simply doing "self.GetCat()" in Plugin.cs or everywhere else!
        private static readonly ConditionalWeakTable<Player, UnboundCat> Unbound = new();
        public static UnboundCat GetCat(this Player player) => Unbound.GetValue(player, _ => new());
    }
}