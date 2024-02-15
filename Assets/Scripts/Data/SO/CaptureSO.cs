using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 사진의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "CaptureSO", menuName = "Scriptable Object/CaptureSO")]
public class CaptureSO : ScriptableObject
{
    public string CaptureName;
    [TextArea] public string CaptureInfo;

    public Sprite Image;

    public Sprite Polaroid;

    /// <summary> 사진 식별 번호 </summary>
    public int CaptureCode;
}
