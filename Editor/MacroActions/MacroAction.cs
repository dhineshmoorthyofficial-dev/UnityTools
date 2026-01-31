using System.Collections.Generic;
using UnityEngine;

namespace UnityProductivityTools.MacroActions
{
    public enum MacroTargetContext
    {
        Any,
        GameObjectsOnly,
        AssetsOnly
    }

    [CreateAssetMenu(fileName = "NewMacroAction", menuName = "Tools/GameDevTools/Macro Action", order = 100)]
    public class MacroAction : ScriptableObject
    {
        [Tooltip("The context in which this macro can be executed.")]
        public MacroTargetContext context = MacroTargetContext.Any;

        [Tooltip("If true, the macro will update its target list from the current Selection after each step. Useful for chaining actions like 'Group -> Rename'.")]
        public bool trackSelection = true;

        [Header("Shortcut")]
        public bool useShortcut = true;
        public KeyCode keyCode = KeyCode.None;
        public EventModifiers modifiers = EventModifiers.None;

        [Tooltip("List of steps to execute in sequence.")]
        [SerializeReference]
        public List<MacroStep> steps = new List<MacroStep>();

        public void Execute(Object[] targets)
        {
            MacroExecutor.Execute(this, targets);
        }
    }
}
