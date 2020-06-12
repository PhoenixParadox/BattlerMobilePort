using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MobileGameTest.Data;
using MobileGameTest.Menu;
using MobileGameTest.SourceLib;
using System;

namespace MobileGameTest
{
    public class Game1 : Game
    {
        public Activity1 gameActivity;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        // текущее состояние игры
        public GameState currentState;

        public double _inputTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;            
        }

        protected override void Initialize()
        {
            DataManager.Instance.Initialize(this);
            currentState = MainMenu.Instance;
            currentState.Initialize(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _inputTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_inputTimer > 1000000)
                _inputTimer = 0;
            currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            currentState.Draw(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// Смена состояния со сбросом текущего состояния.
        /// </summary>
        /// <param name="newState"></param>
        public void SwitchState(GameState newState)
        {
            currentState.Unload();
            currentState = newState;
            currentState.Initialize(this);
            Window.Title = currentState.ToString();
        }

        /// <summary>
        /// Переход в переданное состояние без сброса текущего.
        /// </summary>
        /// <param name="nextState"></param>
        public void ReturnToState(GameState nextState)
        {
            currentState = nextState;
        }

        public void Close(object sender, EventArgs e)
        {
            DataManager.Instance.Save();
            Exit();
        }
    }
}
