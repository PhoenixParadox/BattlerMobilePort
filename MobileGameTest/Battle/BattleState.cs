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
using MobileGameTest.Components;
using MobileGameTest.Data;
using MobileGameTest.Menu;
using MobileGameTest.PlayerInfo;
using MobileGameTest.SourceLib;

namespace MobileGameTest.Battle
{
    public class BattleState
    {

        public enum Stage
        {
            Waiting,
            Choosing,
            Playing,
            FinishingRound
        }

        /// <summary>
        /// Основной класс для битвы.
        /// </summary>

        public class ChainBattleState : GameState
        {
            private static ChainBattleState instance;
            public static ChainBattleState Instance
            {
                get
                {
                    if (instance == null)
                        instance = new ChainBattleState();
                    return instance;
                }
            }

            #region methods
            //перемешать набор талантов
            private void Shuffle(List<Talant> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rnd.Next(n + 1);
                    var value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }

            //достать 3 таланта из добора в руку
            private void DrawTalants(InBattlePlayer plr)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (plr.drawStack.Count <= 0)
                    {
                        ClearDrop(plr);
                        Shuffle(plr.drawStack);
                    }
                    plr.hand[i] = plr.drawStack.Last();
                    plr.drawStack.RemoveAt(plr.drawStack.Count - 1);
                }
            }

            private void FormHand()
            {
                DrawTalants(first);
                DrawTalants(second);
                stage = Stage.Waiting;
            }

            private void DropHand(Player plr)
            {

            }

            private void SetLink(object sender, EventArgs e)
            {
                var i = tlntBtns[(sender as InvisibleButton)];
                first.chain[currentLink] = first.hand[i];
            }

            //переместить содержимое сброса в добор
            private void ClearDrop(InBattlePlayer plr)
            {
                foreach (var t in plr.dropStack)
                {
                    plr.drawStack.Add(t);
                }
                plr.dropStack.Clear();
            }

            //закончить текущий ход
            public void FinishTurn(object sender, EventArgs e)
            {
                if (first.chain[currentLink] == EmptyTalant.Instance)
                    return;
                var j = rnd.Next(1, 3);
                second.chain[currentLink] = second.hand[j];
                for (int i = 0; i < 3; i++)
                {
                    first.dropStack.Add(first.hand[i]);
                    second.dropStack.Add(second.hand[i]);
                }
                if (currentLink == 2)
                {
                    stage = Stage.Playing;
                    currentLink = 0;
                }
                else
                {
                    currentLink++;
                    stage = Stage.Choosing;
                }
            }

            public void FinishRound()
            {
                order = !order;
                stage = Stage.Choosing;
            }
            #endregion

            #region fields

            private new Game1 game;
            public SpriteFont font;

            private TouchButton btn;
            private TouchButton exitBtn;
            private Dictionary<InvisibleButton, int> tlntBtns;
            public Stage stage;
            private int currentLink;

            private Random rnd = new Random();
            public InBattlePlayer first;
            public InBattlePlayer second;

            public bool order;

            public Player plr;
            public Player enm;
            #endregion

