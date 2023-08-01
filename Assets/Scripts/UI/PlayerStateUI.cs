using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
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

    [SerializeField]
    private Image HP_fillImage, MP_fillImage;

    private void Awake()
    {
        instance = this;
    }

    public void SetPlayerState()
    {
        HP_fillImage.fillAmount = PlayerCtrl.instance.cur_HP / PlayerCtrl.instance.max_HP;
        MP_fillImage.fillAmount = PlayerCtrl.instance.cur_MP / PlayerCtrl.instance.max_MP;
    }
}
