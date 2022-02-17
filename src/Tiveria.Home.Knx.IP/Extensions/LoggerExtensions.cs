/*
    Tiveria.Home.Knx - a .Net Core base KNX library
    Copyright (c) 2018-2022 M. Geissler

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

    Linking this library statically or dynamically with other modules is
    making a combined work based on this library. Thus, the terms and
    conditions of the GNU Lesser General Public License cover the whole
    combination.
*/

using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.CompilerServices;
using Tiveria.Home.Knx.Exceptions;

namespace Tiveria.Home.Knx.IP.Extensions
{
#pragma warning disable CS1591 // Missing XML comments
    public static partial class LoggerExtensions
    {
        public const int IDBase = 50000;
        public const int TunnelingConnectionFailedEvent = IDBase;

        #region Trace logs only active in debug mode
#if DEBUG
        [LoggerMessage(Level = LogLevel.Trace, Message = "+ Begin `{functionName}`.")]
        public static partial void TraceBeginFunc(this ILogger logger, [CallerMemberName] string? functionName = null);

        [LoggerMessage(Level = LogLevel.Trace, Message = "+ Begin `{functionName}` ({param1}).")]
        public static partial void TraceBeginFunc1(this ILogger logger, string functionName, object param1);
        
        [LoggerMessage(Level = LogLevel.Trace, Message = "+ Begin `{functionName}` ({param1}, {param2})")]
        public static partial void TraceBeginFunc2(this ILogger logger, string functionName, object param1, object param2);
        
        [LoggerMessage(Level = LogLevel.Trace, Message = "+ Begin `{functionName}` ({param1}, {param2}, {param3})")]
        public static partial void TraceBeginFunc3(this ILogger logger, string functionName, object param1, object param2, object param3);
        
        [LoggerMessage(Level = LogLevel.Trace, Message = "+ Begin `{functionName}` ({param1}, {param2}, {param3}, {param4})")]
        public static partial void TraceBeginFunc4(this ILogger logger, string functionName, object param1, object param2, object param3, object param4);

        [LoggerMessage(Level = LogLevel.Trace, Message = "+ End `{functionName}`.")]
        public static partial void TraceEndFunc(this ILogger logger, [CallerMemberName] string? functionName = null);

        [LoggerMessage(Level = LogLevel.Trace, Message = "+ End `{functionName}`. Returns {param1}.")]
        public static partial void TraceEndFuncRet(this ILogger logger, string functionName, object param1);
#else
        public static void TraceBeginFunc(this ILogger logger, string? functionName = null) {}
        public static void TraceBeginFunc1(this ILogger logger, string functionName, object param1) {}
        public static void TraceBeginFunc2(this ILogger logger, string functionName, object param1, object param2) {}
        public static void TraceBeginFunc3(this ILogger logger, string functionName, object param1, object param2, object param3) {}
        public static void TraceBeginFunc4(this ILogger logger, string functionName, object param1, object param2, object param3, object param4) {}
        public static void TraceEndFunc(this ILogger logger, string? functionName= null) {}
        public static void TraceEndFuncRet(this ILogger logger, string functionName, object param1) {}
#endif
        #endregion

        #region TunnelingConnection Logs
        [LoggerMessage(TunnelingConnectionFailedEvent, LogLevel.Error, "TunnelingConnection could not be set up.")]
        public static partial void LogTunnelingConnectionFailed(this ILogger logger, Exception ex);

        #endregion
    }

    internal static class LogEvents
    {
        private const int IDBase = 50000;
        internal static readonly EventId TunnelingConnectionFailed = new(IDBase + 0);
    }
#pragma warning restore CS1591 // Missing XML comments
}
