using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityProductivityTools.TabScreenshot
{
    public static class TabScreenshotMaker
    {
        private static MethodInfo _readScreenPixelMethod;

        static TabScreenshotMaker()
        {
            _readScreenPixelMethod = typeof(InternalEditorUtility).GetMethod("ReadScreenPixel", 
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public static void CaptureWindow(EditorWindow window, Action<Texture2D> onComplete)
        {
            if (window == null)
            {
                onComplete?.Invoke(null);
                return;
            }

            // Ensure window is in front and focused
            window.Focus();
            
            // Force repaint of everything to ensure no artifacts from moved windows
            InternalEditorUtility.RepaintAllViews();

            // Wait for a robust amount of time (e.g. 0.25 seconds) to ensure OS window manager works
            double startTime = EditorApplication.timeSinceStartup;
            double waitPayload = 0.25; 
            
            EditorApplication.CallbackFunction callback = null;
            callback = () =>
            {
                if (EditorApplication.timeSinceStartup - startTime >= waitPayload)
                {
                    EditorApplication.update -= callback;
                    Texture2D tex = DoCapture(window);
                    onComplete?.Invoke(tex);
                }
            };
            
            EditorApplication.update += callback;
        }

        private static Texture2D DoCapture(EditorWindow window)
        {
            Rect rect = window.position;
            int width = (int)rect.width;
            int height = (int)rect.height;

            // Use reflection call to ReadScreenPixel
            Vector2 screenPos = new Vector2(rect.x, rect.y);
            Color[] pixels = (Color[])_readScreenPixelMethod.Invoke(null, new object[] { screenPos, width, height });

            if (pixels == null || pixels.Length == 0)
            {
                Debug.LogError("Failed to capture pixels from window: " + window.titleContent.text);
                return null;
            }

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        public static string SaveScreenshot(Texture2D tex, string folderPath, string fileName)
        {
            if (tex == null) return null;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fullPath = Path.Combine(folderPath, fileName + ".png");
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(fullPath, bytes);

            AssetDatabase.Refresh();
            return fullPath;
        }
    }
}
