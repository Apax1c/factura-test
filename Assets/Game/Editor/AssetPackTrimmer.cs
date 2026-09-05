using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Factura.EditorTools
{
    /// <summary>
    /// Reports, and optionally deletes, the parts of the imported UI kit the game never touches.
    /// The dependency set comes from Unity's own graph rather than from reading the YAML, so
    /// nested references such as a font's fallbacks are accounted for.
    /// </summary>
    public static class AssetPackTrimmer
    {
        private const string PACK_ROOT = "Assets/Layer Lab";
        private const string GAME_ROOT = "Assets/Game";
        private const string LOG_PREFIX = "[Factura] ";

        [MenuItem("Factura/Asset Pack/Report Unused", priority = 100)]
        public static void Report()
        {
            List<string> unused = FindUnused(out int keptCount, out long unusedBytes);

            Debug.Log(
                LOG_PREFIX + "asset pack: " + keptCount + " files are referenced by the game, " +
                unused.Count + " are not (" + (unusedBytes / 1024f / 1024f).ToString("F1") + " MB).");

            foreach (string path in unused.Take(15))
                Debug.Log(LOG_PREFIX + "  unused: " + path);

            if (unused.Count > 15)
                Debug.Log(LOG_PREFIX + "  ... and " + (unused.Count - 15) + " more.");
        }

        [MenuItem("Factura/Asset Pack/Delete Unused", priority = 101)]
        public static void DeleteUnused()
        {
            List<string> unused = FindUnused(out int keptCount, out long unusedBytes);
            if (unused.Count == 0)
            {
                Debug.Log(LOG_PREFIX + "nothing to delete.");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Trim the UI kit?",
                "Delete " + unused.Count + " unused files (" +
                (unusedBytes / 1024f / 1024f).ToString("F1") + " MB) from " + PACK_ROOT + "?\n\n" +
                keptCount + " files the game actually references will be kept.\n\n" +
                "This cannot be undone.",
                "Delete",
                "Cancel");

            if (!confirmed)
                return;

            List<string> failed = new();
            AssetDatabase.DeleteAssets(unused.ToArray(), failed);
            AssetDatabase.Refresh();

            Debug.Log(
                LOG_PREFIX + "deleted " + (unused.Count - failed.Count) + " files; " +
                failed.Count + " could not be removed.");

            foreach (string path in failed)
                Debug.LogWarning(LOG_PREFIX + "  kept (locked or in use): " + path);
        }

        /// <summary>
        /// Everything reachable from the game's own assets is kept; whatever is left inside the
        /// pack is dead weight. Asking Unity for the dependencies rather than scanning files by
        /// hand means indirect references — a prefab's sprites, a font's fallbacks — are included.
        /// </summary>
        private static List<string> FindUnused(out int keptCount, out long unusedBytes)
        {
            string[] gameAssets = CollectFiles(GAME_ROOT);
            HashSet<string> needed = new(AssetDatabase.GetDependencies(gameAssets, true));

            List<string> unused = new();
            keptCount = 0;
            unusedBytes = 0L;

            foreach (string path in CollectFiles(PACK_ROOT))
            {
                if (needed.Contains(path))
                {
                    keptCount++;
                    continue;
                }

                unused.Add(path);

                FileInfo info = new(path);
                if (info.Exists)
                    unusedBytes += info.Length;
            }

            return unused;
        }

        private static string[] CollectFiles(string root) =>
            AssetDatabase.FindAssets(string.Empty, new[] { root })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrEmpty(path) && !AssetDatabase.IsValidFolder(path))
                .Distinct()
                .ToArray();
    }
}
