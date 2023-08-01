using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public float destroyTime = 0.025f;
    public float damage = 0f;

    private void OnEnable()
    {
        StartCoroutine("DestroyAttack");
    }

    IEnumerator DestroyAttack()
    {
        yield return new WaitForSeconds(destroyTime);

        ObjectPool.ReturnObject(ObjectType.MonsterAttack, this);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.transform.tag.Equals("Player"))
        {
            // �ǰ� ���� => �÷��̾� HP ���� �� UI ����
            PlayerCtrl.instance.cur_HP -= damage;
            PlayerStateUI.instance.SetPlayerState();

            StopCoroutine("DestroyAttack");
            ObjectPool.ReturnObject(ObjectType.MonsterAttack, this);
        }
    }
}
