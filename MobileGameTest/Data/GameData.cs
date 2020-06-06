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
using MobileGameTest.Shop;

namespace MobileGameTest.Data
{
    public struct Milestones
    {
        public int m1;
        public int m2;
        public Milestones(int v1, int v2) { m1 = v1; m2 = v2; }
    }

    public class GameData
    {
        private static GameData instance;
        public static GameData Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameData();
                return instance;
            }
        }

        public void Load(GameData data)
        {
            instance = data;
        }

        public List<int> shopItems;
        public Milestones milestones;
        public int currentGoal;

        public int baseAtckDmg;
        public int baseDefAmount;
        public int strongAtckDmg;
        public int swordSwingMultiplicity;
    }
}