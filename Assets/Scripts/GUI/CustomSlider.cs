using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// extra functionality for sliders
/// </summary>
public class CustomSlider : Slider
{
    public UnityAction<PointerEventData> onDrag;
    public UnityAction<PointerEventData> onPointerDown;
    public UnityAction<PointerEventData> onPointerUp;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Do Something on a drag event
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (onDrag != null)
            onDrag.Invoke(eventData);
    }

    /// <summary>
    /// Do Something on a mouse down event
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (onPointerDown != null)
            onPointerDown.Invoke(eventData);
    }

    /// <summary>
    /// Do Something on a mouse up event
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (onPointerUp != null)
            onPointerUp.Invoke(eventData);
    }
}
