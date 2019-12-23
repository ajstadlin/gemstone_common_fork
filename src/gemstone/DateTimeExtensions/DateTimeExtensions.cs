﻿//******************************************************************************************************
//  DateTimeExtensions.cs - Gbtc
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
//  02/23/2003 - J. Ritchie Carroll
//       Generated original version of source code.
//  12/21/2005 - J. Ritchie Carroll
//       Migrated 2.0 version of source code from 1.1 source (GSF.Shared.DateTime).
//  08/31/2007 - Darrell Zuercher
//       Edited code comments.
//  09/08/2008 - J. Ritchie Carroll
//       Converted to C# extensions.
//  02/16/2009 - Josh L. Patterson
//       Edited Code Comments.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  01/06/2010 - Andrew K. Hill
//       Modified the following methods per unit testing:
//       LocalTimeTo(DateTime, string)
//       LocalTimeTo(DateTime, TimeZoneInfo)
//       UniversalTimeTo(DateTime, string)
//       UniversalTimeTo(DateTime, TimeZoneInfo)
//       TimeZoneToTimeZone(DateTime, string, string)
//       TimeZoneToTimeZone(DateTime, TimeZoneInfo, TimeZoneInfo)
//  12/14/2012 - Starlynn Danyelle Gilliam
//       Modified Header.
//
//******************************************************************************************************

using System;
using System.Globalization;

