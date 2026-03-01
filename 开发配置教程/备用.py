# encoding: utf-8
import requests
import re
import json
import os
from tqdm import tqdm

ClassName = {
    "DREAM": "梦境",
    "WHIZBANG": "威兹班"
}

TypeName = {
    "ENCHANTMENT": "附魔",
    "HERO_POWER": "英雄技能"
}

CardSetName = {
    "TB": "TB乱斗模式",
    "HERO_SKINS": "HERO英雄皮肤和技能",
    "THE_BARRENS": "BAR贫瘠之地的锤炼",
    'SCHOLOMANCE': "SCH通灵学园",
    'BASIC': "CORE基本",
    'BATTLEGROUNDS': "BGS酒馆战棋",
    'BOOMSDAY': "BOT砰砰计划",
    'BRM': "BRM黑石山的火焰",
    'BLACK_TEMPLE': "BT外域的灰烬",
    'DEMON_HUNTER_INITIATE': "BT恶魔猎手新兵",
    'YEAR_OF_THE_DRAGON': "DRG巨龙降临",
    "TGT": "AT冠军的试炼",
    'GANGS': "CFM龙争虎斗加基森",
    'CORE': "CORE基本",
    'CREDITS': "CREDITS暴雪制作人员",
    'EXPERT1': "EX经典",
    'HOF': "HOF荣誉室",
    'DALARAN': "DAL暗影崛起",
    'DARKMOON_FAIRE': "DMF疯狂的暗月马戏团",
    'DRAGONS': "DRG巨龙降临",
    'NAXX': "NAX纳克萨玛斯",
    'GILNEAS': "GIL女巫森林",
    'GVG': "GVG地精大战侏儒",
    'ICECROWN': "ICC冰封王座的骑士",
    'UNGORO': "UNG勇闯安戈洛",
    'LOOTAPALOOZA': "LOOT狗头人与地下世界",
    'KARA': "KAR卡拉赞之夜",
    'LOE': "LOE探险者协会",
    'OG': "OG上古之神的低语",
    'MISSIONS': "MISSIONS新手训练",
    'TROLL': "TRL拉斯塔哈的大乱斗",
    'TAVERNS_OF_TIME': "TOT时光酒馆",
    'ULDUM': "ULD奥丹姆奇兵",
    'VANILLA': "VAN经典模式"
}

file_list = requests.get("https://api.hearthstonejson.com/v1/").text
ver_list = re.findall("/v1/(\d+)/all/", file_list)
new_version = max(ver_list)
print(f"new_version: {new_version}")

print("loading global_json...")
global_json = requests.get('https://api.hearthstonejson.com/v1/strings/zhCN/GLOBAL.json').text
global_data = json.loads(global_json)
assert global_data is not None
for name in global_data:
    if "GLOBAL_CLASS_" in name:
        class_name = name.replace('GLOBAL_CLASS_', '')
        if class_name not in ClassName:
            ClassName[class_name] = global_data[name]
    if "GLOBAL_CARD_SET_" in name:
        card_set = name.replace('GLOBAL_CARD_SET_', '')
        if card_set not in CardSetName:
            CardSetName[card_set] = global_data[name]
    elif "GLOBAL_CARDTYPE_" in name:
        card_type = name.replace('GLOBAL_CARDTYPE_', '')
        if card_type not in TypeName:
            TypeName[card_type] = global_data[name]

print("loaded global_json successfully!")

print("loading card_data...")
cardJson_data = ""
cardJson_File = f'{new_version}.json'
if os.path.exists(cardJson_File):
    print("--file mode")
    with open(cardJson_File, "r", encoding='utf-8') as f:
        cardJson_data = f.read()
    assert cardJson_data != ""
