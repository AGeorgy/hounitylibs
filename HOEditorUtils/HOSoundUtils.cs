// Author: Daniele Giardini
// Copyright (c) 2013 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2013/06/12 20:30

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Various editor sound utils
    /// </summary>
    public static class HOSoundUtils
    {
        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Plays the given clip in the Editor
        /// </summary>
        public static void PlayClip(AudioClip audioClip)
        {
            if (audioClip == null) return;

            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType()
                .GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder, new[] { typeof(AudioClip) }, null);
            mInfo.Invoke(null, new object[] { audioClip });
        }

        /// <summary>
        /// Stops playing the given clip.
        /// </summary>
        public static void StopClip(AudioClip audioClip)
        {
            if (audioClip == null) return;

            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType()
                .GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder, new[] { typeof(AudioClip) }, null);
            mInfo.Invoke(null, new object[] { audioClip });
        }

        /// <summary>
        /// Stops all clips playing.
        /// </summary>
        public static void StopAllClips()
        {
            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType().GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
            mInfo.Invoke(null, null);
        }
    }
}