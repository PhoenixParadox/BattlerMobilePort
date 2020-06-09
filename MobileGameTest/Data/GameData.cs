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

        public GameData()
        {
            nameToType = new Dictionary<string, TalantType>();
            foreach (var k in AllTalants.Keys)
            {
                nameToType.Add(k.ToString(), k);
            }
        }

        public Dictionary<string, TalantType> nameToType;

        // action, multiplicity, maxAction, maxMultiplicity
        public static Dictionary<TalantType, Tuple<int, int, int, int>> AllTalants = new Dictionary<TalantType, Tuple<int, int, int, int>>()
        {
            { TalantType.BaseAttack, Tuple.Create(5, 8, 10, 8) },
            { TalantType.BaseDef, Tuple.Create(5, 8, 10, 8) },
            { TalantType.ArmorBreak, Tuple.Create(7, 4, 14, 10)},
            { TalantType.ArmorPenitration, Tuple.Create(4, 3, 10, 5)},
            { TalantType.AtckBonus, Tuple.Create(2, 2, 5, 4)},
            { TalantType.Heal, Tuple.Create(3, 2, 10, 5)},
            { TalantType.SwordSwing, Tuple.Create(10, 2, 18, 5)}
            
        };

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

        public int healAmount;
        public int healMultiplicity;
        public int armorPenitrationDmg;
        public int armorPenitrationMultiplicity;
        public int armorBreakDmg;
        public int armorBreakMultiplicity;
        public int dmgBonus;
        public int dmgkBonusMultiplicity;

        public Dictionary<TalantType, int[]> allTalants;

        //public List<TalantType> allTalants;
        public List<TalantType> unlockedTalants;


        public void UpgradeAction(TalantType type)
        {
            allTalants[type][0] = Math.Min(allTalants[type][0] + 1, allTalants[type][2]);
        }
        //public void UpgradeAction(TalantType type)
        //{
        //    switch (type)
        //    {
        //        case (TalantType.BaseAttack):
        //            baseAtckDmg++;
        //            break;
        //        case (TalantType.BaseDef):
        //            baseDefAmount++;
        //            break;
        //        case (TalantType.ArmorPenitration):
        //            armorPenitrationDmg++;
        //            break;
        //        case (TalantType.ArmorBreak):
        //            armorBreakDmg++;
        //            break;
        //        case (TalantType.Heal):
        //            healAmount++;
        //            break;
        //        case (TalantType.SwordSwing):
        //            strongAtckDmg++;
        //            break;
        //        case (TalantType.AtckBonus):
        //            dmgBonus++;
        //            break;
        //    }
        //}
    }
}