else:
    cardJson_url = f'https://api.hearthstonejson.com/v1/{new_version}/all/cards.json'
    print(f"--online mode({cardJson_url})")
    cardJson_req = requests.get(cardJson_url, stream=True)
    cardJson_byte = b''
    pbar = tqdm(total=-1, unit='B', unit_scale=True)
    for chunk in cardJson_req.iter_content(chunk_size=1024):
        assert chunk != None
        cardJson_byte += chunk
        pbar.update(1024)
    pbar.close()
    cardJson_data = cardJson_byte.decode()
    assert cardJson_data != ""
    with open(cardJson_File, "w", encoding='utf-8') as f:
        f.write(cardJson_data)

cardData_temp = json.loads(cardJson_data)
assert cardData_temp is not None
print("loaded card_json successfully!")

cardData = {}
for c in cardData_temp:
    cardData[c['id']] = c

sim_path = os.path.join(os.getcwd(), "cards")
print("loading sim_data from", sim_path)
Sim_Context = []
Sim_CardID = []
Sim_text_idx = {}
Sim_id_idx = {}
for root, dirs, files in os.walk(sim_path):
    for file in files:
        card_id = file.replace('Sim_', '').replace('.cs', '')
        with open(os.path.join(root, file), "r", encoding='utf-8') as sim_file:
            sim_content = sim_file.read()
            if "public" not in sim_content:
                continue
            card_idx = len(Sim_Context)
            Sim_Context.append(sim_content)
            Sim_CardID.append(card_id)
            Sim_id_idx[card_id] = card_idx
            if card_id in cardData and 'text' in cardData[card_id]:
                Sim_text_idx[cardData[card_id]['text']['zhCN']] = card_idx
print("loaded " + str(len(Sim_Context)) + " old_sim_data successfully!")

enum_data = ""
if not os.path.exists('sim'):
    os.mkdir('sim')

print("Creating sim file and CardDB_cardIDEnum.cs")
for cardid, c in cardData.items():
    sim_data = ""
    basic = ""
    if 'type' in c and 'name' in c and c['type'] in TypeName:
        # create enum data
        type = TypeName[c['type']]
        name_cn = c['name']['zhCN']
        name = c['name']['enUS']
        card_set = c['set']
        cardtext_cn = ""
        cardtext = ""
        if 'cardClass' in c and 'cost' in c:
            if type == '法术':
                basic = f"{ClassName[c['cardClass']]} 费用：{c['cost']}"
            if type == '随从' and 'attack' in c and 'health' in c:
                basic = f"{ClassName[c['cardClass']]} 费用：{c['cost']} 攻击力：{c['attack']} 生命值：{c['health']}"
            if type == '武器' and 'attack' in c and 'durability' in c:
                basic = f"{ClassName[c['cardClass']]} 费用：{c['cost']} 攻击力：{c['attack']} 耐久度：{c['durability']}"
            if type == '英雄技能':
                basic = f"{ClassName[c['cardClass']]} 费用：{c['cost']}"
        enum_data += "/// <summary>\n"
        enum_data += f"/// <para>{type} {basic}</para>\n"
        enum_data += f"/// <para>{name}</para>\n"
        enum_data += f"/// <para>{name_cn}</para>\n"
        if 'text' in c:
            cardtext_cn = c['text']['zhCN'].replace('\n', ' ')
            cardtext = c['text']['enUS'].replace('\n', ' ')
            enum_data += f"/// <para>{cardtext}</para>\n"
            enum_data += f"/// <para>{cardtext_cn}</para>\n"
        enum_data += "/// </summary>\n"
        enum_data += f"{cardid} = {c['dbfId']},\n"
        # create sim data
        if type == '附魔':
            continue
        while True:
            if cardid in Sim_id_idx:
                sim_data = Sim_Context[Sim_id_idx[cardid]]
                break
            if 'text' in c and c['text']['zhCN'] in Sim_text_idx:
                idx = Sim_text_idx[c['text']['zhCN']]
                sim_id = Sim_CardID[idx]
                if cardid not in sim_id:
                    sim_data = Sim_Context[idx]
                    # fix
                    sim_data = sim_data.replace(f'class Sim_{sim_id}', f'class Sim_{cardid}')
                    break
            sim_data = "using System;\nusing System.Collections.Generic;\nusing System.Text;\n\n"
            sim_data += "namespace HREngine.Bots\n{\n"
            sim_data += f"\tclass Sim_{cardid} : SimTemplate //* {name_cn} {name}\n\t{{\n"
            if cardtext != "":
                sim_data += "\t\t//" + cardtext + "\n"
                sim_data += "\t\t//" + cardtext_cn + "\n"
            sim_data += "\t\t\n\t\t\n\t}\n}\n"
            break

        if card_set not in CardSetName:
            print("check CardSetName:", cardid)
            continue

        # write sim data
        sim_dir = f"sim\\{CardSetName[card_set]}"
        if not os.path.exists(sim_dir):
            os.mkdir(sim_dir)
        with open(sim_dir + "\\Sim_" + cardid + ".cs", 'w', encoding='utf-8') as sim:
            sim.write(sim_data)
