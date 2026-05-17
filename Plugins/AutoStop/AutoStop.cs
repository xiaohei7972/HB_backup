using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using log4net;
using Triton.Bot;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Common.Mvvm;
using Triton.Game;
using Logger = Triton.Common.LogUtilities.Logger;
using HREngine.Bots;

namespace AutoStop
{
    public class AutoStop : IPlugin
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        private bool _enabled;

        private UserControl _control;
        private DateTime _gameStartTime;
        private DispatcherTimer _concedeTimer;
        private int _originalEnfaceReward;
        private bool _facePenaltySwitched;

        /// <summary> The name of the plugin. </summary>
        public string Name
        {
            get { return "自动停止插件"; }
        }

        /// <summary> The description of the plugin. </summary>
        public string Description
        {
            get { return "自动停止插件。"; }
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
            Log.DebugFormat("[自动停止插件] 初始化");
            if (!MainSettings.Instance.EnabledPlugins.Contains(this.Name))
                MainSettings.Instance.EnabledPlugins.Add(this.Name);
        }

        /// <summary> The plugin start callback. Do any initialization here. </summary>
        public void Start()
        {
            Log.DebugFormat("[自动停止插件] 开启");
            GameEventManager.GameOver += GameEventManagerOnGameOver;
            _concedeTimer = new DispatcherTimer();
            _concedeTimer.Interval = TimeSpan.FromSeconds(10);
            _concedeTimer.Tick += ConcedeTimerOnElapsed;
            _concedeTimer.Start();
            AutoStopSettings.Instance.ReloadFile();
            _originalEnfaceReward = DefaultRoutineSettings.Instance.EnfaceReward;
            _facePenaltySwitched = false;
            Log.InfoFormat("[自动停止] 保存原始打脸惩罚值: {0}", _originalEnfaceReward);
        }

        /// <summary> The plugin tick callback. Do any update logic here. </summary>
        public void Tick()
        {
            CheckGameStart();
        }

        /// <summary> The plugin stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            Log.DebugFormat("[自动停止插件] 停止");

            GameEventManager.GameOver -= GameEventManagerOnGameOver;
            
