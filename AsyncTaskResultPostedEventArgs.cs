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
    public sealed class AsyncTaskResultPostedEventArgs : EventArgs {
        private object _data;
        private int _percentComplete;
        private bool _isComplete;

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="percentComplete"></param>
        /// <param name="isComplete"></param>
        internal AsyncTaskResultPostedEventArgs(object data, int percentComplete, bool isComplete) {
            _data = data;
            _percentComplete = percentComplete;
            _isComplete = isComplete;
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
                return _isComplete;
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
