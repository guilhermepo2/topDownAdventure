using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class BossEncounter : EnemyEncounter {
        public GameObject goalRoomKey;

        public override void ProcessEncounter() {
            Instantiate(goalRoomKey, transform.position, Quaternion.identity);
            base.ProcessEncounter();
        }
    }
}
