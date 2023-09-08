using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private const float CREATE_TIME_MIN = 5f;
    private const float CREATE_TIME_MAX = 8f;
    private const float CREATE_DISTANCE = 2f;

    /// <summary> �����ʰ� �����ϴ� ������Ʈ Ÿ�� </summary>
    [SerializeField]
    private ObjectType objectType;

    /// <summary> �� �����ʰ� ������ ������Ʈ ����Ʈ </summary>
    [SerializeField]
    private List<GameObject> objectList;

    /// <summary> ������ �� �ִ� ������ �ִ� �� </summary>
    [SerializeField]
    private int maxCreateNum;

    private int currentCreateNum;

    /// <summary> ���� �����ʰ� �۵� �������� ���� ���� </summary>
    public bool isOn;

    /// <summary> �ֱٿ� ������ ���� �ִ� ���� ���� ���� </summary>
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
            return;

        if (!isCreate)
        {
            isCreate = true;
            StartCoroutine("CreateMonster");
        }
    }


    IEnumerator CreateMonster()
    {
        yield return new WaitForSeconds(Random.Range(CREATE_TIME_MIN, CREATE_TIME_MAX));

        isCreate = false;
        if (maxCreateNum <= currentCreateNum)
        {
            Debug.Log("���� ���� ����: �ִ� ���� ���� �ʰ�");
            yield break; // �ִ� ���� ������ �ѱ�� ���� ���
        }
        if (Vector2.Distance(transform.position, PlayerCtrl.instance.transform.position) < CREATE_DISTANCE)
        {
            Debug.Log("���� ���� ����: �÷��̾� �Ÿ� �����");
            yield break; // �÷��̾� �Ÿ��� ������ ���� ���
        }

        Vector2 create_pos = (Vector2)transform.position + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        currentCreateNum++;
        switch (objectType)
        {
            // ���� ����
            case ObjectType.TempMonster: objectList.Add(ObjectPool.GetObject<TempMonster>(ObjectType.TempMonster, ObjectPool.instance.objectTr, create_pos).gameObject); break;
        }

        Debug.Log("���� ���� ����");
    }


    public void RemoveObject(GameObject _gameObject)
    {
        currentCreateNum--;
        objectList.Remove(_gameObject);
    }
}
