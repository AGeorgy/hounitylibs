// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/09/21 13:34

using UnityEditor;
using UnityEngine;

namespace Holoville.HOEditorGUIFramework.Utils
{
    /// <summary>
    /// Editor undo manager.
    /// To use it:
    /// <list type="number">
    /// <item>
    /// <description>Store an instance in the related Editor Class (instantiate it inside the <code>OnEnable</code> method).</description>
    /// </item>
    /// <item>
    /// <description>Call <code>undoManagerInstance.CheckUndo()</code> BEFORE the first UnityGUI call in <code>OnInspectorGUI</code>.</description>
    /// </item>
    /// <item>
    /// <description>Call <code>undoManagerInstance.CheckDirty()</code> AFTER the last UnityGUI call in <code>OnInspectorGUI</code>.</description>
    /// </item>
    /// </list>
    /// </summary>
    public class HOEditorUndoManager
    {

        // VARS ///////////////////////////////////////////////////

        readonly Object _DefTarget;
        readonly string _DefName;
        readonly bool _AutoSetDirty;
        bool _listeningForGuiChanges;
        Object _waitingToRecordPrefab; // If different than NULL indicates the prefab instance that will need to record its state as soon as the mouse is released. 

        // ***********************************************************************************
        // CONSTRUCTOR
        // ***********************************************************************************

        /// <summary>
        /// Creates a new HOEditorUndoManager,
        /// setting it so that the target is marked as dirty each time a new undo is stored. 
        /// </summary>
        /// <param name="target">
        /// The default <see cref="Object"/> you want to save undo info for.
        /// </param>
        /// <param name="name">
        /// The default name of the thing to undo (displayed as "Undo [name]" in the main menu).
        /// </param>
        public HOEditorUndoManager(Object target, string name) : this(target, name, true) { }
        /// <summary>
        /// Creates a new HOEditorUndoManager. 
        /// </summary>
        /// <param name="target">
        /// The default <see cref="Object"/> you want to save undo info for.
        /// </param>
        /// <param name="name">
        /// The default name of the thing to undo (displayed as "Undo [name]" in the main menu).
        /// </param>
        /// <param name="autoSetDirty">
        /// If TRUE, marks the target as dirty each time a new undo is stored.
        /// </param>
        public HOEditorUndoManager(Object target, string name, bool autoSetDirty)
        {
            _DefTarget = target;
            _DefName = name;
            _AutoSetDirty = autoSetDirty;
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>
        /// Call this method BEFORE any undoable UnityGUI call.
        /// Manages undo for the default target, with the default name.
        /// </summary>
        public void CheckUndo() { CheckUndo(_DefTarget, _DefName); }
        /// <summary>
        /// Call this method BEFORE any undoable UnityGUI call.
        /// Manages undo for the given target, with the default name.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        public void CheckUndo(Object target) { CheckUndo(target, _DefName); }
        /// <summary>
        /// Call this method BEFORE any undoable UnityGUI call.
        /// Manages undo for the given target, with the given name.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        /// <param name="name">
        /// The name of the thing to undo (displayed as "Undo [name]" in the main menu).
        /// </param>
        public void CheckUndo(Object target, string name)
        {
            if (_waitingToRecordPrefab != null) {
                // Record eventual prefab instance modification.
                // TODO Avoid recording if nothing changed (no harm in doing so, but it would be nicer).
                switch (Event.current.type) {
                    case EventType.MouseDown:
                    case EventType.MouseUp:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                        PrefabUtility.RecordPrefabInstancePropertyModifications(_waitingToRecordPrefab);
                        break;
                }
            }
            if ((Event.current.type == EventType.MouseDown && Event.current.button == 0) || (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Tab)) {
                // When the LMB is pressed or the TAB key is released,
                // store a snapshot, but don't register it as an undo
                // (so that if nothing changes we avoid storing a useless undo).
                Undo.SetSnapshotTarget(target, name);
                Undo.CreateSnapshot();
                Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
                _listeningForGuiChanges = true;
            }
        }

        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Manages undo for the default target, with the default name,
        /// and returns a value of TRUE if the target is marked as dirty.
        /// </summary>
        public bool CheckDirty() { return CheckDirty(_DefTarget, _DefName); }
        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Manages undo for the given target, with the default name,
        /// and returns a value of TRUE if the target is marked as dirty.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        public bool CheckDirty(Object target) { return CheckDirty(target, _DefName); }
        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Manages undo for the given target, with the given name,
        /// and returns a value of TRUE if the target is marked as dirty.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        /// <param name="name">
        /// The name of the thing to undo (displayed as "Undo [name]" in the main menu).
        /// </param>
        public bool CheckDirty(Object target, string name)
        {
            if (_listeningForGuiChanges && GUI.changed) {
                // Some GUI value changed after pressing the mouse
                // or releasing the TAB key.
                // Register the previous snapshot as a valid undo.
                SetDirty(target, name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Forces undo for the default target, with the default name.
        /// Used to undo operations that are performed by pressing a button,
        /// which doesn't set the GUI to a changed state.
        /// </summary>
        public void ForceDirty() { ForceDirty(_DefTarget, _DefName); }
        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Forces undo for the given target, with the default name.
        /// Used to undo operations that are performed by pressing a button,
        /// which doesn't set the GUI to a changed state.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        public void ForceDirty(Object target) { ForceDirty(target, _DefName); }
        /// <summary>
        /// Call this method AFTER any undoable UnityGUI call.
        /// Forces undo for the given target, with the given name.
        /// Used to undo operations that are performed by pressing a button,
        /// which doesn't set the GUI to a changed state.
        /// </summary>
        /// <param name="target">
        /// The <see cref="Object"/> you want to save undo info for.
        /// </param>
        /// <param name="name">
        /// The name of the thing to undo (displayed as "Undo [name]" in the main menu).
        /// </param>
        public void ForceDirty(Object target, string name)
        {
            if (!_listeningForGuiChanges) {
                // Create a new snapshot.
                Undo.SetSnapshotTarget(target, name);
                Undo.CreateSnapshot();
                Undo.ClearSnapshotTarget();
            }
            SetDirty(target, name, true);
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        void SetDirty(Object target, string name, bool force = false)
        {
            Undo.SetSnapshotTarget(target, name);
            Undo.RegisterSnapshot();
            Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
            if (_AutoSetDirty || force) EditorUtility.SetDirty(target);
            _listeningForGuiChanges = false;

            if (CheckTargetIsPrefabInstance(target)) {
                // Prefab instance: record immediately and also wait for value to be changed and than re-record it
                // (otherwise prefab instances are not updated correctly when using Custom Inspectors).
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                _waitingToRecordPrefab = target;
            } else {
                _waitingToRecordPrefab = null;
            }
        }

        static bool CheckTargetIsPrefabInstance(Object target)
        {
            return (PrefabUtility.GetPrefabType(target) == PrefabType.PrefabInstance);
        }
    }
}