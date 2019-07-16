using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public enum EPlayerState {
        Idle,
        Moving,
        InBattle,
    }

    public enum EDirection {
        Up,
        Right,
        Down,
        Left,
    }

    [Header("Movement Configuration")]
    public float timeToWalkOneTile = 0.5f;


    private EPlayerState m_currentPlayerState;
    private EDirection m_directionPlayerIsFacing;
    private Vector2 m_movement;

    // Animation Related
    private Animator m_animator;
    private const string STOPPED_UP_ANIMATION = "StoppedUp";
    private const string STOPPED_SIDE_ANIMATION = "StoppedSide";
    private const string STOPPED_DOWN_ANIMATION = "StoppedDown";
    private const string UP_ANIMATION = "WalkingUp";
    private const string SIDE_ANIMATION = "WalkingSide";
    private const string DOWN_ANIMATION = "WalkingDown";

    // Actions and Events
    public event Action PlayerMoved;

    private void Start() {
        m_animator = GetComponentInChildren<Animator>();
        m_directionPlayerIsFacing = EDirection.Down;
    }

    void Update() {
        if(Input.GetKey(KeyCode.W)) {
            m_movement = Vector2.up;
        } else if(Input.GetKey(KeyCode.S)) {
            m_movement = Vector2.down;
        } else if(Input.GetKey(KeyCode.A)) {
            m_movement = Vector2.left;
        } else if(Input.GetKey(KeyCode.D)) {
            m_movement = Vector2.right;
        } else {
            m_movement = Vector2.zero;
        }

        
        if(m_movement != Vector2.zero && m_currentPlayerState == EPlayerState.Idle) {
            MoveOneTile(m_movement);
        } else if(m_movement == Vector2.zero && m_currentPlayerState == EPlayerState.Idle) {
            StopAnimation();
        }

        if(Input.GetKeyDown(KeyCode.Return)) {
            Vector2 itemDirection = Vector2.up;

            switch(m_directionPlayerIsFacing) {
                case EDirection.Up:
                    itemDirection = Vector2.up;
                    break;
                case EDirection.Right:
                    itemDirection = Vector2.right;
                    break;
                case EDirection.Down:
                    itemDirection = Vector2.down;
                    break;
                case EDirection.Left:
                    itemDirection = Vector2.left;
                    break;
            }

            TryToGetItem(itemDirection);
        }
    }

    private void MoveOneTile(Vector2 _direction) {
        Vector2 movementPosition = transform.position;
        movementPosition = movementPosition + _direction;

        // Handling player look
        if (_direction == Vector2.up) {
            m_directionPlayerIsFacing = EDirection.Up;
        } else if (_direction == Vector2.right) {
            m_directionPlayerIsFacing = EDirection.Right;
        } else if (_direction == Vector2.down) {
            m_directionPlayerIsFacing = EDirection.Down;
        } else if (_direction == Vector2.left) {
            m_directionPlayerIsFacing = EDirection.Left;
        }

        Collider2D movementCollision = Physics2D.OverlapCircle(movementPosition, .25f);

        if(movementCollision == null) {
            PlayAnimation(m_movement);
            StartCoroutine(MoveOneTileRoutine(m_movement));
            return;
        }


        // Checking for Triggers
        ITriggerInteraction triggerInteraction = movementCollision.gameObject.GetComponent<ITriggerInteraction>();

        if(triggerInteraction != null) {
            triggerInteraction.Interact();

            // Code Repetition
            PlayAnimation(m_movement);
            StartCoroutine(MoveOneTileRoutine(m_movement));
            return;
        }

        // Collided and it wasn't with a trigger
        StopAnimation();
    }

    private IEnumerator MoveOneTileRoutine(Vector2 _direction) {
        m_currentPlayerState = EPlayerState.Moving;
        Vector2 originalPosition = transform.position;
        Vector2 destinationPosition = originalPosition + _direction;

        for(float i = 0; i < timeToWalkOneTile; i += Time.deltaTime) {
            float t = Mathf.Clamp01(i / timeToWalkOneTile);
            transform.position = Vector2.Lerp(originalPosition, destinationPosition, t);

            yield return null;
        }

        transform.position = destinationPosition;
        PlayerMoved?.Invoke();

        yield return null;
        m_currentPlayerState = EPlayerState.Idle;
    }

    private void PlayAnimation(Vector2 _direction) {
        if(_direction == Vector2.up) {
            m_animator.Play(UP_ANIMATION);
        } else if(_direction == Vector2.right) {
            m_animator.Play(SIDE_ANIMATION);
            transform.localScale = new Vector3(Mathf.Sign(_direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        } else if(_direction == Vector2.down) {
            m_animator.Play(DOWN_ANIMATION);
        } else if(_direction == Vector2.left) {
            m_animator.Play(SIDE_ANIMATION);
            transform.localScale = new Vector3(Mathf.Sign(_direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void StopAnimation() {
        switch(m_directionPlayerIsFacing) {
            case EDirection.Up:
                m_animator.Play(STOPPED_UP_ANIMATION);
                break;
            case EDirection.Down:
                m_animator.Play(STOPPED_DOWN_ANIMATION);
                break;
            case EDirection.Right:
                m_animator.Play(STOPPED_SIDE_ANIMATION);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
            case EDirection.Left:
                m_animator.Play(STOPPED_SIDE_ANIMATION);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
        }
    }

    // -----------------------------------------------------------
    // Item
    // [TO DO]
    // The Logic of what happens when an item is collected could be written on the item script itself and could be handled with interfaces
    // Needless to say that the door handling shouldn't be here!
    private void TryToGetItem(Vector2 _direction) {
        Vector3 positionToCheck = transform.position + new Vector3(_direction.x, _direction.y, 0);
        Collider2D collisionObject = Physics2D.OverlapCircle(positionToCheck, 0.1f);

        if(collisionObject == null) {
            return;
        }

        Item itemCollided = collisionObject.gameObject.GetComponent<Item>();
        Dungeon.Door doorCollided = collisionObject.gameObject.GetComponent<Dungeon.Door>();

        if(itemCollided != null) {
            switch (itemCollided.itemType) {
                case Item.EItemType.BossRoomKey:
                    DependencyManager.Instance.DungeonController.PlayerHasBossRoomKey = true;
                    break;
            }

            Destroy(itemCollided.gameObject);
        } else if(doorCollided != null) {
            Debug.Log($"Player Collided with {doorCollided.doorType}");
            switch(doorCollided.doorType) {
                case Dungeon.Door.EDoorType.BossRoom:
                    if(DependencyManager.Instance.DungeonController.PlayerHasBossRoomKey) {
                        Destroy(collisionObject.gameObject);
                    }
                    break;
                case Dungeon.Door.EDoorType.GoalRoom:
                    if (DependencyManager.Instance.DungeonController.PlayerHasGoalRoomKey) {
                        Destroy(collisionObject.gameObject);
                    }
                    break;
            }

        }
    }

    // -----------------------------------------------------------
    // Battle
    // These could be actions and events...
    public void PlayerEnteredBattle() {
        StopAllCoroutines();
        StopAnimation();
        m_currentPlayerState = EPlayerState.InBattle;
    }

    public void PlayerExitedBattle() {
        m_currentPlayerState = EPlayerState.Idle;
    }
}
