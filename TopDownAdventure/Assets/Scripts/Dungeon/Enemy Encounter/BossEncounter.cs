using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class BossEncounter : EnemyEncounter {
        public GameObject goalRoomKey;

        public override void ProcessEncounter() {
            DependencyManager.Instance.Encounter.ProcessBossEncounter();
            Instantiate(goalRoomKey, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
