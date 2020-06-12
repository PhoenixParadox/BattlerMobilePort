using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using MobileGameTest.Education;
using MobileGameTest.PlayerInfo;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileGameTest
{
    [Activity(Label = "MobileGameTest"
        , MainLauncher = true
        , Icon = "@drawable/icon"
        , Theme = "@style/Theme.Splash"
        , AlwaysRetainTaskState = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleInstance
        , ScreenOrientation = ScreenOrientation.FullUser
        , ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout)]
 public class Activity1 : Microsoft.Xna.Framework.AndroidGameActivity
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ToUnicode(
        uint virtualKeyCode,
        uint scanCode,
        byte[] keyboardState,
        StringBuilder receivingBuffer,
        int bufferSize,
        uint flags
        );


        private Game1 game;
        private StringBuilder charPressed;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            game = new Game1();
            game.gameActivity = this;
            SetContentView((View)game.Services.GetService(typeof(View)));

            var v = (View)game.Services.GetService(typeof(View));
            v.KeyPress += OnKeyPress;

            charPressed = new StringBuilder(256);


            game.Run();
        }

        public void OnKeyPress(object sender, View.KeyEventArgs e)
        {
            if (game._inputTimer < 300)
                return;
            game._inputTimer = 0;
            if (game.currentState == PlayerInfoScreen.Instance)
            {
                if (e.KeyCode == Keycode.Del)
                {
                    if (Player1.Instance.name.Length > 0)
                    {
                        Player1.Instance.name = Player1.Instance.name.Substring(0, Player1.Instance.name.Length - 1);
                    }
                }
                else
                {
                    if (e.KeyCode == Keycode.Enter)
                    {
                        PlayerInfoScreen.Instance.HideKeyBoard();
                        return;
                    }
                    if ((int)e.KeyCode >= 7 && (int)e.KeyCode <= 16)
                    {
                        Player1.Instance.name += (int)e.KeyCode - 7;
                        return;
                    }
                    var str = KeyEvent.KeyCodeToString(e.KeyCode);
                    Player1.Instance.name += str.Substring(str.Length - 1).ToLower();
                }
            }
            else if (game.currentState == CurrentTaskScreen.Instance)
            {
                var taskScreen = CurrentTaskScreen.Instance;
                if (e.KeyCode == Keycode.Del)
                {
                    if (taskScreen.playerAnswer.Length > 0)
                    {
                        taskScreen.playerAnswer = taskScreen.playerAnswer.Substring(0, taskScreen.playerAnswer.Length - 1);
                    }
                }
                else
                {
                    if (e.KeyCode == Keycode.Enter)
                    {
                        taskScreen.CheckAnswer();
                        return;
                    }
                    if (taskScreen.answerState == AnswerState.Right)
                        return;
                    if ((int)e.KeyCode >= 7 && (int)e.KeyCode <= 16)
                    {
                        taskScreen.playerAnswer += (int)e.KeyCode - 7;
                        return;
                    }
                    var str = KeyEvent.KeyCodeToString(e.KeyCode);
                    taskScreen.playerAnswer += str.Substring(str.Length - 1).ToLower();
                }
            }
        }
    }
}

