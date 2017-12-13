// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/12/13 15:57
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core.DebugSystem
{
    internal class NodeProcessDebug
    {
        readonly Data _panningData = new Data();
        Data _currData;

        #region Public Methods

        public void Draw(Rect processArea)
        {
            NodeProcessDebugGUI.Draw(this, processArea);
        }

        public void OnNodeProcessStart(InteractionManager.State interactionState)
        {
            if (Event.current.type != EventType.Layout) return;
            switch (interactionState) {
            case InteractionManager.State.Panning:
                _currData = _panningData;
                break;
            }
            if (_currData != null) _currData.OnGUIStart();
        }

        public void OnNodeProcessEnd()
        {
            if (Event.current.type != EventType.Repaint) return;
            if (_currData == null) return;
            _currData.OnGUIEnd();
            _currData = null;
        }

        public float GetPanningAvrgFps()
        {
            return _panningData.avrgFps;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Data
        {
            public float avrgFps { get; private set; }

            const int _MaxToStore = 30;
            readonly List<float> _elapsedTimes = new List<float>(_MaxToStore);
            readonly List<float> _avrgOverTime = new List<float>(_MaxToStore);
            float _guiStartTime;

            public void OnGUIStart()
            {
                _guiStartTime = Time.realtimeSinceStartup;
            }

            public void OnGUIEnd()
            {
                float elapsed = Time.realtimeSinceStartup - _guiStartTime;
                if (_elapsedTimes.Count > _MaxToStore) _elapsedTimes.RemoveAt(0);
                _elapsedTimes.Add(elapsed);
                StoreAverageFps();
            }

            void StoreAverageFps()
            {
                int len = _elapsedTimes.Count;
                if (len == 0) {
                    avrgFps = 0;
                    return;
                }
                float tot = 0;
                for (int i = 0; i < len; ++i) {
                    tot += _elapsedTimes[i];
                }
                avrgFps = 1 / (tot / len);
                if (_avrgOverTime.Count > _MaxToStore) _avrgOverTime.RemoveAt(0);
                _avrgOverTime.Add(avrgFps);
            }
        }
    }
}