using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    /// <summary> 포탈과 연결된 목적지 </summary>
    [SerializeField]
    private Vector3 destination;


    /// <summary> 포탈과 연결된 목적지 </summary>
    [SerializeField]
    private AudioClip audioClip;


    /// <summary> 현재 사용 가능한 포탈인지에 대한 여부 </summary>
    public bool isOn;


    /// <summary>
    /// 포탈 목적지로 이동하는 함수이다.
    /// </summary>
    public void GoToDestination()
    {
        if (!isOn)
            return; // 만약 닫힌 상태라면 취소

        // Fade 애니메이션과 함께 목적지로 이동
        PlayerTag.instance.isCanTag = false;
        UIManager._interactUI.PlayAudio(audioClip);
        StartCoroutine(GameManager._change.switchPos(destination));
    }


    private void OnTriggerStay2D(Collider2D col)
    {
        // 충돌 범위 안으로 들어온 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") || col.tag.Equals("Player_W"))
        {
            PlayerCtrl.instance.teleport = this;
        }
    }


    private void OnTriggerExit2D(Collider2D col)
    {
        // 충돌 범위 밖으로 나간 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") || col.tag.Equals("Player_W"))
        {
            PlayerCtrl.instance.teleport = null;
        }
    }
}
