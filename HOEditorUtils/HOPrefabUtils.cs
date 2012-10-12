// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/26 13:39

using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOEditorUtils
{
    /// <summary>
    /// Various prefab utility methods.
    /// </summary>
    public static class HOPrefabUtils
    {
        /// <summary>
        /// Completely removes any prefab connection from the given prefab instances.
        /// Based on RodGreen's method (http://forum.unity3d.com/threads/82883-Breaking-connection-from-gameObject-to-prefab-for-good.?p=726602&amp;viewfull=1#post726602)
        /// </summary>
        static public void BreakPrefabInstances(List<GameObject> prefabInstances)
        { foreach (GameObject instance in prefabInstances) BreakPrefabInstance(instance); }
        /// <summary>
        /// Completely removes any prefab connection from the given prefab instance.
        /// Based on RodGreen's method (http://forum.unity3d.com/threads/82883-Breaking-connection-from-gameObject-to-prefab-for-good.?p=726602&amp;viewfull=1#post726602)
        /// </summary>
        static public void BreakPrefabInstance(GameObject prefabInstance)
        {
            string name = prefabInstance.name;
            Transform transform = prefabInstance.transform;
            Transform parent = transform.parent;
            // Unparent the GO so that world transforms are preserved.
            transform.parent = null;
            // Clone and re-assign.
            GameObject newInstance = (GameObject)Object.Instantiate(prefabInstance);
            newInstance.name = name;
            newInstance.active = prefabInstance.active;
            newInstance.transform.parent = parent;
            // Remove old.
            Object.DestroyImmediate(prefabInstance, false);
        }
    }
}