using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Cabinet does't contain Animation!");
            return;
        }

        if (isOpen)
        {
            anim.SetBool("isOpen", true);
        }    
    }

    public void Open()
    {
        isOpen = !isOpen;
        anim.SetBool("isOpen", isOpen);
        GameManager._sound.PlaySE("남주공격");
    }
}
