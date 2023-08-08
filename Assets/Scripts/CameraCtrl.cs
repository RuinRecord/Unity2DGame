using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    static public CameraCtrl instance;

    public float cameraWidth, cameraHeight; // 카메라의 넓이의 반, 높이의 반
    private float moveSpeed; // 카메라가 플레이어를 쫒아가는 속도

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        cameraWidth = Camera.main.orthographicSize * Camera.main.pixelWidth / Camera.main.pixelHeight;
        cameraHeight = Camera.main.orthographicSize;
        moveSpeed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        GoToPlayer();
    }

    private void GoToPlayer()
    {
        Vector2 gap = PlayerCtrl.instance.transform.position - transform.position;
        transform.position += (Vector3)gap * moveSpeed * Time.deltaTime;
    }
}
