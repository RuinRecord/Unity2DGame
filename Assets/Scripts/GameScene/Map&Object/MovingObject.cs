using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class MovingObject : MonoBehaviour
{
    private const float MOVE_SPEED = 3f;
    private const int CANNOT_MOVE_LAYERMASK = 64 + 128 + 256 + 512 + 1024;

    [SerializeField]
    private new AudioSource audio;

    [SerializeField]
    private Vector2 up_gap;

    [SerializeField]
    private Vector2 down_gap;

    [SerializeField]
    private Vector2 right_gap;

    [SerializeField]
    private Vector2 left_gap;

    /// <summary> 현재 물건이 밀릴 방향 벡터 </summary>
    private Vector2 direction;

    public void SetDirection(Vector2 _vec) => direction = _vec;


    /// <summary>
    /// 물체가 움직일 수 있는지 체크하고 물체를 움직이게 하는 함수이다.
    /// </summary>
    /// <returns>이동 수행 여부</returns>
    public bool Push()
    {
        bool isSuccess;

        // 바라보는 방향에 장애물 확인
        if (isSuccess = !Physics2D.Raycast((Vector2)this.transform.position + direction * 0.55f, direction, 0.4f, CANNOT_MOVE_LAYERMASK))
        {
            // 이동 가능한 상태 => 물체 이동
            StartCoroutine("StartMove");

            // 끄는 소리 오디오 실행
            audio.clip = Datapool.instance.GetSE(2);
            audio.Play();
        }
        return isSuccess;
    }


    /// <summary>
    /// 물체를 움직이는 코루틴 함수이다.
    /// </summary>
    IEnumerator StartMove()
    {
        Vector3 savedPos = this.transform.position;
        Vector2 moveVec = direction;
        PlayerCtrl.instance.state = PlayerState.WALK;
        PlayerCtrl.instance.isCanInteract = PlayerCtrl.instance.isCanMove = false;

        // 방향 벡터와 이동 벡터가 반대가 되는 순간까지 이동 수행
        while (moveVec.normalized == direction.normalized)
        {
            PlayerCtrl.instance.transform.position += (Vector3)direction * MOVE_SPEED * Time.deltaTime;
            this.transform.position += (Vector3)direction * MOVE_SPEED * Time.deltaTime;
            moveVec -= direction * MOVE_SPEED * Time.deltaTime;
            yield return null;
        }

        PlayerCtrl.instance.state = PlayerState.IDLE;
        PlayerCtrl.instance.isCanInteract = PlayerCtrl.instance.isCanMove = true;
        PlayerCtrl.instance.transform.position = savedPos;
        PlayerCtrl.instance.SetCurrentPos(savedPos);
        this.transform.position = savedPos + (Vector3)direction;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (PlayerCtrl.instance.movingObject == this)
        {
            // 만약 플레이어가 클릭한 MovingObject에 다다르면
            // 모드 변경
            PlayerCtrl.instance.mode = PlayerMode.PUSH;
        }   
    }
}
