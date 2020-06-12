using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Microsoft.Xna.Framework;
using MobileGameTest.Components;
using MobileGameTest.Data;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Education
{
    public enum AnswerState
    {
        Right,
        Wrong,
        Undefined
    }

    public class CurrentTaskScreen : GameState
    {
        #region creation
        private static CurrentTaskScreen instance;
        public static CurrentTaskScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new CurrentTaskScreen();
                return instance;
            }
        }
        private CurrentTaskScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(640, 20), DataManager.Instance.exitBtnTxtr) { Click = BackToLevel });
            inputButton = new TouchButton(new Vector2(300, 100), DataManager.Instance.inputButton) { Click = GetAnswer };
            components.Add(inputButton);
            components.Add(new TouchButton(new Vector2(50, 400), DataManager.Instance.putLinkButton) { Click = CheckAnswer });
            components.Add(new TouchButton(new Vector2(400, 100), DataManager.Instance.putLinkButton) { Click = RedoTask });
        }

        public override void Unload()
        {

        }

        #endregion

        #region fields
        private new Game1 game;
        private List<TouchButton> components;
        private TouchButton inputButton;
        public Task task;
        private bool keyboradDisplayed;
        public string playerAnswer;
        public AnswerState answerState;
        #endregion

        public override void Initialize(Game game)
        {
            this.game = game as Game1;
            if (task.isSolved)
            {
                playerAnswer = task.answer;
                answerState = AnswerState.Right;
            }
            else
            {
                playerAnswer = "";
                answerState = AnswerState.Undefined;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (keyboradDisplayed)
            {
                inputButton.Update(gameTime);
                return;
            }
            foreach (var c in components)
            {
                c.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(Color.Violet);
            game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, task.description, new Vector2(100, 300), Color.Yellow);

            game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, playerAnswer, new Vector2(100, 100), Color.Black);

            if (answerState == AnswerState.Right)
            {
                game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, "ВЕРНО", new Vector2(100, 50), Color.Green);
            }
            else if (answerState == AnswerState.Wrong)
            {
                game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, "НЕВЕРНО", new Vector2(100, 50), Color.Red);
            }

            foreach (var c in components)
            {
                c.Draw(game.spriteBatch);
            }
        }

        private void BackToLevel(object sender, EventArgs e)
        {
            game.ReturnToState(StartEducationalScreen.Instance);
        }


        private void GetAnswer(object sender, EventArgs e)
        {
            var pView = game.Services.GetService<View>();
            if (!keyboradDisplayed)
            {
                var inputMethodManager = game.gameActivity.Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                inputMethodManager.ShowSoftInput(pView, ShowFlags.Forced);
                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                keyboradDisplayed = true;
            }
            else
            {
                InputMethodManager inputMethodManager = game.gameActivity.Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
                keyboradDisplayed = false;
            }
        }
        
        public void CheckAnswer(object sender, EventArgs e)
        {
            var pView = game.Services.GetService<View>();
            if (keyboradDisplayed)
            {
                InputMethodManager inputMethodManager = game.gameActivity.Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
                keyboradDisplayed = false;
            }
            if (answerState != AnswerState.Undefined)
                return;
            answerState = (playerAnswer == task.answer) ? AnswerState.Right : AnswerState.Wrong;
            task.isSolved = answerState == AnswerState.Right;
            if (answerState == AnswerState.Right)
            {
                PlayerData.Instance.points += task.points;
            }
        }

        public void CheckAnswer()
        {
            var pView = game.Services.GetService<View>();
            if (keyboradDisplayed)
            {
                InputMethodManager inputMethodManager = game.gameActivity.Application.GetSystemService(Context.InputMethodService) as InputMethodManager;
                inputMethodManager.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
                keyboradDisplayed = false;
            }
            if (answerState != AnswerState.Undefined)
                return;

            answerState = (playerAnswer == task.answer) ? AnswerState.Right : AnswerState.Wrong;
            task.isSolved = answerState == AnswerState.Right;
            if (answerState == AnswerState.Right)
            {
                PlayerData.Instance.points += task.points;
            }
        }


        private void RedoTask(object sender, EventArgs e)
        {
            if (task.isGenerated)
            {
                task = Task.Generate(task.generatedType);
                playerAnswer = "";
                answerState = AnswerState.Undefined;
            }
            else if(answerState == AnswerState.Wrong)
            {
                playerAnswer = "";
                answerState = AnswerState.Undefined;
            }
        }
    }
}