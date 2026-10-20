using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentationInterface
{
    public interface ICustomMetrics
    {
        // Counters
        void LoginInvocations(long value = 1);
        void CompletedApprovedPurchases(long value = 1);

        // Gauges
        void SetActiveUserCount(int count);

        // Histograms / Timings
        void RequestDuration(double milliseconds);
        void CompletedApprovedPurchaseAmount(double amount);
        void SetDatabaseSize(long sizeKb);
    }
}
