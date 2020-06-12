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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MobileGameTest.Components;
using MobileGameTest.Data;
using MobileGameTest.Education;
using MobileGameTest.PlayerInfo;
using MobileGameTest.Shop;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Menu
{
    public class MainMenu : GameState
    {
        private static MainMenu instance;
        public static MainMenu Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainMenu();
                }
                return instance;
            }
        }

        private MainMenu()
        {
            Load();
        }

        private new Game1 game;
        private List<TouchButton> components;
        private List<InvisibleButton> components1;


        public override void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            game.spriteBatch.Draw(DataManager.Instance.menuBackground, new Vector2(-400, -150));
            game.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-50, 750));
            game.spriteBatch.Draw(DataManager.Instance.battleUpperPanel, new Vector2(-30, 0));
            foreach (var e in components)
            {
                e.Draw(game.spriteBatch);
            }

            game.spriteBatch.Draw(DataManager.Instance.coinIcon, new Vector2(70, 10));
            game.spriteBatch.DrawString(DataManager.Instance.menuFont, Player1.Instance.points.ToString(), new Vector2(5, 30), Color.LightGoldenrodYellow);
            game.spriteBatch.Draw(DataManager.Instance.trophieIcon, new Vector2(210, 10));
            game.spriteBatch.DrawString(DataManager.Instance.menuFont, Player1.Instance.trophies.ToString(), new Vector2(160, 30), Color.LightGoldenrodYellow);
            Player1.Instance.sprite.Draw(game.spriteBatch);
        }

        public void Load()
        {
            components = new List<TouchButton>();
            components1 = new List<InvisibleButton>();
            components.Add(new TouchButton(new Vector2(190, 1050), DataManager.Instance.battleButtonTxtr) { Click = SwitchToBattle });
            components.Add(new TouchButton(new Vector2(190, 830), DataManager.Instance.tasksButtonTxtr) { Click = SwitchToEducation });
            components.Add(new TouchButton(new Vector2(680, 20), DataManager.Instance.exitBtnTxtr) { Click = CloseGame });
            components.Add(new TouchButton(new Vector2(550, 24), DataManager.Instance.shopIcon) { Click = SwitchToShop });
            components1.Add(new InvisibleButton(new Rectangle(175, 150,
                                                 Player1.Instance.sprite.Width,
                                                 Player1.Instance.sprite.Height))
            { Click = SwitchToPlayerInfo });
        }

        public override void Initialize(Game game)
        {
            if (this.game == null)
            {
                this.game = game as Game1;
            }
            PlayerInfoScreen.Instance.Initialize(game);
            Player1.Instance.Initialize();
            Player1.Instance.sprite.Position = new Vector2(200, 200);
            Player1.Instance.sprite.scale = 1.3f;
            Player1.Instance.sprite.spriteEffects = SpriteEffects.None;
            //PlayerData.Instance.points += 500;
        }

        private void CloseGame(object sender, EventArgs e)
        {
            this.game.Close(sender, e);
        }

        private void SwitchToShop(object sender, EventArgs e)
        {
            game.SwitchState(ItemShop.Instance);
        }

        private void SwitchToPlayerInfo(object sender, EventArgs e)
        {
            game.SwitchState(PlayerInfoScreen.Instance);
        }

        private void SwitchToEducation(object sender, EventArgs e)
        {
            // TODO: переход в меню обучения
            game.SwitchState(StartEducationalScreen.Instance);
        }

        public void SwitchToBattle(object sender, EventArgs e)
        {
            //game.SwitchState(ChainBattleState.Instance);
            game.SwitchState(LoadingScreen.Instance);
        }

        public override void Unload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            var touchState = TouchPanel.GetState();
            foreach (var ts in touchState)
            {
                if (ts.State == TouchLocationState.Pressed)
                {
                    var rect = new Rectangle((int)ts.Position.X, (int)ts.Position.Y, 10, 10);
                    foreach (var e in components)
                    {
                        if (e.frame.Intersects(rect))
                        {
                            e.Click?.Invoke(e, new EventArgs());
                            return;
                        }
                        foreach (var e1 in components1)
                        {
                            if (e1.frame.Intersects(rect))
                            {
                                e1.Click?.Invoke(e1, new EventArgs());
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}