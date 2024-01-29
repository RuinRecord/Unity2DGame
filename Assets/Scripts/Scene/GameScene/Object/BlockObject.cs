using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour
{
    [SerializeField] private BlockType blockType;
    public BlockType BlockType => blockType;

    private void Interaction(int speicalDialogIdx, Vector2 moveDir)
    {
        SetDialog(speicalDialogIdx);
        MovePlayerBack(moveDir);
    }

    private void SetDialog(int speicalDialogIdx) => UIManager.InteractUI.StartDialog(GameManager.Data.specialDialogDatas[speicalDialogIdx].dialogs.ToArray());

    private void MovePlayerBack(Vector2 moveDir) => PlayerCtrl.Instance.SetMove(moveDir, 1, PlayerCtrl.WALK_SPEED);

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 충돌 범위 안으로 들어온 오브젝트가 플레이어라면
        if (col.tag.Equals("Player_M") && PlayerTag.PlayerType.Equals(PlayerType.MEN) ||
            col.tag.Equals("Player_W") && PlayerTag.PlayerType.Equals(PlayerType.WOMEN))
        {
            switch (blockType)
            {
                case BlockType.PlayerMRoom:
                    if (EventCtrl.Instance.CurrentEvent <= 0)
                        Interaction(0, Vector2Int.right);
                    break;
                case BlockType.R1ToHoll:
                    if (EventCtrl.Instance.CurrentEvent <= 2)
                        Interaction(1, Vector2Int.down);
                    break;
                case BlockType.Hall:
                    if (EventCtrl.Instance.CurrentEvent <= 3)
                        Interaction(4, Vector2Int.left);
                    break;
            }
        }
    }
}
