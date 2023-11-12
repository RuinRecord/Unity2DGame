using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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