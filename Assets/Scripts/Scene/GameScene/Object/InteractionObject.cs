using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 플레이어 대사 </summary>
    private List<DialogSet> dialogs;

    public List<DialogSet> Dialogs => dialogs;

    /// <summary> 상호작용 식별 코드 </summary>
    public int Code;

    /// <summary> 이 상호작용이 아이템 획득을 가능케 하는가? </summary>
    [HideInInspector]
    public bool HasItem;

    /// <summary> 이 상호작용이 조사일지 획득을 가능케 하는가? </summary>
    [HideInInspector]
    public bool HasRecord;

    /// <summary> 보유한 아이템 식별 번호 (아이템 없다면 이 값은 -1) </summary>
    [HideInInspector]
    public int ItemCode;

    /// <summary> 보유한 식별 번호 (조사일지가 없다면 이 값은 -1) </summary>
    [HideInInspector]
    public int RecordCode;

    /// <summary> 획득 시 맵 오브젝트가 사라지는지에 대한 여부 </summary>
    public bool IsDestroy;


    private void Start()
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

    /// <summary>
    /// 아이템을 획득하는 함수이다.
    /// </summary>
    public void DropItem()
    {
        if (!HasItem)
            return; // 이 상호작용은 아이템을 보유하지 않음

        // 아이템 획득
        GameManager.Data.player.AddItem(ItemCode);

        // 아이템에 따라 오브젝트 변경
        switch (Code)
        {
            case 5: Code = 12; break; // (아)화분 -> 화분 변경
        }

        SetVariables();

        // 맵 오브젝트 제거
        if (IsDestroy)
        {
            MapCtrl.Instance.DestroyObject(this.gameObject);
        }
    }

    /// <summary>
    /// 조사일지를 획득하는 함수이다.
    /// </summary>
    public void DropRecord()
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
}
