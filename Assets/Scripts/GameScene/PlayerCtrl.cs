using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 현재 플레이어의 종류
/// </summary>
public enum PlayerType
{
    MEN,
    WOMEN
}

/// <summary>
/// 현재 플레이어의 상태
/// </summary>
public enum PlayerState
{
    IDLE,
    WALK,
    EVASION,
    ATTACK,
    CAPTURE,
    DEAD
}

/// <summary>
/// 현재 플레이어의 공격 자세 (남주인공만 유효)
/// </summary>
public enum PlayerAttack
{
    NODE,
    READY,
    BASICATTACK_1,
    BASICATTACK_2,
    STRONGATTACK
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
    /// <summary> 초당 회복하는 마나 수치 </summary>
    private const float MP_CHARGE_SPEED = 1f;


    /// <summary> 회피 쿨타임 시간 </summary>
    private const float EVASION_COOLTIME = 0.5f;


    /// <summary> 회피 이동 강도 </summary>
    private const float EVASION_FORCE = 3f;


    /// <summary> 플레이어 이동 속도 </summary>
    private const float MOVE_SPEED = 3f;


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

    private PlayerType playerType;

    [SerializeField]
    private LayerMask canNotMove_layerMask;

    /// <summary> 현재 플레이어와 접촉한 포탈 (없으면 NULL) </summary>
    public Teleport teleport;

    /// <summary> 현재 플레이어와 클릭한 옮기기 가능 오브젝트 (없으면 NULL) </summary>
    public MovingObject movingObject;

    private NavMeshAgent agent;
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


    /// <summary> 현재 공격 차징 중인가? </summary>
    private bool isAttackCharge;


    /// <summary> 플레이어 MAX HP </summary>
    public float max_HP;


    /// <summary> 플레이어 MAX MP </summary>
    public float max_MP;


    /// <summary> 최근 플레이어의 방향 벡터 </summary>
    private Vector2 moveVec;


    /// <summary> 최근 플레이어의 도착 벡터 </summary>
    private Vector2 goalVec;


    /// <summary> 현재 공격 Count 상태 </summary>
    private int attack_count;


    /// <summary> 현재 공격 상태라면 어떤 공격을 수행 중인가? </summary>
    private int attack_type;


    /// <summary> 공격 준비 시간 </summary>
    private float attack_clickTime;


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


    /// <summary> 플레이어 현재 MP </summary>
    private float CUR_MP;
    public float cur_MP
    {
        set
        {
            CUR_MP = value;
            PlayerStateUI.instance.SetPlayerMP();
        }
        get { return CUR_MP; }
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
        agent= GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateUpAxis = false;
        agent.updateRotation = false;
        state = PlayerState.IDLE;
        teleport = null;

        isCanInteract = isCanMove = isCanAttack = isCanEvasion = true;
        isCanCapture = isCameraOn = isAttackCharge = false;
        max_HP = cur_HP = 100f;
        max_MP = cur_MP = 10f;
        moveVec = Vector2.up;
        attack_count = 0;
        attack_type = -1;
        attack_clickTime = 0f;

        animator.SetInteger("AttackType", attack_type);
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        // State에 따른 행동 수행
        StateFunc();

        // 마나 회복
        ChargeMana();

        // 이동
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool isValid; // 이동 가능 지역을 눌렀는가?


            // 클릭한 위치 벡터에 대해 실제 이동될 위치 벡터 획득
            goalVec = GetValidDestination(destination, out isValid);


            // 만약 남주인공이라면 클릭한 위치에 움직일 수 있는 물체 체크
            if (playerType.Equals(PlayerType.MEN))
            {
                movingObject = CheckMovingObject(destination);
                if (movingObject != null)
                {
                    // 움직이는 물체가 존재
                    // => 자연스럽게 물체에 붙을 수 있도록 이동 목표 지점 재설정
                    goalVec = movingObject.GetValidDestination(destination);
                }
                else
                {
                    // 움직이는 물체 외 클릭
                    // => 밀기 모드였다면 해제
                    if (mode.Equals(PlayerMode.PUSH))
                        mode = PlayerMode.DEFAULT;
                }
            }


            if (!isValid && (goalVec - (Vector2)this.transform.position).magnitude < 0.1f)
            {
                // 이동 불가 지역 => 플레이어 방향만 수정
                SetAnimationDir(moveVec);
            }
            else
            {
                // 이동 가능 지역 => 이동
                if (isAttackCharge || attack_type != -1)
                    SetMove(goalVec, MOVE_SPEED / 2f); // 공격 차징 중일 경우 느린 이동
                else
                    SetMove(goalVec, MOVE_SPEED); // 그 외 보통 이동
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
                    StopMove();
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
                        StopMove();
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

                // 플레이어 설정
                StopMove();
                mode = PlayerMode.DEFAULT;
                movingObject = null;
            }
        }

