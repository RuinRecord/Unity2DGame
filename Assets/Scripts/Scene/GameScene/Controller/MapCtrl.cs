using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

/// <summary>
/// 맵 관련 처리를 담당하는 컨트롤러 클래스이다.
/// </summary>
public class MapCtrl : MonoBehaviour
{
    /// <summary> MapCtrl 싱글톤 </summary>
    private static MapCtrl instance;
    public static MapCtrl Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }

    private readonly int[] changedMonitorIndices = new int[] { 5, 6, 14, 22, 25 };

    /// <summary> 이동 불가능한 오브젝트 레이어 마스트 </summary>
    public LayerMask CanNotMove_layerMask;


    /// <summary> 모든 타일맵 렌더러를 탐색하기 위한 부모 오브젝트 </summary>
    [SerializeField] private GameObject tileMapTr;


    /// <summary> 렌더러를 포함한 모든 오브젝트를 저장한 리스트 </summary>
    [SerializeField] private List<SortRenderer> spritesList;


    /// <summary> 렌더러를 포함한 모든 오브젝트를 저장한 리스트 </summary>
    [SerializeField] private List<CanMoveObject> moveObjectsList;
    public List<CanMoveObject> MoveObjectsList => moveObjectsList;

    [SerializeField] private GameObject R4ToFindMonitor;

    private List<Monitor> monitorsList;

    public List<Monitor> MonitorsList => monitorsList;

    [SerializeField] private Light2D globalLight;

    [SerializeField] private List<EventArea> objetAreas;

    [SerializeField] private List<CanMoveObject> objets;


    private void Awake()
    {
        Instance = this;

        spritesList = new List<SortRenderer>();
        spritesList.AddRange(GetComponentsInChildren<SortRenderer>());

        moveObjectsList = new List<CanMoveObject>();
        moveObjectsList.AddRange(GetComponentsInChildren<CanMoveObject>());

        monitorsList = new List<Monitor>();
        monitorsList.AddRange(R4ToFindMonitor.GetComponentsInChildren<Monitor>());
    }

    private void Start()
    {
        SetGlobalLight(0.5f);
    }


    // Update is called once per frame
    void Update()
    {
        // 만약 리스트에 렌더러가 있다면 렌더링 우선 순위 처리를 수행
        if (spritesList.Count > 0)
            SetDepthAllofMapObjects();
    }


    public bool CheckValidArea(Vector2 _destination)  => !Physics2D.OverlapBox(_destination, Vector2.one * 0.75f, 0f, CanNotMove_layerMask);


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
        int _sortIndex = 0;
        float _currentY = spritesList[0].transform.position.y;

        foreach (var _render in spritesList)
        {
            if (!IsEqualFloat(_currentY, _render.transform.position.y))
                _sortIndex++;
            _render.SetSortingOrder(_sortIndex, out _sortIndex);
            _currentY = _render.transform.position.y;
        }
    }


    public SortRenderer FindRender(Transform _transform)
    {
        foreach (var render in spritesList)
        {
            if (render.transform == _transform)
                return render;
        }
        return null;
    }


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

    public bool IsEqualFloat(float a, float b) => Mathf.Abs(a - b) <= 0.01f;

    public void SetGlobalLight(float intensity)
    {
        globalLight.intensity = intensity;
        if (PlayerCtrl.Instance != null)
            PlayerCtrl.Instance.CurrentLightIntensity = intensity;
    }

    public void ChangeSomeMonitor(MonitorType _type)
    {
        for (int i = 0; i < changedMonitorIndices.Length; i++)
        {
            monitorsList[changedMonitorIndices[i]].ChangeType(_type);
        }
    }

    public void ChangeAllMonitor(MonitorType _type)
    {
        foreach (var monitor in monitorsList)
        {
            monitor.ChangeType(_type);
        }
    }

    public bool CheckObjetsComplete()
    {
        foreach (var area in objetAreas)
        {
            if (!area.isDone)
                return false;
        }
        return true;
    }

    public void SetObjetsComplete(bool isDone)
    {
        foreach (var ob in objets)
        {
            ob.isDone = isDone;
        }
    }
}
