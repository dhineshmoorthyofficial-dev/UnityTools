using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools
{
    public enum SearchScope
    {
        SameFolderOnly,
        IncludeSubfolders,
        EntireAssetsFolder
    }

    public enum MapType
    {
        Albedo,
        Normal,
        Metallic,
        Roughness,
        Occlusion,
        Height,
        Emission,
        DetailMask,
        DetailAlbedo,
        DetailNormal
    }

    public static class MaterialMapScanner
    {
        public class DetectedMap
        {
            public MapType Type;
            public Texture2D Texture;
            public string TexturePath;
            public bool IsAssigned;
            public string Warning;
        }

        private static readonly Dictionary<MapType, string[]> Suffixes = new Dictionary<MapType, string[]>
        {
            { MapType.Albedo, new[] { "_Albedo","_AlbedoTransparency", "_BaseColor", "_Diffuse", "_Color", "_D", "_C" } },
            { MapType.Normal, new[] { "_Normal", "_NormalMap", "_N", "_Norm", "_NRM" } },
            { MapType.Metallic, new[] { "_Metallic","_MetallicSmoothness", "_Metal", "_M", "_Met" } },
            { MapType.Roughness, new[] { "_Roughness", "_Rough", "_R", "_Smoothness", "_Smooth", "_S" } },
            { MapType.Occlusion, new[] { "_AO", "_Occlusion", "_AmbientOcclusion" } },
            { MapType.Height, new[] { "_Height", "_H", "_Displacement", "_Disp" } },
            { MapType.Emission, new[] { "_Emission", "_Emissive", "_E" } },
            { MapType.DetailMask, new[] { "_DetailMask", "_Mask" } },
            { MapType.DetailAlbedo, new[] { "_DetailAlbedo", "_Detail" } },
            { MapType.DetailNormal, new[] { "_DetailNormal" } }
        };

        private static readonly Dictionary<MapType, string[]> PropertyNames = new Dictionary<MapType, string[]>
        {
            { MapType.Albedo, new[] { "_BaseMap", "_BaseColorMap", "_MainTex", "BaseMap" } },
            { MapType.Normal, new[] { "_BumpMap", "_NormalMap" } },
            { MapType.Metallic, new[] { "_MetallicGlossMap", "_Metallic" } },
            { MapType.Roughness, new[] { "_SpecGlossMap", "_SmoothnessMap", "_RoughnessMap" } },
            { MapType.Occlusion, new[] { "_OcclusionMap" } },
            { MapType.Height, new[] { "_ParallaxMap", "_HeightMap" } },
            { MapType.Emission, new[] { "_EmissionMap" } },
            { MapType.DetailMask, new[] { "_DetailMask" } },
            { MapType.DetailAlbedo, new[] { "_DetailAlbedoMap" } },
            { MapType.DetailNormal, new[] { "_DetailNormalMap" } }
        };

        public static List<DetectedMap> Scan(Material material, SearchScope scope)
        {
            var results = new List<DetectedMap>();
            if (material == null) return results;

            string materialPath = AssetDatabase.GetAssetPath(material);
            string materialFolder = Path.GetDirectoryName(materialPath);
            string baseName = ExtractBaseName(material.name);

            string[] candidatePaths = GetCandidatePaths(materialFolder, scope);

            foreach (MapType type in Enum.GetValues(typeof(MapType)))
            {
                DetectedMap map = FindMap(type, baseName, candidatePaths);
                if (map != null) results.Add(map);
            }
            
            ValidateConsistency(results);
            return results;
        }

        private static void ValidateConsistency(List<DetectedMap> maps)
        {
            if (maps.Count <= 1) return;
            var resolutions = maps.Select(m => m.Texture.width + "x" + m.Texture.height).ToList();
            var mostCommon = resolutions.GroupBy(r => r).OrderByDescending(g => g.Count()).First().Key;

            foreach (var map in maps)
            {
                string res = map.Texture.width + "x" + map.Texture.height;
                if (res != mostCommon)
                {
                    string resWarning = $"Resolution mismatch ({res}). Expected {mostCommon}.";
                    map.Warning = string.IsNullOrEmpty(map.Warning) ? resWarning : map.Warning + "\n" + resWarning;
                }
            }
        }
        
        private static string[] GetCandidatePaths(string rootFolder, SearchScope scope)
        {
            if (string.IsNullOrEmpty(rootFolder)) return new string[0];

            if (scope == SearchScope.SameFolderOnly)
            {
                 try {
                     return Directory.GetFiles(rootFolder, "*.*", SearchOption.TopDirectoryOnly)
                         .Where(f => IsTextureFile(f))
                         .Select(f => f.Replace("\\", "/"))
                         .ToArray();
                 } catch { return new string[0]; }
            }
            else if (scope == SearchScope.IncludeSubfolders)
            {
                 try {
                     return Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories)
                         .Where(f => IsTextureFile(f))
                         .Select(f => f.Replace("\\", "/"))
                         .ToArray();
                 } catch { return new string[0]; }
            }
            else
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture2D");
                return guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).ToArray();
            }
        }

        private static bool IsTextureFile(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".tif" || ext == ".tiff" || ext == ".psd";
        }

        private static string ExtractBaseName(string materialName)
        {
            string name = materialName;
            if (name.StartsWith("M_")) name = name.Substring(2);
            if (name.StartsWith("MT_")) name = name.Substring(3);
            if (name.EndsWith("_Mat")) name = name.Substring(0, name.Length - 4);
            return name;
        }

        private static DetectedMap FindMap(MapType type, string baseName, string[] candidatePaths)
        {
            if (!Suffixes.ContainsKey(type)) return null;

            foreach (var suffix in Suffixes[type])
            {
                foreach (var path in candidatePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    if (fileName.Equals(baseName + suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        return CreateDetectedMap(type, path);
                    }
                }
            }
            
            foreach (var suffix in Suffixes[type])
            {
                 foreach (var path in candidatePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                     if (fileName.Equals("T_" + baseName + suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        return CreateDetectedMap(type, path);
                    }
                }
            }
            return null;
        }

        private static DetectedMap CreateDetectedMap(MapType type, string path)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null) return null;

            return new DetectedMap
            {
                Type = type,
                Texture = tex,
                TexturePath = path,
                IsAssigned = true,
                Warning = ValidateMap(type, tex, path)
            };
        }

        private static string ValidateMap(MapType type, Texture2D tex, string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return null;

            List<string> warnings = new List<string>();

            if (type == MapType.Normal)
            {
                if (importer.textureType != TextureImporterType.NormalMap)
                    warnings.Add("Texture Type is not 'Normal Map'");
            }
            else
            {
                 if (importer.textureType == TextureImporterType.NormalMap)
                     warnings.Add("Texture Type is 'Normal Map' but should be Default/Sprite");
            }

            if (type == MapType.Albedo || type == MapType.Emission)
            {
                if (!importer.sRGBTexture) warnings.Add("Texture should likely be sRGB (Color)");
            }
            else if (type == MapType.Normal || type == MapType.Metallic || type == MapType.Roughness || type == MapType.Occlusion || type == MapType.Height)
            {
                if (importer.sRGBTexture) warnings.Add("Texture should likely be Linear (not sRGB)");
            }

            return warnings.Count > 0 ? string.Join("\n", warnings) : null;
        }

        public static void AssignMaps(Material material, List<DetectedMap> mapsToAssign)
        {
            Undo.RecordObject(material, "Auto-Assign Material Maps");
            
            foreach (var map in mapsToAssign)
            {
                if (!map.IsAssigned || map.Texture == null) continue;

                if (PropertyNames.ContainsKey(map.Type))
                {
                    bool assigned = false;
                    foreach (var propName in PropertyNames[map.Type])
                    {
                        if (material.HasProperty(propName))
                        {
                            if (IsTextureProperty(material, propName))
                            {
                                material.SetTexture(propName, map.Texture);
                                assigned = true;
                            }
                        }
                    }
                    
                    if (assigned)
                    {
                        if (map.Type == MapType.Normal) material.EnableKeyword("_NORMALMAP");
                        if (map.Type == MapType.Emission) material.EnableKeyword("_EMISSION");
                        if (map.Type == MapType.Metallic || map.Type == MapType.Roughness) material.EnableKeyword("_METALLICGLOSSMAP");
                    }
                }
            }
        }

        private static bool IsTextureProperty(Material mat, string propName)
        {
            Shader shader = mat.shader;
            int count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                if (shader.GetPropertyName(i) == propName)
                    return shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture;
            }
            return false;
        }
    }

    public class MaterialAutoAssignerWindow : EditorWindow
    {
        private Material selectedMaterial;
        private SearchScope searchScope = SearchScope.SameFolderOnly;
        private List<MaterialMapScanner.DetectedMap> detectedMaps = new List<MaterialMapScanner.DetectedMap>();
        private Vector2 scrollPosition;
        
        [MenuItem("Tools/GameDevTools/Material Auto Assigner", false, 185)]
        public static void ShowWindow()
        {
            GetWindow<MaterialAutoAssignerWindow>("Material Auto Assigner");
        }

        private void OnEnable()
        {
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
            if (selectedMaterial == null) return;

            if (detectedMaps == null || detectedMaps.Count == 0)
            {
                 EditorGUILayout.HelpBox("No maps detected.", MessageType.Warning);
                 return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(200));
            foreach (var map in detectedMaps)
            {
                EditorGUILayout.BeginHorizontal("box");
                map.IsAssigned = EditorGUILayout.Toggle(map.IsAssigned, GUILayout.Width(20));
                EditorGUILayout.LabelField(map.Type.ToString(), GUILayout.Width(80));
                map.Texture = (Texture2D)EditorGUILayout.ObjectField(map.Texture, typeof(Texture2D), false);
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
            if (GUILayout.Button("Scan for Maps", GUILayout.Height(30))) ScanMaps();
            if (GUILayout.Button("Auto-Assign All", GUILayout.Height(30))) AssignMaps();
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
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", "Maps assigned successfully!", "OK");
        }
    }

    public static class MaterialAutoAssignerAssetMenus
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
        private static bool AutoAssignMapsValidation() => Selection.activeObject is Material;

        [MenuItem("Assets/Batch Auto-Assign Materials", false, 1102)]
        private static void BatchAutoAssignMaterials()
        {
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Directory.Exists(folderPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat != null)
                    {
                        var maps = MaterialMapScanner.Scan(mat, SearchScope.SameFolderOnly);
                        MaterialMapScanner.AssignMaps(mat, maps);
                    }
                }
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
