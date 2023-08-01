using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    Idle,
    Walk,
    Roll,
    Attack,
    Dead
}

public enum PlayerAttack
{
    None,
    Ready,
    BasicAttack_1,
    BasicAttack_2,
    StrongAttack
}

public class PlayerCtrl : MonoBehaviour
{
    private const float MP_CHARGE_SPEED = 2f;
    private const float EVASION_COOLTIME = 0.5f;
    private const float ATTACK_COOLTIME = 0.5f;
    private const float STRONG_ATTACK_TIME = 3f;

    private static PlayerCtrl Instance;
    public static PlayerCtrl instance
    {
        set 
        {
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }

    private NavMeshAgent agent;
    private Animator animator;

    private PlayerState playerState;
    public PlayerState state
    {
        set 
        {
            if (playerState == value)
                return;

            playerState = value;
            SetAnimation();
        }
        get { return playerState; }
    }

    private bool isCanMove, isCanAttack, isCanEvasion;
    public float max_HP;
    public float max_MP;
    private float CUR_HP, CUR_MP;

    public float cur_HP
    {
        set 
        { 
            CUR_HP = value;
            PlayerStateUI.instance.SetPlayerState();
        }
        get { return CUR_HP; }
    }

    public float cur_MP
    {
        set
        {
            CUR_MP = value;
            PlayerStateUI.instance.SetPlayerState();
        }
        get { return CUR_MP; }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent= GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateUpAxis = false;
        agent.updateRotation = false;
        state = PlayerState.Idle;

        isCanMove = isCanAttack = isCanEvasion = true;
        max_HP = cur_HP = 100f;
        max_MP = cur_MP = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            // 이동
            Vector2 move_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            agent.SetDestination(move_pos);
        }

        if (isCanAttack && Input.GetKeyDown(KeyCode.Q))
        {
            // 공격
            agent.SetDestination(transform.position);
            StartCoroutine("AttackCoolTime");
        }

        if (isCanEvasion && Input.GetKeyDown(KeyCode.W))
        {
            if (cur_MP >= 1f)
            {
                // 회피 스테미나 감소
                cur_MP -= 1f;

                // 회피 쿨타임 및 기능 수행
                StartCoroutine("EvasionCoolTime");
                StartEvasion();
            }
        }

        // State에 따른 행동 수행
        SetState();

        // 마나 회복
        cur_MP += Time.deltaTime * MP_CHARGE_SPEED;
        if (cur_MP > max_MP)
            cur_MP = max_MP;
    }

    private void SetState()
    {
        switch(state)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
                // 마우스 클릭한 위치까지 남은 거리에 따라 State 변경
                state = (agent.remainingDistance < 0.01f) ? PlayerState.Idle : PlayerState.Walk;

                // 플레이어가 바라보는 방향으로 이미지 좌우 전환
                if (agent.velocity.x > 0.01f)
                    this.transform.localScale = Vector3.one;
                else if (agent.velocity.x < -0.01f)
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
            case PlayerState.Roll:
                break;
        }
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case PlayerState.Idle:
                animator.SetBool("isWalk", false);
                animator.SetBool("isRoll", false);
                break;
            case PlayerState.Walk:
                animator.SetBool("isWalk", true);
                animator.SetBool("isRoll", false);
                break;
            case PlayerState.Roll:
                animator.SetBool("isWalk", false);
                animator.SetBool("isRoll", true);
                break;
        }
    }

    IEnumerator AttackCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;
        yield return new WaitForSeconds(ATTACK_COOLTIME);
        isCanMove = isCanAttack = isCanEvasion = true;
    }

    IEnumerator EvasionCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;
        yield return new WaitForSeconds(EVASION_COOLTIME);
        isCanMove = isCanAttack = isCanEvasion = true;
    }

    private void StartEvasion()
    {
        Vector3 goalPos = transform.position + agent.velocity * 2;
        agent.SetDestination(goalPos);
        agent.speed = 5;
        state = PlayerState.Roll;
    }

    public void EndEvasion()
    {
        agent.speed = 3;
        state = PlayerState.Idle;
    }
}
