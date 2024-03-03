using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 조사일지의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "RecordSO", menuName = "Scriptable Object/RecordSO")]
public class RecordSO : ScriptableObject
{
    public string recordName;
    [TextArea] public string recordInfo;

    /// <summary> 조사일지 식별 번호 </summary>
    public int recordCode;
}