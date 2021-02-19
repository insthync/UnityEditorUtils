using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEditorUtils
{
    /// <summary>
    /// Modified from codes in Wiki: http://wiki.unity3d.com/index.php/FindMissingScripts 
    /// Modified by Insthync
    /// </summary>
    public class FindMissingScriptsRecursively : EditorWindow
    {
        struct MissingScriptData
        {
            public GameObject Object { get; set; }
            public int ComponentIndex { get; set; }
        }
        int searchedCount = 0;
        int componentsCount = 0;
        int missingCount = 0;
        Vector2 scrollPos = Vector2.zero;
        List<MissingScriptData> missingScripts = new List<MissingScriptData>();

        [MenuItem("Window/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }
            GUILayout.EndHorizontal();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < missingScripts.Count; i++)
            {
                if (missingScripts[i].Object == null)
                    continue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(missingScripts[i].Object.name, missingScripts[i].Object, typeof(GameObject), true);
                GUILayout.Space(20);
                GUILayout.Label("Index: " + missingScripts[i].ComponentIndex, GUILayout.Width(80));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", searchedCount, componentsCount, missingCount));
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            searchedCount = 0;
            componentsCount = 0;
            missingCount = 0;
            missingScripts.Clear();
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", searchedCount, componentsCount, missingCount));
        }

        private void FindInGO(GameObject obj)
        {
            searchedCount++;
            Component[] components = obj.GetComponents<Component>();
            for (int idx = 0; idx < components.Length; idx++)
            {
                componentsCount++;
                if (components[idx] == null)
                {
                    missingCount++;
                    string path = obj.name;
                    Transform t = obj.transform;
                    while (t.parent != null)
                    {
                        path = t.parent.name + "/" + path;
                        t = t.parent;
                    }
                    Debug.Log(path + " has an empty script attached in position: " + idx, obj);
                    missingScripts.Add(new MissingScriptData()
                    {
                        Object = obj,
                        ComponentIndex = idx,
                    });
                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in obj.transform)
            {
                FindInGO(childT.gameObject);
            }
        }
    }
}