print("Write sim_data to " + os.path.join(os.getcwd(), "sim") + " successfully!")
# write id_enum data
enum_path = os.path.join(os.getcwd(), "CardDB_cardIDEnum.cs")

enum_file_data = '''namespace HREngine.Bots
{
	partial class CardDB
	{
		public enum cardIDEnum
		{
			None,			
'''
enum_data = enum_data.split('\n')
for line in enum_data:
    enum_file_data += f"\t\t\t{line}\n"

enum_file_data += '''        }
	}
}
'''

with open(enum_path, "w", encoding="utf-8") as cardIDEnum:
    cardIDEnum.write(enum_file_data)
print("Write CardDB_cardIDEnum.cs to " + enum_path + " successfully!")
print("all finish")


import html
import os
import shutil
#输入：C:\\hs\\sim\\ 旧版的Sim文件
#输入：H:\CardDefs.xml 卡牌数据文件

#输出：C:\hs\CardDB_getSimCard.txt 生成的getSimCard文件
#输出：C:\hs\CardDB_cardIDEnum.txt 生成的cardIDEnum文件
#输出：C:\\hs\\newsim\\ 新版的Sim文件，保留旧版的Sim
TAG_CARDTYPE = {
        0: "INVALID",
        1: "GAME",
        2: "PLAYER",
        3: "英雄",
        4: "随从",
        5: "法术",
        6: "附魔",
        7: "武器",
        8: "ITEM",
        9: "TOKEN",
        10: "英雄技能",
        11: "BLANK",
        12: "GAME_MODE_BUTTON",
        22: "MOVE_MINION_HOVER_TARGET"
    }

TAG_CLASS = {
    0: "INVALID",
    1: "巫妖王",
    2: "德鲁伊",
    3: "猎人",
    4: "法师",
    5: "圣骑士",
    6: "潜行者",
    7: "牧师",
    8: "萨满祭司",
    9: "术士",
    10: "战士",
    11: "梦境",
    12: "中立",
    13: "威兹班",
    14: "恶魔猎手"
}
TAG_CARD_SET = {
    0:"INVALID",
    1:"TEST_TEMPORARY",
    2:"0001CORE基本",
    3:"0002EX经典",
    4:"0003HOF荣誉室",
    5:"MISSIONS新手训练",
    6:"DEMO",
    7:"NONE",
    8:"CHEAT",
    9:"BLANK",
    10:"DEBUG_SP",
    11:"PROMO",
    12:"0012NAX纳克萨玛斯",
    13:"0013GVG地精大战侏儒",
    14:"0014BRM黑石山的火焰",
    15:"0015AT冠军的试炼",
    16:"CREDITS暴雪制作人员",
    17:"0017英雄皮肤和技能",
    18:"0018TB乱斗模式",
    19:"SLUSH",
    20:"0020LOE探险者协会",
    21:"0021OG上古之神的低语",
    22:"OG_RESERVE",
    23:"0023KAR卡拉赞之夜",
    24:"KARA_RESERVE",
    25:"0025CFM龙争虎斗加基森",
    26:"GANGS_RESERVE",
    27:"0027UNG勇闯安戈洛",
    1001:"1001ICC冰封王座的骑士",
    1004:"1004LOOT狗头人与地下世界",
    1125:"1125GIL女巫森林",
    1127:"1127BOT砰砰计划",
    1129:"1129TRL拉斯塔哈的大乱斗",
    1130:"1130DAL暗影崛起",
    1158:"1158ULD奥丹姆奇兵" ,
    1347:"1347DRG巨龙降临",
    1439:"1439WE狂野限时回归" ,
    1403:"1403YOD迦拉克隆的觉醒",
    1453:"BATTLEGROUNDS" ,
    1414:"1414BT外域的灰烬" ,
    1463:"1463BT恶魔猎手新兵",
}

