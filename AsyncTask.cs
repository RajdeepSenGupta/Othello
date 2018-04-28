//------------------------------------------------------------------------------
// <copyright from='2001' to='2002' company='Microsoft Corporation'>
//   Copyright (c) Microsoft Corporation. All Rights Reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
//------------------------------------------------------------------------------

namespace Othello {
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// </summary>
    public abstract class AsyncTask {
        private int _isCanceled;
        private bool _isBusy;
        private AsyncTaskResultPostedEventHandler _resultPostedHandler;

        /// <summary>
        /// </summary>
        public bool IsBusy {
            get {
                return _isBusy;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsCanceled {
            get {
                return _isCanceled != 0;
            }
        }

        /// <summary>
        /// </summary>
        public void Cancel() {
            Interlocked.Increment(ref _isCanceled);
        }

        /// <summary>
        /// </summary>
        private void InvokePerformTask() {
            try {
                PerformTask();
            }
            catch (Exception e) {
                Debug.Fail(e.ToString());
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        public void Start(AsyncTaskResultPostedEventHandler handler) {
            _resultPostedHandler = handler;
            _isBusy = true;
            try {
                MethodInvoker mi = new MethodInvoker(this.InvokePerformTask);
                mi.BeginInvoke(null, null);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString());
            }
            finally {
                _isBusy = false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="handler"></param>
        public void StartSynchronous(AsyncTaskResultPostedEventHandler handler) {
            _resultPostedHandler = handler;
            _isBusy = true;
            try {
                InvokePerformTask();
            }
            catch (Exception e) {
                Debug.Fail(e.ToString());
            }
            finally {
                _isBusy = false;
            }
        }

        /// <summary>
        /// </summary>
        protected abstract void PerformTask();

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="percentComplete"></param>
        protected void PostResults(object data, int percentComplete, bool completed) {
            if (_resultPostedHandler != null) {
                AsyncTaskManager.PostMessage(new AsyncTaskMessage(this, data, percentComplete, completed));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="percentComplete"></param>
        internal void RaiseResultPostedEvent(object data, int percentComplete, bool completed) {
            _resultPostedHandler(this, new AsyncTaskResultPostedEventArgs(data, percentComplete, completed));
        }
    }
}
