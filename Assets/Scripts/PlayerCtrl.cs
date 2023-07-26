using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PLAYER_STATE
{
    IDLE,
    WALK
}

public class PlayerCtrl : MonoBehaviour
{
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
    public PLAYER_STATE state;

    private bool isCanMove;

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
        state = PLAYER_STATE.IDLE;

        isCanMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanMove && Input.GetMouseButtonDown(0)) 
        {
            Vector2 move_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            agent.destination = move_pos;
        }

        SetState();
        SetAnimation();
    }

    private void SetState()
    {
        if (agent.remainingDistance < 0.01f)
            state = PLAYER_STATE.IDLE;
        else
            state = PLAYER_STATE.WALK;
    }

    private void SetAnimation()
    {
        switch (state)
        {
            case PLAYER_STATE.IDLE:
                animator.SetBool("isWalk", false);
                break;
            case PLAYER_STATE.WALK:
                animator.SetBool("isWalk", true);
                break;
        }
    }
}
