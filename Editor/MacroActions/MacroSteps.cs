using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityProductivityTools.MacroActions
{
    [Serializable]
    public abstract class MacroStep
    {
        public bool enabled = true;
        
        public abstract string GetName();
        public abstract void Execute(Object[] targets);
        
        protected void RecordUndo(Object target, string actionName)
        {
            if (target != null)
            {
                Undo.RecordObject(target, actionName);
            }
        }
    }

    [Serializable]
    public class AddComponentStep : MacroStep
    {
        public string componentType;

        public override string GetName() => $"Add Component: {componentType}";

        public override void Execute(Object[] targets)
        {
            if (targets == null) return;
            
            var type = System.Type.GetType(componentType);
            if (type == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(componentType);
                    if (type != null) break;
                }
            }

            if (type == null)
            {
                Debug.LogWarning($"Macro: Could not find type {componentType}");
                return;
            }

            foreach (var target in targets)
            {
                if (target is GameObject go)
                {
                    Undo.AddComponent(go, type);
                }
            }
        }
    }

    [Serializable]
    public class RenameStep : MacroStep
    {
        [Tooltip("Use '###' as a placeholder for the index (e.g. 'Item_###' -> 'Item_001')")]
        public string newName;
        public int startIndex = 1;
        [Tooltip("If true, the startIndex will be incremented by the number of objects renamed, persisting the counter for the next run.")]
        public bool persistIncrement = true;

        public override string GetName() => $"Rename to: {newName}";

        public override void Execute(Object[] targets)
        {
            if (targets == null || string.IsNullOrEmpty(newName)) return;
            
            int renameCount = 0;
            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                if (target == null) continue;

                Undo.RecordObject(target, "Macro Rename");
                
                string finalName = newName;
                if (newName.Contains("###"))
                {
                    int number = startIndex + i;
                    finalName = newName.Replace("###", number.ToString());
                }
                else if (persistIncrement)
                {
                    // Fallback: if no placeholder but persist is on, append the index
                    int number = startIndex + i;
                    finalName = $"{newName} {number}";
                }
                
                target.name = finalName;
                renameCount++;
            }

            if (persistIncrement && renameCount > 0)
            {
                startIndex += renameCount;
            }
        }
    }

    [Serializable]
    public class MenuCommandStep : MacroStep
    {
        public string commandPath;

        public override string GetName() => $"Menu Command: {commandPath}";

        public override void Execute(Object[] targets)
        {
            // Menu commands typically act on the global selection already.
            // We run it only once regardless of target count.
            EditorApplication.ExecuteMenuItem(commandPath);
        }
    }
    

    [Serializable]
    public class SetActiveStep : MacroStep
    {
        public bool active = true;

        public override string GetName() => $"Set Active: {active}";

        public override void Execute(Object[] targets)
        {
            if (targets == null) return;
            foreach (var target in targets)
            {
                if (target is GameObject go)
                {
                    Undo.RecordObject(go, "Set Active");
                    go.SetActive(active);
                }
            }
        }
    }
}
