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

namespace MobileGameTest.SourceLib
{
    public class Sprite
    {
        #region Fields

        protected AnimationManager _animationManager;

        protected Dictionary<string, Animation> _animations;

        protected Vector2 _position;

        protected Texture2D _texture;

        public float scale;
        public SpriteEffects spriteEffects;

        public int Width { get { return _texture.Width; } }
        public int Height { get { return _texture.Height; } }

        #endregion

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        #region Methods

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(_texture, Position, color: Color.White, scale: new Vector2(scale), effects: spriteEffects);
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch);
            else throw new Exception("This ain't right..!");
        }

        protected virtual void SetAnimations()
        {
            _animationManager.Play(_animations.First().Value);
        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        public virtual void Update(GameTime gameTime)
        {
            SetAnimations();

            _animationManager.Update(gameTime);
        }
        #endregion
    }
}
