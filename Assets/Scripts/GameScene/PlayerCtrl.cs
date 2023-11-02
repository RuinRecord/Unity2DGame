using System;
using System.Collections;
using UnityEngine;

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

public class PlayerCtrl : MonoBehaviour
{
    /// <summary> 회피 쿨타임 시간 </summary>
    private const float EVASION_COOLTIME = 1.5f;


    /// <summary> 회피 이동 강도 </summary>
    private const int EVASION_FORCE = 3;


    /// <summary> 플레이어 이동 속도 </summary>
    private const float MOVE_SPEED = 4f;


    /// <summary> 플레이어 이동 속도 </summary>
    private const float EVASION_SPEED = 6f;


    /// <summary> PlayerCtrl 싱글톤 패턴 </summary>
    private static PlayerCtrl player_M, player_W;
    public static PlayerCtrl instance
    {
        get 
        { 
            switch (PlayerTag.playerType)
            {
                case PlayerType.MEN: return player_M;
                case PlayerType.WOMEN: return player_W;
            }
            return null;
        }
    }

    private Coroutine moveCo;

    private PlayerType playerType;

    /// <summary> 이동 불가능한 오브젝트 레이어 마스트 </summary>
    [SerializeField]
    private LayerMask canNotMove_layerMask;

    /// <summary> 현재 플레이어와 접촉한 포탈 (없으면 NULL) </summary>
    public Teleport teleport;

    /// <summary> 현재 플레이어와 클릭한 옮기기 가능 오브젝트 (없으면 NULL) </summary>
    public MovingObject movingObject;

    private Animator animator;

    [Obsolete]
    [SerializeField]
    private PlayerMode playerMode;

    public PlayerMode mode
    {
        set
        {
            playerMode = value;
            SetAnimation();
        }
        get { return playerMode; }
    }


    [Obsolete]
    [SerializeField]
    private PlayerState playerState;
    
    public PlayerState state
    {
        set 
        {
            playerState = value;
            SetAnimation();
        }
        get { return playerState; }
    }

    /// <summary> 플레이어가 해당 기능을 사용할 수 있는 상태인가? </summary>
    public bool isCanInteract, isCanMove, isCanAttack, isCanEvasion;
    public bool isCanCapture, isCameraOn;

    /// <summary> 플레이어가 현재 움직이는 중인가? </summary>
    public bool isMoving;


    /// <summary> 플레이어 MAX HP </summary>
    public float max_HP;


    /// <summary> 최근 플레이어의 위치 </summary>
    private Vector2Int currentPos;


    /// <summary> 플레이어 현재 HP </summary>
    private float CUR_HP;
    public float cur_HP
    {
        set 
        { 
            CUR_HP = value;
            PlayerStateUI.instance.SetPlayerHP();
        }
        get { return CUR_HP; }
    }


