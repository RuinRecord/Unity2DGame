using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /// <summary> 스폰 최소 시간 </summary>
    private const float CREATE_TIME_MIN = 5f;

    /// <summary> 스폰 최대 시간 </summary>
    private const float CREATE_TIME_MAX = 8f;

    /// <summary> 스폰 불가능 거리 (플레이어 - 스포너 거리) </summary>
    private const float CREATE_DISTANCE = 2f;


    /// <summary> 스포너가 생성하는 오브젝트 타입 </summary>
    [SerializeField]
    private ObjectType objectType;


    /// <summary> 이 스포너가 생성한 오브젝트 리스트 </summary>
    [SerializeField]
    private List<GameObject> objectList;


    /// <summary> 생성할 수 있는 몬스터의 최대 수 </summary>
    [SerializeField]
    private int maxCreateNum;


    /// <summary> 최근 생성된 몬스터 수 </summary>
    private int currentCreateNum;


    /// <summary> 현재 스포너가 작동 중인지에 대한 여부 </summary>
    public bool isOn;


    /// <summary> 최근에 생성한 적이 있는 지에 대한 여부 </summary>
    private bool isCreate;


    // Start is called before the first frame update
    void Start()
    {
        objectList = new List<GameObject>();
        currentCreateNum = 0;
        isOn = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isOn)
            return; // 활성화 상태가 아니라면 스폰 기능 미작동

        if (!isCreate)
        {
            isCreate = true;
            StartCoroutine("CreateMonster");
        }
    }


    /// <summary>
    /// 몬스터 생성 코루틴 함수이다.
    /// </summary>
    IEnumerator CreateMonster()
    {
        yield return new WaitForSeconds(Random.Range(CREATE_TIME_MIN, CREATE_TIME_MAX));
        // MIN ~ MAX 사이의 랜덤 시간 이후 몬스터 스폰 수행

        isCreate = false;
        if (maxCreateNum <= currentCreateNum)
        {
            Debug.Log("몬스터 생성 실패: 최대 생성 개수 초과");
            yield break; // 최대 생성 개수를 넘기면 생성 취소
        }
        if (Vector2.Distance(transform.position, PlayerCtrl.instance.transform.position) < CREATE_DISTANCE)
        {
            Debug.Log("몬스터 생성 실패: 플레이어 거리 가까움");
            yield break; // 플레이어 거리가 가까우면 생성 취소
        }

        Vector2 create_pos = (Vector2)transform.position + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        currentCreateNum++;
        switch (objectType)
        {
            // 몬스터 생성
            case ObjectType.TempMonster: objectList.Add(ObjectPool.GetObject<TempMonster>(ObjectType.TempMonster, ObjectPool.instance.objectTr, create_pos).gameObject); break;
        }

        Debug.Log("몬스터 생성 성공");
    }


    /// <summary>
    /// 파괴된 'gameObject'에 따라 스포너 데이터를 설정하는 함수이다.
    /// </summary>
    /// <param name="_gameObject">제거된 스포너 오브젝트</param>
    public void RemoveObject(GameObject _gameObject)
    {
        currentCreateNum--;
        objectList.Remove(_gameObject);
    }
}
