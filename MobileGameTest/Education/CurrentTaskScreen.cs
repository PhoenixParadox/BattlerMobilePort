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
            components.Add(new TouchButton(new Vector2(700, 70), DataManager.Instance.exitBtnTxtr) { Click = BackToLevel });
            inputButton = new TouchButton(new Vector2(580, 455), DataManager.Instance.inputButton) { Click = GetAnswer };
            components.Add(inputButton);
            components.Add(new TouchButton(new Vector2(420, 800), DataManager.Instance.checkAnswerBtn) { Click = CheckAnswer });
            components.Add(new TouchButton(new Vector2(600, 800), DataManager.Instance.redoTaskBtn) { Click = RedoTask });

            components.Add(new TouchButton(new Vector2(550, 1100), DataManager.Instance.nextTaskBtn) { Click = NextTask });
            components.Add(new TouchButton(new Vector2(70, 1100), DataManager.Instance.prevTaskBtn) { Click = PrevTask });
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
            if (this.game == null)
            {
                this.game = game as Game1;
            }
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
            game.spriteBatch.Draw(DataManager.Instance.educationBackground, new Vector2(0));
            game.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-50, 50));

            game.spriteBatch.Draw(DataManager.Instance.taskNameFrame, new Vector2(180, 10));
            game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, StartEducationalScreen.Instance.lastTaskName, new Vector2(265, 5), Color.White);

            game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, task.description, new Vector2(10, 120), Color.White);
            game.spriteBatch.Draw(DataManager.Instance.textBox, new Vector2(270, 450));
            game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, "ОТВЕТ:", new Vector2(60, 460), Color.White);
            game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, playerAnswer, new Vector2(290, 460), Color.Brown);

            game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, "СТАТУС:", new Vector2(30, 590), Color.White);

            if (answerState == AnswerState.Right)
            {
                game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, "ВЕРНО", new Vector2(220, 590), Color.Green);
                game.spriteBatch.Draw(DataManager.Instance.correctIcon, new Vector2(350, 570));
            }
            else if (answerState == AnswerState.Wrong)
            {
                game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, "НЕВЕРНО", new Vector2(220, 590), Color.Red);
                game.spriteBatch.Draw(DataManager.Instance.incorrectIcon, new Vector2(400, 580));
            }
            else
            {
                game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, "НЕ РЕШЕНО", new Vector2(220, 590), Color.AliceBlue);
            }
            if (!task.isSolved)
            {
                game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, $"ТЫ ПОЛУЧИШЬ:\n{task.points}", new Vector2(20, 780), Color.Yellow);
                game.spriteBatch.Draw(DataManager.Instance.coinIcon, new Vector2(70, 810), scale: new Vector2(0.7f));
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
            if (task.isSolved)
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
            if (task.isSolved)
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
            if (answerState == AnswerState.Wrong)
            {
                playerAnswer = "";
                answerState = AnswerState.Undefined;
            }
            else if (task.isGenerated)
            {
                task = Task.Generate(task.generatedType);
                playerAnswer = "";
                answerState = AnswerState.Undefined;
            }
        }

        private void NextTask(object sender, EventArgs e)
        {
            StartEducationalScreen.Instance.GoToNextTask();
        }

        private void PrevTask(object sender, EventArgs e)
        {
            StartEducationalScreen.Instance.GoToPrevTask();
        }
    }
}