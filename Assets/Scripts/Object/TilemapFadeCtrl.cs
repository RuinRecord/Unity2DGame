using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapFadeCtrl : MonoBehaviour
{
    private const float FADE_SPEED = 7f;

    private Tilemap tilemap;
    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    /// <summary>
    /// (_alpha) 투명도로 서서히 Fade 시키는 함수
    /// </summary>
    /// <param name="_alpha">목표 페이드 투명도</param>
    /// <returns></returns>
    IEnumerator Fade(float _alpha)
    {
        Color current_color = tilemap.color;
        float current_alpha = current_color.a;

        while (Mathf.Abs(_alpha - current_alpha) > 0.01f)
        {
            current_color.a += (_alpha - current_alpha) * FADE_SPEED * Time.deltaTime;
            current_alpha = current_color.a;
            tilemap.color = current_color;

            yield return null;
        }

        current_color.a = _alpha;
        tilemap.color = current_color;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(Fade(0.75f));
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag.Equals("Player"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(Fade(1f));
        }
    }
}
