using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeManager : MonoBehaviour
{
    private const string FADE_ANIM_IN_NAME = "BlindCtrl_FadeIn";
    private const string FADE_ANIM_OUT_NAME = "BlindCtrl_FadeOut";
    private const float FADE_INIT_TIME = 1.5f;
    private const float FADE_DEFAULT_TIME = 0.75f;

    public Animation Anim;
    public Image BlindImage;

    /// <summary> 현재 Switch가 일어나고 있는 중인지에 대한 여부 </summary>
    public bool IsChanging;
    

    // SceneIndex 표
    /*****************************************
    0. MainScene 
    1. GameScene
    *****************************************/


    public void Init()
    {
        BlindImage.color = new Color(0f, 0f, 0f, 0f);
        StartCoroutine(Fade_Init());
    }


    public void GoToScene(Scene scene, string BGM_name)
    {
        if (IsChanging)
            return;

        StartCoroutine(switchScene(scene, BGM_name, FADE_DEFAULT_TIME));
    }


    public IEnumerator switchScene(Scene scene, string BGM_name, float fadeTime)
    {
        if (IsChanging)
            yield break; // 현재 작업 중이면 취소

        GameManager.Sound.PlayBGM(BGM_name, fadeTime);
        IsChanging = true;
        FadeIn(fadeTime);

        yield return new WaitForSeconds(fadeTime);

        // 씬 전환
        SceneManager.LoadScene((int)scene);

        IsChanging = false;
        FadeOut(fadeTime);

        yield return new WaitForSeconds(fadeTime);
    }


    public IEnumerator switchPos(Vector3 destination, string BGM_name, float fadeTime)
    {
        if (IsChanging)
            yield break; // 현재 작업 중이면 취소

        GameManager.Sound.PlayBGM(BGM_name, fadeTime);
        IsChanging = true;
        FadeIn(fadeTime);

        yield return new WaitForSeconds(fadeTime);

        IsChanging = false;
        FadeOut(fadeTime);
        
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            UIManager.PlayerUI.SetKeyOnHUD(PlayerFunction.Interaction);
            PlayerCtrl playerCtrl = PlayerCtrl.Instance;
            playerCtrl.MovePosition(destination);
            playerCtrl.CurrentTeleport?.Close();
        }

        yield return new WaitForSeconds(fadeTime);

        // 후처리
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerTag.Instance.IsCanTag = true;
        }
    }


    public IEnumerator switchPos(Vector3 destination,  float fadeInTime, float fadeOutTime)
    {
        if (IsChanging)
            yield break; // 현재 작업 중이면 취소

        FadeIn(fadeInTime);
        IsChanging = true;

        yield return new WaitForSeconds(fadeInTime);

        // FadeIn -> Out 전환 시점
        IsChanging = false;
        FadeOut(fadeOutTime);

        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            UIManager.PlayerUI.SetKeyOnHUD(PlayerFunction.Interaction);
            PlayerCtrl playerCtrl = PlayerCtrl.Instance;
            playerCtrl.MovePosition(destination);
            playerCtrl.CurrentTeleport?.Close();
        }

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
        if (SceneManager.GetActiveScene().name.Equals("Sample_Jun") || SceneManager.GetActiveScene().name.Equals("SampleScene"))
        {
            PlayerTag.Instance.IsCanTag = true;
        }
    }


    public IEnumerator switchPos(Vector3 destination)
    {
        if (IsChanging)
            yield break; // 현재 작업 중이면 취소

        StartCoroutine(switchPos(destination, FADE_DEFAULT_TIME, FADE_DEFAULT_TIME));
    }


    private void FadeIn(float _fadeTime)
    {
        Anim.Play(FADE_ANIM_IN_NAME);
        Anim[FADE_ANIM_IN_NAME].speed = 1f / _fadeTime;
    }


    private void FadeOut(float _fadeTime)
    {
        Anim.Play(FADE_ANIM_OUT_NAME);
        Anim[FADE_ANIM_OUT_NAME].speed = 1f / _fadeTime;
    }


    IEnumerator Fade_Init()
    {
        FadeOut(FADE_INIT_TIME);

        yield return new WaitForSeconds(FADE_INIT_TIME);
    }
}