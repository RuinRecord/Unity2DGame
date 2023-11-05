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
    /// <summary> 스폰 몬스터의 경우 spawner를 가집니다. (이때, 스폰 몬스터가 아니면 null) </summary>
    public Spawner spawner;

    protected Animator animator;
    protected NavMeshAgent agent;

    /// <summary> 몬스터의 구분 </summary>
    [SerializeField] protected MonsterQuality quality;

    /// <summary> 몬스터의 종류 </summary>
    [SerializeField] protected MonsterType type;

    /// <summary> 몬스터의 등급 </summary>
    [SerializeField] protected MonsterGrade grade;

    /// <summary> 몬스터의 공격 타입 </summary>
    [SerializeField] protected AttackType attackType;

    /// <summary> 몬스터의 현재 상태 </summary>
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

    /// <summary> 플레이어 감지 시야각 </summary>
    [SerializeField] private float angleRange = 60f;

    /// <summary> 플레이어 감지 거리 </summary>
    [SerializeField] private float radius = 3f;

    /// <summary> 몬스터 공격력 </summary>
    [SerializeField] public float damage = 10f;

    /// <summary> 몬스터 이동속도 </summary>
    [SerializeField] private float moveSpeed = 1f;

    /// <summary> 현재 몬스터가 플레이어를 감지 했는가에 대한 여부 </summary>
    [SerializeField]
    protected bool isRecognized;

    /// <summary> 몬스터가 공격했을 때, 공격 판정 오브젝트가 생성되는 위치 정보 </summary>
    [SerializeField]
    private Transform attackPoint;


    /// <summary> 플레이어를 감지하는 함수 </summary>
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


    /// <summary>
    /// 플레이어를 추적하는 함수이다.
    /// </summary>
    protected virtual void Chase()
    {
        // 이동을 멈춤
        StopPatrol();

        // 추적 설정
        state = MonsterState.Chase;
        agent.SetDestination(PlayerCtrl.instance.transform.position);
        agent.speed = 2f;
    }


    /// <summary>
    /// 플레이어를 공격하는 함수이다. 이후 공격 판정은 아래 이벤트 함수로 수행된다.
    /// </summary>
    protected virtual void Attack()
    {
        state = MonsterState.Attack;
        agent.SetDestination(transform.position);
    }


    /// <summary>
    /// (애니메이션 이벤트 함수 전용) 공격 애니메이션에서 공격 이펙트가 터지는 프레임 때 발동하는 함수이다.
    /// </summary>
    protected virtual void CheckDamage()
    {
        // 몬스터 공격 판정 생성
        MonsterAttack attack = ObjectPool.instance.CreateObject<MonsterAttack>(ObjectType.MonsterAttack, ObjectPool.instance.objectTr, attackPoint.position);
        attack.destroyTime = 0.05f;
        attack.damage = this.damage;
    }


    /// <summary>
    /// (애니메이션 이벤트 함수 전용) 공격 애니메이션이 종료했을 때 발동하는 함수이다.
    /// </summary>
    protected virtual void EndAttack()
    {
        // 공격이 끝나면 다시 추적을 시작
        Chase();
    }


    /// <summary>
    /// 순찰 코루틴을 시작하는 함수이다.
    /// </summary>
    protected virtual void StartPatrol()
    {
        StopCoroutine("Patrol");
        StartCoroutine("Patrol");
    }


    /// <summary>
    /// 순찰 코루틴을 종료하는 함수이다.
    /// </summary>
    protected virtual void StopPatrol()
    {
        StopCoroutine("Patrol");
    }


    /// <summary>
    /// 일정 시간마다 코루틴을 돌며 순찰을 수행하는 함수이다.
    /// </summary>
    IEnumerator Patrol()
    {
        float randX, randY;
        Vector3 destination;

        do
        {
            // 순찰 구역 탐색
            randX = Random.Range(0.75f, 1.5f);
            randY = Random.Range(0.75f, 1.5f);
            destination = transform.position + new Vector3(Mathf.Sign(Random.Range(-1f, 1f)) * randX, Mathf.Sign(Random.Range(-1f, 1f)) * randY);
            yield return null;
        } while (Physics2D.Raycast(destination, Vector3.forward, 10f, 64)); // 이동 가능 지역인지 체크 -> 불가능한 지역이면 재탐색

        // 순찰 구역으로 이동
        agent.SetDestination(destination);
        agent.speed = 1f;
        state = MonsterState.Walk;

        yield return new WaitForSeconds(Random.Range(4f, 6f));
        // 4 ~ 6초 후 새롭게 순찰 구역 탐색

        StartCoroutine("Patrol");
    }


    /// <summary>
    /// 현재 몬스터의 상태에 따라 애니메이션을 처리한다.
    /// </summary>
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
                if (agent.velocity.x > 0.05f)
                    this.transform.localScale = new Vector3(1f, 1f, 1f);
                else if (agent.velocity.x < -0.05f)
                    this.transform.localScale = new Vector3(-1f, 1f, 1f);
                break;
            case MonsterState.Attack:
                animator.SetBool("isAttack", true);
                break;
        }
    }


    /// <summary>
    /// 몬스터가 죽으면 발동하는 함수이다.
    /// </summary>
    public virtual void Dead()
    {
        state = MonsterState.Dead;
        agent.SetDestination(this.transform.position);
        animator.SetBool("isDead", true);
        StopPatrol();

        if (spawner != null)
        {
            // 스폰 몬스터라면 돌려보내기
            spawner.RemoveObject(this.gameObject);
        }
    }
}


/// <summary> 몬스터 구분 </summary>
public enum MonsterQuality
{
    Normal,     /// 일반: 챕터 별로 등장하는 몬스터
    Epic        /// 에픽: 숨겨진 맵에 등장하는 몬스터
}


/// <summary> 몬스터 타입 </summary>
public enum MonsterType
{
    Beast,      /// 짐승형: 짐승 형태의 몬스터
    Robot,      /// 기계형: 기계 형태의 몬스터
    Human       /// 인간형: 인간 형태의 몬스터
}


/// <summary> 몬스터 등급 </summary>
public enum MonsterGrade
{
    Normal,     /// 일반: 낮은 시야각과 시야 범위
    Polluted,   /// 오염된: 짐승형, 인간형에서 나타남, 더 넓은 시야각 & 공격력 체력 증가
    Infected,   /// 감염된: 기계형에서 나타남, 공격력 추격 인식 증가, 체력 감소
    Boss        /// 보스: 보스 몬스터
}


/// <summary> 공격 타입 </summary>
public enum AttackType
{
    Non_Go_First,   /// 비선공: 피격 시 공격
    Go_First        /// 선공: 발견 시 공격
}


/// <summary> 현재 몬스터 상태 </summary>
public enum MonsterState
{
    Idle,       /// Idle: 가만히 있음
    Walk,       /// Walk: 순찰
    Chase,      /// Chase: 추격
    Attack,     /// Attack: 공격
    Dead        /// Dead: 사망
}