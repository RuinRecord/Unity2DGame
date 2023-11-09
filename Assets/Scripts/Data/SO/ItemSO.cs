using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 아이템의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string item_name;
    public string item_info;
    public Sprite item_sprite;

    /// <summary> 아이템 식별 번호 </summary>
    public int itemCode;

    /// <summary> 획득 시 맵 오브젝트가 사라지는지에 대한 여부 </summary>
    public bool isDestroy;
}
