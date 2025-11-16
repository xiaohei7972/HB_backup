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
        // ts.traverseEntity();
        ts.traverseEntity();
        // ts.Toreth();
        // using (TritonHs.AcquireFrame())
        // {
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
        // TAG_STEP tAG_STEP = (TAG_STEP)gameEntity.GetTag(GAME_TAG.STEP);
        // Log.DebugFormat(tAG_STEP.ToString());
        // Log.DebugFormat(TritonHs.IsInTargetMode().ToString());
        // Log.DebugFormat(gameEntity.GetTag(GAME_TAG.PLAYSTATE).ToString())
        // }


    }


    public class debugTest
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        public void GetEntity()
        {
            Entity entity = GameState.Get().GetFriendlySidePlayer().GetHero();
            var creator = entity.GetTag(GAME_TAG.CREATOR);
            // var cpyDeath = entity.GetTag(GAME_TAG.COPY_DEATHRATTLE);
            var ctrlId = entity.GetTag(GAME_TAG.CONTROLLER);

            Log.DebugFormat(entity.ToString());
            Log.DebugFormat(creator.ToString());
            // Log.DebugFormat(cpyDeath.ToString());
            Log.DebugFormat(ctrlId.ToString());

        }
        /// <summary>
        /// 遍历实体
        /// </summary>
        public void traverseEntity()
        {

            using (TritonHs.AcquireFrame())
            {

                // Log.DebugFormat(Board.Get().FindCollider("ShowingStarshipUI").ToString());
                // Collider collider = Board.Get().FindCollider("ShowingStarshipUI");
                GameState gameState = GameState.Get();
                // var FriendlySidePlayer = gameState.GetFriendlySidePlayer().GetBattlefieldZone();
                for (int i = 0; i < 150; i++)
                {
                    Entity entity = gameState.GetEntity(i);
                    if (entity != null)
                    {
                        TagMap tagMap = entity.GetTags();
                        String nameCN = entity.GetName();
                        String cardId = entity.GetCardId();
                        int entitiyID = entity.GetEntityId();
                        Log.WarnFormat("{2}  {0} {1} 位置:{3} ", nameCN, cardId, entitiyID,entity.GetZone());

                        /* if (entity.GetZone() == TAG_ZONE.PLAY)
                        {
                            Log.WarnFormat("{0} (ID:{1})  实体id {2} 是否睡着了{3}", nameCN, cardId, entitiyID, entity.IsAsleep());
                            Log.WarnFormat("{0} (ID:{1})  是否有制亡语{2}", nameCN, cardId, entity.GetTag(GAME_TAG.DEATHRATTLE));
                            var allTags = Enum.GetValues(typeof(GAME_TAG));
                            foreach (GAME_TAG tag in allTags)
                            {
                                int tagValue = entity.GetTag(tag);
                                if (tagValue != 0)
                                    Log.WarnFormat("{0} (ID:{1}) 标签 {2} = {3} 实体id {4}", nameCN, cardId, tag.ToString(), tagValue, entitiyID);
                            }
                        } */
                        // Log.WarnFormat("{0} (ID:{1})  实体id {2} ERROR_TYPE {3}", nameCN, cardId, entitiyID, gameState.GetErrorType(entity));
                    }

                }
            }
            // Log.DebugFormat(GameState.Get().GetMaxSecretZoneSizePerPlayer().ToString());
            // Log.DebugFormat(GameState.Get().GetMaxSecretsPerPlayer().ToString());

        }

        /// <summary>
        /// 测试投降
        /// </summary>
        public void surrender()
        {
            using (TritonHs.AcquireFrame())
            {
                Entity entity = GameState.Get().GetEntity(2);
                Entity entity2 = GameState.Get().GetEntity(3);
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
                GameState gameState = GameState.Get();
                for (int i = 0; i < 200; i++)
                {
                    Entity entity = gameState.GetEntity(i);

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
            for (int i = 0; i < 1000; i++)
            {
                if (GameState.Get().GetEntity(i) != null)
                {
                    Entity entity = GameState.Get().GetEntity(i);
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