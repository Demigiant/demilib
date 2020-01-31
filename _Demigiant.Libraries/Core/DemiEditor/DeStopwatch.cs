// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/01/31

using System.Diagnostics;

namespace DG.DemiEditor
{
    /// <summary>
    /// A stopwatch whose time can be changed manually via <see cref="Goto"/>
    /// </summary>
    public class DeStopwatch
    {
        public float elapsed { get { return _sw.ElapsedMilliseconds * 0.001f + _offset; } }

        readonly Stopwatch _sw = new Stopwatch();
        float _offset;

        #region Public Methods

        /// <summary>
        /// Start or resume playing
        /// </summary>
        public void Start()
        {
            _sw.Start();
        }

        /// <summary>
        /// Stop the watch and reset the time
        /// </summary>
        public void Reset()
        {
            _offset = 0;
            _sw.Reset();
        }

        /// <summary>
        /// Restart measuring from zero
        /// </summary>
        public void Restart()
        {
            Reset();
            Start();
        }

        /// <summary>
        /// Pause the watch
        /// </summary>
        public void Stop()
        {
            _sw.Stop();
        }

        /// <summary>
        /// Send the watch to the given time
        /// </summary>
        public void Goto(float seconds, bool andPlay = false)
        {
            _offset = seconds - _sw.ElapsedMilliseconds * 0.001f;
            if (!andPlay) _sw.Stop();
            else _sw.Start();
        }

        #endregion
    }
}