// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/26 12:25

using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Utils for editor panels (of type <see cref="Editor"/> or <see cref="EditorWindow"/>).
    /// </summary>
    public class HOPanelUtils
    {
        /// <summary>
        /// Connects to a <see cref="ScriptableObject"/> asset.
        /// If the asset already exists at the given path, loads it and returns it.
        /// Otherwise, automatically creates it before loading and returning it.
        /// </summary>
        /// <typeparam name="T">Asset type</typeparam>
        /// <param name="adbFilePath">File path (relative to Unity's project folder)</param>
        public static T ConnectToSourceAsset<T>(string adbFilePath) where T : ScriptableObject
        {
            if (!HOFileUtils.AssetExists(adbFilePath)) CreateScriptableAsset<T>(adbFilePath);
            T source = (T)Resources.LoadAssetAtPath(adbFilePath, typeof (T));
            if (source == null) {
                // Source changed (or editor file was moved from outside of Unity): overwrite it
                CreateScriptableAsset<T>(adbFilePath);
                source = (T)Resources.LoadAssetAtPath(adbFilePath, typeof(T));
            }
            return source;
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void CreateScriptableAsset<T>(string adbFilePath) where T : ScriptableObject
        {
            T data = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(data, adbFilePath);
        }
    }
}