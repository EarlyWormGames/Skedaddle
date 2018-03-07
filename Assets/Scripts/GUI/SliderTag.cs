using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTag : MonoBehaviour {

    public TextMeshProUGUI SliderValueLabel;
    public int NoifDecimals = 1;

    public void ChangeLabel(float value)
    {
        string format = "F" + NoifDecimals.ToString();
        SliderValueLabel.text = value.ToString(format);
    }
}
