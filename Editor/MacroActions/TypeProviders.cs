using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityProductivityTools.MacroActions
{
    public static class MenuPathProvider
    {
        private static string[] cachedPaths;

        public static string[] GetMenuPaths()
        {
            if (cachedPaths != null) return cachedPaths;

            var paths = new HashSet<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var methods = assembly.GetTypes()
                        .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        .Where(m => m.GetCustomAttributes(typeof(MenuItem), false).Length > 0);

                    foreach (var method in methods)
                    {
                        var attr = (MenuItem)method.GetCustomAttribute(typeof(MenuItem));
                        if (attr != null && !string.IsNullOrEmpty(attr.menuItem))
                        {
                            paths.Add(attr.menuItem);
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that fail to reflect
                }
            }

            cachedPaths = paths.OrderBy(p => p).ToArray();
            return cachedPaths;
        }
    }

    public static class ComponentTypeProvider
    {
        private static string[] cachedTypes;

        public static string[] GetEditorGUIComponentNames()
        {
            if (cachedTypes != null) return cachedTypes;

            var types = new List<string>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var componentTypes = assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(UnityEngine.Component)) && !t.IsAbstract);

                    foreach (var type in componentTypes)
                    {
                        types.Add(type.FullName ?? type.Name);
                    }
                }
                catch { }
            }

            cachedTypes = types.OrderBy(t => t).ToArray();
            return cachedTypes;
        }
    }
}
