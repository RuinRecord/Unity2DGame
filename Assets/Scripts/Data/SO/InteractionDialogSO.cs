using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 상호작용 물체에 대한 대사 리스트 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "InteractionDialogSO", menuName = "Scriptable Object/InteractionDialogSO")]
public class InteractionDialogSO : ScriptableObject
{
    /// <summary> 상호작용을 통한 아이템 획득 </summary>
    /// 만약 Null 이라면 아이템 획득이 없는 상호작용
    public ItemSO itemSO;

    /// <summary> 상호작용을 통한 조사일지 획득 </summary>
    /// 만약 Null 이라면 조사일지 획득이 없는 상호작용
    public RecordSO recordSO;

    /// <summary> 상호작용 플레이어 대사 </summary>
    public List<DialogSet> dialogs;
}