namespace HREngine.Bots
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 墓地物品结构体，用于记录卡牌在墓地中的状态
    /// </summary>
    public struct GraveYardItem
    {
        /// <summary>
        /// 是否为己方卡牌
        /// </summary>
        public bool own;
        /// <summary>
        /// 实体ID，唯一标识游戏中的每个实体
        /// </summary>
        public int entityId;
        /// <summary>
        /// 卡牌ID，对应游戏中的具体卡牌
        /// </summary>
        public CardDB.cardIDEnum cardid;
        /// <summary>
        /// 卡牌在墓地中的状态
        /// </summary>
        public EnumGraveyardStatus status;

        /// <summary>
        /// 构造函数，初始化墓地物品
        /// </summary>
        /// <param name="id">卡牌ID</param>
        /// <param name="entityId">实体ID</param>
        /// <param name="own">是否为己方</param>
        /// <param name="status">墓地状态</param>
        public GraveYardItem(CardDB.cardIDEnum id, int entityId, bool own, EnumGraveyardStatus status)
        {
            this.own = own;
            this.cardid = id;
            this.entityId = entityId;
            this.status = status;
        }

        /// <summary>
        /// 墓地状态枚举
        /// </summary>
        public enum EnumGraveyardStatus
        {
            /// <summary>未知状态</summary>
            Unknown,
            /// <summary>已死亡（随从）</summary>
            Died,
            /// <summary>已使用（法术或英雄技能）</summary>
            Used,
            /// <summary>已弃置</summary>
            Discard,
            /// <summary>已烧弃（超过手牌上限）</summary>
            Burnt
        }
    }

    /// <summary>
    /// 奥秘管理类，用于处理游戏中各种奥秘的状态和猜测
    /// 通过修改下面的true -> false，可以去掉一些要防的奥秘，防冰箱和aoe足够
    /// </summary>
    public class SecretItem
    {
        /// <summary>
        /// 奥秘是否已触发
        /// </summary>
        public bool triggered = false;

        /// <summary>
        /// 奥秘数据的位存储
        /// </summary>
        public BitArray data = new BitArray(60); // Todo: 如果奥秘数多了，要更新这个数
        
        /// <summary>
        /// 奥秘名称枚举，用于标识不同类型的奥秘
        /// </summary>
        public enum SecretName
        {
            beartrap = 0,       // 捕熊陷阱
            explosive = 1,      // 爆炸陷阱
            snaketrap = 2,      // 毒蛇陷阱
            missdirection = 3,  // 误导
            freezing = 4,       // 冰冻陷阱
            snipe = 5,          // 狙击
            darttrap = 6,       // 毒镖陷阱
            cattrick = 7,       // 豹子戏法

            counterspell = 8,   // 法术反制
            icebarrier = 9,     // 寒冰护体
            iceblock = 10,      // 寒冰屏障
            mirrorentity = 11,  // 镜像实体
            spellbender = 12,   // 扰咒术
            vaporize = 13,      // 蒸发
            duplicate = 14,     // 复制
            effigy = 15,        // 轮回
            flameward = 16,     // 火焰结界
            explosiverunes = 17, // 爆炸符文

            eyeforaneye = 18,   // 以眼还眼
            noblesacrifice = 19, // 崇高牺牲
            redemption = 20,    // 救赎
            repentance = 21,    // 忏悔
            avenge = 22,        // 复仇
            sacredtrial = 23,   // 神圣审判
        }
        public static SecretItem dataToSecretItem(BitArray ba)
        {
            SecretItem si = new SecretItem();
            if (ba.Get(Convert.ToInt32(SecretName.beartrap)))
                si.canBe_beartrap = true;
            else
                si.canBe_beartrap = false;
            if (ba.Get(Convert.ToInt32(SecretName.explosive)))
                si.canBe_explosive = true;
            else
                si.canBe_explosive = false;
            if (ba.Get(Convert.ToInt32(SecretName.snaketrap)))
                si.canBe_snaketrap = true;
            else
                si.canBe_snaketrap = false;
            if (ba.Get(Convert.ToInt32(SecretName.missdirection)))
                si.canBe_missdirection = true;
            else
                si.canBe_missdirection = false;
            if (ba.Get(Convert.ToInt32(SecretName.freezing)))
                si.canBe_freezing = true;
            else
                si.canBe_freezing = false;
            if (ba.Get(Convert.ToInt32(SecretName.snipe)))
                si.canBe_snipe = true;
            else
                si.canBe_snipe = false;
            if (ba.Get(Convert.ToInt32(SecretName.darttrap)))
                si.canBe_darttrap = true;
            else
                si.canBe_darttrap = false;
            if (ba.Get(Convert.ToInt32(SecretName.cattrick)))
                si.canBe_cattrick = true;
            else
                si.canBe_cattrick = false;

            if (ba.Get(Convert.ToInt32(SecretName.counterspell)))
                si.canBe_counterspell = true;
            else
                si.canBe_counterspell = false;
            if (ba.Get(Convert.ToInt32(SecretName.icebarrier)))
                si.canBe_icebarrier = true;
            else
                si.canBe_icebarrier = false;
            if (ba.Get(Convert.ToInt32(SecretName.iceblock)))
                si.canBe_iceblock = true;
            else
                si.canBe_iceblock = false;
            if (ba.Get(Convert.ToInt32(SecretName.mirrorentity)))
                si.canBe_mirrorentity = true;
            else
                si.canBe_mirrorentity = false;
            if (ba.Get(Convert.ToInt32(SecretName.spellbender)))
                si.canBe_spellbender = true;
            else
                si.canBe_spellbender = false;
            if (ba.Get(Convert.ToInt32(SecretName.vaporize)))
                si.canBe_vaporize = true;
            else
                si.canBe_vaporize = false;
            if (ba.Get(Convert.ToInt32(SecretName.duplicate)))
                si.canBe_duplicate = true;
            else
                si.canBe_duplicate = false;
            if (ba.Get(Convert.ToInt32(SecretName.effigy)))
                si.canBe_effigy = true;
            else
                si.canBe_effigy = false;
            if (ba.Get(Convert.ToInt32(SecretName.flameward)))
                si.canBe_flameward = true;
            else
                si.canBe_flameward = false;
            if (ba.Get(Convert.ToInt32(SecretName.explosiverunes)))
                si.canBe_explosiverunes = true;
            else
                si.canBe_explosiverunes = false;

            if (ba.Get(Convert.ToInt32(SecretName.eyeforaneye)))
                si.canBe_eyeforaneye = true;
            else
                si.canBe_eyeforaneye = false;
            if (ba.Get(Convert.ToInt32(SecretName.noblesacrifice)))
                si.canBe_noblesacrifice = true;
            else
                si.canBe_noblesacrifice = false;
            if (ba.Get(Convert.ToInt32(SecretName.redemption)))
                si.canBe_redemption = true;
            else
                si.canBe_redemption = false;
            if (ba.Get(Convert.ToInt32(SecretName.repentance)))
                si.canBe_repentance = true;
            else
                si.canBe_repentance = false;
            if (ba.Get(Convert.ToInt32(SecretName.avenge)))
                si.canBe_avenge = true;
            else
                si.canBe_avenge = false;
            if (ba.Get(Convert.ToInt32(SecretName.sacredtrial)))
                si.canBe_sacredtrial = true;
            else
                si.canBe_sacredtrial = false;
            return si;
        }
        public static BitArray secretItemToData(SecretItem si)
        {
            BitArray ret = new BitArray(60, false);
            if (si.canBe_beartrap)
                ret.Set(Convert.ToInt32(SecretName.beartrap), true);
            if (si.canBe_explosive)
                ret.Set(Convert.ToInt32(SecretName.explosive), true);
            if (si.canBe_snaketrap)
                ret.Set(Convert.ToInt32(SecretName.snaketrap), true);
            if (si.canBe_missdirection)
                ret.Set(Convert.ToInt32(SecretName.missdirection), true);
            if (si.canBe_freezing)
                ret.Set(Convert.ToInt32(SecretName.freezing), true);
            if (si.canBe_snipe)
                ret.Set(Convert.ToInt32(SecretName.snipe), true);
            if (si.canBe_darttrap)
                ret.Set(Convert.ToInt32(SecretName.darttrap), true);
            if (si.canBe_cattrick)
                ret.Set(Convert.ToInt32(SecretName.cattrick), true);

            if (si.canBe_counterspell)
                ret.Set(Convert.ToInt32(SecretName.counterspell), true);
            if (si.canBe_icebarrier)
                ret.Set(Convert.ToInt32(SecretName.icebarrier), true);
            if (si.canBe_iceblock)
                ret.Set(Convert.ToInt32(SecretName.iceblock), true);
            if (si.canBe_mirrorentity)
                ret.Set(Convert.ToInt32(SecretName.mirrorentity), true);
            if (si.canBe_spellbender)
                ret.Set(Convert.ToInt32(SecretName.spellbender), true);
            if (si.canBe_vaporize)
                ret.Set(Convert.ToInt32(SecretName.vaporize), true);
            if (si.canBe_duplicate)
                ret.Set(Convert.ToInt32(SecretName.duplicate), true);
            if (si.canBe_effigy)
                ret.Set(Convert.ToInt32(SecretName.effigy), true);
            if (si.canBe_flameward)
                ret.Set(Convert.ToInt32(SecretName.flameward), true);
            if (si.canBe_explosiverunes)
                ret.Set(Convert.ToInt32(SecretName.explosiverunes), true);

            if (si.canBe_eyeforaneye)
                ret.Set(Convert.ToInt32(SecretName.eyeforaneye), true);
            if (si.canBe_noblesacrifice)
                ret.Set(Convert.ToInt32(SecretName.noblesacrifice), true);
            if (si.canBe_redemption)
                ret.Set(Convert.ToInt32(SecretName.redemption), true);
            if (si.canBe_repentance)
                ret.Set(Convert.ToInt32(SecretName.repentance), true);
            if (si.canBe_avenge)
                ret.Set(Convert.ToInt32(SecretName.avenge), true);
            if (si.canBe_sacredtrial)
                ret.Set(Convert.ToInt32(SecretName.sacredtrial), true);
            return ret;
        }
        /*not in use temporarily 1
        public bool canbeTriggeredWithAttackingHero = true;
        public bool canbeTriggeredWithAttackingMinion = true;
        public bool canbeTriggeredWithPlayingMinion = true;
        public bool canbeTriggeredWithPlayingHeroPower = true;*/

        //猎人奥秘
        /// <summary>
        /// 毒蛇陷阱
        /// </summary>
        public bool canBe_snaketrap = false; //毒蛇陷阱
        /// <summary>
        /// 狙击（以及一切打出随从时触发的奥秘！需要自己判断！！！）
        /// </summary>
        public bool canBe_snipe = true;      //狙击
        /// <summary>
        /// 爆炸
        /// </summary>
        public bool canBe_explosive = true;  //爆炸
        /// <summary>
        /// 捕熊
        /// </summary>
        public bool canBe_beartrap = false; // 捕熊
        /// <summary>
        /// 冰冻
        /// </summary>
        public bool canBe_freezing = false;   //冰冻
        /// <summary>
        /// 误导
        /// </summary>
        public bool canBe_missdirection = false;// 误导
        /// <summary>
        /// 毒镖
        /// </summary>
        public bool canBe_darttrap = false; // 毒镖
        /// <summary>
        /// 豹子
        /// </summary>
        public bool canBe_cattrick = false; //豹子

        //法师奥秘
        /// <summary>
        /// 法反，以及一些法术触发的奥秘，需要自己判断！！！
        /// </summary>
        public bool canBe_counterspell = true;
        /// <summary>
        /// 不防寒冰护体
        /// </summary>
        public bool canBe_icebarrier = false; // 不防寒冰护体，因为奥秘法不带这张
        /// <summary>
        /// 寒冰屏障
        /// </summary>
        public bool canBe_iceblock = true;    //寒冰屏障
        /// <summary>
        /// 镜像实体
        /// </summary>
        public bool canBe_mirrorentity = false; //镜像实体
        /// <summary>
        /// 不防扰咒术
        /// </summary>
        public bool canBe_spellbender = false; //不防扰咒术，奥秘法基本不带
        /// <summary>
        /// 蒸发
        /// </summary>
        public bool canBe_vaporize = false;  //蒸发
        /// <summary>
        /// 复制
        /// </summary>
        public bool canBe_duplicate = false; //复制
        /// <summary>
        /// 轮回
        /// </summary>
        public bool canBe_effigy = false; //轮回
        /// <summary>
        /// 火焰结界
        /// </summary>
        public bool canBe_flameward = true; // 要防止aoe 火焰结界，全场打3

        /// <summary>
        /// 爆炸符文
        /// </summary>
        public bool canBe_explosiverunes = true; // 有可能是爆炸符文


        //圣骑士
        /// <summary>
        /// 以眼还眼
        /// </summary>
        public bool canBe_eyeforaneye = false; // 以眼还眼 
        /// <summary>
        /// 崇高牺牲
        /// </summary>
        public bool canBe_noblesacrifice = true; //崇高牺牲
        /// <summary>
        /// 救赎
        /// </summary>
        public bool canBe_redemption = false; //救赎
        /// <summary>
        /// 忏悔
        /// </summary>
        public bool canBe_repentance = false; //忏悔
        /// <summary>
        /// 复仇
        /// </summary>
        public bool canBe_avenge = false; // 复仇
        /// <summary>
        /// 审判
        /// </summary>
        public bool canBe_sacredtrial = false; //审判

        public int entityId = 0;


        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SecretItem()
        {
        }

        /// <summary>
        /// 复制构造函数
        /// </summary>
        /// <param name="sec">要复制的SecretItem对象</param>
        public SecretItem(SecretItem sec)
        {
            this.triggered = sec.triggered;
            /*not in use temporarily 1
            this.canbeTriggeredWithAttackingHero = sec.canbeTriggeredWithAttackingHero;
            this.canbeTriggeredWithAttackingMinion = sec.canbeTriggeredWithAttackingMinion;
            this.canbeTriggeredWithPlayingMinion = sec.canbeTriggeredWithPlayingMinion;
            this.canbeTriggeredWithPlayingHeroPower = sec.canbeTriggeredWithPlayingHeroPower;*/

            this.canBe_avenge = sec.canBe_avenge;
            this.canBe_counterspell = sec.canBe_counterspell;
            this.canBe_duplicate = sec.canBe_duplicate;
            this.canBe_effigy = sec.canBe_effigy;
            this.canBe_explosive = sec.canBe_explosive;
            this.canBe_beartrap = sec.canBe_beartrap;
            this.canBe_eyeforaneye = sec.canBe_eyeforaneye;
            this.canBe_freezing = sec.canBe_freezing;
            this.canBe_icebarrier = sec.canBe_icebarrier;
            this.canBe_flameward = sec.canBe_flameward;
            this.canBe_explosiverunes = sec.canBe_explosiverunes;
            this.canBe_iceblock = sec.canBe_iceblock;
            this.canBe_cattrick = sec.canBe_cattrick;
            this.canBe_mirrorentity = sec.canBe_mirrorentity;
            this.canBe_missdirection = sec.canBe_missdirection;
            this.canBe_darttrap = sec.canBe_darttrap;
            this.canBe_noblesacrifice = sec.canBe_noblesacrifice;
            this.canBe_redemption = sec.canBe_redemption;
            this.canBe_repentance = sec.canBe_repentance;
            this.canBe_snaketrap = sec.canBe_snaketrap;
            this.canBe_snipe = sec.canBe_snipe;
            this.canBe_spellbender = sec.canBe_spellbender;
            this.canBe_vaporize = sec.canBe_vaporize;
            this.canBe_sacredtrial = sec.canBe_sacredtrial;

            this.entityId = sec.entityId;

        }

        /// <summary>
        /// 从字符串数据初始化奥秘
        /// </summary>
        /// <param name="secdata">奥秘数据字符串</param>
        public SecretItem(string secdata)
        {
            this.entityId = Convert.ToInt32(secdata.Split('.')[0]);

            string canbe = secdata.Split('.')[1];
            if (canbe.Length < 17)
            {
                Helpfunctions.Instance.ErrorLog("cant read secret " + secdata + " " + canbe.Length);
            }

            this.canBe_snaketrap = false;
            this.canBe_snipe = false;
            this.canBe_explosive = false;
            this.canBe_flameward = false;
            this.canBe_beartrap = false;
            this.canBe_freezing = false;
            this.canBe_missdirection = false;
            this.canBe_darttrap = false;
            this.canBe_cattrick = false;

            this.canBe_counterspell = false;
            this.canBe_icebarrier = false;
            this.canBe_iceblock = false;
            this.canBe_mirrorentity = false;
            this.canBe_spellbender = false;
            this.canBe_vaporize = false;
            this.canBe_duplicate = false;
            this.canBe_effigy = false;

            this.canBe_eyeforaneye = false;
            this.canBe_noblesacrifice = false;
            this.canBe_redemption = false;
            this.canBe_repentance = false;
            this.canBe_avenge = false;
            this.canBe_sacredtrial = false;

            if (canbe.Length == 23) //new
            {
                this.canBe_snaketrap = (canbe[0] == '1');
                this.canBe_snipe = (canbe[1] == '1');
                this.canBe_explosive = (canbe[2] == '1');
                this.canBe_beartrap = (canbe[3] == '1');
                this.canBe_freezing = (canbe[4] == '1');
                this.canBe_missdirection = (canbe[5] == '1');
                this.canBe_darttrap = (canbe[6] == '1');
                this.canBe_cattrick = (canbe[7] == '1');

                this.canBe_counterspell = (canbe[8] == '1');
                this.canBe_icebarrier = (canbe[9] == '1');
                this.canBe_iceblock = (canbe[10] == '1');
                this.canBe_mirrorentity = (canbe[11] == '1');
                this.canBe_spellbender = (canbe[12] == '1');
                this.canBe_vaporize = (canbe[13] == '1');  //蒸发
                this.canBe_duplicate = (canbe[14] == '1');
                this.canBe_effigy = (canbe[15] == '1');
                this.canBe_flameward = (canbe[16] == '1'); // 火焰结界

                this.canBe_eyeforaneye = (canbe[17] == '1');
                this.canBe_noblesacrifice = (canbe[18] == '1');
                this.canBe_redemption = (canbe[19] == '1');
                this.canBe_repentance = (canbe[20] == '1');
                this.canBe_avenge = (canbe[21] == '1');
                this.canBe_sacredtrial = (canbe[22] == '1');
            }
            else
            {
                Helpfunctions.Instance.logg("错误: 奥秘标志位长度不等于23，=" + canbe.Length);
            }

            this.updateCanBeTriggered();
        }

        /// <summary>
        /// 更新奥秘的触发条件
        /// </summary>
        public void updateCanBeTriggered()
        {
            /*not in use temporarily 1
            this.canbeTriggeredWithAttackingHero = false;
            this.canbeTriggeredWithAttackingMinion = false;
            this.canbeTriggeredWithPlayingMinion = false;
            this.canbeTriggeredWithPlayingHeroPower = false;

            if (this.canBe_snipe || this.canBe_mirrorentity || this.canBe_repentance || this.canBe_sacredtrial) this.canbeTriggeredWithPlayingMinion = true;

            if (this.canBe_explosive || this.canBe_beartrap || this.canBe_missdirection || this.canBe_freezing || this.canBe_icebarrier || this.canBe_vaporize || this.canBe_noblesacrifice) this.canbeTriggeredWithAttackingHero = true;

            if (this.canBe_snaketrap || this.canBe_freezing || this.canBe_noblesacrifice) this.canbeTriggeredWithAttackingMinion = true;

            if (this.canBe_darttrap) this.canbeTriggeredWithPlayingHeroPower = true;
            */
        }

        /// <summary>
        /// 当角色被攻击时更新奥秘状态
        /// 没有触发奥秘，说明不是爆炸
        /// </summary>
        /// <param name="DefenderIsHero">防御者是否为英雄</param>
        /// <param name="AttackerIsHero">攻击者是否为英雄</param>
        public void usedTrigger_CharIsAttacked(bool DefenderIsHero, bool AttackerIsHero)
        {
            if (DefenderIsHero)
            {
                this.canBe_explosive = false;
                this.canBe_flameward = false;
                this.canBe_beartrap = false;
                this.canBe_missdirection = false;

                this.canBe_icebarrier = false;
                this.canBe_vaporize = false;

            }
            else
            {
                this.canBe_snaketrap = false;
            }
            if (!AttackerIsHero)
            {
                this.canBe_freezing = false;
            }
            this.canBe_noblesacrifice = false;
            updateCanBeTriggered();
        }

        /// <summary>
        /// 当随从被打出时更新奥秘状态
        /// </summary>
        /// <param name="i">打出的随从数量</param>
        public void usedTrigger_MinionIsPlayed(int i)
        {
            this.canBe_snipe = false;
            this.canBe_mirrorentity = false;
            this.canBe_explosiverunes = false;
            this.canBe_repentance = false;
            if (i == 1) this.canBe_sacredtrial = false;
            updateCanBeTriggered();
        }

        /// <summary>
        /// 当法术被打出时更新奥秘状态
        /// </summary>
        /// <param name="minionIsTarget">法术目标是否为随从</param>
        public void usedTrigger_SpellIsPlayed(bool minionIsTarget)
        {
            this.canBe_counterspell = false;
            this.canBe_cattrick = false;
            if (minionIsTarget) this.canBe_spellbender = false;
            updateCanBeTriggered();
        }

        /// <summary>
        /// 当随从死亡时更新奥秘状态
        /// </summary>
        public void usedTrigger_MinionDied()
        {
            this.canBe_avenge = false;
            this.canBe_redemption = false;
            this.canBe_duplicate = false;
            this.canBe_effigy = false;
            updateCanBeTriggered();
        }

        /// <summary>
        /// 当英雄受到伤害时更新奥秘状态
        /// </summary>
        /// <param name="deadly">伤害是否致命</param>
        public void usedTrigger_HeroGotDmg(bool deadly = false)
        {
            this.canBe_eyeforaneye = false;
            if (deadly) this.canBe_iceblock = false;
            updateCanBeTriggered();
        }
        /// <summary>
        /// 当英雄技能被使用时更新奥秘状态
        /// </summary>
        public void usedTrigger_HeroPowerUsed()
        {
            this.canBe_darttrap = false;
            updateCanBeTriggered();
        }


        /// <summary>
        /// 将奥秘状态转换为字符串
        /// </summary>
        /// <returns>奥秘状态的字符串表示</returns>
        public string returnAString()
        {
            string retval = "" + this.entityId + ".";
            retval += "" + ((canBe_snaketrap) ? "1" : "0");
            retval += "" + ((canBe_snipe) ? "1" : "0");
            retval += "" + ((canBe_explosive) ? "1" : "0");
            retval += "" + ((canBe_beartrap) ? "1" : "0");
            retval += "" + ((canBe_freezing) ? "1" : "0");
            retval += "" + ((canBe_missdirection) ? "1" : "0");
            retval += "" + ((canBe_darttrap) ? "1" : "0");
            retval += "" + ((canBe_cattrick) ? "1" : "0");

            retval += "" + ((canBe_counterspell) ? "1" : "0");
            retval += "" + ((canBe_icebarrier) ? "1" : "0");
            retval += "" + ((canBe_iceblock) ? "1" : "0");
            retval += "" + ((canBe_mirrorentity) ? "1" : "0");
            retval += "" + ((canBe_spellbender) ? "1" : "0");
            retval += "" + ((canBe_vaporize) ? "1" : "0");
            retval += "" + ((canBe_duplicate) ? "1" : "0");
            retval += "" + ((canBe_effigy) ? "1" : "0");
            retval += "" + ((canBe_flameward) ? "1" : "0");

            retval += "" + ((canBe_eyeforaneye) ? "1" : "0");
            retval += "" + ((canBe_noblesacrifice) ? "1" : "0");
            retval += "" + ((canBe_redemption) ? "1" : "0");
            retval += "" + ((canBe_repentance) ? "1" : "0");
            retval += "" + ((canBe_avenge) ? "1" : "0");
            retval += "" + ((canBe_sacredtrial) ? "1" : "0");
            return retval + ",";
        }

        /// <summary>
        /// 比较两个奥秘是否相等
        /// </summary>
        /// <param name="s">要比较的奥秘</param>
        /// <returns>两个奥秘是否相等</returns>
        public bool isEqual(SecretItem s)
        {
            bool result = this.entityId == s.entityId;
            if (!result)
            {
                result = result && this.canBe_avenge == s.canBe_avenge && this.canBe_counterspell == s.canBe_counterspell && this.canBe_duplicate == s.canBe_duplicate && this.canBe_effigy == s.canBe_effigy && this.canBe_explosive == s.canBe_explosive && this.canBe_flameward == s.canBe_flameward;
                result = result && this.canBe_eyeforaneye == s.canBe_eyeforaneye && this.canBe_freezing == s.canBe_freezing && this.canBe_icebarrier == s.canBe_icebarrier && this.canBe_iceblock == s.canBe_iceblock;
                result = result && this.canBe_mirrorentity == s.canBe_mirrorentity && this.canBe_missdirection == s.canBe_missdirection && this.canBe_noblesacrifice == s.canBe_noblesacrifice && this.canBe_redemption == s.canBe_redemption;
                result = result && this.canBe_repentance == s.canBe_repentance && this.canBe_snaketrap == s.canBe_snaketrap && this.canBe_snipe == s.canBe_snipe && this.canBe_spellbender == s.canBe_spellbender && this.canBe_vaporize == s.canBe_vaporize;
                result = result && this.canBe_sacredtrial == s.canBe_sacredtrial && this.canBe_darttrap == s.canBe_darttrap && this.canBe_beartrap == s.canBe_beartrap && this.canBe_cattrick == s.canBe_cattrick;
            }
            return result;
        }

    }

    /// <summary>
    /// 概率计算类，负责管理墓地信息、奥秘猜测和概率计算
    /// </summary>
    public class Probabilitymaker
    {
        /// <summary>
        /// 己方墓地卡牌及其数量
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> ownGraveyard = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 敌方墓地卡牌及其数量
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> enemyGraveyard = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 己方弃置卡牌及其数量
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> ownDiscard = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 敌方弃置卡牌及其数量
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> enemyDiscard = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 己方烧弃卡牌及其数量（超过手牌上限）
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> ownBurnt = new Dictionary<CardDB.cardIDEnum, int>();//目测用不到,就先注释掉了
        /// <summary>
        /// 敌方烧弃卡牌及其数量（超过手牌上限）
        /// </summary>
        public Dictionary<CardDB.cardIDEnum, int> enemyBurnt = new Dictionary<CardDB.cardIDEnum, int>();
        /// <summary>
        /// 墓地物品列表
        /// </summary>
        List<GraveYardItem> graveyard = new List<GraveYardItem>();
        /// <summary>
        /// 当前回合的随从墓地（仅随从）
        /// </summary>
        public List<GraveYardItem> turngraveyard = new List<GraveYardItem>();//MOBS only
        /// <summary>
        /// 当前回合的所有卡牌墓地
        /// </summary>
        public List<GraveYardItem> turngraveyardAll = new List<GraveYardItem>();//All
        /// <summary>
        /// 回合开始时的墓地状态
        /// </summary>
        List<GraveYardItem> graveyartTillTurnStart = new List<GraveYardItem>();

        /// <summary>
        /// 敌方奥秘列表
        /// </summary>
        public List<SecretItem> enemySecrets = new List<SecretItem>();

        /// <summary>
        /// 费根是否死亡
        /// </summary>
        public bool feugenDead = false;
        /// <summary>
        /// 斯塔拉格是否死亡
        /// </summary>
        public bool stalaggDead = false;

        /// <summary>
        /// 单例实例
        /// </summary>
        private static Probabilitymaker instance;
        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static Probabilitymaker Instance
        {
            get
            {
                return instance ?? (instance = new Probabilitymaker());
            }
        }

        /// <summary>
        /// 私有构造函数，防止外部实例化
        /// </summary>
        private Probabilitymaker()
        {

        }

        //public void setOwnCardsOut(Dictionary<CardDB.cardIDEnum, int> og)
        //{
        //    ownCardsOut.Clear();
        //    this.stalaggDead = false;
        //    this.feugenDead = false;
        //    foreach (var tmp in og)
        //    {
        //        ownCardsOut.Add(tmp.Key, tmp.Value);
        //        if (tmp.Key == CardDB.cardIDEnum.FP1_015) this.feugenDead = true;//错的,可能被沉默,懒得改了
        //        if (tmp.Key == CardDB.cardIDEnum.FP1_014) this.stalaggDead = true;
        //    }
        //}
        //public void setEnemyCardsOut(Dictionary<CardDB.cardIDEnum, int> eg)
        //{
        //    enemyCardsOut.Clear();
        //    foreach (var tmp in eg)
        //    {
        //        enemyCardsOut.Add(tmp.Key, tmp.Value);
        //        if (tmp.Key == CardDB.cardIDEnum.FP1_015) this.feugenDead = true;//错的,可能被沉默,懒得改了
        //        if (tmp.Key == CardDB.cardIDEnum.FP1_014) this.stalaggDead = true;
        //    }
        //}

        /// <summary>
        /// 打印当前回合的墓地信息
        /// </summary>
        public void printTurnGraveYard()
        {
            /*string g = "";
            if (Probabilitymaker.Instance.feugenDead) g += " fgn";
            if (Probabilitymaker.Instance.stalaggDead) g += " stlgg";
            Helpfunctions.Instance.logg("GraveYard:" + g);
            if (writetobuffer) Helpfunctions.Instance.writeToBuffer("GraveYard:" + g);*/

            string s = "己方当前回合随从墓地: ";
            foreach (GraveYardItem gyi in this.turngraveyard)
            {
                if (gyi.own) s += gyi.cardid + "," + gyi.entityId + ";";
            }
            Helpfunctions.Instance.logg(s);

            s = "敌方当前回合随从墓地: ";
            foreach (GraveYardItem gyi in this.turngraveyard)
            {
                if (!gyi.own) s += gyi.cardid + "," + gyi.entityId + ";";
            }
            Helpfunctions.Instance.logg(s);


            s = "己方当前回合卡牌墓地: ";
            foreach (GraveYardItem gyi in this.turngraveyardAll)
            {
                if (gyi.own) s += gyi.cardid + "," + gyi.entityId + ";";
            }
            Helpfunctions.Instance.logg(s);

            s = "敌方当前回合卡牌墓地: ";
            foreach (GraveYardItem gyi in this.turngraveyardAll)
            {
                if (!gyi.own) s += gyi.cardid + "," + gyi.entityId + ";";
            }
            Helpfunctions.Instance.logg(s);
        }

        /// <summary>
        /// 设置墓地信息
        /// </summary>
        /// <param name="allGyi">所有墓地物品</param>
        /// <param name="turnStart">是否为回合开始</param>
        public void setGraveYard(List<GraveYardItem> allGyi, bool turnStart)
        {
            graveyard.Clear();
            graveyard.AddRange(allGyi);
            if (turnStart)
            {
                this.graveyartTillTurnStart.Clear();
                this.graveyartTillTurnStart.AddRange(allGyi);
            }

            this.stalaggDead = false;
            this.feugenDead = false;
            this.turngraveyard.Clear();
            this.turngraveyardAll.Clear();

            this.ownGraveyard.Clear();
            this.ownDiscard.Clear();
            this.ownBurnt.Clear();
            this.enemyGraveyard.Clear();
            this.enemyDiscard.Clear();
            this.enemyBurnt.Clear();

            GraveYardItem OwnLastDiedMinion = new GraveYardItem(CardDB.cardIDEnum.None, -1, true, GraveYardItem.EnumGraveyardStatus.Unknown);
            foreach (GraveYardItem ent in allGyi)
            {
                if (ent.cardid == CardDB.cardIDEnum.FP1_015)
                {
                    this.feugenDead = true;
                }

                if (ent.cardid == CardDB.cardIDEnum.FP1_014)
                {
                    this.stalaggDead = true;
                }

                bool found = false;

                foreach (GraveYardItem gyi in this.graveyartTillTurnStart)
                {
                    if (ent.entityId == gyi.entityId)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (CardDB.Instance.getCardDataFromID(ent.cardid).type == CardDB.cardtype.MOB)
                    {
                        this.turngraveyard.Add(ent);
                    }
                    this.turngraveyardAll.Add(ent);
                }
                if (ent.own && CardDB.Instance.getCardDataFromID(ent.cardid).type == CardDB.cardtype.MOB && ent.status == GraveYardItem.EnumGraveyardStatus.Died)
                {
                    OwnLastDiedMinion = ent;
                }


                if (ent.status == GraveYardItem.EnumGraveyardStatus.Died || ent.status == GraveYardItem.EnumGraveyardStatus.Used ||
                    ent.status == GraveYardItem.EnumGraveyardStatus.Unknown)//断线重连后，无法读取，status都为Unknown，坟场怪全当已死或已使用
                {
                    if (ent.own)
                    {
                        if (ownGraveyard.ContainsKey(ent.cardid))
                            ownGraveyard[ent.cardid]++;
                        else
                            ownGraveyard[ent.cardid] = 1;
                    }
                    else
                    {
                        if (enemyGraveyard.ContainsKey(ent.cardid))
                            enemyGraveyard[ent.cardid]++;
                        else
                            enemyGraveyard[ent.cardid] = 1;
                    }
                }
                else if (ent.status == GraveYardItem.EnumGraveyardStatus.Burnt)//目测用不到,就先注释掉了
                {
                    if (ent.own)
                    {
                        if (ownBurnt.ContainsKey(ent.cardid))
                            ownBurnt[ent.cardid]++;
                        else
                            ownBurnt[ent.cardid] = 1;
                    }
                    else
                    {
                        if (enemyBurnt.ContainsKey(ent.cardid))
                            enemyBurnt[ent.cardid]++;
                        else
                            enemyBurnt[ent.cardid] = 1;
                    }
                }
                else if (ent.status == GraveYardItem.EnumGraveyardStatus.Discard)
                {
                    if (ent.own)
                    {
                        if (ownDiscard.ContainsKey(ent.cardid))
                            ownDiscard[ent.cardid]++;
                        else
                            ownDiscard[ent.cardid] = 1;
                    }
                    else
                    {
                        if (enemyDiscard.ContainsKey(ent.cardid))
                            enemyDiscard[ent.cardid]++;
                        else
                            enemyDiscard[ent.cardid] = 1;
                    }
                }
            }
            Hrtprozis.Instance.updateOwnLastDiedMinion(OwnLastDiedMinion.cardid);
        }

        /// <summary>
        /// 设置当前回合的墓地信息
        /// </summary>
        /// <param name="listMobs">随从墓地列表</param>
        /// <param name="listAll">所有卡牌墓地列表</param>
        public void setTurnGraveYard(List<GraveYardItem> listMobs, List<GraveYardItem> listAll)
        {
            this.turngraveyard.Clear();
            this.turngraveyardAll.Clear();
            this.turngraveyard.AddRange(listMobs);
            this.turngraveyardAll.AddRange(listAll);
        }

        /// <summary>
        /// 检查敌方牌库中是否可能还有该卡牌
        /// </summary>
        /// <param name="cardid">卡牌ID</param>
        /// <returns>敌方牌库中是否可能还有该卡牌</returns>
        public bool hasEnemyThisCardInDeck(CardDB.cardIDEnum cardid)
        {
            if (this.enemyGraveyard.ContainsKey(cardid))
            {
                if (this.enemyGraveyard[cardid] == 1)
                {

                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算敌方牌库中该卡牌的剩余数量
        /// </summary>
        /// <param name="cardid">卡牌ID</param>
        /// <returns>敌方牌库中该卡牌的剩余数量</returns>
        public int anzCardsInDeck(CardDB.cardIDEnum cardid)
        {
            int ret = 2;
            CardDB.Card c = CardDB.Instance.getCardDataFromID(cardid);
            if (c.rarity == 5) ret = 1;//you can have only one rare;

            if (this.enemyGraveyard.ContainsKey(cardid))
            {
                if (this.enemyGraveyard[cardid] == 1)
                {

                    return 1;
                }
                return 0;
            }
            return ret;

        }

        /// <summary>
        /// 打印墓地信息
        /// </summary>
        public void printGraveyards()
        {
            string og = "己方坟场: ";
            foreach (KeyValuePair<CardDB.cardIDEnum, int> e in this.ownGraveyard)
            {
                og += e.Key + "," + e.Value + ";";
            }
            string eg = "敌方坟场: ";
            foreach (KeyValuePair<CardDB.cardIDEnum, int> e in this.enemyGraveyard)
            {
                eg += e.Key + "," + e.Value + ";";
            }
            Helpfunctions.Instance.logg(og);
            Helpfunctions.Instance.logg(eg);
        }

        /// <summary>
        /// 计算敌人手牌中拥有指定卡牌的概率
        /// </summary>
        /// <param name="cardid">卡牌ID</param>
        /// <param name="handsize">敌人手牌数量</param>
        /// <param name="decksize">敌人牌库剩余数量</param>
        /// <returns>概率值，范围为[0, 100]</returns>
        public int getProbOfEnemyHavingCardInHand(CardDB.cardIDEnum cardid, int handsize, int decksize)
        {
            //calculates probability \in [0,...,100]


            int cardsremaining = this.anzCardsInDeck(cardid);
            if (cardsremaining == 0) return 0;
            double retval = 0.0;
            //http://de.wikipedia.org/wiki/Hypergeometrische_Verteilung (we calculte 1-p(x=0))

            if (cardsremaining == 1)
            {
                retval = 1.0 - ((double)(decksize)) / ((double)(decksize + handsize));
            }
            else
            {
                retval = 1.0 - ((double)(decksize * (decksize - 1))) / ((double)((decksize + handsize) * (decksize + handsize - 1)));
            }

            retval = Math.Min(retval, 1.0);

            return (int)(100.0 * retval);
        }

        /// <summary>
        /// 检查墓地中是否存在指定卡牌
        /// </summary>
        /// <param name="cardid">卡牌ID</param>
        /// <returns>墓地中是否存在指定卡牌</returns>
        public bool hasCardinGraveyard(CardDB.cardIDEnum cardid)
        {
            foreach (GraveYardItem gyi in this.graveyard)
            {
                if (gyi.cardid == cardid) return true;
            }
            return false;
        }

        /// <summary>
        /// 设置敌方奥秘猜测列表
        /// </summary>
        /// <param name="enemySecretList">敌方奥秘列表</param>
        public void setEnemySecretGuesses(Dictionary<int, TAG_CLASS> enemySecretList) //赋值enemySecretList
        {
            List<SecretItem> newlist = new List<SecretItem>();

            foreach (KeyValuePair<int, TAG_CLASS> eSec in enemySecretList)
            {
                if (eSec.Key >= 1000) continue;
                Helpfunctions.Instance.logg("detect secret with id" + eSec.Key);
                SecretItem sec = getNewSecretGuessedItem(eSec.Key, eSec.Value); // 
                newlist.Add(new SecretItem(sec));
            }

            this.enemySecrets.Clear();
            this.enemySecrets.AddRange(newlist);
        }

        /// <summary>
        /// 更新猜奥秘结果，结合构筑
        /// </summary>
        /// <param name="enemySecrets"></param>
        /// <param name="SecClass"></param>
        public void updateSecretGuess(List<SecretItem> enemySecrets, TAG_CLASS SecClass)
        {
            for (int i = 0; i < enemySecrets.Count; i++)
            {
                SecretItem sec = enemySecrets[i];
                if (SecClass == TAG_CLASS.HUNTER)
                {
                    //// 经典模式没有的奥秘
                    //if ("动物园".Equals(behaviorName))
                    //{
                    //    sec.canBe_beartrap = false;
                    //    sec.canBe_darttrap = false;
                    //    sec.canBe_cattrick = false;
                    //}
                    //// 标准模式没有的奥秘
                    //if ("骑士".Equals(behaviorName))
                    //{
                    //    sec.canBe_beartrap = false;
                    //    sec.canBe_darttrap = false;
                    //    sec.canBe_cattrick = false;
                    //}
                    //法师系
                    sec.canBe_counterspell = false;
                    sec.canBe_icebarrier = false;
                    sec.canBe_iceblock = false;
                    sec.canBe_mirrorentity = false;
                    sec.canBe_explosiverunes = false;
                    sec.canBe_spellbender = false;
                    sec.canBe_vaporize = false;
                    sec.canBe_duplicate = false;
                    sec.canBe_effigy = false;
                    sec.canBe_flameward = false;

                    //圣骑士系
                    sec.canBe_eyeforaneye = false;
                    sec.canBe_noblesacrifice = false;
                    sec.canBe_redemption = false;
                    sec.canBe_repentance = false;
                    sec.canBe_avenge = false;
                    sec.canBe_sacredtrial = false;
                }

                if (SecClass == TAG_CLASS.MAGE)
                {
                    //// 经典模式没有的奥秘
                    //if ("动物园".Equals(behaviorName))
                    //{
                    //    sec.canBe_duplicate = false;
                    //    sec.canBe_effigy = false;
                    //    sec.canBe_flameward = false;
                    //}
                    //// 标准模式没有的奥秘
                    //if ("骑士".Equals(behaviorName))
                    //{
                    //    sec.canBe_iceblock = false;
                    //    sec.canBe_mirrorentity = false;
                    //    sec.canBe_spellbender = false;
                    //    sec.canBe_vaporize = false;
                    //    sec.canBe_duplicate = false;
                    //    sec.canBe_effigy = false;
                    //    sec.canBe_flameward = false;
                    //}

                    sec.canBe_snaketrap = false;
                    //sec.canBe_snipe = false;
                    sec.canBe_explosive = false;
                    sec.canBe_beartrap = false;
                    sec.canBe_freezing = false;
                    sec.canBe_missdirection = false;
                    sec.canBe_darttrap = false;
                    sec.canBe_cattrick = false;

                    sec.canBe_eyeforaneye = false;
                    //sec.canBe_noblesacrifice = false;
                    sec.canBe_redemption = false;
                    sec.canBe_repentance = false;
                    sec.canBe_avenge = false;
                    sec.canBe_sacredtrial = false;
                }

                if (SecClass == TAG_CLASS.ROGUE)
                {
                    sec.canBe_snaketrap = false;
                    sec.canBe_snipe = false;
                    sec.canBe_explosive = false;
                    sec.canBe_beartrap = false;
                    sec.canBe_freezing = false;
                    sec.canBe_missdirection = false;
                    sec.canBe_darttrap = false;
                    sec.canBe_cattrick = false;

                    //sec.canBe_eyeforaneye = false;
                    //sec.canBe_noblesacrifice = false;
                    sec.canBe_redemption = false;
                    sec.canBe_repentance = false;
                    sec.canBe_avenge = false;
                    sec.canBe_sacredtrial = false;

                    sec.canBe_counterspell = false;
                    sec.canBe_icebarrier = false;
                    sec.canBe_iceblock = false;
                    sec.canBe_mirrorentity = false;
                    sec.canBe_explosiverunes = false;
                    sec.canBe_spellbender = false;
                    sec.canBe_vaporize = false;
                    sec.canBe_duplicate = false;
                    sec.canBe_effigy = false;
                    sec.canBe_flameward = false;
                }

                if (SecClass == TAG_CLASS.PALADIN)
                {
                    sec.canBe_snaketrap = false;
                    //sec.canBe_snipe = false;
                    sec.canBe_explosive = false;
                    sec.canBe_beartrap = false;
                    sec.canBe_freezing = false;
                    sec.canBe_missdirection = false;
                    sec.canBe_darttrap = false;
                    sec.canBe_cattrick = false;

                    // 圣骑士的法反，当然就是古神在上了
                    //sec.canBe_counterspell = false;
                    sec.canBe_icebarrier = false;
                    sec.canBe_iceblock = false;
                    sec.canBe_mirrorentity = false;
                    sec.canBe_explosiverunes = false;
                    sec.canBe_spellbender = false;
                    sec.canBe_vaporize = false;
                    sec.canBe_duplicate = false;
                    sec.canBe_effigy = false;
                    sec.canBe_flameward = false;
                }
            }
            // 根据对手牌库来猜测
            if (Hrtprozis.Instance.enemyDeckCode != "" && Hrtprozis.Instance.guessEnemyDeck.Count > 0)
            {
                // 法反
                if (!Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69607")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("113")
                    // 古神在上
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("61187")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("67182")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69908"))
                {
                    for (int i = 0; i < enemySecrets.Count; i++)
                    {
                        if (SecClass == Hrtprozis.Instance.enemyHeroStartClass)
                        {
                            SecretItem sec = enemySecrets[i];
                            sec.canBe_counterspell = false;
                        }
                    }
                }
                // 火焰结界
                if (!Hrtprozis.Instance.guessEnemyDeck.ContainsKey("53823"))
                {
                    for (int i = 0; i < enemySecrets.Count; i++)
                    {
                        if (SecClass == Hrtprozis.Instance.enemyHeroStartClass)
                        {
                            SecretItem sec = enemySecrets[i];
                            sec.canBe_flameward = false;
                        }
                    }
                }
                // 爆炸陷阱
                if (!Hrtprozis.Instance.guessEnemyDeck.ContainsKey("70050")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69603")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("585")
                    // 瑞林的步枪
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("61839"))
                {
                    for (int i = 0; i < enemySecrets.Count; i++)
                    {
                        if (SecClass == Hrtprozis.Instance.enemyHeroStartClass)
                        {
                            SecretItem sec = enemySecrets[i];
                            sec.canBe_explosive = false;
                        }
                    }
                }
                // 狙击
                if (!Hrtprozis.Instance.guessEnemyDeck.ContainsKey("70049")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("814")
                    // 瑞林的步枪
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("61839"))
                {
                    for (int i = 0; i < enemySecrets.Count; i++)
                    {
                        if (SecClass == Hrtprozis.Instance.enemyHeroStartClass)
                        {
                            SecretItem sec = enemySecrets[i];
                            sec.canBe_snipe = false;
                        }
                    }
                }
                // 崇高牺牲
                if (!Hrtprozis.Instance.guessEnemyDeck.ContainsKey("584")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69610")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69856")
                    // 冰冻陷阱
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("70051")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("69604")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("519")
                    // 瑞林的步枪
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("61839")
                    // 蒸发
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("70040")
                    && !Hrtprozis.Instance.guessEnemyDeck.ContainsKey("286")
                    )
                {
                    for (int i = 0; i < enemySecrets.Count; i++)
                    {
                        if (SecClass == Hrtprozis.Instance.enemyHeroStartClass)
                        {
                            SecretItem sec = enemySecrets[i];
                            sec.canBe_noblesacrifice = false;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据职业和实体ID创建新的奥秘猜测项
        /// </summary>
        /// <param name="entityid">奥秘的实体ID</param>
        /// <param name="SecClass">奥秘所属的职业</param>
        /// <returns>新的奥秘猜测项</returns>
        public SecretItem getNewSecretGuessedItem(int entityid, TAG_CLASS SecClass)
        {
            foreach (SecretItem si in this.enemySecrets)
            {
                if (si.entityId == entityid && entityid < 1000) return si;
            }

            switch (SecClass)
            {
                case TAG_CLASS.WARRIOR: break;
                case TAG_CLASS.WARLOCK: break;
                case TAG_CLASS.ROGUE: break;
                case TAG_CLASS.SHAMAN: break;
                case TAG_CLASS.PRIEST: break;
                case TAG_CLASS.PALADIN: break;
                case TAG_CLASS.MAGE: break;
                case TAG_CLASS.HUNTER: break;
                case TAG_CLASS.DRUID: break;
                case TAG_CLASS.DEMONHUNTER: break;
                default:
                    Helpfunctions.Instance.ErrorLog("Problem is detected: undefined Secret class " + SecClass);
                    SecClass = Hrtprozis.Instance.heroEnumtoTagClass(Hrtprozis.Instance.enemyHeroname);
                    Helpfunctions.Instance.ErrorLog("attempt to restore... " + SecClass);
                    break;
            }

            // FIXME
            // 行为名称,暂时用来判断模式
            //string behaviorName = Ai.Instance.botBase.BehaviorName();
            SecretItem sec = new SecretItem { entityId = entityid };
            if (SecClass == TAG_CLASS.HUNTER)
            {
                // 经典模式没有的奥秘
                //if ("动物园".Equals(behaviorName))
                //{
                //    sec.canBe_beartrap = false;
                //    sec.canBe_darttrap = false;
                //    sec.canBe_cattrick = false;
                //}
                //// 标准模式没有的奥秘
                //if ("骑士".Equals(behaviorName))
                //{
                //    sec.canBe_beartrap = false;
                //    sec.canBe_darttrap = false;
                //    sec.canBe_cattrick = false;
                //}

                sec.canBe_counterspell = false;
                sec.canBe_icebarrier = false;
                sec.canBe_iceblock = false;
                sec.canBe_mirrorentity = false;
                sec.canBe_explosiverunes = false;
                sec.canBe_spellbender = false;
                sec.canBe_vaporize = false;
                sec.canBe_duplicate = false;
                sec.canBe_effigy = false;
                sec.canBe_flameward = false;

                sec.canBe_eyeforaneye = false;
                sec.canBe_noblesacrifice = false;
                sec.canBe_redemption = false;
                sec.canBe_repentance = false;
                sec.canBe_avenge = false;
                sec.canBe_sacredtrial = false;

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_554) && enemyGraveyard[CardDB.cardIDEnum.EX1_554] >= 2)
                {
                    sec.canBe_snaketrap = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_609) && enemyGraveyard[CardDB.cardIDEnum.EX1_609] >= 2)
                {
                    sec.canBe_snipe = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_610) && enemyGraveyard[CardDB.cardIDEnum.EX1_610] >= 2)
                {
                    sec.canBe_explosive = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.ULD_239) && enemyGraveyard[CardDB.cardIDEnum.ULD_239] >= 2)
                {
                    sec.canBe_flameward = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.AT_060) && enemyGraveyard[CardDB.cardIDEnum.AT_060] >= 2)
                {
                    sec.canBe_beartrap = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_611) && enemyGraveyard[CardDB.cardIDEnum.EX1_611] >= 2)
                {
                    sec.canBe_freezing = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_533) && enemyGraveyard[CardDB.cardIDEnum.EX1_533] >= 2)
                {
                    sec.canBe_missdirection = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.LOE_021) && enemyGraveyard[CardDB.cardIDEnum.LOE_021] >= 2)
                {
                    sec.canBe_darttrap = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.KAR_004) && enemyGraveyard[CardDB.cardIDEnum.KAR_004] >= 2)
                {
                    sec.canBe_cattrick = false;
                }

            }

            if (SecClass == TAG_CLASS.MAGE)
            {
                // 经典模式没有的奥秘
                //if ("动物园".Equals(behaviorName))
                //{
                //    sec.canBe_duplicate = false;
                //    sec.canBe_effigy = false;
                //    sec.canBe_flameward = false;
                //    sec.canBe_explosiverunes = false;
                //}
                //// 标准模式没有的奥秘
                //if ("骑士".Equals(behaviorName))
                //{
                //    sec.canBe_iceblock = false;
                //    sec.canBe_mirrorentity = false;
                //    sec.canBe_spellbender = false;
                //    sec.canBe_vaporize = false;
                //    sec.canBe_duplicate = false;
                //    sec.canBe_effigy = false;
                //    sec.canBe_flameward = false;
                //    sec.canBe_explosiverunes = false;
                //}

                sec.canBe_snaketrap = false;
                // FIXME 爆炸符文，类似狙击，随便了
                //sec.canBe_snipe = false;
                sec.canBe_explosive = false;
                sec.canBe_beartrap = false;
                sec.canBe_freezing = false;
                sec.canBe_missdirection = false;
                sec.canBe_darttrap = false;
                sec.canBe_cattrick = false;

                sec.canBe_eyeforaneye = false;
                sec.canBe_noblesacrifice = false;
                sec.canBe_redemption = false;
                sec.canBe_repentance = false;
                sec.canBe_avenge = false;
                sec.canBe_sacredtrial = false;

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_287) && enemyGraveyard[CardDB.cardIDEnum.EX1_287] >= 2)
                {
                    sec.canBe_counterspell = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.ULD_239) && enemyGraveyard[CardDB.cardIDEnum.ULD_239] >= 2)
                {
                    sec.canBe_flameward = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_289) && enemyGraveyard[CardDB.cardIDEnum.EX1_289] >= 2)
                {
                    sec.canBe_icebarrier = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_295) && enemyGraveyard[CardDB.cardIDEnum.EX1_295] >= 2)
                {
                    sec.canBe_iceblock = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_294) && enemyGraveyard[CardDB.cardIDEnum.EX1_294] >= 2)
                {
                    sec.canBe_mirrorentity = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.LOOT_101) && enemyGraveyard[CardDB.cardIDEnum.LOOT_101] >= 2)
                {
                    sec.canBe_explosiverunes = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.tt_010) && enemyGraveyard[CardDB.cardIDEnum.tt_010] >= 2)
                {
                    sec.canBe_spellbender = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_594) && enemyGraveyard[CardDB.cardIDEnum.EX1_594] >= 2)
                {
                    sec.canBe_vaporize = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.FP1_018) && enemyGraveyard[CardDB.cardIDEnum.FP1_018] >= 2)
                {
                    sec.canBe_duplicate = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.AT_002) && enemyGraveyard[CardDB.cardIDEnum.AT_002] >= 2)
                {
                    sec.canBe_effigy = false;
                }
            }

            if (SecClass == TAG_CLASS.ROGUE)
            {
                sec.canBe_snaketrap = false;
                sec.canBe_snipe = false;
                sec.canBe_explosive = false;
                sec.canBe_beartrap = false;
                sec.canBe_freezing = false;
                sec.canBe_missdirection = false;
                sec.canBe_darttrap = false;
                sec.canBe_cattrick = false;

                //sec.canBe_eyeforaneye = false;
                sec.canBe_noblesacrifice = false;
                sec.canBe_redemption = false;
                sec.canBe_repentance = false;
                sec.canBe_avenge = false;
                sec.canBe_sacredtrial = false;

                sec.canBe_counterspell = false;
                sec.canBe_icebarrier = false;
                sec.canBe_iceblock = false;
                sec.canBe_mirrorentity = false;
                sec.canBe_explosiverunes = false;
                sec.canBe_spellbender = false;
                sec.canBe_vaporize = false;
                sec.canBe_duplicate = false;
                sec.canBe_effigy = false;
                sec.canBe_flameward = false;
            }

            if (SecClass == TAG_CLASS.PALADIN)
            {
                // 经典模式没有的奥秘
                //if ("动物园".Equals(behaviorName))
                //{
                //    sec.canBe_eyeforaneye = false;
                //    sec.canBe_noblesacrifice = false;
                //    sec.canBe_redemption = false;
                //    sec.canBe_repentance = false;
                //    sec.canBe_counterspell = false;
                //}
                //// 标准模式没有的奥秘
                //if ("骑士".Equals(behaviorName))
                //{
                //    sec.canBe_eyeforaneye = false;
                //    sec.canBe_redemption = false;
                //    sec.canBe_repentance = false;
                //    sec.canBe_snipe = false;
                //    sec.canBe_sacredtrial = false;
                //    //sec.canBe_counterspell = false;
                //}

                sec.canBe_snaketrap = false;
                sec.canBe_snipe = false;
                sec.canBe_explosive = false;
                sec.canBe_beartrap = false;
                sec.canBe_freezing = false;
                sec.canBe_missdirection = false;
                sec.canBe_darttrap = false;
                sec.canBe_cattrick = false;

                //sec.canBe_counterspell = false;
                sec.canBe_icebarrier = false;
                sec.canBe_iceblock = false;
                sec.canBe_mirrorentity = false;
                sec.canBe_explosiverunes = false;
                sec.canBe_spellbender = false;
                sec.canBe_vaporize = false;
                sec.canBe_duplicate = false;
                sec.canBe_effigy = false;
                sec.canBe_flameward = false;

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_132) && enemyGraveyard[CardDB.cardIDEnum.EX1_132] >= 2)
                {
                    sec.canBe_eyeforaneye = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_130) && enemyGraveyard[CardDB.cardIDEnum.EX1_130] >= 2)
                {
                    sec.canBe_noblesacrifice = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_136) && enemyGraveyard[CardDB.cardIDEnum.EX1_136] >= 2)
                {
                    sec.canBe_redemption = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.EX1_379) && enemyGraveyard[CardDB.cardIDEnum.EX1_379] >= 2)
                {
                    sec.canBe_repentance = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.FP1_020) && enemyGraveyard[CardDB.cardIDEnum.FP1_020] >= 2)
                {
                    sec.canBe_avenge = false;
                }

                if (enemyGraveyard.ContainsKey(CardDB.cardIDEnum.LOE_027) && enemyGraveyard[CardDB.cardIDEnum.LOE_027] >= 2)
                {
                    sec.canBe_sacredtrial = false;
                }
            }

            return sec;
        }


        /// <summary>
        /// 检查是否可以使用指定的奥秘
        /// </summary>
        /// <param name="cardID">奥秘卡牌ID</param>
        /// <returns>是否可以使用该奥秘</returns>
        public bool isAllowedSecret(CardDB.cardIDEnum cardID)
        {
            if (ownGraveyard.ContainsKey(cardID) && ownGraveyard[cardID] >= 2) return false;
            return true;
        }


        /// <summary>
        /// 获取敌方奥秘数据的字符串表示
        /// </summary>
        /// <returns>敌方奥秘数据的字符串表示</returns>
        public string getEnemySecretData()
        {
            string retval = "";
            foreach (SecretItem si in this.enemySecrets)
            {

                retval += si.returnAString();
            }

            return retval;
        }

        /// <summary>
        /// 获取指定奥秘列表的数据字符串表示
        /// </summary>
        /// <param name="list">奥秘列表</param>
        /// <returns>奥秘数据的字符串表示</returns>
        public string getEnemySecretData(List<SecretItem> list)
        {
            string retval = "";
            foreach (SecretItem si in list)
            {

                retval += si.returnAString();
            }

            return retval;
        }


        /// <summary>
        /// 设置敌方奥秘数据
        /// </summary>
        /// <param name="enemySecretl">敌方奥秘列表</param>
        public void setEnemySecretData(List<SecretItem> enemySecretl)
        {
            this.enemySecrets.Clear();
            foreach (SecretItem si in enemySecretl)
            {
                this.enemySecrets.Add(new SecretItem(si));
            }
        }

        /// <summary>
        /// 更新奥秘列表，将现有奥秘与新的奥秘列表进行匹配
        /// </summary>
        /// <param name="enemySecretl">新的敌方奥秘列表</param>
        public void updateSecretList(List<SecretItem> enemySecretl)
        {
            List<SecretItem> temp = new List<SecretItem>();
            foreach (SecretItem si in this.enemySecrets)
            {
                bool add = false;
                SecretItem seit = null;
                foreach (SecretItem sit in enemySecretl) // enemySecrets have to be updated to latest entitys
                {
                    if (si.entityId == sit.entityId)
                    {
                        seit = sit;
                        add = true;
                    }
                }

                temp.Add(add ? new SecretItem(seit) : new SecretItem(si));
            }

            this.enemySecrets.Clear();
            this.enemySecrets.AddRange(temp);

        }


        /// <summary>
        /// 根据游戏状态更新奥秘列表，分析回合中触发的奥秘
        /// </summary>
        /// <param name="p">当前游戏状态</param>
        /// <param name="old">上一个游戏状态</param>
        public void updateSecretList(Playfield p, Playfield old)
        {
            if (p.enemySecretCount == 0 || p.optionsPlayedThisTurn == 0) return;
            Action doneMove = Ai.Instance.bestmove;
            if (doneMove == null) return;

            List<CardDB.cardIDEnum> enemySecretsOpenedStep = new List<CardDB.cardIDEnum>();
            List<CardDB.Card> enemyMinionsDiedStep = new List<CardDB.Card>();
            foreach (KeyValuePair<CardDB.cardIDEnum, int> tmp in p.enemyCardsOut)
            {
                if (!old.enemyCardsOut.ContainsKey(tmp.Key) || old.enemyCardsOut[tmp.Key] != tmp.Value)
                {
                    CardDB.Card c = CardDB.Instance.getCardDataFromID(tmp.Key);
                    if (c.Secret) enemySecretsOpenedStep.Add(tmp.Key);
                    else if (c.type == CardDB.cardtype.MOB) enemyMinionsDiedStep.Add(c);
                }
            }

            bool beartrap = false;
            bool explosive = false;
            bool snaketrap = false;
            bool missdirection = false;
            bool freezing = false;
            bool snipe = false;
            bool darttrap = false;
            bool cattrick = false;

            bool mirrorentity = false;
            bool counterspell = false;
            bool flameward = false;
            bool spellbender = false;
            bool iceblock = false;
            bool icebarrier = false;
            bool vaporize = false;
            bool duplicate = false;
            bool effigy = false;
            bool explosiverunes = false;

            bool eyeforaneye = false;
            bool noblesacrifice = false;
            bool redemption = false;
            bool repentance = false;
            bool avenge = false;
            bool sacredtrial = false;

            if (enemyMinionsDiedStep.Count > 0)
            {
                duplicate = true;

                if (old.enemyMinions.Count > 1) avenge = true;
                if (old.enemyMinions.Count < 7)
                {
                    effigy = true;
                    redemption = true;
                }
                else if (!enemyMinionsDiedStep[0].deathrattle) { redemption = true; effigy = true; }
                else
                {
                    switch (enemyMinionsDiedStep[0].cardIDenum)
                    {
                        case CardDB.cardIDEnum.AT_019: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.AT_036: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.BRMC_87: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.EX1_110: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.EX1_534: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.EX1_556: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.FP1_002: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.FP1_007: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.FP1_012: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.GVG_096: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.GVG_105: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.GVG_114: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.LOE_050: redemption = false; effigy = false; break;
                        case CardDB.cardIDEnum.LOE_089: redemption = false; effigy = false; break;
                        default: redemption = true; effigy = true; break;
                    }
                }
            }


            bool targetWasHero = (doneMove.target != null && doneMove.target.entitiyID == p.enemyHero.entitiyID);

            if (doneMove.actionType == actionEnum.attackWithHero || doneMove.actionType == actionEnum.attackWithMinion)
            {
                bool attackerIsHero = doneMove.own.isHero;

                if (enemySecretsOpenedStep.Count == 0)
                {
                    if (old.enemyMinions.Count < 7) noblesacrifice = true;
                    if (doneMove.actionType == actionEnum.attackWithMinion) freezing = true;
                    if (targetWasHero)
                    {
                        explosive = true;
                        if (old.enemyMinions.Count < 7) beartrap = true;
                        missdirection = true;
                        if (attackerIsHero && old.ownMinions.Count == 0 && old.enemyMinions.Count == 0) missdirection = false;
                        icebarrier = true;
                        if (!attackerIsHero) vaporize = true;
                    }
                    else
                    {
                        snaketrap = true;
                    }
                }
                else
                {
                    if (targetWasHero)
                    {
                        explosive = true;
                        icebarrier = true;
                        if (!attackerIsHero) vaporize = true; //?
                    }
                    else snaketrap = true;
                    if (!attackerIsHero) freezing = true;
                    if (old.enemyMinions.Count < 7) noblesacrifice = true;

                    foreach (CardDB.cardIDEnum id in enemySecretsOpenedStep)
                    {
                        switch (id)
                        {
                            case CardDB.cardIDEnum.AT_060:  //beartrap
                                beartrap = true;
                                missdirection = true;
                                continue;
                            case CardDB.cardIDEnum.EX1_610:  //explosivetrap
                                if (enemySecretsOpenedStep.Count == 1)
                                {
                                    missdirection = true;
                                    if (!attackerIsHero && p.ownMinions.Find(x => x.entitiyID == doneMove.own.entitiyID) == null)
                                    {
                                        missdirection = false;
                                        freezing = false;
                                    }
                                }
                                continue;
                            case CardDB.cardIDEnum.EX1_533:  //misdirection
                                missdirection = true;
                                vaporize = false;
                                if (enemySecretsOpenedStep.Contains(CardDB.cardIDEnum.EX1_554)) continue;
                                int hpBalance = 0; //we need to know who has become the new target
                                foreach (Minion m in p.enemyMinions) hpBalance += m.Hp;
                                foreach (Minion m in old.enemyMinions) hpBalance -= m.Hp;
                                if (hpBalance < 0) snaketrap = true;
                                continue;
                            case CardDB.cardIDEnum.EX1_611:  //freezingtrap
                                freezing = true;
                                vaporize = false;
                                continue;
                            case CardDB.cardIDEnum.EX1_594:   //vaporize
                                vaporize = true;
                                freezing = false;
                                continue;
                            case CardDB.cardIDEnum.EX1_130:   //noblesacrifice
                                noblesacrifice = true;
                                snaketrap = true;
                                vaporize = false;
                                icebarrier = false;
                                continue;
                        }
                    }
                }
            }
            else if (doneMove.actionType == actionEnum.playcard)
            {
                if (doneMove.card.card.type == CardDB.cardtype.SPELL)
                {
                    cattrick = true;
                    counterspell = true;
                    if (!targetWasHero) spellbender = true;
                }
                /* else if (doneMove.card.card.type == CardDB.cardtype.MOB) //we need the response from the core
                 {
                     mirrorentity = true;
                     snipe = true;
                     repentance = true;
                     if (p.ownMinions.Count > 3) sacredtrial = true;
                 }*/
            }
            if (p.mobsplayedThisTurn > old.mobsplayedThisTurn) //if we have a response from the core - remove
            {
                mirrorentity = true;
                snipe = true;
                repentance = true;
                if (p.ownMinions.Count > 3) sacredtrial = true;
            }

            if (p.enemyHero.Hp + p.enemyHero.armor < old.enemyHero.Hp + old.enemyHero.armor) eyeforaneye = true;
            if (doneMove.actionType == actionEnum.useHeroPower) darttrap = true;

            foreach (CardDB.cardIDEnum id in enemySecretsOpenedStep)
            {
                switch (id)
                {
                    case CardDB.cardIDEnum.AT_002: effigy = true; continue;
                    case CardDB.cardIDEnum.AT_060: beartrap = true; continue;
                    case CardDB.cardIDEnum.EX1_130: noblesacrifice = true; continue;
                    case CardDB.cardIDEnum.EX1_132: eyeforaneye = true; continue;
                    case CardDB.cardIDEnum.EX1_136: redemption = true; continue;
                    case CardDB.cardIDEnum.EX1_287: counterspell = true; continue;
                    case CardDB.cardIDEnum.ULD_239: flameward = true; continue;
                    case CardDB.cardIDEnum.EX1_289: icebarrier = true; continue;
                    case CardDB.cardIDEnum.EX1_294: mirrorentity = true; continue;
                    case CardDB.cardIDEnum.EX1_295: iceblock = true; continue;
                    case CardDB.cardIDEnum.EX1_379: repentance = true; continue;
                    case CardDB.cardIDEnum.EX1_533: missdirection = true; continue;
                    case CardDB.cardIDEnum.EX1_554: snaketrap = true; continue;
                    case CardDB.cardIDEnum.EX1_594: vaporize = true; continue;
                    case CardDB.cardIDEnum.EX1_609: snipe = true; continue;
                    case CardDB.cardIDEnum.EX1_610: explosive = true; continue;
                    case CardDB.cardIDEnum.EX1_611: freezing = true; continue;
                    case CardDB.cardIDEnum.FP1_018: duplicate = true; continue;
                    case CardDB.cardIDEnum.FP1_020: avenge = true; continue;
                    case CardDB.cardIDEnum.LOE_021: darttrap = true; continue;
                    case CardDB.cardIDEnum.LOE_027: sacredtrial = true; continue;
                    case CardDB.cardIDEnum.tt_010: spellbender = true; continue;
                    case CardDB.cardIDEnum.KAR_004: cattrick = true; continue;
                }
            }

            foreach (SecretItem si in this.enemySecrets)
            {
                if (beartrap) si.canBe_beartrap = false;
                if (explosive) { si.canBe_explosive = false; si.canBe_flameward = false; }
                if (snaketrap) si.canBe_snaketrap = false;
                if (missdirection) si.canBe_missdirection = false;
                if (freezing) si.canBe_freezing = false;
                if (snipe) si.canBe_snipe = false;
                if (darttrap) si.canBe_darttrap = false;
                if (cattrick) si.canBe_cattrick = false;

                if (counterspell) si.canBe_counterspell = false;
                if (icebarrier) si.canBe_icebarrier = false;
                if (iceblock) si.canBe_iceblock = false;
                if (mirrorentity) si.canBe_mirrorentity = false;
                if (spellbender) si.canBe_spellbender = false;
                if (vaporize) si.canBe_vaporize = false;
                if (duplicate) si.canBe_duplicate = false;
                if (effigy) si.canBe_effigy = false;
                if (flameward) si.canBe_flameward = false;
                if (explosiverunes) si.canBe_explosiverunes = false;

                if (eyeforaneye) si.canBe_eyeforaneye = false;
                if (noblesacrifice) si.canBe_noblesacrifice = false;
                if (redemption) si.canBe_redemption = false;
                if (repentance) si.canBe_repentance = false;
                if (avenge) si.canBe_avenge = false;
                if (sacredtrial) si.canBe_sacredtrial = false;
            }
        }


    }

}