fs = open(r"C:\hs\CardDB_getSimCard.txt", 'w', encoding='utf-8')
fb = open(r"C:\hs\CardDB_cardIDEnum.txt", 'w', encoding='utf-8')
cardname = ""
cardnamecn = ""
cardtext = ""
cardtextcn = ""
cardID = ""
enumID = 0
numberID = 0
CARDTYPE = 0
CLASS = 0
CARD_SET = 0
COST = 0
ATK = 0
HEALTH = 0
DURABILITY = 0
with open(r"H:\CardDefs.xml", 'r', encoding='utf-8') as f:
    while 1:
        line = f.readline()
        if not line:
            break
        if r"</Entity>" in line:
            #BATTLEGROUNDS TB
            if CARD_SET != 1453 and CARD_SET != 1143:
                fb.write("/// <summary>\n")
                other = ""
                print(cardID)
                if TAG_CARDTYPE[CARDTYPE] == "法术" :
                    other += " " + TAG_CLASS[CLASS] + " 费用：" + str(COST)
                if TAG_CARDTYPE[CARDTYPE] == "随从" :
                    other += " " + TAG_CLASS[CLASS] + " 费用：" + str(COST) + " 攻击力：" + str(ATK) + " 生命值：" + str(HEALTH)
                if TAG_CARDTYPE[CARDTYPE] == "武器":
                    other += " " + TAG_CLASS[CLASS] + " 费用：" + str(COST) + " 攻击力：" + str(ATK) + " 耐久度：" + str(DURABILITY)
                if TAG_CARDTYPE[CARDTYPE] == "英雄技能":
                    other += " " + TAG_CLASS[CLASS] + " 费用："
                fb.write("/// <para>"+ TAG_CARDTYPE[CARDTYPE] + other +"</para>\n")
                fb.write("/// <para>"+ cardname +"</para>\n")
                fb.write("/// <para>"+ cardnamecn +"</para>\n")
                fb.write("/// <para>"+ cardtext +"</para>\n")
                fb.write("/// <para>"+ cardtextcn +"</para>\n")
                fb.write("/// </summary>\n")
                fb.write(cardID + " = " + str(numberID) + ",\n")
                if TAG_CARDTYPE[CARDTYPE] != "附魔":
                    fs.write("case cardIDEnum." + cardID + ": return new Sim_" + cardID + "();\n")
                    directory = "C:\\hs\\newsim\\" + TAG_CARD_SET[CARD_SET]
                    if not os.path.exists(directory):
                        os.mkdir(directory)
                    if os.path.exists("C:\\hs\\sim\\Sim_" + cardID + ".cs"):
                        shutil.move("C:\\hs\\sim\\Sim_" + cardID + ".cs", directory + "\\Sim_" + cardID + ".cs")
                    else:
                        with open(directory + "\\Sim_" + cardID + ".cs", 'w', encoding='utf-8') as sim:
                            sim.write("using System;\nusing System.Collections.Generic;\nusing System.Text;\n\n");
                            sim.write("namespace HREngine.Bots\n{\n");
                            sim.write("\tclass Sim_" + cardID +" : SimTemplate //* " + cardnamecn + " " + cardname + "\n\t{\n")
                            sim.write("\t\t//" + cardtext + "\n")
                            sim.write("\t\t//" + cardtextcn + "\n")
                            sim.write("\t\t\n\t\t\n\t}\n}\n")

                #print(cardname, cardnamecn, cardID, TAG_CARDTYPE[CARDTYPE], TAG_CLASS[CLASS], cardtext, cardtextcn)
            cardname = ""
            cardnamecn = ""
            cardtext = ""
            cardtextcn = ""
            cardID = ""
            enumID = 0
            numberID = 0
            CARDTYPE = 0
            CLASS = 0
            CARD_SET = 0
            COST = 0
            ATK = 0
            HEALTH = 0
            DURABILITY = 0

        if "<Entity CardID=\"" in line:
            index1 = line.find("<Entity CardID=\"")
            index2 = line.find("\"", index1 + 16)
            if index1 == -1 or index2 == -1:
                print(line)
                exit(0)
            cardID = line[index1 + 16: index2]
            index1 = line.find("ID=\"", index2)
            index2 = line.find("\"", index1 + 4)
            if index1 == -1 or index2 == -1:
                print(line)
                exit(0)
            numberID = line[index1 + 4: index2]

        if "<Tag enumID=\"" in line:
            enumIDl = line.find("<Tag enumID=\"")
            enumIDr = line.find("\"", enumIDl + 13)
            if enumIDl == -1 or enumIDr == -1:
                print(line)
                exit(0)
            enumID = int(line[enumIDl + 13: enumIDr])
            valuel = line.find("value=\"")
            valuer = line.find("\"", valuel + 7)
            if valuel == -1 or valuer == -1:
                continue
            value = int(line[valuel + 7: valuer])
            #CARDTYPE
            if enumID == 202:
                CARDTYPE = value
            #CLASS
            if enumID == 199:
                CLASS = value
            #CARD_SET
            if enumID == 183:
                CARD_SET = value
            #COST
            if enumID == 48:
                COST = value
            #HEALTH
            if enumID == 45:
                HEALTH = value
            #ATK
            if enumID == 47:
                ATK = value
            #DURABILITY
            if enumID == 187:
                DURABILITY = value
        if "<enUS>" in line:
            if enumID != 185 and enumID != 184:
                continue
            index1 = line.find("<enUS>")
            index2 = line.find("</enUS>", index1 + 6)
            text = ""
            while (index2 == -1):
                text += line[index1 + 6:-1]
                line = f.readline()
                index2 = line.find("</enUS>")
                index1 = -6
            text += line[index1 + 6: index2]

            if index1 == -1 or index2 == -1:
                print("错误：" + line)
                exit(0)
            if enumID == 185:
                cardname = html.unescape(text)
            if enumID == 184:
                cardtext = html.unescape(text)

        if "<zhCN>" in line:
            if enumID != 185 and enumID != 184:
                continue
            index1 = line.find("<zhCN>")
            index2 = line.find("</zhCN>", index1 + 6)

            text = ""
            while (index2 == -1):
                text += line[index1 + 6:-1]
                line = f.readline()
                index2 = line.find("</zhCN>")
                index1 = -6
            text += line[index1 + 6: index2]

            if index1 == -1 or index2 == -1:
                print("错误：" + line)
                exit(0)
            if enumID == 185:
                cardnamecn = html.unescape(text)
            if enumID == 184:
                cardtextcn = html.unescape(text)
