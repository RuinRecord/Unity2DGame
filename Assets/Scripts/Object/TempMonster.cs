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
            return; // ���� ���¸� Return

        // �÷��̾� ����
        isRecognized = Recognize();

        if (isRecognized)
        {
            // ���� ���� => ����
            Chase();
        }
        else
        {
            // ���� ����
            switch (state)
            {
                case MonsterState.Walk:
                    if (agent.remainingDistance < 0.01f)
                        state = MonsterState.Idle;
                    break;
                case MonsterState.Chase:
                    // ���� ���� ���¿��ٸ� ���� ���·� ��ȯ
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
                    // �÷��̾� ����
                    Attack();
                    break;
            }
        }
    }
}