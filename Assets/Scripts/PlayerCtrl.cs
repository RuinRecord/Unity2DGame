using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum PlayerType
{
    MEN,
    WOMEN
}

public enum PlayerState
{
    Idle,
    Walk,
    Evasion,
    Attack,
    CAPTURE,
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
    private const float MP_CHARGE_SPEED = 1f;
    private const float EVASION_COOLTIME = 0.5f;
    private const float EVASION_FORCE = 2f;
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

    [SerializeField]
    private PlayerType playerType;

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

    /// <summary> �÷��̾ �ش� ����� ����� �� �ִ� �����ΰ�? </summary>
    private bool isCanMove, isCanAttack, isCanEvasion;
    public bool isCanCapture;


    /// <summary> ���� ���� ��¡ ���ΰ�? </summary>
    private bool isAttackCharge;


    /// <summary> �÷��̾� MAX HP </summary>
    public float max_HP;


    /// <summary> �÷��̾� MAX MP </summary>
    public float max_MP;


    /// <summary> �ֱ� �÷��̾��� ���� ���� </summary>
    private Vector2 moveVec;


    /// <summary> �ֱ� �÷��̾��� ���� ���� </summary>
    private Vector2 goalVec;


    /// <summary> ���� ���� Count ���� </summary>
    private int attack_count;

    /// <summary> ���� ���� ���¶�� � ������ ���� ���ΰ�? </summary>
    private int attack_type;


    /// <summary> ���� �غ� �ð� </summary>
    private float attack_clickTime;


    /// <summary> �÷��̾� ���� HP </summary>
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


    /// <summary> �÷��̾� ���� MP </summary>
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
        isCanCapture = isAttackCharge = false;
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
        // ���� ȸ��
        cur_MP += Time.deltaTime * MP_CHARGE_SPEED;
        if (cur_MP > max_MP)
            cur_MP = max_MP;

        // State�� ���� �ൿ ����
        StateFunc();

        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            // �̵�
            RaycastHit2D hit;
            Vector2 mouseVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            goalVec = mouseVec;
            moveVec = goalVec - (Vector2)this.transform.position;
            moveVec.Set(moveVec.x, moveVec.y);

            // �� �� ���� ������ ���� ���, �� �� �ִ� ���� ����� ������ ��ǥ �缳��
            if (Physics2D.Raycast(mouseVec, Vector3.forward, 10f, 64))
            {
                if (hit = Physics2D.Raycast(this.transform.position, moveVec, moveVec.magnitude, 64))
                    goalVec = hit.point;
            }

