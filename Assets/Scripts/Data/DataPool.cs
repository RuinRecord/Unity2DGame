using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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


[Serializable]
public struct CutSceneAction
{
    [Header("다음 연출로 넘어가는 시간")]
    public float playTime;

    [Header("카메라 이동 관련")]
    public bool isCameraMoveOn;
    public Vector2 camera_destination;
    public float camera_moveSpeed;
    public bool camera_isMoveSmooth;

    [Header("카메라 줌 관련")]
    public bool isCameraZoomOn;
    public float camera_zoomSize;
    public float camera_zoomSpeed;
    public bool camera_isZoomSmooth;
}