using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private Transform SE_tr;

    [SerializeField]
    private List<AudioSource> SE_audioList;

    [SerializeField]
    private AudioSource BGM_audio;


    public void Init()
    {
        SE_audioList = new List<AudioSource>();

        BGM_audio.volume = 0f;
        PlayBGM("Temp_BGM", 1.5f);
    }


    /// <summary>
    /// BGM 오디오를 출력하는 함수이다.
    /// </summary>
    /// <param name="BGM_name">출력할 BGM 이름 </param>
    /// <param name="fadeTime">Fade 전환 시간</param>
    public void PlayBGM(string BGM_name, float fadeTime)
    {
        AudioClip BGM_clip = GameManager._data.Load<AudioClip>($"Audios/BGM/{BGM_name}");
        PlayBGM(BGM_clip, fadeTime);
    }


    /// <summary>
    /// BGM 오디오를 출력하는 함수이다.
    /// </summary>
    /// <param name="BGM_clip">출력할 BGM 오디오 클립 </param>
    /// <param name="fadeTime">Fade 전환 시간</param>
    public void PlayBGM(AudioClip BGM_clip, float fadeTime)
    {
        if (BGM_clip == null)
        {
            Debug.LogError("BGM 오디오 클립이 NULL입니다!");
            return;
        }

        BGM_audio.Play();
        StartCoroutine(BGMFade(BGM_clip, fadeTime));
    }


    /// <summary>
    /// SE 오디오를 출력하는 함수이다.
    /// </summary>
    /// <param name="SE_name">출력할 SE 이름</param>
    public void PlaySE(string SE_name)
    {
        AudioClip SE_clip = GameManager._data.Load<AudioClip>($"Audios/SE/{SE_name}");
        PlaySE(SE_clip);
    }


    /// <summary>
    /// SE 오디오를 출력하는 함수이다.
    /// </summary>
    /// <param name="SE_clip">출력할 SE 오디오 클립</param>
    public void PlaySE(AudioClip SE_clip)
    {
        if (SE_clip == null)
        {
            Debug.LogError("SE 오디오 클립이 NULL입니다!");
            return;
        }

        AudioSource audio = FindIdleAudio();
        if (audio == null)
        {
            Debug.LogError("오디오 소스가 NULL입니다!");
            return;
        }

        audio.clip = SE_clip;
        audio.Play();
    }


    /// <summary>
    /// 현재 사용 가능한 오디오 소스를 탐색하여 반환하는 함수이다.
    /// </summary>
    private AudioSource FindIdleAudio()
    {
        foreach (var audio in SE_audioList)
            if (!audio.isPlaying)
                return audio;
        return CreateAudio();
    }


    /// <summary>
    /// 새로운 오디오 소스를 생성하고 반환하는 함수이다.
    /// </summary>
    private AudioSource CreateAudio()
    {
        AudioSource source = Instantiate(new GameObject(), SE_tr).AddComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
        SE_audioList.Add(source);

        return source;
    }


    /// <summary>
    /// 오디오 볼륨을 서서히 전환시키는 코루틴 함수이다.
    /// </summary>
    /// <param name="BGM_clip">출력할 BGM 오디오 클립 </param>
    /// <param name="fadeTime">Fade 전환 시간</param>
    IEnumerator BGMFade(AudioClip BGM_clip, float fadeTime)
    {
        while (BGM_audio.volume > 0f)
        {
            BGM_audio.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }

        BGM_audio.volume = 0f;
        BGM_audio.clip = BGM_clip;

        while (BGM_audio.volume < 1f)
        {
            BGM_audio.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        BGM_audio.volume = 1f;
    }
}
