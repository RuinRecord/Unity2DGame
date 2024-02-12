using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CutSceneSO", menuName = "Scriptable Object/CutSceneSO")]
public class CutSceneSO : ScriptableObject
{
    /// <summary> 컷씬 식별 번호 </summary>
    public int cutSceneCode;

    /// <summary> 컷씬 연출 액션들 </summary>
    public List<CutSceneAction> actions;
}