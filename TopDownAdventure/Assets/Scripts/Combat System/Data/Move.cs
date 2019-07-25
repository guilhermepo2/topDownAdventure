using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem.Data {
    [System.Serializable]
    [CreateAssetMenu(fileName = "Move", menuName = "Combat System/Move")]
    public class Move : ScriptableObject {

        public enum EDamageCategory {
            Physical = 1,
            Special = 2,
            Status = 3
        }

        public string moveName;
        public Pokemon.EPokemonType moveType;
        public EDamageCategory moveCategory;
        public int movePowerPoints;
        public int movePower;
        [Range(0, 100)]
        public int moveAccuracy;
    }
}