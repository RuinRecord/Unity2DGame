using System.Collections;
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

public class PlayerCtrl : MonoBehaviour
{
    /// <summary> 초당 회복하는 마나 수치 </summary>
    private const float MP_CHARGE_SPEED = 1f;


    /// <summary> 회피 쿨타임 시간 </summary>
    private const float EVASION_COOLTIME = 0.5f;


    /// <summary> 회피 이동 강도 </summary>
    private const float EVASION_FORCE = 2f;


    /// <summary> 강한 공격을 시전하기 위한 차징 시간 </summary>
    private const float STRONG_ATTACK_TIME = 3f;


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

    private NavMeshAgent agent;
    private Animator animator;

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
    public bool isCanMove, isCanAttack, isCanEvasion;
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

        isCanMove = isCanAttack = isCanEvasion = true;
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
        if (state.Equals(PlayerState.DEAD))
            return; // 죽은 상태의 경우 기능 동작 불가

        if (PlayerTag.isTagOn || playerType != PlayerTag.playerType)
            return; // 현재 태그 선택 중이거나, 현재 태그된 플레이어가 아니면 동작 불가

        // State에 따른 행동 수행
        StateFunc();

        // 마나 회복
        ChargeMana();

        // 이동
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            goalVec = GetValidDestination(destination);

            if (isAttackCharge || attack_type != -1)
                SetMove(goalVec, 1.5f); // 공격 차징 중일 경우 느린 이동
            else
                SetMove(goalVec, 3f); // 그 외 보통 이동
        }

        // 남주인공 기능
        if (playerType.Equals(PlayerType.MEN))
        {
            // 공격
            if (isCanAttack)
            {
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
    /// 플레이어 이동을 멈추는 함수이다.
    /// </summary>
    public void StopMove()
    {
        SetMove(transform.position, 3f);
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
    /// '_destination' 월드 위치 벡터로 플레이어가 이동 가능하도록 가공하여 반환합니다.
    /// </summary>
    /// <param name="_destination">도착 위치 벡터</param>
    /// <returns>이동 가능한 도착 위치 벡터</returns>
    private Vector2 GetValidDestination(Vector2 _destination)
    {
        RaycastHit2D hit;
        goalVec = _destination;
        moveVec = goalVec - (Vector2)this.transform.position;
        moveVec.Set(moveVec.x, moveVec.y);

        // 갈 수 없는 지역을 누른 경우
        if (Physics2D.Raycast(_destination, Vector3.forward, 10f, 64))
        {
            // 플레이어 위치에서 도착 위치로 ray를 발사 충돌 검사
            // 충돌 지점으로 임시 도착 위치 재설정
            if (hit = Physics2D.Raycast(this.transform.position, moveVec, moveVec.magnitude, 64))
                goalVec = hit.point;
        }

        return goalVec;
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
                        animator.SetFloat("DirX", agent.velocity.normalized.x);
                        animator.SetFloat("DirY", agent.velocity.normalized.y);
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -moveVec, EVASION_FORCE, 64);

        // 장애물이 있다면 거리를 조절
        if (hit) distance = hit.distance;
        Debug.Log(distance);

        // 정지 상태: 최근 이동한 방향 벡터를 기준으로 회피 도달 위치 설정
        if (playerState.Equals(PlayerState.IDLE))
            goalVec = (Vector2)transform.position - moveVec.normalized * distance;
        // 그 외 상태: 현재 이동 중인 방향 벡터를 기준으로 회피 도달 위치 설정
        else
            goalVec = (Vector2)transform.position - (Vector2)agent.velocity.normalized * distance;

        // 회피 수행
        SetMove(goalVec, 6f);
        state = PlayerState.EVASION;
    }


    /// <summary>
    /// 회피 애니메이션이 끝났음을 알리는 애니메이션 이벤트 함수 (Animator Tab에서 사용)
    /// </summary>
    public void EndEvasion()
    {
        SetMove(transform.position, 3f);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// 현재 공격 상태 변수에 따라 특정 공격을 수행하는 함수이다.
    /// </summary>
    private void StartAttack()
    {
        // 만약 'STRONG_ATTACK_TIME' 이상 차징했다면 강한 공격 수행
        if (attack_clickTime >= STRONG_ATTACK_TIME)
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

        // 카메라 UI 꺼지도록 코루틴 함수 실행
        StartCoroutine(PrintUICtrl.instance.CaptureCameraOut());
    }
}
