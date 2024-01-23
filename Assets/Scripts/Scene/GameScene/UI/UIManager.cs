using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        set 
        {
            if (instance == null)
                instance = value; 
        }
        get { return instance; }
    }

    [SerializeField] private PlayerUICtrl playerUICtrl;
    public static PlayerUICtrl PlayerUI => instance.playerUICtrl;

    [SerializeField] private InteractUICtrl interactUICtrl;
    public static InteractUICtrl InteractUI => instance.interactUICtrl;

    [SerializeField] private CaptureUICtrl captureUICtrl;
    public static CaptureUICtrl CaptureUI => instance.captureUICtrl;

    [SerializeField] private InvenUICtrl invenUICtrl;
    public static InvenUICtrl InvenUI => instance.invenUICtrl;


    private void Awake()
    {
        Instance = this;

        PlayerUI.Init();
        InteractUI.Init();
        CaptureUI.Init();
        InvenUI.Init();
    }

    public void SetActiveUI(bool isActive)
    {
        PlayerUI.SetActivePlayerUI(isActive);
    }
}
