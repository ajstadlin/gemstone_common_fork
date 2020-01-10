﻿//******************************************************************************************************
//  EventExtensions.cs - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/06/2020 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Threading.Tasks;

#pragma warning disable CA1031 // Do not catch general exception types

namespace Gemstone.EventHandlerExtensions
{
    /// <summary>
    /// Defines extension methods related to event handlers.
    /// </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Safely invokes event propagation, continuing even if an attached user handler throws an exception.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">Source <see cref="EventHandler"/> to safely invoke.</param>
        /// <param name="sender">Event source.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="parallel">Call event handlers in parallel.</param>
        /// <remarks>
        /// Accessing event handler invocation list will be locked will be on <c>typeof(EventHandler&lt;TEventArgs&gt;)</c>.
        /// Any exceptions will be suppressed, see other overloads for custom exception handling.
        /// </remarks>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs args, bool parallel = true) =>
            SafeInvoke(eventHandler, null, (Action<Exception, EventHandler<TEventArgs>>)null, sender, args, parallel);

        /// <summary>
        /// Safely invokes event propagation with custom exception handler, continuing even if an attached user handler throws an exception.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">Source <see cref="EventHandler"/> to safely invoke.</param>
        /// <param name="exceptionHandler">Exception handler; when set to <c>null</c>, exception will be suppressed.</param>
        /// <param name="sender">Event source.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="parallel">Call event handlers in parallel.</param>
        /// <remarks>
        /// Accessing event handler invocation list will be locked will be on <c>typeof(EventHandler&lt;TEventArgs&gt;)</c>.
        /// </remarks>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> eventHandler, Action<Exception> exceptionHandler, object sender, TEventArgs args, bool parallel = true) =>
            SafeInvoke(eventHandler, null, (ex, _) => exceptionHandler(ex), sender, args, parallel);

        /// <summary>
        /// Safely invokes event propagation with custom exception handler that accepts user handler delegate, continuing even if an attached user handler throws an exception.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">Source <see cref="EventHandler"/> to safely invoke.</param>
        /// <param name="exceptionHandler">Exception handler; when set to <c>null</c>, exception will be suppressed.</param>
        /// <param name="sender">Event source.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="parallel">Call event handlers in parallel.</param>
        /// <remarks>
        /// Accessing event handler invocation list will be locked will be on <c>typeof(EventHandler&lt;TEventArgs&gt;)</c>.
        /// </remarks>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> eventHandler, Action<Exception, EventHandler<TEventArgs>> exceptionHandler, object sender, TEventArgs args, bool parallel = true) =>
            SafeInvoke(eventHandler, null, exceptionHandler, sender, args, parallel);

        /// <summary>
        /// Safely invokes event propagation with custom exception handler, continuing even if an attached user handler throws an exception.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">Source <see cref="EventHandler"/> to safely invoke.</param>
        /// <param name="eventLock">Locking object for accessing event handler invocation list; when set to <c>null</c>, lock will be on <c>typeof(EventHandler&lt;TEventArgs&gt;)</c>.</param>
        /// <param name="exceptionHandler">Exception handler; when set to <c>null</c>, exception will be suppressed.</param>
        /// <param name="sender">Event source.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="parallel">Call event handlers in parallel.</param>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object eventLock, Action<Exception> exceptionHandler, object sender, TEventArgs args, bool parallel = true) =>
            SafeInvoke(eventHandler, eventLock, (ex, _) => exceptionHandler(ex), sender, args, parallel);

        /// <summary>
        /// Safely invokes event propagation with custom event lock and exception handler that accepts user handler delegate, continuing even if an attached user handler throws an exception.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler">Source <see cref="EventHandler"/> to safely invoke.</param>
        /// <param name="eventLock">Locking object for accessing event handler invocation list; when set to <c>null</c>, lock will be on <c>typeof(EventHandler&lt;TEventArgs&gt;)</c>.</param>
        /// <param name="exceptionHandler">Exception handler; when set to <c>null</c>, exception will be suppressed.</param>
        /// <param name="sender">Event source.</param>
        /// <param name="args">Event arguments.</param>
        /// <param name="parallel">Call event handlers in parallel.</param>
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object eventLock, Action<Exception, EventHandler<TEventArgs>> exceptionHandler, object sender, TEventArgs args, bool parallel = true)
        {
            if (eventHandler == null)
                return;

            if (eventLock == null)
                eventLock = typeof(EventHandler<TEventArgs>);

            Delegate[] handlers;

            lock (eventLock)
                handlers = eventHandler.GetInvocationList();

            void invokeHandler(Delegate handler)
            {
                if (!(handler is EventHandler<TEventArgs> userHandler))
                    return;

                try
                {
                    userHandler(sender, args);
                }
                catch (Exception ex)
                {
                    if (exceptionHandler == null)
                        LibraryEvents.OnSuppressedException(typeof(EventHandlerExtensions), new Exception($"Safe invoke user event handler exception: {ex.Message}", ex));
                    else
                        exceptionHandler(ex, userHandler);
                }
            }

            // Safely iterate each attached handler, continuing on possible exception, so no handlers are missed
            if (parallel)
            {
                Parallel.ForEach(handlers, invokeHandler);
            }
            else
            {
                foreach (Delegate handler in handlers)
                    invokeHandler(handler);
            }
        }
    }
}