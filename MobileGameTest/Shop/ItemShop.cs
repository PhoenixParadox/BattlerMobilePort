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
using Microsoft.Xna.Framework;
using MobileGameTest.Components;
using MobileGameTest.Data;
using MobileGameTest.Menu;
using MobileGameTest.PlayerInfo;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Shop
{
    public class ItemShop : GameState
    {
        private enum ShopTabs
        {
            Items,
            Skins
        }

        private static ItemShop instance;
        public static ItemShop Instance
        {
            get
            {
                if (instance == null)
                    instance = new ItemShop();
                return instance;
            }
        }

        private new Game1 game;
        private List<TouchButton> components;
        public List<Item> items;
        public TouchButton[] tabs;
        private ShopTabs activeTab;
        //private List<InvisibleButton> skinsPoints;
        private List<InvisibleButton> toRemove;
        private InvisibleButton[] SellItemButtons;

        private InvisibleButton[] SellSkinButtons;
        public List<Item> skins;

        private ItemShop()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(550, 30), DataManager.Instance.exitBtnTxtr) { Click = BackToMenu });

            toRemove = new List<InvisibleButton>();

            //skinsPoints = new List<InvisibleButton>();
            //var b1 = new InvisibleButton(new Rectangle(30, 440, 100, 150));
            //b1.Click = SellSkin;
            //skinsPoints.Add(b1);

            tabs = new TouchButton[2];
            tabs[0] = new TouchButton(new Vector2(90, 360), DataManager.Instance.woodenBtn);
            tabs[1] = new TouchButton(new Vector2(90 + 60 + DataManager.Instance.woodenBtn.Width, 360), DataManager.Instance.woodenBtn);
            tabs[0].Click = (object sender, EventArgs e) => this.activeTab = ShopTabs.Items;
            tabs[1].Click = (object sender, EventArgs e) => this.activeTab = ShopTabs.Skins;
            if (items != null)
            {
                MakeItemButtons();
            }

            if (skins != null)
            {
                MakeSkinButtons();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //game.spriteBatch.GraphicsDevice.Clear(Color.DarkGray);

            game.spriteBatch.Draw(DataManager.Instance.shopBackground, new Vector2(0, 0));
            game.spriteBatch.Draw(DataManager.Instance.shopItemsBackground, new Vector2(-5, 402));

            game.spriteBatch.Draw(DataManager.Instance.coinIcon, new Vector2(70, 10));
            game.spriteBatch.DrawString(DataManager.Instance.menuFont, Player1.Instance.points.ToString(), new Vector2(5, 30), Color.LightGoldenrodYellow);
            foreach (var c in components)
                c.Draw(game.spriteBatch);
            foreach (var t in tabs)
            {
                t.Draw(game.spriteBatch);
                //if (t.Text != null)
                //{
                //    game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, t.Text, new Vector2(t.position.X + 30, t.position.Y + 20), Color.White);
                //}
            }

            switch (activeTab)
            {
                case (ShopTabs.Items):
                    var pos = new Vector2(30, 440);
                    var pos1 = new Vector2(pos.X + 100, pos.Y + 10);
                    var pos3 = new Vector2(pos.X - 5, pos.Y + 20);
                    foreach (var i in items)
                    {
                        game.spriteBatch.Draw(DataManager.Instance.shopItemIcon, pos);
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, i.description, pos1, Color.White);
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, i.cost.ToString(), pos3, Color.Yellow);
                        pos1.Y += 100;
                        pos.Y += 100;
                        pos3.Y += 100;
                    };
                    break;
                case (ShopTabs.Skins):
                    var pos2 = new Vector2(30, 440);
                    foreach (var i in skins)
                    {
                        game.spriteBatch.Draw(i.txtr, pos2, scale: new Vector2(0.5f));
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, i.cost.ToString(), new Vector2(pos2.X + 30, pos2.Y + 160), Color.Yellow);
                        pos2.X += 200;
                        if (pos2.X >= 600)
                        {
                            pos2.X = 30;
                            pos2.Y += 200;
                        }
                    };
                    break;
            }

        }

        public override void Initialize(Game game)
        {
            if (this.game == null)
            {
                this.game = game as Game1;
            }
            MakeItemButtons();
            MakeSkinButtons();
        }

        private void MakeItemButtons()
        {
            SellItemButtons = new InvisibleButton[items.Where(i => i.type != ItemType.Skin).Count()];
            var pos = new Vector2(30, 440);
            var ind = 0;
            foreach (var i in items.Where(i => i.type != ItemType.Skin))
            {
                var btn = new InvisibleButton(new Rectangle((int)pos.X, (int)pos.Y, DataManager.Instance.shopItemIcon.Width, DataManager.Instance.shopItemIcon.Height));
                btn.Click += SellItem;
                SellItemButtons[ind++] = btn;
                pos.Y += 100;
            };
        }

        private void MakeSkinButtons()
        {
            SellSkinButtons = new InvisibleButton[skins.Count()];
            var pos = new Vector2(30, 440);
            var ind = 0;
            foreach (var i in skins)
            {
                var btn = new InvisibleButton(new Rectangle((int)pos.X, (int)pos.Y, DataManager.Instance.SkinsAndPortraits[i.id].Item1.Width / 2, DataManager.Instance.SkinsAndPortraits[i.id].Item1.Height / 2));
                btn.Click += SellSkin;
                SellSkinButtons[ind++] = btn;
                pos.X += 200;
                if (pos.X >= 600)
                {
                    pos.Y += 200;
                    pos.X = 30;
                }
            };
        }

        private void SellItem(object sender, EventArgs e)
        {
            var ind = 0;
            for (int i = 0; i < SellItemButtons.Length; i++)
            {
                if (((InvisibleButton)sender) == SellItemButtons[i])
                {
                    ind = i;
                    break;
                }
            }
            if (Player1.Instance.points < items[ind].cost)
                return;
            var type = items[ind].type;
            PlayerData.Instance.points -= items[ind].cost;
            GameData.Instance.shopItems.Remove(items[ind].id);
            Player1.Instance.Potion = type;
            var itemToRemove = items[ind];
            items.Remove(itemToRemove);
            MakeItemButtons();
        }

        private void SellSkin(object sender, EventArgs e)
        {
            var ind = 0;
            for (int i = 0; i < SellSkinButtons.Length; i++)
            {
                if (((InvisibleButton)sender) == SellSkinButtons[i])
                {
                    ind = i;
                    break;
                }
            }

            if (Player1.Instance.points < ItemBase.dict[skins[ind].id].cost)
                return;
            PlayerData.Instance.points -= ItemBase.dict[skins[ind].id].cost;
            //PlayerData.Instance.currentSkin = 1;
            //PlayerData.Instance.maxUnlockedSkin = 1;
            PlayerData.Instance.unlockedSkins.Add(skins[ind].id);
            GameData.Instance.shopSkins.Remove(skins[ind].id);
            toRemove.Add(sender as InvisibleButton);
            var skinToRemove = skins[ind];
            skins.Remove(skinToRemove);
            MakeSkinButtons();
        }

        private void BackToMenu(object sender, EventArgs e)
        {
            game.SwitchState(MainMenu.Instance);
        }

        public override void Unload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in components)
                c.Update(gameTime);
            foreach (var t in tabs)
            {
                t.Update(gameTime);
            }

            switch (activeTab)
            {
                case (ShopTabs.Skins):
                    foreach (var b in SellSkinButtons)
                    {
                        b.Update(gameTime);
                    }
                    break;
                case (ShopTabs.Items):
                    foreach (var s in SellItemButtons)
                    {
                        s.Update(gameTime);
                    }
                    break;
            }
        }
    }
}