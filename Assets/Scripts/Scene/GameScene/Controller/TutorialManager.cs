using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    /// <summary> TutorialManager 싱글톤 </summary>
    private static TutorialManager instance;
    public static TutorialManager Instance
    {
        set
        {
            if (instance == null)
                instance = value;
        }
        get { return instance; }
    }

    public static bool IsTutorialOn = false;

    [SerializeField] private TutorialUI UI;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CloseTutorial();
    }

    public void SetActiveUI(bool isActive) => UI.gameObject.SetActive(isActive);

    public void ShowTutorial(string info)
    {
        IsTutorialOn = true;
        UI.ShowTooltip(info);
    }

    public void CloseTutorial()
    {
        IsTutorialOn = false;
        UI.CloseTooltip();
    }
}
