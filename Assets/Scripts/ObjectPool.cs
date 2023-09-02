using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    TempMonster,
    MonsterAttack,
};

public class ObjectPool : MonoBehaviour
{
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

    [SerializeField]
    private Transform mapTr;
    
    public Transform objectTr;

    public GameObject tempMonster_prefab;
    public GameObject monsterAttack_prefab;

    Queue<TempMonster> tempMonster_queue = new Queue<TempMonster>();
    Queue<MonsterAttack> monsterAttack_queue = new Queue<MonsterAttack>();

    List<SpriteRenderer> sprite_list = new List<SpriteRenderer>();

    private void Awake()
    {
        instance = this;

        sprite_list.AddRange(mapTr.GetComponentsInChildren<SpriteRenderer>());
    }

    private void Update()
    {
        if (sprite_list.Count > 0)
        {
            // Y축 정렬
            sprite_list.Sort(delegate (SpriteRenderer a, SpriteRenderer b)
            {
                if (a.transform.position.y < b.transform.position.y)
                    return 1;
                else
                    return -1;
            });

            for (int i = 0; i < sprite_list.Count; i++)
                sprite_list[i].sortingOrder = i;
        }
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

            var spriteRender = obj.GetComponent<SpriteRenderer>();
            if (spriteRender != null)
                instance.sprite_list.Add(spriteRender);

            return obj;
        }
        else
        {
            var newObj = instance.CreateNewObject<T>(type, tr, pos);
            newObj.transform.SetParent(tr);
            newObj.transform.position = pos;
            newObj.gameObject.SetActive(true);

            var spriteRender = newObj.GetComponent<SpriteRenderer>();
            if (spriteRender != null)
                instance.sprite_list.Add(spriteRender);

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

        var spriteRender = obj.GetComponent<SpriteRenderer>();
        if (spriteRender != null)
            instance.sprite_list.Remove(spriteRender);

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
