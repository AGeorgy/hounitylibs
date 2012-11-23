// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/22 14:18
namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Interface to add to the tk2dTextMesh class in order to use the HO2DToolkit library.
    /// </summary>
    public interface IHOtk2dTextMesh
    {
        /// <summary>
        /// Text
        /// </summary>
        string text { get; set; }

        /// <summary>
        /// Commits the changes to the text
        /// </summary>
        void Commit();
    }
}