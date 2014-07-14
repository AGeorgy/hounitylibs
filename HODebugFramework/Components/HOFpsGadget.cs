// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/10/05 12:51

using System.Text;
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
        int _totFps;
        string _avrgFps;
        float _time = 0;
        float _timeleft;
        string _fps = "";
        string _memory = "";
        readonly StringBuilder _msg = new StringBuilder(40);
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
            // Calculate average
            if (Time.deltaTime > 0) {
                _time += Time.timeScale / Time.deltaTime;
                _totFps++;
                _avrgFps = (_time / _totFps).ToString("f2");
                // Message
                _msg.Remove(0, _msg.Length);
                _msg.Append("FPS: ").Append(_fps).Append(" / ").Append(_avrgFps);
                if (showMemory) _msg.Append("\nMEM: ").Append(_memory);
            }
        }

        void OnGUI()
        {
            if (!_stylesSet) {
                _stylesSet = true;
                _fpsStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, normal = { textColor = Color.white } };
            }

            const int boxW = 150;
            int boxH = (showMemory ? 36 : 23);
            int boxWHalf = (int)(boxW * 0.5f);

            switch (alignment) {
            case TextAlignment.Left:
                GUI.Label(new Rect(4, 4, boxW, boxH), _msg.ToString(), _fpsStyle);
                break;
            case TextAlignment.Center:
                GUI.Label(new Rect(Screen.width * 0.5f - boxWHalf, 4, boxW, boxH), _msg.ToString(), _fpsStyle);
                break;
            default:
                GUI.Label(new Rect(Screen.width - boxW - 4, 4, boxW, boxH), _msg.ToString(), _fpsStyle);
                break;
            }
        }

        // ===================================================================================
        // PUBLIC METHODS --------------------------------------------------------------------

        public void ResetFps()
        {
            _time = _timeleft = _accum = 0;
            _totFps = 0;
        }
    }
}