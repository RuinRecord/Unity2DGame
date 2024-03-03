using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class CanMoveObject : MonoBehaviour
{
    private const int CANNOT_MOVE_LAYERMASK = 64 + 128 + 256 + 512 + 1024;

    [SerializeField] private Collider2D col;

    [SerializeField] private AudioClip clip;

    [SerializeField] private float moveSpeed;

    private Vector3 startPos;

    public Event @event;

    public bool isDone;

    /// <summary> 현재 물건이 밀릴 방향 벡터 </summary>
    private Vector2 direction;

    /// <summary> 여주인공 전용 대사 (남주 대사 설정해도 동작 X) </summary>
    /// 이 대사는 SO 파일이 없습니다. 직접 오브젝트 인스펙터에서 설정해야 합니다.
    public List<DialogSet> Player_m_dialogs;

    public void SetForceDirection(Vector2 _vec) => direction = _vec;

    private void Start()
    {
        startPos = this.transform.localPosition;
    }

    /// <summary> 물체가 움직일 수 있는지 체크하고 물체를 움직이게 하는 함수이다. </summary>
    public bool Push()
    {
        if (!CheckCanMove())
        {
            Debug.Log($"Event timing is not current : {@event}");
            return false;
        }

        bool _isSuccess;
        float _distance = col.bounds.size.x * 0.5f;

        // 바라보는 방향에 장애물 확인
        RaycastHit2D hit = Physics2D.Raycast((Vector2)this.transform.position + direction * (_distance + 0.05f), direction, 0.4f, CANNOT_MOVE_LAYERMASK);
        if (_isSuccess = !hit)
        {
            // 이동 가능한 상태 => 물체 이동
            StartCoroutine(StartMove(moveSpeed));

            // 끄는 소리 오디오 실행
            GameManager.Sound.PlaySE(clip);
        }
        else
        {
            Debug.Log($"Obstacle object is detected : {hit.transform.name}");
        }

        return _isSuccess;
    }


    /// <summary> 물체가 움직일 수 있는지 체크하고 물체를 움직이게 하는 함수이다. </summary>
    public void ReSetPosition()
    {
        if (isDone)
        {
            Debug.Log("This object is already done.");
            return;
        }

        if (@event.Equals(Event.PuzzleForR4) && MapCtrl.Instance.CheckObjetsComplete())
        {
            Debug.Log("Objects are already done.");
            return;
        }

        // 그 외의 경우
        this.transform.localPosition = startPos;
    }

    public bool CheckCanMove() => EventCtrl.Instance.CurrentEvent == @event;

    /// <summary> 물체를 움직이는 코루틴 함수이다. </summary>
    IEnumerator StartMove(float moveSpeed)
    {
        Vector3 _savedPos = this.transform.position;
        Vector2 _moveVec = direction;
        PlayerCtrl.Instance.State = PlayerState.WALK;
        PlayerCtrl.Instance.IsCanInteract = PlayerCtrl.Instance.IsCanMove = false;
        UIManager.PlayerUI.SetKeyOffHUD(PlayerFunction.Interaction);

        // 방향 벡터와 이동 벡터가 반대가 되는 순간까지 이동 수행
        while (_moveVec.normalized == direction.normalized)
        {
            PlayerCtrl.Instance.transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            this.transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            _moveVec -= direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        PlayerCtrl.Instance.State = PlayerState.IDLE;
        PlayerCtrl.Instance.IsCanInteract = PlayerCtrl.Instance.IsCanMove = true;
        UIManager.PlayerUI.SetKeyOnHUD(PlayerFunction.Interaction);
        this.transform.position = _savedPos + (Vector3)direction;
    }


    private void OnTriggerStay2D(Collider2D col)
    {
        if (PlayerCtrl.Instance?.CurrentCanMoveOb == this)
        {
            if (!CheckCanMove())
            {
                Debug.Log($"Event timing is not current : {@event}");
                return;
            }

            // 만약 플레이어가 클릭한 MovingObject에 다다르면
            // 모드 변경
            PlayerCtrl.Instance.Mode = PlayerMode.PUSH;
        }   
    }
}
