using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.AI {
    public class BaseEnemyAI : MonoBehaviour {
        protected BattlePokemon m_pokemonReference;

        private void Start() {
            m_pokemonReference = GetComponent<BattlePokemon>();    
        }

        public virtual Data.Move SelectMovement() {
            return new Data.Move();
        }
    }

}
