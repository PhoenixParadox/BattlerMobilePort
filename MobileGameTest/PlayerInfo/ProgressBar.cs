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
using MobileGameTest.Battle;
using MobileGameTest.Data;

namespace MobileGameTest.PlayerInfo
{
    public class Goal
    {
        public string description;
        public int cost;
        public Action action;
    }

    public class ProgressBar
    {
        private Tuple<int, int> ms;
        public Tuple<int, int> MileStones
        {
            get
            {
                return ms;
            }
            set
            {
                ms = value;
            }
        }
        public Vector2 position;
        //public Texture2D texture;
        public int costCoeffitient { get { return ms.Item2 * 5; } }
        public Goal[] goals;
        public int currentGoal;
        public Point GoalOffset { get { return new Point(DataManager.Instance.goalTxtr.Width / 2, DataManager.Instance.goalTxtr.Height / 2); } }
        public Vector2 currentGoalPosition;
        public Vector2 currentGoalCostPosition;
        public Vector2 m1Pos;
        public Vector2 m2Pos;
        public int width { get { return DataManager.Instance.progressBarTxtr.Width; } }

        public ProgressBar()
        {
            ms = Tuple.Create(GameData.Instance.milestones.m1, GameData.Instance.milestones.m2);
            SetGoals(ms.Item2);
            currentGoal = GameData.Instance.currentGoal;
            position = new Vector2(-20, 700);
            currentGoalPosition = new Vector2(50, 920);
            currentGoalCostPosition = new Vector2(400, 1100);
            m1Pos = new Vector2(85, 800);
            m2Pos = new Vector2(650, 805);
        }

        public void SetGoals(int currentGoal)
        {
            goals = new Goal[6];
            if (currentGoal == 2)
            {
                goals[0] = new Goal() { description = "дополнительное здоровье", cost = 10, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[1] = new Goal() { description = "доплнительное здоровье", cost = 15, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 20, action = () => { GameData.Instance.baseAtckDmg++; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 25, action = () => { GameData.Instance.baseDefAmount++; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительное здоровье", cost = 30, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[5] = new Goal()
                {
                    description = "открой Взмах мечом",
                    cost = 35,
                    action = () =>
                    {
                        if (!PlayerData.Instance.playerCollection.Contains(TalantType.SwordSwing))
                        {
                            PlayerData.Instance.playerCollection.Add(TalantType.SwordSwing);
                            Player1.Instance.Collection.Add(TalantFactory.GetTalant(TalantType.SwordSwing));
                            Player1.Instance.Reload();
                        }
                    }
                };
            }
            else if (currentGoal == 3)
            {
                goals[0] = new Goal() { description = "дополнительное здоровье", cost = 20, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[1] = new Goal() { description = "дополнительное здоровье", cost = 25, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 30, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 35, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительное здоровье", cost = 40, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 45,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
            else if (currentGoal == 4)
            {
                goals[0] = new Goal() { description = "дополнительное здоровье", cost = 30, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[1] = new Goal() { description = "дополнительное здоровье", cost = 35, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 40, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 45, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительное здоровье", cost = 50, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[5] = new Goal()
                {
                    description = "увеличенная кратность\nВзмаха мечом",
                    cost = 55,
                    action = () => { GameData.Instance.swordSwingMultiplicity++; Player1.Instance.Reload(); }
                };
            }
            else
            {
                goals[0] = new Goal() { description = "дополнительное здоровье", cost = 40, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[1] = new Goal() { description = "дополнительное здоровье", cost = 45, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 50, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 55, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительное здоровье", cost = 60, action = () => PlayerData.Instance.MaxHP += 4 };
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 65,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(DataManager.Instance.ProgressBar[currentGoal], position);
            var goalPos = new Vector2(position.X + width / 5 + currentGoal * 64, position.Y - 32);
            spriteBatch.Draw(DataManager.Instance.goalPanel, currentGoalPosition);

            spriteBatch.DrawString(DataManager.Instance.milestonesFont, MileStones.Item1.ToString(), new Vector2(m1Pos.X, m1Pos.Y), Color.BlueViolet);
            spriteBatch.DrawString(DataManager.Instance.milestonesFont, MileStones.Item2.ToString(), new Vector2(m2Pos.X, m2Pos.Y), Color.BlueViolet);
            spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, goals[currentGoal].description, new Vector2(currentGoalPosition.X + 25, currentGoalPosition.Y + 80), Color.White);
            spriteBatch.DrawString(DataManager.Instance.milestonesFont, (goals[currentGoal].cost + costCoeffitient).ToString(), new Vector2(currentGoalPosition.X + 180, currentGoalPosition.Y + 260), Color.White);
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}