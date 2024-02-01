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

    public void CheckEvent(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Capture:
                if (CurrentEvent == 0)
                {
                    ShowTooptipAsEvent("갤러리 저장 완료. 추가로 더 조사하세요");
                    CutSceneCtrl.Instance.StartCutScene(1);
                }
                break;

            case EventType.GetItem:
                if (CurrentEvent == 3)
                {
                    TutorialManager.Instance.CloseTutorial();
                    CutSceneCtrl.Instance.StartCutScene(2);
                    CurrentEvent++;
                }    
                break;

            case EventType.GetRecord:
                if (CurrentEvent == 1)
                    ShowTooptipAsEvent("[E] 버튼을 눌러 인벤토리를 확인하세요");
                break;

            case EventType.Inventory:
                if (CurrentEvent == 2)
                    ShowTooptipAsEvent("인벤토리 확인 완료. 이제 시설을 조사하세요");
                break;

            case EventType.MoveObject:
                if (CurrentEvent == 4)
                {
                    TutorialManager.Instance.CloseTutorial();
                    CutSceneCtrl.Instance.StartCutScene(3);
                    CurrentEvent++;
                }
                break;
        }
    }

    private void ShowTooptipAsEvent(string tooltipText)
    {
        CurrentEvent++;
        TutorialManager.Instance.ShowTooltip(tooltipText);
    }
}