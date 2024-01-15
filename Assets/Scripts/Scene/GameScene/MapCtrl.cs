using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 맵 관련 처리를 담당하는 컨트롤러 클래스이다.
/// </summary>
public class MapCtrl : MonoBehaviour
{
    /// <summary> MapCtrl 싱글톤 </summary>
    private static MapCtrl Instance;
    public static MapCtrl instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    /// <summary> 이동 불가능한 오브젝트 레이어 마스트 </summary>
    public LayerMask canNotMove_layerMask;


    /// <summary> 모든 타일맵 렌더러를 탐색하기 위한 부모 오브젝트 </summary>
    [SerializeField]
    private GameObject tileMapTr;


    /// <summary> 렌더러를 포함한 모든 오브젝트를 저장한 리스트 </summary>
    [SerializeField]
    private List<SortRenderer> spritesList;


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        spritesList = new List<SortRenderer>();
        spritesList.AddRange(GetComponentsInChildren<SortRenderer>());
    }


    // Update is called once per frame
    void Update()
    {
        // 만약 리스트에 렌더러가 있다면 렌더링 우선 순위 처리를 수행
        if (spritesList.Count > 0)
            SetDepthAllofMapObjects();

        if (Input.GetKeyDown(KeyCode.P))
            CutSceneCtrl.instance.SetCutScene(GameManager._data.cutSceneDatas[0]);
    }


    /// <summary>
    /// '_destination' 월드 위치가 막혀있는지 체크하고 반환합니다.
    /// </summary>
    /// <param name="_destination">도착 위치 벡터</param>
    public bool CheckValidArea(Vector2 _destination) => !Physics2D.OverlapBox(_destination, Vector2.one * 0.75f, 0f, canNotMove_layerMask);


    /// <summary>
    /// 맵에 존재하는 모든 오브젝트에 대해 Z Depth에 따른 우선 순위를 설정하는 함수
    /// </summary>
    private void SetDepthAllofMapObjects()
    {
        // Y축 정렬
        spritesList.Sort(delegate (SortRenderer a, SortRenderer b)
        {
            if (a.transform.position.y <= b.transform.position.y)
                return 1;
            else
                return -1;
        });

        // 렌더러 우선순위 지정
        int sortIndex = 0;
        float currentY = spritesList[0].transform.position.y;

        foreach (var render in spritesList)
        {
            if (!IsEqualFloat(currentY, render.transform.position.y))
                sortIndex++;
            render.SetSortingOrder(sortIndex);
            currentY = render.transform.position.y;
        }
    }

    /// <summary>
    /// 'transform'을 가진 Render를 spritesList에서 찾아 반환하는 함수이다.
    /// </summary>
    /// <param name="_transform"></param>
    /// <returns></returns>
    public SortRenderer FindRender(Transform _transform)
    {
        foreach (var render in spritesList)
        {
            if (render.transform == _transform)
                return render;
        }
        return null;
    }


    /// <summary>
    /// 'transform'을 가진 오브젝트를 spritesList에 저장하는 함수이다.
    /// </summary>
    /// <param name="_transform">저장할 오브젝트 Transform</param>
    public void AddSortRenderer(GameObject _ob)
    {
        SortRenderer render = _ob.GetComponent<SortRenderer>();
        if (render == null)
        {
            Debug.LogWarning("Hey! SortRenderer is null!");
            return;
        }

        spritesList.Add(render);
    }


    /// <summary>
    /// '_transform'을 가진 오브젝트를 spritesList에 삭제하는 함수이다.
    /// </summary>
    /// <param name="_transform">삭제할 오브젝트 Transform</param>
    public void RemoveSprite(Transform _transform)
    {
        if (!spritesList.Remove(FindRender(_transform)))
            Debug.LogError("MapCtrl :: RemoveSprite Error!");
    }

    public void DestroyObject(GameObject ob)
    {
        RemoveSprite(ob.transform);
        Destroy(ob);
    }

    private bool IsEqualFloat(float a, float b)
        => Mathf.Abs(a - b) <= 0.01f;
}
