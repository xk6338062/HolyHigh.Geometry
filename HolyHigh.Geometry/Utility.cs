using System;
using System.Drawing;
using System.Globalization;

namespace HolyHigh.Geometry
{
    public static class Utility
    {
        #region  constants
        private const double tol3 = 0.001;

        private const double tol6 = 1E-06;

        private const double tol9 = 1E-09;

        public const double ZeroTolerance = 1e-12;

        public const int HDLGRIDSPAN = 524288;

        public const int maxTry = 32;

        public const double maxAcceptableSpeedRatio = 2.0;

        public const double EPSILON = 2.2204460492503131E-16;

        public const double SQRT_EPSILON = 1.490116119385E-08;

        public const double ZERO_TOLERANCE = 1E-12;

        public const double SQRT2 = 1.4142135623730951;

        public const double SQRT3 = 1.7320508075688772;

        public const double SQRT3_OVER_2 = 0.8660254037844386;

        public const double POS_MIN_DBL = 2.2250738585072014E-308;

        public const string STEP_FORMAT_STRING = "0.0###########";

        public const double TWO_PI = 6.2831853071795862;

        public const double QUARTER_PI = 0.78539816339744828;

        public const double THREE_QUARTER_PI = 2.3561944901923448;

        public const double PI_2 = 1.5707963267948966;

        public const double MAX_ALLOWED_SPEED_RATIO = 1.1;

        public const double UnsetValue = -1.23432101234321e+308;

        public const double DefaultAngleTolerance = Math.PI / 180.0;

        /// <summary>
        /// 1e-3
        /// </summary>
        public static double TOL3 => tol3;

        /// <summary>
        /// 1e-6
        /// </summary>
        public static double TOL6 => tol6;

        /// <summary>
        /// 1e-9
        /// </summary>
        public static double TOL9 => tol9;

        //private static SizeF Empty = SizeF.Empty;
        #endregion

        public static bool IsValidDouble(double x)
        {
            if (x != -1.23432101234321E+308 && !double.IsInfinity(x))
                return !double.IsNaN(x);
            return false;
        }

        public static bool EpsilonEquals(double x, double y, double epsilon)
        {
            if (double.IsNaN(x) || double.IsNaN(y))
                return false;
            if (double.IsPositiveInfinity(x))
                return double.IsPositiveInfinity(y);
            if (double.IsNegativeInfinity(x))
                return double.IsNegativeInfinity(y);
            if (Math.Abs(x) < epsilon && Math.Abs(y) < epsilon)
                return Math.Abs(x - y) < epsilon;
            if (x >= y - epsilon)
                return x <= y + epsilon;
            return false;
        }

        public static int Compare(double a, double b, double epsilon)
        {
            double delta = a - b;
            if (Math.Abs(delta) <= epsilon) return 0;
            return (delta < 0 ? -1 : 1);
        }
        public static void LimitRange<T>(T low, ref T value, T high) where T : IComparable
        {
            if (value.CompareTo(low) < 0)
            {
                value = low;
                return;
            }
            if (value.CompareTo(high) > 0)
            {
                value = high;
            }
        }
        public static double DoubleParse(string value)
        {
            value = value.TrimEnd('.');
            return double.Parse(value, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture);
        }
        public static float FloatParse(string value)
        {
            return float.Parse(value, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture);
        }
        public static bool DoubleTryParse(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.Float, (IFormatProvider)CultureInfo.InvariantCulture, out result);
        }
        //public static Color GetRandomColor(Random rand)
        //{
        //    return Color.FromArgb((int)(rand.NextDouble() * (double)byte.MaxValue), (int)(rand.NextDouble() * (double)byte.MaxValue), (int)(rand.NextDouble() * (double)byte.MaxValue));
        //}
        /// <summary>
        /// ª•ªªŒª÷√
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void Swap<T>(ref T first, ref T second)
        {
            T t = first;
            first = second;
            second = t;
        }
        public static bool AreEqual(double a, double b, double domainSize)
        {
            return Math.Abs(b - a) / domainSize < 1E-09;
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public static double[] SolveWith(this double[] a, double[] b)
        {
            var inv = InvertM3(a);
            return new double[]
            {
                inv[0] * b[0] + inv[1] * b[1] + inv[2] * b[2],
                inv[3] * b[0] + inv[4] * b[1] + inv[5] * b[2],
                inv[6] * b[0] + inv[7] * b[1] + inv[8] * b[2]
            };
        }

        private static double GetDetM3(double[] m)
        {
            return m[0] * (m[4] * m[8] - m[5] * m[7]) -
                        m[1] * (m[3] * m[8] - m[5] * m[6]) +
                        m[2] * (m[3] * m[7] - m[4] * m[6]);
        }

        private static double[] InvertM3(double[] m)
        {
            double determinant, invDeterminant;
            double[] tmp = new double[9];

            tmp[0] = m[4] * m[8] - m[5] * m[7];
            tmp[1] = m[7] * m[2] - m[8] * m[1];
            tmp[2] = m[1] * m[5] - m[2] * m[4];
            tmp[3] = m[5] * m[6] - m[3] * m[8];
            tmp[4] = m[0] * m[8] - m[2] * m[6];
            tmp[5] = m[2] * m[3] - m[0] * m[5];
            tmp[6] = m[3] * m[7] - m[4] * m[6];
            tmp[7] = m[6] * m[1] - m[7] * m[0];
            tmp[8] = m[0] * m[4] - m[1] * m[3];

            // check determinant if it is 0
            determinant = m[0] * tmp[0] + m[1] * tmp[3] + m[2] * tmp[6];
            if (Math.Abs(determinant) <= POS_MIN_DBL)
            {
                throw new ArgumentException("Cannot Inverse"); // cannot inverse, make it idenety matrix
            }

            // divide by the determinant
            double[] inv = new double[9];
            invDeterminant = 1.0f / determinant;
            inv[0] = invDeterminant * tmp[0];
            inv[1] = invDeterminant * tmp[1];
            inv[2] = invDeterminant * tmp[2];
            inv[3] = invDeterminant * tmp[3];
            inv[4] = invDeterminant * tmp[4];
            inv[5] = invDeterminant * tmp[5];
            inv[6] = invDeterminant * tmp[6];
            inv[7] = invDeterminant * tmp[7];
            inv[8] = invDeterminant * tmp[8];
            return inv;
        }
    }
}
