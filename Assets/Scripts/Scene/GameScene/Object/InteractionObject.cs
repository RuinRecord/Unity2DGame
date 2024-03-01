using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 플레이어 대사 </summary>
    private List<DialogSet> dialogs;

    public List<DialogSet> Dialogs => dialogs;

    /// <summary> 상호작용 식별 코드 </summary>
    [SerializeField] private int code;
    public int Code
    {
        get { return code; }
        set
        {
            code = value;
            SetVariables();
        }
    }

    /// <summary> 이 상호작용이 아이템 획득을 가능케 하는가? </summary>
    [HideInInspector] public bool HasItem;

    /// <summary> 이 상호작용이 조사일지 획득을 가능케 하는가? </summary>
    [HideInInspector] public bool HasRecord;

    /// <summary> 보유한 아이템 식별 번호 (아이템 없다면 이 값은 -1) </summary>
    [HideInInspector] public int ItemCode;

    /// <summary> 보유한 식별 번호 (조사일지가 없다면 이 값은 -1) </summary>
    [HideInInspector] public int RecordCode;

    /// <summary> 획득 시 맵 오브젝트가 사라지는지에 대한 여부 </summary>
    public bool IsDestroy;

    /// <summary> 획득 시 이벤트 컷씬이 발동하는 지에 대한 여부 </summary>
    public bool IsEvent;


    protected virtual void Start()
    {
        SetVariables();
    }

    private void SetVariables()
    {
        var dialogData = GameManager.Data.interactionDialogDatas[Code];

        dialogs = dialogData.dialogs;
        HasItem = dialogData.itemSO != null;
        HasRecord = dialogData.recordSO != null;
        ItemCode = HasItem ? dialogData.itemSO.itemCode : -1;
        RecordCode = HasRecord ? dialogData.recordSO.recordCode : -1;
    }

    public virtual void DropItem()
    {
        if (!HasItem)
            return; // 이 상호작용은 아이템을 보유하지 않음

        // 아이템 획득
        GameManager.Data.player.AddItem(ItemCode);

        switch (Code)
        {
            case 5: Code = 12; break; // (열쇠)화분 -> 화분 변경
            case 19: Code = 7; break; // (드라이버)
            case 32: 
                TutorialManager.Instance.ShowTutorial("시설의 환풍구를 조사하세요.");
                InteractionObject interactionObject = GameObject.Find("(Find전용)환풍구").GetComponent<InteractionObject>();
                interactionObject.Code = 33;
                interactionObject.IsEvent = true;
                break;
            case 34: Code = 6; break; // (손전등)화분 -> 철제선반 변경
        }

        // 맵 오브젝트 제거
        if (IsDestroy)
        {
            MapCtrl.Instance.DestroyObject(this.gameObject);
        }
    }

    public virtual void DropRecord()
    {
        if (!HasRecord)
            return; // 이 상호작용은 아이템을 보유하지 않음

        // 아이템 획득
        GameManager.Data.player.AddRecord(RecordCode);

        // 맵 오브젝트 제거
        if (IsDestroy)
        {
            MapCtrl.Instance.DestroyObject(this.gameObject);
        }
    }

    public void EventOn()
    {
        if (!IsEvent)
            return;

        switch (Code)
        {
            case 28:
                CutSceneCtrl.Instance.StartCutScene(8);
                this.GetComponent<Collider2D>().enabled = false;
                break;

            case 33:
                CutSceneCtrl.Instance.StartCutScene(10);
                this.GetComponent<Collider2D>().enabled = false;
                break;

            case 39:
                CutSceneCtrl.Instance.StartCutScene(13);
                IsEvent = false;
                Code = 40;
                break;

            case 47:
                CutSceneCtrl.Instance.StartCutScene(15);
                IsEvent = false;
                Code = 48;
                break;
        }
    }
}
