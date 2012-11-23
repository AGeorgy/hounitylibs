// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/22 13:59

using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Interface to add to the tk2dSprite class in order to use the HO2DToolkit library.
    /// </summary>
    public interface IHOtk2dSprite
    {
        /// <summary>
        /// Sprite id
        /// </summary>
        int spriteId { get; set; }
        /// <summary>
        /// Color
        /// </summary>
        Color color { get; set; }
        /// <summary>
        /// Sprite scale
        /// </summary>
        Vector3 scale { get; set; }
    }
}