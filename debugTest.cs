using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using log4net;
using Logger = Triton.Common.LogUtilities.Logger;

using Screen = Triton.Game.Mapping.Screen;
using Triton.Bot;
using Triton.Common;
using Triton.Game;
using Triton.Game.Mapping;
using Triton.Game.Data;
using Buddy.Coroutines;

public class RuntimeCode
{
    private static readonly ILog Log = Logger.GetLoggerInstanceForType();

    public void Execute()
    {
        // CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.DINO_434);
        // Log.DebugFormat(card.ToString());
        debugTest ts = new debugTest();
        // ts.choieCard();
        // ts.DialogManage();
        // ts.PopupDisplayManag();
        ts.GetEntityMap();
        // ts.judgeResponseMode();
        // ts.getId();
        // using (TritonHs.AcquireFrame())
        // {
        // TAG_STEP tAG_STEP = (TAG_STEP)gameEntity.GetTag(GAME_TAG.STEP);
        // Log.DebugFormat(tAG_STEP.ToString());
        // Log.DebugFormat(TritonHs.IsInTargetMode().ToString());
        // Log.DebugFormat(gameEntity.GetTag(GAME_TAG.PLAYSTATE).ToString())
        // }


    }


    public class debugTest
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private GameState GameState = GameState.Get();
        public void tagMap()
        {
            // Dictionary<int, Entity> dic = GameState.GetEntityMap();
            // foreach (var item in dic)
            // {
            //     Log.DebugFormat("实体  {0} ", item.Value.ToString());
            //     foreach (var tag in item.Value.GetTags().GetMap())
            //     {
            //         Log.WarnFormat("{0} {1} {2} ",(GAME_TAG)tag.Key,tag.Key, tag.Value);
            //     }
            // }

            var dic = GameState.GetEntityMap().Values;
            foreach (var item in dic)
            {
                Log.DebugFormat("实体  {0} ", item.ToString());
                foreach (var tag in item.GetTags().GetMap())
                {
                    Log.WarnFormat("{0} {1} {2} ", (GAME_TAG)tag.Key, tag.Key, tag.Value);
                }
            }
        }
        public void GetEntityMap()
        {
            // Dictionary<int, Entity> dic = GameState.GetEntityMap();
            // foreach (var item in dic)
            // {
            //     Log.DebugFormat("实体  {0} ", item.Value.ToString());
            // }
            Entity entity = GameState.GetEntity(107);
            foreach (var tag in entity.GetTags().GetMap())
            {
                Log.WarnFormat("{0} {1} {2} ", (GAME_TAG)tag.Key, tag.Key, tag.Value);
            }
        }
        // public void PopupDisplayManag()
        // {
        //     PopupDisplayManager popupDisplayManager = PopupDisplayManager.Get();
        //     if (popupDisplayManager != null)
        //     {
        //         popupDisplayManager.
        //         foreach (var reward in popupDisplayManager.m_genericRewards)
        //         {
        //             Log.InfoFormat("实体  {0} ", reward.Transform.Position);

        //         }


