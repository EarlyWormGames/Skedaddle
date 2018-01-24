using UnityEngine;
using UnityEditor;
using System;

public class AudioGroupWindow : EditorWindow
{
    [MenuItem("FMOD/Show Mixer")]
    public static void ShowWindow()
    {
        GetWindow<AudioGroupWindow>().Show();
        GetWindow<AudioGroupWindow>().titleContent = new GUIContent("FMOD Mixer");
    }

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUIStyle label = new GUIStyle(GUI.skin.label);
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = 18;
            GUI.Label(new Rect(0, 0, position.width, position.height), "Only available in Play mode", label);
            return;
        }

        GUILayout.BeginHorizontal();
        int length= Enum.GetNames(typeof(eAudioGroup)).Length;
        for (int i = 0; i < length; ++i)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(((eAudioGroup)i).ToString());
            float volume = GUILayout.VerticalSlider(AudioGroup.GetRawVolume((eAudioGroup)i), 10, -10);

            if (volume != AudioGroup.GetRawVolume((eAudioGroup)i))
            {
                AudioGroup.SetVolume((eAudioGroup)i, volume);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
}