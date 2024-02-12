using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀에 사용되는 오브젝트 종류
/// </summary>
public enum ObjectType
{
    TempMonster,
    MonsterAttack,
};


/// <summary>
/// 오브젝트 풀링 기법으로 사용되는 클래스이다.
/// 재활용을 통해 오브젝트 생성 및 삭제를 최소화 함으로써 최적화를 담당한다.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    private static ObjectPool instance;

    static public ObjectPool Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }


    private void Awake()
    {
        Instance = this;
    }


    /// <summary> 오브젝트가 생성될 위치 </summary>
    public Transform ObjectTr;


    /// <summary> 생성될 오브젝트 Prefab </summary>
    public GameObject TempMonster_prefab;
    public GameObject MonsterAttack_prefab;


    /// <summary> 오브젝트 관리 Queue </summary>
    Queue<TempMonster> tempMonster_queue = new Queue<TempMonster>();
    Queue<MonsterAttack> monsterAttack_queue = new Queue<MonsterAttack>();


    private T CreateNewObject<T>(ObjectType type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        T _newObj = null;

        switch (type)
        {
            case ObjectType.TempMonster: _newObj = Instantiate(TempMonster_prefab, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case ObjectType.MonsterAttack: _newObj = Instantiate(MonsterAttack_prefab, pos, Quaternion.identity, tr).GetComponent<T>(); break;
        }

        _newObj.gameObject.SetActive(false);
        return _newObj;
    }


    public T CreateObject<T>(ObjectType type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        int _count = GetCount(type);
        if (_count > 0)
        {
            T _obj = null;

            switch (type)
            {
                case ObjectType.TempMonster: _obj = tempMonster_queue.Dequeue().GetComponent<T>(); break;
                case ObjectType.MonsterAttack: _obj = monsterAttack_queue.Dequeue().GetComponent<T>(); break;
            }

            _obj.transform.SetParent(tr);
            _obj.transform.position = pos;
            _obj.gameObject.SetActive(true);

            MapCtrl.Instance.AddSortRenderer(_obj.gameObject);

            return _obj;
        }
        else
        {
            var _newObj = CreateNewObject<T>(type, tr, pos);
            _newObj.transform.SetParent(tr);
            _newObj.transform.position = pos;
            _newObj.gameObject.SetActive(true);

            MapCtrl.Instance.AddSortRenderer(_newObj.gameObject);

            return _newObj;
        }
    }


    public void ReturnObject<T>(ObjectType type, T obj) where T : MonoBehaviour
    {
        if (obj == null)
        {
            Debug.LogError("Return Object is Failed.");
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);

        switch (type)
        {
            case ObjectType.TempMonster: tempMonster_queue.Enqueue(obj.GetComponent<TempMonster>()); break;
            case ObjectType.MonsterAttack: monsterAttack_queue.Enqueue(obj.GetComponent<MonsterAttack>()); break;
        }
    }


    private int GetCount(ObjectType type)
    {
        int _count = 0;

        switch (type)
        {
            case ObjectType.TempMonster: _count = tempMonster_queue.Count; break;
            case ObjectType.MonsterAttack: _count = monsterAttack_queue.Count; break;
        }

        return _count;
    }
}
