using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager Instance;
    public static UIManager instance
    {
        set 
        {
            if (Instance == null)
                Instance = value; 
        }
        get { return Instance; }
    }

    [SerializeField] private PlayerUICtrl playerUICtrl;
    public static PlayerUICtrl _playerUI => instance.playerUICtrl;

    [SerializeField] private InteractUICtrl interactUICtrl;
    public static InteractUICtrl _interactUI => instance.interactUICtrl;

    [SerializeField] private CaptureUICtrl captureUICtrl;
    public static CaptureUICtrl _captureUI => instance.captureUICtrl;

    [SerializeField] private InvenUICtrl invenUICtrl;
    public static InvenUICtrl _invenUI => instance.invenUICtrl;


    private void Awake()
    {
        instance = this;

        _playerUI.Init();
        _interactUI.Init();
        _captureUI.Init();
        _invenUI.Init();
    }
}
