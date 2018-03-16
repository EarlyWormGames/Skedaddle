using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Fading : MonoBehaviour {

    public Texture2D FadeOutTexture;    // the texture that will overlay the screen. This can be a block image or loading graphic
    public float FadeSpeed = 0.8f;      // fading speed
    public UnityEvent EventToCall;

    private int drawDepth = -1000;      // the texture's order in the draw hierarchy: Low numbers means its renders on top
    private float alpha = 1.0f;         // the texture's alpha value between 0 and 1 
    private int FadeDir = -1;           // the direction to fade : in = -1 or out = 1
    private bool m_bFinishedFade = true;// has the fade completed its current fade.
    private bool DrawShit = true;

    public bool HasFinishedFade() { return m_bFinishedFade; }

    private void OnGUI()
    {
        if (FadeOutTexture == null)
            return;

        // fade in/out in the alpha value using a direction at a speed set by the Fade speed variable - Converted into seconds with Time.deltaTime
        alpha += FadeDir * FadeSpeed * Time.deltaTime;

        //"Clamp" the alpha between 0 and 1 
        alpha = Mathf.Clamp01(alpha);

        if (DrawShit)
        {
            //set the colour of our GUI. All colour remain the same & alpha is set to our alpha variable.
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);                // Set alpha value
            GUI.depth = drawDepth; GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);                // Set alpha value                  // make the black texture render on top (Draw last)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeOutTexture);       // draw the texture to fill the entire screen area
        }
        else
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1);                // Set alpha value
            GUI.depth = drawDepth; GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);                // Set alpha value
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeOutTexture);       // draw the texture to fill the entire screen area
        }
    }

    private void OnEnable()
    {
        DrawShit = true;
    }
    private void OnDisable()
    {
        DrawShit = false;
    }
    /// <summary>
    /// <para> Set direction to 1 for FadeIn, and 0 for FadeOut </para>
    /// TargetScene is case Sensative : must match your scene name
    /// </summary>
    /// <param name="direction"> </param>

    /// <param name="TargetScene">Case Sensative</param>
    /// <returns></returns>
    public float BeginFade(int direction, string TargetScene = "")
    {
        m_bFinishedFade = false;
        //set the fadeDir to the direciton paremeter making the scene fade in if -1 and out if 1
        FadeDir = direction;
        StartCoroutine(BeginTransition(TargetScene));
        return (FadeSpeed); // return the fade speed variable so its easy to time the SceneManagement.LoadScneen("SCENE_NAME");
    }

    public float BeginFade(int direction)
    {
        m_bFinishedFade = false;
        //set the fadeDir to the direciton paremeter making the scene fade in if -1 and out if 1
        FadeDir = direction;
        StartCoroutine(BeginTransition());
        return (FadeSpeed);
    }

    public float BeginFadeCut(int direction)
    {
        m_bFinishedFade = false;
        FadeDir = direction;
        StartCoroutine(BeginTransitionCut(direction));
        return (FadeSpeed);
    }

    // is called when a level is loaded
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // Fade out on a level Loaded
        FadeOut();
    }

    IEnumerator BeginTransition(string TargetScene)
    {
        //yeild will wait on a different thread until its ready to call the next line
        yield return new WaitForSeconds(FadeSpeed);
        m_bFinishedFade = true;
        //called after yeild finishes on a different thread
        if (TargetScene != "")
            SceneManager.LoadScene(TargetScene);
        else
            Application.Quit();
    }

    IEnumerator BeginTransition()
    {
        //yeild will wait on a different thread until its ready to call the next line
        yield return new WaitForSeconds(FadeSpeed);
        m_bFinishedFade = true;
        EventToCall.Invoke();
    }

    IEnumerator BeginTransitionCut(int direction)
    {
        //yeild will wait on a different thread until its ready to call the next line
        yield return new WaitForSeconds(FadeSpeed);
        m_bFinishedFade = true;
        EventToCall.Invoke();

       
        FadeDir = direction * -1; // Invert current fade dir;
    }

    public void FadeOut()
    {
        alpha = 1;
        FadeDir = -1;
    }

   
}
