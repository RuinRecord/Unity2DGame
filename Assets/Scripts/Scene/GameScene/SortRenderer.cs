using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 맵 렌더링에 대한 우선 순위 처리를 위한 클래스이다.
/// Transform을 기반으로 렌더러를 저장한다. 이때, 아무 렌더러가 없다면 우선 순위 처리 대상에서 제외된다.
/// </summary>
public class SortRenderer : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public TilemapRenderer tilemapRenderer;

    private List<SortRenderer> childSortRenders;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemapRenderer = GetComponent<TilemapRenderer>();

        childSortRenders = new List<SortRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            SortRenderer _render = transform.GetChild(i).GetComponent<SortRenderer>();
            if (_render != null)
                childSortRenders.Add(_render);
        }
    }

    public void SetSortingOrder(int _order)
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = _order;
        else if (tilemapRenderer != null)
            tilemapRenderer.sortingOrder = _order;

        SetChildOrder();
    }

    private void SetChildOrder()
    {
        int order = -1;

        if (spriteRenderer != null)
            order = spriteRenderer.sortingOrder;
        else if (tilemapRenderer != null)
            order = tilemapRenderer.sortingOrder;

        for (int i = 0; i < childSortRenders.Count; i++)
        {
            SortRenderer _childRender = childSortRenders[i];

            if (_childRender.spriteRenderer != null)
                _childRender.spriteRenderer.sortingOrder = ++order;
            if (_childRender.tilemapRenderer != null)
                _childRender.tilemapRenderer.sortingOrder = ++order;
        }
    }
}