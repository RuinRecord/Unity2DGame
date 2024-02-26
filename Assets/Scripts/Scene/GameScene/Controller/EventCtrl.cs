using UnityEngine;

public class EventCtrl : MonoBehaviour
{
    /// <summary> EventCtrl 싱글톤 </summary>
    private static EventCtrl instance;
    public static EventCtrl Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }

    [SerializeField] private Event currentEvent;
    public Event CurrentEvent => currentEvent;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCurrentEvent(Event @event) => currentEvent = @event;

    public void CheckEvent(EventTiming eventType)
    {
        switch (eventType)
        {
            case EventTiming.Capture:
                if (currentEvent == Event.StartPrologue)
                {
                    ShowTooptipAsEvent("갤러리 저장 완료. [Space] 버튼을 눌러 추가로 조사하세요.");
                    CutSceneCtrl.Instance.StartCutScene(1);
                }
                break;

            case EventTiming.GetItem:
                if (currentEvent == Event.BeforeMeetPlayerM)
                {
                    CloseTooptipAsEvent();
                    CutSceneCtrl.Instance.StartCutScene(2);
                }    
                break;

            case EventTiming.GetRecord:
                if (currentEvent == Event.DoneCaptureTutorial)
                    ShowTooptipAsEvent("[E] 버튼을 눌러 인벤토리를 확인하세요");
                break;

            case EventTiming.Inventory:
                if (currentEvent == Event.DoneRecordTutorial)
                    ShowTooptipAsEvent("인벤토리 확인 완료. 이제 시설을 조사하세요");
                break;

            case EventTiming.MoveObject:
                if (currentEvent == Event.PlayerMTutorial)
                {
                    CloseTooptipAsEvent();
                    CutSceneCtrl.Instance.StartCutScene(3);
                }
                else if (currentEvent == Event.StartTagTutorial)
                {
                    currentEvent++;
                    CutSceneCtrl.Instance.StartCutScene(9);
                }
                break;

            case EventTiming.CutScene:
                if (currentEvent == Event.BeforeTagTutorial)
                    ShowTooptipAsEvent("[TAB] 버튼을 눌러 유진 시점은 전환하여 선반을 미세요.");
                break;
        }
    }

    private void ShowTooptipAsEvent(string tooltipText)
    {
        currentEvent++;
        TutorialManager.Instance.ShowTutorial(tooltipText);
    }

    private void CloseTooptipAsEvent()
    {
        currentEvent++;
        TutorialManager.Instance.CloseTutorial();
    }
}