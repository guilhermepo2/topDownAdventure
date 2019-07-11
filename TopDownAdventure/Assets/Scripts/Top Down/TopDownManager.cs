using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TopDownManager : MonoBehaviour {
    public TilemapRenderer collisionObject;

    private void Start() {
        collisionObject.enabled = false;
    }
}
