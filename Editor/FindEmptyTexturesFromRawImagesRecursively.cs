using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Insthync.UnityEditorUtils.Editor
{
    public class FindEmptyTexturesFromRawImagesRecursively : BaseFindMissingObjectsRecursively
    {
        [MenuItem("Window/FindEmptyTexturesFromRawImagesRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindEmptyTexturesFromRawImagesRecursively));
        }

        public override string GetObjectName()
        {
            return "texture(s)";
        }

        protected override bool IsObjectEmpty(Component comp)
        {
            return comp is RawImage rawImage && rawImage.texture == null;
        }
    }
}