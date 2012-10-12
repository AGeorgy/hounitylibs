// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/24 14:51

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Editor file utils.
    /// </summary>
    public static class HOFileUtils
    {
        /// <summary>
        /// Full path to project directory, with backwards (\) slashes.
        /// </summary>
        public static string projectPath
        {
            get
            {
                if (__projectPath == null) {
                    __projectPath = Application.dataPath;
                    __projectPath = __projectPath.Substring(0, __projectPath.LastIndexOf("/"));
                    __projectPath = __projectPath.Replace("/", "\\");
                }
                return __projectPath;
            }
        }
        /// <summary>
        /// Full path to project's Assets directory, with backwards (\) slashes.
        /// </summary>
        public static string assetsPath { get { return projectPath + "\\Assets"; } }

        static string __projectPath;

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Converts the given full path to a path usable with AssetDatabase methods
        /// (relative to Unity's project folder, and with the correct Unity forward (/) slashes).
        /// </summary>
        public static string FullPathToADBPath(string fullPath)
        {
            string adbPath = fullPath.Substring(projectPath.Length + 1);
            return adbPath.Replace("\\", "/");
        }

        /// <summary>
        /// Converts the given project-relative path to a full path,
        /// with backward (\) slashes).
        /// </summary>
        public static string ADBPathToFullPath(string adbPath)
        {
            adbPath = adbPath.Replace("/", "\\");
            return projectPath + "\\" + adbPath;
        }

        /// <summary>
        /// Returns the asset path of the given GUID (relative to Unity project's folder),
        /// or an empty string if either the GUID is invalid or the related path doesn't exist.
        /// </summary>
        public static string GUIDToExistingAssetPath(string guid)
        {
            if (guid == "") return "";
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath == "") return "";
            if (AssetExists(assetPath)) return assetPath;
            return "";
        }

        /// <summary>
        /// Returns TRUE if the file/directory at the given path exists.
        /// </summary>
        /// <param name="adbPath">Path, relative to Unity's project folder</param>
        /// <returns></returns>
        public static bool AssetExists(string adbPath)
        {
            string fullPath = ADBPathToFullPath(adbPath);
            return File.Exists(fullPath) || Directory.Exists(fullPath);
        }
    }
}