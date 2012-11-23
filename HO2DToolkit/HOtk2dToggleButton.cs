// Author: Daniele Giardini
// Copyright (c) 2012 Daniele Giardini - Holoville - http://www.holoville.com
// Created: 2012/11/15 22:32

using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HO2DToolkit
{
    /// <summary>
    /// Toggle button
    /// </summary>
    public class HOtk2dToggleButton : HOtk2dButton
    {
        public event HOTk2dButtonDelegate Toggle;
        public event HOTk2dButtonDelegate Untoggle;

        [SerializeField] string _id = "";

        static readonly Dictionary<string, List<HOtk2dButton>> _ButtonsById = new Dictionary<string, List<HOtk2dButton>>();

        // ===================================================================================
        // UNITY METHODS ---------------------------------------------------------------------

        protected override void OnEnable()
        {
            base.OnEnable();

            // Add to toggle group
            if (_id == "") return;
            if (_ButtonsById.ContainsKey(_id)) _ButtonsById.Add(_id, new List<HOtk2dButton>());
            _ButtonsById[_id].Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Remove from toggle group
            if (_id == "") return;
            List<HOtk2dButton> bts = _ButtonsById[_id];
            bts.RemoveAt(bts.IndexOf(this));
            if (bts.Count <= 0) _ButtonsById.Remove(_id);
        }
    }
}