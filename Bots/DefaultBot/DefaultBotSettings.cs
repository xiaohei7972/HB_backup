using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using log4net;
using Newtonsoft.Json;
using Triton.Bot.Settings;
using Triton.Common;
using Triton.Common.LogUtilities;
using Triton.Game.Mapping;
using GreyMagic;

namespace Triton.Bot.Logic.Bots.DefaultBot
{
    /// <summary>
    /// 默认机器人设置类
    /// <para>存储 DefaultBot 的所有配置和运行时数据</para>
    /// <para>继承自 JsonSettings，支持 JSON 序列化/反序列化和属性变更通知</para>
    /// </summary>
    /// <remarks>
    /// 主要功能：
    /// <list type="bullet">
    ///     <item><description>对战模式选择（狂野/标准/经典/休闲/幻变）</description></item>
    ///     <item><description>卡组选择和缓存</description></item>
    ///     <item><description>自动打招呼设置</description></item>
    ///     <item><description>窗口大小限制设置</description></item>
    ///     <item><description>自动投降设置（保持排名/互投/急速投）</description></item>
    /// </list>
    /// </remarks>
    public class DefaultBotSettings : JsonSettings
    {
        private static readonly ILog _log = Common.LogUtilities.Logger.GetLoggerInstanceForType();

        private static DefaultBotSettings _instance;
        private static readonly object _lock = new object();

        public DefaultBotSettings() : base(GetSettingsFilePath(
            Configuration.Instance.Name, string.Format("{0}.json", "DefaultBot")))
        {

        }

