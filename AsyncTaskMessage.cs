//------------------------------------------------------------------------------
// <copyright from='2001' to='2002' company='Microsoft Corporation'>
//   Copyright (c) Microsoft Corporation. All Rights Reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
//------------------------------------------------------------------------------

namespace Othello {
    using System;
    using System.Diagnostics;

    /// <summary>
    /// </summary>
    internal sealed class AsyncTaskMessage {
        private AsyncTask _task;
        private object _data;
        private int _percentComplete;
        private bool _completed;

        /// <summary>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="data"></param>
        /// <param name="percentComplete"></param>
        public AsyncTaskMessage(AsyncTask task, object data, int percentComplete, bool completed) {
            _task = task;
            _data = data;
            _percentComplete = percentComplete;
            _completed = completed;
        }

        /// <summary>
        /// </summary>
        public AsyncTask Task {
            get {
                return _task;
            }
        }

        /// <summary>
        /// </summary>
        public object Data {
            get {
                return _data;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsComplete {
            get {
                return _completed;
            }
        }

        /// <summary>
        /// </summary>
        public int PercentComplete {
            get {
                return _percentComplete;
            }
        }
    }
}
