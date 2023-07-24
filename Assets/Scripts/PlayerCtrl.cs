using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private bool isCanMove;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent= GetComponent<NavMeshAgent>();

        agent.updateUpAxis = false;
        agent.updateRotation = false;
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
    }
}
