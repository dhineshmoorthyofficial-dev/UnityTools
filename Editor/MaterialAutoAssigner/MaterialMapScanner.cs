using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameDevTools.MaterialAutoAssigner
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

    public class MaterialMapScanner
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

        // Standard Unity Shader Property Names
        private static readonly Dictionary<MapType, string[]> PropertyNames = new Dictionary<MapType, string[]>
        {
            { MapType.Albedo, new[] { "_BaseMap", "_BaseColorMap", "_MainTex", "BaseMap" } },
            { MapType.Normal, new[] { "_BumpMap", "_NormalMap" } },
            { MapType.Metallic, new[] { "_MetallicGlossMap", "_Metallic" } }, // Often used for metallic
            { MapType.Roughness, new[] { "_SpecGlossMap", "_SmoothnessMap", "_RoughnessMap" } }, // Often combined with metallic or separate
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

            // Get all candidate file paths based on scope
            string[] candidatePaths = GetCandidatePaths(materialFolder, scope);

            foreach (MapType type in System.Enum.GetValues(typeof(MapType)))
            {
                DetectedMap map = FindMap(type, baseName, candidatePaths);
                if (map != null)
                {
                    results.Add(map);
                }
            }
            
            // Post-Validation: Check consistency between maps
            ValidateConsistency(results);

            return results;
        }

        private static void ValidateConsistency(List<DetectedMap> maps)
        {
            if (maps.Count <= 1) return;

            // Find most common resolution
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

            string[] guids;
            if (scope == SearchScope.SameFolderOnly)
            {
                 // FindAssets doesn't limit to depth 1, so we must filter later or use Directory.GetFiles
                 // Using Directory.GetFiles is faster for local folder
                 try {
                     return Directory.GetFiles(rootFolder, "*.*", SearchOption.TopDirectoryOnly)
                         .Where(f => IsTextureFile(f))
                         .Select(f => f.Replace("\\", "/")) // Normalize paths
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
            else // EntireAssetsFolder
            {
                guids = AssetDatabase.FindAssets("t:Texture2D");
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
            // Simple heuristic: if material name has common suffixes, strip them?
            // Actually, usually the material name IS the base name (e.g. "Wood_Floor"), 
            // and textures are "Wood_Floor_Albedo". 
            // Often Materials are named "M_Wood_Floor" or "MT_Wood_Floor".
            
            string name = materialName;
            if (name.StartsWith("M_")) name = name.Substring(2);
            if (name.StartsWith("MT_")) name = name.Substring(3);
            
            // Remove common material suffixes if present
            if (name.EndsWith("_Mat")) name = name.Substring(0, name.Length - 4);

            return name;
        }

        private static DetectedMap FindMap(MapType type, string baseName, string[] candidatePaths)
        {
            if (!Suffixes.ContainsKey(type)) return null;

            foreach (var suffix in Suffixes[type])
            {
                // Strict match: BaseName + Suffix
                // We should also handle fuzzy matching later, but for now strict.
                // also check Case Insensitivity
                
                foreach (var path in candidatePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    
                    // Debugging
                    // Debug.Log($"Checking {fileName} against {baseName + suffix}");

                    // Check 1: Exact Match (Case Insensitive)
                    if (fileName.Equals(baseName + suffix, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return CreateDetectedMap(type, path);
                    }

                    // Check 2: Maybe base name is slightly different? 
                    // e.g. Material="Wood", Texture="T_Wood_Albedo"
                    // Not handling "T_" prefix for textures yet in strict match but good to note.
                }
            }
            
            // Try matching with "T_" prefix which is common for textures
            foreach (var suffix in Suffixes[type])
            {
                 foreach (var path in candidatePaths)
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                     if (fileName.Equals("T_" + baseName + suffix, System.StringComparison.OrdinalIgnoreCase))
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
                IsAssigned = true, // Default to true if found
                Warning = ValidateMap(type, tex, path)
            };
        }

        private static string ValidateMap(MapType type, Texture2D tex, string path)
        {
            // Basic validation
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return null;

            List<string> warnings = new List<string>();

            if (type == MapType.Normal)
            {
                if (importer.textureType != TextureImporterType.NormalMap)
                {
                    warnings.Add("Texture Type is not 'Normal Map'");
                }
            }
            else
            {
                 // For non-normal maps, usually we expect Default
                 if (importer.textureType == TextureImporterType.NormalMap)
                 {
                     warnings.Add("Texture Type is 'Normal Map' but should be Default/Sprite");
                 }
            }

            // sRGB check
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
                            // Check if the property is actually a texture
                            if (IsTextureProperty(material, propName))
                            {
                                material.SetTexture(propName, map.Texture);
                                assigned = true;
                                Debug.Log($"Assigned {map.Texture.name} to {propName} on {material.name}");
                            }
                            else
                            {
                                Debug.Log($"Skipping {propName} on {material.name} because it is not a texture property (it might be a float/range property).");
                            }
                        }
                    }
                    
                    if (assigned)
                    {
                        // Enable standard keywords
                        if (map.Type == MapType.Normal) material.EnableKeyword("_NORMALMAP");
                        if (map.Type == MapType.Emission) material.EnableKeyword("_EMISSION");
                        if (map.Type == MapType.Metallic || map.Type == MapType.Roughness) material.EnableKeyword("_METALLICGLOSSMAP");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find a valid Texture property for {map.Type} on material {material.name}. Checked: {string.Join(", ", PropertyNames[map.Type])}.");
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
                {
                    return shader.GetPropertyType(i) == UnityEngine.Rendering.ShaderPropertyType.Texture;
                }
            }
            return false;
        }
    }
}
