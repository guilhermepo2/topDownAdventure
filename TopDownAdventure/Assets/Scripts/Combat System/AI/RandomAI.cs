using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CombatSystem.AI {
    public class RandomAI : BaseEnemyAI {
        public override Data.Move SelectMovement() {
            Data.Move[] validMoves = m_pokemonReference.pokemonMoves.Where((move) => {
                return move != null;
            }).ToArray();

            Debug.Log($"Random Enemy AI, returning a random movement from a pool of {validMoves.Length} moves");

            return validMoves.RandomOrDefault();
        }
    }

}
