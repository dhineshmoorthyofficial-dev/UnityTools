using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.MacroActions
{
    public static class MacroExecutor
    {
        public static void Execute(MacroAction macro, Object[] targets)
        {
            if (macro == null || macro.steps == null || macro.steps.Count == 0)
                return;

            if (targets == null || targets.Length == 0)
            {
                // Some macros might not need targets (e.g., pure menu commands)
                // but usually we want to group the undo.
                ExecuteInternal(macro, targets);
                return;
            }

            Undo.SetCurrentGroupName($"Macro: {macro.name}");
            int group = Undo.GetCurrentGroup();

            ExecuteInternal(macro, targets);
            EditorUtility.SetDirty(macro);

            Undo.CollapseUndoOperations(group);
        }

        private static void ExecuteInternal(MacroAction macro, Object[] targets)
        {
            var targetList = new System.Collections.Generic.List<Object>();
            var currentTargets = targets;

            foreach (var step in macro.steps)
            {
                if (step == null || !step.enabled) continue;

                targetList.Clear();
                if (currentTargets != null && currentTargets.Length > 0)
                {
                    foreach (var target in currentTargets)
                    {
                        if (target == null) continue;

                        // Check context
                        if (macro.context == MacroTargetContext.GameObjectsOnly && !(target is GameObject))
                            continue;
                        if (macro.context == MacroTargetContext.AssetsOnly && !AssetDatabase.Contains(target))
                            continue;

                        targetList.Add(target);
                    }
                }

                // Pass filtered targets (or null if none) to the step
                step.Execute(targetList.Count > 0 ? targetList.ToArray() : null);

                // If tracking selection, update source targets for the next step from Unity selection
                if (macro.trackSelection)
                {
                    currentTargets = Selection.objects;
                }
            }
        }
    }
}
