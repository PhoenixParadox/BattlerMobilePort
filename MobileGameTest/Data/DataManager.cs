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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MobileGameTest.Battle;
using MobileGameTest.PlayerInfo;
using MobileGameTest.Shop;

namespace MobileGameTest.Data
{
    public partial class DataManager
    {
        #region singletone instance
        private static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManager();
                }
                return instance;
            }
        }
        #endregion

        #region fields
        public ContentManager contentManager;

        private ISharedPreferences playerData;
        private ISharedPreferencesEditor playerEdit;
        private ISharedPreferences gameData;
        private ISharedPreferencesEditor gameEdit;
        public string Name;
        #endregion

        public void Initialize(Game game)
        {
            contentManager = new ContentManager(game.Content.ServiceProvider, game.Content.RootDirectory);

            #region save\load managers
            playerData = Application.Context.GetSharedPreferences("Player", FileCreationMode.Private);
            playerEdit = playerData.Edit();
            gameData = Application.Context.GetSharedPreferences("Game", FileCreationMode.Private);
            gameEdit = gameData.Edit();
            #endregion


            //CreatePlayerData();
            //CreateGameData();

            try
            {
                Load();
            }
            catch
            {
                // game is runnig for the first time, or save data was damaged
                CreateGameData();
                CreatePlayerData();
            }
            LoadGameData();
        }

        #region Create New Game Data
        public void CreatePlayerData()
        {
            PlayerData.Instance.Name = "nickname";
            PlayerData.Instance.points =  10000;
            PlayerData.Instance.trophies = 0;
            PlayerData.Instance.maxUnlockedSkin = 0;
            PlayerData.Instance.currentSkin = 0;
            PlayerData.Instance.MaxHP = 25;
            PlayerData.Instance.unlockedSkins = new List<int>() { 1000 };
        }
        public void CreateGameData()
        {
            GameData.Instance.shopItems = new List<int>() { 1, 2, 3 };
            GameData.Instance.shopSkins = new List<int>() { 1001, 1002, 1003, 1004, 1005 };
            GameData.Instance.milestones = new Milestones(1, 2);
            GameData.Instance.currentGoal = 0;
            GameData.Instance.baseAtckDmg = 5;
            GameData.Instance.baseDefAmount = 5;
            GameData.Instance.strongAtckDmg = 10;
            GameData.Instance.swordSwingMultiplicity = 2;

            GameData.Instance.healMultiplicity = 2;
            GameData.Instance.healAmount = 3;

            GameData.Instance.armorBreakDmg = 7;
            GameData.Instance.armorBreakMultiplicity = 4;

            GameData.Instance.armorPenitrationDmg = 4;
            GameData.Instance.armorPenitrationMultiplicity = 3;

            GameData.Instance.dmgBonus = 2;
            GameData.Instance.dmgkBonusMultiplicity = 2;

            CreateAllTalants();
            GameData.Instance.unlockedTalants = new List<TalantType>() { TalantType.BaseAttack, TalantType.BaseDef };
        }
        public void CreateBaseCollection()
        {
            PlayerData.Instance.playerCollection = new List<TalantType>();
            PlayerData.Instance.playerDeck = new List<TalantType>();
            PlayerData.Instance.savedDeck1 = new List<TalantType>();
            PlayerData.Instance.savedDeck2 = new List<TalantType>();
            PlayerData.Instance.savedDeck3 = new List<TalantType>();
            PlayerData.Instance.playerCollection.Add(TalantType.BaseAttack);
            PlayerData.Instance.playerCollection.Add(TalantType.BaseDef);
            for (int i = 0; i < 6; i++)
            {
                PlayerData.Instance.playerDeck.Add(TalantType.BaseAttack);
                PlayerData.Instance.playerDeck.Add(TalantType.BaseDef);
                PlayerData.Instance.savedDeck1.Add(TalantType.BaseDef);
                PlayerData.Instance.savedDeck1.Add(TalantType.BaseAttack);
                PlayerData.Instance.savedDeck2.Add(TalantType.BaseDef);
                PlayerData.Instance.savedDeck2.Add(TalantType.BaseAttack);
                PlayerData.Instance.savedDeck3.Add(TalantType.BaseDef);
                PlayerData.Instance.savedDeck3.Add(TalantType.BaseAttack);
            }
        }

        public void CreateFullCollection()
        {
            CreateBaseCollection();
            PlayerData.Instance.playerCollection.Add(TalantType.ArmorBreak);
            PlayerData.Instance.playerCollection.Add(TalantType.ArmorPenitration);
            PlayerData.Instance.playerCollection.Add(TalantType.AtckBonus);
            PlayerData.Instance.playerCollection.Add(TalantType.Heal);
            PlayerData.Instance.playerCollection.Add(TalantType.SwordSwing);
        }

        public void CreateAllTalants()
        {
            GameData.Instance.allTalants = new Dictionary<TalantType, int[]>();
            foreach (var k in GameData.AllTalants.Keys)
            {
                var val = GameData.AllTalants[k];
                GameData.Instance.allTalants.Add(k, new int[4] { val.Item1, val.Item2, val.Item3, val.Item4 });
            }
        }
        #endregion


        #region Save
        public void Save()
        {
            SaveGame();
            SavePlayer();
        }
        private void SaveGame()
        {
            var str = GameData.Instance.milestones.m1 + ";" + GameData.Instance.milestones.m2;
            gameEdit.PutString("milestones", str);
            str = "";
            for(int i = 0; i < GameData.Instance.shopItems.Count; i++)
            {
                str += GameData.Instance.shopItems[i];
                if (i != GameData.Instance.shopItems.Count - 1)
                {
                    str += ";";
                }
            }
            gameEdit.PutString("shopItems", str);
            str = "";
            for (int i = 0; i < GameData.Instance.shopSkins.Count; i++)
            {
                str += GameData.Instance.shopSkins[i];
                if (i != GameData.Instance.shopSkins.Count - 1)
                {
                    str += ";";
                }
            }
            gameEdit.PutString("shopSkins", str);
            gameEdit.PutInt("currentGoal", GameData.Instance.currentGoal);
            //gameEdit.PutInt("baseAtckDmg", GameData.Instance.baseAtckDmg);
            //gameEdit.PutInt("baseDefAmount", GameData.Instance.baseDefAmount);
            //gameEdit.PutInt("strongAtckDmg", GameData.Instance.strongAtckDmg);
            //gameEdit.PutInt("swordSwingMultiplicity", GameData.Instance.swordSwingMultiplicity);

            //gameEdit.PutInt("healAmount", GameData.Instance.healAmount);
            //gameEdit.PutInt("healMultiplicity", GameData.Instance.healMultiplicity);
            //gameEdit.PutInt("armorBreakMultiplicity", GameData.Instance.armorBreakMultiplicity);
            //gameEdit.PutInt("armorBreakDmg", GameData.Instance.armorBreakDmg);
            //gameEdit.PutInt("armorPenitrationDmg", GameData.Instance.armorPenitrationDmg);
            //gameEdit.PutInt("armorPenitrationMultiplicity", GameData.Instance.armorPenitrationMultiplicity);

            //gameEdit.PutInt("dmgBonus", GameData.Instance.dmgBonus);
            //gameEdit.PutInt("dmgBonusMultiplicity", GameData.Instance.dmgkBonusMultiplicity);

            gameEdit.Commit();
            SaveAllTalants();
        }
        private void SaveAllTalants()
        {
            var dict = GameData.Instance.allTalants;
            foreach (var k in dict.Keys)
            {
                var str = "";
                //str += k.ToString() + ";";
                for (int i = 0; i < 4; i++)
                {
                    str += (i == 3) ? dict[k][i].ToString() : dict[k][i] + ";"; 
                }
                gameEdit.PutString(k.ToString(), str);
            }
            gameEdit.Commit();
        }
        private void SavePlayer()
        {
            PlayerData.Instance.Name = Player1.Instance.name;
            playerEdit.PutString("name", PlayerData.Instance.Name);
            playerEdit.PutInt("points", PlayerData.Instance.points);
            playerEdit.PutInt("trophies", PlayerData.Instance.trophies);
            playerEdit.PutInt("maxUnlockedSkin", PlayerData.Instance.maxUnlockedSkin);
            playerEdit.PutInt("currentSkin", PlayerData.Instance.currentSkin);
            playerEdit.PutInt("MaxHP", PlayerData.Instance.MaxHP);
            var str = "";
            for (int i = 0; i < PlayerData.Instance.playerCollection.Count; i++)
            {
                str += ((int)PlayerData.Instance.playerCollection[i]).ToString();
                if (i != PlayerData.Instance.playerCollection.Count - 1)
                {
                    str += ";";
                }
            }
            playerEdit.PutString("collection", str);
            str = "";
            for (int i = 0; i < 12; i++)
            {
                str += ((int)PlayerData.Instance.playerDeck[i]).ToString();
                if (i != 11)
                {
                    str += ";";
                }
            }
            playerEdit.PutString("deck", str);
            str = "";
            for (int i = 0; i < 12; i++)
            {
                str += ((int)PlayerData.Instance.savedDeck1[i]).ToString();
                if (i != 11)
                {
                    str += ";";
                }
            }
            playerEdit.PutString("deck1", str);
            str = "";
            for (int i = 0; i < 12; i++)
            {
                str += ((int)PlayerData.Instance.savedDeck2[i]).ToString();
                if (i != 11)
                {
                    str += ";";
                }
            }
            playerEdit.PutString("deck2", str);
            str = "";
            for (int i = 0; i < 12; i++)
            {
                str += ((int)PlayerData.Instance.savedDeck3[i]).ToString();
                if (i != 11)
                {
                    str += ";";
                }
            }
            playerEdit.PutString("deck3", str);
            str = "";
            for (int i = 0; i < PlayerData.Instance.unlockedSkins.Count; i++)
            {
                var temp = PlayerData.Instance.unlockedSkins[i].ToString();
                str += (i == PlayerData.Instance.unlockedSkins.Count - 1) ? temp : temp + ";";  
            }
            playerEdit.PutString("unlockedSkins", str);
            playerEdit.Commit();
        }
        #endregion

        #region Load

        public void Load()
        {
            LoadGame();
            LoadPlayer();
            //PlayerData.Instance.points += 10000;
        }
        private void LoadGame()
        {
            var temp = gameData.GetString("milestones", "1;2").Split(';');
            GameData.Instance.milestones = new Milestones(Int32.Parse(temp[0]), Int32.Parse(temp[1]));
            temp = gameData.GetString("shopItems", "1;2;3").Split(';');
            GameData.Instance.shopItems = temp.Select(i => Int32.Parse(i)).ToList();
            temp = gameData.GetString("shopSkins", "1001;1002;1003;1004;1005").Split(';');
            GameData.Instance.shopSkins = temp.Where(i => i != "").Select(i => Int32.Parse(i)).ToList();
            //LoadShop();
            GameData.Instance.currentGoal = gameData.GetInt("currentGoal", 0);
            //GameData.Instance.baseAtckDmg = gameData.GetInt("baseAtckDmg", 5);
            //GameData.Instance.baseDefAmount = gameData.GetInt("baseDefAmount", 5);
            //GameData.Instance.strongAtckDmg = gameData.GetInt("strongAtckDmg", 10);
            //GameData.Instance.swordSwingMultiplicity = gameData.GetInt("swordSwingMultiplicity", 2);

            //GameData.Instance.healMultiplicity = gameData.GetInt("healMultiplicity", 2);
            //GameData.Instance.healAmount = gameData.GetInt("healAmount", 3);
            //GameData.Instance.armorBreakDmg = gameData.GetInt("armorBreakDmg", 5);
            //GameData.Instance.armorBreakMultiplicity = gameData.GetInt("armorBreakMultiplicity", 4);
            //GameData.Instance.armorPenitrationDmg = gameData.GetInt("armorPenitrationDmg", 4);
            //GameData.Instance.armorPenitrationMultiplicity = gameData.GetInt("armorPenitrationMultiplicity", 3);

            //GameData.Instance.dmgBonus = gameData.GetInt("dmgBonus", 2);
            //GameData.Instance.dmgkBonusMultiplicity = gameData.GetInt("dmgBonusMultiplicity", 2);
            LoadAllTalants();
        }
        private void LoadAllTalants()
        {
            var types = GameData.Instance.nameToType;
            var allTalantsBase = GameData.AllTalants;
            GameData.Instance.allTalants = new Dictionary<TalantType, int[]>();
            foreach (var k in types.Keys)
            {
                var res = gameData.GetString(k, "");
                if (res == "")
                {
                    GameData.Instance.allTalants[types[k]] = new int[4] { allTalantsBase[types[k]].Item1, allTalantsBase[types[k]].Item2, allTalantsBase[types[k]].Item3, allTalantsBase[types[k]].Item4 };
                }
                else
                {
                    var vals = res.Split(';').Select(v => Int32.Parse(v)).ToArray();
                    GameData.Instance.allTalants[types[k]] = vals;
                }
            }
        }
        private void LoadPlayer()
        {
            PlayerData.Instance.Name = playerData.GetString("name", "nickname");
            PlayerData.Instance.points = playerData.GetInt("points", 0);
            PlayerData.Instance.trophies = playerData.GetInt("trophies", 0);
            PlayerData.Instance.maxUnlockedSkin= playerData.GetInt("maxUnlockedSkin", 0);
            PlayerData.Instance.currentSkin = playerData.GetInt("currentSkin", 0);
            PlayerData.Instance.MaxHP = playerData.GetInt("MaxHP", 25);
            var temp = playerData.GetString("collection", "0;1").Split(';');
            PlayerData.Instance.playerCollection = new List<TalantType>();
            foreach (var c in temp)
            {
                PlayerData.Instance.playerCollection.Add((TalantType)(Int32.Parse(c)));
            }
            temp = playerData.GetString("deck", "0;1;0;1;0;1;0;1;0;1;0;1").Split(';');
            PlayerData.Instance.playerDeck = new List<TalantType>();
            foreach (var c in temp)
            {
                PlayerData.Instance.playerDeck.Add((TalantType)(Int32.Parse(c)));
            }

            temp = playerData.GetString("deck1", "0;1;0;1;0;1;0;1;0;1;0;1").Split(';');
            PlayerData.Instance.savedDeck1 = new List<TalantType>();
            foreach (var c in temp)
            {
                PlayerData.Instance.savedDeck1.Add((TalantType)(Int32.Parse(c)));
            }
            temp = playerData.GetString("deck2", "0;1;0;1;0;1;0;1;0;1;0;1").Split(';');
            PlayerData.Instance.savedDeck2 = new List<TalantType>();
            foreach (var c in temp)
            {
                PlayerData.Instance.savedDeck2.Add((TalantType)(Int32.Parse(c)));
            }
            temp = playerData.GetString("deck3", "0;1;0;1;0;1;0;1;0;1;0;1").Split(';');
            PlayerData.Instance.savedDeck3 = new List<TalantType>();
            foreach (var c in temp)
            {
                PlayerData.Instance.savedDeck3.Add((TalantType)(Int32.Parse(c)));
            }
            temp = playerData.GetString("unlockedSkins", "1000").Split(';');
            PlayerData.Instance.unlockedSkins = new List<int>();
            foreach (var i in temp)
            {
                PlayerData.Instance.unlockedSkins.Add(Int32.Parse(i));
            }
        }

        public void LoadGameData()
        {
            Skins = new Texture2D[5];
            Portraits = new Texture2D[5];
            LoadAssets();

            EmptyTalant.Instance.txtr = circleIcon;
            if (PlayerData.Instance.playerCollection == null)
            {
                CreateBaseCollection();
                //CreateFullCollection();
            }
            Player1.Instance.Collection = RestoreCollection(PlayerData.Instance.playerCollection);
            Player1.Instance.Deck = RestoreDeck(PlayerData.Instance.playerDeck);
            Player1.Instance.Deck1 = RestoreDeck(PlayerData.Instance.savedDeck1);
            Player1.Instance.Deck2 = RestoreDeck(PlayerData.Instance.savedDeck2);
            Player1.Instance.Deck3 = RestoreDeck(PlayerData.Instance.savedDeck3);
            Player1.Instance.Initialize();
            LoadShop();
        }
        public void LoadShop()
        {
            ItemShop.Instance.items = new List<Item>();
            ItemShop.Instance.skins = new List<Item>();
            foreach (var i in GameData.Instance.shopItems)
            {
               ItemShop.Instance.items.Add(ItemBase.dict[i]);
            }
            foreach (var i in GameData.Instance.shopSkins)
            {
                ItemShop.Instance.skins.Add(ItemBase.dict[i]);
            }
        }

        public List<Talant> RestoreCollection(List<TalantType> col)
        {
            var temp = new List<Talant>();
            foreach (var t in col.Distinct())
                temp.Add(TalantFactory.GetTalant(t));
            return temp;
        }
        public List<Talant> RestoreDeck(List<TalantType> col)
        {
            var temp = new List<Talant>();
            foreach (var t in col)
                temp.Add(TalantFactory.GetTalant(t));
            return temp;
        }
        #endregion

        #region fonts
        public SpriteFont menuFont;
        public SpriteFont font1;
        public SpriteFont localizedMenuFont;
        public SpriteFont textFont;
        public SpriteFont arialBig;
        public SpriteFont testFont;
        public SpriteFont battleTitleFont;
        public SpriteFont playerMenuFont;
        public SpriteFont milestonesFont;
        public SpriteFont goalDescriptionFont;
        #endregion

        #region menu components
        public Texture2D btnTxtr;
        public Texture2D exitBtnTxtr;
        public Texture2D progressBarTxtr;
        public Texture2D goalTxtr;
        public Texture2D tasksButtonTxtr;
        public Texture2D battleButtonTxtr;
        public Texture2D campBackgroundTxtr;
        public Texture2D skillFrame;
        public Texture2D woodenBtn;
        public Texture2D menuBackground;
        public Texture2D menuLowerPanel;

        public Texture2D victoryScreen;
        public Texture2D continueButton;
        public Texture2D lossScreen;
        #endregion

        #region progress bar
        public Texture2D[] ProgressBar;
        public Texture2D milestoneIcon;
        public Texture2D rightArrow;
        public Texture2D leftArrow;
        #endregion

        #region talant icons
        public Texture2D emptyIcon;
        public Texture2D baseAtck;
        public Texture2D baseDef;
        public Texture2D strongAtck;

        public Texture2D armorPenitrationAtck;
        public Texture2D armorBreakAtck;
        public Texture2D heal;
        public Texture2D dmgBonus;
        #endregion

        #region player
        public Texture2D[] Skins;
        public Texture2D[] Portraits;
        private Texture2D plrTxtr;
        public Texture2D plrPortraitTxtr;
        public Texture2D plrTxtr1;
        public Texture2D plrPortraitTxtr1;

        public Dictionary<int, Tuple<Texture2D, Texture2D>> SkinsAndPortraits;
        #endregion

        #region player info
        public Texture2D lowerPanelBorder;
        public Texture2D progressButton;
        public Texture2D collectionButton;
        public Texture2D deckButton;
        public Texture2D downButton;
        public Texture2D upButton;
        public Texture2D goalPanel;
        public Texture2D goalButton;
        public Texture2D playerStats;
        public Texture2D deck1Btn;
        public Texture2D deck2Btn;
        public Texture2D deck3Btn;

        public Texture2D inputButton;
        #endregion

        #region icons
        public Texture2D coinIcon;
        public Texture2D shopIcon;
        public Texture2D shopItemIcon;
        public Texture2D trophieIcon;
        #endregion

        #region shop
        public Texture2D shopBackground;
        public Texture2D shopItemsBackground;

        public Texture2D shopItemsButton;
        public Texture2D shopSkinsButton;
        #endregion

        #region loading screen
        public Texture2D[] loadingAnimation;
        public Texture2D loadingMessage;
        public Texture2D versusScreen;
        #endregion

        #region battle components
        public Texture2D battleBackground;
        public Texture2D battleUpperPanel;
        public Texture2D heartIcon;
        public Texture2D shieldIcon;

        public Texture2D botTxtr;
        public Texture2D botPortraitTxtr;

        public Texture2D circleIcon;
        public Texture2D portraitPanel;
        public Texture2D arenaBackground;
        public Texture2D putLinkButton;
        public Texture2D chainPanel;
        public Texture2D healthPanel;
        public Texture2D messageBox;
        public Texture2D healthPanel1;
        #endregion

        #region education
        public Texture2D educationBackground;
        public Texture2D scroll;

        public Texture2D[] theoryTasks;
        public Texture2D[] mathTasks;
        public Texture2D[] calcTasks;

        public List<Texture2D> levels;
        public List<Texture2D> levelTopics;
        public List<Texture2D> theoryText;
        public Texture2D theoryTasksFrame;
        public Texture2D mathTasksFrame;
        public Texture2D calcTasksFrame;

        public Texture2D correctIcon;
        public Texture2D redoIcon;
        public Texture2D infoIcon;

        #region solve task Screen
        public Texture2D inputAnswerBtn;
        public Texture2D checkAnswerBtn;
        public Texture2D redoTaskBtn;
        public Texture2D textBox;

        public Texture2D nextTaskBtn;
        public Texture2D prevTaskBtn;
        public Texture2D taskNameFrame;
        public Texture2D incorrectIcon;
        #endregion

        #region tutorial
        public List<Texture2D> tutorialPages;
        #endregion
        #endregion

        private void LoadAssets()
        {            
            #region fonts
            menuFont = contentManager.Load<SpriteFont>("font1");
            font1 = contentManager.Load<SpriteFont>("File");
            localizedMenuFont = contentManager.Load<SpriteFont>("localizedMenuFont");
            //textFont = contentManager.Load<SpriteFont>("textFont");
            arialBig = contentManager.Load<SpriteFont>("arialBig");
            battleTitleFont = contentManager.Load<SpriteFont>("fonts/battleTitleFont");
            playerMenuFont = contentManager.Load<SpriteFont>("fonts/playerMenuLocalizedFont");
            milestonesFont = contentManager.Load<SpriteFont>("fonts/milestonesFont");
            goalDescriptionFont = contentManager.Load<SpriteFont>("fonts/goalDescriptionFont");
            #endregion

            #region menu components
            btnTxtr = contentManager.Load<Texture2D>("buttons/next_move_btn_txtr");
            progressBarTxtr = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar");
            exitBtnTxtr = contentManager.Load<Texture2D>("buttons/closeButton");
            goalTxtr = contentManager.Load<Texture2D>("PlayerInfo/Goal");
            tasksButtonTxtr = contentManager.Load<Texture2D>("menu/goToTasksButtonBig");
            battleButtonTxtr = contentManager.Load<Texture2D>("menu/goToBattleButtonBig");
            campBackgroundTxtr = contentManager.Load<Texture2D>("PlayerInfo/campBackground");
            skillFrame = contentManager.Load<Texture2D>("PlayerInfo/skillFrame");
            woodenBtn = contentManager.Load<Texture2D>("buttons/woodenBtn230x100");
            menuBackground = contentManager.Load<Texture2D>("common/menuBackground");
            menuLowerPanel = contentManager.Load<Texture2D>("menu/woodPanelBig");

            victoryScreen = contentManager.Load<Texture2D>("menu/victoryScreenWithScore");
            continueButton = contentManager.Load<Texture2D>("menu/continueButton");
            lossScreen = contentManager.Load<Texture2D>("menu/lossScreenWithScore");
            #endregion

            #region progress bar
            ProgressBar = new Texture2D[6];
            ProgressBar[0] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar0");
            ProgressBar[1] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar1");
            ProgressBar[2] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar2");
            ProgressBar[3] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar3");
            ProgressBar[4] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar4");
            ProgressBar[5] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar5");
            milestoneIcon = contentManager.Load<Texture2D>("icons/milestoneIcon128x128");

            rightArrow = contentManager.Load<Texture2D>("buttons/rightArrow64");
            leftArrow = contentManager.Load<Texture2D>("buttons/leftArrow64");
            #endregion

            #region talant icon
            //emptyIcon = contentManager.Load<Texture2D>("empty");
            baseAtck = contentManager.Load<Texture2D>("TalantIcons/simpleAtttackIconBigSaturated1");
            baseDef = contentManager.Load<Texture2D>("TalantIcons/simpleDeffenceIconBigSaturated1");
            strongAtck = contentManager.Load<Texture2D>("TalantIcons/swordSwingIconBigSaturated1");

            armorPenitrationAtck = contentManager.Load<Texture2D>("TalantIcons/armorPenitrationSkill");
            heal = contentManager.Load<Texture2D>("TalantIcons/healIcon");
            dmgBonus = contentManager.Load<Texture2D>("TalantIcons/dmgBonusIcon");
            armorBreakAtck = contentManager.Load<Texture2D>("TalantIcons/armorBreakSkill");
            #endregion
            #region player
            SkinsAndPortraits = new Dictionary<int, Tuple<Texture2D, Texture2D>>()
            {
                { 1000, Tuple.Create(contentManager.Load<Texture2D>("Player/Cat1"), contentManager.Load<Texture2D>("Player/catPortrait")) },
                { 1001, Tuple.Create(contentManager.Load<Texture2D>("Player/FishKing"), contentManager.Load<Texture2D>("Player/FishKingPortrait")) },
                { 1002, Tuple.Create(contentManager.Load<Texture2D>("Player/blueFluffyCreature"), contentManager.Load<Texture2D>("Player/blueFluffyCreaturePortrait"))},
                { 1003, Tuple.Create(contentManager.Load<Texture2D>("Player/drogo"), contentManager.Load<Texture2D>("Player/drogoPortrait"))},
                { 1004, Tuple.Create(contentManager.Load<Texture2D>("Player/dinasaur"), contentManager.Load<Texture2D>("Player/dinoPortrait"))},
                { 1005, Tuple.Create(contentManager.Load<Texture2D>("battle/battleBirdBig"), contentManager.Load<Texture2D>("battle/battleBirdPortrait"))}
            };
            

            Skins[0] = contentManager.Load<Texture2D>("Player/Cat1");
            Portraits[0] = contentManager.Load<Texture2D>("Player/catPortrait");
            Skins[1] = contentManager.Load<Texture2D>("Player/FishKing");
            Portraits[1] = contentManager.Load<Texture2D>("Player/FishKingPortrait");

            Skins[2] = contentManager.Load<Texture2D>("Player/blueFluffyCreature");
            Portraits[2] = contentManager.Load<Texture2D>("Player/blueFluffyCreaturePortrait");

            Skins[3] = contentManager.Load<Texture2D>("Player/drogo");
            Portraits[3] = contentManager.Load<Texture2D>("Player/drogoPortrait");

            Skins[4] = contentManager.Load<Texture2D>("Player/dinasaur");
            Portraits[4] = contentManager.Load<Texture2D>("Player/dinoPortrait");

            plrTxtr = contentManager.Load<Texture2D>("Player/Cat1");
            plrPortraitTxtr = contentManager.Load<Texture2D>("Player/catPortrait");
            plrTxtr1 = contentManager.Load<Texture2D>("Player/FishKing");
            plrPortraitTxtr1 = contentManager.Load<Texture2D>("Player/FishKingPortrait");
            #endregion
            #region player info
            lowerPanelBorder = contentManager.Load<Texture2D>("PlayerInfo/longWoodenPlank");
            progressButton = contentManager.Load<Texture2D>("buttons/progressButton");
            collectionButton = contentManager.Load<Texture2D>("buttons/collectionButton");
            deckButton = contentManager.Load<Texture2D>("buttons/deckButton");
            downButton = contentManager.Load<Texture2D>("buttons/downButton");
            upButton = contentManager.Load<Texture2D>("buttons/upButton");
            goalPanel = contentManager.Load<Texture2D>("PlayerInfo/goalPanel");
            goalButton = contentManager.Load<Texture2D>("buttons/upgradeButton");
            playerStats = contentManager.Load<Texture2D>("PlayerInfo/playerStats");
            deck1Btn = contentManager.Load<Texture2D>("buttons/deck1");
            deck2Btn = contentManager.Load<Texture2D>("buttons/deck2");
            deck3Btn = contentManager.Load<Texture2D>("buttons/deck3");

            inputButton = contentManager.Load<Texture2D>("buttons/inputButton");
            #endregion

            #region icons
            coinIcon = contentManager.Load<Texture2D>("icons/coin");
            shopIcon = contentManager.Load<Texture2D>("buttons/shop");
            shopItemIcon = contentManager.Load<Texture2D>("icons/shopItemIcon");
            trophieIcon = contentManager.Load<Texture2D>("icons/trophieIcon");
            #endregion
            #region shop
            shopBackground = contentManager.Load<Texture2D>("shop/shopBackgroundNew");
            shopItemsBackground = contentManager.Load<Texture2D>("common/woodenPanel");

            shopItemsButton = contentManager.Load<Texture2D>("buttons/shopItemsButton");
            shopSkinsButton = contentManager.Load<Texture2D>("buttons/shopSkinsButton");
            #endregion

            #region battle components
            battleBackground = contentManager.Load<Texture2D>("battle/battleback1");
            battleUpperPanel = contentManager.Load<Texture2D>("battle/battleUpperPanelNew");
            heartIcon = contentManager.Load<Texture2D>("battle/heartIcon64x64");
            shieldIcon = contentManager.Load<Texture2D>("battle/shieldIcon64x64");
            botTxtr = contentManager.Load<Texture2D>("battle/battleBirdBig");
            botPortraitTxtr = contentManager.Load<Texture2D>("battle/battleBirdPortrait");
            circleIcon = contentManager.Load<Texture2D>("battle/chainLink");
            portraitPanel = contentManager.Load<Texture2D>("battle/battlePortraitPanelNew");
            arenaBackground = contentManager.Load<Texture2D>("battle/arenaBackground775x775");
            putLinkButton = contentManager.Load<Texture2D>("battle/putLinkButton");
            chainPanel = contentManager.Load<Texture2D>("battle/longNarrowWoodenPanel");
            healthPanel = contentManager.Load<Texture2D>("battle/healthPanel");
            messageBox = contentManager.Load<Texture2D>("battle/shortNarrowWoodenPanel");
            healthPanel1 = contentManager.Load<Texture2D>("battle/healthPanel1");
            #endregion

            #region loading screen
            loadingAnimation = new Texture2D[4];
            loadingAnimation[0] = contentManager.Load<Texture2D>("loadingScreen/loadingSymbol1");
            loadingAnimation[1] = contentManager.Load<Texture2D>("loadingScreen/loadingSymbol2");
            loadingAnimation[2] = contentManager.Load<Texture2D>("loadingScreen/loadingSymbol3");
            loadingAnimation[3] = contentManager.Load<Texture2D>("loadingScreen/loadingSymbol4");
            loadingMessage = contentManager.Load<Texture2D>("loadingScreen/loadingMessageBig");
            versusScreen = contentManager.Load<Texture2D>("loadingScreen/VersusScreenBig");
            #endregion

            #region education
            educationBackground = contentManager.Load<Texture2D>("education/Fantasy-library-art");
            scroll = contentManager.Load<Texture2D>("education/scrollShiny");

            theoryTasks = new Texture2D[5];
            theoryTasks[0] = contentManager.Load<Texture2D>("education/theoryTask1");
            theoryTasks[1] = contentManager.Load<Texture2D>("education/theoryTask2");
            theoryTasks[2] = contentManager.Load<Texture2D>("education/theoryTask3");
            theoryTasks[3] = contentManager.Load<Texture2D>("education/theoryTask4");
            theoryTasks[4] = contentManager.Load<Texture2D>("education/theoryTask5");

            mathTasks = new Texture2D[5];
            mathTasks[0] = contentManager.Load<Texture2D>("education/mathTask1");
            mathTasks[1] = contentManager.Load<Texture2D>("education/mathTask2");

            calcTasks = new Texture2D[5];
            calcTasks[0] = contentManager.Load<Texture2D>("education/calcTask1");
            calcTasks[1] = contentManager.Load<Texture2D>("education/calcTask2");
            calcTasks[2] = contentManager.Load<Texture2D>("education/calcTask3");

            levels = new List<Texture2D>();
            levels.Add(contentManager.Load<Texture2D>("education/level1"));

            levelTopics = new List<Texture2D>();
            levelTopics.Add(contentManager.Load<Texture2D>("education/level1Topic"));

            theoryText = new List<Texture2D>();
            theoryText.Add(contentManager.Load<Texture2D>("education/theory1"));

            theoryTasksFrame = contentManager.Load<Texture2D>("education/theoryTasksFrame");
            mathTasksFrame = contentManager.Load<Texture2D>("education/mathTasksFrame");
            calcTasksFrame = contentManager.Load<Texture2D>("education/calcTasksFrame");

            correctIcon = contentManager.Load<Texture2D>("education/correctIcon");
            redoIcon = contentManager.Load<Texture2D>("education/generatedicon");
            infoIcon = contentManager.Load<Texture2D>("education/infoButton");

            #region solve task screen
            checkAnswerBtn = contentManager.Load<Texture2D>("education/solveTaskScreen/checkAnswerBtn");
            inputAnswerBtn = contentManager.Load<Texture2D>("education/solveTaskScreen/inputAnswerBtn");
            redoTaskBtn = contentManager.Load<Texture2D>("education/solveTaskScreen/redoButton");
            textBox = contentManager.Load<Texture2D>("education/solveTaskScreen/textBox1");

            nextTaskBtn = contentManager.Load<Texture2D>("education/solveTaskScreen/rightArrow");
            prevTaskBtn = contentManager.Load<Texture2D>("education/solveTaskScreen/leftArrow");
            taskNameFrame = contentManager.Load<Texture2D>("education/solveTaskScreen/taskNameFrame");
            incorrectIcon = contentManager.Load<Texture2D>("education/solveTaskScreen/incorrectIcon");
            #endregion

            #region tutorial
            tutorialPages = new List<Texture2D>();
            tutorialPages.Add(contentManager.Load<Texture2D>("education/tutorial/tutorial1"));
            tutorialPages.Add(contentManager.Load<Texture2D>("education/tutorial/tutorial2"));
            #endregion

            #endregion

        }
    }
}