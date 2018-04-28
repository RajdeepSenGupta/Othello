//------------------------------------------------------------------------------
// <copyright from='2001' to='2002' company='Microsoft Corporation'>
//   Copyright (c) Microsoft Corporation. All Rights Reserved.
//   Information Contained Herein is Proprietary and Confidential.
// </copyright>
//------------------------------------------------------------------------------

namespace Othello
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    /// <summary>
    /// </summary>
    public sealed class AsyncTaskManager
    {
        private static Control uiController;

        /// <summary>
        /// </summary>
        /// <param name="uiController"></param>
        public static void RegisterUIThread(Control uiController)
        {
            AsyncTaskManager.uiController = uiController;
            IntPtr h = uiController.Handle;
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        private static void UIThreadCallback(AsyncTaskMessage message)
        {
            message.Task.RaiseResultPostedEvent(message.Data, message.PercentComplete, message.IsComplete);
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        internal static void PostMessage(AsyncTaskMessage message)
        {
            if (uiController != null)
            {
                uiController.BeginInvoke(new PostMessageCallback(UIThreadCallback), new object[] { message });
            }
        }

        /// <summary>
        /// </summary>
        private delegate void PostMessageCallback(AsyncTaskMessage message);
    }
}
