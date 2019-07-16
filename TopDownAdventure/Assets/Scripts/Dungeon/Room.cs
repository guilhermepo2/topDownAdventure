using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public class Room {
        public enum ERoomType {
            Start,
            Regular,
            KeyRoom,
            BossRoom,
            GoalRoom,
            Final,
        }

        public Vector2 gridPosition;
        public ERoomType roomType;
        private Room m_parentRoom;
        public Room ParentRoom {
            get {
                return m_parentRoom;
            }
        }
        private List<Room> m_childrenRooms;
        public List<Room> ChildrenRooms {
            get {
                return m_childrenRooms;
            }
        }
        public int ChildrenRoomsAmount {
            get {
                return m_childrenRooms.Count;
            }
        }

        public Room(Vector2 _gridPosition, ERoomType _type) {
            this.gridPosition = _gridPosition;
            this.roomType = _type;
            m_childrenRooms = new List<Room>();
        }

        public void SetParent(Room _room) {
            m_parentRoom = _room;
        }

        public void AddChildren(Room _room) {
            m_childrenRooms.Add(_room);
        }
    }
}
