using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// 0 = Sprite-Default, 1 = Sprite-Outline
    /// </summary>
    [SerializeField]
    private Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Default Setting
        spriteRenderer.material = materials[0];
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag.Equals("Player"))
        {
            spriteRenderer.material = materials[1];

            PlayerCtrl.instance.isCanCapture = true;
            PrintUICtrl.instance.CaptureInfoOn(this.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.transform.tag.Equals("Player"))
        {
            spriteRenderer.material = materials[0];

            PlayerCtrl.instance.isCanCapture = false;
            PrintUICtrl.instance.CaptureInfoOff();
        }
    }
}
