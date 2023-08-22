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

        // target과 나 사이의 거리가 radius 보다 작다면
        if (lhs.magnitude <= radius)
        {
            // '타겟-나 벡터'와 '내 정면 벡터'를 내적
            float dot = Vector2.Dot(lhs.normalized, rhs.normalized);
            // 두 벡터 모두 단위 벡터이므로 내적 결과에 cos의 역을 취해서 theta를 구함
            float theta = Mathf.Acos(dot);
            // angleRange와 비교하기 위해 degree로 변환
            float degree = Mathf.Rad2Deg * theta;

            // 장애물 판별
            if (Physics2D.Raycast(transform.position, lhs, 3f, 64))
                return false;

            // 시야각 판별
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
        // 몬스터 공격 판정 생성
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
/// 몬스터 구분
/// 일반: 챕터 별로 등장하는 몬스터
/// 에픽: 숨겨진 맵에 등장하는 몬스터
/// </summary>
public enum MonsterQuality
{
    Normal,
    Epic
}

/// <summary>
/// 몬스터 타입
/// 짐승형: 짐승 형태의 몬스터
/// 기계형: 기계 형태의 몬스터
/// 인간형: 인간 형태의 몬스터
/// </summary>
public enum MonsterType
{
    Beast,
    Robot,
    Human
}

/// <summary>
/// 몬스터 등급
/// 일반: 낮은 시야각과 시야 범위
/// 오염된: 짐승형, 인간형에서 나타남, 더 넓은 시야각 & 공격력 체력 증가
/// 감염된: 기계형에서 나타남, 공격력 추격 인식 증가, 체력 감소
/// 보스: 보스 몬스터
/// </summary>
public enum MonsterGrade
{
    Normal,
    Polluted,
    Infected,
    Boss
}

/// <summary>
/// 공격 타입
/// 비선공: 피격 시 공격
/// 선공: 발견 시 공격
/// </summary>
public enum AttackType
{
    Non_Go_First,
    Go_First
}

/// <summary>
/// 현재 몬스터 상태
/// Idle: 가만히 있음
/// Walk: 순찰
/// Chase: 추격
/// Attack: 공격
/// Dead: 사망
/// </summary>
public enum MonsterState
{
    Idle,
    Walk,
    Chase,
    Attack,
    Dead
}