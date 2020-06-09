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
        private TouchButton goalButton;
        private TouchButton[] tabs;
        private InvisibleButton[] TalantPickButtons;
        private InvisibleButton[] AddTalantButtons;
        private int TalantToReplaceInd;
        private int initialCollectionSize;

        private int startCollectionViewInd;
        private IEnumerable<Talant> collectionView
        {
            get
            {
                var res = new List<Talant>();
                for (int i = 0; i < 3; i++)
                {
                    var ind = (startCollectionViewInd + i) % Player1.Instance.Collection.Count;
                    yield return Player1.instance.Collection[ind];
                }
            }
        }

        private TouchButton[] collectionViewButtons;

        private TouchButton[] savedDecks;
        private TouchButton saveCurrentDeck;

        private PlayerInfoScreen()
        {
            Load();
        }

        private void Load()
        {
            components = new List<TouchButton>();
            components.Add(new TouchButton(new Vector2(680, 20), DataManager.Instance.exitBtnTxtr) { Click = BackToMenu });
            components.Add(new TouchButton(new Vector2(240, 480), DataManager.Instance.rightArrow) { Click = SwitchSkinRight });
            components.Add(new TouchButton(new Vector2(20, 480), DataManager.Instance.leftArrow) { Click = SwitchSkinLeft });
            activeTab = PlayerInfoTabs.Progress;
            progressBar = new ProgressBar();
            progressBar.MileStones = Tuple.Create(GameData.Instance.milestones.m1, GameData.Instance.milestones.m2);
            progressBar.currentGoal = GameData.Instance.currentGoal;
            goalButton = new TouchButton(new Vector2(570, 960), DataManager.Instance.goalButton);
            goalButton.Click += goalClick;

            tabs = new TouchButton[3];
            tabs[0] = new TouchButton(new Vector2(10, 660), DataManager.Instance.progressButton);
            tabs[1] = new TouchButton(new Vector2(10 + 30 + DataManager.Instance.collectionButton.Width, 660), DataManager.Instance.collectionButton);
            tabs[2] = new TouchButton(new Vector2(10 + 60 + DataManager.Instance.deckButton.Width * 2, 660), DataManager.Instance.deckButton);
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

            collectionViewButtons = new TouchButton[2];
            collectionViewButtons[0] = new TouchButton(new Vector2(650, 790), DataManager.Instance.upButton);
            collectionViewButtons[0].Click += (sender, e) => startCollectionViewInd = (startCollectionViewInd + 1) % Player1.Instance.Collection.Count;
            collectionViewButtons[1] = new TouchButton(new Vector2(650, 1100), DataManager.Instance.downButton);
            collectionViewButtons[1].Click += (sender, e) => startCollectionViewInd = (startCollectionViewInd == 0) ? Player1.Instance.Collection.Count - 1 : startCollectionViewInd - 1;

            savedDecks = new TouchButton[3];
            savedDecks[0] = new TouchButton(new Vector2(50, 300), DataManager.Instance.deck1Btn);
            savedDecks[0].Click += SwitchDeck;
            savedDecks[1] = new TouchButton(new Vector2(270, 300), DataManager.Instance.deck2Btn);
            savedDecks[1].Click += SwitchDeck;
            savedDecks[2] = new TouchButton(new Vector2(490, 300), DataManager.Instance.deck3Btn);
            savedDecks[2].Click += SwitchDeck;

            saveCurrentDeck = new TouchButton(new Vector2(100, 100), DataManager.Instance.woodenBtn);
            saveCurrentDeck.Click += SaveDeck;
        }

        private void SaveDeck(object sender, EventArgs e)
        {
            
        }

        private void SwitchDeck(object sender, EventArgs e)
        {
            var ind = 0;
            for (int i = 0; i < 3; i++ )
            {
                if ((TouchButton)sender == savedDecks[i])
                {
                    ind = i;
                    break;
                }
            }
            Player1.Instance.currentDeck = ind + 1;
            Player1.Instance.SetDeck();
        }

        public override void Draw(GameTime gameTime)
        {
            game.spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
            game.spriteBatch.Draw(DataManager.Instance.campBackgroundTxtr, new Vector2(0, -40), scale: new Vector2(1.5f));
            game.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-35, 700));
            game.spriteBatch.Draw(DataManager.Instance.lowerPanelBorder, new Vector2(-15,700));

            foreach (var t in tabs)
            {
                t.Draw(game.spriteBatch);
            }

            switch (activeTab)
            {
                case (PlayerInfoTabs.Progress):
                    game.spriteBatch.Draw(DataManager.Instance.playerStats, new Vector2(350, 200));
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.name, new Vector2(410, 220), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.MaxHP.ToString(), new Vector2(520, 310), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.Collection.Count.ToString(), new Vector2(600, 390), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.points.ToString(), new Vector2(370, 480), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.trophies.ToString(), new Vector2(530, 480), Color.White);
                    Player1.Instance.sprite.Draw(game.spriteBatch);

                    foreach (var e in components)
                        e.Draw(game.spriteBatch);

                    progressBar.Draw(game.spriteBatch);
                    goalButton.Draw(game.spriteBatch);
                    break;
                case (PlayerInfoTabs.Deck):
                    foreach (var b in savedDecks)
                    {
                        b.Draw(game.spriteBatch);
                    }
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
                    game.spriteBatch.Draw(DataManager.Instance.playerStats, new Vector2(350, 200));
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.name, new Vector2(410, 220), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.MaxHP.ToString(), new Vector2(520, 310), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.Collection.Count.ToString(), new Vector2(600, 390), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.points.ToString(), new Vector2(370, 480), Color.White);
                    game.spriteBatch.DrawString(DataManager.Instance.goalDescriptionFont, Player1.Instance.trophies.ToString(), new Vector2(530, 480), Color.White);
                    Player1.Instance.sprite.Draw(game.spriteBatch);

                    foreach (var e in components)
                        e.Draw(game.spriteBatch);

                    foreach (var b in collectionViewButtons)
                    {
                        b.Draw(game.spriteBatch);
                    }
                    var k = 90;
                    var n = 770;
                    var index = 0;
                    foreach (var t in collectionView)
                    {
                        game.spriteBatch.Draw(t.txtr, new Vector2(k, n));
                        game.spriteBatch.DrawString(DataManager.Instance.playerMenuFont, t.description, new Vector2(k + 140, n + 20), Color.White);
                        game.spriteBatch.DrawString(DataManager.Instance.playerMenuFont, t.multiplicity.ToString(), new Vector2(k - 30, n + 30), Color.White);
                        n += 150;
                        index++;
                    }
                    break;
                case (PlayerInfoTabs.PickingTalant):
                    foreach (var b in collectionViewButtons)
                    {
                        b.Draw(game.spriteBatch);
                    }
                    var l = 90;
                    var m = 770;
                    var ind = startCollectionViewInd;
                    foreach (var t in collectionView)
                    {
                        game.spriteBatch.Draw(t.txtr, new Vector2(l, m));
                        game.spriteBatch.DrawString(DataManager.Instance.playerMenuFont, t.description, new Vector2(l + 140, m + 20), Color.White);
                        game.spriteBatch.DrawString(DataManager.Instance.playerMenuFont, NumberOfAvailableCopies(ind).ToString(), new Vector2(l - 30, m + 30), Color.White);
                        m += 150;
                        ind = (ind + 1) % Player1.Instance.Collection.Count;
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

            Player1.Instance.sprite.Position = new Vector2(50, 100);
            Player1.Instance.sprite.scale = 0.9f;
            Player1.Instance.sprite.spriteEffects = SpriteEffects.None;
            initialCollectionSize = Player1.Instance.Collection.Count;
        }

        private void MakeAddtalantButtons()
        {
            AddTalantButtons = new InvisibleButton[3];
            var k = 90;
            var n = 770;
            var ind = 0;
            for(int i = 0; i < 3; i++)
            {
                var b = new InvisibleButton(new Rectangle(k, n, 100, 96));
                b.Click += AddButton;
                AddTalantButtons[i] = b;
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
                    talantToPlace = (startCollectionViewInd + i) % Player1.Instance.Collection.Count;
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
            progressBar.SetGoals(progressBar.MileStones.Item2);
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


            foreach (var t in tabs)
                t.Update(gameTime);
            if (activeTab == PlayerInfoTabs.Collection)
            {
                foreach (var e in components)
                    e.Update(gameTime);
                foreach (var b in collectionViewButtons)
                {
                    b.Update(gameTime);
                }
            }
            if (activeTab == PlayerInfoTabs.Deck)
            {
                foreach (var b in savedDecks)
                {
                    b.Update(gameTime);
                }
                foreach (var t in TalantPickButtons)
                {
                    t.Update(gameTime);
                }
            }
            if (activeTab == PlayerInfoTabs.Progress)
            {
                foreach (var e in components)
                    e.Update(gameTime);
                goalButton.Update(gameTime);
            }
            if (activeTab == PlayerInfoTabs.PickingTalant)
            {
                foreach (var t in AddTalantButtons)
                {
                    t.Update(gameTime);
                }
                foreach (var b in collectionViewButtons)
                {
                    b.Update(gameTime);
                }
            }
        }
    }
}