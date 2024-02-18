using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CutSceneSO", menuName = "Scriptable Object/CutSceneSO")]
public class CutSceneSO : ScriptableObject
{
    /// <summary> 컷씬 연출 액션들 </summary>
    public List<CutSceneAction> actions;
}