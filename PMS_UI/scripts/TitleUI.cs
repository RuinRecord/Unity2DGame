using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    public string sceneName = "Sample_PMS";
    private GameObject loadError;
    private GameObject setup;
    private ChangeManager Scenectrl = null;

    private void Start()
    {
        loadError = GameObject.FindWithTag("LoadErrorMessage");
        loadError.gameObject.SetActive(false);
        setup = GameObject.FindWithTag("SetupPanel");
        setup.gameObject.SetActive(false);

        Scenectrl = GameManager._change;


    }

    public void ClickStart()
    {
        Debug.Log("새로운 시작");
        //SceneManager.LoadScene(sceneName);
        Scenectrl.GoToScene(Scene.MainScene,0);
    }

    public void ClickLoad()
    {
        // if(loadData !=null){ 
        // sceneName = loadData.lastScene;
        // SceneManager.LoadScene(sceneName); }
        Debug.Log("로드");
        loadError.gameObject.SetActive(true);
    }
    public void ClickSetUp()
    {
        // if(loadData !=null
        Debug.Log("환경설정");
        setup.gameObject.SetActive(true);
    }

    public void ClickExit()
    {
        Debug.Log("게임 종료");
        UnityEditor.EditorApplication.isPlaying = false;
        // Application.Quit();
    }

    public void BackloadToOri()
    {
        if (loadError.active)
        {
            loadError.gameObject.SetActive(false);
        }
        else if(setup.active)
        {
            setup.gameObject.SetActive(false);
        }
    }

    public void setScreenOption(string Mode)
    {
        int setWidth;
        int setHeight;
        if(Mode =="1")
        {
            setWidth = 1920;
            setHeight= 1200;
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

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            Debug.Log("screen Size = " + setWidth + " X " + setHeight);
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            Debug.Log("screen Size = " + setWidth + " X " + setHeight);
        }
    }
}
