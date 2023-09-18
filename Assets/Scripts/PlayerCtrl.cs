using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ���� �÷��̾��� ����
/// </summary>
public enum PlayerType
{
    MEN,
    WOMEN
}

/// <summary>
/// ���� �÷��̾��� ����
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
/// ���� �÷��̾��� ���� �ڼ� (�����ΰ��� ��ȿ)
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
    /// <summary> �ʴ� ȸ���ϴ� ���� ��ġ </summary>
    private const float MP_CHARGE_SPEED = 1f;


    /// <summary> ȸ�� ��Ÿ�� �ð� </summary>
    private const float EVASION_COOLTIME = 0.5f;


    /// <summary> ȸ�� �̵� ���� </summary>
    private const float EVASION_FORCE = 2f;


    /// <summary> ���� ������ �����ϱ� ���� ��¡ �ð� </summary>
    private const float STRONG_ATTACK_TIME = 3f;


    /// <summary> PlayerCtrl �̱��� ���� </summary>
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
        state = PlayerState.IDLE;

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
        if (state.Equals(PlayerState.DEAD))
            return; // ���� ������ ��� ��� ���� �Ұ�

        // State�� ���� �ൿ ����
        StateFunc();

        // ���� ȸ��
        ChargeMana();

        // �̵�
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            Vector2 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            goalVec = GetValidDestination(destination);

            if (isAttackCharge || attack_type != -1)
                SetMove(goalVec, 1.5f); // ���� ��¡ ���� ��� ���� �̵�
            else
                SetMove(goalVec, 3f); // �� �� ���� �̵�
        }

        // �����ΰ� ���
        if (playerType.Equals(PlayerType.MEN))
        {
            // ����
            if (isCanAttack)
            {
                // ���� ��ư �� ������ ��
                if (Input.GetKey(KeyCode.Q))
                {
                    // ���� ��¡ �ð� �ǽð����� ����
                    attack_clickTime += Time.deltaTime;

                    // ���� ��¡ ù ����
                    if (!isAttackCharge)
                    {
                        SetMoveSpeed(1.5f);
                        isAttackCharge = true;
                        isCanEvasion = false;
                        animator.SetBool("isAttack", true);
                    }
                }
                // ���� ��ư ��
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    // ���� ����
                    StartAttack();
                }
            }

            // ȸ��
            if (isCanEvasion && Input.GetKeyDown(KeyCode.W))
            {
                // ȸ�� ���׹̳� üũ
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
            // ����
            if (isCanCapture && Input.GetKey(KeyCode.Q))
            {
                // ���� ����
                StartCapture();
            }
        }
    }


    /// <summary>
    /// ������ ȸ����Ű�� �Լ��̴�.
    /// </summary>
    private void ChargeMana()
    {
        cur_MP += Time.deltaTime * MP_CHARGE_SPEED;
        if (cur_MP > max_MP)
            cur_MP = max_MP;
    }
    

    /// <summary>
    /// '_destination' ���� ��ġ ���ͷ� �÷��̾ �̵� �����ϵ��� �����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="_destination">���� ��ġ ����</param>
    /// <returns>�̵� ������ ���� ��ġ ����</returns>
    private Vector2 GetValidDestination(Vector2 _destination)
    {
        RaycastHit2D hit;
        goalVec = _destination;
        moveVec = goalVec - (Vector2)this.transform.position;
        moveVec.Set(moveVec.x, moveVec.y);

        // �� �� ���� ������ ���� ���
        if (Physics2D.Raycast(_destination, Vector3.forward, 10f, 64))
        {
            // �÷��̾� ��ġ���� ���� ��ġ�� ray�� �߻� �浹 �˻�
            // �浹 �������� �ӽ� ���� ��ġ �缳��
            if (hit = Physics2D.Raycast(this.transform.position, moveVec, moveVec.magnitude, 64))
                goalVec = hit.point;
        }

        return goalVec;
    }


    /// <summary>
    /// 'moveSpeed'�� �ӵ��� 'destination' ���� ��ġ�� �ִ� ��θ� ���� �÷��̾ �̵���Ű�� �Լ��̴�.
    /// </summary>
    /// <param name="_destination">���� ��ġ ����</param>
    /// <param name="_moveSpeed">�÷��̾��� �̵��ӵ�</param>
    private void SetMove(Vector2 _destination, float _moveSpeed)
    {
        state = PlayerState.WALK;
        agent.SetDestination(_destination);
        SetMoveSpeed(_moveSpeed);
    }


    /// <summary>
    /// �÷��̾��� �̵� �ӵ��� 'moveSpeed'���� �����ϴ� �Լ��̴�.
    /// </summary>
    /// <param name="_moveSpeed">������ �÷��̾� �̵� �ӵ�</param>
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
            case PlayerState.IDLE:
                break;
            case PlayerState.WALK:
                // �������� ���� �Ÿ��� ������ Idle�� ����
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
    /// �÷��̾��� ���¿� ���� �ִϸ��̼��� �����ϴ� �Լ��̴�.
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
    /// ȸ�� ��Ÿ�� ���� �Լ�
    /// </summary>
    IEnumerator EvasionCoolTime()
    {
        isCanMove = isCanAttack = isCanEvasion = false;

        yield return new WaitForSeconds(EVASION_COOLTIME);
        // 'EVASION_COOLTIME' �ʰ� �帥 �� �Ʒ� ������ ����˴ϴ�.

        isCanMove = isCanAttack = isCanEvasion = true;
    }


    /// <summary>
    /// ȸ�� ����� �����ϴ� �Լ��̴�.
    /// </summary>
    private void StartEvasion()
    {
        float distance = EVASION_FORCE;

        // ȸ�� �������� ��ֹ��� �ִ��� üũ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -moveVec, EVASION_FORCE, 64);

        // ��ֹ��� �ִٸ� �Ÿ��� ����
        if (hit) distance = hit.distance;
        Debug.Log(distance);

        // ���� ����: �ֱ� �̵��� ���� ���͸� �������� ȸ�� ���� ��ġ ����
        if (playerState.Equals(PlayerState.IDLE))
            goalVec = (Vector2)transform.position - moveVec.normalized * distance;
        // �� �� ����: ���� �̵� ���� ���� ���͸� �������� ȸ�� ���� ��ġ ����
        else
            goalVec = (Vector2)transform.position - (Vector2)agent.velocity.normalized * distance;

        // ȸ�� ����
        SetMove(goalVec, 6f);
        state = PlayerState.EVASION;
    }


    /// <summary>
    /// ȸ�� �ִϸ��̼��� �������� �˸��� �ִϸ��̼� �̺�Ʈ �Լ� (Animator Tab���� ���)
    /// </summary>
    public void EndEvasion()
    {
        SetMove(transform.position, 3f);
        state = PlayerState.IDLE;
    }


    /// <summary>
    /// ���� ���� ���� ������ ���� Ư�� ������ �����ϴ� �Լ��̴�.
    /// </summary>
    private void StartAttack()
    {
        // ���� 'STRONG_ATTACK_TIME' �̻� ��¡�ߴٸ� ���� ���� ����
        if (attack_clickTime >= STRONG_ATTACK_TIME)
        {
            // �Ϲ� ���� 1ȸ �̻��� ���
            if (attack_count > 0)
            {
                // �޺� ���� ������ ����
                attack_type = 2;
            }
            // �Ϲ� ���� 0ȸ �� ���
            else
            {
                // ���� ���� ������ ����
                attack_type = 1;
                cur_MP -= 3;
            }

            // ���� ȸ�� �ʱ�ȭ
            attack_count = 0;
        }
        else
        {
            // �Ϲ� ���� ������ ����
            attack_type = 0;
            cur_MP -= 2;

            // �Ϲ� ���� 3ȸ �� ���� ȸ�� �ʱ�ȭ
            if (++attack_count > 2)
                attack_count = 0;
        }

        // ���� ����
        isCanMove = isCanAttack = isAttackCharge = false;
        attack_clickTime = 0f;
        state = PlayerState.ATTACK;
        animator.SetInteger("AttackType", attack_type);
    }


    /// <summary>
    /// (�ִϸ��̼� �̺�Ʈ �Լ�) ���� �ִϸ��̼��� ����Ǿ��� �� �ߵ�, ���� ���� �����͸� �ʱ�ȭ�ϴ� �Լ��̴�.
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
    /// ���� ����� �����ϴ� �Լ��̴�.
    /// </summary>
    private void StartCapture()
    {
        // ���ڸ��� ������ ����
        agent.SetDestination(this.transform.position);
        isCanMove = isCanCapture = false;
        state = PlayerState.CAPTURE;
    }


    /// <summary>
    ///  (�ִϸ��̼� �̺�Ʈ �Լ�) ���� �ִϸ��̼��� ����Ǿ��� �� �ߵ�, ���� ���� �����͸� �ʱ�ȭ�ϴ� �Լ��̴�.
    /// </summary>
    public void EndCapture()
    {
        state = PlayerState.IDLE;
        isCanMove = isCanCapture = true;
    }
}