        //     }
        // }
        public void exterAttacks()
        {
            // GameState gameEntity = GameState.Get();
            // var entity = gameEntity.GetEntity(106);
            // Log.DebugFormat("枯竭的 EXHAUSTED 43：" + entity.GetTag(GAME_TAG.EXHAUSTED).ToString());
            // Log.DebugFormat("枯竭的 TAG_SCRIPT_DATA_NUM_1 43：" + entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString());
            // Log.DebugFormat("枯竭的 TAG_SCRIPT_DATA_NUM_2 43：" + entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString());

            /*   GameState gameEntity = GameState.Get();
              var entity = gameEntity.GetEntity(14);
              Log.DebugFormat(entity.ToString());
              Log.DebugFormat("这回合攻击次数 NUM_ATTACKS_THIS_TURN 297：" + entity.GetTag(297).ToString());
              Log.DebugFormat("这回合额外攻击的次数 EXTRA_ATTACKS_THIS_TURN 444：" + entity.GetTag(444).ToString()); */
            /*             var HERP = gameEntity.GetEntity(68);
                        Log.DebugFormat(HERP.ToString());
                        Log.DebugFormat("这回合攻击次数 NUM_ATTACKS_THIS_TURN 297：" + HERP.GetTag(297).ToString());
                        Log.DebugFormat("这回合额外攻击的次数 EXTRA_ATTACKS_THIS_TURN 444：" + HERP.GetTag(444).ToString());
                        Log.DebugFormat("本回合打出的卡牌数量 NUM_CARDS_PLAYED_THIS_TURN 269：" + HERP.GetTag(269).ToString());
                        Log.DebugFormat("本场打出的法术数量 NUM_SPELLS_PLAYED_THIS_GAME 1780：" + HERP.GetTag(1780).ToString());
                        Log.DebugFormat("本场英雄力量伤害数量 NUM_HERO_POWER_DAMAGE_THIS_GAME 1025：" + HERP.GetTag(1025).ToString()); */

        }
        public void 回溯()
        {
            RewindUIManager rewindUIManager = RewindUIManager.Get();

            // Log.DebugFormat("回溯{0}", rewindUIManager.m_rewindButton.Transform.Position + rewindUIManager.m_rewindButton.m_ClickDownOffset);
            // Log.DebugFormat("保持{0}", rewindUIManager.m_keepButton.Transform.Position + rewindUIManager.m_keepButton.m_ClickDownOffset);
            Log.DebugFormat("回溯{0}", rewindUIManager.m_rewindButton.m_RootObject.Transform.Position);
            Log.DebugFormat("保持{0}", rewindUIManager.m_keepButton.m_RootObject.Transform.Position);
            // 回溯-12.57256,5.995536,-12.70758
            // 保持-12.61,5.81,-12.76
            // 回溯-12.57256,5.995536,-12.70758
            // 保持-12.61,5.81,-12.76
        }
        /// <summary>
        /// 读取选择的卡牌
        /// </summary>
        public void choieCard()
        {
            var choiceCardMgr = ChoiceCardMgr.Get();
            var cards = choiceCardMgr.GetFriendlyCards();
            foreach (var card in cards)
            {
                Entity entity = card.GetEntity();
                string nameCN = card.GetName();
                string cardId = card.GetCardId();
                int entitiyID = card.GetEntityId();
                Zone tAG_ZONE = card.GetZone();

                Log.InfoFormat("实体id {2}  {0} {1} 位置:{3} ", nameCN, cardId, entitiyID, tAG_ZONE);
                var allTags = Enum.GetValues(typeof(GAME_TAG));

                foreach (GAME_TAG tag in allTags)
                {
                    int tagValue = entity.GetTag(tag);
                    if (tagValue != 0)
                        Log.WarnFormat("{0} (ID:{1}) 标签 {2} = {3} 实体id {4}", nameCN, cardId, tag.ToString(), tagValue, entitiyID);
                }

            }

        }
        public void decks()
        {

            Log.InfoFormat("{0}", RewindUIManager.m_isShowingRewindUI);
            // List<Card> cards = GameState.GetFriendlySidePlayer().GetGraveyardZone().GetCards();
            // foreach (var card in cards)
            // {
            //     Log.InfoFormat(card.GetPrevZone().ToString());
            // }
        }
        public void Zones()
        {
            ZoneMgr zoneMgr = ZoneMgr.Get();


            foreach (var zone in zoneMgr.GetZones())
            {
                if (zone.m_layoutDirty)
                {
                    switch (zone.m_ServerTag)
                    {
                        case TAG_ZONE.HAND:
                        case TAG_ZONE.DECK:
                        case TAG_ZONE.GRAVEYARD:
                        case TAG_ZONE.PLAY:
                        case TAG_ZONE.SECRET:
                            {

                                Log.DebugFormat("区域变化{0} 玩家{1}", zone.m_ServerTag.ToString(), zone.m_controller.ToString());
                            }
                            break;

                    }
                    // Log.DebugFormat("{0}", a.ToString());

                }
            }

            // }
            // Log.DebugFormat("{0}", RewindUIManager.IsShowingRewindUI);
            // Log.DebugFormat("{0}", GameState.Get().m_responseMode);

            // ZoneMgr zoneMgr = ZoneMgr.Get();
            // if (zoneMgr != null)
            // {
            //     List<ZoneChangeList> changeLists = zoneMgr.m_activeLocalChangeLists;
            //     foreach(var a in changeLists)
            //     {
            //         Log.DebugFormat("{0}",a.ToString());
            //     }
            // }
            // Player FriendlyPlayer = GameState.GetFriendlySidePlayer();
            // Player OpposingPlayer = GameState.GetOpposingSidePlayer();
            // update(FriendlyPlayer, FriendlyPlayer.GetControllerId());
            // update(OpposingPlayer, OpposingPlayer.GetControllerId());
            // foreach (var zone in ZoneMgr.Get().GetZones())
            // {
            //     Log.DebugFormat("{0}", zone.ToString());

            // }
            // Log.DebugFormat("{0}", FriendlyPlayer.GetControllerId());
            // Log.DebugFormat("{0}", FriendlyPlayer.GetHero().ToString());
            // Log.DebugFormat("{0}", FriendlyPlayer.GetHero().IsControlledByFriendlySidePlayer());
            // Log.DebugFormat("{0}", OpposingPlayer.GetControllerId());
            // Log.DebugFormat("{0}", OpposingPlayer.GetHero().ToString());
            // Log.DebugFormat("{0}", OpposingPlayer.GetHero().IsControlledByFriendlySidePlayer());
            // Log.DebugFormat("{0}", GameState.Get().GetFriendlyPlayerId());
            // Log.DebugFormat("{0}", GameState.Get().GetOpposingPlayerId());



        }
        public void GetEntity()
        {
            Entity entity = GameState.GetFriendlySidePlayer().GetHero();
            var creator = entity.GetTag(GAME_TAG.CREATOR);
            // var cpyDeath = entity.GetTag(GAME_TAG.COPY_DEATHRATTLE);
            var ctrlId = entity.GetTag(GAME_TAG.CONTROLLER);

            Log.DebugFormat(entity.ToString());
            Log.DebugFormat(creator.ToString());
            // Log.DebugFormat(cpyDeath.ToString());
            Log.DebugFormat(ctrlId.ToString());

        }
        public void getId()
        {
            using (TritonHs.AcquireFrame())
            {
                BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();

                //仅获取名字
                Player Friendlyplayer = GameState.GetFriendlySidePlayer();
                Player Oppoplayer = GameState.GetOpposingSidePlayer();
                BnetPlayer OppobnetPlayer = Oppoplayer.GetBnetPlayer();
                BnetPlayer Friendlynetplayer = Friendlyplayer.GetBnetPlayer();
                // Log.DebugFormat("{0}",Oppoplayer.GetName());
                // Log.DebugFormat("{0}", player.m_heroPower.GetName());

                Log.DebugFormat("{0}", Friendlynetplayer.GetAccount().GetFullName());
                Log.DebugFormat("{0}", Friendlynetplayer.GetBattleTag().GetNumber());


            }
        }

