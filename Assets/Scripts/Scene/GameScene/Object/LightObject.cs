using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightObject : MonoBehaviour
{
    private readonly string[] animNames =
    {
        "None",
        "Light_Blink_Slow",
        "Light_Blink_Normal",
        "Light_Blink_Fast",
    };

    private readonly Color[] colors =
    {
        Color.white,                    // 흰색
        new Color(0.3f, 0.5f, 0.8f),    // 붉은색
        new Color(0.6f, 0.8f, 1f),      // 푸른색
        new Color(0.8f, 0.6f, 0.3f),      // 노란색
    };


    [SerializeField]
    private LightAnim lightAnim;

    [SerializeField]
    private LightColor lightColor;

    [SerializeField]
    private Light2D light2D;

    [SerializeField]
    private Animation anim;

    [SerializeField]
    private bool isOn;


    // Start is called before the first frame update
    void Start()
    {
        if (!isOn)
            light2D.intensity = 0f;
        light2D.color = colors[(int)lightColor];

        if (!lightAnim.Equals(LightAnim.None))
        {
            anim.Play(animNames[(int)lightAnim]);
        }
    }

    public void SetLight(float intensity)
    {
        light2D.intensity = intensity;
        if (intensity == 0f)
            isOn = false;
        else
            isOn = true;
    }

    public void DisableLight() => SetLight(0f);
}
