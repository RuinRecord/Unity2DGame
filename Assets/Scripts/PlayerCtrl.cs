using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerState
{
    Idle,
    Walk,
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
    public PlayerState state;

    private bool isCanMove, isCanAttack, isCanEvasion;
    public float max_HP, cur_HP;
    public float max_MP, cur_MP;

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

        isCanMove = true;
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
            // 회피
            StartCoroutine("EvasionCoolTime");
        }

        SetState();
        SetAnimation();
    }

    private void SetState()
    {
        if (agent.remainingDistance < 0.01f)
            state = PlayerState.Idle;
        else
            state = PlayerState.Walk;
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case PlayerState.Idle:
                animator.SetBool("isWalk", false);
                break;
            case PlayerState.Walk:
                animator.SetBool("isWalk", true);
                if (agent.velocity.x >= 0f)
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                else
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
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
}
