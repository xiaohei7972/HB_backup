import html
import os
import shutil
import time
import re
from datetime import datetime
from typing import Union
import json
from typing import *

"""
auther:xiaohei
time:2025/10/24
"""

print(f"{os.getcwd()}\\..\\Routines\\DefaultRoutine\\Silverfish\\Helpers\\CardHelper.cs")


# search_mode 搜索类型
# 1 英雄皮肤
# 2 英雄技能
# 3 幸运币
def secretsCardSkins(search_mode: int) -> Dict[int, Set[str]]:
    # 字典,储存英雄类型：卡牌id
    heroSkins: Dict[int, Set[str]] = {1: set(), 2: set(), 3: set(), 4: set(), 5: set(), 6: set(), 7: set(), 8: set(),
                                      9: set(), 10: set(), 14: set()}

    heroPower: Dict[int, Set[str]] = {1: set(), 2: set(), 3: set(), 4: set(), 5: set(), 6: set(), 7: set(), 8: set(),
                                      9: set(), 10: set(), 14: set()}

    coinCards: Dict[int, Set[str]] = {12: set()}
    # 储存英雄类型
    cardClass: int = 0
    # 卡牌id
    tempCardID: str = ""
    cardEnumID = 0
    # 扩展包id,需要英雄皮肤 17
    cardSet: int = 0
    # 卡牌类型,需要卡牌类型 3(英雄)
    cardType: int = 0
    # 可收藏
    collection: int = 0
    # 幸运币tag
    coinCard: int = 0
    # CardDefs.xml的路径
    file_cardDefsPath = f"{os.getcwd()}\\CardDefs.xml"

    with open(file_cardDefsPath, 'r', encoding='utf-8') as file_cardDefs:
        while 1:
            line = file_cardDefs.readline()
            if not line:
                break

            if "<Entity CardID=\"" in line:
                index1 = line.find("<Entity CardID=\"")
                index2 = line.find("\"", index1 + 16)
                if index1 == -1 or index2 == -1:
                    print(line)
                    exit(0)
                tempCardID = line[index1 + 16: index2]

            if "</Entity>" in line:
                # 储存英雄类型
                cardClass = 0
                # 卡牌id
                tempCardID = ""
                cardEnumID = 0
                # 扩展包id,需要英雄皮肤 17
                cardSet = 0
                # 卡牌类型,需要卡牌类型 3(英雄)
                cardType = 0
                # 可收藏
                collection = 0
                # 幸运币tag
                coinCard = 0


            # 遇到这类标签跳过
            if "<zhCN>" or "<enUs>" in line:
                if cardEnumID == 185 and cardEnumID == 184:
                    continue

            # 读卡牌关键字
            if "<Tag enumID=\"" in line:
                enumIDl = line.find("<Tag enumID=\"")
                # <Tag enumID="是13个字符所以要+13
                enumIDr = line.find("\"", enumIDl + 13)
                if enumIDl == -1 or enumIDr == -1:
                    print(line)
                    exit(0)
                cardEnumID = int(line[enumIDl + 13: enumIDr])
                # value="是七个字符所以要+7
                valuel = line.find("value=\"")
                valuer = line.find("\"", valuel + 7)
                if valuel == -1 or valuer == -1:
                    continue
                value = int(line[valuel + 7: valuer])
                match cardEnumID:
                    case 202:
                        cardType = value
                    case 183:
                        cardSet = value
                        # print(cardSet)
                    case 199:
                        cardClass = value
                    case 321:
                        collection = value
                    case 2088:
                        coinCard = value
                    case _:
                        continue

                match search_mode:
                    case 1:
                        # 英雄皮肤
                        if cardSet == 17 and cardType == 3 and collection == 1:
                            heroSkins[cardClass].add(tempCardID)
                            # cardSet = 0
                            # cardType = 0
                            # collection = 0
                    case 2:
                        # 英雄技能
                        if cardSet == 17 and cardType == 10:
                            print(f"{cardSet} cardSet")
                            print(f"{cardType} cardType")
                            print(f"{collection} collection")
                            heroPower[cardClass].add(tempCardID)
                            # cardSet = 0
                            # cardType = 0
                    case 3:
                        if coinCard == 1 and cardType == 5:
                            coinCards[12].add(tempCardID)
                            # coinCard = 0
                    case _:
                        break

        match search_mode:
            case 1:
                return heroSkins
            case 2:
                return heroPower
            case 3:
                return coinCards


#     print(f"职业 class {i}： {len(lists)}")
#     pass
print(f"英雄皮肤： + {secretsCardSkins(1)}")
print(f"英雄技能： + {secretsCardSkins(2)}")
print(f"幸运币： + {secretsCardSkins(3)}")


def handle_special_types(obj):
    # 处理 set 类型：转为 list
    if isinstance(obj, set):
        return list(obj)
    # 处理 datetime 类型：转为字符串（如 "2003-02-18"）
    if isinstance(obj, datetime):
        return obj.strftime('%Y-%m-%d')
    if isinstance(obj, dict):
        datas = {
            obj.keys(): list(obj.values())
        }
        return datas
    # 其他未处理类型：抛异常（可选）
    raise TypeError(f"不支持的类型：{type(obj)}")


# write_mode
# 1 英雄皮肤
# 2 英雄技能
# 3 幸运币
def writeJson(ls: Dict[int, Set[str]], write_mode: int) -> None:
    path: str = ""
    match write_mode:
        case 1:
            path = r'.\heroSkin.json'
        case 2:
            path = r'.\heroPower.json'
        case 3:
            path = r'.\coinCards.json'
        case _:
            print("错误的模式，准备退出")
            return
    with open(path, mode='w', encoding='utf-8') as f:
        json.dump(
            ls,
            f,
            separators=(',', ':'),  # 列表元素用 ',' 分隔，字典 key-value 用 ':' 分隔（无空格）
            ensure_ascii=False,  # 显示中文
            indent=4,  # 缩进 4 个空格（美化）
            default=handle_special_types  # 指定自定义处理函数
        )

    print(f"{path} 数据写入文件完成！")


if __name__ == '__main__':
    writeJson(secretsCardSkins(3), 3)
    # writeJson(secretsCardSkins(2), 2)
    # writeJson(secretsCardSkins(3), 3)

    # with open(f"{os.getcwd()}\\..\\Routines\\DefaultRoutine\\Silverfish\\Helpers\\CardHelper.cs", "r", encoding="utf-8") as CardHelper:
    #     print(CardHelper)
    #     while 1:
    #         line = CardHelper.readline()
    #         if not line:
    #             break
    #         print(line)
