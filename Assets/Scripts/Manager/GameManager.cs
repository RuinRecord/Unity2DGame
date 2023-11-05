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
    public static ChangeManager _change => instance.changeManager;

    [SerializeField] private DataManager dataManager;
    public static DataManager _data => instance.dataManager;

    [SerializeField] private SoundManager soundManager;
    public static SoundManager _sound => instance.soundManager;

    [SerializeField] private ObjectManager objectManager;
    public static ObjectManager _object => instance.objectManager;


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;

            _change.Init();
            _data.Init();
            _sound.Init();
            _object.Init();
        }
        else
        {
            Destroy(this.gameObject);
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

        _sound.Init();
        _change.Init();
        _data.Init();
        _object.Init();
    }
}
