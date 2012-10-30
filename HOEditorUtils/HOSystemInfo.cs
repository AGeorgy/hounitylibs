// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/30 12:27

using Holoville.HOEditorUtils.Enums;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// System infos.
    /// </summary>
    public static class HOSystemInfo
    {
        /// <summary>
        /// Operating system where Unity Editor is currently running.
        /// </summary>
        public static OS editorOS { get; private set; }

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        static HOSystemInfo()
        {
            editorOS = GetEditorOS();
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static OS GetEditorOS()
        {
            string os = SystemInfo.operatingSystem;
            if (os.IndexOf("Windows") != -1) return OS.Windows;
            if (os.IndexOf("Linux") != -1) return OS.Linux;
            return OS.Mac;
        }
    }
}