﻿//******************************************************************************************************
//  ProcessProgressHandler.cs - Gbtc
//
//  Copyright © 2012, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/04/2008 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;

namespace Gemstone;

/// <summary>
/// Defines a delegate handler for a <see cref="ProcessProgress{TUnit}"/> instance.
/// </summary>
/// <remarks>
/// This handler is used by methods with an <see cref="Action"/> delegate parameter (e.g., Action&lt;ProcessProgress&lt;long&gt;&gt;)
/// providing a simple callback mechanism for reporting progress on a long operation.
/// </remarks>
/// <typeparam name="TUnit">Unit of progress used (long, double, int, etc.)</typeparam>
public class ProcessProgressHandler<TUnit> where TUnit : struct
{
    #region [ Constructors ]

    /// <summary>
    /// Constructs a new process progress handler for the specified parameters.
    /// </summary>
    /// <param name="progressHandler">Delegate callback to invoke as process progresses.</param>
    /// <param name="processName">Descriptive name of process, if useful.</param>
    public ProcessProgressHandler(Action<ProcessProgress<TUnit>> progressHandler, string processName)
    {
        ProgressHandler = progressHandler;
        ProcessProgress = new ProcessProgress<TUnit>(processName);
    }

    /// <summary>
    /// Constructs a new process progress handler for the specified parameters.
    /// </summary>
    /// <param name="progressHandler">Delegate callback to invoke as process progresses.</param>
    /// <param name="processName">Descriptive name of process, if useful.</param>
    /// <param name="total">Total number of units to be processed.</param>
    public ProcessProgressHandler(Action<ProcessProgress<TUnit>> progressHandler, string processName, TUnit total)
        : this(progressHandler, processName)
    {
        ProcessProgress.Total = total;
    }

    #endregion

    #region [ Properties ]

    /// <summary>
    /// Gets instance of <see cref="ProcessProgress{TUnit}"/> used to track progress for this handler.
    /// </summary>
    public ProcessProgress<TUnit> ProcessProgress { get; }

    /// <summary>
    /// Gets or sets reference to delegate handler used as a callback to report process progress.
    /// </summary>
    public Action<ProcessProgress<TUnit>> ProgressHandler { get; set; }

    /// <summary>
    /// Gets or sets current process progress (i.e., number of units completed processing so far) - note that when this
    /// property value is assigned, the callback function is automatically called with updated <see cref="ProcessProgress{TUnit}"/>
    /// instance so consumer can track progress.
    /// </summary>
    /// <value>Number of units completed processing so far.</value>
    public TUnit Complete
    {
        get
        {
            return ProcessProgress.Complete;
        }
        set
        {
            UpdateProgress(value);
        }
    }

    /// <summary>
    /// Gets or sets total number of units to be processed.
    /// </summary>
    public TUnit Total
    {
        get
        {
            return ProcessProgress.Total;
        }
        set
        {
            ProcessProgress.Total = value;
        }
    }

    #endregion

    #region [ Methods ]

    /// <summary>
    /// Calls callback function with updated <see cref="ProcessProgress{TUnit}"/> instance so consumer can track progress.
    /// </summary>
    /// <param name="completed">Number of units completed processing so far.</param>
    /// <remarks>
    /// Note that assigning a value to the <see cref="Complete"/> property will have the same effect as calling this method.
    /// </remarks>
    public void UpdateProgress(TUnit completed)
    {
        // Update bytes completed
        ProcessProgress.Complete = completed;

        // Call user function
        ProgressHandler(ProcessProgress);
    }

    #endregion
}
