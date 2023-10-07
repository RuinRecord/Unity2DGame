using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 오브젝트 식별 번호 </summary>
    public int code;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 만약 플레이어가 충돌 범위 안으로 들어온다면
        if (col.transform.tag.Equals("Player_M") || col.transform.tag.Equals("Player_W"))
        {
            // 이 오브젝트를 최근 감지 상호작용 오브젝트로 설정
            PlayerCtrl.instance.currentInteractionObject = this;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // 만약 플레이어가 충돌 범위 밖으로 나간다면
        if (col.transform.tag.Equals("Player_M") || col.transform.tag.Equals("Player_W"))
        {
            // 최근 상호작용 오브젝트 비우기
            PlayerCtrl.instance.currentInteractionObject = null;
        }
    }
}
