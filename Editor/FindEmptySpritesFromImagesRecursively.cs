using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace UnityEditorUtils
{
    public class FindEmptySpritesFromImagesRecursively : BaseFindMissingObjectsRecursively
    {
        [MenuItem("Window/FindEmptySpritesFromImagesRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindEmptySpritesFromImagesRecursively));
        }

        public override string GetObjectName()
        {
            return "sprites(s)";
        }

        protected override bool IsObjectEmpty(Component comp)
        {
            return comp is Image image && image.sprite == null;
        }
    }
}