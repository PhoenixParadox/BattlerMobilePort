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
using MobileGameTest.Battle;
using MobileGameTest.Data;
using MobileGameTest.SourceLib;

namespace MobileGameTest.PlayerInfo
{

    public enum BotType 
    {
        Weak,
        Tank,
        Assasin,
        Knight,
        Healer
    }
    public class Bot : Player
    {
        private static Bot instance;
        public static Bot Instance
        {
            get
            {
                if (instance == null)
                    instance = new Bot();
                return instance;
            }
        }

        public override int MaxHP { get; set; }

        public Bot()
        {
            MaxHP = 25;
            HP = 25;
            Collection = new List<Talant>() { TalantFactory.BotAtck, TalantFactory.BotDef };
            Deck = new List<Talant>();
            for (int i = 0; i < 6; i++)
            {
                Deck.Add(TalantFactory.BotAtck);
                Deck.Add(TalantFactory.BotDef);
            }
            name = "Battle Bird";
            sprite = new Sprite(DataManager.Instance.botTxtr);
            portraitSprite = new Sprite(DataManager.Instance.botPortraitTxtr);
        }

        public Bot(BotType type) 
        {
            switch (type) 
            {
                case (BotType.Weak):
                    MaxHP = 20;
                    HP = 20;
                    Collection = new List<Talant>() { TalantFactory.BotAtck, TalantFactory.BotDef };
                    Deck = new List<Talant>();
                    for (int i = 0; i < 6; i++)
                    {
                        Deck.Add(TalantFactory.BotAtck);
                        Deck.Add(TalantFactory.BotDef);
                    }
                    name = "Battle Bird";
                    sprite = new Sprite(DataManager.Instance.botTxtr);
                    portraitSprite = new Sprite(DataManager.Instance.botPortraitTxtr);
                    break;
                case (BotType.Tank):
                    MaxHP = 60;
                    HP = 60;
                    Collection = new List<Talant>() { TalantFactory.BotGreatDef, TalantFactory.BotAtck };
                    Deck = new List<Talant>();
                    for (int i = 0; i < 6; i++) 
                    {
                        Deck.Add(TalantFactory.BotGreatDef);
                        Deck.Add(TalantFactory.BotAtck);
                    }
                    name = "DINO";
                    sprite = new Sprite(DataManager.Instance.Skins[4]);
                    portraitSprite = new Sprite(DataManager.Instance.Portraits[4]);
                    break;
                case (BotType.Assasin):
                    // fish king
                    MaxHP = 40;
                    HP = 40;
                    Collection = new List<Talant>() { TalantFactory.BotStrongAtck, TalantFactory.BotArmorPenitration, TalantFactory.BotDef };
                    Deck = new List<Talant>();
                    for (int i = 0; i < 3; i++)
                    {
                        Deck.Add(TalantFactory.BotStrongAtck);
                        Deck.Add(TalantFactory.BotDef);
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        Deck.Add(TalantFactory.BotArmorPenitration);
                    }
                    name = "Fish King";
                    sprite = new Sprite(DataManager.Instance.Skins[1]);
                    portraitSprite = new Sprite(DataManager.Instance.Portraits[1]);
                    break;
                case (BotType.Healer):
                    // blue creature
                    MaxHP = 30;
                    HP = 30;
                    Collection = new List<Talant>() { TalantFactory.BotDef, TalantFactory.BotDmgBonus, TalantFactory.BotHeal, TalantFactory.BotAtck };
                    Deck = new List<Talant>();
                    for (int i = 0; i < 3; i++)
                    {
                        Deck.Add(TalantFactory.BotAtck);
                        Deck.Add(TalantFactory.BotDmgBonus);
                        Deck.Add(TalantFactory.BotHeal);
                        Deck.Add(TalantFactory.BotDef);
                    }
                    name = "BLUE";
                    sprite = new Sprite(DataManager.Instance.Skins[2]);
                    portraitSprite = new Sprite(DataManager.Instance.Portraits[2]);
                    break;
                case (BotType.Knight):
                    // drogo
                    MaxHP = 80;
                    HP = 80;
                    Collection = new List<Talant>() { TalantFactory.BotStrongAtck, TalantFactory.BotGreatDef, TalantFactory.BotArmorBreak, TalantFactory.BotDmgBonus };
                    Deck = new List<Talant>();
                    for (int i = 0; i < 4; i++)
                    {
                        Deck.Add(TalantFactory.BotGreatDef);
                        Deck.Add(TalantFactory.BotArmorBreak);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        Deck.Add(TalantFactory.BotStrongAtck);
                        Deck.Add(TalantFactory.BotDmgBonus);
                    }
                    name = "DROGO";
                    sprite = new Sprite(DataManager.Instance.Skins[3]);
                    portraitSprite = new Sprite(DataManager.Instance.Portraits[3]);
                    break;
            }
        }

        public void SwitchBot(BotType type)
        {
            instance = new Bot(type);
        }


        public void Refresh()
        {
            HP = MaxHP;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}