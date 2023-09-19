using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 맵 렌더링에 대한 우선 순위 처리를 위한 클래스이다.
/// Transform을 기반으로 렌더러를 저장한다. 이때, 아무 렌더러가 없다면 우선 순위 처리 대상에서 제외된다.
/// </summary>
public class Render
{
    public Transform transform;
    public SpriteRenderer spriteRenderer;
    public TilemapRenderer tilemapRenderer;

    public Render(Transform _transform)
    {
        transform = _transform;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        tilemapRenderer = transform.GetComponent<TilemapRenderer>();
    }
}


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


    /// <summary> 모든 타일맵 렌더러를 탐색하기 위한 부모 오브젝트 </summary>
    [SerializeField]
    private GameObject tileMapTr;


    /// <summary> 렌더러를 포함한 모든 오브젝트를 저장한 리스트 </summary>
    [SerializeField]
    private List<Render> spritesList;


    [SerializeField]
    private List<int> getCapturesList;

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        spritesList = new List<Render>();
        getCapturesList = new List<int>();

        var tilemaps = tileMapTr.GetComponentsInChildren<TilemapRenderer>();
        for (int i = 0; i < tilemaps.Length; i++)
            spritesList.Add(new Render(tilemaps[i].transform));

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
            spritesList.Add(new Render(sprites[i].transform));
    }


    // Update is called once per frame
    void Update()
    {
        // 만약 리스트에 렌더러가 있다면 렌더링 우선 순위 처리를 수행
        if (spritesList.Count > 0)
            SetDepthAllofMapObjects();
    }


    /// <summary>
    /// 맵에 존재하는 모든 오브젝트에 대해 Z Depth에 따른 우선 순위를 설정하는 함수
    /// </summary>
    private void SetDepthAllofMapObjects()
    {
        // Y축 정렬
        spritesList.Sort(delegate (Render a, Render b)
        {
            if (a.transform.position.y < b.transform.position.y)
                return 1;
            else
                return -1;
        });

        // 렌더러 우선순위 지정
        for (int i = 0; i < spritesList.Count; i++)
        {
            if (spritesList[i].spriteRenderer != null)
                spritesList[i].spriteRenderer.sortingOrder = i;
            else if (spritesList[i].tilemapRenderer != null)
                spritesList[i].tilemapRenderer.sortingOrder = i;
        }
    }


    /// <summary>
    /// 'transform'을 가진 오브젝트를 spritesList에 저장하는 함수이다.
    /// </summary>
    /// <param name="_transform">저장할 오브젝트 Transform</param>
    public void AddSprite(Transform _transform)
    {
        spritesList.Add(new Render(_transform));
    }


    /// <summary>
    /// 'render'을 가진 오브젝트를 spritesList에 삭제하는 함수이다.
    /// </summary>
    /// <param name="_transform">삭제할 render</param>
    public void RemoveSprite(Render _render)
    {
        if (!spritesList.Remove(_render))
            Debug.LogError("MapCtrl | RemoveSprite Error!");
    }

    /// <summary>
    /// 'capture_code'에 해당하는 조사 이벤트를 getCapturesList에 저장하는 함수이다.
    /// </summary>
    /// <param name="capture_code">조사 이벤트 식별 번호</param>
    public void AddCapture(int capture_code)
    {
        if (getCapturesList.IndexOf(capture_code) != -1)
        {
            Debug.Log("이미 확인된 조사 이벤트");
            return;
        }

        getCapturesList.Add(capture_code);
        EventCtrl.instance.FindNewEvent(capture_code);
    }
}
