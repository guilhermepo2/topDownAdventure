using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class DungeonController : MonoBehaviour {
        private bool m_playerHasBossRoomKey;
        public bool PlayerHasBossRoomKey {
            get {
                return m_playerHasBossRoomKey;
            }
            set {
                m_playerHasBossRoomKey = value;
            }
        }

        private bool m_playerHasGoalRoomKey;
        public bool PlayerHasGoalRoomKey {
            get {
                return m_playerHasGoalRoomKey;
            }
            set {
                m_playerHasGoalRoomKey = value;
            }
        }

        private void Start() {
            m_playerHasBossRoomKey = false;
            m_playerHasGoalRoomKey = false;
        }
    }
}
