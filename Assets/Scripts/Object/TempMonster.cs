using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            return; // 죽은 상태면 Return
        // 플레이어 감지
        isRecognized = Recognize();

        if (isRecognized)
        {
            // 감지 성공 => 추적
            // Chase();
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

    protected override void Dead()
    {
        base.Dead();

        ObjectPool.ReturnObject(ObjectType.TempMonster, this);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.transform.tag.Equals("Player"))
        {
            switch (state)
            {
                case MonsterState.Chase:
                    // 플레이어 공격
                    Attack();
                    break;
            }
        }
    }
}
