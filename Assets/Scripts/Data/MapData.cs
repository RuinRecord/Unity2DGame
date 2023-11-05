using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 맵의 데이터를 저장하는 클래스이다.
/// </summary>
[Serializable]
public class MapData
{
    public List<int> hasEvents;

    public MapData()
    {
        hasEvents = new List<int>();
        hasEvents.Add(0); // 시작 이벤트 추가
    }
}
