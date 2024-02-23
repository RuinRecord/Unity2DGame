using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{


}

/// <summary>
/// 현재 플레이어의 종류
/// </summary>
public enum PlayerType
{
    NONE,
    MEN,    // 남주인공
    WOMEN   // 여주인공
}

/// <summary>
/// 현재 플레이어의 상태
/// </summary>
public enum PlayerState
{
    IDLE,       // 정지
    WALK,       // 이동
    JUMP,       // 점프
    EVASION,    // 회피
    ATTACK,     // 공격
    CAPTURE,    // 조사
    DEAD        // 죽음
}

/// <summary>
/// 현재 플레이어의 모드
/// </summary>
public enum PlayerMode
{
    DEFAULT,    // 평소
    PUSH,       // 밀기 (남주만 해당)
    CRAWL,      // 기기 (여주만 해당)
}

public enum LightAnim
{
    None,
    Blink_Slow,
    Blink_Normal,
    Blink_Fast,
}

public enum LightColor
{
    White,
    Red,
    Blue,
    Yellow,
}

public enum Event
{
    StartPrologue,
    DoneCaptureTutorial,
    DoneRecordTutorial,
    BeforeMeetPlayerM,
    PlayerMTutorial,
    AfterMeetPlayerM,
    BeforeTagTutorial,
    StartTagTutorial,
    FindSecretRoom,
    OpenVent,
    GetToR4,
    TurnONR4,
}

public enum EventTiming
{
    Capture,
    GetItem,
    GetRecord,
    Inventory,
    MoveObject,
    CutScene,
}

public enum BlockType
{
    PlayerMRoom,
    R1ToHoll,
    Hall,
    R2ToHoll,
}

public enum CameraMode
{
    PlayerW,
    PlayerM,
    Free
}

public enum DoorType
{
    Ivory_window,
    Navy_no_window,
    Ivory_no_window,
}

public enum Scene
{
    MainScene,
    GameScene
}

public enum MonitorType
{
    Off,
    On,
    Error
}