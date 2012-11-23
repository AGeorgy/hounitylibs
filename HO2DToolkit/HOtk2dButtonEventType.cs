// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 21:32
namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Enums for an <see cref="HOtk2dButtonEvent"/> type.
    /// </summary>
    public enum HOtk2dButtonEventType
    {
        /// <summary>
        /// A toggle button is selected
        /// </summary>
        Select,
        /// <summary>
        /// A toggle buttons is deselected
        /// </summary>
        Deselect,
        /// <summary>
        /// A toggle button is toggled (selected or deselected)
        /// </summary>
        Toggle,
        /// <summary>
        /// The mouse was pressed over the button
        /// </summary>
        Press,
        /// <summary>
        /// The mouse was released inside or outside the button,
        /// after the button was pressed
        /// </summary>
        Release,
        /// <summary>
        /// The mouse was released inside the same button that was initially pressed
        /// </summary>
        Click,
        /// <summary>
        /// The mouse rolled over a button which has a rollover animation
        /// </summary>
        RollOver,
        /// <summary>
        /// The mouse rolled out of a button which has a rollover animation
        /// </summary>
        RollOut
    }
}