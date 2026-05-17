using System;
using System.Collections.Generic;
using System.IO;

namespace HREngine.Bots
{
    public static class CardDefsCache
    {
        private const int FORMAT_VERSION = 1;
        private const uint MAGIC = 0x43415244; // "CARD"

        public static bool TryLoad(string path,
            out List<CardDB.Card> cardlist,
            out Dictionary<CardDB.cardIDEnum, CardDB.Card> cardidToCardList,
            out Dictionary<string, CardDB.Card> carddbfidToCardList,
            out Dictionary<CardDB.cardNameCN, CardDB.Card> cardNameCNToCardList,
            out Dictionary<CardDB.cardNameEN, CardDB.Card> cardNameENToCardList)
        {
            cardlist = null;
            cardidToCardList = null;
            carddbfidToCardList = null;
            cardNameCNToCardList = null;
            cardNameENToCardList = null;

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var r = new BinaryReader(fs))
                {
                    if (r.ReadUInt32() != MAGIC) return false;
                    if (r.ReadInt32() != FORMAT_VERSION) return false;

                    int cardCount = r.ReadInt32();
                    var cards = new List<CardDB.Card>(cardCount);
                    for (int i = 0; i < cardCount; i++)
                    {
                        cards.Add(ReadCard(r));
                    }

                    cardidToCardList = ReadCardIDEnumDict(r, cards);
                    carddbfidToCardList = ReadStringKeyDict(r, cards);
                    cardNameCNToCardList = ReadCardNameCNDict(r, cards);
                    cardNameENToCardList = ReadCardNameENDict(r, cards);

                    cardlist = cards;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void Save(string path,
            List<CardDB.Card> cardlist,
            Dictionary<CardDB.cardIDEnum, CardDB.Card> cardidToCardList,
            Dictionary<string, CardDB.Card> carddbfidToCardList,
            Dictionary<CardDB.cardNameCN, CardDB.Card> cardNameCNToCardList,
            Dictionary<CardDB.cardNameEN, CardDB.Card> cardNameENToCardList)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var w = new BinaryWriter(fs))
            {
                w.Write(MAGIC);
                w.Write(FORMAT_VERSION);

                var indexMap = new Dictionary<CardDB.Card, int>(cardlist.Count);
                for (int i = 0; i < cardlist.Count; i++)
                {
                    indexMap[cardlist[i]] = i;
                }

                w.Write(cardlist.Count);
                foreach (var card in cardlist)
                {
                    WriteCard(w, card);
                }

                WriteIntKeyDict(w, cardidToCardList, indexMap);
                WriteStringKeyDict(w, carddbfidToCardList, indexMap);
                WriteIntKeyDict(w, cardNameCNToCardList, indexMap);
                WriteIntKeyDict(w, cardNameENToCardList, indexMap);
            }
        }

        #region Dictionary serialization helpers

        private static void WriteIntKeyDict(BinaryWriter w, Dictionary<CardDB.cardIDEnum, CardDB.Card> dict, Dictionary<CardDB.Card, int> indexMap)
        {
            w.Write(dict.Count);
            foreach (var kvp in dict)
            {
                w.Write((int)kvp.Key);
                w.Write(indexMap[kvp.Value]);
            }
        }

        private static void WriteIntKeyDict(BinaryWriter w, Dictionary<CardDB.cardNameCN, CardDB.Card> dict, Dictionary<CardDB.Card, int> indexMap)
        {
            w.Write(dict.Count);
            foreach (var kvp in dict)
            {
                w.Write((int)kvp.Key);
                w.Write(indexMap[kvp.Value]);
            }
        }

        private static void WriteIntKeyDict(BinaryWriter w, Dictionary<CardDB.cardNameEN, CardDB.Card> dict, Dictionary<CardDB.Card, int> indexMap)
        {
            w.Write(dict.Count);
            foreach (var kvp in dict)
            {
                w.Write((int)kvp.Key);
                w.Write(indexMap[kvp.Value]);
            }
        }

        private static void WriteStringKeyDict(BinaryWriter w, Dictionary<string, CardDB.Card> dict, Dictionary<CardDB.Card, int> indexMap)
        {
            w.Write(dict.Count);
            foreach (var kvp in dict)
            {
                w.Write(kvp.Key);
                w.Write(indexMap[kvp.Value]);
            }
        }

