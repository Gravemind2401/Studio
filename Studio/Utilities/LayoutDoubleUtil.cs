using System.Reflection;
using System.Windows;

//https://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/windows/FrameworkElement.cs,8a262f3298ea54c0

namespace Studio.Utilities
{
    // LayoutDoubleUtil, uses fixed eps unlike DoubleUtil which uses relative one.
    // This is more suitable for some layout comparisons because the computation
    // paths in layout may easily be quite long so DoubleUtil method gives a lot of false
    // results, while bigger absolute deviation is normally harmless in layout.
    // Note that FP noise is a big problem and using any of these compare methods is
    // not a complete solution, but rather the way to reduce the probability
    // of the dramatically bad-looking results.
    internal static class LayoutDoubleUtil
    {
        private const double eps = 0.00000153; //more or less random more or less small number

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
                return true;

            var diff = value1 - value2;
            return (diff < eps) && (diff > -eps);
        }

        public static bool LessThan(double value1, double value2) => value1 < value2 && !AreClose(value1, value2);

        public static double RoundLayoutValue(double pixels, double dpiScale)
        {
            var method = typeof(UIElement).GetMethod(nameof(RoundLayoutValue), BindingFlags.Static | BindingFlags.NonPublic);
            return (double)method.Invoke(null, new object[] { pixels, dpiScale });
        }
    }
}
