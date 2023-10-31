using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 상태 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class PlayerStateUI : MonoBehaviour
{
    /// <summary> PlayerStateUI 싱글톤 </summary>
    private static PlayerStateUI Instance;
    public static PlayerStateUI instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }


    /// <summary> 플레이어 HP 및 MP 출력용 UI 이미지 </summary>
    [SerializeField]
    private Image HP_fillImage;


    private void Awake()
    {
        instance = this;
    }


    /// <summary>
    /// 플레이어의 MP 및 HP UI를 설정하는 함수이다.
    /// </summary>
    public void SetPlayerState()
    {
        SetPlayerHP();
    }


    /// <summary>
    /// 플레이어의 HP UI를 설정하는 함수이다.
    /// </summary>
    public void SetPlayerHP()
    {
        HP_fillImage.fillAmount = PlayerCtrl.instance.cur_HP / PlayerCtrl.instance.max_HP;
    }
}
