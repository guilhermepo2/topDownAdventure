using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class Room {
        public enum ERoomType {
            Start,
            Regular,
            KeyRoom,
            Final,
        }

        public Vector2 gridPosition;
        public ERoomType roomType;

        public Room(Vector2 _gridPosition, ERoomType _type) {
            this.gridPosition = _gridPosition;
            this.roomType = _type;
        }
    }
}
