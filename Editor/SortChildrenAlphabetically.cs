using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class SortChildrenAlphabetically
{
    [MenuItem("Tools/GameDevTools/Sort Children Alphabetically &%S", false, 150)]
    [MenuItem("GameObject/Sort Children Alphabetically", false, 0)]
    private static void SortChildren(MenuCommand command)
    {
        GameObject parent = GetTargetObject(command);
        if (parent == null || parent.transform.childCount < 2)
        {
            if (parent != null && parent.transform.childCount < 2)
                Debug.LogWarning("[Sort Children] Target object must have at least 2 children to sort.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(parent.transform, "Sort Children Alphabetically");

        var children = parent.transform
            .Cast<Transform>()
            .OrderBy(t => t.name, new NaturalStringComparer())
            .ToList();

        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }

        EditorUtility.SetDirty(parent);
    }

    [MenuItem("Tools/GameDevTools/Sort Children Alphabetically &%S", true)]
    [MenuItem("GameObject/Sort Children Alphabetically", true)]
    private static bool ValidateSortChildren(MenuCommand command)
    {
        GameObject parent = GetTargetObject(command);
        return parent != null && parent.transform.childCount > 1;
    }

    private static GameObject GetTargetObject(MenuCommand command)
    {
        if (command.context != null)
            return command.context as GameObject;
        
        return Selection.activeGameObject;
    }

    private class NaturalStringComparer : IComparer<string>
    {
        [System.Runtime.InteropServices.DllImport("shlwapi.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}
