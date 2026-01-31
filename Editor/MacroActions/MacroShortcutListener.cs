using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.MacroActions
{
    [InitializeOnLoad]
    public static class MacroShortcutListener
    {
        static MacroShortcutListener()
        {
            // Use reflection to hook into globalEventHandler since it is internal
            const string fieldName = "globalEventHandler";
            var field = typeof(EditorApplication).GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);

            if (field != null)
            {
                EditorApplication.CallbackFunction currentDelegate = (EditorApplication.CallbackFunction)field.GetValue(null);
                currentDelegate -= OnGlobalGuiEvent;
                currentDelegate += OnGlobalGuiEvent;
                field.SetValue(null, currentDelegate);
                // Debug.Log("MacroShortcutListener: Hooked into EditorApplication.globalEventHandler");
            }
            else
            {
                SceneView.duringSceneGui -= OnGlobalGuiEvent;
                SceneView.duringSceneGui += OnGlobalGuiEvent;
                // Debug.Log("MacroShortcutListener: Fallback to SceneView.duringSceneGui");
            }
        }

        private static void OnGlobalGuiEvent(SceneView scene) => OnGlobalGuiEvent();

        private static void OnGlobalGuiEvent()
        {
            Event e = Event.current;
            if (e == null || !e.isKey || e.type != EventType.KeyDown || e.keyCode == KeyCode.None)
                return;

            if (EditorGUIUtility.editingTextField)
                return;

            var macros = MacroManager.GetAllMacros();
            foreach (var macro in macros)
            {
                if (macro == null || !macro.useShortcut || macro.keyCode == KeyCode.None)
                    continue;

                if (e.keyCode == macro.keyCode && MatchModifiers(e, macro.modifiers))
                {
                    Debug.Log($"[Macro] Triggered Shortcut: {e.modifiers} + {e.keyCode} for macro '{macro.name}'");
                    macro.Execute(Selection.objects);
                    e.Use();
                    return;
                }
            }
        }


        private static bool MatchModifiers(Event e, EventModifiers target)
        {
            bool targetShift = (target & EventModifiers.Shift) != 0;
            bool targetCtrl = (target & EventModifiers.Control) != 0;
            bool targetAlt = (target & EventModifiers.Alt) != 0;
            bool targetCmd = (target & EventModifiers.Command) != 0;

            // On Windows, Control is usually what users want for "Ctrl"
            // e.control handles the platform-specific Ctrl key
            return e.shift == targetShift && 
                   e.control == targetCtrl && 
                   e.alt == targetAlt && 
                   e.command == targetCmd;
        }
    }
}
