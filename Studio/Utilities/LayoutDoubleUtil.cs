using System.Windows;

namespace Studio.Utilities
{
    internal static class LayoutDoubleUtil
    {
        private const double eps = 0.00000153; //more or less random more or less small number

        internal static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
                return true;

            double diff = value1 - value2;
            return (diff < eps) && (diff > -eps);
        }

        internal static double RoundLayoutValue(double pixels, double dpiScale)
        {
            var method = typeof(UIElement).GetMethod(nameof(RoundLayoutValue), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (double)method.Invoke(null, new object[] { pixels, dpiScale });
        }
    }
}
