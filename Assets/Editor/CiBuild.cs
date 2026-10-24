using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AsteraX.Editor
{
    public static class CiBuild
    {
        public static void BuildWebGL()
        {
            try
            {
                var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled)
                    .Select(scene => scene.path).ToArray();
                if (scenes.Length == 0)
                    throw new InvalidOperationException("Build Settings has no enabled scenes.");

                var arguments = Environment.GetCommandLineArgs();
                var pathIndex = Array.IndexOf(arguments, "-customBuildPath");
                if (pathIndex < 0 || pathIndex + 1 >= arguments.Length ||
                    string.IsNullOrWhiteSpace(arguments[pathIndex + 1]) ||
                    arguments[pathIndex + 1].StartsWith("-"))
                    throw new InvalidOperationException("CI requires -customBuildPath followed by an output path.");

                if (AssetDatabase.LoadAssetAtPath<TMP_Settings>(
                        "Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
                    throw new InvalidOperationException(
                        "Import TMP Essential Resources with -importPackage before invoking the CI build.");

                // Include a JavaScript decompressor so the preview works on static HTTP hosts.
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
                PlayerSettings.WebGL.decompressionFallback = true;

                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
                {
                    scenes = scenes,
                    locationPathName = arguments[pathIndex + 1],
                    target = BuildTarget.WebGL,
                    options = BuildOptions.None
                });
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException(
                        $"WebGL build failed: {report.summary.result}, {report.summary.totalErrors} errors.");

                Debug.Log("CI WebGL build succeeded.");
                EditorApplication.Exit(0);
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                EditorApplication.Exit(1);
            }
        }
    }
}
