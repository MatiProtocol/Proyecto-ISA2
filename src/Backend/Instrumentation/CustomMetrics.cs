using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstrumentationInterface;

namespace Instrumentation
{
    public class CustomMetrics : ICustomMetrics, IDisposable
    {
        private readonly Meter _meter;
        private readonly Counter<long> _loginInvocationsCounter;
        private readonly Counter<long> _completedApprovedPurchasesCounter;
        private readonly Histogram<double> _requestDurationHistogram;
        private readonly Histogram<double> _approvedPurchaseAmountHistogram;
        private int _activeUserCount;
        private long _databaseSizeKb;

        // The ObservableGauge is created with a callback that reads the current _activeUserCount.
        // Note: We don't keep a reference to the ObservableGauge since the Meter tracks it internally.
        public CustomMetrics()
        {
            _meter = new Meter("PharmaGo.CustomMetrics");

            _loginInvocationsCounter = _meter.CreateCounter<long>(
                "login_invocations",
                unit: "1",
                description: "Counts the number of login invocations"
            );

            _completedApprovedPurchasesCounter = _meter.CreateCounter<long>(
                "completed_approved_purchases",
                unit: "1",
                description: "Counts the completed approved purchases"
            );

            _requestDurationHistogram = _meter.CreateHistogram<double>(
                "request_duration",
                unit: "ms",
                description: "Records the duration of requests in milliseconds"
            );

            // Create an observable gauge for active user count.
            // This gauge will invoke the lambda to return the current value of _activeUserCount.
            _meter.CreateObservableGauge(
                "active_user_count",
                () => new Measurement<int>[] { new Measurement<int>(_activeUserCount) },
                unit: "1",
                description: "The current number of active users"
            );

            _meter.CreateObservableGauge(
                "database_size",
                () => new Measurement<long>[] { new Measurement<long>(_databaseSizeKb) },
                unit: "kB",
                description: "The current database size in kilobytes"
            );

            _approvedPurchaseAmountHistogram = _meter.CreateHistogram<double>(
                "approved_purchase_amount",
                unit: "currency",
                description: "Amount of each fully approved purchase"
            );

            _meter.CreateObservableCounter<double>(
                "process_cpu_seconds_total",
                () => new[]
                {
                    new Measurement<double>(Process.GetCurrentProcess().TotalProcessorTime.TotalSeconds)
                },
                unit: "s",
                description: "Total CPU seconds used by this process."
            );
        }

        // Increment the login invocation counter
        public void LoginInvocations(long value = 1)
        {
            Console.WriteLine("Logging invocation");
            _loginInvocationsCounter.Add(value);
        }

        public void CompletedApprovedPurchases(long value = 1)
        {
            _completedApprovedPurchasesCounter.Add(value);
        }

        // Update the active user count; the observable gauge's callback will reflect this new value.
        public void SetActiveUserCount(int count)
        {
            _activeUserCount = count;
        }

        // Record the request duration in milliseconds using the histogram.
        public void RequestDuration(double milliseconds)
        {
            _requestDurationHistogram.Record(milliseconds);
        }

        public void CompletedApprovedPurchaseAmount(double amount)
        {
            _approvedPurchaseAmountHistogram.Record(amount);
        }

        public void SetDatabaseSize(long sizeKb)
        {
            _databaseSizeKb = sizeKb;
        }

        public void Dispose()
        {
            _meter?.Dispose();
        }
    }
}