        // 남주인공 기능
        if (playerType.Equals(PlayerType.MEN))
        {
            // 공격
            if (isCanAttack)
            {
                /*
                // 공격 버튼 꾹 누르는 중
                if (Input.GetKey(KeyCode.Q))
                {
                    // 공격 차징 시간 실시간으로 증가
                    attack_clickTime += Time.deltaTime;

                    // 공격 차징 첫 시작
                    if (!isAttackCharge)
                    {
                        SetMoveSpeed(1.5f);
                        isAttackCharge = true;
                        isCanEvasion = false;
                        animator.SetBool("isAttack", true);
                    }
                }
                // 공격 버튼 땜
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    // 공격 수행
                    StartAttack();
                }
                */

                if (Input.GetKey(KeyCode.Q))
                {
                    isCanEvasion = false;
                    animator.SetBool("isAttack", true);
                    StopMove();
                    StartAttack();
                }
            }

            // 회피
            if (isCanEvasion && Input.GetKeyDown(KeyCode.W))
            {
                // 회피 스테미나 체크
                if (cur_MP >= 1f)
                {
                    // 회피 스테미나 감소
                    cur_MP -= 1f;

                    // 회피 쿨타임 및 기능 수행
                    StartCoroutine("EvasionCoolTime");
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
        if (state.Equals(PlayerState.DEAD))
            return false; // 죽은 상태의 경우 기능 동작 불가

        if (PlayerTag.isTagOn || playerType != PlayerTag.playerType)
            return false; // 현재 태그 선택 중이거나, 현재 태그된 플레이어가 아니면 동작 불가

        if (InteractUICtrl.instance.isInteractOn)
            return false; // 현재 상호작용 대화 시스템이 작동 중이면 동작 불가

        if (BlindCtrl.instance.isBlind)
            return false; // 현재 씬 및 위치 전환 중이면 동작 불가

        return true;
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
    /// 플레이어 이동을 멈추는 함수이다.
    /// </summary>
    public void StopMove()
    {
        SetMove(transform.position, MOVE_SPEED);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 플레이어를 텔레포트하는 함수이다.
    /// </summary>
    /// <param name="_destination">목적지 위치</param>
    public void Teleport(Vector3 _destination)
    {
        agent.Warp(_destination);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 마나를 회복시키는 함수이다.
    /// </summary>
    private void ChargeMana()
    {
        cur_MP += Time.deltaTime * MP_CHARGE_SPEED;
        if (cur_MP > max_MP)
            cur_MP = max_MP;
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
    /// '_destination' 월드 위치 벡터로 플레이어가 이동 가능하도록 가공하여 반환합니다.
    /// </summary>
    /// <param name="_destination">도착 위치 벡터</param>
    /// <returns>이동 가능한 도착 위치 벡터</returns>
    private Vector2 GetValidDestination(Vector2 _destination, out bool _isValid)
    {
        RaycastHit2D hit;
        goalVec = _destination;
        moveVec = goalVec - (Vector2)this.transform.position;
        moveVec.Set(moveVec.x, moveVec.y);
        _isValid = true;

        // 갈 수 없는 지역을 누른 경우
        if (Physics2D.Raycast(_destination, Vector3.forward, 10f, canNotMove_layerMask))
        {
            // 플레이어 위치에서 도착 위치로 ray를 발사 충돌 검사
            // 충돌 지점으로 임시 도착 위치 재설정
            if (hit = Physics2D.Raycast(this.transform.position, moveVec, moveVec.magnitude, canNotMove_layerMask))
                goalVec = hit.point;
            _isValid = false;
        }

        return goalVec;
    }


    /// <summary>
    /// (_clickPos) 위치에 움직일 수 있는 오브젝트가 있는지 체크하고 반환하는 함수이다.
    /// </summary>
    /// <param name="_clickPos">마우크 클릭 위치</param>
    /// <returns>움직일 수 있는 오브젝트 (없다면 NULL 반환)</returns>
    private MovingObject CheckMovingObject(Vector2 _clickPos)
    {
        RaycastHit2D hit;
        if (hit = Physics2D.Raycast(_clickPos, Vector3.forward, 10f, 512))
        {
            // 플레이어 위치에서 도착 위치로 ray를 발사 충돌 검사
            // 충돌 지점에 옮기긱 가능한 오브젝트가 있는지 체크
            return hit.transform.gameObject.GetComponent<MovingObject>();
        }
        else
            return null;
    }


    /// <summary>
    /// 'moveSpeed'의 속도로 'destination' 월드 위치로 최단 경로를 통해 플레이어를 이동시키는 함수이다.
    /// </summary>
    /// <param name="_destination">도착 위치 벡터</param>
    /// <param name="_moveSpeed">플레이어의 이동속도</param>
    private void SetMove(Vector2 _destination, float _moveSpeed)
    {
        state = PlayerState.WALK;
        agent.SetDestination(_destination);
        SetMoveSpeed(_moveSpeed);
    }


    /// <summary>
    /// 플레이어의 이동 속도를 'moveSpeed'으로 설정하는 함수이다.
    /// </summary>
    /// <param name="_moveSpeed">설정할 플레이어 이동 속도</param>
    private void SetMoveSpeed(float _moveSpeed)
    {
        agent.speed = _moveSpeed;
    }


    /// <summary>
    /// 플레이어의 상태에 따른 기본 행동 요령을 수행하는 함수이다.
    /// </summary>
    private void StateFunc()
    {
        switch(state)
        {
            case PlayerState.IDLE:
                break;
            case PlayerState.WALK:
                // 도착까지 남은 거리가 작으면 Idle로 변경
                if (agent.remainingDistance < Time.deltaTime)
                    state = PlayerState.IDLE;
                else
                {
                    if (agent.velocity != Vector3.zero)
                    {
                        SetAnimationDir(agent.velocity);
                    }
                }
                break;
        }
    }


    /// <summary>
    /// 플레이어의 상태에 따른 애니메이션을 설정하는 함수이다.
    /// </summary>
    private void SetAnimation()
    {
        if (mode.Equals(PlayerMode.DEFAULT))
        {
            animator.SetBool("isPush", false);
            switch (state)
            {
                case PlayerState.IDLE:
                    animator.SetBool("isWalk", false);
                    animator.SetBool("isEvasion", false);
                    animator.SetBool("isCapture", false);
                    break;
                case PlayerState.WALK:
                    animator.SetBool("isWalk", true);
                    animator.SetBool("isEvasion", false);
                    break;
                case PlayerState.EVASION:
                    animator.SetBool("isWalk", false);
                    animator.SetBool("isEvasion", true);
                    break;
                case PlayerState.ATTACK:
                    break;
                case PlayerState.CAPTURE:
                    animator.SetBool("isCapture", true);
                    break;
            }
        }
        else
        {
            animator.SetBool("isPush", true);
            switch (state)
            {
                case PlayerState.IDLE:
                    animator.SetBool("isWalk", false);
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
        float distance = EVASION_FORCE;

        // 회피 방향으로 장애물이 있는지 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVec, EVASION_FORCE, 64);

        // 장애물이 있다면 거리를 조절
        if (hit) distance = hit.distance;

        // 정지 상태: 최근 이동한 방향 벡터를 기준으로 회피 도달 위치 설정
        if (state.Equals(PlayerState.IDLE))
            goalVec = (Vector2)transform.position + moveVec.normalized * distance;
        // 그 외 상태: 현재 이동 중인 방향 벡터를 기준으로 회피 도달 위치 설정
        else
            goalVec = (Vector2)transform.position + (Vector2)agent.velocity.normalized * distance;

        // 회피 수행
        SetMove(goalVec, 6f);
        state = PlayerState.EVASION;
    }


    /// <summary>
    /// 회피 애니메이션이 끝났음을 알리는 애니메이션 이벤트 함수 (Animator Tab에서 사용)
    /// </summary>
    public void EndEvasion()
    {
        SetMove(transform.position, MOVE_SPEED);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 현재 공격 상태 변수에 따라 특정 공격을 수행하는 함수이다.
    /// </summary>
    private void StartAttack()
    {
        // 만약 'STRONG_ATTACK_TIME' 이상 차징했다면 강한 공격 수행
        if (attack_clickTime >= MOVE_SPEED)
        {
            // 일반 공격 1회 이상인 경우
            if (attack_count > 0)
            {
                // 콤보 공격 데이터 설정
                attack_type = 2;
            }
            // 일반 공격 0회 인 경우
            else
            {
                // 강한 공격 데이터 설정
                attack_type = 1;
                cur_MP -= 3;
            }

            // 공격 회수 초기화
            attack_count = 0;
        }
        else
        {
            // 일반 공격 데이터 설정
            attack_type = 0;
            cur_MP -= 2;

            // 일반 공격 3회 시 공격 회수 초기화
            if (++attack_count > 2)
                attack_count = 0;
        }

        // 공격 수행
        isCanMove = isCanAttack = isAttackCharge = false;
        attack_clickTime = 0f;
        state = PlayerState.ATTACK;
        animator.SetInteger("AttackType", attack_type);
    }


    /// <summary>
    /// (애니메이션 이벤트 함수) 공격 애니메이션이 종료되었을 때 발동, 공격 관련 데이터를 초기화하는 함수이다.
    /// </summary>
    public void EndAttack()
    {
        state = PlayerState.IDLE;
        isCanMove = isCanAttack = isCanEvasion = true;
        attack_type = -1;
        animator.SetBool("isAttack", false);
        animator.SetInteger("AttackType", attack_type);
    }


    /// <summary>
    /// 조사 기능을 수행하는 함수이다.
    /// </summary>
    private void StartCapture()
    {
        // 제자리에 서도록 만듬
        agent.SetDestination(this.transform.position);
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
