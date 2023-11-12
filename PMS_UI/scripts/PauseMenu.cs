using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool GameIsPaused = false;
    public GameObject pauseMenuCanvas =null;
    private GameObject setup;
    private GameObject CtrlInto;
    public UseInfoMat infoPannel = null;
    private int infonum = 0;

    private void Start()
    {
        CtrlInto = GameObject.FindWithTag("CtrlInto");
        
        setup = GameObject.FindWithTag("SetupPanel");

        pauseMenuCanvas.SetActive(false);
        
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Debug.Log("is Resume");
        Time.timeScale = 1f;
        GameIsPaused = false;
        pauseMenuCanvas.SetActive(false);
        if (setup.active)
        {
            setup.gameObject.SetActive(false);
        }
        if (CtrlInto.active)
        {
            CtrlInto.gameObject.SetActive(false);
        }

    }

    public void Pause()
    {
        Debug.Log("is Pause");
        pauseMenuCanvas.SetActive(true);
        if(setup.active)
        {
            setup.gameObject.SetActive(false);
        }
        if (CtrlInto.active)
        {
            CtrlInto.gameObject.SetActive(false);
        }
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void SaveDataMenu()
    {
        
    }

    public void ToMain()
    {
       
    }
    public void ClickSetUp()
    {
        Debug.Log("환경설정");
        
        setup.gameObject.SetActive(true);
    }
    public void BackloadToOri()
    {
        if(setup.active)
        {
            setup.gameObject.SetActive(false);
        }
        if (CtrlInto.active)
        {
            CtrlInto.gameObject.SetActive(false);
        }
    }
    public void setScreenOption(string Mode)
    {
        int setWidth;
        int setHeight;
        if (Mode == "1")
        {
            setWidth = 1920;
            setHeight = 1200;
        }
        else if (Mode == "2")
        {
            setWidth = 960;
            setHeight = 600;
        }
        else if (Mode == "3")
        {
            setWidth = 480;
            setHeight = 300;
        }
        else
        {
            setWidth = 1920;
            setHeight = 1200;
        }

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); 

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) 
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            Debug.Log("screen Size = " + setWidth + " X " + setHeight);
        }
        else 
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            Debug.Log("screen Size = " + setWidth + " X " + setHeight);
        }
    }
    public void ClickUse()
    {
       CtrlInto.gameObject.SetActive(true);
    }
    public void ClickUseNextPage()
    {
        if (infonum < 3)
        {
            infonum++;
            infoPannel.ChangeImage(infonum);
        }
        
    }
    public void ClickUseBeforePage()
    {
        if (infonum > 0)
        {
            infonum--;
            infoPannel.ChangeImage(infonum);
        }
    }

    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }


}
