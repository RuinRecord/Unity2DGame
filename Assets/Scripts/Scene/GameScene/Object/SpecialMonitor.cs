using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpecialMonitor : InteractionObject
{
    [SerializeField] private Animator animator;

    private new Light2D light;

    [SerializeField] private MonitorType type;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        light = GetComponent<Light2D>();

        ChangeType(type);
    }

    private void SetAnimator()
    {
        animator.SetBool("isOn", false);
        animator.SetBool("isError", false);

        switch (type)
        {
            case MonitorType.On: animator.SetBool("isOn", true); break;
            case MonitorType.Error: animator.SetBool("isError", true); break;
        }
    }

    private void SetLight()
    {
        switch (type)
        {
            case MonitorType.On:
                light.intensity = 2f;
                light.color = new Color(0.3f, 0.3f, 1f);
                break;
            case MonitorType.Off:
                light.intensity = 0f;
                light.color = new Color(0.3f, 0.3f, 1f);
                break;
            case MonitorType.Error:
                light.intensity = 3f;
                light.color = Color.red;
                break;
        }
    }

    public void ChangeType(MonitorType type)
    {
        this.type = type;
        SetAnimator();
        SetLight();

        switch (type)
        {
            case MonitorType.Off: Code = 41; break;
            case MonitorType.On: Code = 42; break;
            case MonitorType.Error: Code = 43; break;
        }
    }
}
