using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 출력 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary> UIManager 싱글톤 </summary>
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


    private void Awake()
    {
        instance = this;

        _playerUI.Init();
        _interactUI.Init();
        _captureUI.Init();
    }
}
