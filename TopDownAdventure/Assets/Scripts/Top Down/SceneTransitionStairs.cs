using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionStairs : MonoBehaviour, ITriggerInteraction {
    public int levelToTransitionTo;

    public void Interact() {
        DependencyManager.Instance.LevelManager.LoadLevel(levelToTransitionTo);
    }
}
