using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 임시 몬스터 클래스이다.
/// </summary>
public class TempMonster : Monster
{
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateUpAxis = false;
        agent.updateRotation = false;
        state = MonsterState.Idle;
        StartPatrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == MonsterState.Dead)
            return; // 죽은 상태면 아래 기능 수행 안함

        // 플레이어 감지
        isRecognized = Recognize();

        if (isRecognized)
        {
            // 감지 성공 => 추적
            Chase();
        }
        else
        {
            // 감지 실패
            switch (state)
            {
                case MonsterState.Walk:
                    // 현재 방향에서 x의 부호에 따른 좌우 방향 전환
                    this.transform.localScale = (agent.velocity.x >= 0f) ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);

                    // 만약 남은 거리가 일정 이하라면 Walk -> Idle로 변경
                    if (agent.remainingDistance < 0.01f)
                        state = MonsterState.Idle;
                    break;
                case MonsterState.Chase:
                    // 만약 남은 거리가 일정 이하라면 Chase를 멈추고 Patrol로 변경
                    if (agent.remainingDistance < 0.01f)
                        StartPatrol();
                    break;
            }
        }
    }

    /// <summary>
    /// 사망했을 경우 발동하는 함수이다. 오브젝트 풀링을 위해 리턴을 수행한다.
    /// </summary>
    protected override void Dead()
    {
        base.Dead();

        ObjectPool.ReturnObject(ObjectType.TempMonster, this);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // 만약 충돌 감지된 오브젝트가 플레이어라면
        if (col.transform.tag.Equals("Player"))
        {
            switch (state)
            {
                case MonsterState.Chase:
                    // 공격을 수행
                    Attack();
                    break;
            }
        }
    }
}
