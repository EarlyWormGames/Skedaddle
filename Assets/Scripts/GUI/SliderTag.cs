using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTag : MonoBehaviour {

    public TextMeshProUGUI SliderValueLabel;

    public void ChangeLabel(float value)
    {
        SliderValueLabel.text = value.ToString("0.0");
    }
}