    private void Awake()
    {
        if (tag.Equals("Player_M"))
        {
            playerType = PlayerType.MEN;
            player_M = this;
        }
        else if (tag.Equals("Player_W"))
        {
            playerType = PlayerType.WOMEN;
            player_W = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        state = PlayerState.IDLE;
        teleport = null;

        isCanInteract = isCanMove = isCanAttack = isCanEvasion = true;
        isCanCapture = isCameraOn = isMoving = false;
        max_HP = cur_HP = 100f;

        SetCurrentPos();
        this.transform.position = new Vector3(currentPos.x, currentPos.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        // 최근 위치에서 크기 1만큼 변경될 경우 currentPos 재갱신
        if (currentPos.x != Mathf.RoundToInt(this.transform.position.x) || currentPos.y != Mathf.RoundToInt(this.transform.position.y))
            SetCurrentPos();

        // 이동
        if (isCanMove)
        {
            Vector2Int dir = Vector2Int.zero;
            if (Input.GetKey(KeyCode.UpArrow)) dir = Vector2Int.up;
            else if (Input.GetKey(KeyCode.DownArrow)) dir = Vector2Int.down;
            else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector2Int.right;
            else if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector2Int.left;

            // 기본 모드의 경우, state를 Idle로 초기화
            if (mode.Equals(PlayerMode.DEFAULT))
                state = PlayerState.IDLE;

            if (dir != Vector2Int.zero)
            {
                // 이동 방향키 눌림
                Vector2Int destination = currentPos + dir;
                bool isValid = CheckValidArea(destination); // 이동 가능 지역인가?

                // 만약 남주인공이라면 클릭한 위치에 움직일 수 있는 물체 체크
                if (playerType.Equals(PlayerType.MEN))
                {
                    movingObject = CheckMovingObject(destination);
                    if (movingObject != null)
                    {
                        mode = PlayerMode.PUSH;
                        movingObject.SetDirection(GetDirection());
                    }
                    else
                    {
                        // 움직이는 물체 외 클릭
                        // => 밀기 모드였다면 해제
                        if (mode.Equals(PlayerMode.PUSH))
                            mode = PlayerMode.DEFAULT;
                    }
                }

                // 이동
                if (isValid)
                    SetMove(dir, 1, MOVE_SPEED);
                SetAnimationDir(dir);
            }
        }

        // 상호작용 및 포탈 사용
        if (isCanInteract && Input.GetKeyDown(KeyCode.Space))
        {
            if (mode.Equals(PlayerMode.DEFAULT))
            {
                if (teleport != null)
                {
                    // 포탈 사용
                    teleport.GoToDestination();
                }
                else
                {
                    // 상호작용
                    Vector2Int dir = GetDirection();
                    // Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0f), Color.green, 3f);

                    // 상호작용 오브젝트 탐색
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, 1f, 256);
                    if (hit)
                    {
                        // 있으면 상호작용 대화 시스템 시작
                        InteractUICtrl.instance.StartDialog(hit.transform.GetComponent<InteractionObject>().dialogs);
                    }
                }
            }
            else if(mode.Equals(PlayerMode.PUSH))
            {
                // 물건 밀기
                if (movingObject == null)
                {
                    // 움직이는 오브젝트가 Null이면 오류 반환
                    Debug.LogError("Error!! MovingObject is null?!");
                    return;
                }

                // 물체 이동
                movingObject.Push();
            }
        }

        // 남주인공 기능
        if (playerType.Equals(PlayerType.MEN))
        {
            if (mode.Equals(PlayerMode.DEFAULT))
            {
                // 공격
                if (isCanAttack && Input.GetKey(KeyCode.Q))
                {
                    StartAttack();
                }

                // 회피
                if (isCanEvasion && Input.GetKeyDown(KeyCode.W))
                {
                    StartEvasion();
                }
            }
        }
        // 여자 주인공 기능
        else if (playerType.Equals(PlayerType.WOMEN))
        {
            // 조사
            if (isCanCapture && Input.GetKeyDown(KeyCode.Q))
            {
                // 조사 시작
                if (!isCameraOn)
                    StartCapture();
                // 카메라 끄기
                else
                    EndCapture();
            }
        }
    }


    /// <summary>
    /// 현재 플레이어 조작이 가능한 지 체크하는 함수이다.
    /// </summary>
    private bool CheckCanUpdate()
    {
        if (GameManager._change.isChanging)
            return false; // 현재 씬 및 위치 전환 중이면 동작 불가

        if (state.Equals(PlayerState.DEAD))
            return false; // 죽은 상태의 경우 기능 동작 불가

        if (PlayerTag.isTagOn || playerType != PlayerTag.playerType)
            return false; // 현재 태그 선택 중이거나, 현재 태그된 플레이어가 아니면 동작 불가

        if (InteractUICtrl.instance.isInteractOn)
            return false; // 현재 상호작용 대화 시스템이 작동 중이면 동작 불가

        return true;
    }


    /// <summary>
    /// CurrentPos를 플레이어의 현재 위치에 맞춰 재갱신하는 함수이다.
    /// </summary>
    public void SetCurrentPos() => SetCurrentPos(this.transform.position);


    /// <summary>
    /// CurrentPos를 '_pos'위치에 맞춰 재갱신하는 함수이다.
    /// </summary>
    public void SetCurrentPos(Vector3 _pos)
    {
        currentPos.x = Mathf.RoundToInt(_pos.x); 
        currentPos.y = Mathf.RoundToInt(_pos.y);
    }


    /// <summary>
    /// 'moveSpeed'의 속도로 플레이어를 '_dir' 한칸 이동 시키는 함수이다.
    /// </summary>
    /// <param name="_dir">이동 방향 벡터</param>
    /// <param name="_dis">이동 거리</param>
    /// <param name="_moveSpeed">플레이어의 이동속도</param>
    private void SetMove(Vector2Int _dir, int _dis, float _moveSpeed)
    {
        state = PlayerState.WALK;

        if (moveCo != null)
            StopCoroutine(moveCo);
        moveCo = StartCoroutine(StartMove(_dir, _dis, _moveSpeed));
    }


    /// <summary>
    /// 'moveSpeed'의 속도로 플레이어를 '_dir'로 '_dir'거리 만큼 부드럽게 이동시키는 코루틴 함수이다.
    /// </summary>
    /// <param name="_dir">이동 방향 벡터</param>
    /// <param name="_dis">이동 거리</param>
    /// <param name="_moveSpeed">플레이어의 이동속도</param>
    IEnumerator StartMove(Vector2Int _dir, int _dis, float _moveSpeed)
    {
        Vector3 dir_vec3 = new Vector3(_dir.x, _dir.y, 0);
        Vector2 cur_dir = _dir * _dis;

        // 만약 이동(or 회피) 중에 발동됐다면 이동한 만큼 재조정
        cur_dir -= new Vector2(this.transform.position.x - currentPos.x, this.transform.position.y - currentPos.y);

        // 데이터 세팅
        SetAnimationDir(_dir);
        isMoving = true;
        isCanInteract = isCanMove = isCanAttack = false;

        while (cur_dir.normalized == _dir)
        {
            // 플레이어 이동
            this.transform.position += Time.deltaTime * _moveSpeed * dir_vec3;
            cur_dir -= Time.deltaTime * _moveSpeed * (Vector2)dir_vec3;
            yield return null;
        }

        // 위치 보정
        this.transform.position = new Vector3(currentPos.x, currentPos.y, 0);

        // 데이터 설정
        isMoving = false;
        isCanInteract = isCanMove = isCanAttack = true;
    }


    /// <summary>
    /// 현재 플레이어가 바라보는 방향을 반환하는 함수이다.
    /// </summary>
    public Vector2Int GetDirection()
    {
        Vector2Int vec;
        float degree = Mathf.Atan2(animator.GetFloat("DirY"), animator.GetFloat("DirX")) * Mathf.Rad2Deg;

        if (45f <= degree && degree < 135f)
            vec = Vector2Int.up;
        else if (-135f <= degree && degree < -45f)
            vec = Vector2Int.down;
        else if (45f > Mathf.Abs(degree))
            vec = Vector2Int.right;
        else
            vec = Vector2Int.left;

        return vec;
    }


    /// <summary>
    /// 플레이어를 텔레포트하는 함수이다.
    /// </summary>
    /// <param name="_destination">목적지 위치</param>
    public void Teleport(Vector3 _destination)
    {
        this.transform.position = _destination;
        SetCurrentPos(_destination);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 방향 벡터에 따라 바라보는 방향을 설정하느 함수이다.
    /// </summary>
    /// <param name="dir">뱡향 벡터</param>
    public void SetAnimationDir(Vector2 dir)
    {
        animator.SetFloat("DirX", dir.normalized.x);
        animator.SetFloat("DirY", dir.normalized.y);
    }
    

    /// <summary>
    /// '_destination' 월드 위치 벡터로 플레이어가 이동한지 체크하고 반환합니다.
    /// </summary>
    /// <param name="_destination">도착 위치 벡터</param>
    /// <returns>이동 가능한지에 대한 여부</returns>
    private bool CheckValidArea(Vector2 _destination) => !Physics2D.Raycast(_destination, Vector2.up, 0.25f, canNotMove_layerMask);


    /// <summary>
    /// (_clickPos) 위치에 움직일 수 있는 오브젝트가 있는지 체크하고 반환하는 함수이다.
    /// </summary>
    /// <param name="_clickPos">체크 위치</param>
    /// <returns>움직일 수 있는 오브젝트 (없다면 NULL 반환)</returns>
    private MovingObject CheckMovingObject(Vector2 _clickPos)
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


    /// <summary>
    /// 플레이어의 상태에 따른 애니메이션을 설정하는 함수이다.
    /// </summary>
    private void SetAnimation()
    {
        if (mode.Equals(PlayerMode.DEFAULT))
        {
            animator.SetBool("isPush", false);
            animator.SetBool("isWalk", false);
            animator.SetBool("isEvasion", false);
            animator.SetBool("isCapture", false);
            switch (state)
            {
                case PlayerState.IDLE:
                    break;
                case PlayerState.WALK:
                    animator.SetBool("isWalk", true);
                    break;
                case PlayerState.EVASION:
                    animator.SetBool("isEvasion", true);
                    break;
                case PlayerState.ATTACK:
                    animator.SetBool("isAttack", true);
                    break;
                case PlayerState.CAPTURE:
                    animator.SetBool("isCapture", true);
                    break;
            }
        }
        else
        {
            animator.SetBool("isPush", true);
            animator.SetBool("isWalk", false);
            switch (state)
            {
                case PlayerState.IDLE:
                    break;
                case PlayerState.WALK:
                    animator.SetBool("isWalk", true);
                    break;
            }
        }
    }


    /// <summary>
    /// 회피 쿨타임 시작 함수
    /// </summary>
    IEnumerator EvasionCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;

        yield return new WaitForSeconds(EVASION_COOLTIME);
        // 'EVASION_COOLTIME' 초가 흐른 뒤 아래 구문이 수행됩니다.

        isCanMove = isCanAttack = isCanEvasion = true;
    }


    /// <summary>
    /// 회피 기능을 수행하는 함수이다.
    /// </summary>
    private void StartEvasion()
    {
        int distance = EVASION_FORCE;
        Vector2Int dir = GetDirection();
        Vector2 dir_half = new Vector2(dir.x * 0.51f, dir.y * 0.51f);

        // 회피 방향으로 장애물이 있는지 체크
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + dir_half, dir, EVASION_FORCE, canNotMove_layerMask);

        // 장애물이 있다면 거리를 조절
        if (hit) distance = Mathf.RoundToInt(hit.distance);
        if (distance == 0) return; // 회피 불가능 => 취소

        // 이전에 이동 중이라면 중지
        if (moveCo != null)
            StopCoroutine(moveCo);

        // 회피 쿨타임 및 기능 수행
        StartCoroutine("EvasionCoolTime");
        moveCo = StartCoroutine(StartMove(dir, distance, EVASION_SPEED));
        state = PlayerState.EVASION;
    }


