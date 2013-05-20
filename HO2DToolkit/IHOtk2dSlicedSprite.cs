// Author: Daniele Giardini
// Copyright (c) 2013 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2013/01/05 21:16

using UnityEngine;

namespace Holoville.HO2DToolkit
{
    public interface IHOtk2dSlicedSprite : IHOtk2dSprite
    {
        /// <summary>
        /// Slice dimensions
        /// </summary>
        Vector2 dimensions { get; set; }
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
        /// Sets the sprite to the one with the given name, or returns FALSE if none corresponds
        /// </summary>
        bool SetSprite(string name);
        /// <summary>
        /// Gets the local space bounds of the sprite.
        /// </summary>
        /// <returns></returns>
        Bounds GetBounds();

        // MODS ADDED TO CODE

        /// <summary>
        /// Returns the name of the sprite in the collection (add this method to tk2dBaseSprite)
        /// </summary>
        string GetSpriteName();
    }
}