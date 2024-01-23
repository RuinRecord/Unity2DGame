using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
    Ivory_window,
    Navy_no_window,
    Ivory_no_window,
}

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private DoorType doorType;


    /// <summary> 포탈과 연결된 목적지 </summary>
    [SerializeField]
    private Vector3 destination;


    /// <summary> 포탈 사용 시 출력되는 오디오 </summary>
    [SerializeField]
    private AudioClip audioClip;


    [SerializeField]
    private Animator animator;


    /// <summary> 현재 사용 가능한 포탈인지에 대한 여부 </summary>
    public bool IsOn;


    private void Start()
    {
        if (animator != null)
            animator.SetInteger("DoorType", (int)doorType);
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
    }

    /// <summary>
    /// 포탈 목적지로 이동하는 함수이다.
    /// </summary>
    public void GoToDestination()
    {
        if (!IsOn)
            return; // 만약 닫힌 상태라면 취소

        Open();

        // Fade 애니메이션과 함께 목적지로 이동
        PlayerTag.Instance.IsCanTag = false;
        GameManager.Sound.PlaySE(audioClip);

        // 막힌 공간이라면 플레이어가 바라보는 방향으로 1칸 더 던진
        while (!MapCtrl.Instance.CheckValidArea(destination))
            destination += (Vector3Int)PlayerCtrl.Instance.GetDirection();

        StartCoroutine(GameManager.Change.switchPos(destination));
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        // 충돌 범위 안으로 들어온 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") && PlayerTag.PlayerType.Equals(PlayerType.MEN) ||
            col.tag.Equals("Player_W") && PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
        {
            switch (this.gameObject.name)
            {
                case "R2-1_To_R2 (No_Door)":
                    if (EventCtrl.Instance.CurrentEvent <= 0)
                    {
                        UIManager.InteractUI.StartDialog(GameManager.Data.specialDialogDatas[0].dialogs.ToArray());
                        PlayerCtrl.Instance.SetMove(Vector2Int.right, 1, PlayerCtrl.WALK_SPEED);
                    }
                    break;
                case "H_To_R2 (Door)":
                    if (EventCtrl.Instance.CurrentEvent <= 2)
                    {
                        UIManager.InteractUI.StartDialog(GameManager.Data.specialDialogDatas[1].dialogs.ToArray());
                        PlayerCtrl.Instance.SetMove(Vector2Int.down, 1, PlayerCtrl.WALK_SPEED);
                    }
                    break;
            }
        }
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
