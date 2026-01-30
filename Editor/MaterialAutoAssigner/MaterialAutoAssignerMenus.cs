using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace GameDevTools.MaterialAutoAssigner
{
    public static class MaterialAutoAssignerMenus
    {
        [MenuItem("Assets/Auto-Assign Maps", false, 1100)]
        private static void AutoAssignMaps()
        {
            Material mat = Selection.activeObject as Material;
            if (mat != null)
            {
                var maps = MaterialMapScanner.Scan(mat, SearchScope.SameFolderOnly);
                MaterialMapScanner.AssignMaps(mat, maps);
                Debug.Log($"Auto-Assigned maps for {mat.name}");
            }
        }

        [MenuItem("Assets/Auto-Assign Maps", true)]
        private static bool AutoAssignMapsValidation()
        {
            return Selection.activeObject is Material;
        }

        [MenuItem("Assets/Auto-Assign Maps (Preview)", false, 1101)]
        private static void AutoAssignMapsPreview()
        {
             MaterialAutoAssignerWindow.ShowWindow();
        }
         [MenuItem("Assets/Auto-Assign Maps (Preview)", true)]
        private static bool AutoAssignMapsPreviewValidation()
        {
            return Selection.activeObject is Material;
        }


        [MenuItem("Assets/Batch Auto-Assign Materials", false, 1102)]
        private static void BatchAutoAssignMaterials()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(folderPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
                int processedCount = 0;
                
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat != null)
                    {
                        var maps = MaterialMapScanner.Scan(mat, SearchScope.SameFolderOnly); // Default scope for batch
                        MaterialMapScanner.AssignMaps(mat, maps);
                        processedCount++;
                    }
                }
                
                Debug.Log($"Batch processed {processedCount} materials in {folderPath}");
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("Assets/Batch Auto-Assign Materials", true)]
        private static bool BatchAutoAssignValidation()
        {
             string path = AssetDatabase.GetAssetPath(Selection.activeObject);
             return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }
    }
}
