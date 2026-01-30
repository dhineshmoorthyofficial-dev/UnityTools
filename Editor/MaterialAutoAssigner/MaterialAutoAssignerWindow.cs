using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace GameDevTools.MaterialAutoAssigner
{
    public class MaterialAutoAssignerWindow : EditorWindow
    {
        private Material selectedMaterial;
        private SearchScope searchScope = SearchScope.SameFolderOnly;
        private List<MaterialMapScanner.DetectedMap> detectedMaps = new List<MaterialMapScanner.DetectedMap>();
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/GameDevTools/Material Map Auto-Assigner")]
        public static void ShowWindow()
        {
            GetWindow<MaterialAutoAssignerWindow>("Material Auto-Assigner");
        }

        private void OnEnable()
        {
            // Optional: Auto-select material if one is selected in Project view
            if (Selection.activeObject is Material mat)
            {
                selectedMaterial = mat;
                ScanMaps();
            }
        }

        private void OnGUI()
        {
            DrawTopSection();
            DrawSearchSettings();
            DrawDetectedMaps();
            DrawBottomSection();
        }

        private void DrawTopSection()
        {
            EditorGUILayout.LabelField("Material Selection", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            selectedMaterial = (Material)EditorGUILayout.ObjectField("Target Material", selectedMaterial, typeof(Material), false);
            if (EditorGUI.EndChangeCheck())
            {
                ScanMaps();
            }

            if (GUILayout.Button("Use Selected Material"))
            {
                if (Selection.activeObject is Material mat)
                {
                    selectedMaterial = mat;
                    ScanMaps();
                }
                else
                {
                    EditorUtility.DisplayDialog("Info", "Please select a material in the Project View first.", "OK");
                }
            }

            EditorGUILayout.Space();
            if (selectedMaterial != null)
            {
                // Simple preview or name display
                EditorGUILayout.HelpBox($"Selected: {selectedMaterial.name}", MessageType.Info);
            }
        }

        private void DrawSearchSettings()
        {
            EditorGUILayout.LabelField("Search Settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            searchScope = (SearchScope)EditorGUILayout.EnumPopup("Search Scope", searchScope);
            if (EditorGUI.EndChangeCheck() && selectedMaterial != null)
            {
                ScanMaps();
            }
            EditorGUILayout.Space();
        }

        private void DrawDetectedMaps()
        {
            EditorGUILayout.LabelField("Detected Maps", EditorStyles.boldLabel);
            
            if (selectedMaterial == null)
            {
                EditorGUILayout.HelpBox("Select a material to start scanning.", MessageType.None);
                return;
            }

            if (detectedMaps == null || detectedMaps.Count == 0)
            {
                 EditorGUILayout.HelpBox("No maps detected based on naming conventions.", MessageType.Warning);
                 if (GUILayout.Button("Manual Rescan"))
                 {
                     ScanMaps();
                 }
                 return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(200));

            foreach (var map in detectedMaps)
            {
                EditorGUILayout.BeginHorizontal("box");
                
                // Checkbox
                map.IsAssigned = EditorGUILayout.Toggle(map.IsAssigned, GUILayout.Width(20));

                // Type Label
                EditorGUILayout.LabelField(map.Type.ToString(), GUILayout.Width(80));

                // Texture Object
                map.Texture = (Texture2D)EditorGUILayout.ObjectField(map.Texture, typeof(Texture2D), false);

                // Validation Warning
                if (!string.IsNullOrEmpty(map.Warning))
                {
                    var icon = EditorGUIUtility.IconContent("console.warnicon.sml");
                    icon.tooltip = map.Warning;
                    GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBottomSection()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Scan for Maps", GUILayout.Height(30)))
            {
                ScanMaps();
            }

            if (GUILayout.Button("Auto-Assign All", GUILayout.Height(30)))
            {
                AssignMaps();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void ScanMaps()
        {
            if (selectedMaterial == null) return;
            detectedMaps = MaterialMapScanner.Scan(selectedMaterial, searchScope);
        }

        private void AssignMaps()
        {
            if (selectedMaterial == null) return;
            MaterialMapScanner.AssignMaps(selectedMaterial, detectedMaps);
            AssetDatabase.SaveAssets(); // Ensure changes are saved
            EditorUtility.DisplayDialog("Success", "Maps assigned successfully!", "OK");
        }
    }
}
