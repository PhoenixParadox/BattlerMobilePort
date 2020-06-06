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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MobileGameTest.SourceLib
{
    /// <summary>
    /// Состояние игры, задающее происходящее на экране.
    /// </summary>
    public abstract class GameState
    {
        protected Game game;

        protected GraphicsDevice graphics;

        protected ContentManager content;

        public abstract void Draw(GameTime gameTime);

        public abstract void Update(GameTime gameTime);

        public abstract void Initialize(Game game);

        public abstract void Unload();        
    }
}