using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class Door : MonoBehaviour {
        public enum EDoorType {
            BossRoom,
            GoalRoom,
        }

        public EDoorType doorType;
    }
}