        public void DialogManage()
        {
            RewardBoxesDisplay rewardBoxesDisplay = RewardBoxesDisplay.Get();
            rewardBoxesDisplay.m_RewardObjects.ForEach(
                (o) =>
                {

                    Log.InfoFormat("{0}", o.Transform.Position);
                    Client.LeftClickAt(o.Transform.Position);

                });
            // List<RewardBoxesDisplay.BoxRewardData> boxRewardData = rewardBoxesDisplay.m_RewardSets.m_RewardData;

            /*             for (int i = 0; i < boxRewardData.Count; i++)
                        {
                            foreach (var a in boxRewardData[i].m_PackageData)
                            {
                                Log.InfoFormat("{0}", boxRewardData[i].m_PackageData.m_StartBone);

                            }
                            // rewardBoxesDisplay.OpenReward(i, rewardBoxesDisplay.m_RewardPackages[i].m_StartBone.Transform.Position);

                        } */
            // foreach (RewardBoxesDisplay.RewardPackageData pack in rewardBoxesDisplay.m_RewardPackages)
            // {

            //     Log.InfoFormat("{0}", pack.m_TargetBone.Transform.Position);
            //     rewardBoxesDisplay.OpenReward()
            //     Client.LeftClickAt(pack.m_TargetBone.Transform.Position);


            // }
            /* DialogManager dialogManager = DialogManager.Get();
            if (dialogManager != null)
            {
                DialogBase currentDialog = dialogManager.m_currentDialog;
                if (currentDialog != null)
                {
                    string realClassName = currentDialog.RealClassName;
                    Log.InfoFormat("[HandleDialogs] A dialog of type {0} is showing.", realClassName);
                    SeasonEndDialog seasonEndDialog = new SeasonEndDialog(currentDialog.Address);

                    Log.DebugFormat("{0}", seasonEndDialog.m_chestOpened);
                    UIBButton okayButton3 = seasonEndDialog.m_okayButton;

                        if (okayButton3 != null)
                        {
                            okayButton3.TriggerPress();
                            okayButton3.TriggerRelease();
                        }
                    if (!seasonEndDialog.m_chestOpened)
                    {


                        List<PegUIElement> m_rewardChests = seasonEndDialog.m_rewardChests;
                        if (m_rewardChests != null)
                        {
                            Log.InfoFormat("开始循环");
                            for (int i = 0; i < 1; i++)
                            {
                                if (m_rewardChests[i] != null)
                                {
                                    Log.InfoFormat("pui{0}", i);

                                    Client.LeftClickAtDialog(m_rewardChests[i].Transform.Position);

                                }
                            }
                        }

                    }
                    else
                    {

                        // Log.InfoFormat("pui 位置{0}", seasonEndDialog.m_rewardChestLegacy.Transform.Position);

                        Log.InfoFormat("开始打开小宝箱");
                        // Log.InfoFormat("pui 位置{0}", seasonEndDialog.m_rewardBoxesBone.Transform.Position);
                        UIBButton okayButton3 = seasonEndDialog.m_okayButton;

                        if (okayButton3 != null)
                        {
                            okayButton3.TriggerPress();
                            okayButton3.TriggerRelease();
                        }
                        // foreach (var a in seasonEndDialog.m_seasonEndInfo.m_rankedRewards)
                        // {
                        //     Log.InfoFormat("pui {0}", a.m_type);

                        // }

                        // List<PegUIElement> m_rewardChests = seasonEndDialog.m_rewardChests;
                        // foreach (PegUIElement pegUIElement in m_rewardChests)
                        // {
                        //     Log.InfoFormat("pui 位置{0}", pegUIElement.Transform.Position);

                        // }
                        // for (int i = 0; i < m_rewardChests.Count; i++)
                        // {
                        //     if (m_rewardChests[i] != null)
                        //     {
                        //         Log.InfoFormat("pui{0} 位置{1}", i, m_rewardChests[i].Transform.Position);
                        //         // Log.InfoFormat(" {0}", m_rewardChests[i].m_collider.Bounds.m_Center);
                        //         // var center = collider.Bounds.m_Center;
                        //         // m_rewardChests[i].TriggerPress();
                        //     }
                        // }
                    }

                }
            } */
            /* PegUIElement rewardChestLegacy = seasonEndDialog.m_rewardChestLegacy;
            if (rewardChestLegacy == null)
            {
                return;
            }
            else
            {
                Log.InfoFormat("[HandleDialogs] Now clicking on the \"rewardChestLegacy\" button.");
                rewardChestLegacy.TriggerPress();
                rewardChestLegacy.TriggerRelease();
            } */

            // // List<PegUIElement> a = seasonEndDialog.m_rewardChests;
            // Log.DebugFormat("{0}", seasonEndDialog.m_header.Text);
            // Log.DebugFormat("{0}", seasonEndDialog.m_rankAchieved.Text);
            // Log.DebugFormat("{0}", seasonEndDialog.m_rankName.Text);

            // var bounds = rewardChestLegacy.m_collider.Bounds;
            // var center = bounds.m_Center;
            // var screenPoint = Camera.Main.WorldToScreenPoint(center);

            // Log.InfoFormat(" {0}", rewardChestLegacy.m_originalLocalPosition.ToString());    

            // var center = bounds.m_Center;
            // Log.InfoFormat(" {0}", Camera.Main.WorldToScreenPoint(center));
            // UberText local = seasonEndDialog.m_rewardChestHeader;
        }
        /// <summary>
        /// 判断游戏响应模式
        /// </summary>
        public void judgeResponseMode()
        {
            GameState.ResponseMode responseMode = GameState.GetResponseMode();

            // Log.DebugFormat("{0}", responseMode);

            switch (responseMode)
            {
                case GameState.ResponseMode.OPTION: Log.DebugFormat("选项：{0}", responseMode); break;
                case GameState.ResponseMode.SUB_OPTION: Log.DebugFormat("子选项：{0}", responseMode); break;
                case GameState.ResponseMode.OPTION_TARGET: Log.DebugFormat("选项目标：{0}", responseMode); break;
                case GameState.ResponseMode.OPTION_REVERSE_TARGET: Log.DebugFormat("选项目标：{0}", responseMode); break;
                case GameState.ResponseMode.CHOICE: Log.DebugFormat("选择：{0}", responseMode); break;
                default: Log.DebugFormat("{0}", responseMode); break;
            }
            // Log.DebugFormat("实体有目标{0}", GameState.HasResponse(GameState.GetEntity(21)));
            // Log.DebugFormat("实体有目标{0}", GameState.GetLastOptions());
            // foreach (Entity entity in GameState.GetChosenEntities())
            // {
            //     Log.DebugFormat("{0}", entity.ToString());
            // }

            // Log.DebugFormat("11{0}", GameState.GetEntity(21).GetTag(GAME_TAG.CARDTYPE));
            // TargetOption


            Log.DebugFormat("{0}", GameState.GetEntity(21).CanBeTargetedByOpponents());
            // Log.DebugFormat("{0}", GameState.IsInChoiceMode());


        }
        public void traverse()
        {
            Player FriendlyPlayer = GameState.GetFriendlySidePlayer();
            Player OpposingPlayer = GameState.GetOpposingSidePlayer();

            GameState.GetFriendlySidePlayer().GetHandZone().GetCards();
            GameState.GetFriendlySidePlayer().GetGraveyardZone().GetCards();
            foreach (Card c in GameState.GetFriendlySidePlayer().GetHandZone().GetCards())
            {
                Log.InfoFormat("{0} ", c.GetEntity().ToString());
            }
            foreach (Card c in GameState.GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
            {
                Log.InfoFormat("{0} ", c.GetEntity().ToString());
            }
            // Log.InfoFormat("{0} ", GameState.GetFriendlySidePlayer().GetBattlefieldZone());
            // Log.InfoFormat("{0} ", GameState.GetFriendlySidePlayer().GetPlayerId());


        }
        public void updateEntity(Player player)
        {
            player.GetHeroCard();
            player.GetHeroPowerCard();
            player.GetWeaponCard();
            player.GetBattlefieldZone().GetCards();
            player.GetHandZone().GetCards();
            player.GetSecretZone();
            player.GetDeckZone().GetCards();
            player.GetGraveyardZone().GetCards();
        }
        /// <summary>
        /// 遍历实体
        /// </summary>
        public void traverseEntity()
        {

            // Log.DebugFormat(GameState.Get().GetMaxSecretZoneSizePerPlayer().ToString());
            // Log.DebugFormat(GameState.Get().GetMaxSecretsPerPlayer().ToString());
            // Log.DebugFormat(Board.Get().FindCollider("ShowingStarshipUI").ToString());
            // Collider collider = Board.Get().FindCollider("ShowingStarshipUI");
            // var FriendlySidePlayer = gameState.GetFriendlySidePlayer().GetBattlefieldZone();
            List<TAG_ZONE> tAG_ZONEs = new List<TAG_ZONE>() { TAG_ZONE.PLAY };
            // foreach (var item in tAG_ZONEs)
            // {
            //     Log.InfoFormat("{0}  ", item);

            // }
            //Log.InfoFormat("{0}  ", tAG_ZONEs.Contains(TAG_ZONE.SECRET));

            for (int i = 0; i < 300; i++)
            {
                Entity entity = GameState.GetEntity(i);
                if (entity != null)
                {

                    string nameCN = entity.GetName();
                    string cardId = entity.GetCardId();
                    int entitiyID = entity.GetEntityId();
                    TAG_ZONE tAG_ZONE = entity.GetZone();
                    TAG_CARDTYPE tAG_CARDTYPE = entity.GetCardType();
                    // if (cardId == "WORK_042") {
                    //     Log.InfoFormat("{0} ", entity.GetTag(2));               
                    // } else
                    // {
                    //     continue;
                    // }
                    if (tAG_ZONEs.Contains(tAG_ZONE))
                    {
                        Log.InfoFormat("实体id {2}  {0} {1} 位置:{3} ", nameCN, cardId, entitiyID, tAG_ZONE);
                        /*
                        foreach (GAME_TAG tag in allTags)
                                    {
                                        int tagValue = entity.GetTag(tag);
                                        if (tagValue != 0)
                                            Log.InfoFormat("{0} {1} tag:{5} {2} = {3} ", nameCN, cardId, tag.ToString(), tagValue, entitiyID,(int)tag);
                                    } */

                        var allTags = Enum.GetValues(typeof(GAME_TAG));

                        switch (tAG_CARDTYPE)
                        {

                            case TAG_CARDTYPE.MINION:
                                // case TAG_CARDTYPE.SPELL:
                                // case TAG_CARDTYPE.LOCATION:
                                // case TAG_CARDTYPE.HERO:
                                {
                                    // Log.InfoFormat("{0} (ID:{1})  实体id {2} 是否睡着了{3}", nameCN, cardId, entitiyID, entity.IsAsleep());
                                    // Log.InfoFormat("{0} (ID:{1})  是否有复制亡语{2}", nameCN, cardId, entity.GetTag(GAME_TAG.DEATHRATTLE));
                                    // Log.InfoFormat("{0} (ID:{1})  选择角色类型{2}", nameCN, cardId, entity.GetTag(1692));
                                    // Log.InfoFormat("{0} (ID:{1})  使用条件{2}", nameCN, cardId, GameState.GetErrorType(entity));
                                    /* if (entity.m_entityRaces != null)
                                    {

                                        foreach (TAG_RACE a in entity.m_entityRaces)
                                        {
                                            Log.InfoFormat("{0} ", a);

                                        }
                                    } */
                                    foreach (GAME_TAG tag in allTags)
                                    {
                                        int tagValue = entity.GetTag(tag);
                                        if (tagValue != 0)
                                            Log.InfoFormat("{0} {1} tag:{4} {2} = {3} ", nameCN, cardId, tag.ToString(), tagValue, (int)tag);
                                    }
                                    var eachs = entity.GetEnchantments();
                                    foreach (Entity each in eachs)
                                    {
                                        Log.WarnFormat(each.ToString());
                                    }
                                    continue;
                                }
                            case TAG_CARDTYPE.ENCHANTMENT:
                                {
                                    foreach (GAME_TAG tag in allTags)
                                    {
                                        int tagValue = entity.GetTag(tag);
                                        if (tagValue != 0)
                                            Log.WarnFormat("附魔 实体id {4} {0} {1} 标签 {2} = {3} ", nameCN, cardId, tag.ToString(), tagValue, entitiyID);
                                    }
                                    continue;
                                }
                            default: continue;

                        }

                    }
                    else
                    {
                        continue;
                    }
                    // Log.WarnFormat("{0} (ID:{1})  实体id {2} ERROR_TYPE {3}", nameCN, cardId, entitiyID, gameState.GetErrorType(entity));
                }

            }



        }

        /// <summary>
        /// 测试投降
        /// </summary>
        public void surrender()
        {
            using (TritonHs.AcquireFrame())
            {
                Entity entity = GameState.GetEntity(2);
                Entity entity2 = GameState.GetEntity(3);
                String enemyName = ("xiaohei" == entity.ToString() || "HelloWorld" == entity.ToString()) ? entity2.ToString() : entity.ToString();
                String pattern = ".{2,4}[之,的].{1,4}";
                Log.DebugFormat("对手的id为： " + enemyName);
                bool flag = Regex.IsMatch(enemyName, pattern);
                Log.DebugFormat(flag ? "是人机格式的id" : "不是人机格式的id");
                if (!flag)
                {
                    Log.DebugFormat("准备投降");
                    TritonHs.Concede(true);
                }
            }
        }
        /// <summary>
        /// 奇利亚斯3000测试
        /// </summary>
        public void ZILLIAXTest()
        {
            using (TritonHs.AcquireFrame())
            {
                for (int i = 0; i < 200; i++)
                {
                    Entity entity = GameState.GetEntity(i);

                    if (GameState.Get().GetEntity(i) != null)
                    {

                        // if (entity.GetName() == "奇利亚斯豪华版3000型")
                        if (entity.GetName() == "恐惧小道")
                        {
                            String nameCN = entity.GetName();
                            String cardId = entity.GetCardId();
                            int entitiyID = entity.GetEntityId();
                            var allTags = Enum.GetValues(typeof(GAME_TAG));

                            foreach (GAME_TAG tag in allTags)
                            {
                                int tagValue = entity.GetTag(tag);
                                if (tagValue != 0)
                                    Log.WarnFormat("{0} (ID:{1}) 标签 {2} = {3} 实体id {4}", nameCN, cardId, tag.ToString(), tagValue, entitiyID);
                            }
                        }
                    }
                }


            }
        }
        /// <summary>
        /// 星舰测试
        /// </summary>
        public void starship()
        {
            for (int i = 0; i < 150; i++)
            {
                Entity entity = GameState.GetEntity(i);
                if (entity != null)
                {
                    if (entity.GetTag(3555) == 1)
                    {
                        Log.DebugFormat(entity.GetTag(3555).ToString());

                        foreach (Entity StarshipPiece in entity.GetEnchantments())
                        {
                            Log.DebugFormat(StarshipPiece.ToString());
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 泰坦测试
        /// </summary>
        public void Titan()
        {
            GameState gameState = GameState.Get();
            for (int i = 0; i < 200; i++)
            {
                if (gameState.GetEntity(i) != null)
                {
                    Entity entity = GameState.Get().GetEntity(i);
                    string name = entity.GetCardId();
                    if (name == "TTN_903" || name == "YOG_516")
                    {
                        Log.DebugFormat("\t"); Log.DebugFormat("\t");
                        Log.DebugFormat(entity.ToString());
                        Log.DebugFormat("这回合攻击次数 NUM_ATTACKS_THIS_TURN 297：" + entity.GetTag(297).ToString());

                    }
                }
            }
        }
        /// <summary>
        /// 邪能地窖
        /// </summary>
        public void UnderfelRift()
        {
            using (TritonHs.AcquireFrame())
            {
                // CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.TLC_446t1);
                GameState gameState = GameState.Get();
                Entity a = gameState.GetEntity(2);

                // foreach (var item in a.GetEnchantments())
                // {
                //     Log.DebugFormat(item.ToString());

                // }
                for (int i = 0; i < 200; i++)
                {
                    if (gameState.GetEntity(i) != null)
                    {
                        Entity entity = GameState.Get().GetEntity(i);


                        // Log.DebugFormat(GameState.Get().GetEntity(i).ToString());
                        if (entity.GetCardId() == "TLC_446t1")
                        {
                            Log.DebugFormat("\t");
                            Log.DebugFormat("\t");
                            Log.DebugFormat("\t");

                            Log.DebugFormat(entity.ToString());
                            Log.DebugFormat("这回合攻击次数 NUM_ATTACKS_THIS_TURN 297：" + entity.GetTag(297).ToString());
                            Log.DebugFormat("tag2 TAG_SCRIPT_DATA_NUM_1 2：" + entity.GetTag(2).ToString());
                            Log.DebugFormat("可交互对象消耗 INTERACTABLE_OBJECT_COST 4090：" + entity.GetTag(4090).ToString());
                            Log.DebugFormat("可目标手牌 CAN_TARGET_CARDS_IN_HAND 1508：" + entity.GetTag(1508).ToString());
                            Log.DebugFormat("可交互对象 INTERACTABLE_OBJECT 4089：" + entity.GetTag(4089).ToString());
                            Log.DebugFormat("使用次数 USES_CHARGES 4257：" + entity.GetTag(4257).ToString());
                            foreach (var item in entity.GetEnchantments())
                            {
                                Log.DebugFormat(item.ToString());

                            }


                        }

                        /* if (entity.GetCardId() == "TLC_446t2")
                        {
                                Log.DebugFormat(entity.ToString());
                            Log.DebugFormat("这回合攻击次数 NUM_ATTACKS_THIS_TURN 297：" + entity.GetTag(297).ToString());
                            foreach (var item in entity.GetEnchantments())
                            {
                                Log.DebugFormat(item.ToString());

                            }

                        } */

                    }
                }
            }
        }

        public void Lcoationa()
        {
            using (TritonHs.AcquireFrame())
            {
                for (int i = 0; i < 111; i++)
                {
                    if (GameState.Get().GetEntity(i) != null)
                    {
                        Entity entity = GameState.Get().GetEntity(i);
                        // Log.DebugFormat(entity.ToString());

                        // Log.DebugFormat(GameState.Get().GetEntity(i).ToString());
                        if (entity.GetName() == "安戈洛丛林")
                        {
                            Log.DebugFormat("\t");
                            Log.DebugFormat(entity.ToString());
                            Log.DebugFormat(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString());
                            Log.DebugFormat(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString());

                            foreach (var item in entity.GetEnchantments())
                            {
                                Log.DebugFormat(item.ToString());

                            }
                            // Log.DebugFormat(entity.GetTag(4161).ToString());
                            // Log.DebugFormat(entity.GetTag(4162).ToString());
                            // Log.DebugFormat(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString());
                            // Log.DebugFormat(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString());
                            // Log.DebugFormat(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString());


                        }

                    }
                }
            }



        }

        public async Task use()
        {

            // ZonePlay battlefieldZone = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone();
            ZonePlay battlefieldZone1 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();

            // int count = battlefieldZone.m_cards.Count;
            // Log.DebugFormat(count.ToString());
            // Log.DebugFormat(battlefieldZone1.m_controller.ToString());

            ZonePlay battlefieldZone = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone();
            Vector3 cardPosition = battlefieldZone1.GetCardPosition(1);
            int count1 = battlefieldZone1.m_cards.Count;
            Client.MouseOver(cardPosition);
            Log.DebugFormat(cardPosition.ToString());
            Log.DebugFormat(count1.ToString());

            await Client.MoveCursorHumanLike(cardPosition);
            Client.LeftClickAt(cardPosition);
            /* ZonePlay battlefieldZone = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone();
            int count = battlefieldZone.m_cards.Count;
            if (count == 0)
            {
                Vector3 cardPosition = battlefieldZone.GetCardPosition(0);
                await Client.MoveCursorHumanLike(cardPosition);
                Client.LeftClickAt(cardPosition);
            }
            else if (slot > count)
            {
                Vector3 cardPosition = battlefieldZone.GetCardPosition(count - 1);
                cardPosition.X += battlefieldZone.m_slotWidth / 2f;
                await Client.MoveCursorHumanLike(cardPosition);
                await Coroutine.Sleep(250);
                Client.LeftClickAt(cardPosition);
            }
            else
            {
                Vector3 cardPosition = battlefieldZone.GetCardPosition(slot - 1);
                cardPosition.X -= battlefieldZone.m_slotWidth / 2f;
                await Client.MoveCursorHumanLike(cardPosition);
                await Coroutine.Sleep(250);
                Client.LeftClickAt(cardPosition);
            } */

            /*    using ns26;
           using ns27;
           using System;
           using System.Collections.Generic;
           using Triton.Game.Mono;

           namespace Triton.Game.Mapping
           {
               [Attribute38("StarshipHUDManager")]
               public class StarshipHUDManager : MonoBehaviour
               {
                   public StarshipHUDManager(IntPtr address, string className)
               : base(address, className)
                   {
                   }
                   public StarshipHUDManager(IntPtr address) : this(address, "StarshipHUDManager")
                   {
                   }
                   public static StarshipHUDManager Get()
                   {
                       return MonoClass.smethod_15<StarshipHUDManager>(TritonHs.MainAssemblyPath, "", "StarshipHUDManager", "Get", Array.Empty<object>());
                   }
                   public PlayButton m_launchButton => method_3<PlayButton>("m_launchButton");

                   public PlayButton m_abortLaunchButton => method_3<PlayButton>("m_abortLaunchButton");
               }
           } */

            /* public static async Task LaunchStarship()
                   {
                       Vector3 center = StarshipHUDManager.Get().m_launchButton.Transform.Position;
                       await Client.MoveCursorHumanLike(center);
                       Client.LeftClickAt(center);
                   }
                   public static async Task AbortLaunchStarship()
                   {
                       Vector3 center = StarshipHUDManager.Get().m_abortLaunchButton.Transform.Position;
                       await Client.MoveCursorHumanLike(center);
                       Client.LeftClickAt(center);
                   } */
        }




    }

}