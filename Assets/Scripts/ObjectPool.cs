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
    /// <summary> ObjectPool 싱글톤 </summary>
    private static ObjectPool Instance;
    public static ObjectPool instance
    {
        set 
        {
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }

    /// <summary> 오브젝트가 생성될 위치 </summary>
    public Transform objectTr;


    /// <summary> 생성될 오브젝트 Prefab </summary>
    public GameObject tempMonster_prefab;
    public GameObject monsterAttack_prefab;


    /// <summary> 오브젝트 관리 Queue </summary>
    Queue<TempMonster> tempMonster_queue = new Queue<TempMonster>();
    Queue<MonsterAttack> monsterAttack_queue = new Queue<MonsterAttack>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 현재 오브젝트 풀에서 꺼내 쓸 자원이 없을 경우 새로운 오브젝트를 풀에 추가하는 함수
    /// </summary>
    /// <param name="type">생성할 오브젝트 종류</param>
    /// <param name="tr">생성 위치를 위한 부모 Transform</param>
    /// <param name="pos">생성 위치</param>
    private T CreateNewObject<T>(ObjectType type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        T newObj = null;

        switch (type)
        {
            case ObjectType.TempMonster: newObj = Instantiate(tempMonster_prefab, pos, Quaternion.identity, tr).GetComponent<T>(); break;
            case ObjectType.MonsterAttack: newObj = Instantiate(monsterAttack_prefab, pos, Quaternion.identity, tr).GetComponent<T>(); break;
        }

        newObj.gameObject.SetActive(false);
        return newObj;
    }

    /// <summary>
    /// (tr)을 부모 오브젝트로 삼고 (pos) 월드 포지션으로 (type) 종류에 해당하는 오브젝트를 풀에서 꺼내는 함수
    /// </summary>
    /// <param name="type">생성할 오브젝트 종류</param>
    /// <param name="tr">생성 위치를 위한 부모 Transform</param>
    /// <param name="pos">생성 위치</param>
    public static T GetObject<T>(ObjectType type, Transform tr, Vector3 pos) where T : MonoBehaviour
    {
        int count = GetCount(type);
        if (count > 0)
        {
            T obj = null;

            switch (type)
            {
                case ObjectType.TempMonster: obj = instance.tempMonster_queue.Dequeue().GetComponent<T>(); break;
                case ObjectType.MonsterAttack: obj = instance.monsterAttack_queue.Dequeue().GetComponent<T>(); break;
            }

            obj.transform.SetParent(tr);
            obj.transform.position = pos;
            obj.gameObject.SetActive(true);

            MapCtrl.instance.AddSprite(obj.transform);

            return obj;
        }
        else
        {
            var newObj = instance.CreateNewObject<T>(type, tr, pos);
            newObj.transform.SetParent(tr);
            newObj.transform.position = pos;
            newObj.gameObject.SetActive(true);

            MapCtrl.instance.AddSprite(newObj.transform);

            return newObj;
        }
    }

    /// <summary>
    /// (type) 종류에 해당하는 (obj) 오브젝트를 풀로 돌려보내는 함수
    /// </summary>
    /// <param name="type">돌려보낼 오브젝트 종류</param>
    /// <param name="obj">오브젝트 풀로 돌려보낼 Instance Object</param>
    public static void ReturnObject<T>(ObjectType type, T obj) where T : MonoBehaviour
    {
        if (obj == null)
        {
            Debug.LogError("Return Object is Failed.");
            return;
        }

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(instance.transform);

        switch (type)
        {
            case ObjectType.TempMonster: instance.tempMonster_queue.Enqueue(obj.GetComponent<TempMonster>()); break;
            case ObjectType.MonsterAttack: instance.monsterAttack_queue.Enqueue(obj.GetComponent<MonsterAttack>()); break;
        }
    }

    /// <summary>
    /// 현재까지 풀에 생성된 (type)에 해당하는 오브젝트 개수를 반환하는 함수
    /// </summary>
    /// <param name="type">수를 세릴 오브젝트 종류</param>
    private static int GetCount(ObjectType type)
    {
        int count = 0;

        switch (type)
        {
            case ObjectType.TempMonster: count = instance.tempMonster_queue.Count; break;
            case ObjectType.MonsterAttack: count = instance.monsterAttack_queue.Count; break;
        }

        return count;
    }
}
