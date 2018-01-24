using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EWCinema
{
    public class CinemaWindow : EditorWindow
    {
        [MenuItem("Window/Cinema")]
        public static void OpenWindow()
        {
            CinemaWindow window = GetWindow<CinemaWindow>();
            window.Show();
        }

        public static void OpenWindow(CinemaObject a_object)
        {
            CinemaWindow window = GetWindow<CinemaWindow>();
            window.Show();
            window.currentObject = a_object;
        }


        private CinemaObject currentObject = null;
        private bool init = false;
        private GUIStyle headerStyle;
        private GUIStyle wrapStyle;

        private float leftBarWidth = 200f;

        void Init()
        {
            init = true;
            titleContent = new GUIContent("Cinema");
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 18;

            wrapStyle = new GUIStyle(GUI.skin.label);
            wrapStyle.wordWrap = true;
        }

        void OnGUI()
        {
            if (!init)
                Init();

            if (currentObject == null)
            {
                //DEFAULT, NO OBJECT TEXT

                float x = Mathf.Max(100, (position.width / 2) - 250);
                float y = Mathf.Max(100, (position.height / 2) - 35);

                GUI.Label(new Rect(x, y, 500, 50), "No cinema object selected. To continue to must either", headerStyle);
                if (GUI.Button(new Rect(x + 125, y + 50, 100, 25), "SELECT one"))
                {
                    string file = EditorUtility.OpenFilePanel("Open Cinema Object", "", "asset");
                    if (file.Length != 0)
                    {
                        CinemaObject obj = AssetDatabase.LoadAssetAtPath<CinemaObject>(file);
                        if (obj != null)
                            currentObject = obj;
                    }
                }
                GUI.Label(new Rect(x + 125 + 105, y + 50, 25, 25), "OR");
                if (GUI.Button(new Rect(x + 125 + 132, y + 50, 100, 25), "CREATE one"))
                {
                    string file = EditorUtility.SaveFilePanelInProject("New Cinema Object", "CinemaObject", "asset", "New Cinema Object");
                    if (file.Length != 0)
                    {
                        CinemaObject cin = CreateInstance<CinemaObject>();
                        AssetDatabase.CreateAsset(cin, file);
                        currentObject = cin;
                    }
                }
                return;
            }


            #region LEFT-BAR
            {
                if (currentObject.m_Items.Length == 0)
                {
                    GUI.Label(new Rect(5, 5, leftBarWidth - 10, position.height - 10), "Drag an object in to begin", wrapStyle);
                }
            }
            #endregion
        }
    }
}