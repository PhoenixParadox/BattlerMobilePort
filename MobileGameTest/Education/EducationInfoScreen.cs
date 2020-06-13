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
using MobileGameTest.SourceLib;

namespace MobileGameTest.Education
{
    public class EducationInfoScreen : GameState
    {
        #region fields
        private new Game1 game;
        private List<TouchButton> components;
        private int currentPage;
        #endregion

        #region creation

        private static EducationInfoScreen instance;
        public static EducationInfoScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new EducationInfoScreen();
                return instance;
            }
        }

        private EducationInfoScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(700, 110), DataManager.Instance.exitBtnTxtr) { Click = BackToLevel });

            components.Add(new TouchButton(new Vector2(550, 1100), DataManager.Instance.nextTaskBtn) { Click = NextPage });
            components.Add(new TouchButton(new Vector2(70, 1100), DataManager.Instance.prevTaskBtn) { Click = PrevPage });
        }

        public override void Unload()
        {

        }
        #endregion

        public override void Initialize(Game game)
        {
            if (this.game == null)
            {
                this.game = game as Game1;
            }
            currentPage = 0;
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
            game.GraphicsDevice.Clear(Color.Yellow);
            game.spriteBatch.Draw(DataManager.Instance.educationBackground, new Vector2(0));
            game.spriteBatch.Draw(DataManager.Instance.tutorialPages[currentPage], new Vector2(-20, 100));
            foreach (var c in components)
            {
                c.Draw(game.spriteBatch);
            }
        }

        private void BackToLevel(object sender, EventArgs e)
        {
            game.ReturnToState(StartEducationalScreen.Instance);
        }

        private void PrevPage(object sender, EventArgs e)
        {
            currentPage = (currentPage != 0) ? currentPage - 1 : DataManager.Instance.tutorialPages.Count - 1;
        }

        private void NextPage(object sender, EventArgs e)
        {
            currentPage = (currentPage + 1) % DataManager.Instance.tutorialPages.Count;
        }
    }
}