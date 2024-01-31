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

public enum EventType
{
    Capture,
    GetItem,
    GetRecord,
    Inventory,
}

public enum BlockType
{
    PlayerMRoom,
    R1ToHoll,
    Hall,
}

public enum CameraMode
{
    PlayerW,
    PlayerM,
    Free
}