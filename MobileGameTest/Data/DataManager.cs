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


            //CreateGameData();
            //CreatePlayerData();

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
            PlayerData.Instance.points =  0;
            PlayerData.Instance.trophies = 0;
            PlayerData.Instance.maxUnlockedSkin = 0;
            PlayerData.Instance.currentSkin = 0;
            PlayerData.Instance.MaxHP = 25;
        }
        public void CreateGameData()
        {
            GameData.Instance.shopItems = new List<int>() { 1, 2, 3, 20 };
            GameData.Instance.milestones = new Milestones(1, 2);
            GameData.Instance.currentGoal = 0;
            GameData.Instance.baseAtckDmg = 5;
            GameData.Instance.baseDefAmount = 5;
            GameData.Instance.strongAtckDmg = 10;
            GameData.Instance.swordSwingMultiplicity = 2;

            GameData.Instance.healMultiplicity = 2;
            GameData.Instance.healAmount = 3;
            GameData.Instance.armorBreakDmg = 5;
            GameData.Instance.armorBreakMultiplicity = 4;
            GameData.Instance.armorPenitrationDmg = 4;
            GameData.Instance.armorPenitrationMultiplicity = 3;
        }
        public void CreateBaseCollection()
        {
            PlayerData.Instance.playerCollection = new List<TalantType>();
            PlayerData.Instance.playerDeck = new List<TalantType>();
            PlayerData.Instance.playerCollection.Add(TalantType.BaseAttack);
            PlayerData.Instance.playerCollection.Add(TalantType.BaseDef);
            for (int i = 0; i < 6; i++)
            {
                PlayerData.Instance.playerDeck.Add(TalantType.BaseAttack);
                PlayerData.Instance.playerDeck.Add(TalantType.BaseDef);
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
            gameEdit.PutInt("currentGoal", GameData.Instance.currentGoal);
            gameEdit.PutInt("baseAtckDmg", GameData.Instance.baseAtckDmg);
            gameEdit.PutInt("baseDefAmount", GameData.Instance.baseDefAmount);
            gameEdit.PutInt("strongAtckDmg", GameData.Instance.strongAtckDmg);
            gameEdit.PutInt("swordSwingMultiplicity", GameData.Instance.swordSwingMultiplicity);

            gameEdit.PutInt("healAmount", GameData.Instance.healAmount);
            gameEdit.PutInt("healMultiplicity", GameData.Instance.healMultiplicity);
            gameEdit.PutInt("armorBreakMultiplicity", GameData.Instance.armorBreakMultiplicity);
            gameEdit.PutInt("armorBreakDmg", GameData.Instance.armorBreakDmg);
            gameEdit.PutInt("armorPenitrationDmg", GameData.Instance.armorPenitrationDmg);
            gameEdit.PutInt("armorPenitrationMultiplicity", GameData.Instance.armorPenitrationMultiplicity);
            gameEdit.Commit();
        }
        private void SavePlayer()
        {
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
            playerEdit.Commit();
        }
        #endregion

        #region Load

        public void Load()
        {
            LoadGame();
            LoadPlayer();
        }
        private void LoadGame()
        {
            var temp = gameData.GetString("milestones", "1;2").Split(';');
            GameData.Instance.milestones = new Milestones(Int32.Parse(temp[0]), Int32.Parse(temp[1]));
            temp = gameData.GetString("shopItems", "1;2;3;20").Split(';');
            GameData.Instance.shopItems = temp.Select(i => Int32.Parse(i)).ToList();
            //LoadShop();
            GameData.Instance.currentGoal = gameData.GetInt("currentGoal", 0);
            GameData.Instance.baseAtckDmg = gameData.GetInt("baseAtckDmg", 5);
            GameData.Instance.baseDefAmount = gameData.GetInt("baseDefAmount", 5);
            GameData.Instance.strongAtckDmg = gameData.GetInt("strongAtckDmg", 10);
            GameData.Instance.swordSwingMultiplicity = gameData.GetInt("swordSwingMultiplicity", 2);

            GameData.Instance.healMultiplicity = gameData.GetInt("healMultiplicity", 2);
            GameData.Instance.healAmount = gameData.GetInt("healAmount", 3);
            GameData.Instance.armorBreakDmg = gameData.GetInt("armorBreakDmg", 5);
            GameData.Instance.armorBreakMultiplicity = gameData.GetInt("armorBreakMultiplicity", 4);
            GameData.Instance.armorPenitrationDmg = gameData.GetInt("armorPenitrationDmg", 4);
            GameData.Instance.armorPenitrationMultiplicity = gameData.GetInt("armorPenitrationMultiplicity", 3);
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
            }
            Player1.Instance.Collection = RestoreCollection(PlayerData.Instance.playerCollection);
            Player1.Instance.Deck = RestoreDeck(PlayerData.Instance.playerDeck);
            Player1.Instance.Initialize();
            LoadShop();
        }
        public void LoadShop()
        {
            ItemShop.Instance.items = new List<Item>();
            foreach (var i in GameData.Instance.shopItems)
            {
                if (PlayerData.Instance.maxUnlockedSkin == 1 && i == 20)
                    continue;
                ItemShop.Instance.items.Add(ItemBase.dict[i]);
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

        private void LoadAssets()
        {            
            #region fonts
            menuFont = contentManager.Load<SpriteFont>("font1");
            font1 = contentManager.Load<SpriteFont>("File");
            localizedMenuFont = contentManager.Load<SpriteFont>("localizedMenuFont");
            //textFont = contentManager.Load<SpriteFont>("textFont");
            arialBig = contentManager.Load<SpriteFont>("arialBig");
            battleTitleFont = contentManager.Load<SpriteFont>("fonts/battleTitleFont");
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
            woodenBtn = contentManager.Load<Texture2D>("buttons/woodenBtn192x84");
            menuBackground = contentManager.Load<Texture2D>("common/menuBackground");
            menuLowerPanel = contentManager.Load<Texture2D>("menu/woodPanelBig");

            victoryScreen = contentManager.Load<Texture2D>("menu/victoryScreenWithScore");
            continueButton = contentManager.Load<Texture2D>("menu/continueButton");
            lossScreen = contentManager.Load<Texture2D>("menu/lossScreenWithScore");
            #endregion

            #region progress bar
            ProgressBar = new Texture2D[6];
            ProgressBar[0] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBar");
            ProgressBar[1] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBarFirstMilestone");
            ProgressBar[2] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBarSecondMilestone");
            ProgressBar[3] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBarThirdMilestone");
            ProgressBar[4] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBarFourthMilestone");
            ProgressBar[5] = contentManager.Load<Texture2D>("PlayerInfo/ProgressBarFithMilestone");
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

            #region icons
            coinIcon = contentManager.Load<Texture2D>("icons/coin");
            shopIcon = contentManager.Load<Texture2D>("buttons/shop");
            shopItemIcon = contentManager.Load<Texture2D>("icons/shopItemIcon");
            trophieIcon = contentManager.Load<Texture2D>("icons/trophieIcon");
            #endregion
            #region shop
            shopBackground = contentManager.Load<Texture2D>("shop/shopBackgroundNew");
            shopItemsBackground = contentManager.Load<Texture2D>("common/woodenPanel");
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
            
        }
    }
}