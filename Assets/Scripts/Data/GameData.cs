using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 모든 게임 데이터를 저장하는 클래스이다.
/// </summary>
[Serializable]
public class GameData 
{
    public PlayerData player;
    public MapData map;
    public string savedTime;
    public int slotIndex;
}

