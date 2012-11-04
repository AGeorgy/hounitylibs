// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/02 20:56

using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Various texture utils.
    /// </summary>
    public static class HOTextureUtils
    {
        /// <summary>
        /// Various import settings to be used with the SetImportSettings method.
        /// </summary>
        public enum TextureImportSettings
        {
            /// <summary>
            /// Sets this texture for use within a GUI texture.
            /// </summary>
            GUITexture,
            /// <summary>
            /// Sets texture for use with a custom EditorWindow title icon.
            /// </summary>
            WindowIcon
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Sets the import settings of the given texture to some default values,
        /// only if some of the settings doesn't already coincide.
        /// </summary>
        /// <param name="texture">Texture to set</param>
        /// <param name="importSettings">Import settings</param>
        public static void SetImportSettings(Texture texture, TextureImportSettings importSettings)
        { SetImportSettings(AssetDatabase.GetAssetPath(texture), importSettings); }

        /// <summary>
        /// Sets the import settings of the given texture to some default values,
        /// only if some of the settings doesn't already coincide.
        /// </summary>
        /// <param name="textureADBPath">AssetDatabase path to the texture to set</param>
        /// <param name="importSettings">Import settings</param>
        public static void SetImportSettings(string textureADBPath, TextureImportSettings importSettings)
        {
            bool settingsChanged = false;
            TextureImporter tImporter = AssetImporter.GetAtPath(textureADBPath) as TextureImporter;
            switch (importSettings) {
                case TextureImportSettings.GUITexture:
                case TextureImportSettings.WindowIcon:
                    if (tImporter.textureType != TextureImporterType.GUI) {
                        tImporter.textureType = TextureImporterType.GUI;
                        settingsChanged = true;
                    }
                    if (tImporter.npotScale != TextureImporterNPOTScale.None) {
                        tImporter.npotScale = TextureImporterNPOTScale.None;
                        settingsChanged = true;
                    }
                    if (tImporter.filterMode != FilterMode.Point) {
                        tImporter.filterMode = FilterMode.Point;
                        settingsChanged = true;
                    }
                    if (tImporter.wrapMode != TextureWrapMode.Clamp) {
                        tImporter.wrapMode = TextureWrapMode.Clamp;
                        settingsChanged = true;
                    }
                    if (tImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor) {
                        tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                        settingsChanged = true;
                    }
                    if (importSettings == TextureImportSettings.WindowIcon) {
                        if (tImporter.maxTextureSize != 32) {
                            tImporter.maxTextureSize = 32;
                            settingsChanged = true;
                        }
                    }
                    break;
            }
            if (settingsChanged) AssetDatabase.ImportAsset(textureADBPath);
        }
    }
}