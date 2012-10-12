// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/10 10:59

using System.Collections.Generic;
using UnityEngine;

namespace Holoville.DebugFramework
{
    /// <summary>
    /// Static methods to draw gizmos automatically every frame.
    /// </summary>
    public class HORuntimeGizmos : MonoBehaviour
    {
        enum GizmoType
        {
            Point,
            TargetBounds,
            Line
        }

        enum TargetType
        {
            Object,
            Collider,
            Mesh,
            Renderer
        }

        /// <summary>
        /// if <see cref="randomPointRadius"/> is set to TRUE,
        /// this is just a reference size used to create points of random sizes
        /// (useful to avoid overlapping points being hidden).
        /// </summary>
        public static float defPointRadius = 0.1f;
        /// <summary>
        /// If TRUE, points will have a random radius.
        /// </summary>
        public static bool randomPointRadius = true;
        /// <summary>
        /// If TRUE and no color is passed to methods, uses a random color.
        /// </summary>
        public static bool randomDefColor = true;
        /// <summary>
        /// Default color.
        /// </summary>
        public static Color defColor = Color.magenta;

        static bool _isInstantiated;
        static List<HOGizmoData> _gizmosData = new List<HOGizmoData>();


        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        void Awake()
        {
            _isInstantiated = true;
        }

