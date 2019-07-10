using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem.Data {
    [System.Serializable]
    [CreateAssetMenu(fileName = "Move", menuName = "Combat System/Move")]
    public class Move : ScriptableObject {
        public enum EMoveType {
            Normal = 1,
            Fighting = 2,
            Flying = 3,
            Poison = 4,
            Ground = 5,
            Rock = 6,
            Bug = 7,
            Ghost = 8,
            Steel = 9,
            Fire = 10,
            Water = 11,
            Grass = 12,
            Electric = 13,
            Psychic = 14,
            Ice = 15,
            Dragon = 16,
            Dark = 17,
            Fairy = 18
        }

        public enum EDamageCategory {
            Physical = 1,
            Special = 2,
            Status = 3
        }

        public string moveName;
        public EMoveType moveType;
        public EDamageCategory moveCategory;
        public int movePowerPoints;
        public int movePower;
        [Range(0, 100)]
        public int moveAccuracy;
    }
}