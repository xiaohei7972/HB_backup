using System;
using Triton.Game.Mapping;
using Triton.Game;
using Logger = Triton.Common.LogUtilities.Logger;

namespace HREngine.Bots
{
    public class OpponentInfo
    {
        private static OpponentInfo instance;
        private static bool hasShownOpponentInfo = false;
        private static int retryCount = 0;
        private const int MAX_RETRY_COUNT = 5;

        public static OpponentInfo GetInstance
        {
            get
            {   
                if (instance == null)
                {   
                    instance = new OpponentInfo();
                }
                return instance;
            }
        }

        public string GetOpponentInfo()
        {
            try
            {   
                // 获取游戏状态
                GameState gameState = GameState.Get();
                if (gameState == null)
                {   
                    return "";
                }

                // 获取对手玩家
                Player opponentPlayer = gameState.GetOpposingSidePlayer();
                if (opponentPlayer == null)
                {   
                    return "";
                }

                // 获取对手名称
                string opponentName = opponentPlayer.GetName() ?? "未知对手";

                // 获取对手BattleTag
                string battleTag = GetBattleTag(opponentPlayer);

                // 获取对手英雄
                string heroName = GetHeroName(opponentPlayer);

                return !string.IsNullOrEmpty(battleTag)
                    ? string.Format("[对手信息] 对手: {0} ({1}) - 英雄: {2}", opponentName, battleTag, heroName)
                    : string.Format("[对手信息] 对手: {0} - 英雄: {1}", opponentName, heroName);
            }
            catch (Exception ex)
            {   
                Logger.GetLoggerInstanceForType().DebugFormat("获取对手信息时出错: {0}", ex.Message);
                return "";
            }
        }

        private string GetBattleTag(Player opponentPlayer)
        {   
            try
            {   
                // 使用Triton框架的标准方法获取BattleTag
                if (opponentPlayer.IsBnetPlayer())
                {   
                    var bnetPlayer = opponentPlayer.GetBnetPlayer();
                    if (bnetPlayer != null)
                    {   
                        var battleTagObj = bnetPlayer.GetBattleTag();
                        if (battleTagObj != null)
                        {   
                            var tagName = battleTagObj.GetName();
                            var tagNumber = battleTagObj.GetNumber();
                            if (!string.IsNullOrEmpty(tagName) && !string.IsNullOrEmpty(tagNumber))
                            {   
                                return string.Format("{0}#{1}", tagName, tagNumber);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {   
                Logger.GetLoggerInstanceForType().DebugFormat("获取BattleTag时出错: {0}", ex.Message);
            }

            return string.Empty;
        }

        private string GetHeroName(Player opponentPlayer)
        {   
            try
            {   
                Entity hero = opponentPlayer.GetHero();
                return hero != null ? hero.GetName() : "未知英雄";
            }
            catch (Exception ex)
            {   
                Logger.GetLoggerInstanceForType().DebugFormat("获取英雄名称时出错: {0}", ex.Message);
                return "未知英雄";
            }
        }

        public void ShowOpponentInfoOnce()
        {   
            if (!hasShownOpponentInfo)
            {   
                string info = GetOpponentInfo();
                if (!string.IsNullOrEmpty(info))
                {   
                    Logger.GetLoggerInstanceForType().WarnFormat("{0}", info);
                    hasShownOpponentInfo = true;
                    retryCount = 0; // 重置重试计数
                }
                else if (retryCount < MAX_RETRY_COUNT)
                {   
                    retryCount++;
                    // 继续尝试获取信息
                }
                else
                {   
                    hasShownOpponentInfo = true; // 达到最大重试次数，停止重试
                }
            }
        }

        public void Reset()
        {   
            hasShownOpponentInfo = false;
            retryCount = 0;
        }
    }
}