            if (isAttackCharge || attack_type != -1)
                SetMove(goalVec, 1.5f); // ��¡ ���̰ų� ���� ���� ��� ���� �̵�
            else
                SetMove(goalVec, 3f);
        }

        // ���� ���ΰ� ���
        if (playerType.Equals(PlayerType.MEN))
        {
            if (isCanAttack)
            {
                // ����
                if (Input.GetKey(KeyCode.Q))
                {
                    // ���� ��ư ������ ��
                    attack_clickTime += Time.deltaTime;
                    if (!isAttackCharge)
                    {
                        // ���� ��¡ ����
                        SetMoveSpeed(1.5f);
                        isAttackCharge = true;
                        isCanEvasion = false;
                        animator.SetBool("isAttack", true);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    // ���� ��ư ��
                    StartAttack();
                }
            }

            if (isCanEvasion && Input.GetKeyDown(KeyCode.W))
            {
                // ȸ��
                if (cur_MP >= 1f)
                {
                    // ȸ�� ���׹̳� ����
                    cur_MP -= 1f;

                    // ȸ�� ��Ÿ�� �� ��� ����
                    StartCoroutine("EvasionCoolTime");
                    StartEvasion();
                }
            }
        }
        // ���� ���ΰ� ���
        else if (playerType.Equals(PlayerType.WOMEN))
        {
            if (isCanCapture)
            {
                // ���� �Կ�
                if (Input.GetKey(KeyCode.Q))
                    StartCapture();
            }
        }
    }

    private void SetMove(Vector2 _destination, float _moveSpeed)
    {
        state = PlayerState.Walk;
        agent.SetDestination(_destination);
        SetMoveSpeed(_moveSpeed);
    }

    private void SetMoveSpeed(float _moveSpeed)
    {
        agent.speed = _moveSpeed;
    }

    /// <summary>
    /// �÷��̾��� ���¿� ���� �⺻ �ൿ ����� �����ϴ� �Լ��̴�.
    /// </summary>
    private void StateFunc()
    {
        switch(state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walk:
                // �������� ���� �Ÿ��� ������ Idle�� ����
                if (agent.remainingDistance < Time.deltaTime)
                    state = PlayerState.Idle;
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
    /// �÷��̾��� ���¿� ���� �ִϸ��̼��� �����ϴ� �Լ��̴�.
    /// </summary>
    private void SetAnimation()
    {
        switch (state)
        {
            case PlayerState.Idle:
                animator.SetBool("isWalk", false);
                animator.SetBool("isEvasion", false);
                animator.SetBool("isCapture", false);
                break;
            case PlayerState.Walk:
                animator.SetBool("isWalk", true);
                animator.SetBool("isEvasion", false);
                break;
            case PlayerState.Evasion:
                animator.SetBool("isWalk", false);
                animator.SetBool("isEvasion", true);
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.CAPTURE:
                animator.SetBool("isCapture", true);
                break;
        }
    }

    /// <summary>
    /// ȸ�� ��Ÿ�� ���� �Լ�
    /// </summary>
    IEnumerator EvasionCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;
        yield return new WaitForSeconds(EVASION_COOLTIME);
        isCanMove = isCanAttack = isCanEvasion = true;
    }

    /// <summary>
    /// ȸ�� ���·� �����ϴ� �Լ�
    /// </summary>
    private void StartEvasion()
    {
        float distance = EVASION_FORCE;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -moveVec, EVASION_FORCE, 64);
        if (hit) distance = hit.distance;
        Debug.Log(distance);

        if (playerState.Equals(PlayerState.Idle))
            goalVec = (Vector2)transform.position - moveVec.normalized * distance;
        else
            goalVec = (Vector2)transform.position - (Vector2)agent.velocity.normalized * distance;

        SetMove(goalVec, 6f);
        state = PlayerState.Evasion;
    }

    /// <summary>
    /// ȸ�� �ִϸ��̼��� �������� �˸��� �ִϸ��̼� �̺�Ʈ �Լ� (Animator Tab���� ���)
    /// </summary>
    public void EndEvasion()
    {
        SetMove(transform.position, 3f);
        state = PlayerState.Idle;
    }

    /// <summary>
    /// ���� ���� ���� ������ ���� Ư�� ������ �����ϴ� �Լ��̴�.
    /// </summary>
    private void StartAttack()
    {
        if (attack_clickTime >= STRONG_ATTACK_TIME)
        {
            if (attack_count > 0)
            {
                // �޺� ����
                attack_type = 2;
            }
            else
            {
                // ���� ����
                attack_type = 1;
                cur_MP -= 3;
            }
            attack_count = 0;
        }
        else
        {
            // �Ϲ� ����
            attack_type = 0;
            cur_MP -= 2;
            if (++attack_count > 2)
                attack_count = 0;
        }

        isCanMove = isCanAttack = isAttackCharge = false;
        attack_clickTime = 0f;
        state = PlayerState.Attack;
        animator.SetInteger("AttackType", attack_type);
    }

    public void EndAttack()
    {
        state = PlayerState.Idle;
        isCanMove = isCanAttack = isCanEvasion = true;
        attack_type = -1;
        animator.SetBool("isAttack", false);
        animator.SetInteger("AttackType", attack_type);
    }

    private void StartCapture()
    {
        agent.SetDestination(this.transform.position);
        isCanMove = isCanCapture = false;
        state = PlayerState.CAPTURE;
    }

    public void EndCapture()
    {
        state = PlayerState.Idle;
        isCanMove = isCanCapture = true;
    }
}
