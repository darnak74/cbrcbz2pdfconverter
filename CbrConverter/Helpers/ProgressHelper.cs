using System;

namespace CbrConverter.Helpers
{
    public static class ProgressHelper
    {
        public static double ClampPercent(double value)
        {
            return Math.Max(0, Math.Min(100, value));
        }

        public static double ComputeGlobalProgress(int completed, int total)
        {
            if (total <= 0)
                return 0;

            return ClampPercent((completed / (double)total) * 100.0);
        }
    }
}
