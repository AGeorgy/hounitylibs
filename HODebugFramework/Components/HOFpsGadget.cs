// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/05 12:51

using UnityEngine;

namespace Holoville.DebugFramework.Components
{
    /// <summary>
    /// Shows framerate and eventual memory info during runtime.
    /// Can also force a specific framerate.
    /// </summary>
    [AddComponentMenu("Holoville/HOFpsGadget")]
    public class HOFpsGadget : MonoBehaviour
    {
        /// <summary>
        /// If TRUE also shows memory info.
        /// </summary>
        public bool showMemory;
        /// <summary>
        /// Delay between each update of the fps calculation.
        /// </summary>
        public float updateDelay = 0.5f;
        /// <summary>
        /// Alignment of the info gadget.
        /// </summary>
        public TextAlignment alignment = TextAlignment.Right;
        /// <summary>
        /// If different than 0 forces the given framerate.
        /// Set it to 0 if you don't want HOFpsGadget to do anything with Application.targetFrameRate.
        /// </summary>
        public int limitFrameRate = -1;

        float _accum = 0;
        int _frames = 0;
        float _timeleft;
        string _fps = "";
        string _memory = "";
        bool _stylesSet;
        GUIStyle _fpsStyle;


        // ===================================================================================
        // MONOBEHAVIOUR METHODS -------------------------------------------------------------

        void Awake()
        {
            _timeleft = updateDelay;
        }

        void Start()
        {
            if (limitFrameRate != 0) Application.targetFrameRate = limitFrameRate;
        }

        void Update()
        {
            // Calculate FPS to show
            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            ++_frames;
            if (_timeleft <= 0) {
                _fps = (_accum / _frames).ToString("f2");
                _timeleft = updateDelay;
                _accum = 0;
                _frames = 0;
                if (showMemory) _memory = string.Format("{0:#,0}", System.GC.GetTotalMemory(false));
            }
        }

        void OnGUI()
        {
            if (!_stylesSet) {
                _stylesSet = true;
                _fpsStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, normal = { textColor = Color.white } };
            }

            int boxW = (showMemory ? 190 : 100);
            int boxWHalf = (int)(boxW * 0.5f);
            string msg = "FPS: " + _fps + (showMemory ? " / " + _memory : "");

            switch (alignment) {
                case TextAlignment.Left:
                    GUI.Label(new Rect(4, 4, boxW, 22), msg, _fpsStyle);
                    break;
                case TextAlignment.Center:
                    GUI.Label(new Rect(Screen.width * 0.5f - boxWHalf, 4, boxW, 22), msg, _fpsStyle);
                    break;
                default:
                    GUI.Label(new Rect(Screen.width - 104, 4, boxW, 22), msg, _fpsStyle);
                    break;
            }
        }
    }
}