using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여주인공이 조사 상호작용이 가능한 오브젝트 클래스이다.
/// </summary>
public class CaptureObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// 쉐이더가 포함된 메터리얼
    /// 0 = Sprite-Default (외곽선 없음)
    /// 1 = Sprite-Outline (외곽선 있음)
    /// </summary>
    [SerializeField]
    private Material[] materials;

    /// <summary> 조사 이벤트 식별 번호 </summary>
    public int code;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Default Setting - 외곽선 없음
        spriteRenderer.material = materials[0];
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        // 만약 플레이어가 충돌 범위 안으로 들어온다면
        if (col.transform.tag.Equals("Player_W"))
        {
            // 외곽선 있는 메터리얼로 변경
            spriteRenderer.material = materials[1];

            // 플레이어 조사 기능 활성화
            PlayerCtrl.instance.isCanCapture = true;

            // 조사 UI 이미지 켜기
            UIManager._captureUI.CaptureInfoOn(this);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // 만약 플레이어가 충돌 범위 밖으로 나간다면
        if (col.transform.tag.Equals("Player_W"))
        {
            // 외곽선 없는 메터리얼로 변경
            spriteRenderer.material = materials[0];

            // 플레이어 조사 기능 비활성화
            PlayerCtrl.instance.isCanCapture = false;

            // 조사 UI 이미지 끄기
            UIManager._captureUI.CaptureInfoOff();
        }
    }
}