            if (_concedeTimer != null)
            {
                _concedeTimer.Stop();
                _concedeTimer.Tick -= ConcedeTimerOnElapsed;
                _concedeTimer = null;
            }
        }

        public JsonSettings Settings
        {
            get { return AutoStopSettings.Instance; }
        }

        /// <summary> The plugin's settings control. This will be added to the Hearthbuddy Settings tab.</summary>
        public UserControl Control
        {
            get
            {
                if (_control != null)
                {
                    return _control;
                }

                using (var fs = new FileStream(@"Plugins\AutoStop\SettingsGui.xaml", FileMode.Open))
                {
                    var root = (UserControl) XamlReader.Load(fs);
                    var viewModel = new AutoStopViewModel(AutoStopSettings.Instance);
                    viewModel.RequestReset += ResetButtonOnClick;
                    root.DataContext = viewModel;
                    _control = root;
                }

                return _control;
            }
        }

        private void ResetButtonOnClick()
        {
            using (TritonHs.AcquireFrame())
            {
                AutoStopSettings.Instance.Reset();
            }
        }

        private void CheckGameStart()
        {
            try
            {
                using (TritonHs.AcquireFrame())
                {
                    if (TritonHs.GameState != null && _gameStartTime == DateTime.MinValue)
                    {
                        _gameStartTime = DateTime.Now;
                        _facePenaltySwitched = false;
                        Log.InfoFormat("[自动停止] 游戏开始时间记录: {0}", _gameStartTime.ToString("HH:mm:ss"));
                        
                        if (AutoStopSettings.Instance.DynamicFacePenaltyEnabled)
                        {
                            printUtils.enfaceReward = AutoStopSettings.Instance.FacePenaltyBeforeTime;
                            Log.InfoFormat("[自动停止] 动态打脸惩罚启用，设置打脸惩罚为 {0}（时间前）", AutoStopSettings.Instance.FacePenaltyBeforeTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[自动停止] 检测游戏开始错误: {0}", ex);
            }
        }

        private void ConcedeTimerOnElapsed(object sender, EventArgs e)
        {
            try
            {
                if (_gameStartTime == DateTime.MinValue)
                {
                    return;
                }

                using (TritonHs.AcquireFrame())
                {
                    if (TritonHs.GameState == null)
                    {
                        return;
                    }

                    TimeSpan gameTime = DateTime.Now - _gameStartTime;

                    if (AutoStopSettings.Instance.DynamicFacePenaltyEnabled && !_facePenaltySwitched)
                    {
                        int facePenaltyMinutes = AutoStopSettings.Instance.DynamicFacePenaltyMinutes;
                        if (gameTime.TotalMinutes >= facePenaltyMinutes)
                        {
                            int oldReward = printUtils.enfaceReward;
                            printUtils.enfaceReward = AutoStopSettings.Instance.FacePenaltyAfterTime;
                            _facePenaltySwitched = true;
                            Log.InfoFormat("[自动停止] 游戏已进行 {0:0.0} 分钟，打脸惩罚从 {1} 切换为 {2}（时间后）",
                                gameTime.TotalMinutes, oldReward, AutoStopSettings.Instance.FacePenaltyAfterTime);
                        }
                    }

                    if (!AutoStopSettings.Instance.ConcedeAfterXMinutes)
                    {
                        return;
                    }

                    int minutesLimit = AutoStopSettings.Instance.ConcedeMinutesCount;
                    
                    if (gameTime.TotalMinutes >= minutesLimit)
                    {
                        Log.InfoFormat("[自动停止] 游戏时长已达到 {0} 分钟，执行自动投降", minutesLimit);
                        
                        if (TritonHs.GameState.IsFriendlySidePlayerTurn())
                        {
                            Log.InfoFormat("[自动停止] 当前是我方回合，执行投降");
                            TritonHs.Concede(true);
                            _gameStartTime = DateTime.MinValue;
                        }
                        else
                        {
                            Log.InfoFormat("[自动停止] 当前不是我方回合，等待我方回合时投降");
                        }
                    }
                    else
                    {
                        int remainingSeconds = minutesLimit * 60 - (int)gameTime.TotalSeconds;
                        if (remainingSeconds % 60 == 0 && remainingSeconds <= 300)
                        {
                            Log.InfoFormat("[自动停止] 游戏已进行 {0:0.0} 分钟，还有 {1} 分钟将执行自动投降", 
                                gameTime.TotalMinutes, 
                                remainingSeconds / 60);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("[自动停止] 计时器错误: {0}", ex);
            }
        }

        private void GameEventManagerOnGameOver(object sender, GameOverEventArgs gameOverEventArgs)
        {
            if (AutoStopSettings.Instance.DynamicFacePenaltyEnabled)
            {
                printUtils.enfaceReward = _originalEnfaceReward;
                DefaultRoutineSettings.Instance.EnfaceReward = _originalEnfaceReward;
                Log.InfoFormat("[自动停止] 游戏结束，恢复打脸惩罚为 {0}", _originalEnfaceReward);
            }
            _facePenaltySwitched = false;
            _gameStartTime = DateTime.MinValue;
            
            if (gameOverEventArgs.Result == GameOverFlag.Victory)
            {
                AutoStopSettings.Instance.Wins++;
            }
            else if (gameOverEventArgs.Result == GameOverFlag.Defeat)
            {
                AutoStopSettings.Instance.Losses++;
            }
            if (AutoStopSettings.Instance.StopAfterXGames)
            {
                var total = AutoStopSettings.Instance.Losses + AutoStopSettings.Instance.Wins;
                if (total >= AutoStopSettings.Instance.StopGameCount)
                {
                    Log.InfoFormat(
    "[自动停止]现在停止脚本，因为已经完成了目标总场数.",
    total, AutoStopSettings.Instance.StopGameCount);
                    BotManager.Stop();
                    AutoStopSettings.Instance.Reset();
                    return;
                }
            }

            if (AutoStopSettings.Instance.StopAfterXWins)
            {
                var total = AutoStopSettings.Instance.Wins;
                if (total >= AutoStopSettings.Instance.StopWinCount)
                {
                    Log.InfoFormat(
    "[自动停止]现在停止脚本，因为已经完成了目标胜场.",
    total, AutoStopSettings.Instance.StopGameCount);
                    BotManager.Stop();
                    AutoStopSettings.Instance.Reset();
                    return;
                }
            }

            if (AutoStopSettings.Instance.StopAfterXLosses)
            {
                var total = AutoStopSettings.Instance.Losses;
                if (total >= AutoStopSettings.Instance.StopLossCount)
                {
                    Log.InfoFormat(
    "[自动停止]现在停止脚本，因为已经完成了目标败场.",
    total, AutoStopSettings.Instance.StopGameCount);
                    BotManager.Stop();
                    AutoStopSettings.Instance.Reset();
                    return;
                }
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
            Log.DebugFormat("[自动停止插件] 启用");
            _enabled = true;
        }

        /// <summary> The plugin is being disabled.</summary>
        public void Disable()
        {
            Log.DebugFormat("[自动停止插件] 禁用");
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
    }
}

