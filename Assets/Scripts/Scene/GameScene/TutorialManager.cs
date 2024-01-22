using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public static bool isTutorialOn = false;

    [SerializeField]
    private TutorialUI UI;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CloseTutorial();
    }

    public void ShowTooltip(string info)
    {
        isTutorialOn = true;
        UI.ShowTooltip(info);
    }

    public void CloseTutorial()
    {
        isTutorialOn = false;
        UI.CloseTooltip();
    }
}
