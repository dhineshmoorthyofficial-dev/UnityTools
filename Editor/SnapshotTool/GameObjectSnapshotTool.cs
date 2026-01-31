using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.SnapshotTool
{
    public class GameObjectSnapshotTool : EditorWindow
    {
        [SerializeField] private SnapshotUI ui = new SnapshotUI();

        [MenuItem("Tools/GameDevTools/Snapshot Manager", false, 230)]
        public static void ShowWindow()
        {
            GetWindow<GameObjectSnapshotTool>("Snapshot Tool");
        }

        private void OnEnable()
        {
            ui.Initialize();
        }

        private void OnGUI()
        {
            ui.Draw();
        }
    }
}