namespace Gemstone.DateTimeExtensions
{
    /// <summary>
    /// Defines extension functions related to Date/Time manipulation.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>Converts given local time to Eastern time.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to Eastern time.</param>
        /// <returns>
        /// <para>Timestamp in Eastern time.</para>
        /// </returns>
        public static DateTime LocalTimeToEasternTime(this DateTime timestamp) => TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, USTimeZones.Eastern);

        /// <summary>Converts given local time to Central time.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to Central time.</param>
        /// <returns>
        /// <para>Timestamp in Central time.</para>
        /// </returns>
        public static DateTime LocalTimeToCentralTime(this DateTime timestamp) => TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, USTimeZones.Central);

        /// <summary>Converts given local time to Mountain time.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to Mountain time.</param>
        /// <returns>
        /// <para>Timestamp in Mountain time.</para>
        /// </returns>
        public static DateTime LocalTimeToMountainTime(this DateTime timestamp) => TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, USTimeZones.Mountain);

        /// <summary>Converts given local time to Pacific time.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to Pacific time.</param>
        /// <returns>
        /// <para>Timestamp in Pacific time.</para>
        /// </returns>
        public static DateTime LocalTimeToPacificTime(this DateTime timestamp) => TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, USTimeZones.Pacific);

        /// <summary>Converts given local time to Universally Coordinated Time (a.k.a., Greenwich Meridian Time).</summary>
        /// <remarks>This function is only provided for the sake of completeness. All it does is call the
        /// "ToUniversalTime" property on the given timestamp.</remarks>
        /// <param name="timestamp">Timestamp in local time to be converted to Universal time.</param>
        /// <returns>
        /// <para>Timestamp in UniversalTime (a.k.a., GMT).</para>
        /// </returns>
        public static DateTime LocalTimeToUniversalTime(this DateTime timestamp) => timestamp.ToUniversalTime();

        /// <summary>Converts given local time to time in specified time zone.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to time in specified time zone.</param>
        /// <param name="destinationTimeZoneStandardName">Standard name of desired end time zone for given
        /// timestamp.</param>
        /// <returns>
        /// <para>Timestamp in specified time zone.</para>
        /// </returns>
        public static DateTime LocalTimeTo(this DateTime timestamp, string destinationTimeZoneStandardName) => destinationTimeZoneStandardName == null ? throw new ArgumentNullException(nameof(destinationTimeZoneStandardName)) : TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneStandardName));

        /// <summary>Converts given local time to time in specified time zone.</summary>
        /// <param name="timestamp">Timestamp in local time to be converted to time in specified time zone.</param>
        /// <param name="destinationTimeZone">Desired end time zone for given timestamp.</param>
        /// <returns>
        /// <para>Timestamp in specified time zone.</para>
        /// </returns>
        public static DateTime LocalTimeTo(this DateTime timestamp, TimeZoneInfo destinationTimeZone) => destinationTimeZone == null ? throw new ArgumentNullException(nameof(destinationTimeZone)) : TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.Local, destinationTimeZone);

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to Eastern time timestamp.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <returns>The timestamp in Eastern time.</returns>
        public static DateTime UniversalTimeToEasternTime(this DateTime universalTimestamp) => TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, USTimeZones.Eastern);

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to Central time timestamp.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <returns>The timestamp in Central time.</returns>
        public static DateTime UniversalTimeToCentralTime(this DateTime universalTimestamp) => TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, USTimeZones.Central);

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to Mountain time timestamp.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <returns>The timestamp in Mountain time.</returns>
        public static DateTime UniversalTimeToMountainTime(this DateTime universalTimestamp) => TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, USTimeZones.Mountain);

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to Pacific time timestamp.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <returns>The timestamp in Pacific time.</returns>
        public static DateTime UniversalTimeToPacificTime(this DateTime universalTimestamp) => TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, USTimeZones.Pacific);

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to timestamp in specified time zone.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <param name="destinationTimeZoneStandardName">The time zone standard name to which the Universally
        /// Coordinated Time timestamp is to be converted to.</param>
        /// <returns>The timestamp in the specified time zone.</returns>
        public static DateTime UniversalTimeTo(this DateTime universalTimestamp, string destinationTimeZoneStandardName) => destinationTimeZoneStandardName == null ? throw new ArgumentNullException(nameof(destinationTimeZoneStandardName)) : TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneStandardName));

        /// <summary>
        /// Converts the specified Universally Coordinated Time timestamp to timestamp in specified time zone.
        /// </summary>
        /// <param name="universalTimestamp">The Universally Coordinated Time timestamp that is to be converted.</param>
        /// <param name="destinationTimeZone">The time zone to which the Universally Coordinated Time timestamp
        /// is to be converted to.</param>
        /// <returns>The timestamp in the specified time zone.</returns>
        public static DateTime UniversalTimeTo(this DateTime universalTimestamp, TimeZoneInfo destinationTimeZone) => destinationTimeZone == null ? throw new ArgumentNullException(nameof(destinationTimeZone)) : TimeZoneInfo.ConvertTime(universalTimestamp, TimeZoneInfo.Utc, destinationTimeZone);

        /// <summary>Converts given timestamp from one time zone to another using standard names for time zones.</summary>
        /// <param name="timestamp">Timestamp in source time zone to be converted to time in destination time zone.</param>
        /// <param name="sourceTimeZoneStandardName">Standard name of time zone for given source timestamp.</param>
        /// <param name="destinationTimeZoneStandardName">Standard name of desired end time zone for given source
        /// timestamp.</param>
        /// <returns>
        /// <para>Timestamp in destination time zone.</para>
        /// </returns>
        public static DateTime TimeZoneToTimeZone(this DateTime timestamp, string sourceTimeZoneStandardName, string destinationTimeZoneStandardName)
        {
            if (sourceTimeZoneStandardName == null)
                throw new ArgumentNullException(nameof(sourceTimeZoneStandardName));

            if (destinationTimeZoneStandardName == null)
                throw new ArgumentNullException(nameof(destinationTimeZoneStandardName));

            return TimeZoneInfo.ConvertTime(timestamp, TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneStandardName), TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneStandardName));
        }

        /// <summary>Converts given timestamp from one time zone to another.</summary>
        /// <param name="timestamp">Timestamp in source time zone to be converted to time in destination time
        /// zone.</param>
        /// <param name="sourceTimeZone">Time zone for given source timestamp.</param>
        /// <param name="destinationTimeZone">Desired end time zone for given source timestamp.</param>
        /// <returns>
        /// <para>Timestamp in destination time zone.</para>
        /// </returns>
        public static DateTime TimeZoneToTimeZone(this DateTime timestamp, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            if (sourceTimeZone == null)
                throw new ArgumentNullException(nameof(sourceTimeZone));

            if (destinationTimeZone == null)
                throw new ArgumentNullException(nameof(destinationTimeZone));

            return TimeZoneInfo.ConvertTime(timestamp, sourceTimeZone, destinationTimeZone);
        }

        /// <summary>Gets the abbreviated month name for month of the timestamp.</summary>
        /// <param name="timestamp">Timestamp from which month name is extracted.</param>
        /// <returns>String representation of the month name based on <paramref name="timestamp"/></returns>
        public static string AbbreviatedMonthName(this DateTime timestamp) => DateTimeFormatInfo.CurrentInfo?.GetAbbreviatedMonthName(timestamp.Month);

        /// <summary>Gets the full month name for month of the timestamp.</summary>
        /// <param name="timestamp">Timestamp from which month name is extracted.</param>
        /// <returns>String representation of the month name based on <paramref name="timestamp"/></returns>
        public static string MonthName(this DateTime timestamp) => DateTimeFormatInfo.CurrentInfo?.GetMonthName(timestamp.Month);

        /// <summary>Gets the abbreviated weekday name for weekday of the timestamp.</summary>
        /// <param name="timestamp">Timestamp from which weekday name is extracted.</param>
        /// <returns>String representation of the weekday name based on <paramref name="timestamp"/></returns>
        public static string AbbreviatedWeekdayName(this DateTime timestamp) => DateTimeFormatInfo.CurrentInfo?.GetAbbreviatedDayName(timestamp.DayOfWeek);

        /// <summary>Gets the shortest weekday name for weekday of the timestamp.</summary>
        /// <param name="timestamp">Timestamp from which weekday name is extracted.</param>
        /// <returns>String representation of the short weekday name based on <paramref name="timestamp"/></returns>
        public static string ShortWeekdayName(this DateTime timestamp) => DateTimeFormatInfo.CurrentInfo?.GetShortestDayName(timestamp.DayOfWeek);

        /// <summary>Gets the full weekday name for weekday of the timestamp.</summary>
        /// <param name="timestamp">Timestamp from which weekday name is extracted.</param>
        /// <returns>String representation of the weekday name based on <paramref name="timestamp"/></returns>
        public static string WeekdayName(this DateTime timestamp) => DateTimeFormatInfo.CurrentInfo?.GetDayName(timestamp.DayOfWeek);
    }
}