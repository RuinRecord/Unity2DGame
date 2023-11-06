using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 아이템의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public string item_name;
    public string item_info;

    /// <summary> 아이템 식별 번호 </summary>
    public int itemCode;
}


/// <summary> 사진의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "CaptureData", menuName = "Scriptable Object/CaptureData")]
public class CaptureData : ScriptableObject
{
    public string capture_name;
    public string capture_info;

    /// <summary> 사진 식별 번호 </summary>
    public int captureCode;

    /// <summary> 해금되는 이벤트 식별 번호 </summary>
    public int eventCode;
}


/// <summary> 조사일지의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "RecordData", menuName = "Scriptable Object/RecordData")]
public class RecordData : ScriptableObject
{
    public string record_name;
    public string record_info;

    /// <summary> 조사일지 식별 번호 </summary>
    public int recordCode;
}


/// <summary> 상호작용 물체에 대한 대사 리스트 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "InteractionDialog", menuName = "Scriptable Object/InteractionDialog")]
public class InteractionDialog : ScriptableObject
{
    public List<PlayerDialog> dialogs;
}


[Serializable]
public struct PlayerDialog
{
    /// <summary> 남주 대사 </summary>
    [SerializeField]
    private Dialog Player_M_dialog;

    /// <summary> 여주 대사 </summary>
    [SerializeField]
    private Dialog Player_W_dialog;

    public AudioClip GetAudioClip()
    {
        if (PlayerTag.playerType.Equals(PlayerType.MEN)) return Player_M_dialog.getAudioClip();
        else return Player_W_dialog.getAudioClip();
    }

    public string GetWords()
    {
        if (PlayerTag.playerType.Equals(PlayerType.MEN)) return Player_M_dialog.getWord();
        else return Player_W_dialog.getWord();
    }

    public float GetPrintTime()
    {
        if (PlayerTag.playerType.Equals(PlayerType.MEN)) return Player_M_dialog.getPrintTime();
        else return Player_W_dialog.getPrintTime();
    }
}


[Serializable]
public struct Dialog
{
    /// <summary> 대사 출력 시 등장하는 오디오 (NULL = 오디오 없음) </summary>
    [SerializeField]
    private AudioClip audioClip;

    /// <summary> 상호작용 문장 </summary>
    [SerializeField]
    private string words;

    /// <summary> 대사 출력 속도 </summary>
    [SerializeField]
    private float print_time;

    public AudioClip getAudioClip() => audioClip;

    public string getWord() => words;

    public float getPrintTime() => print_time;
}