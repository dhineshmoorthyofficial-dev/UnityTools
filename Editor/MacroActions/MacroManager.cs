using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.MacroActions
{
    public static class MacroManager
    {
        private const string DefaultFolder = "Assets/Editor/MacroActions";

        public static List<MacroAction> GetAllMacros()
        {
            List<MacroAction> macros = new List<MacroAction>();
            string[] guids = AssetDatabase.FindAssets("t:MacroAction");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                macros.Add(AssetDatabase.LoadAssetAtPath<MacroAction>(path));
            }
            return macros;
        }

        public static MacroAction CreateMacro(string name)
        {
            if (!AssetDatabase.IsValidFolder(DefaultFolder))
            {
                Directory.CreateDirectory(DefaultFolder);
                AssetDatabase.Refresh();
            }

            MacroAction asset = ScriptableObject.CreateInstance<MacroAction>();
            string path = AssetDatabase.GenerateUniqueAssetPath($"{DefaultFolder}/{name}.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        public static void DeleteMacro(MacroAction macro)
        {
            if (macro == null) return;
            string path = AssetDatabase.GetAssetPath(macro);
            AssetDatabase.DeleteAsset(path);
        }
    }
}
