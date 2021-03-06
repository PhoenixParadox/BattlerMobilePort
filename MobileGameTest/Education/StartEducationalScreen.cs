﻿using System;
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
using MobileGameTest.Menu;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Education
{
    public class StartEducationalScreen : GameState
    {
        #region fields
        private new Game1 game;
        private List<TouchButton> components;

        private EducationalLevel currentLevel;
        private TouchButton[] theoryTasksBtns;
        private TouchButton[] mathTasksBtns;
        private TouchButton[] calcTasksBtns;

        private int lastTaskInd;
        private Task lastTask;
        public string lastTaskName
        {
            get
            {
                var str = (lastTask.type == TaskType.Theory) ? "Задание " : (lastTask.type == TaskType.Math) ? "Задача " : "Пример ";
                str += (lastTaskInd + 1);
                return str;
            }
        }
        #endregion

        #region creation
        private static StartEducationalScreen instance;
        public static StartEducationalScreen Instance
        {
            get
            {
                if (instance == null)
                    instance = new StartEducationalScreen();
                return instance;
            }
        }
        

        private StartEducationalScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(700, 10), DataManager.Instance.exitBtnTxtr) { Click = BackToMenu });
            components.Add(new TouchButton(new Vector2(260, 140), DataManager.Instance.scroll) { Click = ShowTheory });
            components.Add(new TouchButton(new Vector2(20, 20), DataManager.Instance.infoIcon) { Click = ShowInfo });
        }

        public override void Unload()
        {

        }
        #endregion

        public override void Initialize(Game game)
        {
            if(this.game == null)
                this.game = game as Game1;

            currentLevel = EducationData.Instance.CurrentLevel;
            MakeTaskButtons();
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
            game.GraphicsDevice.Clear(Color.Azure);
            game.spriteBatch.Draw(DataManager.Instance.educationBackground, new Vector2(0));
            game.spriteBatch.Draw(DataManager.Instance.levels[EducationData.Instance.currentLevel], new Vector2(100, 5));

            game.spriteBatch.Draw(DataManager.Instance.theoryTasksFrame, new Vector2(180, 320));
            game.spriteBatch.Draw(DataManager.Instance.mathTasksFrame, new Vector2(180, 770));
            game.spriteBatch.Draw(DataManager.Instance.calcTasksFrame, new Vector2(180, 990));
            foreach (var c in components)
            {
                c.Draw(game.spriteBatch);
            }

            var pos = new Vector2(280, 390);
            var lvl = EducationData.Instance.CurrentLevel;
            for (int i = 0; i < theoryTasksBtns.Length; i++)
            {
                if (lvl.TheoryTasks[i].isSolved)
                {
                    game.spriteBatch.Draw(DataManager.Instance.correctIcon, new Vector2(pos.X + 220, pos.Y - 10));
                }
                if (lvl.TheoryTasks[i].isGenerated)
                {
                    game.spriteBatch.Draw(DataManager.Instance.redoIcon, new Vector2(pos.X - 70, pos.Y - 5));
                }
                pos.Y += 60;
            }
            var pos1 = new Vector2(280, 830);
            for (int i = 0; i < mathTasksBtns.Length; i++)
            {
                if (lvl.MathTasks[i].isSolved)
                {
                    game.spriteBatch.Draw(DataManager.Instance.correctIcon, new Vector2(pos1.X + 220, pos1.Y - 10));
                }
                if (lvl.MathTasks[i].isGenerated)
                {
                    game.spriteBatch.Draw(DataManager.Instance.redoIcon, new Vector2(pos1.X - 70, pos1.Y - 5));
                }
                pos1.Y += 60;
            }
            var pos2 = new Vector2(280, 1050);
            for (int i = 0; i < calcTasksBtns.Length; i++)
            {
                if (lvl.CalculationTasks[i].isSolved)
                {
                    game.spriteBatch.Draw(DataManager.Instance.correctIcon, new Vector2(pos2.X + 220, pos2.Y - 10));
                }
                if (lvl.CalculationTasks[i].isGenerated)
                {
                    game.spriteBatch.Draw(DataManager.Instance.redoIcon, new Vector2(pos2.X - 70, pos2.Y - 5));
                }
                pos2.Y += 60;
            }
        }

        private void BackToMenu(object sender, EventArgs e)
        {
            game.SwitchState(MainMenu.Instance);
        }

        private void ShowTheory(object sender, EventArgs e)
        {
            game.SwitchState(TheoryScreen.Instance);
        }

        private void MakeTaskButtons()
        {
            var pos = new Vector2(280, 390);
            theoryTasksBtns = new TouchButton[currentLevel.TheoryTasks.Count];
            for (int i = 0; i < theoryTasksBtns.Length; i++)
            {
                theoryTasksBtns[i] = new TouchButton(pos, DataManager.Instance.theoryTasks[i]) { Click = SolveTask };
                components.Add(theoryTasksBtns[i]);
                pos.Y += 60;
            }
            var pos1 = new Vector2(280, 830);
            mathTasksBtns = new TouchButton[currentLevel.MathTasks.Count];
            for (int i = 0; i < mathTasksBtns.Length; i++)
            {
                mathTasksBtns[i] = new TouchButton(pos1, DataManager.Instance.mathTasks[i]) { Click = SolveTask };
                components.Add(mathTasksBtns[i]);
                pos1.Y += 60;
            }
            var pos2 = new Vector2(280, 1050);
            calcTasksBtns = new TouchButton[currentLevel.CalculationTasks.Count];
            for (int i = 0; i < calcTasksBtns.Length; i++)
            {
                calcTasksBtns[i] = new TouchButton(pos2, DataManager.Instance.calcTasks[i]) { Click = SolveTask };
                components.Add(calcTasksBtns[i]);
                pos2.Y += 60;
            }
        }

        private void SolveTask(object sender, EventArgs e)
        {
            var btn = sender as TouchButton;
            if (theoryTasksBtns.Contains(btn))
            {
                var ind = 0;
                for(int i = 0; i < theoryTasksBtns.Length; i++)
                {
                    if (theoryTasksBtns[i] == btn)
                    {
                        ind = i;
                        break;
                    }
                }
                lastTaskInd = ind;
                CurrentTaskScreen.Instance.task = EducationData.Instance.CurrentLevel.TheoryTasks[ind];
                //game.SwitchState(CurrentTaskScreen.Instance);
            }
            else if (mathTasksBtns.Contains(btn))
            {
                var ind = 0;
                for (int i = 0; i < mathTasksBtns.Length; i++)
                {
                    if (mathTasksBtns[i] == btn)
                    {
                        ind = i;
                        break;
                    }
                }
                lastTaskInd = ind;
                CurrentTaskScreen.Instance.task = EducationData.Instance.CurrentLevel.MathTasks[ind];
                //game.SwitchState(CurrentTaskScreen.Instance);
            }
            else if (calcTasksBtns.Contains(btn))
            {
                var ind = 0;
                for (int i = 0; i < calcTasksBtns.Length; i++)
                {
                    if (calcTasksBtns[i] == btn)
                    {
                        ind = i;
                        break;
                    }
                }
                lastTaskInd = ind;
                CurrentTaskScreen.Instance.task = EducationData.Instance.CurrentLevel.CalculationTasks[ind];
            }
            lastTask = CurrentTaskScreen.Instance.task;
            game.SwitchState(CurrentTaskScreen.Instance);
        }

        public void GoToNextTask()
        {
            var lvl = EducationData.Instance.CurrentLevel;
            switch (lastTask.type)
            {
                case (TaskType.Theory):
                    if (lastTaskInd == lvl.TheoryTasks.Count - 1)
                    {
                        CurrentTaskScreen.Instance.task = lvl.MathTasks[0];
                        lastTaskInd = 0;
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.TheoryTasks[++lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
                case (TaskType.Math):
                    if (lastTaskInd == lvl.MathTasks.Count - 1)
                    {
                        CurrentTaskScreen.Instance.task = lvl.CalculationTasks[0];
                        lastTaskInd = 0;
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.MathTasks[++lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
                case (TaskType.Calc):
                    if (lastTaskInd == lvl.CalculationTasks.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.CalculationTasks[++lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
            }
            CurrentTaskScreen.Instance.Initialize(game);
        }

        public void GoToPrevTask()
        {
            var lvl = EducationData.Instance.CurrentLevel;
            switch (lastTask.type)
            {
                case (TaskType.Theory):
                    if (lastTaskInd == 0)
                    {
                        return;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.TheoryTasks[--lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
                case (TaskType.Math):
                    if (lastTaskInd == 0)
                    {
                        CurrentTaskScreen.Instance.task = lvl.TheoryTasks[lvl.TheoryTasks.Count - 1];
                        lastTaskInd = lvl.TheoryTasks.Count - 1;
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.MathTasks[--lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
                case (TaskType.Calc):
                    if (lastTaskInd == 0)
                    {
                        CurrentTaskScreen.Instance.task = lvl.MathTasks[lvl.MathTasks.Count - 1];
                        lastTaskInd = lvl.MathTasks.Count - 1;
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    else
                    {
                        CurrentTaskScreen.Instance.task = lvl.CalculationTasks[--lastTaskInd];
                        lastTask = CurrentTaskScreen.Instance.task;
                    }
                    break;
            }
            CurrentTaskScreen.Instance.Initialize(game);
        }


        private void ShowInfo(object sender, EventArgs e)
        {
            game.SwitchState(EducationInfoScreen.Instance);
        }
    }
}