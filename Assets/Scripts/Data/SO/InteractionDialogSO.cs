using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 상호작용 물체에 대한 대사 리스트 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "InteractionDialogSO", menuName = "Scriptable Object/InteractionDialogSO")]
public class InteractionDialogSO : ScriptableObject
{
    public List<PlayerDialog> dialogs;
}