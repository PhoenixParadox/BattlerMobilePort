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
using Microsoft.Xna.Framework.Graphics;
using MobileGameTest.Data;
using MobileGameTest.PlayerInfo;

namespace MobileGameTest.Battle
{

    public enum TalantType
    {
        BaseDef,
        BaseAttack,
        SwordSwing,
        BotAttack,
        BotDef
    }

    public class Talant
    {
        public Action<Player, Player> action;
        public Texture2D txtr;
        public string description;
        public string shortDescription;
        public int multiplicity;
        public TalantType type;
    }

    public class EmptyTalant : Talant
    {
        private static EmptyTalant instance;
        public static EmptyTalant Instance
        {
            get
            {
                if (instance == null)
                    instance = new EmptyTalant();
                return instance;
            }
        }
    }

    public static class TalantFactory
    {
        public static int botAtckDmg = 5;
        public static int botDefAmount = 5;

        public static int baseAtckDmg { get { return GameData.Instance.baseAtckDmg; } }
        public static int strongAtckDmg { get { return GameData.Instance.strongAtckDmg; } }
        public static int baseDefAmount { get { return GameData.Instance.baseDefAmount; } }

        private static Action<Player, Player> botDef = (p1, p2) => p1.DEF += botDefAmount;
        private static Action<Player, Player> botAtck = (p1, p2) =>
        {
            var dmg = p1.DMGBonus + botAtckDmg;
            if (p2.DEF + p2.DMGBuffer >= dmg)
            {
                if (p2.DMGBuffer >= dmg)
                {
                    p2.DMGBuffer = 0;
                    p1.DMGBonus = 0;
                    return;
                }
                p2.DEF -= Math.Abs(dmg - p2.DMGBuffer);
            }
            else
            {
                p2.HP -= (dmg - p2.DMGBuffer - p2.DEF);
                p2.DEF = 0;
                p2.DMGBuffer = 0;
            }
            p1.DMGBonus = 0;
        };

        private static Action<Player, Player> atck = (p1, p2) =>
        {
            var dmg = p1.DMGBonus + GameData.Instance.baseAtckDmg;
            if (p2.DEF + p2.DMGBuffer >= dmg)
            {
                if (p2.DMGBuffer >= dmg)
                {
                    p2.DMGBuffer = 0;
                    p1.DMGBonus = 0;
                    return;
                }
                p2.DEF -= Math.Abs(-p2.DMGBuffer + dmg);
            }
            else
            {
                p2.HP -= (dmg - p2.DMGBuffer - p2.DEF);
                p2.DEF = 0;
                p2.DMGBuffer = 0;
            }
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> strongAtck = (p1, p2) =>
        {
            var dmg = GameData.Instance.strongAtckDmg;
            if (p2.DEF >= GameData.Instance.strongAtckDmg)
            {
                if (p2.DMGBuffer >= dmg)
                {
                    p2.DMGBuffer = 0;
                    p1.DMGBonus = 0;
                    return;
                }
                p2.DEF -= Math.Abs(dmg - p2.DMGBuffer);
            }
            else
            {
                p2.HP -= (dmg - p2.DMGBuffer - p2.DEF);
                p2.DEF = 0;
                p2.DMGBuffer = 0;
            }
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> def = (p1, p2) => p1.DEF += GameData.Instance.baseDefAmount;

        public static Talant BaseAttack { get { return new Talant() { action = atck, txtr = DataManager.Instance.baseAtck, description = $"СЛАБАЯ АТАКА, НАНОСЯЩАЯ {GameData.Instance.baseAtckDmg} DMG", shortDescription = $"{GameData.Instance.baseAtckDmg} DMG", multiplicity = 8, type = TalantType.BaseAttack }; } }
        public static Talant BaseDef { get { return new Talant() { action = def, txtr = DataManager.Instance.baseDef, description = $"ДАЕТ {GameData.Instance.baseDefAmount} DEF", shortDescription = $"{GameData.Instance.baseDefAmount} DEF", multiplicity = 8, type = TalantType.BaseDef }; } }
        public static Talant SwordSwing { get { return new Talant() { action = strongAtck, txtr = DataManager.Instance.strongAtck, description = "БЫСТРЫЙ И МОЩНЫЙ ВЗМАХ,\n" + $"НАНОСЯЩИЙ {GameData.Instance.strongAtckDmg} DMG", shortDescription = $"{GameData.Instance.strongAtckDmg} DMG", multiplicity = GameData.Instance.swordSwingMultiplicity, type = TalantType.SwordSwing }; } }

        public static Talant BotAtck { get { return new Talant() { action = botAtck, txtr = DataManager.Instance.baseAtck, description = "", shortDescription = "5 DMG", multiplicity = 0, type = TalantType.BotAttack }; } }
        public static Talant BotDef
        {
            get
            {
                return new Talant() { action = botDef, txtr = DataManager.Instance.baseDef, description = "", shortDescription = "5 DEF", multiplicity = 0, type = TalantType.BotDef };
            }
        }

        private static Dictionary<TalantType, Func<Talant>> dict = new Dictionary<TalantType, Func<Talant>> { { TalantType.BaseDef, () => TalantFactory.BaseDef },
                                                                                                              { TalantType.BaseAttack, () => TalantFactory.BaseAttack },
                                                                                                              { TalantType.SwordSwing, () => TalantFactory.SwordSwing},
    { TalantType.BotAttack, () => TalantFactory.BotAtck},
    { TalantType.BotDef, () => TalantFactory.BotDef} };

        public static Talant GetTalant(TalantType type)
        {
            return dict[type]();
        }

    }
}