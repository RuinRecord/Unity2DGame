using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 플레이어와 겹치는 벽 같은 타일맵을 Fade In & Out을 통해 투명화 처리를 담당하는 클래스이다.
/// 해당 클래스는 투명화 시킬 타일맵 오브젝트 위에 배치된다.
/// </summary>
public class TilemapFadeCtrl : MonoBehaviour
{
    /// <summary> 타일맵 투명화 속도 </summary>
    private const float FADE_SPEED = 7f;


    /// <summary> 투명화가 적용될 타일맵 </summary>
    private Tilemap tilemap;


    /// <summary> Fade 효과를 수행하는 코루틴 정보 </summary>
    private Coroutine coroutine;


    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        tilemap.color = Color.white;
    }

    IEnumerator Fade(float _alpha)
    {
        Color _currentColor = tilemap.color;
        float _currentAlpha = _currentColor.a;

        // 타일맵의 투명도가 목표치와 1% 이상 차이날 경우
        while (Mathf.Abs(_alpha - _currentAlpha) > 0.01f)
        {
            // 매 프레임마다 투명도를 설정
            _currentColor.a += (_alpha - _currentAlpha) * FADE_SPEED * Time.deltaTime;
            _currentAlpha = _currentColor.a;
            tilemap.color = _currentColor;

            yield return null;
        }

        // 목표 투명도로 설정
        _currentColor.a = _alpha;
        tilemap.color = _currentColor;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 충돌 범위 안으로 들어온 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") || col.tag.Equals("Player_W"))
        {
            // 이미 Fade 중일 경우 해당 Fade 코루틴을 중단
            if (coroutine != null)
                StopCoroutine(coroutine);

            // 투명도 75% 까지 Fade 효과 적용
            coroutine = StartCoroutine(Fade(0.75f));
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // 충돌 범위 밖으로 나간 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") || col.tag.Equals("Player_W"))
        {
            // 이미 Fade 중일 경우 해당 Fade 코루틴을 중단
            if (coroutine != null)
                StopCoroutine(coroutine);

            // 투명도 100% 까지 Fade 효과 적용
            coroutine = StartCoroutine(Fade(1f));
        }
    }
}
