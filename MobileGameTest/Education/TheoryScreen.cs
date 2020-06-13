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
using MobileGameTest.Education;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Education
{
    public class TheoryScreen : GameState
    {
        #region creation
        private static TheoryScreen instance;
        public static TheoryScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new TheoryScreen();
                return instance;
            }
        }

        private TheoryScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(250, 1100), DataManager.Instance.continueButton) { Click = BackToLevel });
            components.Add(new TouchButton(new Vector2(700, 150), DataManager.Instance.exitBtnTxtr) { Click = BackToLevel });
        }

        public override void Unload()
        {

        }
        #endregion
        #region fields
        private new Game1 game;
        private List<TouchButton> components;
        #endregion

        public override void Initialize(Game game)
        {
            this.game = game as Game1;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var c in components)
            {
                c.Update(gameTime);
            }
        }


        public override void Draw(GameTime gameTime)
        {
            game.spriteBatch.Draw(DataManager.Instance.educationBackground, new Vector2(0));
            game.spriteBatch.Draw(DataManager.Instance.theoryText[EducationData.Instance.currentLevel], new Vector2(-30, 140), scale: new Vector2(1.1f));
            game.spriteBatch.Draw(DataManager.Instance.levelTopics[EducationData.Instance.currentLevel], new Vector2(90, 5));

            foreach (var c in components)
            {
                c.Draw(game.spriteBatch);
            }
        }


        private void BackToLevel(object sender, EventArgs e)
        {
            game.ReturnToState(StartEducationalScreen.Instance);
        }

    }
}