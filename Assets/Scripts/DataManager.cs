using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    /// <summary> 게임 BGM 리소스 </summary>
    private AudioClip[] BGMs;


    /// <summary> 게임 SE 리소스 </summary>
    private AudioClip[] SEs;


    public void Init()
    {
        BGMs = Resources.LoadAll<AudioClip>("Audios/BGM");
        SEs = Resources.LoadAll<AudioClip>("Audios/SE");
    }


    /// <summary>
    /// BGM 오디오 소스를 반환하는 함수
    /// </summary>
    /// <param name="_index">원하는 BGM</param>
    /// <returns>BGM 오디오 소스</returns>
    public AudioClip GetBGM(int _index)
    {
        return BGMs[_index];
    }


    /// <summary>
    /// SE 오디오 소스를 반환하는 함수
    /// </summary>
    /// <param name="_index">원하는 SE</param>
    /// <returns>SE 오디오 소스</returns>
    public AudioClip GetSE(int _index)
    {
        return SEs[_index];
    }
}
