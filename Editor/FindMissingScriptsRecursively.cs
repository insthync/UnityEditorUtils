using UnityEngine;
using UnityEditor;

namespace Insthync.UnityEditorUtils.Editor
{
    public class FindMissingScriptsRecursively : BaseFindMissingObjectsRecursively
    {
        [MenuItem("Window/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public override string GetObjectName()
        {
            return "script(s)";
        }

        protected override bool IsObjectEmpty(Component comp)
        {
            return comp == null;
        }
    }
}
