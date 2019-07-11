using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    public class Pokemon : MonoBehaviour {
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

        public Sprite topSidePokemonSprite;
        public Sprite bottomSidePokemonSprite;
        public string pokemonName;
        public Data.Stats baseStats;
        public Data.Stats individualValues;
        public Data.Stats effortValues;
        public EPokemonNature nature;
        public Data.Move[] pokemonMoves;

        public int currentLevel;
        public int currentHealth;
        public int currentExperience;

        private Data.InBattleStats m_inBattleStats;
        public Data.InBattleStats BattleStats { get { return m_inBattleStats;  } }

        private void Awake() {
            m_inBattleStats = CombatFunctions.CalculateInBattleStats(baseStats, individualValues, effortValues, currentLevel);

            currentHealth = m_inBattleStats.maxHp;
        }
    }
}
