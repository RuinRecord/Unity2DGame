using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 플레이어 대사 </summary>
    private List<DialogSet> dialogs;

    public List<DialogSet> GetDialogs() => dialogs;

    /// <summary> 상호작용 식별 코드 </summary>
    public int code;

    /// <summary> 이 상호작용이 아이템 획득을 가능케 하는가? </summary>
    [HideInInspector]
    public bool hasItem;

    /// <summary> 이 상호작용이 조사일지 획득을 가능케 하는가? </summary>
    [HideInInspector]
    public bool hasRecord;

    /// <summary> 보유한 아이템 식별 번호 (아이템 없다면 이 값은 -1) </summary>
    [HideInInspector]
    public int itemCode;

    /// <summary> 보유한 식별 번호 (조사일지가 없다면 이 값은 -1) </summary>
    [HideInInspector]
    public int recordCode;

    /// <summary> 획득 시 맵 오브젝트가 사라지는지에 대한 여부 </summary>
    public bool isDestroy;


    private void Start()
    {
        var dialogData = GameManager._data.interactionDialogDatas[code];

        dialogs = dialogData.dialogs;
        hasItem = dialogData.itemSO != null;
        hasRecord = dialogData.recordSO != null;
        itemCode = hasItem ? dialogData.itemSO.itemCode : -1;
        recordCode = hasRecord ? dialogData.recordSO.recordCode : -1;
    }

    /// <summary>
    /// 아이템을 획득하는 함수이다.
    /// </summary>
    public void DropItem()
    {
        if (!hasItem)
            return; // 이 상호작용은 아이템을 보유하지 않음

        // 아이템 획득
        GameManager._data.player.AddItem(itemCode);

        // 맵 오브젝트 제거
        if (isDestroy)
        {
            MapCtrl.instance.DestroyObject(this.gameObject);
        }
    }

    /// <summary>
    /// 조사일지를 획득하는 함수이다.
    /// </summary>
    public void DropRecord()
    {
        if (!hasRecord)
            return; // 이 상호작용은 아이템을 보유하지 않음

        // 아이템 획득
        GameManager._data.player.AddRecord(recordCode);

        // 맵 오브젝트 제거
        if (isDestroy)
        {
            MapCtrl.instance.DestroyObject(this.gameObject);
        }
    }
}
