using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// GUI SETTINGS for Height Check testing scene
/// </summary>
public class BoxGapTesting : MonoBehaviour {

    public TextMeshProUGUI SliderValue;
    public SplineMovement splinePoints;
    public Slider DistanceSlider;
    public Dropdown LeftBoxList;
    public Dropdown RightBoxList;
    public GameObject[] LeftBoxes;
    public GameObject[] RightBoxes;
    public GameObject LeftTeleport;
    public GameObject RightTeleport;
    public Vector3 LeftOffset;
    public Vector3 RightOffset;

    internal GameObject currentLeftBox;
    internal GameObject currentRightBox;

	// Use this for initialization
	void Awake () {
        if(currentLeftBox == null)
        currentLeftBox = LeftBoxes[0];
        if(currentRightBox == null)
        currentRightBox = RightBoxes[0];
        LeftBoxChange();
        RightBoxChange();
        ChangeBoxDistance();
	}

    /// <summary>
    /// public function for a button call.
    /// </summary>
    public void LeftBoxChange()
    {
        int index = LeftBoxList.value;
        if (index < LeftBoxes.Length && index >= 0)
        {
            currentLeftBox = LeftBoxes[index];
            foreach(GameObject go in LeftBoxes)
            {
                if(go != currentLeftBox)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                }
            }
        }
        ChangeBoxDistance();
    }

    /// <summary>
    /// public function for a button call.
    /// </summary>
    public void RightBoxChange()
    {
        int index = RightBoxList.value;
        if (index < RightBoxes.Length && index >= 0)
        {
            currentRightBox = RightBoxes[index];
            foreach (GameObject go in RightBoxes)
            {
                if (go != currentRightBox)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                }
            }
        }
        ChangeBoxDistance();
    }

    /// <summary>
    /// public function for a button call using the settings from the inspector.
    /// </summary>
    public void ChangeBoxDistance()
    {
        int distance = (int)DistanceSlider.value;
        Collider LeftBoxCol = currentLeftBox.GetComponent<Collider>();
        Collider RightBoxCol = currentRightBox.GetComponent<Collider>();
        if(LeftBoxCol != null && RightBoxCol != null)
        currentRightBox.transform.position = new Vector3(currentLeftBox.transform.position.x + LeftBoxCol.bounds.size.x * 0.5f + RightBoxCol.bounds.size.x * 0.5f + distance * 0.01f, 
                                                        currentLeftBox.transform.position.y - LeftBoxCol.bounds.size.y * 0.5f + RightBoxCol.bounds.size.y * 0.5f, 
                                                        currentLeftBox.transform.position.z);
        LeftTeleport.transform.position = new Vector3(currentLeftBox.transform.position.x, currentLeftBox.transform.position.y + LeftBoxCol.bounds.size.y * 0.5f + 0.001f, currentLeftBox.transform.position.z);
        RightTeleport.transform.position = new Vector3(currentRightBox.transform.position.x, currentRightBox.transform.position.y + RightBoxCol.bounds.size.y * 0.5f + 0.001f, currentRightBox.transform.position.z);
        splinePoints.m_Spline.points[0] = splinePoints.m_Spline.transform.InverseTransformPoint(LeftTeleport.transform.position + LeftOffset);
        splinePoints.m_Spline.points[1] = splinePoints.m_Spline.points[0];
        splinePoints.m_Spline.points[3] = splinePoints.m_Spline.transform.InverseTransformPoint(RightTeleport.transform.position + RightOffset);
        splinePoints.m_Spline.points[2] = splinePoints.m_Spline.points[3];
        splinePoints.RegeneratePoints = true;
        SliderValue.text = distance.ToString();
    }
}
