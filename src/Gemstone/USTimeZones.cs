﻿//******************************************************************************************************
//  USTimeZones.cs - Gbtc
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
//  09/12/2008 - J. Ritchie Carroll
//       Generated original version of source code.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;

// ReSharper disable InconsistentNaming
namespace Gemstone;

/// <summary>
/// Defines a few common United States time zones.
/// </summary>
public static class USTimeZones
{
    // We define a few common timezones for convenience.
    private static TimeZoneInfo? s_atlanticTimeZone;
    private static TimeZoneInfo? s_easternTimeZone;
    private static TimeZoneInfo? s_centralTimeZone;
    private static TimeZoneInfo? s_mountainTimeZone;
    private static TimeZoneInfo? s_pacificTimeZone;
    private static TimeZoneInfo? s_alaskanTimeZone;
    private static TimeZoneInfo? s_hawaiianTimeZone;
    private static TimeZoneInfo? s_westPacificTimeZone;
    private static TimeZoneInfo? s_samoaTimeZone;

    /// <summary>
    /// Gets the Atlantic time zone.
    /// </summary>
    /// <remarks>This time zone is used by the Commonwealth of Puerto Rico and the United States Virgin Islands.</remarks>
    public static TimeZoneInfo Atlantic
    {
        get
        {
            return s_atlanticTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Atlantic Standard Time");
        }
    }

    /// <summary>
    /// Gets the Eastern time zone.
    /// </summary>
    public static TimeZoneInfo Eastern
    {
        get
        {
            return s_easternTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
    }

    /// <summary>
    /// Gets the Central time zone.
    /// </summary>
    public static TimeZoneInfo Central
    {
        get
        {
            return s_centralTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        }
    }

    /// <summary>
    /// Gets the Mountain time zone.
    /// </summary>
    public static TimeZoneInfo Mountain
    {
        get
        {
            return s_mountainTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
        }
    }

    /// <summary>
    /// Gets the Pacific time zone.
    /// </summary>
    public static TimeZoneInfo Pacific
    {
        get
        {
            return s_pacificTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }
    }

    /// <summary>
    /// Gets the Alaskan time zone.
    /// </summary>
    public static TimeZoneInfo Alaskan
    {
        get
        {
            return s_alaskanTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
        }
    }

    /// <summary>
    /// Gets the Hawaiian time zone.
    /// </summary>
    public static TimeZoneInfo Hawaiian
    {
        get
        {
            return s_hawaiianTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Hawaiian Standard Time");
        }
    }

    /// <summary>
    /// Gets the West Pacific time zone.
    /// </summary>
    /// <remarks>
    /// <para>This time zone is used by Guam and the Commonwealth of the Northern Mariana Islands.</para>
    /// <para>This is also known as the Chamorro time zone.</para>
    /// </remarks>
    public static TimeZoneInfo WestPacific
    {
        get
        {
            return s_westPacificTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("West Pacific Standard Time");
        }
    }

    /// <summary>
    /// Gets the Samoa time zone.
    /// </summary>
    /// <remarks>This time zone is used by the American Samoa.</remarks>
    public static TimeZoneInfo Samoa
    {
        get
        {
            return s_samoaTimeZone ??= TimeZoneInfo.FindSystemTimeZoneById("Samoa Standard Time");
        }
    }
}
