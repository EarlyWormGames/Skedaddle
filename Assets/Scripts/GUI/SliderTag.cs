using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



/// <summary>
/// Timer formatting for test scene
/// ty mitch
/// </summary>
public class SliderTag : MonoBehaviour {

    public TextMeshProUGUI SliderValueLabel;
    public int NoifDecimals = 1;

    public void ChangeLabel(float value)
    {
        string format = "F" + NoifDecimals.ToString();
        SliderValueLabel.text = value.ToString(format);
    }
}
