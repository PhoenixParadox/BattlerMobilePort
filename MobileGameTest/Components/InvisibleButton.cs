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
using Microsoft.Xna.Framework.Input.Touch;

namespace MobileGameTest.Components
{
    public class InvisibleButton
    {
        public Vector2 position;
        public Vector2 size;
        public Rectangle frame;
        public EventHandler Click;
        private double touchRegisterRate; //ms
        private double touchRegisterTimer;

        public InvisibleButton(Rectangle rect)
        {
            frame = rect;
            touchRegisterRate = 300;
        }
        public void Update(GameTime gameTime)
        {
            touchRegisterTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            var touchState = TouchPanel.GetState();
            foreach (var ts in touchState)
            {
                var rect1 = new Rectangle((int)ts.Position.X, (int)ts.Position.Y, 1, 1);
                if (rect1.Intersects(frame) && touchRegisterTimer >= touchRegisterRate)
                {
                    touchRegisterTimer = 0;
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}