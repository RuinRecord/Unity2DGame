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
            return; // 죽으면 아무것도 하지 못함

        isRecognized = Recognize();
        if (isRecognized)
        {
            Chase();
            switch (state)
            {
                case MonsterState.Chase:
                    float distance = Vector2.Distance(PlayerCtrl.instance.transform.position, transform.position);
                    if (distance < 1f)
                        Attack();
                    break;
            }
        }
        else
        {
            switch (state)
            {
                case MonsterState.Walk:
                    if (agent.remainingDistance < 0.01f)
                        state = MonsterState.Idle;
                    break;
                case MonsterState.Chase:
                    if (agent.remainingDistance < 0.01f)
                        StartPatrol();
                    break;
            }
        }

        SetAnimation();
    }
}
