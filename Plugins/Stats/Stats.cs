using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using log4net;
using Triton.Bot;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Common.Mvvm;
using Triton.Game;
using Triton.Game.Mapping;
using Logger = Triton.Common.LogUtilities.Logger;

namespace Stats
{
    public class Stats : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private bool _enabled;

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get { return "统计插件"; }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get { return "统计对战十一职业的胜场、败场以及胜率。"; }
        }

        /// <summary>The author of the plugin.</summary>
        public string Author
        {
            get { return "开源软件"; }
        }

        /// <summary>The version of the plugin.</summary>
        public string Version
        {
            get { return "1.0.0.0"; }
        }

        /// <summary>Initializes this object. This is called when the object is loaded into the bot.</summary>
        public void Initialize()
        {
            Log.DebugFormat("[统计插件] 初始化");
            if (!MainSettings.Instance.EnabledPlugins.Contains(this.Name))
                MainSettings.Instance.EnabledPlugins.Add(this.Name);
        }

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[统计插件] 开启");
            GameEventManager.GameOver += GameEventManagerOnGameOver;
            StatsSettings.Instance.ReloadFile();
        }

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            Log.DebugFormat("[统计插件] 停止");
            GameEventManager.GameOver -= GameEventManagerOnGameOver;
        }


        public JsonSettings Settings
        {
            get { return StatsSettings.Instance; }
        }

        private UserControl _control;

        /// <summary> The plugin's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                if (_control != null)
                {
                    return _control;
                }

                using (var fs = new FileStream(@"Plugins\Stats\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl)XamlReader.Load(fs);
                    var viewModel = new StatsViewModel(StatsSettings.Instance);
                    viewModel.RequestReset += ResetButtonOnClick;
                    root.DataContext = viewModel;
                    UpdateMainGuiStats();
                    _control = root;
                }

                return _control;
            }
        }

        /// <summary>Is this plugin currently enabled?</summary>
        public bool IsEnabled
        {
            get { return _enabled; }
        }

        /// <summary> The plugin is being enabled.</summary>
        public void Enable()
        {
            Log.DebugFormat("[统计插件] 启用");
            _enabled = true;
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[统计插件] 禁用");
            _enabled = false;
        }

        /// <summary>Deinitializes this object. This is called when the object is being unloaded from the bot.</summary>
        public void Deinitialize()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + Description;
        }

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {
            try
            {
                using (TritonHs.AcquireFrame())
                {
                    HSCard c = TritonHs.EnemyHero;
                    if (c == null) return;
                    TAG_CLASS cc = c.Class;
                    if (gameOverEventArgs.Result == GameOverFlag.Victory)
                    {
                        StatsSettings.Instance.Wins++;

                        switch ((int)cc)
                        {
                            case 10: StatsSettings.Instance.Wins1++; break;//战士
                            case 8: StatsSettings.Instance.Wins2++; break;//萨满
                            case 7: StatsSettings.Instance.Wins3++; break;//盗贼
                            case 5: StatsSettings.Instance.Wins4++; break;//圣骑士
                            case 3: StatsSettings.Instance.Wins5++; break;//猎人
                            case 2: StatsSettings.Instance.Wins6++; break;//德鲁伊
                            case 9: StatsSettings.Instance.Wins7++; break;//术士
                            case 4: StatsSettings.Instance.Wins8++; break;//法师
                            case 6: StatsSettings.Instance.Wins9++; break;//牧师
                            case 14: StatsSettings.Instance.Wins10++; break;//恶魔猎手
                            case 1: StatsSettings.Instance.Wins11++; break;//死亡骑士
                        }
                        UpdateMainGuiStats();
                    }
                    else if (gameOverEventArgs.Result == GameOverFlag.Defeat)
                    {

                        StatsSettings.Instance.Losses++;
                        switch ((int)cc)
                        {
                            case 10: StatsSettings.Instance.Losses1++; break;//战士
                            case 8: StatsSettings.Instance.Losses2++; break;//萨满
                            case 7: StatsSettings.Instance.Losses3++; break;//盗贼
                            case 5: StatsSettings.Instance.Losses4++; break;//圣骑士
                            case 3: StatsSettings.Instance.Losses5++; break;//猎人
                            case 2: StatsSettings.Instance.Losses6++; break;//德鲁伊
                            case 9: StatsSettings.Instance.Losses7++; break;//术士
                            case 4: StatsSettings.Instance.Losses8++; break;//法师
                            case 6: StatsSettings.Instance.Losses9++; break;//牧师
                            case 14: StatsSettings.Instance.Losses10++; break;//恶魔猎手
                            case 1: StatsSettings.Instance.Losses11++; break;//死亡骑士
                        }
                        UpdateMainGuiStats();
                    }
                }
                //GameEventManager.GameOver += GameEventManagerOnGameOver;
            }

            catch (Exception e)
            {
                Log.ErrorFormat("[统计插件] 错误: {0}", e);
            }
            //Log.ErrorFormat("[统计插件]----------------------------------");
        }

        private void ResetButtonOnClick()
        {
            StatsSettings.Instance.Wins = 0;
            StatsSettings.Instance.Wins1 = 0;
            StatsSettings.Instance.Wins2 = 0;
            StatsSettings.Instance.Wins3 = 0;
            StatsSettings.Instance.Wins4 = 0;
            StatsSettings.Instance.Wins5 = 0;
            StatsSettings.Instance.Wins6 = 0;
            StatsSettings.Instance.Wins7 = 0;
            StatsSettings.Instance.Wins8 = 0;
            StatsSettings.Instance.Wins9 = 0;
            StatsSettings.Instance.Wins10 = 0;
            StatsSettings.Instance.Wins11 = 0;

            StatsSettings.Instance.Losses = 0;
            StatsSettings.Instance.Losses1 = 0;
            StatsSettings.Instance.Losses2 = 0;
            StatsSettings.Instance.Losses3 = 0;
            StatsSettings.Instance.Losses4 = 0;
            StatsSettings.Instance.Losses5 = 0;
            StatsSettings.Instance.Losses6 = 0;
            StatsSettings.Instance.Losses7 = 0;
            StatsSettings.Instance.Losses8 = 0;
            StatsSettings.Instance.Losses9 = 0;
            StatsSettings.Instance.Losses10 = 0;
            StatsSettings.Instance.Losses11 = 0;

            StatsSettings.Instance.Winrate = "0";
            StatsSettings.Instance.Winrate1 = "0";
            StatsSettings.Instance.Winrate2 = "0";
            StatsSettings.Instance.Winrate3 = "0";
            StatsSettings.Instance.Winrate4 = "0";
            StatsSettings.Instance.Winrate5 = "0";
            StatsSettings.Instance.Winrate6 = "0";
            StatsSettings.Instance.Winrate7 = "0";
            StatsSettings.Instance.Winrate8 = "0";
            StatsSettings.Instance.Winrate9 = "0";
            StatsSettings.Instance.Winrate10 = "0";
            StatsSettings.Instance.Winrate11 = "0";

            StatsSettings.Instance.environment = "0";
            StatsSettings.Instance.environment1 = "0";
            StatsSettings.Instance.environment2 = "0";
            StatsSettings.Instance.environment3 = "0";
            StatsSettings.Instance.environment4 = "0";
            StatsSettings.Instance.environment5 = "0";
            StatsSettings.Instance.environment6 = "0";
            StatsSettings.Instance.environment7 = "0";
            StatsSettings.Instance.environment8 = "0";
            StatsSettings.Instance.environment9 = "0";
            StatsSettings.Instance.environment10 = "0";
            StatsSettings.Instance.environment11 = "0";

            UpdateMainGuiStats();
        }

        private string EM(int win, int lost, int allwin, int alllost)
        {
            if (win + lost == 0) return "0";
            return string.Format("{0:0}%", 100 *  (float)(win + lost) / (float)(allwin+alllost) );
        }
        private string WR(int win, int lost)
        {
            if (win + lost == 0) return "0";
            return string.Format("{0:0}%", 100 * (float)win / (float)(win + lost));
        }

        private void UpdateMainGuiStats()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var statusBar = Triton.Common.Mvvm.ViewModelLocator.GetSingleton<Triton.Common.Mvvm.StatusBarViewModel>();

                if (StatsSettings.Instance.Wins + StatsSettings.Instance.Losses == 0)
                {
                    if (StatsSettings.Instance.Winrate == "0")
                    {
                        statusBar.RightText = string.Format("(没有对局信息)");
                    }

                }
                else
                {
                    StatsSettings.Instance.Winrate = WR(StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.Winrate1 = WR(StatsSettings.Instance.Wins1, StatsSettings.Instance.Losses1);
                    StatsSettings.Instance.Winrate2 = WR(StatsSettings.Instance.Wins2, StatsSettings.Instance.Losses2);
                    StatsSettings.Instance.Winrate3 = WR(StatsSettings.Instance.Wins3, StatsSettings.Instance.Losses3);
                    StatsSettings.Instance.Winrate4 = WR(StatsSettings.Instance.Wins4, StatsSettings.Instance.Losses4);
                    StatsSettings.Instance.Winrate5 = WR(StatsSettings.Instance.Wins5, StatsSettings.Instance.Losses5);
                    StatsSettings.Instance.Winrate6 = WR(StatsSettings.Instance.Wins6, StatsSettings.Instance.Losses6);
                    StatsSettings.Instance.Winrate7 = WR(StatsSettings.Instance.Wins7, StatsSettings.Instance.Losses7);
                    StatsSettings.Instance.Winrate8 = WR(StatsSettings.Instance.Wins8, StatsSettings.Instance.Losses8);
                    StatsSettings.Instance.Winrate9 = WR(StatsSettings.Instance.Wins9, StatsSettings.Instance.Losses9);
                    StatsSettings.Instance.Winrate10 = WR(StatsSettings.Instance.Wins10, StatsSettings.Instance.Losses10);
                    StatsSettings.Instance.Winrate11 = WR(StatsSettings.Instance.Wins11, StatsSettings.Instance.Losses11);

                    StatsSettings.Instance.environment = EM(StatsSettings.Instance.Wins, StatsSettings.Instance.Losses, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment1 = EM(StatsSettings.Instance.Wins1, StatsSettings.Instance.Losses1, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment2 = EM(StatsSettings.Instance.Wins2, StatsSettings.Instance.Losses2, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment3 = EM(StatsSettings.Instance.Wins3, StatsSettings.Instance.Losses3, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment4 = EM(StatsSettings.Instance.Wins4, StatsSettings.Instance.Losses4, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment5 = EM(StatsSettings.Instance.Wins5, StatsSettings.Instance.Losses5, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment6 = EM(StatsSettings.Instance.Wins6, StatsSettings.Instance.Losses6, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment7 = EM(StatsSettings.Instance.Wins7, StatsSettings.Instance.Losses7, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment8 = EM(StatsSettings.Instance.Wins8, StatsSettings.Instance.Losses8, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment9 = EM(StatsSettings.Instance.Wins9, StatsSettings.Instance.Losses9, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment10 = EM(StatsSettings.Instance.Wins10, StatsSettings.Instance.Losses10, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                    StatsSettings.Instance.environment11 = EM(StatsSettings.Instance.Wins11, StatsSettings.Instance.Losses11, StatsSettings.Instance.Wins, StatsSettings.Instance.Losses);
                }

                Configuration.Instance.SaveAll();
            }));
        }

    }
}
