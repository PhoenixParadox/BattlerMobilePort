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

namespace MobileGameTest.Components
{
    public class TouchButton
    {
        public Texture2D Texture { get; set; }
        public Vector2 position;
        public Vector2 size;
        public Rectangle frame;
        public EventHandler Click;
        private double touchRegisterRate; //ms
        private double touchRegisterTimer;
        private TouchLocationState prevState;
        private TouchLocationState curState;

        public TouchButton(Vector2 position, Texture2D txtr, Vector2 size = default(Vector2))
        {
            this.position = position;
            Texture = txtr;
            if (size == default(Vector2))
            {
                this.size = new Vector2(txtr.Width, txtr.Height);
            }
            else
            {
                this.size = size;
            }
            frame = new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y);
            touchRegisterRate = 300;
            prevState = TouchLocationState.Released;
        }
        public void Update(GameTime gameTime)
        {
            touchRegisterTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            var touchState = TouchPanel.GetState();
            foreach (var ts in touchState)
            {
                curState = ts.State;
                var rect1 = new Rectangle((int)ts.Position.X, (int)ts.Position.Y, 10, 10);
                if (rect1.Intersects(frame) && touchRegisterTimer >= touchRegisterRate)
                {
                    touchRegisterTimer = 0;
                    curState = TouchLocationState.Released;
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture: Texture, position: position);
        }
    }
}