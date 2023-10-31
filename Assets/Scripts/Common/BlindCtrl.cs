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

    static private BlindCtrl Instance;

    static public BlindCtrl instance
    {
        set
        {
            if (Instance == null)
                Instance = value;
        }
        get { return Instance; }
    }

    public Animation anim;
    public new AudioSource audio;
    public Image blindImage;

    public bool isBlind;

    /// <summary> 현재 위치한 씬의 번호 </summary>
    /*****************************************
    0. MainScene 
    1. GameScene
    *****************************************/
    private int currentSceneIndex;


    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentSceneIndex = 0;

        blindImage.color = new Color(0f, 0f, 0f, 0f);
        audio.volume = 0f;
        audio.Play();
        BGMSetting(0, FADE_INIT_TIME);
        StartCoroutine(Fade_Init());
    }


    /// <summary>
    /// 씬 전환 함수이다.
    /// </summary>
    /// <param name="_destination">전환될 씬 번호</param>
    public void GoToScene(int _destination)
    {
        if (isBlind)
            return; // 현재 작업 중이면 취소

        StartCoroutine(switchScene(currentSceneIndex, _destination, FADE_DEFAULT_TIME, FADE_DEFAULT_TIME));
    }


    /// <summary>
    /// BGM이 변하면서 씬을 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="Sceneindex">전환할 씬 번호</param>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchScene(int Sceneindex, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (isBlind)
            yield break; // 현재 작업 중이면 취소

        isBlind = true;
        FadeIn(fadeInTime);
        BGMSetting(-1, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        isBlind = false;
        audio.Play();
        FadeOut(fadeOutTime);
        SceneManager.LoadScene(Sceneindex);
        BGMSetting(BGMindex, fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }


    /// <summary>
    /// BGM이 변하면서 플레이어 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchPos(Vector3 _pos, int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (isBlind)
            yield break; // 현재 작업 중이면 취소

        // 전처리
        PlayerTag.instance.isCanTag = false;

        isBlind = true;
        FadeIn(fadeInTime);
        BGMSetting(-1, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        isBlind = false;
        audio.Play();
        FadeOut(fadeOutTime);
        PlayerCtrl.instance.Teleport(_pos);
        BGMSetting(BGMindex, fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
        PlayerTag.instance.isCanTag = true;
    }


    /// <summary>
    /// BGM이 변하지 않으면서 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    public IEnumerator switchPos(Vector3 _pos, float fadeInTime, float fadeOutTime)
    {
        if (isBlind)
            yield break; // 현재 작업 중이면 취소

        // 전처리
        PlayerTag.instance.isCanTag = false;

        FadeIn(fadeInTime);
        isBlind = true;

        yield return new WaitForSeconds(fadeInTime);

        // FadeIn -> Out 전환 시점
        isBlind = false;
        FadeOut(fadeOutTime);
        PlayerCtrl.instance.Teleport(_pos);
        PlayerCtrl.instance.SetCurrentPos(_pos);

        yield return new WaitForSeconds(fadeOutTime);

        // 후처리
        PlayerTag.instance.isCanTag = true;
    }


    /// <summary>
    /// BGM이 변하지 않으면서 위치를 전환하는 코루틴 함수이다.
    /// </summary>
    /// <param name="_pos">목적지 위치</param>
    public IEnumerator switchPos(Vector3 _pos)
    {
        if (isBlind)
            yield break; // 현재 작업 중이면 취소

        StartCoroutine(switchPos(_pos, FADE_DEFAULT_TIME, FADE_DEFAULT_TIME));
    }


    /// <summary>
    /// BGM을 변경하는 코루틴 함수이다.
    /// </summary>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="fadeInTime">FadeIn 전환 시간</param>
    /// <param name="fadeOutTime">FadeOut 전환 시간</param>
    /// <returns></returns>
    public IEnumerator switchBGM(int BGMindex, float fadeInTime, float fadeOutTime)
    {
        if (isBlind)
            yield break; // 현재 작업 중이면 취소

        BGMSetting(-1, fadeInTime);

        yield return new WaitForSeconds(fadeInTime);

        BGMSetting(BGMindex, fadeOutTime);
        audio.Play();
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
    /// BGM을 설정하는 함수이다.
    /// </summary>
    /// <param name="bgmIndex">전환할 BGM 번호</param>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    public void BGMSetting(int bgmIndex, float _fadeTime)
    {
        audio.clip = Datapool.instance.GetBGM(bgmIndex);
        StartCoroutine(BGMFade(_fadeTime));
    }


    /// <summary>
    /// BGM을 서서히 전환시키는 코루틴 함수이다.
    /// </summary>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    IEnumerator BGMFade(float _fadeTime)
    {
        while (audio.volume > 0f)
        {
            audio.volume -= Time.deltaTime / _fadeTime;
            yield return null;
        }

        audio.volume = 0f;

        while (audio.volume < 1f)
        {
            audio.volume += Time.deltaTime / _fadeTime;
            yield return null;
        }

        audio.volume = 1f;
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