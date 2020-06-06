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
            #region goals
            goals = new Goal[6];
            if (ms.Item2 == 2)
            {
                goals[0] = new Goal() { description = "дополнительно здоровье", cost = 10, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[1] = new Goal() { description = "доплнительное здоровье", cost = 15, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[2] = new Goal() { description = "базовая атака наносит больше урона", cost = 20, action = () => { GameData.Instance.baseAtckDmg++; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает больше DEF", cost = 25, action = () => { GameData.Instance.baseDefAmount++; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительное здоровье", cost = 30, action = () => PlayerData.Instance.MaxHP += 1 };
                goals[5] = new Goal()
                {
                    description = "открой Взмах мечом",
                    cost = 35,
                    action = () =>
                    {
                        if (!PlayerData.Instance.playerCollection.Contains(TalantType.SwordSwing))
                        {
                            Player1.Instance.Collection.Add(TalantFactory.GetTalant(TalantType.SwordSwing));
                            Player1.Instance.Reload();
                            PlayerData.Instance.playerCollection.Add(TalantType.SwordSwing);
                        }
                    }
                };
            }
            else if (ms.Item2 == 3)
            {
                goals[0] = new Goal() { description = "дополнительно здоровье", cost = 20, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[1] = new Goal() { description = "дополнительно здоровье", cost = 25, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[2] = new Goal() { description = "базовая атака наносит больше урона", cost = 30, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает больше DEF", cost = 35, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительно здоровье", cost = 40, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 45,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
            else if (ms.Item2 == 4)
            {
                goals[0] = new Goal() { description = "дополнительно здоровье", cost = 30, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[1] = new Goal() { description = "дополнительно здоровье", cost = 35, action = () => PlayerData.Instance.MaxHP += 2 };
                goals[2] = new Goal() { description = "базовая атака наносит больше урона", cost = 40, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает больше DEF", cost = 45, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительно здоровье", cost = 50, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[5] = new Goal()
                {
                    description = "увеличенная кратность Взмаха мечом",
                    cost = 55,
                    action = () => { GameData.Instance.swordSwingMultiplicity++; Player1.Instance.Reload(); }
                };
            }
            else
            {
                goals[0] = new Goal() { description = "дополнительно здоровье", cost = 40, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[1] = new Goal() { description = "дополнительно здоровье", cost = 45, action = () => PlayerData.Instance.MaxHP += 3 };
                goals[2] = new Goal() { description = "базовая атака наносит больше урона", cost = 50, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает больше DEF", cost = 55, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = new Goal() { description = "дополнительно здоровье", cost = 60, action = () => PlayerData.Instance.MaxHP += 4 };
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 65,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
            #endregion

            currentGoal = GameData.Instance.currentGoal;
            position = new Vector2(160, 620);
            currentGoalPosition = new Vector2(288, 764);
            currentGoalCostPosition = new Vector2(302, 784);
            m1Pos = new Vector2(64, 600);
            m2Pos = new Vector2(500, 600);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(DataManager.Instance.milestoneIcon, m1Pos, scale: new Vector2(0.5f));
            spriteBatch.Draw(DataManager.Instance.milestoneIcon, m2Pos, scale: new Vector2(0.5f));

            spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, MileStones.Item1.ToString(), new Vector2(m1Pos.X + 25, m1Pos.Y + 20), Color.BlueViolet);
            spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, MileStones.Item2.ToString(), new Vector2(m2Pos.X + 25, m2Pos.Y + 20), Color.BlueViolet);
            //spriteBatch.Draw(DataManager.Instance.progressBarTxtr, position);
            spriteBatch.Draw(DataManager.Instance.ProgressBar[currentGoal], position);
            var goalPos = new Vector2(position.X + width / 5 + currentGoal * 64, position.Y - 32);
            spriteBatch.Draw(DataManager.Instance.goalTxtr, currentGoalPosition);
            spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, goals[currentGoal].description, new Vector2(currentGoalPosition.X - 80, currentGoalPosition.Y + 80), Color.BlueViolet);
            spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, (goals[currentGoal].cost + costCoeffitient).ToString(), currentGoalCostPosition, Color.BlueViolet);
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}