[/collapse]

[collapse title="转移PlayRequirement到新的CardDefs文件"]
#输入：H:\CardDefsold.xml
#输出：H:\CardDefs.xml

import html
playreq = {}
playname = {}
with open(r"H:\CardDefsold.xml", 'r', encoding='utf-8') as f:
    for line in f:
        if r"</Entity>" in line:
            if cardID in playreq:
                playname[cardnamecn] = playreq[cardID]
            cardID = ""
            enumID = 0
            cardnamecn = ""
        if "<Entity CardID=\"" in line:
            index1 = line.find("<Entity CardID=\"")
            index2 = line.find("\"", index1 + 16)
            if index1 == -1 or index2 == -1:
                print(line)
                exit(0)
            cardID = line[index1 + 16: index2]

        if "<PlayRequirement" in line:
            if cardID not in playreq:
                playreq[cardID] = ""
            playreq[cardID] += line

        if "<Tag enumID=\"" in line:
            enumIDl = line.find("<Tag enumID=\"")
            enumIDr = line.find("\"", enumIDl + 13)
            if enumIDl == -1 or enumIDr == -1:
                print(line)
                exit(0)
            enumID = int(line[enumIDl + 13: enumIDr])
        if "<zhCN>" in line:
            if enumID != 185 and enumID != 184:
                continue
            index1 = line.find("<zhCN>")
            index2 = line.find("</zhCN>", index1 + 6)
            text = ""
            while (index2 == -1):
                text += line[index1 + 6:-1]
                line = f.readline()
                index2 = line.find("</zhCN>")
                index1 = -6
            text += line[index1 + 6: index2]
            if index1 == -1 or index2 == -1:
                print("错误：" + line)
                exit(0)
            if enumID == 185:
                cardnamecn = html.unescape(text)

