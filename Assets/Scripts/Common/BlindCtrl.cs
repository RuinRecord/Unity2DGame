using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlindCtrl : MonoBehaviour
{
    private const string FADE_ANIM_IN_NAME = "BlindCtrl_FadeIn";
    private const string FADE_ANIM_OUT_NAME = "BlindCtrl_FadeOut";
    private const float FADE_INIT_TIME = 1.5f;
    private const float FADE_DEFAULT_TIME = 0.75f;

    static public BlindCtrl instance;

    public Animation anim;
    public new AudioSource audio;
    public Image blindImage;

    private Coroutine fadeCo;
    private int currentSceneIndex;
    private float fadeTime;


    /*****************************************
    0. MainScene 
    1. GameScene
    *****************************************/

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            currentSceneIndex = 0;

            blindImage.color = new Color(0f, 0f, 0f, 0f);
            audio.volume = 0f;
            audio.Play();
            // BGMSetting(0, false, 1f);
            fadeCo = StartCoroutine(Fade_Init());
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }

    public void GoToScene(int _destination)
    {
        StartCoroutine(switchScene(currentSceneIndex, _destination, FADE_DEFAULT_TIME, FADE_DEFAULT_TIME));
    }

    IEnumerator Fade_Init()
    {
        FadeOut(FADE_INIT_TIME);

        yield return new WaitForSeconds(1f);

        // 후처리
    }

    public void FadeIn(float _fadeTime)
    {
        anim.Play(FADE_ANIM_IN_NAME);
        anim[FADE_ANIM_IN_NAME].speed = 1f / _fadeTime;
    }

    public void FadeOut(float _fadeTime)
    {
        anim.Play(FADE_ANIM_OUT_NAME);
        anim[FADE_ANIM_OUT_NAME].speed = 1f / _fadeTime;
    }

    // BGM이 변하면서 씬을 변경
    public IEnumerator switchScene(int Sceneindex, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (fadeCo != null)
            yield break; // 이미 Fade 진행 중이면 코루틴 나가기

        FadeIn(fadeInTime);
        
        BGMSetting(-1, true, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        FadeOut(fadeOutTime);

        SceneManager.LoadScene(Sceneindex);
        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
    }

    // BGM이 변하면서 위치 이동
    public IEnumerator switchPos(Vector3 _pos, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (fadeCo != null)
            yield break; // 이미 Fade 진행 중이면 코루틴 나가기

        FadeIn(fadeInTime);
        BGMSetting(-1, true, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        FadeOut(fadeOutTime);
        Camera.main.transform.position = _pos + new Vector3Int(0, 0, -10);
        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
    }

    // BGM이 변하지 않으면서 위치 이동
    public IEnumerator switchPos(Vector3 _pos, float fadeInTime, float fadeOutTime)
    {
        if (fadeCo != null)
            yield break; // 이미 Fade 진행 중이면 코루틴 나가기

        FadeIn(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        FadeOut(fadeOutTime);
        Camera.main.transform.position = _pos + new Vector3Int(0, 0, -10);

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
    }

    public IEnumerator switchBGM(int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (fadeCo != null)
            yield break; // 이미 Fade 진행 중이면 코루틴 나가기

        BGMSetting(-1, true, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        BGMSetting(BGMindex, false, fadeOutTime);
        audio.Play();
    }

    private void BGMSetting(int bgmIndex, bool _isFade, float _fadeTime)
    {
        //if (bgmIndex != -1)
        //    audio.clip = SaveScript.BGMs[bgmIndex];
        fadeTime = _fadeTime;
        StartCoroutine(BGMFade());
    }

    IEnumerator BGMFade()
    {
        while (audio.volume > 0f)
        {
            audio.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        audio.volume = 0f;

        while (audio.volume < 1f)
        {
            audio.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audio.volume = 1f;
    }
}