        private static Dictionary<CardDB.cardIDEnum, CardDB.Card> ReadCardIDEnumDict(BinaryReader r, List<CardDB.Card> cards)
        {
            int count = r.ReadInt32();
            var dict = new Dictionary<CardDB.cardIDEnum, CardDB.Card>(count);
            for (int i = 0; i < count; i++)
            {
                int keyRaw = r.ReadInt32();
                int idx = r.ReadInt32();
                dict[(CardDB.cardIDEnum)keyRaw] = cards[idx];
            }
            return dict;
        }

        private static Dictionary<CardDB.cardNameCN, CardDB.Card> ReadCardNameCNDict(BinaryReader r, List<CardDB.Card> cards)
        {
            int count = r.ReadInt32();
            var dict = new Dictionary<CardDB.cardNameCN, CardDB.Card>(count);
            for (int i = 0; i < count; i++)
            {
                int keyRaw = r.ReadInt32();
                int idx = r.ReadInt32();
                dict[(CardDB.cardNameCN)keyRaw] = cards[idx];
            }
            return dict;
        }

        private static Dictionary<CardDB.cardNameEN, CardDB.Card> ReadCardNameENDict(BinaryReader r, List<CardDB.Card> cards)
        {
            int count = r.ReadInt32();
            var dict = new Dictionary<CardDB.cardNameEN, CardDB.Card>(count);
            for (int i = 0; i < count; i++)
            {
                int keyRaw = r.ReadInt32();
                int idx = r.ReadInt32();
                dict[(CardDB.cardNameEN)keyRaw] = cards[idx];
            }
            return dict;
        }

        private static Dictionary<string, CardDB.Card> ReadStringKeyDict(BinaryReader r, List<CardDB.Card> cards)
        {
            int count = r.ReadInt32();
            var dict = new Dictionary<string, CardDB.Card>(count);
            for (int i = 0; i < count; i++)
            {
                string key = r.ReadString();
                int idx = r.ReadInt32();
                dict[key] = cards[idx];
            }
            return dict;
        }

        #endregion

        #region Card serialization

