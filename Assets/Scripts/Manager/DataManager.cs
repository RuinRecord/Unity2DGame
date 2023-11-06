using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    /// <summary> 게임에 존재하는 아이템 개수 </summary>
    public static readonly int itemNum = 10;

    /// <summary> 게임에 존재하는 사진 개수 </summary>
    public static readonly int captureNum = 10;

    /// <summary> 게임에 존재하는 조사일지 개수 </summary>
    public static readonly int recordNum = 10;

    /// <summary> 게임에 존재하는 스토리 이벤트 개수 </summary>
    public static readonly int storyEventNum = 10;


    /// <summary> 현재 플레이어의 데이터 </summary>
    public PlayerData player;


    /// <summary> 현재 맵의 데이터 </summary>
    public MapData map;


    /// <summary> 아이템 SO 데이터 </summary>
    public ItemData[] itemDatas;


    /// <summary> 사진 SO 데이터 </summary>
    public CaptureData[] captureDatas;


    /// <summary> 조사 일지 SO 데이터 </summary>
    public RecordData[] recordDatas;


    /// <summary> 상호 작용 대사 SO 데이터 </summary>
    public InteractionDialog[] interactionDialogDatas;


    /// <summary> 게임 BGM 리소스 </summary>
    private AudioClip[] BGMs;


    /// <summary> 게임 SE 리소스 </summary>
    private AudioClip[] SEs;


    public void Init()
    {
        itemDatas = LoadAll<ItemData>("ScriptObjects/Items");
        captureDatas = LoadAll<CaptureData>("ScriptObjects/Captures");
        recordDatas = LoadAll<RecordData>("ScriptObjects/Records");
        interactionDialogDatas = LoadAll<InteractionDialog>("ScriptObjects/Dialogs/InteractionObjects");
        BGMs = LoadAll<AudioClip>("Audios/BGM");
        SEs = LoadAll<AudioClip>("Audios/SE");

        Debug.Log(captureDatas.Length);
        Debug.Log(interactionDialogDatas.Length);
    }


    /// <summary>
    /// 리소스를 한번에 불러오는 함수이다.
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="path">리소스 폴더 경로</param>
    public T[] LoadAll<T>(string path) where T : Object
    {
        T[] resource = Resources.LoadAll<T>(path);

        if (resource == null)
        {
            Debug.Log($"Failed to load Resource : {path}");
            return null;
        }

        return resource;
    }


    /// <summary>
    /// 리소스를 하나만 불러오는 함수이다.
    /// </summary>
    /// <typeparam name="T">데이터 타입</typeparam>
    /// <param name="path">리소스 파일 경로</param>
    public T Load<T>(string path) where T : Object
    {
        T resource = Resources.Load<T>(path);

        if (resource == null)
        {
            Debug.Log($"Failed to load Resource : {path}");
            return null;
        }

        return resource;
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
