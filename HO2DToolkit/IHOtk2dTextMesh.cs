// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/22 14:18

using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Interface to add to the tk2dTextMesh class in order to use the HO2DToolkit library.
    /// Also, add this code as the first line inside tk2dTextMesh class:
    /// public void SetMaxChars(int val) { _maxChars = val; }
    /// </summary>
    public interface IHOtk2dTextMesh : IHOtk2dBase
    {
        /// <summary>
        /// Text
        /// </summary>
        string text { get; set; }
        /// <summary>
        /// Color
        /// </summary>
        Color color { get; set; }
        /// <summary>
        /// Scale
        /// </summary>
        Vector3 scale { get; set; }
        /// <summary>
        /// GameObject
        /// </summary>
        GameObject gameObject { get; }
        /// <summary>
        /// Transform
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// Commits the changes to the text
        /// </summary>
        void Commit();
        /// <summary>
        /// Returns the mesh dimension for the given string
        /// </summary>
        Vector2 GetMeshDimensionsForString(string str);
        /// <summary>
        /// Max characters. Call Commit to commit changes. NOTE: This will free and allocate memory, avoid using at runtime
        /// </summary>
        void SetMaxChars(int val);

        /// <summary>
        /// Returns the number of characters excluding texture gradient escape codes.
        /// </summary>
        int NumTotalCharacters();
    }
}