        private static void WriteCard(BinaryWriter w, CardDB.Card c)
        {
            // Enum fields (write as int)
            w.Write((int)c.cardIDenum);
            w.Write((int)c.nameEN);
            w.Write((int)c.nameCN);
            w.Write((int)c.race);
            w.Write((int)c.type);
            w.Write((int)c.SpellSchool);

            // String fields
            w.Write(c.dbfId ?? "");
            w.Write(c.textCN ?? "");
            w.Write(c.TreatItAsTheSameCard ?? "");

            // Int fields (core stats)
            w.Write(c.rarity);
            w.Write(c.cost);
            w.Write(c.Attack);
            w.Write(c.Health);
            w.Write(c.Class);
            w.Write(c.Durability);
            w.Write(c.dormant);
            w.Write(c.overload);
            w.Write(c.spellpowervalue);
            w.Write(c.targetPriority);
            w.Write(c.TAG_SCRIPT_DATA_NUM_1);
            w.Write(c.TAG_SCRIPT_DATA_NUM_2);
            w.Write(c.TAG_SCRIPT_DATA_NUM_3);
            w.Write(c.TAG_SCRIPT_DATA_NUM_4);
            w.Write(c.DECK_ACTION_COST);
            w.Write(c.CooldownTurn);
            w.Write(c.InfuseNum);
            w.Write(c.Manathirst);
            w.Write(c.ForgeCost);
            w.Write(c.TradeCost);
            w.Write(c.count);

            // Int fields (armor/hero/class)
            w.Write(c.armor);
            w.Write(c.heroPower);
            w.Write(c.upgradedHeroPower);
            w.Write(c.KeepHeroClass);
            w.Write(c.Miniaturize);
            w.Write(c.Gigantity);
            w.Write(c.CollectionRelatedCardDataBaseId);
            w.Write(c.CardAlternateCost);
            w.Write(c.Objective);
            w.Write(c.ObjectiveAura);
            w.Write(c.Sigil);
            w.Write(c.costBlood);
            w.Write(c.costFrost);
            w.Write(c.costUnholy);
            w.Write(c.CastsWhenDrawn);
            w.Write(c.InteractableObjectCost);
            w.Write(c.UsesCharges);
            w.Write(c.TriggerVisual);
            w.Write(c.MODULAR_ENTITY_PART_1);
            w.Write(c.MODULAR_ENTITY_PART_2);
            w.Write(c.magneticToRace);
            w.Write(c.MultipleClasses);

            // Int fields (play requirements)
            w.Write(c.needEmptyPlacesForPlaying);
            w.Write(c.needWithMinAttackValueOf);
            w.Write(c.needWithMaxAttackValueOf);
            w.Write(c.needWithExactAttackValueOf);
            w.Write(c.needWithMinimumCorpeses);
            w.Write(c.needRaceForPlaying);
            w.Write(c.needRaceInHand);
            w.Write((int)c.needTagForPlaying);
            w.Write(c.needMinNumberOfEnemy);
            w.Write(c.needMinTotalMinions);
            w.Write(c.needMinOwnMinions);
            w.Write(c.needMinionsCapIfAvailable);
            w.Write(c.needControlaSecret);

            // Bool fields (core keywords)
            w.Write(c.tank);
            w.Write(c.Shield);
            w.Write(c.Charge);
            w.Write(c.Rush);
            w.Write(c.Stealth);
            w.Write(c.Elusive);
            w.Write(c.windfury);
            w.Write(c.megaWindfury);
            w.Write(c.poisonous);
            w.Write(c.lifesteal);
            w.Write(c.reborn);
            w.Write(c.HonorableKill);
            w.Write(c.Overkill);
            w.Write(c.Spellburst);
            w.Write(c.Frenzy);
            w.Write(c.battlecry);
            w.Write(c.choice);
            w.Write(c.deathrattle);
            w.Write(c.Silence);
            w.Write(c.discover);
            w.Write(c.oneTurnEffect);
            w.Write(c.Enrage);
            w.Write(c.Aura);
            w.Write(c.Elite);
            w.Write(c.Combo);
            w.Write(c.immuneWhileAttacking);
            w.Write(c.untouchable);
            w.Write(c.Freeze);
            w.Write(c.AdjacentBuff);
            w.Write(c.Secret);
            w.Write(c.Nature);
            w.Write(c.Quest);
            w.Write(c.Questline);
            w.Write(c.Morph);
            w.Write(c.Spellpower);
            w.Write(c.Inspire);
            w.Write(c.Outcast);
            w.Write(c.Corrupted);
            w.Write(c.Corrupt);
            w.Write(c.CantAttack);
            w.Write(c.Collectable);
            w.Write(c.Tradeable);
            w.Write(c.isToken);
            w.Write(c.isSpecialMinion);

            // Bool fields (modern keywords)
            w.Write(c.Dredge);
            w.Write(c.Infuse);
            w.Write(c.Infused);
            w.Write(c.Finale);
            w.Write(c.Overheal);
            w.Write(c.Titan);
            w.Write(c.TitanAbilityUsed1);
            w.Write(c.TitanAbilityUsed2);
            w.Write(c.TitanAbilityUsed3);
            w.Write(c.Forge);
            w.Write(c.Forged);
            w.Write(c.Quickdraw);
            w.Write(c.Excavate);
            w.Write(c.Echo);
            w.Write(c.nonKeywordEcho);
            w.Write(c.Twinspell);
            w.Write(c.Temporary);
            w.Write(c.HideCost);
            w.Write(c.ShiftingSpell);
            w.Write(c.CanTargetCardsInHand);
            w.Write(c.InteractableObject);

            // Bool fields (card classifications)
            w.Write(c.SilverHandRecruit);
            w.Write(c.SI_7);
            w.Write(c.markOfEvil);
            w.Write(c.Treant);
            w.Write(c.Ancient);
            w.Write(c.IMP);
            w.Write(c.Whelp);
            w.Write(c.StarshipPiece);
            w.Write(c.Starship);
            w.Write(c.Crewmate);
            w.Write(c.Zerg);
            w.Write(c.Terran);
            w.Write(c.Protoss);
            w.Write(c.Wipe);
            w.Write(c.Rafaam);
            w.Write(c.Ysera);

            // Races list
            if (c.races == null)
            {
                w.Write(0);
            }
            else
            {
                w.Write(c.races.Count);
                for (int i = 0; i < c.races.Count; i++)
                {
                    w.Write((int)c.races[i]);
                }
            }
        }

