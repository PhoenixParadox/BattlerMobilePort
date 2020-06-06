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
using MobileGameTest.SourceLib;

namespace MobileGameTest.Battle
{
    public class Victory : GameState
    {
        private static Victory instance;
        public static Victory Instance
        {
            get
            {
                if (instance == null)
                    instance = new Victory();
                return instance;
            }
        }

        private new Game1 game;
        private TouchButton continueBtn;
        public override void Draw(GameTime gameTime)
        {
            game.spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
            game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, "Ты победил!", new Vector2(230, 250), Color.White);
            game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, "Заработано трофеев: 10", new Vector2(230, 300), Color.White);
            continueBtn.Draw(game.spriteBatch);
            //game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, continueBtn.Text, new Vector2(290, 430), Color.White);
        }

        public override void Initialize(Game game)
        {
            this.game = game as Game1;
            continueBtn = new TouchButton(new Vector2(220, 400), DataManager.Instance.woodenBtn) { Click = SwitchToMenu };
        }

        private void SwitchToMenu(object sender, EventArgs e)
        {
            game.SwitchState(MainMenu.Instance);
        }

        public override void Unload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            continueBtn.Update(gameTime);
        }
    }
}