    /// <summary>
    /// 회피 애니메이션이 끝났음을 알리는 애니메이션 이벤트 함수 (Animator Tab에서 사용)
    /// </summary>
    public void EndEvasion()
    {
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 현재 공격 상태 변수에 따라 특정 공격을 수행하는 함수이다.
    /// </summary>
    private void StartAttack()
    {
        // 공격 수행
        isCanMove = isCanAttack = isCanEvasion = false;
        state = PlayerState.ATTACK;
        animator.SetBool("isAttack", true);
    }


    /// <summary>
    /// (애니메이션 이벤트 함수) 공격 애니메이션이 종료되었을 때 발동, 공격 관련 데이터를 초기화하는 함수이다.
    /// </summary>
    public void EndAttack()
    {
        isCanMove = isCanAttack = isCanEvasion = true;
        state = PlayerState.IDLE;
        animator.SetBool("isAttack", false);
    }


    /// <summary>
    /// 조사 기능을 수행하는 함수이다.
    /// </summary>
    private void StartCapture()
    {
        // 제자리에 서도록 만듬
        isCanMove = isCanCapture = false;
        isCameraOn = true;
        PlayerTag.instance.isCanTag = false;
        state = PlayerState.CAPTURE;

        // 카메라 UI 켜지도록 코루틴 함수 실행
        StartCoroutine(PrintUICtrl.instance.CaptureCameraIn());
    }


    /// <summary>
    /// 카메라를 종료하는 함수이다.
    /// </summary>
    public void EndCapture()
    {
        state = PlayerState.IDLE;
        isCanMove = isCanCapture = false;
        isCameraOn = false;
        PlayerTag.instance.isCanTag = true;

        // 카메라 UI 꺼지도록 코루틴 함수 실행
        StartCoroutine(PrintUICtrl.instance.CaptureCameraOut());
    }
}
