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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "MovingObject")
        {
            CanMoveObject moveGo = col.GetComponent<CanMoveObject>();

            if (@event.Equals(Event.PuzzleForR4))
            {
                isDone = true;
                if (MapCtrl.Instance.CheckObjetsComplete())
                {
                    // R4 전용 퍼즐 완료
                    EventCtrl.Instance.CheckEvent(EventTiming.MoveObject);
                    MapCtrl.Instance.SetObjetsComplete(true);
                    CutSceneCtrl.Instance.StartCutScene(14);
                }
            }
            else
            {
                if (moveGo.@event == this.@event)
                {
                    EventCtrl.Instance.CheckEvent(EventTiming.MoveObject);
                    moveGo.isDone = true;
                    this.isDone = true;
                }
            }
        }
    }
}
