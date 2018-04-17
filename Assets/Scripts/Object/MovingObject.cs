using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("")]
public class MovingObject : MonoBehaviour
{
    [Tooltip("Time (in s) to transition from point A to B")]
    public float Speed = 1;
    public AnimationCurve MovingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public UnityEvent ForwardEnd;
    public UnityEvent BackwardEnd;

    protected bool isLerping;
    protected float lerpTimer = 0;
    protected bool movingForward = true;

    // Use this for initialization
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping)
            Slide();

        OnUpdate();
    }

    void Slide()
    {
        float timer = Mathf.Clamp(lerpTimer + Time.deltaTime, 0, Speed);
        if (DoSlide(timer))
        {
            lerpTimer = timer;         
        }

        if (lerpTimer >= Speed)
        {
            Stop();

            if (movingForward)
                ForwardEnd.Invoke();
            else
                BackwardEnd.Invoke();
        }
    }

    /// <summary>
    /// Allows for child/parent classes to still call Start properly 
    /// </summary>
    protected virtual void OnStart() { }

    /// <summary>
    /// For generic update usage (animations etc)
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// Called for every frame the object is moving
    /// </summary>
    protected virtual bool DoSlide(float time)
    {
        return true;
    }

    /// <summary>
    /// Begin movement, starting at 0 (or inverse percentage of current time)
    /// </summary>
    /// <param name="forward">The direction to move in</param>
    public virtual void Move(bool forward)
    {
        if (forward != movingForward)
        {
            lerpTimer = Speed - lerpTimer;
        }

        isLerping = true;
        movingForward = forward;
    }

    /// <summary>
    /// Begin movement, starting at 0 (or inverse percentage of current time), and switch direction
    /// </summary>
    public virtual void Move()
    {
        movingForward = !movingForward;
        isLerping = true;
        lerpTimer = Speed - lerpTimer;
    }

    /// <summary>
    /// Stop movement
    /// </summary>
    public virtual void Stop()
    {
        isLerping = false;
    }

    /// <summary>
    /// Begin/continue to move in the current direction at the current time
    /// </summary>
    public virtual void Play()
    {
        isLerping = true;
    }
}