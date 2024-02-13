using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAlphaCaster : MonoBehaviour
{
    private const float DISABLE_THRESHOLD = 0f;
    private const float ENABLE_THRESHOLD = 0.1f;

    [SerializeField] private bool isEnable;

    private void OnEnable()
    {
        SetEnable(isEnable);
    }

    public void SetEnable(bool isEnable)
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = (isEnable) ? ENABLE_THRESHOLD : DISABLE_THRESHOLD;
        this.isEnable = isEnable;
    }
}
