using System;
using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    #region 상수 변수
    public const float WALK_SPEED = 4f;

    public const float RUN_SPEED = 6f;

    private const float EVASION_COOLTIME = 1f;

    private const int EVASION_FORCE = 3;

    private const float EVASION_SPEED = 6f;

    private const float MOVE_OBJECT_DETECT_DISTANCE = 0.525f;

    private const float INTERACTION_OBJECT_DETECT_DISTANCE = 1.25f;

    private const float RESET_MOVEOBJECT_FADE_TIME = 0.5f;
    #endregion

    /// <summary> PlayerCtrl 싱글톤 패턴 </summary>
    private static PlayerCtrl player_M, player_W;
    public static PlayerCtrl Instance
    {
        get 
        { 
            switch (PlayerTag.PlayerType)
            {
                case PlayerType.MEN: return player_M;
                case PlayerType.WOMEN: return player_W;
            }
            return null;
        }
    }


    /// <summary> 현재 플레이어와 접촉한 포탈 (없으면 NULL) </summary>
    public Teleport CurrentTeleport;


    /// <summary> 현재 플레이어와 클릭한 옮기기 가능 오브젝트 (없으면 NULL) </summary>
    public CanMoveObject CurrentCanMoveOb;


    private Coroutine currentMoveCo;


    private PlayerType playerType;


    private Animator animator;


    private new Rigidbody2D rigidbody;


    [Obsolete]
    [SerializeField]
    private PlayerMode playerMode;

    public PlayerMode Mode
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
    
    public PlayerState State
    {
        set 
        {
            playerState = value;
            SetAnimation();
        }
        get { return playerState; }
    }


    /// <summary> 플레이어가 해당 기능을 사용할 수 있는 상태인가? </summary>
    public bool IsCanReset, IsCanInteract, IsCanMove, IsCanAttack, IsCanEvasion;
    public bool IsCanCapture, IsCameraOn, IsCanInven;


    /// <summary> 플레이어가 현재 움직이는 중인가? </summary>
    public bool IsMoving;


    /// <summary> 플레이어 현재 속도 </summary>
    [Obsolete]
    private float moveSpeed;

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set
        {
            moveSpeed = value;
            SetAnimationSpeed(MoveSpeedToAnimSpeed(moveSpeed));
        }
    }


    /// <summary> 플레이어 MAX HP </summary>
    public float Max_HP;


    /// <summary> 플레이어 현재 HP </summary>
    private float CUR_HP;
    public float cur_HP
    {
        set 
        { 
            CUR_HP = value;
            UIManager.PlayerUI.SetPlayerHP();
        }
        get { return CUR_HP; }
    }

    #region Unity 콜백 함수

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
        rigidbody = GetComponent<Rigidbody2D>();

        State = PlayerState.IDLE;
        CurrentTeleport = null;

        IsCanReset = IsCanInteract = IsCanMove = IsCanAttack = IsCanEvasion = IsCanCapture = IsCanInven = true;
        IsCameraOn = IsMoving = false;
        Max_HP = cur_HP = 100f;
        MoveSpeed = WALK_SPEED;

        //CutSceneCtrl.Instance.StartCutScene(7);
    }


    private void FixedUpdate()
    {
        // 이동 모드 + 이동 중 아님 -> state를 Idle로 초기화
        if (State.Equals(PlayerState.WALK) && !IsMoving && IsCanInteract)
            State = PlayerState.IDLE;

        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        if (IsCanMove)
        {
            Vector2Int _dir = GetInputDir();
 
            if (_dir != Vector2Int.zero) 
            {
                // 이동 방향키가 눌린 경우 플레이어 이동
                Move(_dir);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        // 실시간으로 움직일 수 있는 물체 감지
        CurrentCanMoveOb = CheckMovingObject(GetDirection(), MOVE_OBJECT_DETECT_DISTANCE);

        if (CurrentCanMoveOb != null && !CurrentCanMoveOb.isDone)
        {
            if (Mode.Equals(PlayerMode.DEFAULT))
                Mode = PlayerMode.PUSH;
            CurrentCanMoveOb.SetForceDirection(GetDirection());
        }
        else
        {
            // 움직이는 물체 외 클릭
            // => 밀기 모드였다면 해제
            if (Mode.Equals(PlayerMode.PUSH))
                Mode = PlayerMode.DEFAULT;
        }

        // 입력에 대한 처리
        InputProcess();
    }

    #endregion


    private bool CheckCanUpdate()
    {
        if (State.Equals(PlayerState.DEAD))
            return false; // 죽은 상태의 경우 & 움직이는 경우 기능 동작 불가

        if (GameManager.Change.IsChanging)
            return false; // 현재 씬 및 위치 전환 중이면 동작 불가

        if (CutSceneCtrl.IsCutSceneOn)
            return false; // 컷씬이 진행중이면 동작 불가

        if (PlayerTag.IsTagOn || playerType != PlayerTag.PlayerType)
            return false; // 현재 태그 선택 중이거나, 현재 태그된 플레이어가 아니면 동작 불가

        if (UIManager.InteractUI.IsDialog)
            return false; // 현재 상호작용 대화 시스템이 작동 중이면 동작 불가

        return true;
    }


    private Vector2Int GetInputDir()
    {
        Vector2Int _dir = Vector2Int.zero;
        if (Input.GetKey(KeyCode.UpArrow)) _dir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.DownArrow)) _dir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.RightArrow)) _dir = Vector2Int.right;
        else if (Input.GetKey(KeyCode.LeftArrow)) _dir = Vector2Int.left;

        return _dir;
    }


    private void InputProcess()
    {
        // 달리기
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            MoveSpeed = RUN_SPEED;
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            MoveSpeed = WALK_SPEED;

        // 맵 초기화
        if (IsCanReset && Input.GetKeyDown(KeyCode.R))
            StartCoroutine(ResetMoveObject());


        // 상호작용
        if (IsCanInteract && Input.GetKeyDown(KeyCode.Space))
            Interaction();


        // 남주인공 전용 입력 처리
        if (playerType.Equals(PlayerType.MEN))
        {
            if (Mode.Equals(PlayerMode.DEFAULT))
            {
                if (IsCanAttack && Input.GetKey(KeyCode.Q))
                {
                    // 공격
                    StartAttack();
                }

                if (IsCanEvasion && Input.GetKeyDown(KeyCode.W))
                {
                    // 회피
                    StartEvasion();
                }
            }
        }
        // 여주인공 전용 입력 처리
        else if (playerType.Equals(PlayerType.WOMEN))
        {
            if (IsCanCapture && Input.GetKeyDown(KeyCode.Q))
            {
                if (!IsCameraOn)
                    StartCapture(); // 촬영
                else
                    EndCapture(); // 카메라 끄기 (Q 버튼)
            }

            if (IsCanInven && Input.GetKeyDown(KeyCode.E))
            {
                // 인벤토리 ON & OFF
                UIManager.InvenUI.OnOffInven();
                IsCanReset = IsCanMove = IsCanInteract = !UIManager.InvenUI.IsOnInven;
            }
        }
    }


    IEnumerator ResetMoveObject()
    {
        CutSceneCtrl.Instance.FadeOut(RESET_MOVEOBJECT_FADE_TIME);
        IsCanReset = false;

        yield return new WaitForSeconds(RESET_MOVEOBJECT_FADE_TIME);

        foreach (var item in MapCtrl.Instance.MoveObjectsList)
            item.ReSetPosition();

        CutSceneCtrl.Instance.FadeIn(RESET_MOVEOBJECT_FADE_TIME);
        IsCanReset = true;
    }


    private void Interaction()
    {
        if (IsCameraOn)
        {
            // 카메라 끄기
            EndCapture();
        }
        else
        {
            if (Mode.Equals(PlayerMode.DEFAULT))
            {
                if (CurrentTeleport != null)
                {
                    // 포탈 사용
                    CurrentTeleport.GoToDestination();
                }
                else
                {
                    // 상호작용
                    Vector2Int _dir = GetDirection();

                    // 상호작용 오브젝트 탐색
                    RaycastHit2D _hit = Physics2D.Raycast(this.transform.position, _dir, INTERACTION_OBJECT_DETECT_DISTANCE, 256);
                    if (_hit)
                    {
                        // 있으면 상호작용 대화 시스템 시작
                        InteractionObject interaction = _hit.transform.GetComponent<InteractionObject>();

                        if (interaction is Cabinet)
                        {
                            Cabinet cabinet = interaction as Cabinet;
                            if (cabinet.IsOpen && cabinet.Type == 2)
                            {
                                // 특수 캐비넷의 경우
                                UIManager.InteractUI.StartDialog(cabinet);
                            }
                            else
                            {
                                cabinet.Open();
                            }
                        }
                        else
                        {
                            // 일반 상호작용
                            if (interaction != null)
                                UIManager.InteractUI.StartDialog(interaction);
                        }
                    }
                }
            }
            else if (Mode.Equals(PlayerMode.PUSH))
            {
                if (playerType.Equals(PlayerType.MEN))
                {
                    // 물건 밀기
                    if (CurrentCanMoveOb == null)
                    {
                        // 움직이는 오브젝트가 Null이면 오류 반환
                        Debug.LogError("Error!! MovingObject is null?!");
                        return;
                    }

                    // 물체 이동
                    CurrentCanMoveOb.Push();
                }
                else if (playerType.Equals(PlayerType.WOMEN))
                {
                    // 상호작용 대사
                    DialogSet[] _dialogs = CurrentCanMoveOb.Player_m_dialogs.ToArray();
                    UIManager.InteractUI.StartDialog(_dialogs);
                }
            }
        }
    }


    public void Move(Vector2 dir)
    {
        State = PlayerState.WALK;

        Vector2 movePos = (Vector2)this.transform.position + dir * Time.deltaTime * MoveSpeed;
        rigidbody.MovePosition(movePos);
        SetAnimationDir(dir);
    }


    public void SetMove(Vector2 dir, float dis, float moveSpeed, bool fixedDir = false)
    { 
        State = PlayerState.WALK;
        MoveSpeed = moveSpeed;

        if (!fixedDir)
            SetAnimationDir(dir);

        if (currentMoveCo != null)
            StopCoroutine(currentMoveCo);
        currentMoveCo = StartCoroutine(StartMove(dir, dis, fixedDir));
    }


    IEnumerator StartMove(Vector2 dir, float dis, bool fixedDir = false)
    {
        Vector2 _curDir = dir * dis;
        
        if (!fixedDir)
            SetAnimationDir(dir);
        
        IsMoving = true;
        IsCanInteract = IsCanMove = IsCanAttack = false;

        while (_curDir.normalized == dir)
        {
            // 플레이어 이동
            Vector2 _moveVec = dir * MoveSpeed * Time.deltaTime;
            this.transform.position = (Vector2)this.transform.position + _moveVec;
            _curDir -= _moveVec;
            yield return null;
        }

        IsMoving = false;

        // 특별한 수행이 없을 때 기능 활성화
        if (!UIManager.InvenUI.IsOnInven)
            IsCanInteract = IsCanMove = IsCanAttack = true;
    }


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


    public void SetAnimationDir(Vector2 dir)
    {
        animator.SetFloat("DirX", dir.normalized.x);
        animator.SetFloat("DirY", dir.normalized.y);
    }


    private void SetAnimationSpeed(float speed)
        => animator.speed = speed;


    private float MoveSpeedToAnimSpeed(float moveSpeed)
        => moveSpeed / 4f;


    private CanMoveObject CheckMovingObject(Vector2 dir, float dis)
    {
        RaycastHit2D _hit;

        if (_hit = Physics2D.Raycast(this.transform.position, dir, dis, 512))
            return _hit.transform.gameObject.GetComponent<CanMoveObject>();
        else
            return null;
    }


    private void SetAnimation()
    {
        if (Mode.Equals(PlayerMode.DEFAULT))
        {
            animator.SetBool("isPush", false);
            animator.SetBool("isWalk", false);
            animator.SetBool("isEvasion", false);
            animator.SetBool("isCapture", false);
            animator.SetBool("isJump", false);
            switch (State)
            {
                case PlayerState.IDLE: break;
                case PlayerState.WALK: animator.SetBool("isWalk", true); break;
                case PlayerState.JUMP: animator.SetBool("isJump", true); break;
                case PlayerState.EVASION: animator.SetBool("isEvasion", true); break;
                case PlayerState.ATTACK: animator.SetBool("isAttack", true); break;
                case PlayerState.CAPTURE: animator.SetBool("isCapture", true); break;
            }
        }
        else
        {
            animator.SetBool("isPush", true);
            animator.SetBool("isWalk", false);
            switch (State)
            {
                case PlayerState.IDLE: break;
                case PlayerState.WALK: animator.SetBool("isWalk", true); break;
            }
        }
    }


    IEnumerator EvasionCoolTime()
    {
        IsCanMove = IsCanAttack = IsCanEvasion = false;

        yield return new WaitForSeconds(EVASION_COOLTIME);
        // 'EVASION_COOLTIME' 초가 흐른 뒤 아래 구문이 수행됩니다.

        IsCanMove = IsCanAttack = IsCanEvasion = true;
    }


    private void StartEvasion()
    {
        float distance = EVASION_FORCE;
        Vector2Int dir = GetDirection();

        // 회피 방향으로 장애물이 있는지 체크
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (Vector2)dir * MOVE_OBJECT_DETECT_DISTANCE, dir, EVASION_FORCE, MapCtrl.Instance.CanNotMove_layerMask);

        // 장애물이 있다면 거리를 조절
        if (hit)
            distance = hit.distance;

        SetMove(dir, distance, EVASION_SPEED);

        // 회피 쿨타임 및 기능 수행
        StartCoroutine("EvasionCoolTime");
        State = PlayerState.EVASION;
    }


    public void EndEvasion() => State = PlayerState.IDLE;


    private void StartAttack()
    {
        // 공격 수행
        IsCanMove = IsCanAttack = IsCanEvasion = false;
        State = PlayerState.ATTACK;
        animator.SetBool("isAttack", true);

        // 공격 소리 출력
        GameManager.Sound.PlaySE("남주공격");
    }


    public void EndAttack()
    {
        IsCanMove = IsCanAttack = IsCanEvasion = true;
        State = PlayerState.IDLE;
        animator.SetBool("isAttack", false);
    }


    private void StartCapture()
    {
        // 제자리에 서도록 만듬
        State = PlayerState.CAPTURE;

        // 사진기 소리 출력
        GameManager.Sound.PlaySE("여주조사");

        IsCanReset = IsCanMove = IsCanInteract = IsCanCapture = IsCanInven = false;
        PlayerTag.Instance.IsCanTag = false;

        // 카메라 UI 켜지도록 코루틴 함수 실행
        StartCoroutine(UIManager.CaptureUI.CaptureCameraIn());
    }


    public void EndCapture()
    {
        State = PlayerState.IDLE;
        IsCanReset = IsCanMove = IsCanInteract = IsCanCapture = IsCanInven = false;
        IsCameraOn = false;
        PlayerTag.Instance.IsCanTag = true;

        // 카메라 UI 꺼지도록 코루틴 함수 실행
        StartCoroutine(UIManager.CaptureUI.CaptureCameraOut());
    }

    public void StartJump() => State = PlayerState.JUMP;

    public void EndJump() => State = PlayerState.IDLE;


    public void MovePosition(Vector3 destination)
    {
        this.transform.localPosition = destination;
        MoveSpeed = WALK_SPEED;
        State = PlayerState.IDLE;
    }
}
