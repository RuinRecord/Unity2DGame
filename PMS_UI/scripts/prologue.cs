using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class prologue : MonoBehaviour
{
    
    private int pageNum = 0;
    private ChangeManager Scenectrl;

    public GameObject target =null;
    Image targetimg;
    public Sprite[] imgs = new Sprite[4];

    public GameObject txtarget= null;
    TMP_Text txt;

    public string[] txts;



    void Start()
    {
        targetimg = target.GetComponent<Image>();
        txt = txtarget.GetComponent<TMP_Text>();

            Scenectrl = GameManager._change;

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(pageNum);
            NextScnen();
        }
    }
            // Update is called once per frame
    public void NextScnen()
    {
        
        if (pageNum < 4 && pageNum >= 0)
        {
            targetimg.sprite = imgs[pageNum];
            txt.text = txts[pageNum];
            pageNum++;
        }
        else if(pageNum>=4)
        {
            LoadMainScene();
        }
    }

    public void LoadMainScene()
    { 
            Scenectrl.GoToScene(Scene.GameScene,0);
    }

}
