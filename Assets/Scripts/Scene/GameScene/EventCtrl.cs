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

    public int CurrentEvent;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentEvent = 0;
    }

    public void CheckEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Capture:
                if (CurrentEvent == 0)
                {
                    CurrentEvent = 1;
                    TutorialManager.Instance.ShowTooltip("갤러리 저장 완료. 추가로 더 조사하세요.");
                    CutSceneCtrl.Instance.StartCutScene(1);
                }
                break;
            case EventType.Interact:
                if (CurrentEvent == 1)
                {
                    CurrentEvent = 2;
                    TutorialManager.Instance.ShowTooltip("[E] 버튼을 눌러 인벤토리를 확인하세요.");
                }
                break;
            case EventType.Inventory:
                if (CurrentEvent == 2)
                {
                    CurrentEvent = 3;
                    TutorialManager.Instance.ShowTooltip("인벤토리 확인 완료. 이제 시설을 조사하세요");
                }
                break;
        }
    }
}