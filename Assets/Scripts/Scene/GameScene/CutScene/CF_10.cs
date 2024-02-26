using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CF_10 : CutSceneFunction
{
    [SerializeField] private GameObject ladder;
    [SerializeField] private GameObject playerW;
    [SerializeField] private GameObject vent;
    [SerializeField] private Teleport teleport;
    [SerializeField] private PlayerCtrl playerWCtrl;

    private void Start()
    {
        ladder.SetActive(false);
    }

    public override void OnFuntionEnter()
    {
        base.OnFuntionEnter();

        GameManager.Data.player.RemoveItem(3);
    }

    public override void Play(int actionIdx)
    {
        switch (actionIdx)
        {
            case 0: StartFadeOut(); break;
            case 1: StartFadeIn(); break;
            case 6: OpenVent(); break;
            case 7: EndFadeOut(); break;
            case 8: EndFadeIn(); break;
        }
    }

    public override void OnFunctionExit()
    {
        base.OnFunctionExit();

        playerW.SetActive(false);
        EventCtrl.Instance.SetCurrentEvent(Event.OpenVent);
        TutorialManager.Instance.ShowTutorial("환풍구 안으로 들어가세요.");
    }

    private void StartFadeOut() => CutSceneCtrl.Instance.FadeOut(1f);

    private void StartFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1f);

        TutorialManager.Instance.CloseTutorial();
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.NONE);
        CameraCtrl.Instance.SetCameraMode(CameraMode.Free);
        CameraCtrl.Instance.SetCameraPos(new Vector3(20f, 7f, 0f));

        playerWCtrl.MovePosition(new Vector3(20f, 100f, 0f));
        ladder.SetActive(true);
    }

    private void OpenVent()
    {
        teleport.gameObject.SetActive(true);
        vent.GetComponent<Animator>().SetBool("isOpen", true);
        Destroy(vent.GetComponent<InteractionObject>());
    }

    private void EndFadeOut()
    {
        CutSceneCtrl.Instance.FadeOut(1.5f);
    }

    private void EndFadeIn()
    {
        CutSceneCtrl.Instance.FadeIn(1.5f);
        PlayerTag.Instance.SwitchTagImmedately(PlayerType.WOMEN);
        CameraCtrl.Instance.SetCameraMode(CameraMode.PlayerW);
        CameraCtrl.Instance.SetCameraSize(5f);

        playerW.SetActive(false);
        playerWCtrl.MovePosition(new Vector3(20f, 4.5f, 0f));
    }
}
