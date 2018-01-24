using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class TextureStringGenerator: EditorWindow
    {
        [MenuItem("Edit/Texture2Base64String")]
        static void OpenWindow()
        {
            GetWindow<TextureStringGenerator>();
        }

        Texture2D tex;
        string generatedString = "";

        void OnGUI()
        {
            EditorGUILayout.HelpBox("Converts a Texture to a base64 string", MessageType.None);
            tex = EditorGUILayout.ObjectField(tex, typeof(Texture2D), false) as Texture2D;
            if (tex)
            {
                GUILayout.Label(string.Format("Texture size is: {0}x{1}", tex.width, tex.height));
                if (GUILayout.Button("Generate", GUILayout.Width(100)))
                {
                    if (tex == null) { return; }
                    byte[] texBytes = tex.EncodeToPNG();
                    generatedString = System.Convert.ToBase64String(texBytes);
                }
                GUI.enabled = generatedString.Length > 0;
                EditorGUILayout.TextArea(generatedString, GUILayout.Height(100));
                if (GUILayout.Button("Copy to clipboard"))
                {
                    System.Windows.Forms.Clipboard.SetText(generatedString);
                }
            }
        }
    }
}
