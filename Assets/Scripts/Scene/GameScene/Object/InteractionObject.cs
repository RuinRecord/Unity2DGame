using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 대사 데이터 </summary>
    [HideInInspector]
    public InteractionDialogSO dialogData;

    /// <summary> 상호작용 식별 코드 </summary>
    public int code;

    private void Start()
    {
        dialogData = GameManager._data.interactionDialogDatas[code];
    }
}
