// This class can be used to create any ScriptableObject type via Reflection.
// To use it, simply right click in the Project Area and select Create ->
// ScriptableObject.
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

class EndNameEdit : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
    }
}

public class CreateScriptableObject : EditorWindow
{
    public static string[] assemblyNames = new string[] { "Assembly-CSharp" };

    Vector2 scrollViewPosition = Vector2.zero;

    static List<Type> FindTypes(string name)
    {
        var types = new List<Type>();
        try
        {
            // get project assembly
            var asm = Assembly.Load(new AssemblyName(name));

            // filter out all the ScriptableObject types
            foreach (Type t in asm.GetTypes())
            {
                if (t.IsSubclassOf(typeof(ScriptableObject)) && !t.IsAbstract)
                    types.Add(t);
            }
        }
        catch { }

        return types;
    }

    void OnGUI()
    {
        GUILayout.Label("Select the type to create:");
        scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition, false, false);
        foreach (var assemblyName in assemblyNames)
        {
            foreach (Type t in FindTypes(assemblyName))
            {
                if (GUILayout.Button(t.FullName))
                {
                    // create the asset, select it, allow renaming, close
                    var asset = CreateInstance(t);
                    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), CreateInstance<EndNameEdit>(), t.FullName + ".asset", AssetPreview.GetMiniThumbnail(asset), null);
                    Close();
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Assets/Create/ScriptableObject")]
    public static void ShowWindow()
    {
        var win = GetWindow<CreateScriptableObject>(true, "Create ScriptableObject");
        win.ShowPopup();
    }
}
