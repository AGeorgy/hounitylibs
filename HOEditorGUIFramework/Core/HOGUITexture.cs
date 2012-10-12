// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/23 12:19

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework.Core
{
    /// <summary>
    /// Contains various useful textures for GUI usage.
    /// </summary>
    internal static class HOGUITexture
    {
        internal static Texture2D checkboxOn { get { return HOGUIStyle.IsProSkin ? CheckboxOnPro : CheckboxOnFree; } }
        internal static Texture2D checkboxOff { get { return HOGUIStyle.IsProSkin ? CheckboxOffPro : CheckboxOffFree; } }
        internal static Texture2D collapse { get { return HOGUIStyle.IsProSkin ? CollapseIcoPro : CollapseIcoFree; } }
        internal static Texture2D expand { get { return HOGUIStyle.IsProSkin ? ExpandIcoPro : ExpandIcoFree; } }

        internal static readonly Texture2D CheckboxOnFree,
                                  CheckboxOffFree,
                                  CheckboxOnPro,
                                  CheckboxOffPro,
                                  ExpandIcoFree,
                                  CollapseIcoFree,
                                  ExpandIcoPro,
                                  CollapseIcoPro;

        const string _GUITexturesDirectory = "HOEditorGUIFrameworkTextures";

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        static HOGUITexture()
        {
            // Find textures directory
            string assetsDir = Application.dataPath;
            string[] guiTexturesDirs = Directory.GetDirectories(assetsDir, _GUITexturesDirectory, SearchOption.AllDirectories);
            int dirsCount = guiTexturesDirs.Length;
            if (dirsCount == 0) {
                Debug.LogError("HOUnityEditor.EditorGUIFramework > textures directory couldn't be found");
                return;
            }
            if (dirsCount > 1) {
                Debug.LogError("HOUnityeditor.EditorGUIFramework > multiple textures directories found");
                return;
            }
            string guiTexturesDir = guiTexturesDirs[0].Substring(assetsDir.Length - 6);
            guiTexturesDir = guiTexturesDir.Replace("\\", "/");

            // Load textures
            CheckboxOnFree = LoadTexture(guiTexturesDir, "checkboxOn.png");
            CheckboxOffFree = LoadTexture(guiTexturesDir, "checkboxOff.png");
            CheckboxOnPro = LoadTexture(guiTexturesDir, "checkboxOn_pro.png");
            CheckboxOffPro = LoadTexture(guiTexturesDir, "checkboxOff_pro.png");
            ExpandIcoFree = LoadTexture(guiTexturesDir, "expand.png");
            CollapseIcoFree = LoadTexture(guiTexturesDir, "collapse.png");
            ExpandIcoPro = LoadTexture(guiTexturesDir, "expand_pro.png");
            CollapseIcoPro = LoadTexture(guiTexturesDir, "collapse_pro.png");
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static Texture2D LoadTexture(string guiTexturesDirectory, string fileName)
        {
            Texture2D tx = AssetDatabase.LoadAssetAtPath(guiTexturesDirectory + "/" + fileName, typeof(Texture2D)) as Texture2D;
            if (tx == null) Debug.LogError(fileName + " texture missing from " + guiTexturesDirectory);
            else SetTextureImportSettings(tx);
            return tx;
        }

        /// <summary>
        /// Checks that the given textures use the correct import settings,
        /// and applies them if they're incorrect.
        /// </summary>
        static void SetTextureImportSettings(Texture2D texture)
        {
            if (texture.wrapMode == TextureWrapMode.Clamp) return;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            tImporter.textureType = TextureImporterType.GUI;
            tImporter.npotScale = TextureImporterNPOTScale.None;
            tImporter.filterMode = FilterMode.Point;
            tImporter.wrapMode = TextureWrapMode.Clamp;
            tImporter.maxTextureSize = 32;
            tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            AssetDatabase.ImportAsset(path);
        }
    }
}