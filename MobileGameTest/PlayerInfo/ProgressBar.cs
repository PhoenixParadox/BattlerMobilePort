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

        private Random rnd;

        public ProgressBar()
        {
            rnd = new Random();
            ms = Tuple.Create(GameData.Instance.milestones.m1, GameData.Instance.milestones.m2);
            //SetGoals(ms.Item2);
            CreateGoals();
            currentGoal = GameData.Instance.currentGoal;
            position = new Vector2(-20, 700);
            currentGoalPosition = new Vector2(50, 920);
            currentGoalCostPosition = new Vector2(400, 1100);
            m1Pos = new Vector2(75, 800);
            m2Pos = new Vector2(645, 805);
        }

        private Goal MakeHalthGoal(int cost, int increaseAmount)
        {
            return new Goal() { description = $"дополнительное здоровье\n{increaseAmount} HP", cost = cost, action = () => PlayerData.Instance.MaxHP += increaseAmount };
        }
        public void CreateGoals()
        {
            var hpUp = (int)Math.Max(1, Player1.Instance.MaxHP * 0.02);
            goals = new Goal[6];
            goals[0] = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 0, hpUp);
            goals[1] = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 1, hpUp);
            goals[4] = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 4, hpUp);
            Goal g = null;
            if (MileStones.Item2 % 5 == 0)
            {
                var temp = GameData.GoldTier.Intersect(PlayerData.Instance.playerCollection).ToList();
                if (temp.Count == 0)
                {
                    g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 2, hpUp);
                }
                else
                {
                    var t = temp[rnd.Next(0, temp.Count)]; // случайный "золотой" талант, имеющийся у игрока
                    var tal = TalantFactory.GetTalant(t);
                    g = new Goal { description = $"улучшить\n{tal.name}", cost = MileStones.Item1 * 10 + MileStones.Item2 * 2, action = () => { GameData.Instance.UpgradeAction(t); Player1.Instance.Reload(); } };
                }
            }
            else if (MileStones.Item2 % 3 == 0)
            {
                var temp = GameData.SilverTier.Intersect(PlayerData.Instance.playerCollection).ToList();
                if (temp.Count == 0)
                {
                    g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 2, hpUp);
                }
                else
                {
                    var t = temp[rnd.Next(0, temp.Count)]; // случайный "серебряный" талант, имеющийся у игрока
                    var tal = TalantFactory.GetTalant(t);
                    g = new Goal { description = $"улучшить\n{tal.name}", cost = MileStones.Item1 * 10 + MileStones.Item2 * 2, action = () => { GameData.Instance.UpgradeAction(t); Player1.Instance.Reload(); } };
                }
            }
            else if (MileStones.Item2 % 2 == 0)
            {
                var temp = GameData.BronzeTier.Intersect(PlayerData.Instance.playerCollection).ToList();
                if (temp.Count == 0)
                {
                    g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 2, hpUp);
                }
                else
                {
                    var t = temp[rnd.Next(0, temp.Count)]; // случайный "бронзовый" талант, имеющийся у игрока
                    var tal = TalantFactory.GetTalant(t);
                    g = new Goal { description = $"улучшить\n{tal.name}", cost = MileStones.Item1 * 10 + MileStones.Item2 * 2, action = () => { GameData.Instance.UpgradeAction(t); Player1.Instance.Reload(); } };
                }
            }
            else
            {
                g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 2, hpUp);
            }
            goals[2] = g;
            if (MileStones.Item2 % 10 == 0)
            {
                var t = PlayerData.Instance.playerCollection[rnd.Next(0, Player1.Instance.Collection.Count)];
                var tal = TalantFactory.GetTalant(t);
                g = new Goal() { description = $"увеличить кратность\n{tal.name}", cost = MileStones.Item1 * 10 + MileStones.Item2 * 3, action = () => { GameData.Instance.UpgradeMultiplicity(t); Player1.Instance.Reload(); } };
            }
            else
            {
                g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 3, hpUp);
            }
            goals[3] = g;
            if (MileStones.Item2 % 5 == 0)
            {
                if (PlayerData.Instance.playerCollection.Count == GameData.AllTalants.Keys.Count)
                {
                    // у игрока открыты все доступные таланты
                    g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 5, hpUp);
                }
                else
                {
                    var temp = GameData.AllTalants.Keys.Where(k => !PlayerData.Instance.playerCollection.Contains(k)).ToList();
                    var t = temp[rnd.Next(0, temp.Count)]; // случайный отсутствующий у игрока талант
                    var tal = TalantFactory.GetTalant(t);
                    g = new Goal() { description = $"открой\n{tal.name}", cost = MileStones.Item1 * 10 + MileStones.Item2 * 5, action = () => { PlayerData.Instance.AddTalant(t); Player1.Instance.AddTalant(t); } };
                }
            }
            else
            {
                g = MakeHalthGoal(MileStones.Item1 * 10 + MileStones.Item2 * 5, hpUp);
            }

            goals[5] = g;
        }

        /*
        public void SetGoals(int currentGoal)
        {
            goals = new Goal[6];
            if (currentGoal == 2)
            {
                goals[0] = MakeHalthGoal(10, 1);
                goals[1] = MakeHalthGoal(15, 1);
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 20, action = () => { GameData.Instance.baseAtckDmg++; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 25, action = () => { GameData.Instance.baseDefAmount++; Player1.Instance.Reload(); } };
                goals[4] = MakeHalthGoal(30, 1);
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
                goals[0] = MakeHalthGoal(20, 2);
                goals[1] = MakeHalthGoal(25, 2);
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 30, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 35, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = MakeHalthGoal(40, 2);
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 45,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
            else if (currentGoal == 4)
            {
                goals[0] = MakeHalthGoal(30, 2);
                goals[1] = MakeHalthGoal(35, 2);
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 40, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 45, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = MakeHalthGoal(50, 3);
                goals[5] = new Goal()
                {
                    description = "увеличенная кратность\nВзмаха мечом",
                    cost = 55,
                    action = () => { GameData.Instance.swordSwingMultiplicity++; Player1.Instance.Reload(); }
                };
            }
            else
            {
                goals[0] = MakeHalthGoal(40, 3);
                goals[1] = MakeHalthGoal(45, 3);
                goals[2] = new Goal() { description = "базовая атака наносит\nбольше урона", cost = 50, action = () => { GameData.Instance.baseAtckDmg += 2; Player1.Instance.Reload(); } };
                goals[3] = new Goal() { description = "базовая защита дает\nбольше DEF", cost = 55, action = () => { GameData.Instance.baseDefAmount += 2; Player1.Instance.Reload(); } };
                goals[4] = MakeHalthGoal(60, 3);
                goals[5] = new Goal()
                {
                    description = "улучшенный Взмах мечом",
                    cost = 65,
                    action = () => { GameData.Instance.strongAtckDmg += 2; Player1.Instance.Reload(); }
                };
            }
        }
        */

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