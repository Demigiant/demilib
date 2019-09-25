// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/04/12

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    // Uses code from ChevyRay on TIGForums
    // https://forums.tigsource.com/index.php?topic=32178.0
    public static class DeEditorCoroutines
    {
        static readonly List<IEnumerator> _Coroutines = new List<IEnumerator>();

        #region Public Methods

        /// <summary>
        /// Starts an editor coroutine. You can't use normal <code>yield new WaitFor</code> methods because
        /// those are Unity runtime, but you can instead use <see cref="DeEditorCoroutines.WaitForSeconds"/>.
        /// Other than that, you can use normal <code>yield null/etc</code>.
        /// </summary>
        public static void StartCoroutine(IEnumerator coroutine)
        {
            _Coroutines.Add(coroutine);
            if (_Coroutines.Count == 1) EditorApplication.update += UpdateCoroutines;
        }

        public static IEnumerator WaitForSeconds(float seconds)
        {
            Stopwatch watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < seconds) yield return 0;
        }

        #endregion

        #region Methods

        static void UpdateCoroutines()
        {
            for (int i = 0; i < _Coroutines.Count; ++i) {
                if (_Coroutines[i].Current is IEnumerator) {
                    if (MoveNext((IEnumerator)_Coroutines[i].Current)) continue;
                }
                if (!_Coroutines[i].MoveNext()) _Coroutines.RemoveAt(i--);
            }

            if (_Coroutines.Count == 0) EditorApplication.update -= UpdateCoroutines;
        }

        static bool MoveNext(IEnumerator coroutine)
        {
//            if (coroutine.Current is IEnumerator current) {
//                if (MoveNext(current)) return true;
//            }

            IEnumerator current = coroutine.Current as IEnumerator;
            if (current != null) {
                if (MoveNext(current)) return true;
            }
            return coroutine.MoveNext();
        }

        #endregion
    }
}