using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct DialogSet
{
    /// <summary> 남주 대사 </summary>
    [SerializeField] private Dialog Player_M_dialog;

    /// <summary> 여주 대사 </summary>
    [SerializeField] private Dialog Player_W_dialog;

    public DialogType GetDialogType(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetDialogType();
        else return Player_W_dialog.GetDialogType();
    }

    public AudioClip GetAudioClip(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetAudioClip();
        else return Player_W_dialog.GetAudioClip();
    }

    public Sprite GetLeftSprite(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetLeftSprite();
        else return Player_W_dialog.GetLeftSprite();
    }

    public Sprite GetRightSprite(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetRightSprite();
        else return Player_W_dialog.GetRightSprite();
    }

    public string GetWords(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetWord();
        else return Player_W_dialog.GetWord();
    }

    public float GetPrintTime(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.MEN)) return Player_M_dialog.GetPrintTime();
        else return Player_W_dialog.GetPrintTime();
    }
}

public enum DialogType
{
    Interaction,
    PlayerM,
    PlayerW
}


[Serializable]
public struct Dialog
{
    /// <summary> 대화 타입 (0 = 상호작용, 1 = 남주, 2 = 여주) </summary>
    [SerializeField] private DialogType type;

    /// <summary> 대사 출력 시 등장하는 오디오 (NULL = 오디오 없음) </summary>
    [SerializeField] private AudioClip audioClip;

    /// <summary> 대사 출력 시 왼쪽 이미지 (NULL = 이미지 없음) </summary>
    [SerializeField] private Sprite leftSprite;

    /// <summary> 대사 출력 시 오른쪽 이미지 (NULL = 이미지 없음) </summary>
    [SerializeField] private Sprite rightSprite;

    /// <summary> 상호작용 문장 </summary>
    [SerializeField] [TextArea] private string words;

    /// <summary> 대사 출력 속도 </summary>
    [SerializeField] private float print_time;

    public DialogType GetDialogType() => type;

    public AudioClip GetAudioClip() => audioClip;

    public Sprite GetLeftSprite() => leftSprite;

    public Sprite GetRightSprite() => rightSprite;

    public string GetWord() => words;

    public float GetPrintTime() => print_time;
}


[Serializable]
public struct CutSceneAction
{
    [Header("다음 연출로 넘어가는 시간")]
    public float playTime;

    [Header("플레이어 대사 관련")]
    public bool isDialogOn;
    public DialogSet dialogs;

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