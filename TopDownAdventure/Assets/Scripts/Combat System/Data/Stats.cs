using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem.Data {
    [System.Serializable]
    [CreateAssetMenu(fileName = "Stats", menuName = "Combat System/Stats")]
    public class Stats : ScriptableObject {
        public enum EStatsType {
            BaseStats,
            IV,
            EV
        }

        public EStatsType statsType;
        public int hp;
        public int attack;
        public int defense;
        public int specialAttack;
        public int specialDefense;
        public int speed;
    }
}