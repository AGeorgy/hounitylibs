// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/29 19:26

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Creates and manages an external file used to create a menu item that will open an editor window.
    /// Its main usage is to customize an editor window shortcut and dockability.
    /// </summary>
    public static class HOMenuItemFileCreator
    {
        const string _ReplaceUsingStr = "XXXUSING";
        const string _ReplaceFileClassStr = "XXXFILECLASS";
        const string _ReplaceClassStr = "XXXCLASS";
        const string _ReplaceNameStr = "XXXNAME";
        const string _ReplaceDockableStr = "XXXDOCKABLE";
        const string _ReplaceShortcutStr = "XXXSHORTCUT";
        const string _ClassString = _ReplaceUsingStr + "using UnityEditor;\npublic class " + _ReplaceFileClassStr + "\n{"
                                    + "\n   [MenuItem (\"Window/" + _ReplaceNameStr + _ReplaceShortcutStr + "\")]"
                                    + "\n   static void ShowWindow() {"
                                    + "\n       EditorWindow.GetWindow(typeof(" + _ReplaceClassStr + "), " + _ReplaceDockableStr + ", \"" + _ReplaceNameStr + "\");"
                                    + "\n   }\n}";

        // ===================================================================================
        // METHODS --------------------------------------------------------------------

        /// <summary>
        /// Generates a file which can be used to create a menu item (under the Window menu) 
        /// that will open an editor window of the given type.
        /// </summary>
        /// <typeparam name="T">Type of editor window to target</typeparam>
        /// <param name="directoryPath">Full path to the directory where the file will be created (without final slash)</param>
        /// <param name="name">Name of the menu item</param>
        /// <param name="isDockableWindow">If TRUE the window will be opened as dockable</param>
        public static void CreateMenuItem<T>(string directoryPath, string name, bool isDockableWindow)
        { CreateMenuItem<T>(directoryPath, name, isDockableWindow, false, false, false, ""); }

        /// <summary>
        /// Generates a file which can be used to create a menu item (under the Window menu) 
        /// that will open an editor window of the given type, accessed via the set shortcut.
        /// </summary>
        /// <typeparam name="T">Type of editor window to target</typeparam>
        /// <param name="directoryPath">Full path to the directory where the file will be created (without final slash)</param>
        /// <param name="name">Name of the menu item</param>
        /// <param name="isDockableWindow">If TRUE the window will be opened as dockable</param>
        /// <param name="useCtrl">If TRUE CTRL/CMD key will be used when creating the shortcut</param>
        /// <param name="useShift">If TRUE SHIFT key will be used when creating the shortcut</param>
        /// <param name="letter">Letter to assign to the shortcut</param>
        public static void CreateMenuItem<T>(string directoryPath, string name, bool isDockableWindow, bool useCtrl, bool useShift, string letter)
        { CreateMenuItem<T>(directoryPath, name, isDockableWindow, true, useCtrl, useShift, letter); }

        static void CreateMenuItem<T>(string directoryPath, string name, bool isDockableWindow, bool useShortcut, bool useCtrl, bool useShift, string letter)
        {
            directoryPath = directoryPath.Replace(HOFileUtils.pathSlashToReplace, HOFileUtils.pathSlash);
            Type type = typeof (T);
            string nameSpace = type.Namespace;
            string className = type.Name;
            string fileClassName = className + "MenuItem";
            string filePath = directoryPath + HOFileUtils.pathSlash + fileClassName + ".cs";

            string dockable = (isDockableWindow ? "false" : "true");
            string shortcut = "";
            if (useShortcut) {
                shortcut += " ";
                if (useCtrl) shortcut += "%";
                if (useShift) shortcut += "#";
                shortcut += letter;
            }

            string fileContent = _ClassString.Replace(_ReplaceUsingStr, nameSpace == null ? "" : "using " + nameSpace + ";\n");
            fileContent = fileContent.Replace(_ReplaceFileClassStr, fileClassName);
            fileContent = fileContent.Replace(_ReplaceClassStr, className);
            fileContent = fileContent.Replace(_ReplaceNameStr, name);
            fileContent = fileContent.Replace(_ReplaceDockableStr, dockable);
            fileContent = fileContent.Replace(_ReplaceShortcutStr, shortcut);
            File.WriteAllText(filePath, fileContent);
            AssetDatabase.ImportAsset(HOFileUtils.FullPathToADBPath(filePath), ImportAssetOptions.TryFastReimportFromMetaData);
        }
    }
}