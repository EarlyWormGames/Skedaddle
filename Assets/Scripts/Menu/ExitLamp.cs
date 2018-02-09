using UnityEngine;
using System.Collections;

public class ExitLamp : Area
{
    public float m_fLerpSpeed;


    internal Animator m_aAnimator;

    internal float MaxSwing = 0.2f;

    internal Vector2 TargetPos;
    internal Vector2 ActualPos;
    internal Vector2 PreviousPos;

    internal bool SwingLeft = false;


    internal float counter;

    // Use this for initialization
    void Start()
    {
        m_aAnimator = GetComponent<Animator>();

        TargetPos = new Vector2(Random.Range(-MaxSwing, MaxSwing), Random.Range(-MaxSwing, MaxSwing));
        ActualPos = Vector2.zero;
        PreviousPos = ActualPos;
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        counter += Time.deltaTime;


        m_aAnimator.SetFloat("LeftRight Swing", Mathf.Sin(counter * m_fLerpSpeed) * MaxSwing);
        m_aAnimator.SetFloat("ForwardBack Swing", 0);
    }

    public override void Click()
    {
        EWApplication.Quit();
        m_IsActive = false;
    }

    public override void Select()
    {
        m_IsActive = true;
        Selected = this;
    }

    public override void Deselect()
    {
        m_IsActive = false;
    }
}

