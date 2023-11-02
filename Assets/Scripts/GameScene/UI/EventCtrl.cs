using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 연출을 담당하는 이벤트 클래스이다.
/// </summary>
public class EventCtrl : MonoBehaviour
{
    private const float EVENT_START_TIME = 0.5f;

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


    /// <summary> 이벤트 관리 리스트 </summary>
    [SerializeField]
    private List<Event> eventsList;


    private void Awake()
    {
        instance = this;
    }


    /// <summary>
    /// 새로운 조사 이벤트를 추가하는 함수이다.
    /// </summary>
    /// <param name="capture_code">조사 이벤트 식별코드</param>
    public void FindNewEvent(int capture_code)
    {
        StartCoroutine(StartEvent(capture_code));
    }


    /// <summary>
    /// 조사 이벤트에 해당하는 연출 및 변경을 수행하는 함수이다.
    /// </summary>
    /// <param name="capture_code"></param>
    IEnumerator StartEvent(int capture_code)
    {
        yield return new WaitForSeconds(EVENT_START_TIME);

        Event e = eventsList[capture_code];
        if (e == null || e.eventObject == null) 
            yield break;

        switch (capture_code)
        {
            case 0:
                e.eventObject.SetActive(true);
                break;
        }
    }
}
