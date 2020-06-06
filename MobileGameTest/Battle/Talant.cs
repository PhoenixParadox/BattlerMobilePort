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
        BotDef,
        Heal,
        ArmorPenitration,
        ArmorBreak,
        BotHeal,
        BotArmorPenitration,
        BotArmorBreak,
        BotGreatDef,
        BotStrongAttack,
        BotAtckBonus,
        AtckBonus
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
        public static int botArmorPenitrationDmg = 4;
        public static int botArmorBreakDmg = 7;
        public static int botStrongAttackDmg = 12;
        public static int botHealAmount = 6;
        public static int botGreatDefAmount = 8;
        public static int botDmgBonus = 5;

        //public static int baseAtckDmg { get { return GameData.Instance.baseAtckDmg; } }
        //public static int strongAtckDmg { get { return GameData.Instance.strongAtckDmg; } }
        //public static int baseDefAmount { get { return GameData.Instance.baseDefAmount; } }

        private static Action<Player, Player> botDef = (p1, p2) => p1.DEF += botDefAmount;
        private static Action<Player, Player> botGreatDef = (p1, p2) => p1.DEF += botGreatDefAmount;
        private static Action<Player, Player> botHeal = (p1, p2) => p1.HP = Math.Min(p1.MaxHP, p1.HP + botHealAmount);
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
        private static Action<Player, Player> botStrongAtck = (p1, p2) => 
        {
            var dmg = p1.DMGBonus + botStrongAttackDmg;
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
        private static Action<Player, Player> botArmorBreakAtck = (p1, p2) =>
        {
            p2.DEF = Math.Max(0, p2.DEF - botArmorBreakDmg - p1.DMGBonus);
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> botArmorPenitrationAtck = (p1, p2) =>
        {
            var dmg = Math.Max(0, botArmorPenitrationDmg + p1.DMGBonus - p2.DMGBuffer);
            p2.HP -= dmg;
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> botAtckBonus = (p1, p2) =>
        {
            p1.DMGBonus += botDmgBonus;
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
        private static Action<Player, Player> heal = (p1, p2) => p1.HP = Math.Min(p1.MaxHP, p1.HP + GameData.Instance.healAmount);
        private static Action<Player, Player> armorPenitration = (p1, p2) =>
        {
            var dmg = GameData.Instance.armorPenitrationDmg  + p1.DMGBonus - p2.DMGBuffer;
            p2.HP -= dmg;
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> armorBreak = (p1, p2) => 
        {
            p2.DEF = Math.Max(0, p2.DEF - GameData.Instance.armorBreakDmg - p1.DMGBonus);
            p1.DMGBonus = 0;
        };
        private static Action<Player, Player> atckBonus = (p1, p2) =>
        {
            p1.DMGBonus += GameData.Instance.dmgBonus;
        };

        public static Talant BaseAttack 
        {
            get
            {
                return new Talant()
                {
                    action = atck,
                    txtr = DataManager.Instance.baseAtck,
                    description = $"АТАКА, НАНОСЯЩАЯ {GameData.Instance.baseAtckDmg} DMG",
                    shortDescription = $"{GameData.Instance.baseAtckDmg} DMG",
                    multiplicity = 8,
                    type = TalantType.BaseAttack 
                };
            }
        }
        public static Talant BaseDef 
        { 
            get 
            {
                return new Talant()
                { 
                    action = def,
                    txtr = DataManager.Instance.baseDef, 
                    description = $"ДАЕТ {GameData.Instance.baseDefAmount} DEF",
                    shortDescription = $"{GameData.Instance.baseDefAmount} DEF",
                    multiplicity = 8, 
                    type = TalantType.BaseDef
                };
            }
        }
        public static Talant SwordSwing 
        {
            get 
            {
                return new Talant() 
                {
                    action = strongAtck, 
                    txtr = DataManager.Instance.strongAtck, 
                    description = "БЫСТРЫЙ И МОЩНЫЙ ВЗМАХ,\n" + $"НАНОСЯЩИЙ {GameData.Instance.strongAtckDmg} DMG",
                    shortDescription = $"{GameData.Instance.strongAtckDmg} DMG",
                    multiplicity = GameData.Instance.swordSwingMultiplicity, 
                    type = TalantType.SwordSwing 
                }; 
            }
        }
        public static Talant CrushingMaul 
        {
            get 
            {
                return new Talant() 
                {
                    action = armorBreak,
                    txtr = DataManager.Instance.armorBreakAtck,
                    description = "СОКРУШАЮЩИЙ УДАР,\n" + $"НАНОСЯЩИЙ {GameData.Instance.armorBreakDmg} DMG БРОНЕ",
                    shortDescription = $"{GameData.Instance.armorBreakDmg} DMG БРОНЕ",
                    multiplicity = GameData.Instance.armorBreakMultiplicity,
                    type = TalantType.ArmorBreak
                };
            }
        }
        public static Talant SharpEdge
        {
            get
            {
                return new Talant()
                {
                    action = armorPenitration,
                    txtr = DataManager.Instance.armorPenitrationAtck,
                    description = "КОЛЮЩИЙ УДАР,\n" + $"НАНОСЯЩИЙ {GameData.Instance.armorBreakDmg} DMG СКВОЗЬ БРОНЮ",
                    shortDescription = $"{GameData.Instance.armorBreakDmg} DMG СКВОЗЬ БРОНЮ",
                    multiplicity = GameData.Instance.armorPenitrationMultiplicity,
                    type = TalantType.ArmorPenitration
                };
            }
        }
        public static Talant LifeSource 
        {
            get 
            {
                return new Talant()
                {
                    action = heal,
                    txtr = DataManager.Instance.heal,
                    description = $"ВОССТАНОВИ {GameData.Instance.healAmount } HP",
                    shortDescription = $"+ {GameData.Instance.healAmount} HP",
                    multiplicity = GameData.Instance.healMultiplicity,
                    type = TalantType.Heal
                };
            }
        }
        public static Talant PowerWave
        {
            get
            {
                return new Talant()
                {
                    action = atckBonus,
                    txtr = DataManager.Instance.dmgBonus,
                    description = $"+ {GameData.Instance.dmgBonus} DMG\nК СЛЕДУЮЩЕЙ\nАТАКЕ",
                    shortDescription = $"+ {GameData.Instance.dmgBonus} DMG\nК СЛЕДУЮЩЕЙ\nАТАКЕ",
                    multiplicity = GameData.Instance.dmgkBonusMultiplicity,
                    type = TalantType.AtckBonus
                };
            }
        }

        public static Talant BotAtck 
        { 
            get
            {
                return new Talant()
                {
                    action = botAtck, 
                    txtr = DataManager.Instance.baseAtck,
                    shortDescription = "5 DMG",
                    type = TalantType.BotAttack 
                }; 
            }
        }
        public static Talant BotStrongAtck 
        {
            get 
            {
                return new Talant() 
                {
                    action = botStrongAtck,
                    txtr = DataManager.Instance.strongAtck,
                    shortDescription = "12 DMG",
                    type = TalantType.BotStrongAttack
                };
            }
        }
        public static Talant BotDef
        {
            get
            {
                return new Talant()
                { 
                    action = botDef, 
                    txtr = DataManager.Instance.baseDef,
                    shortDescription = "5 DEF", 
                    type = TalantType.BotDef 
                };
            }
        }
        public static Talant BotGreatDef
        {
            get
            {
                return new Talant()
                {
                    action = botGreatDef,
                    txtr = DataManager.Instance.baseDef,
                    shortDescription = "8 DEF",
                    type = TalantType.BotGreatDef
                };
            }
        }
        public static Talant BotArmorPenitration 
        {
            get 
            {
                return new Talant()
                {
                    action = botArmorPenitrationAtck,
                    txtr = DataManager.Instance.armorPenitrationAtck,
                    shortDescription = "4 DMG,\nИгнорирует\nDEF",
                    type = TalantType.BotArmorPenitration
                };
            }
        }
        public static Talant BotArmorBreak 
        {
            get 
            {
                return new Talant() 
                {
                    action = botArmorBreakAtck,
                    txtr = DataManager.Instance.armorBreakAtck,
                    shortDescription = "7 DMG\nпо броне",
                    type = TalantType.BotArmorBreak
                };
            }
        }
        public static Talant BotHeal 
        {
            get 
            {
                return new Talant() 
                {
                    action = botHeal,
                    txtr = DataManager.Instance.heal,
                    shortDescription = "Восстановить\n6 HP",
                    type = TalantType.BotHeal
                };
            }
        }
        public static Talant BotDmgBonus
        {
            get
            {
                return new Talant()
                {
                    action = botAtckBonus,
                    txtr = DataManager.Instance.dmgBonus,
                    shortDescription = "+ 5 DMG\nК СЛЕДУЮЩЕЙ\nАТАКЕ",
                    type = TalantType.BotAtckBonus
                };
            }
        }


        private static Dictionary<TalantType, Func<Talant>> dict = new Dictionary<TalantType, Func<Talant>> 
        { 
            { TalantType.BaseDef, () => TalantFactory.BaseDef },
            { TalantType.BaseAttack, () => TalantFactory.BaseAttack },
            { TalantType.SwordSwing, () => TalantFactory.SwordSwing },
            { TalantType.BotAttack, () => TalantFactory.BotAtck },
            { TalantType.BotStrongAttack, () => TalantFactory.BotStrongAtck},
            { TalantType.BotDef, () => TalantFactory.BotDef },
            { TalantType.BotGreatDef, () => TalantFactory.BotGreatDef },
            { TalantType.BotArmorPenitration, () => TalantFactory.BotArmorPenitration },
            { TalantType.BotArmorBreak, () => TalantFactory.BotArmorBreak },
            { TalantType.BotHeal, () => TalantFactory.BotHeal },
            { TalantType.ArmorBreak, () => TalantFactory.CrushingMaul },
            { TalantType.ArmorPenitration, () => TalantFactory.SharpEdge },
            { TalantType.Heal, () => TalantFactory.LifeSource },
            { TalantType.BotAtckBonus, () => TalantFactory.BotDmgBonus },
            { TalantType.AtckBonus, () => TalantFactory.PowerWave }
        };

        public static Talant GetTalant(TalantType type)
        {
            return dict[type]();
        }

    }
}