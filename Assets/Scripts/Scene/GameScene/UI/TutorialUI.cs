using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panelOb;

    // Start is called before the first frame update
    void Start()
    {
        ClosePanel();
    }

    public void ClosePanel()
        => panelOb.SetActive(false);
}
