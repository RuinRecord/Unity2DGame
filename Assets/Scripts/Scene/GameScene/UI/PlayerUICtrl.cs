using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 상태 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class PlayerUICtrl : MonoBehaviour
{
    [Header("[ 프로필 관련 변수 ]")]
    [SerializeField]
    private GameObject profile_object;

    [SerializeField]
    private Image profile_image;

    [SerializeField]
    private Sprite profile_w_sprite, profile_m_sprite;


    [SerializeField]
    private Image HP_fillImage;


    [Header("[ 조작키 관련 변수 ]")]

    [SerializeField]
    private UIBox player_w_ctrlBox, player_m_ctrlBox;


    public void Init()
    {
        
    }

    public void SetActivePlayerUI(bool isActive)
    {
        profile_object.SetActive(isActive);
        player_w_ctrlBox.gameObject.SetActive(isActive);
        player_m_ctrlBox.gameObject.SetActive(isActive);
    }


    public void SetPlayerUIAll(PlayerType playerType)
    {
        SetPlayerProfile(playerType);
        SetPlayerCtrlUI(playerType);
    }


    private void SetPlayerProfile(PlayerType playerType)
    {
        if (playerType.Equals(PlayerType.WOMEN))
        {
            profile_image.sprite = profile_w_sprite;
        }
        else if (playerType.Equals(PlayerType.MEN))
        {
            profile_image.sprite = profile_m_sprite;
        }
        SetPlayerHP();
    }


    public void SetPlayerHP()
    {
        //HP_fillImage.fillAmount = PlayerCtrl.instance.cur_HP / PlayerCtrl.instance.max_HP;
    }


    private void SetPlayerCtrlUI(PlayerType playerType)
    {
        player_w_ctrlBox.gameObject.SetActive(playerType.Equals(PlayerType.WOMEN));
        player_m_ctrlBox.gameObject.SetActive(playerType.Equals(PlayerType.MEN));
    }
}
