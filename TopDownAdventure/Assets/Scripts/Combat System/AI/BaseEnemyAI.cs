using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.AI {
    public class BaseEnemyAI : MonoBehaviour {
        protected Pokemon m_pokemonReference;

        private void Start() {
            m_pokemonReference = GetComponent<Pokemon>();    
        }

        public virtual Data.Move SelectMovement() {
            return new Data.Move();
        }
    }

}
