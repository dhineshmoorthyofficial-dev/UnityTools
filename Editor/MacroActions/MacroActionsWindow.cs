using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.MacroActions
{
    public class MacroActionsWindow : EditorWindow
    {
        [SerializeField] private MacroActionsUI ui = new MacroActionsUI();

        [MenuItem("Tools/GameDevTools/Macro Actions", false, 100)]
        public static void ShowWindow()
        {
            GetWindow<MacroActionsWindow>("Macro Actions");
        }

        private void OnEnable()
        {
            ui.Initialize(this);
        }

        private void OnFocus()
        {
            ui.RefreshMacroList();
        }

        private void OnGUI()
        {
            ui.Draw();
        }
    }
}
