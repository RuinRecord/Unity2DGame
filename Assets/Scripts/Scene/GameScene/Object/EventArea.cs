using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : MonoBehaviour
{
    [SerializeField] private int eventCode;

    public bool isDone;

    private void Start()
    {
        isDone = false;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        Debug.Log(col.tag);
        if (col.tag == "MovingObject")
        {
            CanMoveObject moveGo = col.GetComponent<CanMoveObject>();
            if (moveGo.eventCode == this.eventCode)
            {
                EventCtrl.Instance.CheckEvent(EventType.MoveObject);
                moveGo.isDone = true;
                this.isDone = true;
            }
        }
    }
}
