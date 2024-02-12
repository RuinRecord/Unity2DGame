using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여주인공이 조사 상호작용이 가능한 오브젝트 클래스이다.
/// </summary>
public class CaptureObject : MonoBehaviour
{
    private const float SCALE_ZOOM_POWER = 0.1f;

    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// 쉐이더가 포함된 메터리얼
    /// 0 = Sprite-Default (외곽선 없음)
    /// 1 = Sprite-Outline (외곽선 있음)
    /// </summary>
    [SerializeField]
    private Material[] materials;

    /// <summary> 조사 이벤트 식별 번호 </summary>
    public int Code;


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

            this.transform.localScale += Vector3.one * SCALE_ZOOM_POWER;

            // 조사 UI 이미지 켜기
            UIManager.CaptureUI.CaptureInfoOn(this);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // 만약 플레이어가 충돌 범위 밖으로 나간다면
        if (col.transform.tag.Equals("Player_W"))
        {
            // 외곽선 없는 메터리얼로 변경
            spriteRenderer.material = materials[0];

            this.transform.localScale -= Vector3.one * SCALE_ZOOM_POWER;

            // 조사 UI 이미지 끄기
            UIManager.CaptureUI.CaptureInfoOff();
        }
    }
}
