using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class Monster : MonoBehaviour
{
    protected Animator animator;
    protected NavMeshAgent agent;

    [SerializeField] protected MonsterQuality quality;
    [SerializeField] protected MonsterType type;
    [SerializeField] protected MonsterGrade grade;
    [SerializeField] protected AttackType attackType;
    [SerializeField] private MonsterState State;

    protected MonsterState state
    {
        set
        {
            State = value;
            SetAnimation();
        }
        get { return State; }
    }

    [SerializeField] private float angleRange = 60f;
    [SerializeField] private float radius = 3f;
    [SerializeField] public float damage = 10f;
    [SerializeField] private float moveSpeed = 1f;

    [SerializeField]
    protected bool isRecognized;

    [SerializeField]
    private Transform attackPoint;

    public virtual bool Recognize()
    {
        Vector2 lhs = PlayerCtrl.instance.transform.position - transform.position;
        Vector2 rhs = transform.right * Mathf.Sign(transform.localScale.x);
        if (state != MonsterState.Idle)
            rhs = agent.velocity;

        // target�� �� ������ �Ÿ��� radius ���� �۴ٸ�
        if (lhs.magnitude <= radius)
        {
            // 'Ÿ��-�� ����'�� '�� ���� ����'�� ����
            float dot = Vector2.Dot(lhs.normalized, rhs.normalized);
            // �� ���� ��� ���� �����̹Ƿ� ���� ����� cos�� ���� ���ؼ� theta�� ����
            float theta = Mathf.Acos(dot);
            // angleRange�� ���ϱ� ���� degree�� ��ȯ
            float degree = Mathf.Rad2Deg * theta;

            // ��ֹ� �Ǻ�
            if (Physics2D.Raycast(transform.position, lhs, 3f, 64))
                return false;

            // �þ߰� �Ǻ�
            if (degree <= angleRange / 2f)
                return true;
            else
                return false;
        }
        return false;
    }

    protected virtual void Chase()
    {
        state = MonsterState.Chase;
        agent.SetDestination(PlayerCtrl.instance.transform.position);
        agent.speed = 2f;
        StopPatrol();
    }

    protected virtual void Attack()
    {
        state = MonsterState.Attack;
        agent.SetDestination(transform.position);
    }

    protected virtual void CheckDamage()
    {
        // ���� ���� ���� ����
        MonsterAttack attack = ObjectPool.GetObject<MonsterAttack>(ObjectType.MonsterAttack, ObjectPool.instance.objectTr, attackPoint.position);
        attack.destroyTime = 0.025f;
        attack.damage = this.damage;
    }

    protected virtual void EndAttack()
    {
        Chase();
    }

    protected virtual void StartPatrol()
    {
        StopCoroutine("Patrol");
        StartCoroutine("Patrol");
    }

    protected virtual void StopPatrol()
    {
        StopCoroutine("Patrol");
    }

    IEnumerator Patrol()
    {
        float randX = Random.Range(0.75f, 1.5f);
        float randY = Random.Range(0.75f, 1.5f);
        Vector3 randVec = new Vector3(Mathf.Sign(Random.Range(-1f, 1f)) * randX, Mathf.Sign(Random.Range(-1f, 1f)) * randY);
        agent.SetDestination(this.transform.position + randVec);
        agent.speed = 1f;
        state = MonsterState.Walk;

        yield return new WaitForSeconds(Random.Range(4f, 6f));

        StartCoroutine("Patrol");
    }

    protected virtual void SetAnimation()
    {
        switch (state)
        {
            case MonsterState.Idle:
                animator.SetBool("isWalk", false);
                animator.SetBool("isAttack", false);
                break;
            case MonsterState.Walk:
            case MonsterState.Chase:
                animator.SetBool("isWalk", true);
                animator.SetBool("isAttack", false);
                if (agent.velocity.x >= 0f)
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                else
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
            case MonsterState.Attack:
                animator.SetBool("isAttack", true);
                break;
        }
    }

    protected virtual void Dead()
    {
        animator.SetBool("isDead", true);
    }
}

/// <summary>
/// ���� ����
/// �Ϲ�: é�� ���� �����ϴ� ����
/// ����: ������ �ʿ� �����ϴ� ����
/// </summary>
public enum MonsterQuality
{
    Normal,
    Epic
}

/// <summary>
/// ���� Ÿ��
/// ������: ���� ������ ����
/// �����: ��� ������ ����
/// �ΰ���: �ΰ� ������ ����
/// </summary>
public enum MonsterType
{
    Beast,
    Robot,
    Human
}

/// <summary>
/// ���� ���
/// �Ϲ�: ���� �þ߰��� �þ� ����
/// ������: ������, �ΰ������� ��Ÿ��, �� ���� �þ߰� & ���ݷ� ü�� ����
/// ������: ��������� ��Ÿ��, ���ݷ� �߰� �ν� ����, ü�� ����
/// ����: ���� ����
/// </summary>
public enum MonsterGrade
{
    Normal,
    Polluted,
    Infected,
    Boss
}

/// <summary>
/// ���� Ÿ��
/// �񼱰�: �ǰ� �� ����
/// ����: �߰� �� ����
/// </summary>
public enum AttackType
{
    Non_Go_First,
    Go_First
}

/// <summary>
/// ���� ���� ����
/// Idle: ������ ����
/// Walk: ����
/// Chase: �߰�
/// Attack: ����
/// Dead: ���
/// </summary>
public enum MonsterState
{
    Idle,
    Walk,
    Chase,
    Attack,
    Dead
}