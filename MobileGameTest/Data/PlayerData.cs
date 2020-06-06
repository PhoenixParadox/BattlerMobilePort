using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileGameTest.Battle;

namespace MobileGameTest.Data
{
    public class PlayerData
    {
        private static PlayerData instance;
        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerData();
                return instance;
            }
        }

        public void Load(PlayerData data)
        {
            instance = data;
        }
        public int currentSkin;
        public int maxUnlockedSkin;
        public int points;
        public int trophies;
        public int gamesPlayed;
        public int level;
        public int MaxHP;
        public string Name;
        public List<TalantType> playerCollection;
        public List<TalantType> playerDeck;
    }
}