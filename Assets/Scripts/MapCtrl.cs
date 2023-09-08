using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

public class MapCtrl : MonoBehaviour
{
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

    [SerializeField]
    private GameObject tileMapTr;

    [SerializeField]
    private List<Render> sprites_List;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        sprites_List = new List<Render>();

        var tilemaps = tileMapTr.GetComponentsInChildren<TilemapRenderer>();
        for (int i = 0; i < tilemaps.Length; i++)
            sprites_List.Add(new Render(tilemaps[i].transform));

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
            sprites_List.Add(new Render(sprites[i].transform));
    }

    // Update is called once per frame
    void Update()
    {
        if (sprites_List.Count > 0)
            SetDepthAllofMapObjects();
    }

    /// <summary>
    /// �ʿ� �����ϴ� ��� ������Ʈ�� ���� Z Depth�� ���� �켱 ������ �����ϴ� �Լ�
    /// </summary>
    private void SetDepthAllofMapObjects()
    {
        // Y�� ����
        sprites_List.Sort(delegate (Render a, Render b)
        {
            if (a.transform.position.y < b.transform.position.y)
                return 1;
            else
                return -1;
        });

        for (int i = 0; i < sprites_List.Count; i++)
        {
            if (sprites_List[i].spriteRenderer != null)
                sprites_List[i].spriteRenderer.sortingOrder = i;
            else if (sprites_List[i].tilemapRenderer != null)
                sprites_List[i].tilemapRenderer.sortingOrder = i;
        }
    }

    public void AddSprite(Transform _transform)
    {
        sprites_List.Add(new Render(_transform));
    }

    public void RemoveSprite(Render _render)
    {
        if (!sprites_List.Remove(_render))
            Debug.LogError("MapCtrl | RemoveSprite Error!");
    }
}
