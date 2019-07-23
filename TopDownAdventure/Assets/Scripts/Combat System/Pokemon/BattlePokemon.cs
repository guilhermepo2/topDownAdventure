using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    public class BattlePokemon : MonoBehaviour {
        public enum EPokemonNature {
            Hardy = 1,
            Lonely = 2,
            Brave = 3,
            Adamant = 4,
            Naughty = 5,
            Bold = 6,
            Docile = 7,
            Relaxed = 8,
            Impish = 9,
            Lax = 10,
            Timid = 11,
            Hasty = 12,
            Serious = 13,
            Jolly = 14,
            Naive = 15,
            Modest = 16,
            Mild = 17,
            Quiet = 18,
            Bashful = 19,
            Rash = 20,
            Calm = 21,
            Gentle = 22,
            Sassy = 23,
            Careful = 24,
            Quirky = 25
        }

        [Header("Pokemon Info")]
        public Pokemon basePokemon;
        public string customPokemonName;
        public string PokemonName {
            get {
                if(customPokemonName != "") {
                    return customPokemonName;
                } else {
                    return basePokemon.pokemonName;
                }
            }
        }

        public EPokemonNature nature;
        public Data.Stats individualValues;
        public Data.Stats effortValues;
        public Data.Move[] pokemonMoves;

        [Header("Pokemon Status")]
        public int currentLevel;
        public int currentHealth;
        public int currentExperience;
        public bool isFainted;

        // Battle Stats
        private Data.InBattleStats m_inBattleStats;
        public Data.InBattleStats BattleStats { get { return m_inBattleStats; } }

        // [TO DO]
        // Having bool parameters is considered a bad practice, maybe I can change it?!
        public void CalculateStats(bool _updateMaxHealth) {
            m_inBattleStats = CombatFunctions.CalculateInBattleStats(basePokemon.baseStats, individualValues, effortValues, currentLevel);

            if(_updateMaxHealth) {
                currentHealth = m_inBattleStats.maxHp;
            }
        }
    }
}
