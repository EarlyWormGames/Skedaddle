using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider
{
    public UnityAction<PointerEventData> onDrag;
    public UnityAction<PointerEventData> onPointerDown;
    public UnityAction<PointerEventData> onPointerUp;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (onDrag != null)
            onDrag.Invoke(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (onPointerDown != null)
            onPointerDown.Invoke(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (onPointerUp != null)
            onPointerUp.Invoke(eventData);
    }
}
