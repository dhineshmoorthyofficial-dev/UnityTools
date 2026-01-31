using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityProductivityTools.MacroActions
{
    [CustomPropertyDrawer(typeof(MacroStep), true)]
    public class MacroStepDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get the managed reference value directly
            object step = property.managedReferenceValue;
            if (step == null)
            {
                EditorGUI.LabelField(position, "Null Step Reference");
                EditorGUI.EndProperty();
                return;
            }

            Type type = step.GetType();

            // Layout constants
            float line = EditorGUIUtility.singleLineHeight;
            float enabledWidth = 20;
            float spacing = 5;

            Rect enabledRect = new Rect(position.x, position.y, enabledWidth, line);
            SerializedProperty enabledProp = property.FindPropertyRelative("enabled");
            if (enabledProp != null)
            {
                enabledProp.boolValue = EditorGUI.Toggle(enabledRect, enabledProp.boolValue);
            }

            Rect contentRect = new Rect(position.x + enabledWidth + spacing, position.y, position.width - enabledWidth - spacing, line);

            if (type == typeof(MenuCommandStep))
            {
                DrawMenuCommandStep(contentRect, property);
            }
            else if (type == typeof(AddComponentStep))
            {
                DrawAddComponentStep(contentRect, property);
            }
            else if (type == typeof(RenameStep))
            {
                DrawRenameStep(contentRect, property);
            }
            else if (type == typeof(SetActiveStep))
            {
                DrawSetActiveStep(contentRect, property);
            }
            else
            {
                EditorGUI.LabelField(contentRect, $"Step: {type.Name}");
            }

            EditorGUI.EndProperty();
        }

        private void DrawSetActiveStep(Rect rect, SerializedProperty property)
        {
            SerializedProperty activeProp = property.FindPropertyRelative("active");
            if (activeProp != null)
            {
                activeProp.boolValue = EditorGUI.ToggleLeft(rect, "Set Active", activeProp.boolValue);
            }
        }

        private void DrawMenuCommandStep(Rect rect, SerializedProperty property)
        {
            SerializedProperty pathProp = property.FindPropertyRelative("commandPath");
            if (pathProp != null)
                DrawSearchableField(rect, "Command:", pathProp, MenuPathProvider.GetMenuPaths(), "Select Menu Command");
        }

        private void DrawAddComponentStep(Rect rect, SerializedProperty property)
        {
            SerializedProperty typeProp = property.FindPropertyRelative("componentType");
            if (typeProp != null)
                DrawSearchableField(rect, "Component:", typeProp, ComponentTypeProvider.GetEditorGUIComponentNames(), "Select Component");
        }

        private void DrawRenameStep(Rect rect, SerializedProperty property)
        {
            SerializedProperty nameProp = property.FindPropertyRelative("newName");
            if (nameProp != null)
            {
                nameProp.stringValue = EditorGUI.TextField(rect, "Rename To:", nameProp.stringValue);
            }
        }


        private void DrawSearchableField(Rect rect, string label, SerializedProperty prop, string[] items, string title)
        {
            float labelWidth = 65;
            float btnWidth = 20;
            
            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect fieldRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth - btnWidth - 2, rect.height);
            Rect btnRect = new Rect(rect.x + rect.width - btnWidth, rect.y, btnWidth, rect.height);

            EditorGUI.LabelField(labelRect, label);
            prop.stringValue = EditorGUI.TextField(fieldRect, prop.stringValue);

            if (GUI.Button(btnRect, "▼", EditorStyles.miniButtonRight))
            {
                var provider = ScriptableObject.CreateInstance<MenuPickerProvider>();
                provider.Initialize(title, items, (selected) => {
                    prop.serializedObject.Update();
                    prop.stringValue = selected;
                    prop.serializedObject.ApplyModifiedProperties();
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
        }
    }

    public class MenuPickerProvider : ScriptableObject, ISearchWindowProvider
    {
        private string title;
        private string[] items;
        private Action<string> onSelected;

        public void Initialize(string title, string[] items, Action<string> onSelected)
        {
            this.title = title;
            this.items = items;
            this.onSelected = onSelected;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent(title)) };
            var groups = new HashSet<string>();

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item)) continue;
                
                string[] parts = item.Split('/');
                if (parts.Length > 1)
                {
                    string groupPath = "";
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        if (i > 0) groupPath += "/";
                        groupPath += parts[i];
                        if (!groups.Contains(groupPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(parts[i]), i + 1));
                            groups.Add(groupPath);
                        }
                    }
                    tree.Add(new SearchTreeEntry(new GUIContent(parts[^1])) { level = parts.Length, userData = item });
                }
                else
                {
                    tree.Add(new SearchTreeEntry(new GUIContent(item)) { level = 1, userData = item });
                }
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            onSelected?.Invoke((string)entry.userData);
            return true;
        }
    }
}
