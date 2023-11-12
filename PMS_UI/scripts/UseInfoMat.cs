using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseInfoMat : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] imgs = new Sprite[4];
    Image target;
    void Start()
    {
        target = GetComponent<Image>();
    }

    // Update is called once per frame
    public void ChangeImage(int pageNum)
    {
        if(pageNum<4&& pageNum>=0)
        {
            target.sprite = imgs[pageNum];
        }
    }
}
