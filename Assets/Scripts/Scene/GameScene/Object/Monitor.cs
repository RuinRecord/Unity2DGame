using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Monitor : InteractionObject
{
    private static Sprite onSprite;
    private static Sprite offSprite; 
    private static Sprite errorSprite;

    private SpriteRenderer spriteRenderer;

    private new Light2D light;

    [SerializeField] private MonitorType type;

    protected override void Start()
    {
        base.Start();

        if (onSprite == null || offSprite == null || errorSprite == null)
        {
            onSprite = Resources.Load<Sprite>("Images/Maps/Monitor/On");
            offSprite = Resources.Load<Sprite>("Images/Maps/Monitor/Off");
            errorSprite = Resources.Load<Sprite>("Images/Maps/Monitor/Error");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        light = GetComponent<Light2D>();
        if (light == null)
            light = gameObject.AddComponent<Light2D>();

        ChangeType(type);
    }

    private Sprite GetSprite()
    {
        switch (type)
        {
            case MonitorType.On: return onSprite;
            case MonitorType.Off: return offSprite;
            case MonitorType.Error: return errorSprite;
            default:
                return null;
        }
    }

    private void SetLight()
    {
        switch (type)
        {
            case MonitorType.On:
                light.intensity = 0.5f;
                light.color = Color.blue;
                break;
            case MonitorType.Off:
                light.intensity = 0f;
                light.color = Color.blue;
                break;
            case MonitorType.Error:
                light.intensity = 0.5f;
                light.color = Color.red;
                break;
        }
    }

    public void ChangeType(MonitorType type)
    {
        this.type = type;
        spriteRenderer.sprite = GetSprite();
        SetLight();

        switch (type)
        {
            case MonitorType.Off:
                Code = 36;
                break;
            case MonitorType.On:
                Code = 37;
                break;
            case MonitorType.Error:
                Code = 38;
                break;
        }
    }
}
