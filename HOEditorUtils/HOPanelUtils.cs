// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/26 12:25

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Utils for editor panels (of type <see cref="Editor"/> or <see cref="EditorWindow"/>).
    /// </summary>
    public class HOPanelUtils
    {
        static Dictionary<EditorWindow, GUIContent> _winTitleContentByEditor;

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Connects to a <see cref="ScriptableObject"/> asset.
        /// If the asset already exists at the given path, loads it and returns it.
        /// Otherwise, either returns NULL or automatically creates it before loading and returning it
        /// (depending on the given parameters).
        /// </summary>
        /// <typeparam name="T">Asset type</typeparam>
        /// <param name="adbFilePath">File path (relative to Unity's project folder)</param>
        /// <param name="createIfMissing">If TRUE and the requested asset doesn't exist, forces its creation</param>
        public static T ConnectToSourceAsset<T>(string adbFilePath, bool createIfMissing = false) where T : ScriptableObject
        {
            if (!HOFileUtils.AssetExists(adbFilePath)) {
                if (createIfMissing) CreateScriptableAsset<T>(adbFilePath);
                else return null;
            }
            T source = (T)Resources.LoadAssetAtPath(adbFilePath, typeof (T));
            if (source == null) {
                // Source changed (or editor file was moved from outside of Unity): overwrite it
                CreateScriptableAsset<T>(adbFilePath);
                source = (T)Resources.LoadAssetAtPath(adbFilePath, typeof(T));
            }
            return source;
        }

        /// <summary>
        /// Sets the icon and title of an editor window.
        /// Call this at the beginning of every OnGUI call.
        /// </summary>
        /// <param name="editor">Reference to the editor panel whose icon to set</param>
        /// <param name="icon">Icon to apply</param>
        /// <param name="title">Title</param>
        public static void SetWindowTitle(EditorWindow editor, Texture icon, string title)
        {
            GUIContent titleContent;
            if (_winTitleContentByEditor == null) _winTitleContentByEditor = new Dictionary<EditorWindow, GUIContent>();
            if (_winTitleContentByEditor.ContainsKey(editor)) {
                titleContent = _winTitleContentByEditor[editor];
                if (titleContent != null) {
                    titleContent.image = icon;
                    return;
                }
                _winTitleContentByEditor.Remove(editor);
            }
            titleContent = GetWinTitleContent(editor);
            if (titleContent != null) {
                titleContent.image = icon;
                _winTitleContentByEditor.Add(editor, titleContent);
            }
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void CreateScriptableAsset<T>(string adbFilePath) where T : ScriptableObject
        {
            T data = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(data, adbFilePath);
        }

        static GUIContent GetWinTitleContent(EditorWindow editor)
        {
            const BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo p = typeof(EditorWindow).GetProperty("cachedTitleContent", bFlags);
            if (p == null) return null;
            return p.GetValue(editor, null) as GUIContent;
        }
    }
}