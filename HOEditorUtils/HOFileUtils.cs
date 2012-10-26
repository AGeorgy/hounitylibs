// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/24 14:51

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

        /// <summary>
        /// Returns the path (in AssetDatabase format: "/" slashes and no final slash)
        /// to the assembly that contains the given target.
        /// </summary>
        /// <param name="target">Target whose assembly path to return</param>
        /// <returns></returns>
        public static string GetAssemblyADBPath(object target)
        {
//            UriBuilder uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            UriBuilder uri = new UriBuilder(target.GetType().Assembly.CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string adbAssemblyPath = Path.GetDirectoryName(path);
            adbAssemblyPath = adbAssemblyPath.Replace("\\", "/");
            adbAssemblyPath = adbAssemblyPath.Substring(projectPath.Length + 1);
            return adbAssemblyPath.Substring(0, adbAssemblyPath.LastIndexOf("/"));
        }

        /// <summary>
        /// Returns a new asset path based on the given fileName and current selection,
        /// so that it can be used to create new assets in the project window.
        /// </summary>
        /// <param name="fileName">New name to apply, extension included 
        /// (the correct int will be added if that name already exist)</param>
        static public string GetNewAssetPathFromSelection(string fileName)
        {
            Object selectedObj = Selection.activeObject;
            string selectionADBPath = AssetDatabase.GetAssetPath(selectedObj);
            string assetDirectoryPath;
            if (selectionADBPath.Length > 0) {
                assetDirectoryPath = ADBPathToFullPath(selectionADBPath);
                if ((File.GetAttributes(assetDirectoryPath) & FileAttributes.Directory) != FileAttributes.Directory) {
                    assetDirectoryPath = assetDirectoryPath.Substring(0, assetDirectoryPath.LastIndexOf("\\"));
                }
            } else {
                assetDirectoryPath = assetsPath;
            }
            // Set correct filename
            int dotInd = fileName.IndexOf(".");
            if (dotInd == -1) {
                Debug.LogError("HOFileUtils.GetNewAssetPathFromSelection: fileName parameter is missing extension");
                return null;
            }
            string name = fileName.Substring(0, dotInd);
            string extension = fileName.Substring(dotInd);
            string assetPath = assetDirectoryPath + "\\" + fileName;
            int num = 0;
            while (File.Exists(assetPath)) {
                assetPath = assetDirectoryPath + "\\" + name + " " + (++num) + extension;
            }

            return FullPathToADBPath(assetPath);
        }

        /// <summary>
        /// Renames the given asset with the new name,
        /// adding an eventual int to the name in case the new name already exists
        /// </summary>
        /// <param name="assetADBPath"></param>
        /// <param name="fileName">File name extension included</param>
        /// <returns></returns>
        static public void RenameAsset(string assetADBPath, string fileName)
        {
            string assetDirectoryPath = ADBPathToFullPath(assetADBPath);
            assetDirectoryPath = assetDirectoryPath.Substring(0, assetDirectoryPath.LastIndexOf("\\"));
            int dotInd = fileName.IndexOf(".");
            if (dotInd == -1) {
                Debug.LogError("HOFileUtils.RenameAsset: fileName parameter is missing extension");
                return;
            }
            string name = fileName.Substring(0, dotInd);
            string extension = fileName.Substring(dotInd);
            string newAssetPath = assetDirectoryPath + "\\" + fileName;
            int num = 0;
            while (File.Exists(newAssetPath)) {
                fileName = name + " " + (++num) + extension;
                newAssetPath = assetDirectoryPath + "\\" + fileName;
            }
            string res = AssetDatabase.RenameAsset(assetADBPath, fileName);
            if (res != "") Debug.LogError(res);
        }
    }
}