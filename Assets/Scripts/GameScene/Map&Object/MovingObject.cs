using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class MovingObject : MonoBehaviour
{
    [SerializeField]
    private Vector2 up_gap;

    [SerializeField]
    private Vector2 down_gap;

    [SerializeField]
    private Vector2 right_gap;

    [SerializeField]
    private Vector2 left_gap;

    /// <summary> 현재 물건이 밀릴 방향 벡터 </summary>
    public Vector2 arrowVec;


    /// <summary>
    /// 플레이어가 오브젝트에 붙을 정확한 위치 벡터를 반환하는 함수이다.
    /// </summary>
    /// <param name="_destination">클릭한 위치</param>
    /// <returns>플레이어가 오브젝트에 붙을 정확한 위치 벡터</returns>
    public Vector2 GetValidDestination(Vector2 _destination)
    {
        Vector2 new_destination = _destination;
        Vector2 moveVec;
        RaycastHit2D hit;
        moveVec = _destination - (Vector2)PlayerCtrl.instance.transform.position;
        moveVec.Set(moveVec.x, moveVec.y);

        // 플레이어 위치에서 도착 위치로 ray를 발사 충돌 검사
        // 충돌 지점으로 임시 도착 위치 재설정
        if (hit = Physics2D.Raycast(PlayerCtrl.instance.transform.position, moveVec, moveVec.magnitude, 64))
            new_destination = hit.point;

        // 플레이어와 도착 지점 벡터를 (UP, DOWN, RIGHT, LEFT) 벡터 중 가장 가까운 벡터 얻기
        arrowVec = GetDirection(PlayerCtrl.instance.transform.position, new_destination);

        // 방향 벡터에 맞춰 위치 조정
        new_destination = (Vector2)this.transform.position;
        if (arrowVec == Vector2.up)
            new_destination += down_gap;
        else if (arrowVec == Vector2.down)
            new_destination += up_gap;
        else if (arrowVec == Vector2.right)
            new_destination += left_gap;
        else if (arrowVec == Vector2.left)
            new_destination += right_gap;

        // 탑뷰 시점에 맞춰 위치 조정
        new_destination += Vector2.down * 0.25f;

        Debug.Log("방향 벡터 < " + arrowVec + " > 설정");

        return new_destination;
    }

    public void Push()
    {

    }

    /// <summary>
    /// (_start)에서 시작하여 (_end)로 끝나는 벡터에 가장 가까운 방향 벡터(UP, DOWN, RIGHT, LEFT)를 반환하는 함수이다.
    /// </summary>
    private Vector2 GetDirection(Vector2 _start, Vector2 _end)
    {
        Vector2 vec;
        Vector2 arrowVec = (_end - _start).normalized;
        float degree = Mathf.Atan2(arrowVec.y, arrowVec.x) * Mathf.Rad2Deg;

        if (45f <= degree && degree < 135f)
            vec = Vector2.up;
        else if (-135f <= degree && degree < -45f)
            vec = Vector2.down;
        else if (45f > Mathf.Abs(degree))
            vec = Vector2.right;
        else
            vec = Vector2.left;

        return vec;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (PlayerCtrl.instance.movingObject == this)
        {
            // 만약 플레이어가 클릭한 MovingObject에 다다르면
            // 모드 변경
            PlayerCtrl.instance.mode = PlayerMode.PUSH;
            PlayerCtrl.instance.state = PlayerState.IDLE;

            // 바라보는 방향 설정
            if (arrowVec != Vector2.zero)
                PlayerCtrl.instance.SetAnimationDir(arrowVec);
        }   
    }
}
