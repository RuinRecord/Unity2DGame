using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스토리 진행을 위한 이벤트 클래스이다.
/// </summary>
[Serializable]
public class EventFunction
{
    /// <summary> 이벤트 식별 코드 </summary>
    [SerializeField]
    private int eventCode;

    /// <summary> 해당 이벤트를 완료하면 추가되는 새로운 이벤트 식별 코드 </summary>
    [SerializeField]
    private int nextEventCode;

    /// <summary> 해당 이벤트에 상호작용될 오브젝트 </summary>
    [SerializeField]
    private GameObject eventObject;

    /// <summary> 해당 이벤트에 상호작용될 사운드 </summary>
    [SerializeField]
    private AudioSource eventSE;

    public void OnEvent()
    {
        Debug.Log($"이벤트 : {eventCode} 발동");
        Debug.Log($"이벤트 : {nextEventCode} 잠금 해제");

        var events = GameManager._data.map.hasEvents;
        if (!events.Remove(eventCode))
            Debug.LogError("이벤트 삭제 실패");
        events.Add(nextEventCode);

        // 이벤트 기능 수행
        switch (eventCode)
        {
            case 0:
                if (eventObject != null) 
                    eventObject.SetActive(false);
                break;
        }
    }
}


public class EventCtrl : MonoBehaviour
{
    /// <summary> EventCtrl 싱글톤 </summary>
    private static EventCtrl Instance;
    public static EventCtrl instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    /// <summary> 이벤트 처리 내용을 담은 리스트 (데이터 설정은 GameScene -> Map 오브젝트에서 설정) </summary>
    [SerializeField]
    private List<EventFunction> EventFunctions;


    private void Awake()
    {
        instance = this;
    }
    

    /// <summary>
    /// 이벤트에 해당하는 연출 및 변경을 수행하는 함수이다.
    /// </summary>
    /// <param name="eventCode"></param>
    public void StartEvent(int eventCode)
    {
        if (GameManager._data.map.hasEvents.IndexOf(eventCode) == -1)
        {
            Debug.LogError("현재 해당 이벤트가 존재하지 않음!");
            return;
        }

        if (EventFunctions.Count <= eventCode)
        {
            Debug.LogError("이벤트 Index 에러!");
            return;
        }

        // 현재 이벤트 기능 수행
        EventFunction e = EventFunctions[eventCode];
        e.OnEvent();
    }
}