        void OnDrawGizmos()
        {
            for (int i = _gizmosData.Count - 1; i > -1; --i) {
                HOGizmoData gd = _gizmosData[i];
                Gizmos.color = gd.color;
                switch (gd.type) {
                    case GizmoType.Point:
                        Gizmos.DrawWireSphere(gd.position, gd.radius);
                        break;
                    case GizmoType.TargetBounds:
                        Bounds bounds = new Bounds();
                        bool boundsSet = true;
                        switch (gd.targetType) {
                            case TargetType.Collider:
                                bounds = ((Collider)gd.target).bounds;
                                break;
                            case TargetType.Mesh:
                                bounds = ((Mesh)gd.target).bounds;
                                break;
                            case TargetType.Renderer:
                                bounds = ((Renderer)gd.target).bounds;
                                break;
                            default:
                                boundsSet = false;
                                break;
                        }
                        if (boundsSet) {
                            Gizmos.DrawWireCube(bounds.center, bounds.size);
                        }
                        break;
                    case GizmoType.Line:
                        Gizmos.DrawLine(gd.position, gd.endPosition);
                        break;
                }
                if (gd.drawOnlyOnce) _gizmosData.RemoveAt(i);
            }
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        /// <summary>Adds a point to be constantly shown as a gizmo.</summary>
        public static void AddPoint(Vector3 position, bool drawOnlyOnce = false) { AddPoint(position, randomDefColor ? GetRandomColor() : defColor, defPointRadius, drawOnlyOnce); }
        /// <summary>Adds a point to be constantly shown as a gizmo.</summary>
        public static void AddPoint(Vector3 position, Color color, bool drawOnlyOnce = false) { AddPoint(position, color, defPointRadius, drawOnlyOnce); }
        /// <summary>Adds a point to be constantly shown as a gizmo.</summary>
        public static void AddPoint(Vector3 position, float radius, bool drawOnlyOnce = false) { AddPoint(position, randomDefColor ? GetRandomColor() : defColor, radius, drawOnlyOnce); }
        /// <summary>Adds a point to be constantly shown as a gizmo.</summary>
        public static void AddPoint(Vector3 position, Color color, float radius, bool drawOnlyOnce = false)
        {
            CheckInit();
            HOGizmoData gd = new HOGizmoData {
                type = GizmoType.Point,
                position = position,
                color = color,
                radius = radius,
                drawOnlyOnce = drawOnlyOnce
            };
            _gizmosData.Add(gd);
        }

        /// <summary>Adds a target to be constantly evidenced.</summary>
        public static void AddTarget(UnityEngine.Object target, bool drawOnlyOnce = false) { AddTarget(target, randomDefColor ? GetRandomColor() : defColor, drawOnlyOnce); }
        /// <summary>Adds a target to be constantly evidenced.</summary>
        public static void AddTarget(UnityEngine.Object target, Color color, bool drawOnlyOnce = false)
        {
            CheckInit();
            HOGizmoData gd = new HOGizmoData {
                type = GizmoType.TargetBounds,
                target = target,
                color = color,
                drawOnlyOnce = drawOnlyOnce
            };
            _gizmosData.Add(gd);
        }

        /// <summary>Adds a line to be constantly drawn as a gizmo.</summary>
        public static void AddLine(Vector3 origin, Vector3 direction, float length, bool drawOnlyOnce = false) { AddLine(origin, direction, length, randomDefColor ? GetRandomColor() : defColor, drawOnlyOnce); }
        /// <summary>Adds a line to be constantly drawn as a gizmo.</summary>
        public static void AddLine(Vector3 origin, Vector3 direction, float length, Color color, bool drawOnlyOnce = false)
        {
            Ray ray = new Ray(origin, direction);
            Vector3 endPosition = ray.GetPoint(length);
            AddLine(origin, endPosition, color, drawOnlyOnce);
        }
        /// <summary>Adds a line to be constantly drawn as a gizmo.</summary>
        public static void AddLine(Vector3 from, Vector3 to, bool drawOnlyOnce = false) { AddLine(from, to, randomDefColor ? GetRandomColor() : defColor, drawOnlyOnce); }
        /// <summary>Adds a line to be constantly drawn as a gizmo.</summary>
        public static void AddLine(Vector3 from, Vector3 to, Color color, bool drawOnlyOnce = false)
        {
            CheckInit();
            HOGizmoData gd = new HOGizmoData {
                type = GizmoType.Line,
                position = from,
                endPosition = to,
                color = color,
                drawOnlyOnce = drawOnlyOnce
            };
            _gizmosData.Add(gd);
        }

        /// <summary>
        /// Clears all gizmos managed by <see cref="HORuntimeGizmos"/>.
        /// </summary>
        public static void Clear()
        {
            _gizmosData = new List<HOGizmoData>();
        }

        // ===================================================================================
        // METHODS ---------------------------------------------------------------------------

        static void CheckInit()
        {
            if (_isInstantiated) return;
            new GameObject("HORuntimeGizmos").AddComponent<HORuntimeGizmos>();
        }

        static Color GetRandomColor()
        {
            return new Color(
                UnityEngine.Random.value,
                UnityEngine.Random.value,
                UnityEngine.Random.value,
                1
            );
        }


        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // ||| INTERNAL CLASSES ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private class HOGizmoData
        {
            public GizmoType type;
            public UnityEngine.Object target; // Used in case of targets
            public Vector3 position;
            public Vector3 endPosition; // Used in case of lines
            public Color color;
            public float _radius; // Used in case of points
            public bool drawOnlyOnce; // If TRUE draws this gizmo only once

            bool _targetTypeSet;
            TargetType _targetType;
            public TargetType targetType
            {
                get
                {
                    if (!_targetTypeSet) {
                        if (target != null) {
                            if (target is Collider)
                                _targetType = TargetType.Collider;
                            else if (target is Mesh)
                                _targetType = TargetType.Mesh;
                            else if (target is Renderer)
                                _targetType = TargetType.Renderer;
                            else {
                                _targetType = TargetType.Object;
                            }
                        }
                        _targetTypeSet = true;
                    }
                    return _targetType;
                }
            }

            public float radius
            {
                get { return _radius; }
                set { _radius = HORuntimeGizmos.randomPointRadius ? GetRandomRadiusFrom(value) : value; }
            }

            // ===================================================================================
            // METHODS ---------------------------------------------------------------------------

            float GetRandomRadiusFrom(float p_radius)
            {
                float diff = p_radius * 0.25f;
                float maxRadius = p_radius + diff;
                float minRadius = p_radius - diff;
                return UnityEngine.Random.Range(minRadius, maxRadius);
            }
        }
    }
}