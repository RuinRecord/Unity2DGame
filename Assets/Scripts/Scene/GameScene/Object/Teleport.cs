using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private const float LIGHT_DEFAULT_INTENSITY = 0.5f;

    [SerializeField] private DoorType doorType;

    /// <summary> 포탈과 연결된 목적지 </summary>
    [SerializeField] private Vector3 destination;

    [SerializeField] private Vector2 direction;
    public Vector2 Direction => direction;

    /// <summary> 포탈 사용 시 출력되는 오디오 </summary>
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Animator animator;

    public InteractionDialogSO blockDialog;

    /// <summary> 현재 사용 가능한 포탈인지에 대한 여부 </summary>
    public bool IsOn;

    [SerializeField] private bool IsGoVent;

    public float lightIntensity;

    private void Start()
    {
        if (animator != null)
            animator.SetInteger("DoorType", (int)doorType);

        if (lightIntensity == 0f)
            lightIntensity = LIGHT_DEFAULT_INTENSITY;
    }

    public void Open()
    {
        if (animator != null)
            animator.SetBool("isOpen", true);
    }

    public void Close()
    {
        if (animator != null)
            animator.SetBool("isOpen", false);

        PlayerCtrl playerCtrl = PlayerCtrl.Instance;

        if (Direction != Vector2.zero)
            playerCtrl.SetAnimationDir(Direction);
        
        MapCtrl.Instance.SetGlobalLight(lightIntensity);
        playerCtrl.SetLight(lightIntensity < LIGHT_DEFAULT_INTENSITY);

        if (IsGoVent)
        {
            playerCtrl.SetShadow(false);
            playerCtrl.StartCrawl();
            playerCtrl.MoveSpeed = PlayerCtrl.WALK_SPEED * 0.5f;

            if (!GameManager.Data.player.CheckHasItem(4))
            {
                // 손전등이 없을 시
                CutSceneCtrl.Instance.StartCutScene(11);
            }
        }
        else
        {
            playerCtrl.SetShadow(true);
            playerCtrl.EndCrawl();
            playerCtrl.MoveSpeed = PlayerCtrl.WALK_SPEED;
        }

        if (gameObject.name.Equals("VToR4"))
        {
            if (EventCtrl.Instance.CurrentEvent < Event.GetToR4)
            {
                EventCtrl.Instance.SetCurrentEvent(Event.GetToR4);
                CutSceneCtrl.Instance.StartCutScene(12);
            }
        }
        else if (gameObject.name.Equals("VToH"))
        {
            if (EventCtrl.Instance.CurrentEvent == Event.EscapeR4)
            {
                CutSceneCtrl.Instance.StartCutScene(18);
            }
        }
    }

    /// <summary>
    /// 포탈 목적지로 이동하는 함수이다.
    /// </summary>
    public void GoToDestination()
    {
        if (IsGoVent && PlayerTag.PlayerType.Equals(PlayerType.MEN))
            return; // 벤트 전용은 남주 동작 불가

        if (!IsOn)
        {
            if (blockDialog != null)
                UIManager.InteractUI.StartDialog(blockDialog.dialogs.ToArray());
            return; // 만약 닫힌 상태라면 취소
        }

        Open();

        // Fade 애니메이션과 함께 목적지로 이동
        PlayerTag.Instance.IsCanTag = false;
        GameManager.Sound.PlaySE(audioClip);

        // 막힌 공간이라면 플레이어가 바라보는 방향으로 1칸 더 던진
        while (!MapCtrl.Instance.CheckValidArea(destination))
            destination += (Vector3Int)PlayerCtrl.Instance.GetDirection();

        StartCoroutine(GameManager.Change.switchPos(destination));
    }


    private void OnTriggerStay2D(Collider2D col)
    {
        // 충돌 범위 안으로 들어온 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") && PlayerTag.PlayerType.Equals(PlayerType.MEN) || 
            col.tag.Equals("Player_W") && PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
        {
            PlayerCtrl.Instance.CurrentTeleport = this;
        }
    }


    private void OnTriggerExit2D(Collider2D col)
    {
        // 충돌 범위 밖으로 나간 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") && PlayerTag.PlayerType.Equals(PlayerType.MEN) ||
            col.tag.Equals("Player_W") && PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
        {
            PlayerCtrl.Instance.CurrentTeleport = null;
        }
    }
}
