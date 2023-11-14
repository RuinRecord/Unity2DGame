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

    private new AudioSource audio;


    private void Awake()
    {
        instance = this;

        _playerUI.Init();
        _interactUI.Init();
        _captureUI.Init();
        _invenUI.Init();
    }

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio == null)
            audio = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 'SEIndex' 번호의 사운드 이펙트를 출력하는 함수이다.
    /// </summary>
    /// <param name="SEIndex">SE 효과음 식별 번호</param>
    public void PlayAudio(int SEIndex)
    {
        audio.clip = GameManager._data.GetSE(SEIndex);
        audio.Play();
    }

    /// <summary>
    /// 'clip' 사운드 이펙트를 출력하는 함수이다.
    /// </summary>
    /// <param name="clip">오디오 클립</param>
    public void PlayAudio(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }
}
