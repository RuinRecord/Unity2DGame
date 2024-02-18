using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea : MonoBehaviour
{
    [SerializeField] private Event @event;

    public bool isDone;

    private void Start()
    {
        isDone = false;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "MovingObject")
        {
            CanMoveObject moveGo = col.GetComponent<CanMoveObject>();
            if (moveGo.@event == this.@event)
            {
                EventCtrl.Instance.CheckEvent(EventTiming.MoveObject);
                moveGo.isDone = true;
                this.isDone = true;
            }
        }
    }
}
