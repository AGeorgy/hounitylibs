// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 21:32

namespace Holoville.HO2DToolkit
{
    public class HOtk2dButtonEvent
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public HOtk2dButtonEventType type { get; private set; }
        /// <summary>
        /// Button that dispatched this event
        /// </summary>
        public HOtk2dButton target { get; private set; }


        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        public HOtk2dButtonEvent(HOtk2dButtonEventType type, HOtk2dButton target)
        {
            this.target = target;
            this.type = type;
        } 
    }
}