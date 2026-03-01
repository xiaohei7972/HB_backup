import html
import os
import shutil
import time
import re
from datetime import datetime
from typing import Union
import json

"""
auther:xiaohei
time:2025/10/24
"""

# 字典,储存英雄类型：卡牌id
# heroSkins: dict[int, list[str]] = {1: [], 2: [], 3: [], 4: [], 5: [], 6: [], 7: [], 8: [], 9: [], 10: [], 14: []}
heroSkins: dict[int, set[str]] = {1: set(), 2: set(), 3: set(), 4: set(), 5: set(), 6: set(), 7: set(), 8: set(),
                                  9: set(), 10: set(), 14: set()}
# 卡牌id
cardEnumID: str = ""
# 储存英雄类型
cardClass: int = 0
tagEnumID = 0
# 扩展包id,需要英雄皮肤 17
cardSet = 0
# 卡牌类型,需要卡牌类型 3(英雄)
cardType = 0

collection = 0
file_cardDefsPath = f"{os.getcwd()}\\CardDefs.xml"
# print(file_cardDefsPath)

###
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
            cardEnumID = line[index1 + 16: index2]
        #
        # if "</Entity>" in line:
        #     print(line)
        #     collection = 0
        #     continue

        # 遇到这类标签跳过
        if "<zhCN>" or "<enUs>" in line:
            if tagEnumID == 185 and tagEnumID == 184:
                continue

        # 读卡牌关键字
        if "<Tag enumID=\"" in line:
            enumIDl = line.find("<Tag enumID=\"")
            # <Tag enumID="是13个字符所以要+13
            enumIDr = line.find("\"", enumIDl + 13)
            if enumIDl == -1 or enumIDr == -1:
                print(line)
                exit(0)
            tagEnumID = int(line[enumIDl + 13: enumIDr])
            # value="是七个字符所以要+7
            valuel = line.find("value=\"")
            valuer = line.find("\"", valuel + 7)
            if valuel == -1 or valuer == -1:
                continue
            value = int(line[valuel + 7: valuer])
            match tagEnumID:
                case 202:
                    # if value != 17:
                    #     cardSet = 0
                    cardType = value
                    break
                case 183:
                    cardSet = value
                    break
                    # print(cardSet)
                case 199:
                    cardClass = value
                    break
                case 321:
                    print(value)
                    # if value != 1:
                    #     continue
                    collection = value
                    break
                case _:
                    break
        print(f"cardSet{cardSet} cardType{cardType}")
            # if cardSet == 17 and cardType == 3 and collection == 1:
        if cardSet == 17 and cardType == 3:
                # heroSkins[cardClass].append(cardEnumID)
            heroSkins[cardClass].add(cardEnumID)

for i, lists in heroSkins.items():
    print(f"职业 class {i}： {len(lists)}")
    pass


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


def writerJson(ls: dict[int, list[str]]):
    with open(r'.\heroSkins.json', mode='w', encoding='utf-8') as f:
        json.dump(
            ls,
            f,
            separators=(',', ':'),  # 列表元素用 ',' 分隔，字典 key-value 用 ':' 分隔（无空格）
            ensure_ascii=False,  # 显示中文
            indent=4,  # 缩进 4 个空格（美化）
            default=handle_special_types  # 指定自定义处理函数
        )
    print("JSON 数据写入文件完成！")


# writerJson(heroSkins)

print(type(heroSkins.items()))
