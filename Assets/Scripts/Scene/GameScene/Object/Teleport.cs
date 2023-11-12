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

        // 막힌 공간이라면 플레이어가 바라보는 방향으로 1칸 더 던진
        while (!MapCtrl.instance.CheckValidArea(destination))
            destination += (Vector3Int)PlayerCtrl.instance.GetDirection();

        StartCoroutine(GameManager._change.switchPos(destination));
    }

    /// <summary>
    /// (_clickPos) 위치에 움직일 수 있는 오브젝트가 있는지 체크하고 반환하는 함수이다.
    /// </summary>
    /// <param name="_clickPos">체크 위치</param>
    /// <returns>움직일 수 있는 오브젝트 (없다면 NULL 반환)</returns>
    private MovingObject CheckObject(Vector2 _clickPos)
    {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(_clickPos, Vector2.up, 0.25f, 512))
        {
            // 플레이어 위치에서 도착 위치로 ray를 발사 충돌 검사
            // 충돌 지점에 옮기긱 가능한 오브젝트가 있는지 체크
            return hit.transform.gameObject.GetComponent<MovingObject>();
        }
        else
            return null;
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