#for key in playreq:
    #print(key)
    #print(playreq[key])
with open(r"H:\CardDefscopy.xml", 'w', encoding='utf-8') as fc:
    with open(r"H:\CardDefs.xml", 'r', encoding='utf-8') as f:
        for line in f:
            if r"</Entity>" in line:
                if cardID in playreq:
                    fc.write(playreq[cardID])
                else:
                    if cardnamecn in playname and CARDTYPE != 6 and CARDTYPE != 3:
                        print(cardID, cardnamecn)
                        print(playname[cardnamecn])
                        fc.write(playname[cardnamecn])
                cardID = ""
            if "<Entity CardID=\"" in line:
                index1 = line.find("<Entity CardID=\"")
                index2 = line.find("\"", index1 + 16)
                if index1 == -1 or index2 == -1:
                    print(line)
                    exit(0)
                cardID = line[index1 + 16: index2]
            fc.write(line)
            if "<Tag enumID=\"" in line:
                enumIDl = line.find("<Tag enumID=\"")
                enumIDr = line.find("\"", enumIDl + 13)
                if enumIDl == -1 or enumIDr == -1:
                    print(line)
                    exit(0)
                enumID = int(line[enumIDl + 13: enumIDr])
                valuel = line.find("value=\"")
                valuer = line.find("\"", valuel + 7)
                if valuel == -1 or valuer == -1:
                    continue
                value = int(line[valuel + 7: valuer])
                # CARDTYPE
                if enumID == 202:
                    CARDTYPE = value
            if "<zhCN>" in line:
                if enumID != 185 and enumID != 184:
                    continue
                index1 = line.find("<zhCN>")
                index2 = line.find("</zhCN>", index1 + 6)
                text = ""
                while (index2 == -1):
                    text += line[index1 + 6:-1]
                    line = f.readline()
                    fc.write(line)
                    index2 = line.find("</zhCN>")
                    index1 = -6
                text += line[index1 + 6: index2]
                if index1 == -1 or index2 == -1:
                    print("错误：" + line)
                    exit(0)
                if enumID == 185:
                    cardnamecn = html.unescape(text)
[/collapse]

[collapse title="从指定目录转移有内容的Sim卡到指定位置(和1搭配使用)"]
import os
import shutil

def readFilename(file_dir):
    for root, dirs, files in os.walk(file_dir):
        return files, dirs, root


