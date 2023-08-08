using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    /// <summary> �÷��̾ �ش� ����� ����� �� �ִ� �����ΰ�? </summary>
    private bool isCanMove, isCanAttack, isCanEvasion;


    /// <summary> ���� ���� ��¡ ���ΰ�? </summary>
    private bool isAttackCharge;


    /// <summary> �÷��̾� MAX HP </summary>
    public float max_HP;


    /// <summary> �÷��̾� MAX MP </summary>
    public float max_MP;


    /// <summary> �ֱ� �÷��̾ �̵��� ��ġ ���� </summary>
    private Vector2 moveVec;


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
        isAttackCharge = false;
        max_HP = cur_HP = 100f;
        max_MP = cur_MP = 10f;
        moveVec = Vector2.right;
        attack_count = 0;
        attack_type = -1;
        attack_clickTime = 0f;

        animator.SetInteger("AttackType", attack_type);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            // �̵�
            Vector2 move_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveVec = move_pos - (Vector2)transform.position;
            if (isAttackCharge || attack_type != -1)
                SetMove(move_pos, 1.5f); // ��¡ ���̰ų� ���� ���� ��� ���� �̵�
            else
                SetMove(move_pos, 3f);
        }

        if (isCanAttack)
        {
            // ����
            if (Input.GetKey(KeyCode.Q))
            {
                // ���� ��ư ����
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

        // State�� ���� �ൿ ����
        StateFunc();

        // �÷��̾ �ٶ󺸴� �������� �̹��� �¿� ��ȯ
        if (moveVec.x > 0.01f)
            this.transform.localScale = Vector3.one;
        else if (moveVec.x < -0.01f)
            this.transform.localScale = new Vector3(-1f, 1f, 1f);

        // ���� ȸ��
        cur_MP += Time.deltaTime * MP_CHARGE_SPEED;
        if (cur_MP > max_MP)
            cur_MP = max_MP;
    }

    private void SetMove(Vector2 _destination, float _moveSpeed)
    {
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
            case PlayerState.Walk:
                // ���콺 Ŭ���� ��ġ���� ���� �Ÿ��� ���� State ����
                state = (agent.remainingDistance < 0.01f) ? PlayerState.Idle : PlayerState.Walk;
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
            case PlayerState.Attack:
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
        Vector2 goalPos = (Vector2)transform.position - moveVec.normalized * EVASION_FORCE;
        SetMove(goalPos, 6f);
        state = PlayerState.Roll;
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
            }
            attack_count = 0;
        }
        else
        {
            // �Ϲ� ����
            attack_type = 0;
            if (++attack_count > 2)
                attack_count = 0;
        }

        isCanAttack = false;
        isAttackCharge = false;
        attack_clickTime = 0f;
        playerState = PlayerState.Attack;
        animator.SetInteger("AttackType", attack_type);
    }

    public void EndAttack()
    {
        playerState = PlayerState.Idle;
        isCanAttack = true;
        isCanEvasion = true;
        attack_type = -1;
        animator.SetBool("isAttack", false);
        animator.SetInteger("AttackType", attack_type);
    }
}
