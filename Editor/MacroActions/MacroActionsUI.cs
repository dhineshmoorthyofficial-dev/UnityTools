using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityProductivityTools.MacroActions
{
    [Serializable]
    public class MacroActionsUI
    {
        private List<MacroAction> macros;
        private MacroAction selectedMacro;
        private Vector2 leftScrollPos;
        private Vector2 rightScrollPos;
        private string searchString = "";
        private string newMacroName = "New Macro";
        private bool isCreatingNew = false;

        private ReorderableList stepList;
        private EditorWindow hostWindow;
        private SerializedObject serializedMacro;

        public void Initialize(EditorWindow window)
        {
            hostWindow = window;
            RefreshMacroList();
        }

        public void RefreshMacroList()
        {
            macros = MacroManager.GetAllMacros();
        }

        public void Draw()
        {
            // Toolbar
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Macros", EditorStyles.boldLabel, GUILayout.Width(70));
            searchString = GUILayout.TextField(searchString, EditorStyles.toolbarSearchField, GUILayout.Width(200));
            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+ New Macro", EditorStyles.toolbarButton))
            {
                isCreatingNew = true;
                selectedMacro = null;
                newMacroName = "New Macro";
            }
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                RefreshMacroList();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            // Left Panel
            DrawMacroList();

            // Divider
            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));

            // Right Panel
            DrawMacroDetails();

            GUILayout.EndHorizontal();
        }

        private void DrawMacroList()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            leftScrollPos = GUILayout.BeginScrollView(leftScrollPos);

            if (isCreatingNew)
            {
                DrawCreateNewInterface();
            }

            if (macros != null)
            {
                foreach (var macro in macros)
                {
                    if (macro == null) continue;
                    if (!string.IsNullOrEmpty(searchString) && !macro.name.ToLower().Contains(searchString.ToLower()))
                        continue;

                    GUIStyle style = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
                    if (selectedMacro == macro)
                    {
                        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.9f);
                    }

                    if (GUILayout.Button(macro.name, style, GUILayout.Height(30)))
                    {
                        SelectMacro(macro);
                    }
                    GUI.backgroundColor = Color.white;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawCreateNewInterface()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Create New Macro", EditorStyles.boldLabel);
            newMacroName = EditorGUILayout.TextField("Name", newMacroName);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel")) isCreatingNew = false;
            if (GUILayout.Button("Create", GUILayout.Width(60)))
            {
                if (!string.IsNullOrEmpty(newMacroName))
                {
                    var macro = MacroManager.CreateMacro(newMacroName);
                    RefreshMacroList();
                    SelectMacro(macro);
                    isCreatingNew = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void SelectMacro(MacroAction macro)
        {
            selectedMacro = macro;
            isCreatingNew = false;
            if (macro != null)
            {
                serializedMacro = new SerializedObject(macro);
                InitializeStepList();
            }
            else
            {
                serializedMacro = null;
                stepList = null;
            }
            GUI.FocusControl(null);
        }

        private void InitializeStepList()
        {
            SerializedProperty stepsProp = serializedMacro.FindProperty("steps");
            stepList = new ReorderableList(serializedMacro, stepsProp, true, true, true, true);

            stepList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Execution Steps");
            stepList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = stepsProp.GetArrayElementAtIndex(index);
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, element, true);
            };
            stepList.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight + 6;
            };
            stepList.onAddDropdownCallback = (rect, list) =>
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Component"), false, () => AddStep(typeof(AddComponentStep)));
                menu.AddItem(new GUIContent("Rename"), false, () => AddStep(typeof(RenameStep)));
                menu.AddItem(new GUIContent("Menu Command"), false, () => AddStep(typeof(MenuCommandStep)));
                menu.AddItem(new GUIContent("Set Active"), false, () => AddStep(typeof(SetActiveStep)));
                menu.ShowAsContext();
            };
        }

        private void AddStep(Type type)
        {
            serializedMacro.Update();
            SerializedProperty stepsProp = serializedMacro.FindProperty("steps");
            int index = stepsProp.arraySize;
            stepsProp.InsertArrayElementAtIndex(index);
            var element = stepsProp.GetArrayElementAtIndex(index);
            element.managedReferenceValue = Activator.CreateInstance(type);
            serializedMacro.ApplyModifiedProperties();
        }

        private void DrawMacroDetails()
        {
            GUILayout.BeginVertical();
            rightScrollPos = GUILayout.BeginScrollView(rightScrollPos);

            if (selectedMacro != null && serializedMacro != null)
            {
                serializedMacro.Update();

                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(selectedMacro.name, EditorStyles.boldLabel);
                if (GUILayout.Button("Execute", GUILayout.Width(100), GUILayout.Height(25)))
                {
                    serializedMacro.ApplyModifiedProperties();
                    selectedMacro.Execute(Selection.objects);
                    serializedMacro.Update(); // CRITICAL: Refresh UI data after macro modifies asset
                }
                GUILayout.EndHorizontal();

                EditorGUILayout.ObjectField("Macro Asset", selectedMacro, typeof(MacroAction), false);
                
                SerializedProperty contextProp = serializedMacro.FindProperty("context");
                EditorGUILayout.PropertyField(contextProp);
                
                SerializedProperty trackSelectionProp = serializedMacro.FindProperty("trackSelection");
                EditorGUILayout.PropertyField(trackSelectionProp);

                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                
                SerializedProperty useShortcutProp = serializedMacro.FindProperty("useShortcut");
                useShortcutProp.boolValue = EditorGUILayout.ToggleLeft("Shortcut", useShortcutProp.boolValue, GUILayout.Width(EditorGUIUtility.labelWidth - 2));

                EditorGUI.BeginDisabledGroup(!useShortcutProp.boolValue);
                
                SerializedProperty modifiersProp = serializedMacro.FindProperty("modifiers");
                EventModifiers currentModifiers = (EventModifiers)modifiersProp.intValue;

                EditorGUI.BeginChangeCheck();
                bool ctrl = (currentModifiers & EventModifiers.Control) != 0;
                bool shift = (currentModifiers & EventModifiers.Shift) != 0;
                bool alt = (currentModifiers & EventModifiers.Alt) != 0;

                // Compact buttons
                ctrl = GUILayout.Toggle(ctrl, "Ctrl", "Button", GUILayout.Width(35));
                shift = GUILayout.Toggle(shift, "Shift", "Button", GUILayout.Width(45));
                alt = GUILayout.Toggle(alt, "Alt", "Button", GUILayout.Width(35));

                if (EditorGUI.EndChangeCheck())
                {
                    EventModifiers m = EventModifiers.None;
                    if (ctrl) m |= EventModifiers.Control;
                    if (shift) m |= EventModifiers.Shift;
                    if (alt) m |= EventModifiers.Alt;
                    modifiersProp.intValue = (int)m;
                }

                EditorGUILayout.LabelField("+", GUILayout.Width(12));

                SerializedProperty keyCodeProp = serializedMacro.FindProperty("keyCode");
                EditorGUILayout.PropertyField(keyCodeProp, GUIContent.none, GUILayout.ExpandWidth(true));
                
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                if (stepList != null)
                {
                    stepList.DoLayoutList();
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete Macro", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("Delete Macro", $"Delete {selectedMacro.name}?", "Yes", "No"))
                    {
                        MacroManager.DeleteMacro(selectedMacro);
                        SelectMacro(null);
                        RefreshMacroList();
                    }
                }

                serializedMacro.ApplyModifiedProperties();
            }
            else
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Select or create a macro to start", EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
