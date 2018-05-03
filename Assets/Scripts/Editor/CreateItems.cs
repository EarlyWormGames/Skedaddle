using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;

public class CreateItems
{
    public static void StartRenameSelectedAsset()
    {
        var e = new Event();
        e.keyCode = KeyCode.F2;
        e.type = EventType.KeyDown;
        EditorWindow.focusedWindow.SendEvent(e);
    }

    [MenuItem("Assets/Create/Object Script", false, 84)]
    public static void CreateObject()
    {
        string file = AssetDatabase.GetAssetPath(Selection.activeObject);
        CloneScript("Assets/Resources/Scripts/System/ObjectTemplate.cs", file + "/New Object Script.cs", file);
    }

    [MenuItem("Assets/Create/Animal Script", false, 85)]
    public static void CreateAnimal()
    {
        string file = AssetDatabase.GetAssetPath(Selection.activeObject);
        CloneScript("Assets/Resources/Scripts/System/AnimalTemplate.cs", file + "/New Animal Script.cs", file);
    }

    [MenuItem("Early Worm/Unload Assets")]
    public static void Unload()
    {
        Resources.UnloadUnusedAssets();
    }

    public static void CloneScript(string a_sourceFile, string a_newFile, string a_directory)
    {
        string dataPath = Application.dataPath.Replace("Assets", "");

        //Open up the dialogue
        EditorWindow.GetWindow<Dialogue>("Save File").Init(a_sourceFile, dataPath + a_newFile, a_directory);
        EditorWindow.FocusWindowIfItsOpen<Dialogue>();
    }

    //GameObject creation stuff
    [MenuItem("GameObject/EW", false, 10)]

    [MenuItem("GameObject/EW/GameController", false, 5)]
    public static void CreateGC()
    {
        CreateGameObject(ITEM_LIST.GC);
    }

    [MenuItem("GameObject/EW/Animals/Loris", false, 5)]
    public static void CreateLoris()
    {
        CreateGameObject(ITEM_LIST.LORIS);
    }

    public static void CreateGameObject(ITEM_LIST a_objType)
    {
        GameObject obj = new GameObject();
        Items items = obj.AddComponent<Items>();
        FieldInfo[] fields = typeof(Items).GetFields(BindingFlags.Public | BindingFlags.Instance);
        object gameObj = null;
        gameObj = fields[(int)a_objType].GetValue(items);
        if (gameObj != null)
        {
            Object.Instantiate(gameObj as GameObject);
        }
        gameObj = null;
        Object.DestroyImmediate(obj);
    }
}

public class Dialogue : EditorWindow
{
    string newFile;
    string sourceFile;
    string folder;

    bool start = true;

    public void Init(string a_source, string a_file, string a_directory)
    {
        folder = a_directory;
        sourceFile = a_source;
        newFile = new FileInfo(a_file).Name.Replace(".cs", "");
    }

    void OnGUI()
    {
        //Draw the text field and the button
        FocusWindowIfItsOpen<Dialogue>();
        GUI.SetNextControlName("File Name");
        newFile = EditorGUILayout.TextField("File Name", newFile);
        if (start)
        {
            start = false;
            EditorGUI.FocusTextInControl("File Name");
        }

        if (GUILayout.Button("Save Script"))
        {
            OnClickSave();
            //GUIUtility.ExitGUI();
        }

        if (Event.current.keyCode == KeyCode.Return)
        {
            OnClickSave();
        }
        
    }

    void OnClickSave()
    {
        newFile = newFile.Trim();

        if (string.IsNullOrEmpty(newFile))
        {
            EditorUtility.DisplayDialog("Unable to save script", "Please specify a valid name.", "Close");
            return;
        }

        newFile = newFile + (newFile.Contains(".cs") ? "" : ".cs");

        //Clone the asset
        AssetDatabase.CopyAsset(sourceFile, folder + "/" + newFile);

        //Get the path, the filename and read the contents
        string path = Application.dataPath.Replace("Assets", "") + folder + "/" + newFile + (newFile.Contains(".cs")? "" : ".cs");
        string name = System.IO.Path.GetFileNameWithoutExtension(path);

        File.SetAttributes(path, FileAttributes.Normal);

        string text = File.ReadAllText(path);

        //Get the filename of the source
        string sourceName = System.IO.Path.GetFileNameWithoutExtension(Application.dataPath.Replace("Assets", "") + sourceFile.Replace("Assets", ""));

        name = name.Replace(" ", "");
        text = text.Replace(sourceName, name);

        File.WriteAllText(path, text);
        AssetDatabase.Refresh();
        Close();
    }
}
