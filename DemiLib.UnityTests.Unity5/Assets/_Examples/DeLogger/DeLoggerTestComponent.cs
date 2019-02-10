// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/02/10 10:23
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.Debugging;
using DG.DeInspektor.Attributes;
using UnityEngine;

namespace _Examples.DeLogger
{
    public class DeLoggerTestComponent : MonoBehaviour
    {
        #region Serialized

        [DeToggleButton]
        [SerializeField] bool _stripHtmlTags = false;

        #endregion

        readonly List<int> _intList = new List<int>() {6, 8, 10};
        readonly Vector2[] _vectorsList = new[] {new Vector2(4, 5), new Vector2(8,9)};
        readonly SampleObject[] _sampleObjArray = new [] {
            new SampleObject("Luna", 678), // properties are: id, code
            new SampleObject("Mars", 5489), // properties are: id, code
            new SampleObject("Phobos", 21), // properties are: id, code
        };

        void Examples()
        {
            Debug.Log(_intList.ToLogString());
            // ▼ Logs ▼
            // 6 - 8 - 10
            Debug.Log("_vectorsList: " + _vectorsList.ToLogString());
            // ▼ Logs ▼
            // _vectorsList: (4.0, 5.0) - (8.0, 9.0)
            Debug.Log("Oxygen tanks values:" + _intList.ToLogString(true));
            // ▼ Logs ▼
            // Oxygen tanks values:
            // 0: 6
            // 1: 8
            // 2: 10
            Debug.Log("Something something IDs:" + _sampleObjArray.ToLogString(x => x.id, true));
            // ▼ Logs ▼
            // Something something IDs:
            // 0: Luna
            // 1: Mars
            // 2: Phobos
        }

        #region Test Methods

        [DeMethodButton("Log Lists/Arrays", 0, false)]
        [DeMethodButton("Log Lists/Arrays w lineBreak", 0, true)]
        void LogLists(bool lineBreak)
        {
            Debug.Log("An int list: " + _intList.ToLogString(lineBreak));
            Debug.Log("A Vector2 list: " + _vectorsList.ToLogString(lineBreak));
            Debug.Log("A sample object array: " + _sampleObjArray.ToLogString(lineBreak));
        }

        [DeMethodButton("Log Lists/Arrays (ids only)", 1, false)]
        [DeMethodButton("Log Lists/Arrays (ids only) w linebreak", 1, true)]
        void LogListsWFormatting(bool lineBreak)
        {
            Debug.Log("A sample object array (string id only): " + _sampleObjArray.ToLogString(x => "id: " + x.id, lineBreak));
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class SampleObject
        {
            public string id;
            public int code;
            public SampleObject(string id, int code)
            {
                this.id = id;
                this.code = code;
            }
        }
    }
}