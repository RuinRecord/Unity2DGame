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
    public SpriteRenderer SpriteRenderer;

    [HideInInspector]
    public TilemapRenderer TilemapRenderer;

    private List<SortRenderer> childSortRenders;

    public bool isChild;

    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        TilemapRenderer = GetComponent<TilemapRenderer>();

        childSortRenders = new List<SortRenderer>();
        for (int i = 0; i < transform.childCount; i++)
        {
            SortRenderer _render = transform.GetChild(i).GetComponent<SortRenderer>();
            if (_render != null)
            {
                _render.isChild = true;
                childSortRenders.Add(_render);
            }
        }
    }

    public void SetSortingOrder(int startOrder, out int outOrder)
    {
        int _order = startOrder;

        if (!isChild)
        {
            if (SpriteRenderer != null)
                SpriteRenderer.sortingOrder = _order;
            else if (TilemapRenderer != null)
                TilemapRenderer.sortingOrder = _order;
            SetChildOrder(_order, out _order);
        }
        
        outOrder = _order;
    }

    private void SetChildOrder(int startOrder, out int outOrder)
    {
        int _order = startOrder;
        float _currentY = this.transform.position.y;

        for (int i = 0; i < childSortRenders.Count; i++)
        {
            SortRenderer _childRender = childSortRenders[i];
            if (!MapCtrl.Instance.IsEqualFloat(_currentY, _childRender.transform.position.y))
                _order++;
            _currentY = _childRender.transform.position.y;

            if (_childRender.SpriteRenderer != null)
                _childRender.SpriteRenderer.sortingOrder = _order;
            else if (_childRender.TilemapRenderer != null)
                _childRender.TilemapRenderer.sortingOrder = _order;
        }

        outOrder = _order;
    }
}