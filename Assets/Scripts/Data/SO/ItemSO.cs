using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 아이템의 정보를 담는 SO이다. </summary>
[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string itemInfo;
    public Sprite itemSprite;

    /// <summary> 아이템 식별 번호 </summary>
    public int itemCode;
}
