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
            Chase();
        }
        else
        {
            // 감지 실패
            switch (state)
            {
                case MonsterState.Walk:
                    if (agent.remainingDistance < 0.01f)
                        state = MonsterState.Idle;
                    break;
                case MonsterState.Chase:
                    // 만약 추적 상태였다면 순찰 상태로 전환
                    if (agent.remainingDistance < 0.01f)
                        StartPatrol();
                    break;
            }
        }

        SetAnimation();
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
