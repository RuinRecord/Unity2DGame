using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    public InteractionObject InteractionOb;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private bool isOpen;

    [SerializeField]
    private bool isLock;

    public bool IsOpen => isOpen;

    public int Type;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Cabinet does't contain Animation!");
            return;
        }

        anim.SetInteger("type", Type);
        if (isOpen)
        {
            anim.SetBool("isOpen", true);
        }    
    }

    public void Open()
    {
        if (isLock)
        {
            if (GameManager.Data.player.CheckHasItem(0))
            {
                UIManager.InteractUI.StartDialog(GameManager.Data.specialDialogDatas[3].dialogs.ToArray());
                GameManager.Sound.PlaySE("사물함풀림");
                isLock = false;
            }
            else
            {
                UIManager.InteractUI.StartDialog(GameManager.Data.specialDialogDatas[2].dialogs.ToArray());
                GameManager.Sound.PlaySE("사물함잠김");
            }

            return;
        }

        isOpen = !isOpen;
        anim.SetBool("isOpen", isOpen);
        GameManager.Sound.PlaySE("사물함열림");
    }

    public void SetAnimOfGetItem()
    {
        Type = 1;
        anim.SetInteger("type", Type);
    }

    public void StartDialog()
    {
        UIManager.InteractUI.StartDialog(this);
    }
}
