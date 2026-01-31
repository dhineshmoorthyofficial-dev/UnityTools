using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityProductivityTools.TabScreenshot
{
    public class TabScreenshotWindow : EditorWindow
    {
        // Static state to persist across Close/Open cycles
        private static Texture2D s_lastCapture;
        private static string s_lastCapturePath;
        private static Vector2 s_scrollPos;
        
        // EditorPrefs Keys
        private const string PREF_SAVE_FOLDER = "TabScreenshotTool_SaveFolder";
        private const string PREF_PREFIX = "TabScreenshotTool_Prefix";
        private const string PREF_TIMESTAMP = "TabScreenshotTool_Timestamp";
        private const string PREF_HIDE = "TabScreenshotTool_Hide";

        [MenuItem("Tools/GameDevTools/Tab Screenshot Maker", false, 250)]
        public static void ShowWindow()
        {
            var window = GetWindow<TabScreenshotWindow>("Tab Screenshot");
            window.minSize = new Vector2(350, 500);
        }

        // [MenuItem("Tools/GameDevTools/Capture Focused Tab %&s")]
        // public static void CaptureFocusedTab()
        // {
        //     EditorWindow focused = EditorWindow.focusedWindow;
        //     if (focused != null)
        //     {
        //         // Create an instance if it doesn't exist to get settings, or use defaults
        //         TabScreenshotWindow win = GetWindow<TabScreenshotWindow>("Tab Screenshot", false);
        //         win.CaptureAndSave(focused);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("No focused EditorWindow to capture.");
        //     }
        // }

        private string _saveFolder = "Assets/Screenshots";
        private string _fileNamePrefix = "Capture";
        private bool _includeTimestamp = true;
        private bool _hideWindow = true;
        
        private List<EditorWindow> _activeWindows = new List<EditorWindow>();
        private Vector2 _scrollPos;
        
        // Local Instance State (populated from static on Enable if available)
        private Texture2D _lastCapture;
        private string _lastCapturePath;

        private void OnEnable()
        {
            LoadPrefs();
            
            // Restore session state
            if (s_lastCapture != null) _lastCapture = s_lastCapture;
            if (!string.IsNullOrEmpty(s_lastCapturePath)) _lastCapturePath = s_lastCapturePath;
            _scrollPos = s_scrollPos;

            RefreshWindowList();
        }

        private void OnDisable()
        {
            SavePrefs();
            s_scrollPos = _scrollPos;
        }

        private void LoadPrefs()
        {
            _saveFolder = EditorPrefs.GetString(PREF_SAVE_FOLDER, "Assets/Screenshots");
            _fileNamePrefix = EditorPrefs.GetString(PREF_PREFIX, "Capture");
            _includeTimestamp = EditorPrefs.GetBool(PREF_TIMESTAMP, true);
            _hideWindow = EditorPrefs.GetBool(PREF_HIDE, true);
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(PREF_SAVE_FOLDER, _saveFolder);
            EditorPrefs.SetString(PREF_PREFIX, _fileNamePrefix);
            EditorPrefs.SetBool(PREF_TIMESTAMP, _includeTimestamp);
            EditorPrefs.SetBool(PREF_HIDE, _hideWindow);
        }

        private void RefreshWindowList()
        {
            _activeWindows = Resources.FindObjectsOfTypeAll<EditorWindow>()
                .Where(w => w != null && w != this)
                .OrderBy(w => w.titleContent.text)
                .ToList();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string newFolder = EditorGUILayout.TextField("Save Folder", _saveFolder);
            if (EditorGUI.EndChangeCheck()) _saveFolder = newFolder;
            
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Screenshot Folder", _saveFolder, "");
                if (!string.IsNullOrEmpty(path))
                {
                    // Try to make it relative to project
                    if (path.StartsWith(Application.dataPath))
                    {
                        path = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                    _saveFolder = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            _fileNamePrefix = EditorGUILayout.TextField("File Name Prefix", _fileNamePrefix);
            _includeTimestamp = EditorGUILayout.Toggle("Include Timestamp", _includeTimestamp);
            _hideWindow = EditorGUILayout.Toggle(new GUIContent("Hide Tool Window", "Temporarily closes and re-opens the tool window to ensure it is not visible in the screenshot. Note: This may undock the window if it was docked."), _hideWindow);
            if (EditorGUI.EndChangeCheck()) SavePrefs(); // Save immediately on toggle change
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Active Windows", EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                RefreshWindowList();
            }
            EditorGUILayout.EndHorizontal();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (var win in _activeWindows)
            {
                if (win == null) continue;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                // Icon if available
                if (win.titleContent.image != null)
                {
                    GUILayout.Label(win.titleContent.image, GUILayout.Width(20), GUILayout.Height(20));
                }
                
                GUILayout.Label(win.titleContent.text, GUILayout.ExpandWidth(true));
                
                if (GUILayout.Button("Capture", GUILayout.Width(70)))
                {
                    CaptureAndSave(win);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (_lastCapture != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Last Capture Preview", EditorStyles.boldLabel);
                
                float aspect = (float)_lastCapture.width / _lastCapture.height;
                float previewWidth = position.width - 30;
                float previewHeight = previewWidth / aspect;
                
                if (previewHeight > 200)
                {
                    previewHeight = 200;
                    previewWidth = previewHeight * aspect;
                }

                Rect previewRect = GUILayoutUtility.GetRect(previewWidth, previewHeight);
                GUI.DrawTexture(previewRect, _lastCapture, ScaleMode.ScaleToFit);
                
                if (GUILayout.Button("Open File"))
                {
                    EditorUtility.RevealInFinder(_lastCapturePath);
                }
                EditorGUILayout.EndVertical();
            }
        }

        public void CaptureAndSave(EditorWindow window)
        {
            if (window == null) return;

            // Save state before potential close
            SavePrefs();
            s_scrollPos = _scrollPos;

            if (_hideWindow)
            {
                Close();
            }

            TabScreenshotMaker.CaptureWindow(window, (tex) =>
            {
                try
                {
                    if (tex != null)
                    {
                        string timestamp = _includeTimestamp ? "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") : "";
                        string winName = window.titleContent.text.Replace(" ", "_");
                        // Remove invalid path chars
                        winName = string.Join("_", winName.Split(Path.GetInvalidFileNameChars()));

                        string fileName = $"{_fileNamePrefix}_{winName}{timestamp}";

                        string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), _saveFolder);
                        string savedPath = TabScreenshotMaker.SaveScreenshot(tex, absolutePath, fileName);
                        
                        Debug.Log($"Screenshot saved to: {savedPath}");

                        // Update static state for the reborn window
                        s_lastCapture = tex;
                        s_lastCapturePath = savedPath;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    if (_hideWindow)
                    {
                        // Re-open and show success
                        TabScreenshotWindow win = GetWindow<TabScreenshotWindow>("Tab Screenshot");
                        if (tex != null)
                        {
                            win.ShowNotification(new GUIContent("Saved!"));
                            win.Repaint();
                        }
                    }
                    else
                    {
                        // We are still alive
                        if (tex != null)
                        {
                            _lastCapture = tex;
                            _lastCapturePath = s_lastCapturePath; // Logic above set s_lastCapturePath
                            ShowNotification(new GUIContent("Saved!"));
                            Repaint();
                        }
                    }
                }
            });
        }
    }
}
