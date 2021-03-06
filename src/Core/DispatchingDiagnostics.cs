using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GreenDonut
{
    internal static class DispatchingDiagnostics
    {
        private const string _diagnosticSourceName = "GreenDonut.Dispatching";
        private const string _batchActivityName = "ExecuteBatchRequest";
        private const string _cachedValueEventName = "CachedValue";
        private const string _errorEventName = "Error";

        private static readonly DiagnosticSource _source =
            new DiagnosticListener(_diagnosticSourceName);

        public static void RecordCachedValue<TKey, TValue>(TKey key, TValue value)
        {
            var context = new
            {
                Key = key,
                Value = value
            };

            if (_source.IsEnabled(_cachedValueEventName, context))
            {
                _source.Write(_cachedValueEventName, context);
            }
        }

        public static void RecordError<TKey>(TKey key, Exception exception)
        {
            var context = new
            {
                Exception = exception,
                Key = key
            };

            if (_source.IsEnabled(_errorEventName, context))
            {
                _source.Write(_errorEventName, context);
            }
        }

        public static Activity StartBatching<TKey>(
            IReadOnlyList<TKey> keys)
        {
            var context = new
            {
                Keys = keys
            };

            if (_source.IsEnabled(_batchActivityName, context))
            {
                var activity = new Activity(_batchActivityName);

                _source.StartActivity(activity, context);

                return activity;
            }

            return null;
        }

        public static void StopBatching<TKey, TValue>(
            Activity activity,
            IReadOnlyList<TKey> keys,
            IReadOnlyList<IResult<TValue>> results)
        {
            if (activity != null)
            {
                var context = new
                {
                    Keys = keys,
                    Results = results
                };

                if (_source.IsEnabled(_batchActivityName, context))
                {
                    _source.StopActivity(activity, context);
                }
            }
        }
    }
}
