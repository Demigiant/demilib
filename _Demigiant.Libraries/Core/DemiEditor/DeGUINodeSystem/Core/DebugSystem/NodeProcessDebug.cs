// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/12/13 15:57
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core.DebugSystem
{
    internal class NodeProcessDebug
    {
        public readonly DataStore panningData = new DataStore();
        DataStore _currDataStore;

        #region Public Methods

        public void Draw(Rect processArea)
        {
            NodeProcessDebugGUI.Draw(this, processArea);
        }

        public void OnNodeProcessStart(InteractionManager.State interactionState)
        {
            switch (interactionState) {
            case InteractionManager.State.Panning:
                _currDataStore = panningData;
                break;
            }
            if (_currDataStore != null) _currDataStore.OnGUIStart();
        }

        public void OnNodeProcessEnd()
        {
            if (_currDataStore == null) return;
            _currDataStore.OnGUIEnd();
            _currDataStore = null;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        internal class DataStore
        {
            public float avrgFps_Layout { get { return _eventData[0].avrgFps; } }
            public float avrgFps_Repaint { get { return _eventData[1].avrgFps; } }
            public float avrgFps_LayoutAndRepaint { get { return _eventData[2].avrgFps; } }
            public float avrgDrawTime_Layout { get { return _eventData[0].avrgMs; } }
            public float avrgDrawTime_Repaint { get { return _eventData[1].avrgMs; } }
            public float avrgDrawTime_LayoutAndRepaint { get { return _eventData[2].avrgMs; } }

            /// <summary>Layout, Repaint, LayoutAndRepaint</summary>
            readonly Data[] _eventData;

            public DataStore()
            {
                _eventData = new[] {new Data(), new Data(), new Data()};
            }

            public void OnGUIStart()
            {
                float time = Time.realtimeSinceStartup;
                switch (Event.current.type) {
                case EventType.Layout:
                    _eventData[0].OnGUIStart(time);
                    _eventData[2].OnGUIStart(time);
                    break;
                case EventType.Repaint:
                    _eventData[1].OnGUIStart(time);
                    break;
                }
            }

            public void OnGUIEnd()
            {
                float time = Time.realtimeSinceStartup;
                switch (Event.current.type) {
                case EventType.Layout:
                    _eventData[0].OnGUIEnd(time);
                    break;
                case EventType.Repaint:
                    _eventData[1].OnGUIEnd(time);
                    _eventData[2].OnGUIEnd(time);
                    break;
                }
            }
        }

        class Data
        {
            public float avrgFps { get; private set; }
            public float avrgMs { get; private set; }

            const int _MaxToStore = 30;
            readonly List<float> _elapsedTimes = new List<float>(_MaxToStore);
            readonly List<float> _avrgFpsOverTime = new List<float>(_MaxToStore);
            readonly List<float> _avrgMsOverTime = new List<float>(_MaxToStore);
            float _guiStartTime;

            public void OnGUIStart(float time)
            {
                _guiStartTime = time;
            }

            public void OnGUIEnd(float time)
            {
                float elapsed = time - _guiStartTime;
                if (_elapsedTimes.Count > _MaxToStore) _elapsedTimes.RemoveAt(0);
                _elapsedTimes.Add(elapsed);
                StoreAverageFps();
            }

            void StoreAverageFps()
            {
                int len = _elapsedTimes.Count;
                if (len == 0) {
                    avrgFps = avrgMs = 0;
                    return;
                }
                float tot = 0;
                for (int i = 0; i < len; ++i) {
                    tot += _elapsedTimes[i];
                }
                avrgFps = 1 / (tot / len);
                if (_avrgFpsOverTime.Count > _MaxToStore) _avrgFpsOverTime.RemoveAt(0);
                _avrgFpsOverTime.Add(avrgFps);
                avrgMs = (tot / len) * 1000;
                if (_avrgMsOverTime.Count > _MaxToStore) _avrgMsOverTime.RemoveAt(0);
                _avrgMsOverTime.Add(avrgMs);
            }
        }
    }
}