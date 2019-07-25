using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class EnemyEncounter : MonoBehaviour {
        public enum EMoveType {
            Random,
            PlayerBased
        }

        public float minimumWaitTimeToMove = 2.5f;
        public float maximumWaitTimeToMove = 5.0f;
        private Vector2[] possibleMovements = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        private float m_timeToMoveOneTile = 1.0f;
        private PlayerController m_playerReference;
        private EMoveType m_moveType;

        private void Start() {
            if(Random.value < 0.3f) {
                m_moveType = EMoveType.PlayerBased;
            } else {
                m_moveType = EMoveType.Random;
            }

            m_playerReference = FindObjectOfType<PlayerController>();
            StartCoroutine(MoveRoutine());   
        }

        public virtual void ProcessEncounter() {
            DependencyManager.Instance.Encounter.ProcessEncounter();
            Destroy(gameObject);
        }

        private IEnumerator MoveRoutine() {
            yield return new WaitForSeconds(Random.Range(minimumWaitTimeToMove, maximumWaitTimeToMove));
            
            if(m_moveType == EMoveType.Random) {
                StartCoroutine(MoveRoutine(possibleMovements.RandomOrDefault()));
            } else {
                Vector2 movement;
                Debug.Log($"Player Movement Move Routine: Player Position: {m_playerReference.transform.position} - Enemy Position: {transform.position}");

                if(Mathf.Round(m_playerReference.transform.position.x) > Mathf.Round(transform.position.x)) {
                    movement = Vector2.right;
                } else if(Mathf.Round(m_playerReference.transform.position.x) < Mathf.Round(transform.position.x)) {
                    movement = Vector2.left;
                } else if(Mathf.Round(m_playerReference.transform.position.y) > Mathf.Round(transform.position.y)) {
                    movement = Vector2.up;
                } else {
                    movement = Vector2.down;
                }

                StartCoroutine(MoveRoutine(movement));
            }
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
            } else {
                PlayerController collidedWithPlayerController = collision.gameObject.GetComponent<PlayerController>();

                if(collidedWithPlayerController != null) {
                    ProcessEncounter();
                }
            }

            StartCoroutine(MoveRoutine());
        }
    }
}
