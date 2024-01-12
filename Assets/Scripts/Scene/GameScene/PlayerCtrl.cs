using System;
using System.Collections;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private const float EVASION_COOLTIME = 1.5f;

    private const int EVASION_FORCE = 3;

    private const float WALK_SPEED = 4f;

    private const float RUN_SPEED = 6f;

    private const float MOVE_OBJECT_DETECT_DISTANCE = 0.275f;

    private const float INTERACTION_OBJECT_DETECT_DISTANCE = 1f;


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

    /// <summary> 현재 플레이어와 접촉한 포탈 (없으면 NULL) </summary>
    public Teleport teleport;

    /// <summary> 현재 플레이어와 클릭한 옮기기 가능 오브젝트 (없으면 NULL) </summary>
    public MovingObject movingObject;

    private Animator animator;

    private Rigidbody2D rigidbody;

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
    public bool isCanCapture, isCameraOn, isCanInven;

    /// <summary> 플레이어가 현재 움직이는 중인가? </summary>
    public bool isMoving;


    /// <summary> 플레이어 MAX HP </summary>
    public float max_HP;


    /// <summary> 플레이어 현재 속도 </summary>
    public float moveSpeed;


    /// <summary> 최근 플레이어의 위치 </summary>
    private Vector2Int currentPos;


    /// <summary> 플레이어 현재 HP </summary>
    private float CUR_HP;
    public float cur_HP
    {
        set 
        { 
            CUR_HP = value;
            UIManager._playerUI.SetPlayerHP();
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
        rigidbody = GetComponent<Rigidbody2D>();

        state = PlayerState.IDLE;
        teleport = null;

        isCanInteract = isCanMove = isCanAttack = isCanEvasion = isCanInven = true;
        isCanCapture = isCameraOn = isMoving = false;
        max_HP = cur_HP = 100f;
        moveSpeed = WALK_SPEED;
        animator.speed = 1;

        SetCurrentPos();
        this.transform.position = new Vector3(currentPos.x, currentPos.y, 0);
    }

    private void FixedUpdate()
    {
        #region 항상 수행되는 구간

        // 최근 위치에서 크기 1만큼 변경될 경우 currentPos 재갱신
        if (currentPos.x != Mathf.RoundToInt(this.transform.position.x) || currentPos.y != Mathf.RoundToInt(this.transform.position.y))
            SetCurrentPos();

        // 이동 모드 + 이동 중 아님 -> state를 Idle로 초기화
        if (state.Equals(PlayerState.WALK) && !isMoving && isCanInteract)
            state = PlayerState.IDLE;

        #endregion

        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        // 이동
        if (isCanMove)
        {
            Vector2Int dir = Vector2Int.zero;
            if (Input.GetKey(KeyCode.UpArrow)) dir = Vector2Int.up;
            else if (Input.GetKey(KeyCode.DownArrow)) dir = Vector2Int.down;
            else if (Input.GetKey(KeyCode.RightArrow)) dir = Vector2Int.right;
            else if (Input.GetKey(KeyCode.LeftArrow)) dir = Vector2Int.left;

            if (dir != Vector2Int.zero)
            {
                // 이동 방향키 눌림
                Move(dir);

                // 위치에 움직일 수 있는 물체 감지
                movingObject = CheckMovingObject(dir);
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckCanUpdate())
            return; // 아래 기능을 수행하지 못하는 상태

        // 달리기 기능
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            moveSpeed = RUN_SPEED;
            animator.speed = 1.5f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            moveSpeed = WALK_SPEED;
            animator.speed = 1f;
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
                    //Debug.DrawRay(this.transform.position, new Vector3(dir.x, dir.y, 0f), Color.green, 3f);

                    // 상호작용 오브젝트 탐색
                    RaycastHit2D hit = Physics2D.Raycast(this.transform.position, dir, INTERACTION_OBJECT_DETECT_DISTANCE, 256);
                    if (hit)
                    {
                        // 있으면 상호작용 대화 시스템 시작
                        InteractionObject @object = hit.transform.GetComponent<InteractionObject>();
                        UIManager._interactUI.StartDialog(@object);
                    }
                }
            }
            else if(mode.Equals(PlayerMode.PUSH))
            {
                if (playerType.Equals(PlayerType.MEN))
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
                else if (playerType.Equals(PlayerType.WOMEN))
                {
                    // 상호작용 대사
                    DialogSet[] dialogs = movingObject.player_m_dialogs.ToArray();
                    UIManager._interactUI.StartDialog(dialogs);
                }
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

            // 인벤토리 ON & OFF
            if (isCanInven && Input.GetKeyDown(KeyCode.E))
            {
                UIManager._invenUI.OnOffInven();
                isCanMove = isCanInteract = !UIManager._invenUI.isOnInven;
            }
        }
    }

    private bool CheckCanUpdate()
    {
        if (state.Equals(PlayerState.DEAD))
            return false; // 죽은 상태의 경우 & 움직이는 경우 기능 동작 불가

        if (GameManager._change.isChanging)
            return false; // 현재 씬 및 위치 전환 중이면 동작 불가

        if (CutSceneCtrl.isCutSceneOn)
            return false; // 컷씬이 진행중이면 동작 불가

        if (PlayerTag.isTagOn || playerType != PlayerTag.playerType)
            return false; // 현재 태그 선택 중이거나, 현재 태그된 플레이어가 아니면 동작 불가

        if (UIManager._interactUI.isDialog)
            return false; // 현재 상호작용 대화 시스템이 작동 중이면 동작 불가

        return true;
    }

    public void SetCurrentPos() 
        => SetCurrentPos(this.transform.position);

    public void SetCurrentPos(Vector3 _pos)
    {
        currentPos.x = Mathf.RoundToInt(_pos.x); 
        currentPos.y = Mathf.RoundToInt(_pos.y);
    }

    public void Move(Vector2Int _dir)
    {
        state = PlayerState.WALK;

        Vector2 movePos = (Vector2)this.transform.position + new Vector2(_dir.x, _dir.y) * Time.deltaTime * moveSpeed;
        rigidbody.MovePosition(movePos);
        SetAnimationDir(_dir);
    }

    public void SetMove(Vector2Int _dir, int _dis, float _moveSpeed)
    { 
        state = PlayerState.WALK;
        SetAnimationDir(_dir);

        if (moveCo != null)
            StopCoroutine(moveCo);
        moveCo = StartCoroutine(StartMove(_dir, _dis, _moveSpeed));
    }

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

        // 특별한 수행이 없을 때 기능 활성화
        if (!UIManager._invenUI.isOnInven)
            isCanInteract = isCanMove = isCanAttack = true;
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

    public void Teleport(Vector3 _destination)
    {
        teleport.Close();
        this.transform.position = _destination;
        SetCurrentPos(_destination);
        state = PlayerState.IDLE;
    }

    public void SetAnimationDir(Vector2 dir)
    {
        animator.SetFloat("DirX", dir.normalized.x);
        animator.SetFloat("DirY", dir.normalized.y);
    }

    private MovingObject CheckMovingObject(Vector2 _dir)
    {
        RaycastHit2D hit;

        if (hit = Physics2D.Raycast(this.transform.position, _dir, MOVE_OBJECT_DETECT_DISTANCE, 512))
            return hit.transform.gameObject.GetComponent<MovingObject>();
        else
            return null;
    }

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

    IEnumerator EvasionCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;

        yield return new WaitForSeconds(EVASION_COOLTIME);
        // 'EVASION_COOLTIME' 초가 흐른 뒤 아래 구문이 수행됩니다.

        isCanMove = isCanAttack = isCanEvasion = true;
    }

    private void StartEvasion()
    {
        int distance = EVASION_FORCE;
        Vector2Int dir = GetDirection();
        Vector2 dir_half = new Vector2(dir.x * 0.51f, dir.y * 0.51f);

        // 회피 방향으로 장애물이 있는지 체크
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + dir_half, dir, EVASION_FORCE, MapCtrl.instance.canNotMove_layerMask);

        // 장애물이 있다면 거리를 조절
        if (hit) 
            distance = Mathf.RoundToInt(hit.distance);

        if (distance == 0) 
            return; // 회피 불가능 => 취소

        // 회피 쿨타임 및 기능 수행
        StartCoroutine("EvasionCoolTime");
        state = PlayerState.EVASION;
    }

    public void EndEvasion()
        => state = PlayerState.IDLE;

    private void StartAttack()
    {
        // 공격 수행
        isCanMove = isCanAttack = isCanEvasion = false;
        state = PlayerState.ATTACK;
        animator.SetBool("isAttack", true);

        // 공격 소리 출력
        GameManager._sound.PlaySE("남주공격");
    }

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
        isCanMove = isCanCapture = isCanInven = false;
        isCameraOn = true;
        PlayerTag.instance.isCanTag = false;
        state = PlayerState.CAPTURE;

        // 사진기 소리 출력
        GameManager._sound.PlaySE("여주조사");

        // 카메라 UI 켜지도록 코루틴 함수 실행
        StartCoroutine(UIManager._captureUI.CaptureCameraIn());
    }


    /// <summary>
    /// 카메라를 종료하는 함수이다.
    /// </summary>
    public void EndCapture()
    {
        state = PlayerState.IDLE;
        isCanMove = isCanCapture = isCanInven = false;
        isCameraOn = false;
        PlayerTag.instance.isCanTag = true;

        // 카메라 UI 꺼지도록 코루틴 함수 실행
        StartCoroutine(UIManager._captureUI.CaptureCameraOut());
    }
}