        public static DefaultBotSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new DefaultBotSettings();
                    }
                }
                return _instance;
            }
        }

        public void ReloadFile()
        {
            Reload(GetSettingsFilePath(Configuration.Instance.Name,
                string.Format("{0}.json", "DefaultBot")));
            if (CommandLine.Arguments.Exists("rule"))
            {
                try
                {
                    ConstructedGameRule = (VisualsFormatType)(int.Parse(CommandLine.Arguments.Single("rule")) + 1);
                    _log.ErrorFormat("[中控设置] 传统对战模式 = {0}.", ConstructedGameRule);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("[中控设置] 解析 rule 参数失败: {0}.", e.Message);
                }
            }
            if (CommandLine.Arguments.Exists("deck"))
            {
                ConstructedCustomDeck = CommandLine.Arguments.Single("deck");
                _log.ErrorFormat("[中控设置] 对战卡组名称 = {0}.", ConstructedCustomDeck);
            }
            if (CommandLine.Arguments.Exists("width"))
            {
                ReleaseLimit = true;
                try
                {
                    ReleaseLimitW = int.Parse(CommandLine.Arguments.Single("width"));
                    _log.ErrorFormat("[中控设置] 炉石窗口宽度 = {0}.", ReleaseLimitW);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("[中控设置] 解析 width 参数失败: {0}.", e.Message);
                }
            }
            if (CommandLine.Arguments.Exists("height"))
            {
                ReleaseLimit = true;
                try
                {
                    ReleaseLimitH = int.Parse(CommandLine.Arguments.Single("height"));
                    _log.ErrorFormat("[中控设置] 炉石窗口高度 = {0}.", ReleaseLimitH);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("[中控设置] 解析 height 参数失败: {0}.", e.Message);
                }
            }
        }

        //下拉框数据
        private ObservableCollection<VisualsFormatType> _allConstructedRules;
        [JsonIgnore]
        public ObservableCollection<VisualsFormatType> AllConstructedRules
        {
            get
            {
                ObservableCollection<VisualsFormatType> result;
                if ((result = _allConstructedRules) == null)
                {
                    ObservableCollection<VisualsFormatType> observableCollection = new ObservableCollection<VisualsFormatType>();
                    observableCollection.Add(VisualsFormatType.狂野);
                    observableCollection.Add(VisualsFormatType.标准);
                    observableCollection.Add(VisualsFormatType.经典);
                    observableCollection.Add(VisualsFormatType.休闲);
                    observableCollection.Add(VisualsFormatType.幻变);
                    _allConstructedRules = observableCollection;
                    result = observableCollection;
                }
                return result;
            }
        }

        //当前对战模式
        private VisualsFormatType _constructedGameRule;
        [DefaultValue(VisualsFormatType.狂野)]
        public VisualsFormatType ConstructedGameRule
        {
            get { return _constructedGameRule; }
            set
            {
                if (!value.Equals(_constructedGameRule))
                {
                    _constructedGameRule = value;
                    NotifyPropertyChanged(() => ConstructedGameRule);
                    _log.InfoFormat("[天梯脚本设置] 对战模式 = {0}.", _constructedGameRule);
                }
            }
        }

        //卡组名称
        private string _constructedCustomDeck;
        [DefaultValue("请选择卡组")]
        public string ConstructedCustomDeck
        {
            get { return _constructedCustomDeck; }
            set
            {
                string text = value;
                if (text == null) text = string.Empty;
                if (!text.Equals(_constructedCustomDeck))
                {
                    _constructedCustomDeck = text;
                    NotifyPropertyChanged(() => ConstructedCustomDeck);
                    _log.InfoFormat("[天梯脚本设置] 卡组名称 = {0}.", _constructedCustomDeck);
                }
            }
        }

        //上次卡组ID
        private long _lastDeckId;
        public long LastDeckId
        {
            get { return _lastDeckId; }
            internal set { _lastDeckId = value; }
        }

        //开局自动打招呼
        private bool _autoGreet;
        [DefaultValue(false)]
        public bool AutoGreet
        {
            get { return _autoGreet; }
            set
            {
                if (!value.Equals(_autoGreet))
                {
                    _autoGreet = value;
                    NotifyPropertyChanged(() => AutoGreet);
                    _log.InfoFormat("[天梯脚本设置] 自动打招呼 = {0}.", _autoGreet);
                }
            }
        }

        //需要缓存卡组
        private bool _needsToCacheCustomDecks;
        [JsonIgnore]
        [DefaultValue(true)]
        public bool NeedsToCacheCustomDecks
        {
            get { return _needsToCacheCustomDecks; }
            set
            {
                if (!value.Equals(_needsToCacheCustomDecks))
                {
                    _needsToCacheCustomDecks = value;
                    NotifyPropertyChanged(() => NeedsToCacheCustomDecks);
                    _log.InfoFormat("[天梯脚本设置] 需要缓存卡组 = {0}.", _needsToCacheCustomDecks);
                }
            }
        }

        //炉石窗口宽度
        private int _releaseLimitW = 144;
        [DefaultValue(144)]
        public int ReleaseLimitW
        {
            get { return _releaseLimitW; }
            set
            {
                if (!value.Equals(_releaseLimitW))
                {
                    _releaseLimitW = value;
                    if (_releaseLimitW < 120) _releaseLimitW = 120;
                    if (_releaseLimitW > 1920) _releaseLimitW = 1920;
                    if (ReleaseLimitH != (_releaseLimitW / 4 * 3))
                    {
                        ReleaseLimitH = _releaseLimitW / 4 * 3;
                    }
                    NotifyPropertyChanged(() => ReleaseLimitW);
                    _log.InfoFormat("[天梯脚本设置] 炉石窗口宽度 = {0}.", _releaseLimitW);
                }
                try
                {
                    if (BotManager.IsRunning && ReleaseLimit)
                        Screen.SetResolution(_releaseLimitW, _releaseLimitH, 3, 0);
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("An exception occurred: {0}.", e);
                }
            }
        }

        //炉石窗口高度
        private int _releaseLimitH = 108;
        [DefaultValue(108)]
        public int ReleaseLimitH
        {
            get { return _releaseLimitH; }
            set
            {
                if (!value.Equals(_releaseLimitH))
                {
                    _releaseLimitH = value;
                    if (_releaseLimitH < 90) _releaseLimitH = 90;
                    if (_releaseLimitH > 1080) _releaseLimitH = 1080;
                    NotifyPropertyChanged(() => ReleaseLimitH);
                    _log.InfoFormat("[天梯脚本设置] 炉石窗口高度 = {0}.", _releaseLimitH);
                }
            }
        }

        //设置炉石窗口大小
        private bool _releaseLimit;
        [DefaultValue(true)]
        public bool ReleaseLimit
        {
            get { return _releaseLimit; }
            set
            {
                if (!value.Equals(_releaseLimit))
                {
                    _releaseLimit = value;
                    NotifyPropertyChanged(() => ReleaseLimit);
                    _log.InfoFormat("[天梯脚本设置] 自动设置炉石窗口宽高 = {0}.", _releaseLimit);
                }
                try
                {
                    if (_releaseLimit)
                    {
                        if (BotManager.IsRunning)
                            Screen.SetResolution(_releaseLimitW, _releaseLimitH, 3, 0);
                    }
                    else
                    {
                        if (BotManager.IsRunning)
                            Screen.SetResolution(1280, 720, 3, 0);
                    }
                }
                catch (Exception e)
                {
                    _log.ErrorFormat("An exception occurred: {0}.", e);
                }
            }
        }

        //保持排名(赢1投1)
        private bool _autoConcedeAfterConstructedWin;
        [DefaultValue(false)]
        public bool AutoConcedeAfterConstructedWin
        {
            get { return _autoConcedeAfterConstructedWin; }
            set
            {
                if (!value.Equals(_autoConcedeAfterConstructedWin))
                {
                    _autoConcedeAfterConstructedWin = value;
                    NotifyPropertyChanged(() => AutoConcedeAfterConstructedWin);
                    _log.InfoFormat("[天梯脚本设置] 保持排名(赢{0}投{1}) = {2}.", _autoConcedeNumberOfWins, _autoConcedeNumberOfLosses, _autoConcedeAfterConstructedWin);
                }
                if (AutoConcedeAfterConstructedWin)
                {
                    if (ForceConcedeAtMulligan) ForceConcedeAtMulligan = false;
                    if (NormalConcede) NormalConcede = false;
                }
                if (NeedNowConcede) NeedNowConcede = false;
            }
        }

        private int _autoConcedeNumberOfWins = 1;
        private int _autoConcedeNumberOfLosses = 1;
        public int NowWinGameCount = 0;
        public int NowLoseGameCount = 0;

        [DefaultValue(1)]
        public int AutoConcedeNumberOfWins
        {
            get { return _autoConcedeNumberOfWins; }
            set
            {
                if (!value.Equals(_autoConcedeNumberOfWins))
                {
                    _autoConcedeNumberOfWins = value;
                    if (_autoConcedeNumberOfWins < 0) _autoConcedeNumberOfWins = 1;
                    NotifyPropertyChanged(() => AutoConcedeNumberOfWins);
                    _log.InfoFormat("[天梯脚本设置] 保持排名(赢{0}投{1}) = 赢{2}.", _autoConcedeNumberOfWins, _autoConcedeNumberOfLosses, _autoConcedeNumberOfWins);
                }
            }
        }

        [DefaultValue(1)]
        public int AutoConcedeNumberOfLosses
        {
            get { return _autoConcedeNumberOfLosses; }
            set
            {
                if (!value.Equals(_autoConcedeNumberOfLosses))
                {
                    _autoConcedeNumberOfLosses = value;
                    if (_autoConcedeNumberOfLosses < 0) _autoConcedeNumberOfLosses = 1;
                    NotifyPropertyChanged(() => AutoConcedeNumberOfLosses);
                    _log.InfoFormat("[天梯脚本设置] 保持排名(赢{0}投{1}) = 投{2}.", _autoConcedeNumberOfWins, _autoConcedeNumberOfLosses, _autoConcedeNumberOfLosses);
                }
            }
        }

        //普通互投拿千胜头像
        private bool _normalConcede;
        [DefaultValue(false)]
        public bool NormalConcede
        {
            get { return _normalConcede; }
            set
            {
                if (!value.Equals(_normalConcede))
                {
                    _normalConcede = value;
                    NotifyPropertyChanged(() => NormalConcede);
                    _log.InfoFormat("[天梯脚本设置] 普通互投拿千胜头像 = {0}.", _normalConcede);
                }
                if (NormalConcede)
                {
                    if (AutoConcedeAfterConstructedWin) AutoConcedeAfterConstructedWin = false;
                    if (ForceConcedeAtMulligan) ForceConcedeAtMulligan = false;
                }
                if (NeedNowConcede) NeedNowConcede = false;
            }
        }

        //急速投降至互投区
        private bool _forceConcedeAtMulligan;
        [DefaultValue(false)]
        public bool ForceConcedeAtMulligan
        {
            get { return _forceConcedeAtMulligan; }
            set
            {
                if (!value.Equals(_forceConcedeAtMulligan))
                {
                    _forceConcedeAtMulligan = value;
                    NotifyPropertyChanged(() => ForceConcedeAtMulligan);
                    _log.InfoFormat("[天梯脚本设置] 急速投降至互投区 = {0}.", _forceConcedeAtMulligan);
                }
                if (ForceConcedeAtMulligan)
                {
                    if (AutoConcedeAfterConstructedWin) AutoConcedeAfterConstructedWin = false;
                    if (NormalConcede) NormalConcede = false;
                    if (!NeedNowConcede) NeedNowConcede = true;
                }
                else
                {
                    if (NeedNowConcede) NeedNowConcede = false;
                }
            }
        }

        //判断对面名字投降
        private bool _judgmentOpponentNameConcede;
        [DefaultValue(false)]
        public bool JudgmentOpponentNameConcede
        {
            get { return _judgmentOpponentNameConcede; }
            set
            {
                if (!value.Equals(_judgmentOpponentNameConcede))
                {
                    _judgmentOpponentNameConcede = value;
                    NotifyPropertyChanged(() => JudgmentOpponentNameConcede);
                    _log.InfoFormat("[天梯脚本设置] 判断对面名字投降 = {0}.", _judgmentOpponentNameConcede);
                }
            }
        }

        //内置极速投降参数
        private bool _needNowConcede;
        [DefaultValue(false)]
        [JsonIgnore]
        public bool NeedNowConcede
        {
            get { return _needNowConcede; }
            set
            {
                if (!value.Equals(_needNowConcede))
                {
                    _needNowConcede = value;
                    if (value && !NormalConcede &&
                        !AutoConcedeAfterConstructedWin && !ForceConcedeAtMulligan)
                    {
                        _needNowConcede = false;
                    }
                    NotifyPropertyChanged(() => NeedNowConcede);
                    _log.InfoFormat("[天梯脚本设置] 立即投降 = {0}.", _needNowConcede);
                }
            }
        }

        //投降最小延时
        private int _autoConcedeMinDelayMs = 1500;
        [DefaultValue(1500)]
        public int AutoConcedeMinDelayMs
        {
            get { return _autoConcedeMinDelayMs; }
            set
            {
                if (!value.Equals(_autoConcedeMinDelayMs))
                {
                    _autoConcedeMinDelayMs = value;
                    if (_autoConcedeMinDelayMs < 0) _autoConcedeMinDelayMs = 0;
                    if (_autoConcedeMinDelayMs > _autoConcedeMaxDelayMs) _autoConcedeMinDelayMs = _autoConcedeMaxDelayMs;
                    NotifyPropertyChanged(() => AutoConcedeMinDelayMs);
                    _log.InfoFormat("[天梯脚本设置] 投降最小延时(ms) = {0}.", _autoConcedeMinDelayMs);
                }
            }
        }

        //投降最大延时
        private int _autoConcedeMaxDelayMs = 3000;
        [DefaultValue(3000)]
        public int AutoConcedeMaxDelayMs
        {
            get { return _autoConcedeMaxDelayMs; }
            set
            {
                if (!value.Equals(_autoConcedeMaxDelayMs))
                {
                    _autoConcedeMaxDelayMs = value;
                    if (_autoConcedeMaxDelayMs < 0) _autoConcedeMaxDelayMs = 0;
                    if (_autoConcedeMaxDelayMs < _autoConcedeMinDelayMs) _autoConcedeMaxDelayMs = _autoConcedeMinDelayMs;
                    NotifyPropertyChanged(() => AutoConcedeMaxDelayMs);
                    _log.InfoFormat("[天梯脚本设置] 投降最大延时(ms)  = {0}.", _autoConcedeMaxDelayMs);
                }
            }
        }

        //全局动画速度
        /* private string s_001;
        [DefaultValue("1.0")]
        public string SliderShopSpeedRatioText
        {
            get { return s_001; }
            set
            {
                if (!value.Equals(s_001))
                {
                    s_001 = value;
                    NotifyPropertyChanged(() => SliderShopSpeedRatioText);
                }
                ilog_0.InfoFormat("[天梯脚本设置] 全局动画速度(齿轮) = {0}.", s_001);
            }
        } */

        //全局动画速度滑动条
        /* private float i_001;
        private int i_002;
        private float i_013;
        [DefaultValue(1.0f)]
        public float SliderShopSpeedRatio
        {
            get { return i_001; }
            set
            {
                if (!value.Equals(i_001))
                {
                    i_001 = value;
                    NotifyPropertyChanged(() => SliderShopSpeedRatio);
                    Configuration.Instance.SaveAll();
                }
                try
                {
                    if (value > 1 && value < 2)
                    {
                        float f = value;
                        int i = (int)(f * 100);
                        f = (float)(i * 1.0) / 100;
                        if (i_013 != f)
                        {
                            SliderShopSpeedRatioText = f.ToString("F1");
                            if (BotManager.IsRunning)
                                TimeScaleMgr.Get().SetGameTimeScale((float)f);
                            i_013 = f;
                        }
                    }
                    else
                    {
                        int a = (int)value;
                        if (i_002 != a)
                        {
                            SliderShopSpeedRatioText = a.ToString("F1");
                            if (BotManager.IsRunning)
                                TimeScaleMgr.Get().SetGameTimeScale((float)a);
                            i_002 = a;
                        }
                    }
                }
                catch (Exception e)
                {
                    ilog_0.ErrorFormat("An exception occurred: {0}.", e);
                }
            }
        }*/


    }
    
} 
