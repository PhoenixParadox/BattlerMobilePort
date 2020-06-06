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


        public Bot()
        {
            MaxHP = 10;
            HP = 10;
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