​    
def findstring(pathfile):
    fp = open(pathfile, "r", encoding='UTF-8')  # 注意这里的打开文件编码方式
    strr = fp.read()
    # print strr.find("DoubleVec")
    if (strr.find("public") != -1):
        print('here?')
        return True
    return False


​    
def startfind(files, dirs, root):
    for ii in files:
        # print(ii)
        # if ii.endswith('.lua'):
        try:
            if (findstring(root + "\\" + ii)):
                shutil.copy(root + "\\" + ii, "C:\\hs\\sim\\" + ii)
                print(ii)
        except Exception as err:
            print(err)
            continue

    for jj in dirs:
        fi, di, ro = readFilename(root + "\\" + jj)
        startfind(fi, di, ro)


​    
if __name__ == '__main__':
    default_dir = u"H:\\Hearthbuddy\\Routines\\DefaultRoutine\\Silverfish\\cards"  # 设置默认打开目录
    file_path = default_dir  # th.expanduser(default_dir)))
    files, dirs, root = readFilename(file_path)
    startfind(files, dirs, root)
[/collapse]

[collapse title="读取卡牌代码，输出策略中的卡牌特征函数"]
import base64
import win32clipboard as w
import win32con

def setText(aString):#写入剪切板
    w.OpenClipboard()
    w.EmptyClipboard()
    w.SetClipboardData(win32con.CF_UNICODETEXT, aString)
    w.CloseClipboard()

def read_varint(data):
    shift = 0
    result = 0
    while True:
        temp = data.pop(0)
        result |= (temp & 0x7f) << shift
        shift += 7
        if (temp & 0x80) == 0:
            break
    return  result

CardDB = {}
with open(r"H:\CardDefs.xml", 'r', encoding='utf-8') as f:
        while True:
            line = f.readline()
            if not line:
                break
            if r"</Entity>" in line:
                CardDB[numberID] = cardID
                cardID = ""
                numberID = 0

            if "<Entity CardID=\"" in line:
                index1 = line.find("<Entity CardID=\"")
                index2 = line.find("\"", index1 + 16)
                if index1 == -1 or index2 == -1:
                    print(line)
                    exit(0)
                cardID = line[index1 + 16: index2]
                index1 = line.find("ID=\"", index2)
                index2 = line.find("\"", index1 + 4)
                if index1 == -1 or index2 == -1:
                    print(line)
                    exit(0)
                numberID = int(line[index1 + 4: index2])

while True:
    try:
        code = input()
        codelist = list(base64.b64decode(code))
    except:
        continue
    if len(codelist) == 0:
        continue
    reserve = read_varint(codelist)
    if reserve != 0:
        continue
    version = read_varint(codelist)
    if version != 1:
        continue
    format = read_varint(codelist)
    heroes = []
    num_heroes = read_varint(codelist)
    for i in range(num_heroes):
        card_id = read_varint(codelist)
        heroes.append(CardDB[card_id])
    cards = []
    num_cards_x1 = read_varint(codelist)
    for i in range(num_cards_x1):
        card_id = read_varint(codelist)
        cards.append([CardDB[card_id], 1])

    num_cards_x2 = read_varint(codelist)
    for i in range(num_cards_x2):
        card_id = read_varint(codelist)
        cards.append([CardDB[card_id], 2])

    num_cards_xn = read_varint(codelist)
    for i in range(num_cards_xn):
        card_id = read_varint(codelist)
        count = read_varint(codelist)
        cards.append({CardDB[card_id], count})
    str = "public List<CardDB.cardIDEnum> ArchetypeCardSet = new List<CardDB.cardIDEnum>\n{\n"
    cardslen = len(cards)
    for i in range(cardslen):
        str += "\tCardDB.cardIDEnum." + cards[i][0]
        if i != cardslen:
            str += ",\n"
    str += "};"
    print(str)
    setText(str)