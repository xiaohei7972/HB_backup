
public enum ErrorType2
{
    /// <summary>
    /// <value> 无效的 </value>
    /// </summary>
    INVALID = -1,
    /// <summary>
    /// <value> 异常类型 </value>
    /// </summary>
    NONE = 0,
    /// <summary>
    /// <value> 目标只能是随从 </value>
    /// </summary>
    REQ_MINION_TARGET = 1,
    /// <summary>
    /// <value> 目标只能是友方 </value>
    /// </summary>
    REQ_FRIENDLY_TARGET = 2,
    /// <summary>
    /// <value> 目标只能是敌方 </value>
    /// </summary>
    REQ_ENEMY_TARGET = 3,
    /// <summary>
    /// <value> 目标只能是受伤的随从 </value>
    /// </summary>
    REQ_DAMAGED_TARGET = 4,
    /// <summary>
    /// <value> 最大奥秘 </value>
    /// </summary>
    REQ_MAX_SECRETS = 5,
    /// <summary>
    /// <value> 目标只能是被冻结的随从 </value>
    /// </summary>
    REQ_FROZEN_TARGET = 6,
    /// <summary>
    /// <value> 冲锋 </value>
    /// </summary>
    REQ_CHARGE_TARGET = 7,
    /// <summary>
    /// <value> 目标攻击力要求小于 参数攻击力</value>
    /// </summary>
    REQ_TARGET_MAX_ATTACK = 8,
    /// <summary>
    /// <value> 目标是除了自身英雄以外的目标 </value>
    /// </summary>
    REQ_NONSELF_TARGET = 9,
    /// <summary>
    /// <value> 目标只能是指定种族的 参数为种族数字</value>
    /// </summary>
    REQ_TARGET_WITH_RACE = 10,
    /// <summary>
    /// <value> 需要有目标 </value>
    /// </summary>
    REQ_TARGET_TO_PLAY = 11,
    /// <summary>
    /// <value> 随从数量插槽 有参数 </value>
    /// </summary>
    REQ_NUM_MINION_SLOTS = 12,
    /// <summary>
    /// <value> 需要武器才能使用 </value>
    /// </summary>
    REQ_WEAPON_EQUIPPED = 13,
    /// <summary>
    /// <value> 要求足够的法力值 </value>
    /// </summary>
    REQ_ENOUGH_MANA = 14,
    /// <summary>
    /// <value> 要求在你的回合 </value>
    /// </summary>
    REQ_YOUR_TURN = 15,
    /// <summary>
    /// <value> 要求非潜行敌方目标</value>
    /// </summary>
    REQ_NONSTEALTH_ENEMY_TARGET = 16,
    /// <summary>
    /// <value> 目标只能是英雄 </value>
    /// </summary>
    REQ_HERO_TARGET = 17,
    /// <summary>
    /// <value> 要求奥秘区域上限 </value>
    /// </summary>
    REQ_SECRET_ZONE_CAP = 18,
    /// <summary>
    /// <value> 要求随从上限（如果有目标） </value>
    /// </summary>
    REQ_MINION_CAP_IF_TARGET_AVAILABLE = 19,
    /// <summary>
    /// <value> 要求随从上限 </value>
    /// </summary>
    REQ_MINION_CAP = 20,
    /// <summary>
    /// <value> 要求目标本回合攻击 </value>
    /// </summary>
    REQ_TARGET_ATTACKED_THIS_TURN = 21,
    /// <summary>
    /// <value> 无目标时也可以使用（抉择星辰降落，无面腐蚀者） </value>
    /// </summary>
    REQ_TARGET_IF_AVAILABLE = 22,
    /// <summary>
    /// <value> 对面场上的全部随从最少需要X个，有参数 </value>
    /// </summary>
    REQ_MINIMUM_ENEMY_MINIONS = 23,
    /// <summary>
    /// <value> 连击有目标 </value>
    /// </summary>
    REQ_TARGET_FOR_COMBO = 24,
    /// <summary>
    /// <value> 要求剩余法力水晶 </value>
    /// </summary>
    REQ_NOT_EXHAUSTED_ACTIVATE = 25,
    /// <summary>
    /// <value> 要求控制一个奥秘或任务 </value>
    /// </summary>
    REQ_UNIQUE_SECRET_OR_QUEST = 26,
    /// <summary>
    /// <value> 要求目标嘲讽 </value>
    /// </summary>
    REQ_TARGET_TAUNTER = 27,
    /// <summary>
    /// <value> 要求目标可以被攻击 </value>
    /// </summary>
    REQ_CAN_BE_ATTACKED = 28,
    /// <summary>
    /// <value> 要求英雄技能能使用 </value>
    /// </summary>
    REQ_ACTION_PWR_IS_MASTER_PWR = 29,
    /// <summary>
    /// <value> 要求磁力目标 </value>
    /// </summary>
    REQ_TARGET_MAGNET = 30,
    /// <summary>
    /// <value> 要求目标攻击力大于0 </value>
    /// </summary>
    REQ_ATTACK_GREATER_THAN_0 = 31,
    /// <summary>
    /// <value> 要求攻击者未被冻结 </value>
    /// </summary>
    REQ_ATTACKER_NOT_FROZEN = 32,
    /// <summary>
    /// <value> 要求目标为英雄或随从 </value>
    /// </summary>
    REQ_HERO_OR_MINION_TARGET = 33,
    /// <summary>
    /// <value> 要求目标能被法术指定 </value>
    /// </summary>
    REQ_CAN_BE_TARGETED_BY_SPELLS = 34,
    /// <summary>
    /// <value> 暂时不清楚 </value>
    /// </summary>
    REQ_SUBCARD_IS_PLAYABLE = 35,
    /// <summary>
    /// <value> 连击无目标 </value>
    /// </summary>
    REQ_TARGET_FOR_NO_COMBO = 36,
    /// <summary>
    /// <value> 要求没有控制其他随从 </value>
    /// </summary>
    REQ_NOT_MINION_JUST_PLAYED = 37,
    /// <summary>
    /// <value> 要求未使用英雄技能 </value>
    /// </summary>
    REQ_NOT_EXHAUSTED_HERO_POWER = 38,
    /// <summary>
    /// <value> 可由对手指定目标 </value>
    /// </summary>
    REQ_CAN_BE_TARGETED_BY_OPPONENTS = 39,
    /// <summary>
    /// <value> 攻击者可以攻击 </value>
    /// </summary>
    REQ_ATTACKER_CAN_ATTACK = 40,
    /// <summary>
    /// <value> 要求目标攻击力大于  参数为攻击力</value>
    /// </summary>
    REQ_TARGET_MIN_ATTACK = 41, // 有参数
    /// <summary>
    /// <value> 要求可被英雄技能瞄准 </value>
    /// </summary>
    REQ_CAN_BE_TARGETED_BY_HERO_POWERS = 42,
    /// <summary>
    /// <value> 随从目标 </value>
    /// </summary>
    REQ_ENEMY_TARGET_NOT_IMMUNE = 43,
    /// <summary>
    /// <value> 要求全场面没有随从 </value>
    /// </summary>
    REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY = 44,
    /// <summary>
    /// <value> 对面场上的全部随从最少需要X个  参数最少随从数量</value>
    /// </summary>
    REQ_MINIMUM_TOTAL_MINIONS = 45,
    /// <summary>
    /// <value> 目标必须是嘲讽随从 </value>
    /// </summary>
    REQ_MUST_TARGET_TAUNTER = 46,
    /// <summary>
    /// <value> 目标只能是未受伤的随从 </value>
    /// </summary>
    REQ_UNDAMAGED_TARGET = 47,
    /// <summary>
    /// <value> 可以被战吼指定目标 </value>
    /// </summary>
    REQ_CAN_BE_TARGETED_BY_BATTLECRIES = 48,
    /// <summary>
    /// <value> 猎人英雄技能稳固射击 </value>
    /// </summary>
    REQ_STEADY_SHOT = 49,
    /// <summary>
    /// <value> 英雄技能 </value>
    /// </summary>
    REQ_MINION_OR_ENEMY_HERO = 50,
    /// <summary>
    /// <value> 有龙牌在手 </value>
    /// </summary>
    REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND = 51,
    /// <summary>
    /// <value> 目标只能是传说随从 </value>
    /// </summary>
    REQ_LEGENDARY_TARGET = 52,
    /// <summary>
    /// <value> 需要一个死亡的友方随从在当前回合死亡 </value>
    /// </summary>
    REQ_FRIENDLY_MINION_DIED_THIS_TURN = 53,
    /// <summary>
    /// <value> 需要一个死亡的友方随从 </value>
    /// </summary>
    REQ_FRIENDLY_MINION_DIED_THIS_GAME = 54,
    /// <summary>
    /// <value> 敌方英雄已装备武器 </value>
    /// </summary>
    REQ_ENEMY_WEAPON_EQUIPPED = 55,
    /// <summary>
    /// <value> 我方随从最少要X个可以使用 </value>
    /// </summary>
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS = 56,
    /// <summary>
    /// <value> 目标只能是战吼随从 </value>
    /// </summary>
    REQ_TARGET_WITH_BATTLECRY = 57,
    /// <summary>
    /// <value> 目标只能是亡语随从 </value>
    /// </summary>
    REQ_TARGET_WITH_DEATHRATTLE = 58,
    /// <summary>
    /// <value> 最少需要X个奥秘才有目标 </value>
    /// </summary>
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_SECRETS = 59,
    /// <summary>
    /// <value> 奥秘数未达到奥秘区域上限 </value>
    /// </summary>
    REQ_SECRET_ZONE_CAP_FOR_NON_SECRET = 60,
    /// <summary>
    /// <value> 目标精确费用？ 不确定 </value>
    /// </summary>
    REQ_TARGET_EXACT_COST = 61,
    /// <summary>
    /// <value> 目标只能是潜行随从 </value>
    /// </summary>
    REQ_STEALTHED_TARGET = 62,
    /// <summary>
    /// <value> 场上可以放一个随从且小于10个水晶 </value>
    /// </summary>
    REQ_MINION_SLOT_OR_MANA_CRYSTAL_SLOT = 63,
    /// <summary>
    /// <value> 最少需要x个任务 </value>
    /// </summary>
    REQ_MAX_QUESTS = 64,
    /// <summary>
    /// <value> 如果上个回合打过元素牌则有目标 </value>
    /// </summary>
    REQ_TARGET_IF_AVAILABE_AND_ELEMENTAL_PLAYED_LAST_TURN = 65,
    /// <summary>
    /// <value> 要求目标不是吸血鬼 冒险boss的技能</value>
    /// </summary>
    REQ_TARGET_NOT_VAMPIRE = 66,
    /// <summary>
    /// <value> 目标只会受到武器伤害 冒险boss的技能</value>
    /// </summary>
    REQ_TARGET_NOT_DAMAGEABLE_ONLY_BY_WEAPONS = 67,
    /// <summary>
    /// <value> 要求英雄技能未禁用 </value>
    /// </summary>
    REQ_NOT_DISABLED_HERO_POWER = 68,
    /// <summary>
    /// <value> 必须先使用其他卡牌 </value>
    /// 例如：暗影映像 - 每当你使用一张牌，变形成为该卡牌的复制。
    /// </summary>
    REQ_MUST_PLAY_OTHER_CARD_FIRST = 69,
    /// <summary>
    /// <value> 手牌未满 </value>
    /// </summary>
    REQ_HAND_NOT_FULL = 70,
    /// <summary>
    /// <value> 必须先使用其他卡牌 </value>
    /// <value> 例如：暗影映像 - 每当你使用一张牌，变形成为该卡牌的复制 </value>
    /// </summary>
    REQ_DRAG_TO_PLAY = 71,
    /// <summary>
    /// <value> 需要有目标2 </value>
    /// </summary>
    REQ_TARGET_TO_PLAY2 = 75,
    /// <summary>
    /// <value> 目标法术无自然属性 </value>
    /// </summary>
    REQ_TARGET_NO_NATURE = 77,
    REQ_LITERALLY_UNPLAYABLE,
    REQ_TARGET_IF_AVAILABLE_AND_HERO_HAS_ATTACK,
    REQ_FRIENDLY_MINION_OF_RACE_DIED_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_SPELLS_PLAYED_THIS_TURN,
    REQ_FRIENDLY_MINION_OF_RACE_IN_HAND,
    REQ_FRIENDLY_DEATHRATTLE_MINION_DIED_THIS_GAME = 86,
    REQ_FRIENDLY_REBORN_MINION_DIED_THIS_GAME = 89,
    REQ_MINION_DIED_THIS_GAME,
    REQ_BOARD_NOT_COMPLETELY_FULL = 92,
    REQ_TARGET_IF_AVAILABLE_AND_HAS_OVERLOADED_MANA,
    REQ_TARGET_IF_AVAILABLE_AND_HERO_ATTACKED_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_DRAWN_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_NOT_DRAWN_THIS_TURN,
    REQ_TARGET_NON_TRIPLED_MINION,
    REQ_BOUGHT_MINION_THIS_TURN,
    REQ_SOLD_MINION_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_PLAYER_HEALTH_CHANGED_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_SOUL_FRAGMENT_IN_DECK,
    REQ_DAMAGED_TARGET_UNLESS_COMBO,
    REQ_NOT_MINION_DORMANT,
    REQ_TARGET_NOT_UNTOUCHABLE,
    REQ_TARGET_IF_AVAILABLE_AND_BOUGHT_RACE_THIS_TURN,
    REQ_TARGET_IF_AVAILABLE_AND_SOLD_RACE_THIS_TURN,
    REQ_NOT_IN_COOLDOWN,
    REQ_TARGET_IS_MERC,
    REQ_TARGET_IS_NON_MERC,
    REQ_TWO_OF_A_KIND,
    REQ_HAS_OVERLOADED_MANA,
    REQ_LETTUCE_ABILITY_CANNOT_TARGET_OWNER,
    REQ_TARGET_NOT_HAVE_TAG = 116,
    REQ_TARGET_MUST_HAVE_TAG,
    REQ_TRADEABLE = 119,
    REQ_NOT_LEGENDARY_TARGET = 123,
    REQ_MINIMUM_TAVERN_TIER_LEVEL_TO_PLAY = 128,
    REQ_CARD_TAVERN_TIER_LEVEL_TO_PLAY,
    REQ_NOT_EXHAUSTED_LOCATION,
    REQ_LOCATION_TARGET,
    REQ_TARGET_SILVER_HAND_RECRUIT,
    REQ_MINIMUM_CORPSES,
    REQ_LOCATION_OR_MINION_TARGET,
    REQ_CAN_BE_TARGETED_BY_LOCATIONS,
    REQ_FORGE,
    REQ_TARGET_MAX_COST,
    REQ_HAS_PLAYED_SPELL_THIS_GAME,
    REQ_TARGET_IS_NON_TITAN = 141,
    REQ_BACON_DUO_PASSABLE,
    REQ_TARGET_EXACT_ATTACK,
    REQ_MINIMUM_NON_GOLDEN_ENEMY_MINIONS = 146,
}
