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
using MobileGameTest.Data;
using MobileGameTest.SourceLib;
using static MobileGameTest.Battle.BattleState;

namespace MobileGameTest.Menu
{
    public class LoadingScreen : GameState
    {
        private static LoadingScreen instance;
        public static LoadingScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new LoadingScreen();
                return instance;
            }
        }

        private double timer;
        private new Game1 game;
        private Vector2 loadingSymbolPosition = new Vector2(300, 600);
        private Vector2 loadingMessagePosition = new Vector2(170, 200);
        private Vector2 backgroundPosition = new Vector2(-300, 0);
        private int currentFrame;
        private double animationTimer;

        public override void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.White);
            game.spriteBatch.Draw(DataManager.Instance.menuBackground, backgroundPosition, scale: new Vector2(1.5f));
            game.spriteBatch.Draw(DataManager.Instance.loadingAnimation[currentFrame], loadingSymbolPosition);
            game.spriteBatch.Draw(DataManager.Instance.loadingMessage, loadingMessagePosition);
        }

        public override void Initialize(Game game)
        {
            if(this.game == null)
                this.game = game as Game1;
        }

        public override void Unload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            animationTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer >= 5000)
            {
                timer = 0;
                game.SwitchState(ChainBattleState.Instance);
            }
            if (animationTimer >= 200)
            {
                animationTimer = 0;
                currentFrame = (currentFrame == 3) ? 0 : currentFrame + 1;
            }
        }
    }
}