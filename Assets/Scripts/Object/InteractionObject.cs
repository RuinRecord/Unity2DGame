using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Dialog
{
    public string dialog;
    public float print_time;
}

public class InteractionObject : MonoBehaviour
{
    /// <summary> 상호작용 대사  </summary>
    public Dialog[] dialogs;
}
