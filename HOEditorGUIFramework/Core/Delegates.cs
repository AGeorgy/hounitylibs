// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/23 17:48

using System;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework.Core
{
    internal delegate void GUILayoutAction(
        GUIStyle guiStyle, Color backgroundShade, Action blockAction, GUILayoutOption[] options
    );
}