        private static CardDB.Card ReadCard(BinaryReader r)
        {
            var c = new CardDB.Card();

            // Enum fields
            c.cardIDenum = (CardDB.cardIDEnum)r.ReadInt32();
            c.nameEN = (CardDB.cardNameEN)r.ReadInt32();
            c.nameCN = (CardDB.cardNameCN)r.ReadInt32();
            c.race = (CardDB.Race)r.ReadInt32();
            c.type = (CardDB.cardtype)r.ReadInt32();
            c.SpellSchool = (CardDB.SpellSchool)r.ReadInt32();

            // String fields
            c.dbfId = r.ReadString();
            c.textCN = r.ReadString();
            c.TreatItAsTheSameCard = r.ReadString();

            // Int fields (core stats)
            c.rarity = r.ReadInt32();
            c.cost = r.ReadInt32();
            c.Attack = r.ReadInt32();
            c.Health = r.ReadInt32();
            c.Class = r.ReadInt32();
            c.Durability = r.ReadInt32();
            c.dormant = r.ReadInt32();
            c.overload = r.ReadInt32();
            c.spellpowervalue = r.ReadInt32();
            c.targetPriority = r.ReadInt32();
            c.TAG_SCRIPT_DATA_NUM_1 = r.ReadInt32();
            c.TAG_SCRIPT_DATA_NUM_2 = r.ReadInt32();
            c.TAG_SCRIPT_DATA_NUM_3 = r.ReadInt32();
            c.TAG_SCRIPT_DATA_NUM_4 = r.ReadInt32();
            c.DECK_ACTION_COST = r.ReadInt32();
            c.CooldownTurn = r.ReadInt32();
            c.InfuseNum = r.ReadInt32();
            c.Manathirst = r.ReadInt32();
            c.ForgeCost = r.ReadInt32();
            c.TradeCost = r.ReadInt32();
            c.count = r.ReadInt32();

            // Int fields (armor/hero/class)
            c.armor = r.ReadInt32();
            c.heroPower = r.ReadInt32();
            c.upgradedHeroPower = r.ReadInt32();
            c.KeepHeroClass = r.ReadInt32();
            c.Miniaturize = r.ReadInt32();
            c.Gigantity = r.ReadInt32();
            c.CollectionRelatedCardDataBaseId = r.ReadInt32();
            c.CardAlternateCost = r.ReadInt32();
            c.Objective = r.ReadInt32();
            c.ObjectiveAura = r.ReadInt32();
            c.Sigil = r.ReadInt32();
            c.costBlood = r.ReadInt32();
            c.costFrost = r.ReadInt32();
            c.costUnholy = r.ReadInt32();
            c.CastsWhenDrawn = r.ReadInt32();
            c.InteractableObjectCost = r.ReadInt32();
            c.UsesCharges = r.ReadInt32();
            c.TriggerVisual = r.ReadInt32();
            c.MODULAR_ENTITY_PART_1 = r.ReadInt32();
            c.MODULAR_ENTITY_PART_2 = r.ReadInt32();
            c.magneticToRace = r.ReadInt32();
            c.MultipleClasses = r.ReadInt32();

            // Int fields (play requirements)
            c.needEmptyPlacesForPlaying = r.ReadInt32();
            c.needWithMinAttackValueOf = r.ReadInt32();
            c.needWithMaxAttackValueOf = r.ReadInt32();
            c.needWithExactAttackValueOf = r.ReadInt32();
            c.needWithMinimumCorpeses = r.ReadInt32();
            c.needRaceForPlaying = r.ReadInt32();
            c.needRaceInHand = r.ReadInt32();
            c.needTagForPlaying = (CardDB.Specialtags)r.ReadInt32();
            c.needMinNumberOfEnemy = r.ReadInt32();
            c.needMinTotalMinions = r.ReadInt32();
            c.needMinOwnMinions = r.ReadInt32();
            c.needMinionsCapIfAvailable = r.ReadInt32();
            c.needControlaSecret = r.ReadInt32();

            // Bool fields (core keywords)
            c.tank = r.ReadBoolean();
            c.Shield = r.ReadBoolean();
            c.Charge = r.ReadBoolean();
            c.Rush = r.ReadBoolean();
            c.Stealth = r.ReadBoolean();
            c.Elusive = r.ReadBoolean();
            c.windfury = r.ReadBoolean();
            c.megaWindfury = r.ReadBoolean();
            c.poisonous = r.ReadBoolean();
            c.lifesteal = r.ReadBoolean();
            c.reborn = r.ReadBoolean();
            c.HonorableKill = r.ReadBoolean();
            c.Overkill = r.ReadBoolean();
            c.Spellburst = r.ReadBoolean();
            c.Frenzy = r.ReadBoolean();
            c.battlecry = r.ReadBoolean();
            c.choice = r.ReadBoolean();
            c.deathrattle = r.ReadBoolean();
            c.Silence = r.ReadBoolean();
            c.discover = r.ReadBoolean();
            c.oneTurnEffect = r.ReadBoolean();
            c.Enrage = r.ReadBoolean();
            c.Aura = r.ReadBoolean();
            c.Elite = r.ReadBoolean();
            c.Combo = r.ReadBoolean();
            c.immuneWhileAttacking = r.ReadBoolean();
            c.untouchable = r.ReadBoolean();
            c.Freeze = r.ReadBoolean();
            c.AdjacentBuff = r.ReadBoolean();
            c.Secret = r.ReadBoolean();
            c.Nature = r.ReadBoolean();
            c.Quest = r.ReadBoolean();
            c.Questline = r.ReadBoolean();
            c.Morph = r.ReadBoolean();
            c.Spellpower = r.ReadBoolean();
            c.Inspire = r.ReadBoolean();
            c.Outcast = r.ReadBoolean();
            c.Corrupted = r.ReadBoolean();
            c.Corrupt = r.ReadBoolean();
            c.CantAttack = r.ReadBoolean();
            c.Collectable = r.ReadBoolean();
            c.Tradeable = r.ReadBoolean();
            c.isToken = r.ReadBoolean();
            c.isSpecialMinion = r.ReadBoolean();

            // Bool fields (modern keywords)
            c.Dredge = r.ReadBoolean();
            c.Infuse = r.ReadBoolean();
            c.Infused = r.ReadBoolean();
            c.Finale = r.ReadBoolean();
            c.Overheal = r.ReadBoolean();
            c.Titan = r.ReadBoolean();
            c.TitanAbilityUsed1 = r.ReadBoolean();
            c.TitanAbilityUsed2 = r.ReadBoolean();
            c.TitanAbilityUsed3 = r.ReadBoolean();
            c.Forge = r.ReadBoolean();
            c.Forged = r.ReadBoolean();
            c.Quickdraw = r.ReadBoolean();
            c.Excavate = r.ReadBoolean();
            c.Echo = r.ReadBoolean();
            c.nonKeywordEcho = r.ReadBoolean();
            c.Twinspell = r.ReadBoolean();
            c.Temporary = r.ReadBoolean();
            c.HideCost = r.ReadBoolean();
            c.ShiftingSpell = r.ReadBoolean();
            c.CanTargetCardsInHand = r.ReadBoolean();
            c.InteractableObject = r.ReadBoolean();

            // Bool fields (card classifications)
            c.SilverHandRecruit = r.ReadBoolean();
            c.SI_7 = r.ReadBoolean();
            c.markOfEvil = r.ReadBoolean();
            c.Treant = r.ReadBoolean();
            c.Ancient = r.ReadBoolean();
            c.IMP = r.ReadBoolean();
            c.Whelp = r.ReadBoolean();
            c.StarshipPiece = r.ReadBoolean();
            c.Starship = r.ReadBoolean();
            c.Crewmate = r.ReadBoolean();
            c.Zerg = r.ReadBoolean();
            c.Terran = r.ReadBoolean();
            c.Protoss = r.ReadBoolean();
            c.Wipe = r.ReadBoolean();
            c.Rafaam = r.ReadBoolean();
            c.Ysera = r.ReadBoolean();

            // Races list
            int raceCount = r.ReadInt32();
            c.races = new List<CardDB.Race>(raceCount);
            for (int i = 0; i < raceCount; i++)
            {
                c.races.Add((CardDB.Race)r.ReadInt32());
            }

            return c;
        }

        #endregion
    }
}
