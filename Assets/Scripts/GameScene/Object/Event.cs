using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 이벤트 정보를 담는 클래스이다.
/// </summary>
public class Event : MonoBehaviour 
{
    /// <summary> 이벤트 연출 대상 오브젝트 </summary>
    public GameObject eventObject;

    /// <summary> 이벤트 사용 가능 여부 </summary>
    public bool isOn;
}