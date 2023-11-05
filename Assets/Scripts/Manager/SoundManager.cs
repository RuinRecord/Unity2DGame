using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private new AudioSource audio;

    public void Init()
    {
        audio.volume = 0f;
        BGMSetting(0, 1.5f);
    }


    /// <summary>
    /// BGM을 설정하는 함수이다.
    /// </summary>
    /// <param name="BGMindex">전환할 BGM 번호</param>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    public void BGMSetting(int BGMindex, float _fadeTime)
    {
        audio.Play();
        StartCoroutine(BGMFade(BGMindex, _fadeTime));
    }


    /// <summary>
    /// 오디오 볼륨을 서서히 전환시키는 코루틴 함수이다.
    /// </summary>
    /// <param name="_fadeTime">Fade 전환 시간</param>
    IEnumerator BGMFade(int BGMindex, float _fadeTime)
    {
        while (audio.volume > 0f)
        {
            audio.volume -= Time.deltaTime / _fadeTime;
            yield return null;
        }

        audio.volume = 0f;
        audio.clip = GameManager._data.GetBGM(BGMindex);

        while (audio.volume < 1f)
        {
            audio.volume += Time.deltaTime / _fadeTime;
            yield return null;
        }

        audio.volume = 1f;
    }
}
