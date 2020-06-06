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
using MobileGameTest.Components;
using MobileGameTest.Data;
using MobileGameTest.Menu;
using MobileGameTest.SourceLib;

namespace MobileGameTest.PlayerInfo
{
    // экран информации игрока
    public class PlayerInfoScreen : GameState
    {
        private enum PlayerInfoTabs
        {
            Collection,
            Deck,
            Progress,
            PickingTalant
        }

        private static PlayerInfoScreen instance;
        public static PlayerInfoScreen Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlayerInfoScreen();
                }
                return instance;
            }
        }



        private new Game1 game;
        private List<TouchButton> components;
        private PlayerInfoTabs activeTab;
        public ProgressBar progressBar;
        private InvisibleButton goalButton;
        private TouchButton[] tabs;
        private InvisibleButton[] TalantPickButtons;
        private InvisibleButton[] AddTalantButtons;
        private int TalantToReplaceInd;
        private int initialCollectionSize;

        private PlayerInfoScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(550, 30), DataManager.Instance.exitBtnTxtr) { Click = BackToMenu });
            components.Add(new TouchButton(new Vector2(150, 300), DataManager.Instance.rightArrow) { Click = SwitchSkinRight });
            components.Add(new TouchButton(new Vector2(20, 300), DataManager.Instance.leftArrow) { Click = SwitchSkinLeft });
            activeTab = PlayerInfoTabs.Progress;
            progressBar = new ProgressBar();
            progressBar.MileStones = Tuple.Create(GameData.Instance.milestones.m1, GameData.Instance.milestones.m2);
            progressBar.currentGoal = GameData.Instance.currentGoal;
            goalButton = new InvisibleButton(new Rectangle((int)progressBar.currentGoalPosition.X, (int)progressBar.currentGoalPosition.Y, DataManager.Instance.goalTxtr.Width, DataManager.Instance.goalTxtr.Height));
            goalButton.Click += goalClick;

            tabs = new TouchButton[3];
            tabs[0] = new TouchButton(new Vector2(10, 360), DataManager.Instance.woodenBtn);
            tabs[1] = new TouchButton(new Vector2(10 + 20 + DataManager.Instance.woodenBtn.Width, 360), DataManager.Instance.woodenBtn);
            tabs[2] = new TouchButton(new Vector2(10 + 40 + DataManager.Instance.woodenBtn.Width * 2, 360), DataManager.Instance.woodenBtn);
            tabs[0].Click = (object sender, EventArgs e) => this.activeTab = PlayerInfoTabs.Progress;
            tabs[1].Click = (object sender, EventArgs e) => this.activeTab = PlayerInfoTabs.Collection;
            tabs[2].Click = (object sender, EventArgs e) => this.activeTab = PlayerInfoTabs.Deck;


            TalantPickButtons = new InvisibleButton[12];
            var i = 50;
            var j = 770;
            var ind = 0;
            foreach (var t in Player1.Instance.Deck)
            {
                var b = new InvisibleButton(new Rectangle(i, j, 100, 96));
                b.Click += DeckTalantClick;
                TalantPickButtons[ind++] = b;
                i = (i >= 480) ? 50 : i + 180;
                j = (i == 50) ? j + 150 : j;
            }
            MakeAddtalantButtons();
        }

        public override void Draw(GameTime gameTime)
        {
            game.spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
            game.spriteBatch.Draw(DataManager.Instance.campBackgroundTxtr, new Vector2(0, -40), scale: new Vector2(1.5f));
            game.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-30, 700));
            var text = Player1.Instance.name + "\nHP " + Player1.Instance.MaxHP + "\nталантов собрано " + Player1.Instance.Collection.Count;
            game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, text, new Vector2(300, 128), Color.White);
            foreach (var e in components)
                e.Draw(game.spriteBatch);
            foreach (var t in tabs)
            {
                t.Draw(game.spriteBatch);
            }
            Player1.Instance.sprite.Position = new Vector2(30, 30);
            Player1.Instance.sprite.scale = 0.7f;
            Player1.Instance.sprite.spriteEffects = SpriteEffects.None;
            Player1.Instance.sprite.Draw(game.spriteBatch);
            switch (activeTab)
            {
                case (PlayerInfoTabs.Progress):
                    progressBar.Draw(game.spriteBatch);
                    break;
                case (PlayerInfoTabs.Deck):
                    var i = 50;
                    var j = 770;
                    foreach (var t in Player1.Instance.Deck)
                    {
                        game.spriteBatch.Draw(t.txtr, new Vector2(i, j));
                        i = (i >= 480) ? 50 : i + 180;
                        j = (i == 50) ? j + 150 : j;
                    }
                    break;
                case (PlayerInfoTabs.Collection):
                    var k = 90;
                    var n = 770;
                    var index = 0;
                    foreach (var t in Player1.Instance.Collection)
                    {
                        game.spriteBatch.Draw(t.txtr, new Vector2(k, n));
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, t.description, new Vector2(k + 104, n + 10), Color.White);
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, Player1.Instance.Collection[index].multiplicity.ToString(), new Vector2(k - 30, n + 10), Color.White);
                        n += 150;
                        index++;
                    }
                    break;
                case (PlayerInfoTabs.PickingTalant):
                    var l = 90;
                    var m = 770;
                    var ind = 0;
                    foreach (var t in Player1.Instance.Collection)
                    {
                        game.spriteBatch.Draw(t.txtr, new Vector2(l, m));
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, t.description, new Vector2(l + 104, m + 10), Color.White);
                        game.spriteBatch.DrawString(DataManager.Instance.localizedMenuFont, NumberOfAvailableCopies(ind).ToString(), new Vector2(l - 30, m + 10), Color.White);
                        m += 150;
                        ind++;
                    }
                    break;
            }
        }

        public override void Initialize(Game game)
        {
            if (this.game == null)
            {
                this.game = game as Game1;
            }
            Player1.Instance.sprite.Position = new Vector2(30, 30);
            Player1.Instance.sprite.scale = 0.7f;
            Player1.Instance.sprite.spriteEffects = SpriteEffects.None;
            initialCollectionSize = Player1.Instance.Collection.Count;
        }

        private void MakeAddtalantButtons()
        {
            AddTalantButtons = new InvisibleButton[Player1.Instance.Collection.Count];
            var k = 90;
            var n = 770;
            var ind = 0;
            foreach (var t in Player1.Instance.Collection)
            {
                var b = new InvisibleButton(new Rectangle(k, n, 100, 96));
                b.Click += AddButton;
                AddTalantButtons[ind++] = b;
                n += 150;
            }
        }

        private int NumberOfAvailableCopies(int talantInd)
        {
            var res = 0;
            for (int i = 0; i < 12; i++)
            {
                if (Player1.Instance.Deck[i].type == Player1.Instance.Collection[talantInd].type && i != TalantToReplaceInd)
                {
                    res++;
                }
            }
            return Player1.Instance.Collection[talantInd].multiplicity - res;
        }

        private void AddButton(object sender, EventArgs e)
        {
            var talantToPlace = 0;
            for (int i = 0; i < 12; i++)
            {
                if (AddTalantButtons[i] == ((InvisibleButton)sender))
                {
                    talantToPlace = i;
                    break;
                }
            }
            if (NumberOfAvailableCopies(talantToPlace) > 0)
            {
                Player1.Instance.Deck[TalantToReplaceInd] = TalantFactory.GetTalant(Player1.Instance.Collection[talantToPlace].type);
                Player1.Instance.Reload();
            }

            activeTab = PlayerInfoTabs.Deck;
        }

        private void DeckTalantClick(object sender, EventArgs e)
        {
            activeTab = PlayerInfoTabs.PickingTalant;
            for (int i = 0; i < 12; i++)
            {
                if (TalantPickButtons[i] == ((InvisibleButton)sender))
                {
                    TalantToReplaceInd = i;
                    break;
                }
            }
            //Player1.Instance.Deck.RemoveAt(j);
            //Player1.Instance.Deck[j] = TalantFactory.GetTalant(TalantType.BaseAttack);
            //Player1.Instance.Reload();
        }

        private void SwitchSkinLeft(object sender, EventArgs e)
        {
            if (PlayerData.Instance.currentSkin == 0)
            {
                PlayerData.Instance.currentSkin = PlayerData.Instance.maxUnlockedSkin;
            }
            else
            {
                PlayerData.Instance.currentSkin--;
            }
            Player1.Instance.Initialize();
        }

        private void SwitchSkinRight(object sender, EventArgs e)
        {
            if (PlayerData.Instance.currentSkin == PlayerData.Instance.maxUnlockedSkin)
            {
                PlayerData.Instance.currentSkin = 0;
            }
            else
            {
                PlayerData.Instance.currentSkin++;
            }
            Player1.Instance.Initialize();
        }

        private void goalClick(object sender, EventArgs e)
        {
            if (Player1.Instance.points < progressBar.goals[progressBar.currentGoal].cost + progressBar.costCoeffitient)
            {
                return;
            }
            progressBar.goals[progressBar.currentGoal].action();
            if (progressBar.currentGoal + 1 == progressBar.goals.Length)
            {
                progressBar.MileStones = Tuple.Create(progressBar.MileStones.Item2, progressBar.MileStones.Item2 + 1);
                GameData.Instance.milestones.m1 = progressBar.MileStones.Item1;
                GameData.Instance.milestones.m2 = progressBar.MileStones.Item2;
            }
            progressBar.currentGoal = (progressBar.currentGoal + 1) % progressBar.goals.Length;
            GameData.Instance.currentGoal = progressBar.currentGoal;
            PlayerData.Instance.points -= progressBar.goals[progressBar.currentGoal].cost + progressBar.costCoeffitient;
        }

        private void BackToMenu(object sender, EventArgs e)
        {
            game.SwitchState(MainMenu.Instance);
        }

        public override void Unload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (Player1.Instance.Collection.Count != initialCollectionSize)
            {
                MakeAddtalantButtons();
                initialCollectionSize++;
            }

            foreach (var e in components)
                e.Update(gameTime);
            foreach (var t in tabs)
                t.Update(gameTime);
            if (activeTab == PlayerInfoTabs.Deck)
            {
                foreach (var t in TalantPickButtons)
                {
                    t.Update(gameTime);
                }
            }
            if (activeTab == PlayerInfoTabs.Progress)
            {
                goalButton.Update(gameTime);
            }
            if (activeTab == PlayerInfoTabs.PickingTalant)
            {
                foreach (var t in AddTalantButtons)
                {
                    t.Update(gameTime);
                }
            }
        }
    }
}