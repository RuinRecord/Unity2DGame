using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;

    static public GameManager instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get 
        {
            if (Instance == null)
                CreateGameManager();
            return Instance; 
        }
    }

    [SerializeField] private ChangeManager changeManager;
    public static ChangeManager Change => instance.changeManager;

    [SerializeField] private DataManager dataManager;
    public static DataManager Data => instance.dataManager;

    [SerializeField] private SoundManager soundManager;
    public static SoundManager Sound => instance.soundManager;

    [SerializeField] private ObjectManager objectManager;
    public static ObjectManager Object => instance.objectManager;


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;

            Change.Init();
            Data.Init();
            Sound.Init();
            Object.Init();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Time.timeScale = 1f;
        else if (Input.GetKeyDown(KeyCode.F2))
            Time.timeScale = 5f;
    }

    static private void CreateGameManager()
    {
        GameObject go = GameObject.Find("@GameManager");

        if (go == null)
        {
            go = new GameObject("@GameManager");
            go.AddComponent<GameManager>();
        }

        DontDestroyOnLoad(go);
        instance = go.GetComponent<GameManager>();

        Sound.Init();
        Change.Init();
        Data.Init();
        Object.Init();
    }
}
