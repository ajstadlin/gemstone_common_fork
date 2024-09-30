//******************************************************************************************************
//  AssemblyLoadedVersionNumber.cs - Gbtc
//
//  Copyright � 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  11/19/2016 - Steven E. Chisholm
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Threading;

namespace Gemstone.Reflection;

/// <summary>
/// Maintains a version number that increments every time an <see cref="AppDomain.AssemblyLoad"/> event is raised.
/// </summary>
public static class AssemblyLoadedVersionNumber
{
    private static int s_versionNumber = 1;

    /// <summary>
    /// The number of times that the AppDomains's assembly could have changed; initial value starts at 1.
    /// </summary>
    public static int VersionNumber
    {
        get
        {
            return s_versionNumber;
        }
    }

    static AssemblyLoadedVersionNumber()
    {
        AppDomain.CurrentDomain.AssemblyLoad += (_, _) => Interlocked.Increment(ref s_versionNumber);
    }
}
