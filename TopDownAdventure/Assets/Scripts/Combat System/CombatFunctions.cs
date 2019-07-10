using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    public static class CombatFunctions {
        public static Data.InBattleStats CalculateInBattleStats(Data.Stats _baseStats, Data.Stats _iv, Data.Stats _ev, int _level) {
            Data.InBattleStats pokemonInBattleStats = new Data.InBattleStats();

            pokemonInBattleStats.maxHp = CalculateHP(_baseStats.hp, _iv.hp, _ev.hp, _level);
            pokemonInBattleStats.attack = CalculateSingleStat(_baseStats.attack, _iv.attack, _ev.attack, _level);
            pokemonInBattleStats.defense = CalculateSingleStat(_baseStats.defense, _iv.defense, _ev.defense, _level);
            pokemonInBattleStats.specialAttack = CalculateSingleStat(_baseStats.specialAttack, _iv.specialAttack, _ev.specialAttack, _level);
            pokemonInBattleStats.specialDefense = CalculateSingleStat(_baseStats.specialDefense, _iv.specialDefense, _ev.specialDefense, _level);
            pokemonInBattleStats.speed = CalculateSingleStat(_baseStats.speed, _iv.speed, _ev.speed, _level);
            pokemonInBattleStats.evasion = 100;
            pokemonInBattleStats.accuracy = 100;

            return pokemonInBattleStats;
        }

        public static int CalculateHP(int _baseHP, int _ivHP, int _evHP, int _level) {
            return Mathf.FloorToInt((((2 * _baseHP + _ivHP + (_evHP / 4)) * _level) / 100) + _level + 10);
        }

        public static int CalculateSingleStat(int _baseValue, int _ivValue, int _evValue, int _level) {
            return Mathf.FloorToInt( (((2 * _baseValue + _ivValue + (_evValue / 4)) * _level) / 100) + 5 );
        }
    }
}
