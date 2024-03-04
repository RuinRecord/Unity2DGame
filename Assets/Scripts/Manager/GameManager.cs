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

    private static bool isGM = true;


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
        if (!isGM)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 10;
        }
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
