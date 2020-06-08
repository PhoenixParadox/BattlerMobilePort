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
using MobileGameTest.Shop;
using MobileGameTest.SourceLib;

namespace MobileGameTest.PlayerInfo
{

    /// <summary>
    /// Обобщённый класс игрока.
    /// </summary>
    public abstract class Player
    {
        public virtual int MaxHP { get; set; }
        public int HP;
        public int DEF;
        public List<Talant> Collection;
        public List<List<Talant>> Decks;
        public List<Talant> Deck;
        public List<Talant> Deck1;
        public List<Talant> Deck2;
        public List<Talant> Deck3;
        public int currentDeck;
        public Sprite sprite;
        public Sprite portraitSprite;
        public string name;
        public int DMGBuffer;
        public int DMGBonus;

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }

    /// <summary>
    /// Игрок, привязанный к игре.
    /// </summary>
    public class Player1 : Player
    {
        public static Player1 instance;
        public static Player1 Instance
        {
            get
            {
                if (instance == null)
                    instance = new Player1();
                return instance;
            }
        }
        public ItemType Potion;
        public int points
        {
            get
            {
                return PlayerData.Instance.points;
            }
        }
        public int trophies
        {
            get
            {
                return PlayerData.Instance.trophies;
            }
        }
        public override int MaxHP
        {
            get
            {
                if (PlayerData.Instance.MaxHP == 0)
                {
                    PlayerData.Instance.MaxHP = 25;
                }
                return PlayerData.Instance.MaxHP;
            }
        }

        public Player1()
        {
            name = PlayerData.Instance.Name;
        }

        public void Initialize()
        {
            sprite = new Sprite(DataManager.Instance.Skins[PlayerData.Instance.currentSkin]);
            portraitSprite = new Sprite(DataManager.Instance.Portraits[PlayerData.Instance.currentSkin]);
        }

        public void Reload()
        {
            
            for (int i = 0; i < Collection.Count; i++)
            {
                Collection[i] = TalantFactory.GetTalant(Collection[i].type);
                PlayerData.Instance.playerCollection[i] = Collection[i].type;
            }
            for (int i = 0; i < Deck.Count; i++)
            {
                Deck[i] = TalantFactory.GetTalant(Deck[i].type);
                PlayerData.Instance.playerDeck[i] = Deck[i].type;
            }
            
        }

        public void SetDeck()
        {
            Deck = (currentDeck == 1) ? Deck1 : (currentDeck == 2) ? Deck2 : Deck3;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }


    /// <summary>
    /// Класс для представления игрока в битве.
    /// </summary>
    public class InBattlePlayer
    {
        public Player plr;

        public string name { get { return plr.name; } }

        //стопки добора
        public List<Talant> drawStack;

        //стопки сброса
        public List<Talant> dropStack;

        public Talant[] hand;

        public Talant[] chain;

        //набор из 12 карт, используемых в игре
        public Talant[] deck { get { return plr.Deck.ToArray(); } }

        public InBattlePlayer(Player plr)
        {
            this.plr = plr;
            hand = new Talant[3];
            chain = new Talant[3];
            drawStack = new List<Talant>();
            dropStack = new List<Talant>();
        }
    }
}