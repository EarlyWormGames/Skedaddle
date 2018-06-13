using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GUI SETTINGS for Animation Testing Scene
/// </summary>
public class HammerTesting : MonoBehaviour {

    public Pendulum[] Hammers;
    public TrailRenderer[] Trails;
    public Slider trailMinSlider;
    public Slider trailMaxSlider;
    public Slider SwingMinSlider;
    public Slider SwingMaxSlider;
    public Slider LengthMaxSlider;
    public Slider LengthMinSlider;
    public Slider OffsetMaxSlider;
    public Slider OffsetMinSlider;
    public Slider IntensityMaxSlider;
    public Slider IntensityMinSlider;
    private float trailTimeMin;
    private float trailTimeMax;
    private float swingTimeMin;
    private float swingTimeMax;
    private float swingLengthMin;
    private float swingLengthMax;
    private float swingOffsetMin;
    private float swingOffsetMax;
    private float swingIntensityMin;
    private float swingIntensityMax;

    void Start()
    {
        trailTimeMin = trailMinSlider.value;
        trailTimeMax = trailMaxSlider.value;
        swingTimeMin = SwingMinSlider.value;
        swingTimeMax = SwingMaxSlider.value;
        swingLengthMax = LengthMaxSlider.value;
        swingLengthMin = LengthMinSlider.value;
        swingOffsetMax = OffsetMaxSlider.value;
        swingOffsetMin = OffsetMinSlider.value;
        swingIntensityMax = IntensityMaxSlider.value;
        swingIntensityMin = IntensityMinSlider.value;
    }

    void Update()
    {
        for(int i = 0; i < Hammers.Length; i++)
        {
            float multi = ((float)i / (float)(Hammers.Length != 1 ? Hammers.Length - 1 : Hammers.Length));
            Hammers[i].m_fSwingTime = multi * (swingTimeMax - swingTimeMin) + swingTimeMin;
            Keyframe[] keys = Hammers[i].m_aSwingCurve.keys;
            keys[1].time = Mathf.Clamp01(0.5f - multi * (swingLengthMax - swingLengthMin) * 0.5f - swingLengthMin * 0.5f + multi * (swingOffsetMax - swingOffsetMin) + swingOffsetMin);
            keys[1].value = Mathf.Clamp01(0.5f - multi * (swingIntensityMax - swingIntensityMin) * 0.5f - swingIntensityMin * 0.5f);
            keys[2].time = Mathf.Clamp01(0.5f + multi * (swingLengthMax - swingLengthMin) * 0.5f + swingLengthMin * 0.5f + multi * (swingOffsetMax - swingOffsetMin) + swingOffsetMin);
            keys[2].value = Mathf.Clamp01(0.5f + multi * (swingIntensityMax - swingIntensityMin) * 0.5f + swingIntensityMin * 0.5f);
            Hammers[i].m_aSwingCurve.keys = keys;
        }
        for(int i = 0; i < Trails.Length; i++)
        {
            Trails[i].time = ((float)i / (float)(Trails.Length != 1 ? Trails.Length - 1 : Trails.Length)) * (trailTimeMax - trailTimeMin) + trailTimeMin;
        }
        trailMinSlider.value = Mathf.Clamp(trailMinSlider.value, -Mathf.Infinity, trailTimeMax);
        trailMaxSlider.value = Mathf.Clamp(trailMaxSlider.value, trailTimeMin, Mathf.Infinity);
        SwingMinSlider.value = Mathf.Clamp(SwingMinSlider.value, -Mathf.Infinity, swingTimeMax);
        SwingMaxSlider.value = Mathf.Clamp(SwingMaxSlider.value, swingTimeMin, Mathf.Infinity);
        LengthMaxSlider.value = Mathf.Clamp(LengthMaxSlider.value, swingLengthMin, Mathf.Infinity);
        LengthMinSlider.value = Mathf.Clamp(LengthMinSlider.value, -Mathf.Infinity, swingLengthMax);
        OffsetMaxSlider.value = Mathf.Clamp(OffsetMaxSlider.value, swingOffsetMin, Mathf.Infinity);
        OffsetMinSlider.value = Mathf.Clamp(OffsetMinSlider.value, -Mathf.Infinity, swingOffsetMax);
        IntensityMaxSlider.value = Mathf.Clamp(IntensityMaxSlider.value, swingIntensityMin, Mathf.Infinity);
        IntensityMinSlider.value = Mathf.Clamp(IntensityMinSlider.value, -Mathf.Infinity, swingIntensityMax);
    }
    /// <summary>
    /// public function for slider call
    /// </summary>
    public void ChangeTrailTimeMin(float SetTime)
    {
        trailTimeMin = SetTime;
        trailTimeMin = Mathf.Clamp(trailTimeMin, -Mathf.Infinity, trailTimeMax);
    }
    /// <summary>
    /// public function for slider call
    /// </summary>
    public void ChangeTrailTimeMax(float SetTime)
    {
        trailTimeMax = SetTime;
        trailTimeMax = Mathf.Clamp(trailTimeMax, trailTimeMin, Mathf.Infinity);
    }
    /// <summary>
    /// public function for slider call
    /// </summary>
    public void ChangeSwingTimeMin(float SetTime)
    {
        swingTimeMin = SetTime;
        swingTimeMin = Mathf.Clamp(swingTimeMin, -Mathf.Infinity, swingTimeMax);
    }
    /// <summary>
    /// public function for slider call
    /// </summary>
    public void ChangeSwingTimeMax(float SetTime)
    {
        swingTimeMax = SetTime;
        swingTimeMax = Mathf.Clamp(swingTimeMax, swingTimeMin, Mathf.Infinity);
    }

    public void ChangeSwingLengthMax(float SetLength)
    {
        swingLengthMax = SetLength;
        swingLengthMax = Mathf.Clamp(swingLengthMax, swingLengthMin, Mathf.Infinity);
    }

    public void ChangeSwingLengthMin(float SetLength)
    {
        swingLengthMin = SetLength;
        swingLengthMin = Mathf.Clamp(swingLengthMin, -Mathf.Infinity, swingLengthMax);
    }

    public void ChangeSwingOffsetMax(float SetOffset)
    {
        swingOffsetMax = SetOffset;
        swingOffsetMax = Mathf.Clamp(swingOffsetMax, swingOffsetMin, Mathf.Infinity);
    }

    public void ChangeSwingOffsetMin(float SetOffset)
    {
        swingOffsetMin = SetOffset;
        swingOffsetMin = Mathf.Clamp(swingOffsetMin, -Mathf.Infinity, swingOffsetMax);
    }

    public void ChangeSwingIntensityMax(float SetIntense)
    {
        swingIntensityMax = SetIntense;
        swingIntensityMax = Mathf.Clamp(swingIntensityMax, swingIntensityMin, Mathf.Infinity);
    }

    public void ChangeSwingIntensityMin(float SetIntense)
    {
        swingIntensityMin = SetIntense;
        swingIntensityMin = Mathf.Clamp(swingIntensityMin, -Mathf.Infinity, swingIntensityMax);
    }

    public void ResetSwing()
    {
        foreach(Pendulum hammer in Hammers)
        {
            hammer.ResetTimer();
        }
    }

    float SliderOutput(float max, float min)
    {
        return (max - min) + min;
    }
}
