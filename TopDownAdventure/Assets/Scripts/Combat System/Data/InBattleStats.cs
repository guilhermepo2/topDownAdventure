using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Data {
    [System.Serializable]
    public class InBattleStats {
        public int maxHp;
        public int attack;
        public int defense;
        public int specialAttack;
        public int specialDefense;
        public int speed;
        public int evasion;
        public int accuracy;
    }
}
