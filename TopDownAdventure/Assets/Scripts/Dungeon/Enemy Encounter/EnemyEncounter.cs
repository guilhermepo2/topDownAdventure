using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class EnemyEncounter : MonoBehaviour {
        public float minimumWaitTimeToMove = 2.5f;
        public float maximumWaitTimeToMove = 5.0f;
        private Vector2[] possibleMovements = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        private float m_timeToMoveOneTile = 1.0f;

        private void Start() {
            StartCoroutine(MoveRoutine());   
        }

        public virtual void ProcessEncounter() {
            DependencyManager.Instance.Encounter.ProcessEncounter();
            Destroy(gameObject);
        }

        private IEnumerator MoveRoutine() {
            yield return new WaitForSeconds(Random.Range(minimumWaitTimeToMove, maximumWaitTimeToMove));
            StartCoroutine(MoveRoutine(possibleMovements.RandomOrDefault()));
        }

        private IEnumerator MoveRoutine(Vector2 _direction) {
            Vector2 startingPosition = transform.position;
            Vector2 goalPosition = startingPosition + _direction;

            Collider2D collision = Physics2D.OverlapCircle(goalPosition, 0.25f);

            if (collision == null) {
                for(float i = 0; i < m_timeToMoveOneTile; i += Time.deltaTime) {
                    float t = Mathf.Clamp01(i / m_timeToMoveOneTile);
                    transform.position = Vector2.Lerp(startingPosition, goalPosition, t);
                    yield return null;
                }
            }

            StartCoroutine(MoveRoutine());
        }
    }
}
