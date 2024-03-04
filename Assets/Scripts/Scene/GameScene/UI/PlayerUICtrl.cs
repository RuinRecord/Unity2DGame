using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 상태 관련 UI 컨트롤러 클래스이다.
/// </summary>
public class PlayerUICtrl : MonoBehaviour
{
    [Header("[ 프로필 관련 변수 ]")]
    [SerializeField]
    private GameObject profileObject;

    [SerializeField] private Image profileImage;
    [SerializeField] private Sprite playerWProfileSprite, playerMProfileSprite;

    [Header("[ 조작키 관련 변수 ]")]

    [SerializeField] private GameObject playerWCtrlBox;
    [SerializeField] private GameObject playerMCtrlBox;
    [SerializeField] private GameObject[] playerTagKeyGO;

    private Dictionary<PlayerFunction, Image> playerWkeyImageDic = new Dictionary<PlayerFunction, Image>();
    private Dictionary<PlayerFunction, Image> playerMkeyImageDic = new Dictionary<PlayerFunction, Image>();
    private Dictionary<PlayerFunction, Sprite> onKeySpriteDic = new Dictionary<PlayerFunction, Sprite>();
    private Dictionary<PlayerFunction, Sprite> offKeySpriteDic = new Dictionary<PlayerFunction, Sprite>();

    public void Init()
    {
        InitKeyImages();
        InitKeySprites();
        SetPlayerTageKey(false);
    }

    public void SetActivePlayerUI(bool isActive)
    {
        profileObject.SetActive(isActive);
        playerWCtrlBox.gameObject.SetActive(isActive);
        playerMCtrlBox.gameObject.SetActive(isActive);

        if (isActive)
        {
            SetPlayerProfile(PlayerTag.PlayerType);
            SetPlayerCtrlUI(PlayerTag.PlayerType);
        }
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
            profileImage.sprite = playerWProfileSprite;
        }
        else if (playerType.Equals(PlayerType.MEN))
        {
            profileImage.sprite = playerMProfileSprite;
        }
    }


    private void SetPlayerCtrlUI(PlayerType playerType)
    {
        playerWCtrlBox.gameObject.SetActive(playerType.Equals(PlayerType.WOMEN));
        playerMCtrlBox.gameObject.SetActive(playerType.Equals(PlayerType.MEN));
    }

    public void SetKeyOnHUD(PlayerFunction function)
    {
        PlayerType playerType = PlayerTag.PlayerType;

        switch (playerType)
        {
            case PlayerType.WOMEN: playerWkeyImageDic[function].sprite = onKeySpriteDic[function]; break;
            case PlayerType.MEN: playerMkeyImageDic[function].sprite = onKeySpriteDic[function]; break;
        }
    }

    public void SetKeyOffHUD(PlayerFunction function)
    {
        PlayerType playerType = PlayerTag.PlayerType;

        switch (playerType)
        {
            case PlayerType.WOMEN: playerWkeyImageDic[function].sprite = offKeySpriteDic[function]; break;
            case PlayerType.MEN: playerMkeyImageDic[function].sprite = offKeySpriteDic[function]; break;
        }
    }

    public void SetPlayerTageKey(bool isActice)
    {
        foreach (var item in playerTagKeyGO)
        {
            item.SetActive(isActice);
        }
    }

    private void InitKeySprites()
    {
        var keySprites = Resources.LoadAll<Sprite>("Images/UI/Key");

        onKeySpriteDic.Add(PlayerFunction.Capture, keySprites[0]);
        offKeySpriteDic.Add(PlayerFunction.Capture, keySprites[1]);
        onKeySpriteDic.Add(PlayerFunction.Inventory, keySprites[2]);
        offKeySpriteDic.Add(PlayerFunction.Inventory, keySprites[3]);
        onKeySpriteDic.Add(PlayerFunction.Tag, keySprites[4]);
        offKeySpriteDic.Add(PlayerFunction.Tag, keySprites[5]);
        onKeySpriteDic.Add(PlayerFunction.Interaction, keySprites[6]);
        offKeySpriteDic.Add(PlayerFunction.Interaction, keySprites[7]);
        onKeySpriteDic.Add(PlayerFunction.Retry, keySprites[8]);
        offKeySpriteDic.Add(PlayerFunction.Retry, keySprites[9]);
    }

    private void InitKeyImages()
    {
        var keyImages = playerWCtrlBox.GetComponentsInChildren<Image>();

        playerWkeyImageDic.Add(PlayerFunction.Capture, keyImages[0]);
        playerWkeyImageDic.Add(PlayerFunction.Inventory, keyImages[1]);
        playerWkeyImageDic.Add(PlayerFunction.Tag, keyImages[2]);
        playerWkeyImageDic.Add(PlayerFunction.Interaction, keyImages[3]);

        keyImages = playerMCtrlBox.GetComponentsInChildren<Image>();

        playerMkeyImageDic.Add(PlayerFunction.Retry, keyImages[0]);
        playerMkeyImageDic.Add(PlayerFunction.Tag, keyImages[1]);
        playerMkeyImageDic.Add(PlayerFunction.Interaction, keyImages[2]);
    }
}
