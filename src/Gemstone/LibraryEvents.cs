﻿//******************************************************************************************************
//  LibraryEvents.cs - Gbtc
//
//  Copyright © 2019, Grid Protection Alliance.  All Rights Reserved.
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
//  12/27/2019 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Runtime.CompilerServices;
using Gemstone.EventHandlerExtensions;

// TODO: Add new libraries to internals visible list as needed
[assembly: InternalsVisibleTo("Gemstone.Communication")]
[assembly: InternalsVisibleTo("Gemstone.Data")]
[assembly: InternalsVisibleTo("Gemstone.Expressions")]
[assembly: InternalsVisibleTo("Gemstone.IO")]
[assembly: InternalsVisibleTo("Gemstone.Numeric")]
[assembly: InternalsVisibleTo("Gemstone.Threading")]
[assembly: InternalsVisibleTo("Gemstone.Security")]

#pragma warning disable CA1031 // Do not catch general exception types
// ReSharper disable DelegateSubtraction

namespace Gemstone
{
    /// <summary>
    /// Defines library-level static events.
    /// </summary>
    public static class LibraryEvents
    {
        private static EventHandler<UnhandledExceptionEventArgs>? s_suppressedExceptionHandler;
        private static readonly object s_suppressedExceptionLock = new object();

        /// <summary>
        /// Exposes exceptions that were suppressed but otherwise unhandled.  
        /// </summary>
        /// <remarks>
        /// End users should attach to this event so that suppressed exceptions can be exposed to a log.
        /// </remarks>
        public static event EventHandler<UnhandledExceptionEventArgs> SuppressedException
        {
            add
            {
                lock (s_suppressedExceptionLock)
                    s_suppressedExceptionHandler += value;
            }
            remove
            {
                lock (s_suppressedExceptionLock)
                    s_suppressedExceptionHandler -= value;
            }
        }

        internal static void OnSuppressedException(object sender, Exception ex)
        {
            if (s_suppressedExceptionHandler == null)
                return;

            static void exceptionHandler(Exception ex, EventHandler handler) =>
                throw new Exception($"Failed in {nameof(SuppressedException)} event handler \"{GetHandlerName(handler)}\": {ex.Message}", ex);

            s_suppressedExceptionHandler.SafeInvoke(s_suppressedExceptionLock, exceptionHandler, sender, new UnhandledExceptionEventArgs(ex, false));
        }

        private static string GetHandlerName(EventHandler userHandler)
        {
            try
            {
                return userHandler.Method.Name;
            }
            catch
            {
                return "<undetermined>";
            }
        }
    }
}
