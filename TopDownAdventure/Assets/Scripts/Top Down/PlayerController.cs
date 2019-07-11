using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public enum EPlayerState {
        Idle,
        Moving
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
}
