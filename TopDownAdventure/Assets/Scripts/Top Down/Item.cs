﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public enum EItemType {
        BossRoomKey,
        GoalRoomKey,
    }

    public EItemType itemType;
}
