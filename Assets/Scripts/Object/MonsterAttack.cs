using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 몬스터 공격 판정 오브젝트 클래스이다.
/// </summary>
public class MonsterAttack : MonoBehaviour
{
    /// <summary> 공격 판정이 지속되는 시간 </summary>
    public float destroyTime = 0.05f;

    /// <summary> 공격 데미지 </summary>
    public float damage = 0f;

    private void OnEnable()
    {
        // 파괴 체크 코루틴 시작
        StartCoroutine("DestroyAttack");
    }

    /// <summary>
    /// 공격 판정 오브젝트를 삭제하는 코루틴 함수이다.
    /// </summary>
    IEnumerator DestroyAttack()
    {
        yield return new WaitForSeconds(destroyTime);
        // 파괴 시간(destroyTime)이 지나면 사라짐

        // 오브젝트 풀링 삭제 요청 수행
        ObjectPool.ReturnObject(ObjectType.MonsterAttack, this);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // 충돌된 오브젝트가 플레이어라면
        if (col.transform.tag.Equals("Player"))
        {
            // 피격 판정 => 플레이어 HP 감소
            PlayerCtrl.instance.cur_HP -= damage;

            // 실행되던 파괴 코루틴 함수 중지
            StopCoroutine("DestroyAttack");

            // 즉시 오브젝트 풀링 삭제 요청
            ObjectPool.ReturnObject(ObjectType.MonsterAttack, this);
        }
    }
}
