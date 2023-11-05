using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scene
{
    MainScene,
    GameScene
}

public class ChangeManager : MonoBehaviour
{
    private const string FADE_ANIM_IN_NAME = "BlindCtrl_FadeIn";
    private const string FADE_ANIM_OUT_NAME = "BlindCtrl_FadeOut";
    private const float FADE_INIT_TIME = 1.5f;
    private const float FADE_DEFAULT_TIME = 0.75f;

    public Animation anim;
    public Image blindImage;

    /// <summary> 현재 Switch가 일어나고 있는 중인지에 대한 여부 </summary>
    public bool isChanging;
    

    // SceneIndex 표
    /*****************************************
    0. MainScene 
    1. GameScene
    *****************************************/


    public void Init()
    {
        blindImage.color = new Color(0f, 0f, 0f, 0f);
        StartCoroutine(Fade_Init());
    }


    /// <summary>
    /// 씬 전환 함수이다.
    /// </summary>
    /// <param name="destination">전환될 씬 번호</param>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    public void GoToScene(Scene scene, int BGMindex)
    {
        if (isChanging)
            return;

        StartCoroutine(switchScene(scene, BGMindex, FADE_DEFAULT_TIME));
    }


    /// <summary>
    /// BGM이 변하면서 씬을 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="Sceneindex">전환할 씬 번호</param>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchScene(Scene scene, int BGMindex, float fadeTime)
    {
        if (isChanging)
            yield break; // 현재 작업 중이면 취소

        GameManager._sound.BGMSetting(BGMindex, fadeTime);
        isChanging = true;
        FadeIn(fadeTime);

        yield return new WaitForSeconds(fadeTime);

        // 씬 전환
        SceneManager.LoadScene((int)scene);

        isChanging = false;
        FadeOut(fadeTime);

        yield return new WaitForSeconds(fadeTime);
    }


    /// <summary>
    /// BGM이 변하면서 플레이어 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchPos(Vector3 _pos, int BGMindex, float fadeTime)
    {
        if (isChanging)
            yield break; // 현재 작업 중이면 취소

        GameManager._sound.BGMSetting(BGMindex, fadeTime);
        isChanging = true;
        FadeIn(fadeTime);

        yield return new WaitForSeconds(fadeTime);

        isChanging = false;
        FadeOut(fadeTime);
        
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerCtrl.instance.Teleport(_pos);
        }

        yield return new WaitForSeconds(fadeTime);

        // 후처리
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerTag.instance.isCanTag = true;
        }
    }


    /// <summary>
    /// BGM이 변하지 않으면서 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchPos(Vector3 _pos, float fadeInTime, float fadeOutTime)
    {
        if (isChanging)
            yield break; // 현재 작업 중이면 취소

        FadeIn(fadeInTime);
        isChanging = true;

        yield return new WaitForSeconds(fadeInTime);

        // FadeIn -> Out 전환 시점
        isChanging = false;
        FadeOut(fadeOutTime);

        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerCtrl.instance.Teleport(_pos);
        }

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerTag.instance.isCanTag = true;
        }
    }


    /// <summary>
    /// BGM이 변하지 않으면서 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    public IEnumerator switchPos(Vector3 _pos)
    {
        if (isChanging)
            yield break; // 현재 작업 중이면 취소

        StartCoroutine(switchPos(_pos, FADE_DEFAULT_TIME, FADE_DEFAULT_TIME));
    }


    /// <summary>
    /// 서서히 어두워지는 애니메이션 수행 함수이다.
    /// </summary>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    private void FadeIn(float _fadeTime)
    {
        anim.Play(FADE_ANIM_IN_NAME);
        anim[FADE_ANIM_IN_NAME].speed = 1f / _fadeTime;
    }


    /// <summary>
    /// 서서히 밝아지는 애니메이션 수행 함수이다.
    /// </summary>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    private void FadeOut(float _fadeTime)
    {
        anim.Play(FADE_ANIM_OUT_NAME);
        anim[FADE_ANIM_OUT_NAME].speed = 1f / _fadeTime;
    }


    /// <summary>
    /// 게임 시작할 때 발동하는 전환 코루틴 함수
    /// </summary>
    IEnumerator Fade_Init()
    {
        FadeOut(FADE_INIT_TIME);

        yield return new WaitForSeconds(FADE_INIT_TIME);
    }
}