            public override void Draw(GameTime gameTime)
            {
                game.GraphicsDevice.Clear(Color.Yellow);
                game.spriteBatch.Draw(DataManager.Instance.battleBackground, new Vector2(0, 45), scale: new Vector2(1.5f));
                game.spriteBatch.Draw(DataManager.Instance.battleUpperPanel, new Vector2(-30, 0));
                game.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-50, 950));

                Player1.Instance.sprite.Position = new Vector2(520, 620);
                Player1.Instance.sprite.scale = 0.8f;
                Player1.Instance.sprite.spriteEffects = SpriteEffects.FlipHorizontally;
                Player1.Instance.Draw(game.spriteBatch);
                game.spriteBatch.Draw(DataManager.Instance.healthPanel, new Vector2(520, 520));
                game.spriteBatch.DrawString(DataManager.Instance.menuFont, Player1.Instance.HP.ToString(), new Vector2(580, 540), Color.OrangeRed);
                game.spriteBatch.DrawString(DataManager.Instance.menuFont, Player1.Instance.DEF.ToString(), new Vector2(680, 540), Color.DeepSkyBlue);

                enm.sprite.Position = new Vector2(50, 150);
                enm.sprite.scale = 0.8f;
                enm.sprite.spriteEffects = SpriteEffects.FlipHorizontally;
                enm.Draw(game.spriteBatch);
                game.spriteBatch.Draw(DataManager.Instance.healthPanel, new Vector2(50, 350));
                game.spriteBatch.DrawString(DataManager.Instance.menuFont, enm.HP.ToString(), new Vector2(110, 370), Color.OrangeRed);
                game.spriteBatch.DrawString(DataManager.Instance.menuFont, enm.DEF.ToString(), new Vector2(210, 370), Color.DeepSkyBlue);

                game.spriteBatch.Draw(DataManager.Instance.chainPanel, new Vector2(0, 772), scale: new Vector2(1.1f));
                game.spriteBatch.Draw(DataManager.Instance.chainPanel, new Vector2(290, 172), scale: new Vector2(1.1f));
                for (int i = 0; i < 3; i++)
                {
                    var txtr1 = (first.chain[i] == EmptyTalant.Instance) ? DataManager.Instance.circleIcon : first.chain[i].txtr;
                    var txtr2 = (second.chain[i] == EmptyTalant.Instance) ? DataManager.Instance.circleIcon : second.chain[i].txtr;
                    if (first.plr == Player1.Instance)
                    {
                        game.spriteBatch.Draw(first.hand[i].txtr, new Vector2(10 + i * 160, 1050));
                        game.spriteBatch.Draw(txtr1, new Vector2(30 + i * 137, 800));
                        game.spriteBatch.Draw(txtr2, new Vector2(320 + i * 137, 200));
                    }
                    else
                    {
                        game.spriteBatch.Draw(second.hand[i].txtr, new Vector2(30 + i * 137, 800));
                        game.spriteBatch.Draw(txtr2, new Vector2(320 + i * 137, 200));
                    }
                }
                btn.Draw(game.spriteBatch);
                exitBtn.Draw(game.spriteBatch);

                game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, enm.name + "     VS     " + Player1.Instance.name, new Vector2(40, 30), Color.White);
            }

            public override void Initialize(Game game)
            {
                this.game = game as Game1;
                this.content = new Microsoft.Xna.Framework.Content.ContentManager(game.Content.ServiceProvider, game.Content.RootDirectory);

                #region components
                exitBtn = new TouchButton(new Vector2(680, 20), DataManager.Instance.exitBtnTxtr) { Click = BackToMenu };

                btn = new TouchButton(new Vector2(550, 1030), DataManager.Instance.putLinkButton);
                btn.Click += FinishTurn;

                tlntBtns = new Dictionary<InvisibleButton, int>();
                for (int i = 0; i < 3; i++)
                {
                    var b = new InvisibleButton(new Rectangle(10 + i * 160, 1050, 150, 144));
                    b.Click += SetLink;
                    tlntBtns.Add(b, i);
                }
                #endregion

                stage = Stage.Choosing;
                Player1.Instance.HP = Player1.Instance.MaxHP;
                plr = Player1.Instance;
                enm = Bot.Instance;
                Bot.Instance.Refresh();

                if (rnd.Next(1, 2) == 1)
                {
                    first = new InBattlePlayer(plr);
                    second = new InBattlePlayer(enm);
                }
                else
                {
                    second = new InBattlePlayer(plr);
                    first = new InBattlePlayer(enm);
                }

                for (int i = 0; i < 3; i++)
                {
                    first.chain[i] = EmptyTalant.Instance;
                    second.chain[i] = EmptyTalant.Instance;
                }

                foreach (var t in first.deck)
                {
                    first.drawStack.Add(t);
                }
                foreach (var t in second.deck)
                {
                    second.drawStack.Add(t);
                }
                Shuffle(first.drawStack);
                Shuffle(second.drawStack);
                FormHand();
                if (Player1.Instance.Potion == Shop.ItemType.DEFPotion)
                {
                    Player1.Instance.DEF += 5;
                    Player1.Instance.Potion = Shop.ItemType.Default;
                }
                else if (Player1.Instance.Potion == Shop.ItemType.ATCKPotion)
                {
                    Player1.Instance.DMGBonus = 3;
                    Player1.Instance.Potion = Shop.ItemType.Default;
                }
                else if (Player1.Instance.Potion == Shop.ItemType.ProtPotion)
                {
                    Player1.Instance.DMGBuffer = 3;
                    Player1.Instance.Potion = Shop.ItemType.Default;
                }
                this.game.SwitchState(VersusScreen.Instance);
            }

            private void BackToMenu(object sender, EventArgs e)
            {
                game.SwitchState(MainMenu.Instance);
            }

            public override void Unload()
            {
                //TODO: unload textures
            }

            public override void Update(GameTime gameTime)
            {
                if (first.plr.HP <= 0 || second.plr.HP <= 0)
                {
                    PlayerData.Instance.gamesPlayed++;
                    if (first.plr == Player1.Instance)
                    {
                        if (first.plr.HP <= 0)
                        {
                            PlayerData.Instance.trophies -= 5;
                            game.SwitchState(Loss.Instance);
                            return;
                        }
                    }
                    else if (second.plr.HP <= 0)
                    {
                        PlayerData.Instance.trophies -= 5;
                        game.SwitchState(Loss.Instance);
                        return;
                    }
                    PlayerData.Instance.trophies += 10;
                    game.SwitchState(Victory.Instance);
                }
                btn.Update(gameTime);
                exitBtn.Update(gameTime);
                foreach (var b in tlntBtns.Keys)
                {
                    b.Update(gameTime);
                }
                switch (stage)
                {
                    case (Stage.Waiting):
                        break;
                    case (Stage.Choosing):
                        FormHand();
                        break;
                    case (Stage.Playing):
                        game.SwitchState(new PlayChainState());
                        break;
                    case (Stage.FinishingRound):
                        FinishRound();
                        break;

                }
            }
        }

        class PlayChainState : GameState
        {
            private InBattlePlayer first;
            private InBattlePlayer second;

            private Talant currentTalant;

            private Talant[] order;
            private int current;
            private Texture2D currentlyPlayed;
            private string currentPlayer;
            private string currentDescription;
            private Action<Player, Player> currentAction;

            private float timer;

            public override void Draw(GameTime gameTime)
            {
                var g = game as Game1;
                g.GraphicsDevice.Clear(Color.Green);

                var enm = ChainBattleState.Instance.enm;
                var plr = Player1.Instance;

                g.spriteBatch.Draw(DataManager.Instance.battleBackground, new Vector2(0, 150), scale: new Vector2(1.5f));
                //g.spriteBatch.Draw(DataManager.Instance.shopItemsBackground, new Vector2(-5, 750));
                g.spriteBatch.Draw(DataManager.Instance.menuLowerPanel, new Vector2(-50, 950));
                g.spriteBatch.Draw(DataManager.Instance.portraitPanel, new Vector2(-30, -110));

                enm.portraitSprite.Position = new Vector2(0, 60);
                enm.portraitSprite.scale = 1f;
                enm.portraitSprite.spriteEffects = SpriteEffects.None;
                enm.portraitSprite.Draw(g.spriteBatch);

                g.spriteBatch.Draw(DataManager.Instance.healthPanel1, new Vector2(250, 0), scale: new Vector2(0.5f));
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, enm.HP.ToString(), new Vector2(390, 50), Color.OrangeRed);
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, enm.DEF.ToString(), new Vector2(590, 50), Color.DeepSkyBlue);

                plr.portraitSprite.Position = new Vector2(530, 1000);
                plr.portraitSprite.scale = 0.9f;
                plr.portraitSprite.spriteEffects = SpriteEffects.FlipHorizontally;
                plr.portraitSprite.Draw(g.spriteBatch);

                g.spriteBatch.Draw(DataManager.Instance.healthPanel1, new Vector2(30, 1000), scale: new Vector2(0.5f));
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, plr.HP.ToString(), new Vector2(180, 1050), Color.OrangeRed);
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, plr.DEF.ToString(), new Vector2(370, 1050), Color.DeepSkyBlue);

                var pos1 = new Vector2[] { new Vector2(58, 300), new Vector2(328, 248), new Vector2(588, 300) };
                var pos2 = new Vector2[] { new Vector2(58, 698), new Vector2(328, 748), new Vector2(588, 698) };

                for (int i = 0; i < 3; i++)
                {
                    var txtr = (first.chain[i] == EmptyTalant.Instance) ? DataManager.Instance.circleIcon : first.chain[i].txtr;
                    var txtr1 = (second.chain[i] == EmptyTalant.Instance) ? DataManager.Instance.circleIcon : second.chain[i].txtr;
                    if (first.plr == Player1.Instance)
                    {
                        g.spriteBatch.Draw(txtr, pos2[i]);
                        g.spriteBatch.Draw(txtr1, pos1[i]);
                    }
                    else
                    {
                        g.spriteBatch.Draw(txtr1, pos2[i]);
                        g.spriteBatch.Draw(txtr, pos1[i]);
                    }
                }


                g.spriteBatch.Draw(currentlyPlayed, new Vector2(328, 498));
                g.spriteBatch.Draw(DataManager.Instance.messageBox, new Vector2(478, 500));
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, currentPlayer, new Vector2(485, 505), Color.White);
                g.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, currentDescription, new Vector2(485, 540), Color.White);

            }

            public override void Initialize(Game game)
            {
                this.game = game;

                if (ChainBattleState.Instance.order)
                {
                    first = ChainBattleState.Instance.first;
                    second = ChainBattleState.Instance.second;
                }
                else
                {
                    first = ChainBattleState.Instance.second;
                    second = ChainBattleState.Instance.first;
                }
                currentPlayer = "";
                currentDescription = "";
                MakeOrder();
                currentlyPlayed = DataManager.Instance.circleIcon;
            }

            public override void Unload()
            {

            }

            public override void Update(GameTime gameTime)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (timer > 1 && currentAction == null)
                {
                    if (current == 6 || first.plr.HP <= 0 || second.plr.HP <= 0)
                    {
                        ChainBattleState.Instance.stage = Stage.FinishingRound;
                        (game as Game1).ReturnToState(ChainBattleState.Instance);
                        return;
                    }

                    currentAction = (current % 2 == 0) ? order[current].action : order[current].action;
                    currentlyPlayed = order[current].txtr;
                    currentDescription = order[current].shortDescription;
                    if (current % 2 == 0)
                    {
                        currentPlayer = first.plr.name;
                        first.chain[current / 2] = EmptyTalant.Instance;
                    }
                    else
                    {
                        currentPlayer = second.plr.name;
                        second.chain[current / 2] = EmptyTalant.Instance;
                    }
                }
                else if (timer > 2.5)
                {
                    currentlyPlayed = DataManager.Instance.circleIcon;
                    currentPlayer = "";
                    currentDescription = "";
                    if (current % 2 == 0)
                    {
                        currentAction(first.plr, second.plr);
                    }
                    else
                    {
                        currentAction(second.plr, first.plr);
                    }
                    currentAction = null;
                    current++;
                    timer = 0;
                }

            }

            private void MakeOrder()
            {
                order = new Talant[6];
                for (int i = 0; i < 6; i = i + 2)
                {
                    order[i] = first.chain[i / 2];
                }
                for (int i = 1; i < 6; i = i + 2)
                {
                    order[i] = second.chain[i / 2];
                }
            }
        }

        class VersusScreen : GameState
        {
            private static VersusScreen instance;
            public static VersusScreen Instance
            {
                get
                {
                    if (instance == null)
                        instance = new VersusScreen();
                    return instance;
                }
            }
            private double timer;
            private double animationTimer;
            private new Game1 game;
            private Vector2 enmPosition;
            private Vector2 plrPosition;

            public override void Draw(GameTime gameTime)
            {
                game.GraphicsDevice.Clear(Color.White);
                game.spriteBatch.Draw(DataManager.Instance.versusScreen, new Vector2(-310, 0), scale: new Vector2(0.9f));
                ChainBattleState.Instance.plr.Draw(game.spriteBatch);
                ChainBattleState.Instance.enm.Draw(game.spriteBatch);
                game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, ChainBattleState.Instance.plr.name, new Vector2(plrPosition.X + 20, plrPosition.Y - 30), Color.Black);
                game.spriteBatch.DrawString(DataManager.Instance.battleTitleFont, ChainBattleState.Instance.enm.name, new Vector2(enmPosition.X + 10, enmPosition.Y + 250), Color.Black);
            }

            public override void Initialize(Game game)
            {
                this.game = game as Game1;
                enmPosition = new Vector2(400, 100);
                plrPosition = new Vector2(-20, 750);
                ChainBattleState.Instance.plr.sprite.Position = plrPosition;
                ChainBattleState.Instance.plr.sprite.scale = 1.2f;
                ChainBattleState.Instance.enm.sprite.scale = 1.2f;
                ChainBattleState.Instance.enm.sprite.Position = enmPosition;
            }

            public override void Unload()
            {

            }

            public override void Update(GameTime gameTime)
            {
                timer += gameTime.ElapsedGameTime.TotalMilliseconds;
                animationTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer >= 5000)
                {
                    timer = 0;
                    game.ReturnToState(ChainBattleState.Instance);
                }
                if (animationTimer >= 50)
                {
                    animationTimer = 0;
                    enmPosition.X -= 1;
                    plrPosition.X += 1;
                    ChainBattleState.Instance.plr.sprite.Position = plrPosition;
                    ChainBattleState.Instance.enm.sprite.Position = enmPosition;
                }
            }
        }
    }
}