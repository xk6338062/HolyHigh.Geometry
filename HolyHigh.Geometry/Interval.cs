using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents an interval in one-dimensional space,
    /// that is defined as two extrema or bounds.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 16)]
    [DebuggerDisplay("({m_t0}, {m_t1})")]
    [Serializable]
    public struct Interval : ISerializable, IEquatable<Interval>, IComparable<Interval>, IComparable, IEpsilonComparable<Interval>
    {
        #region Members
        private double m_t0;
        private double m_t1;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Rhino.Geometry.Interval class.
        /// </summary>
        /// <param name="t0">The first value.</param>
        /// <param name="t1">The second value.</param>
        public Interval(double t0, double t1)
        {
            m_t0 = t0;
            m_t1 = t1;
        }

        /// <summary>
        /// Initializes a new instance copying the other instance values.
        /// </summary>
        /// <param name="other">The Rhino.Geometry.Interval to use as a base.</param>
        public Interval(Interval other)
        {
            m_t0 = other.m_t0;
            m_t1 = other.m_t1;
        }

        private Interval(SerializationInfo info, StreamingContext context)
        {
            m_t0 = info.GetDouble("T0");
            m_t1 = info.GetDouble("T1");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("T0", m_t0);
            info.AddValue("T1", m_t1);
        }

        #endregion

        #region Operators
        /// <summary>
        /// Determines whether the two Intervals have equal values.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>true if the components of the two intervals are exactly equal; otherwise false.</returns>
        public static bool operator ==(Interval a, Interval b)
        {
            return a.CompareTo(b) == 0;
        }

        /// <summary>
        /// Determines whether the two Intervals have different values.
        /// </summary>
        /// <param name="a">The first interval.</param>
        /// <param name="b">The second interval.</param>
        /// <returns>true if the two intervals are different in any value; false if they are equal.</returns>
        public static bool operator !=(Interval a, Interval b)
        {
            return a.CompareTo(b) != 0;
        }

        /// <summary>
        /// Shifts a <see cref="Interval" /> by a specific amount (addition).
        /// </summary>
        /// <param name="interval">The interval to be used as a base.</param>
        /// <param name="number">The shifting value.</param>
        /// <returns>A new interval where T0 and T1 are summed with number.</returns>
        public static Interval operator +(Interval interval, double number)
        {
            return new Interval(interval.m_t0 + number, interval.m_t1 + number);
        }

        /// <summary>
        /// Shifts an interval by a specific amount (addition).
        /// </summary>
        /// <param name="number">The shifting value.</param>
        /// <param name="interval">The interval to be used as a base.</param>
        /// <returns>A new interval where T0 and T1 are summed with number.</returns>
        public static Interval operator +(double number, Interval interval)
        {
            return new Interval(interval.m_t0 + number, interval.m_t1 + number);
        }

        /// <summary>
        /// Shifts an interval by a specific amount (subtraction).
        /// </summary>
        /// <param name="interval">The base interval (minuend).</param>
        /// <param name="number">The shifting value to be subtracted (subtrahend).</param>
        /// <returns>A new interval with [T0-number, T1-number].</returns>
        public static Interval operator -(Interval interval, double number)
        {
            return new Interval(interval.m_t0 - number, interval.m_t1 - number);
        }

        /// <summary>
        /// Shifts an interval by a specific amount (subtraction).
        /// </summary>
        /// <param name="number">The shifting value to subtract from (minuend).</param>
        /// <param name="interval">The interval to be subtracted from (subtrahend).</param>
        /// <returns>A new interval with [number-T0, number-T1].</returns>
        public static Interval operator -(double number, Interval interval)
        {
            return new Interval(number - interval.m_t0, number - interval.m_t1);
        }

        /// <summary>
        /// Determines whether the first specified <see cref="Interval"/> comes before
        /// (has inferior sorting value than) the second Interval.
        /// <para>The lower bound has first evaluation priority.</para>
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>true if a[0] is smaller than b[0], or a[0] == b[0] and a[1] is smaller than b[1]; otherwise, false.</returns>
        public static bool operator <(Interval a, Interval b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Determines whether the first specified <see cref="Interval"/> comes before
        /// (has inferior sorting value than) the second Interval, or is equal to it.
        /// <para>The lower bound has first evaluation priority.</para>
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>true if a[0] is smaller than b[0], or a[0] == b[0] and a[1] is smaller than or equal to b[1]; otherwise, false.</returns>
        public static bool operator <=(Interval a, Interval b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified <see cref="Interval"/> comes after
        /// (has superior sorting value than) the second Interval.
        /// <para>The lower bound has first evaluation priority.</para>
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>true if a[0] is larger than b[0], or a[0] == b[0] and a[1] is larger than b[1]; otherwise, false.</returns>
        public static bool operator >(Interval a, Interval b)
        {
            return a.CompareTo(b) > 0;
        }

        /// <summary>
        /// Determines whether the first specified <see cref="Interval"/> comes after
        /// (has superior sorting value than) the second Interval, or is equal to it.
        /// <para>The lower bound has first evaluation priority.</para>
        /// </summary>
        /// <param name="a">First interval.</param>
        /// <param name="b">Second interval.</param>
        /// <returns>true if a[0] is larger than b[0], or a[0] == b[0] and a[1] is larger than or equal to b[1]; otherwise, false.</returns>
        public static bool operator >=(Interval a, Interval b)
        {
            return a.CompareTo(b) >= 0;
        }
        #endregion

        #region Constants
        // David thinks: This is not really "empty" is it? Empty would be {0,0}.
        /////<summary>Sets interval to (Utility.UnsetValue, Utility.UnsetValue)</summary>
        //public static Interval Empty
        //{
        //  get { return new Interval(Utility.UnsetValue, Utility.UnsetValue); }
        //}

        /// <summary>
        /// Gets an Interval whose limits are Utility.UnsetValue.
        /// </summary>
        public static Interval Unset
        {
            get
            {
                return new Interval(Utility.UnsetValue, Utility.UnsetValue);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the lower bound of the Interval.
        /// </summary>
        public double T0 { get { return m_t0; } set { m_t0 = value; } }

        /// <summary>
        /// Gets or sets the upper bound of the Interval.
        /// </summary>
        public double T1 { get { return m_t1; } set { m_t1 = value; } }

        /// <summary>
        /// Gets or sets the indexed bound of this Interval.
        /// </summary>
        /// <param name="index">Bound index (0 = lower; 1 = upper).</param>
        public double this[int index]
        {
            get
            {
                if (0 == index) { return m_t0; }
                if (1 == index) { return m_t1; }

                // IronPython works with indexing is we thrown an IndexOutOfRangeException
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (0 == index) { m_t0 = value; }
                else if (1 == index) { m_t1 = value; }
                else { throw new IndexOutOfRangeException(); }
            }
        }

        /// <summary>
        /// Gets the smaller of T0 and T1.
        /// </summary>
        public double Min
        {
            get { return ((Utility.IsValidDouble(m_t0) && Utility.IsValidDouble(m_t1)) ? (m_t0 <= m_t1 ? m_t0 : m_t1) : Utility.UnsetValue); }
        }

        /// <summary>
        /// Gets the larger of T0 and T1.
        /// </summary>
        public double Max
        {
            get { return ((Utility.IsValidDouble(m_t0) && Utility.IsValidDouble(m_t1)) ? (m_t0 <= m_t1 ? m_t1 : m_t0) : Utility.UnsetValue); }
        }

        /// <summary>
        /// Gets the average of T0 and T1.
        /// </summary>
        /// <example>
        /// <code source='examples\vbnet\ex_extendcurve.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_extendcurve.cs' lang='cs'/>
        /// <code source='examples\py\ex_extendcurve.py' lang='py'/>
        /// </example>
        public double Mid
        {
            get { return ((Utility.IsValidDouble(m_t0) && Utility.IsValidDouble(m_t1)) ? ((m_t0 == m_t1) ? m_t0 : (0.5 * (m_t0 + m_t1))) : Utility.UnsetValue); }
        }

        /// <summary>
        /// Gets the signed length of the numeric range. 
        /// If the interval is decreasing, a negative length will be returned.
        /// </summary>
        public double Length
        {
            get { return ((Utility.IsValidDouble(m_t0) && Utility.IsValidDouble(m_t1)) ? m_t1 - m_t0 : 0.0); }
        }

        /// <summary>
        /// Gets a value indicating whether or not this Interval is valid. 
        /// Valid intervals must contain valid numbers.
        /// </summary>
        public bool IsValid
        {
            get { return Utility.IsValidDouble(m_t0) && Utility.IsValidDouble(m_t1); }
        }

        // If we decide that Interval.Empty should indeed be replaced with Interval.Unset, this function becomes pointless
        /////<summary>Returns true if T[0] == T[1] == ON.UnsetValue.</summary>
        //public bool IsEmpty
        //{
        //  get { return (Utility.UnsetValue == m_t0 && Utility.UnsetValue == m_t1); }
        //}

        /// <summary>
        /// Returns true if T0 == T1 != ON.UnsetValue.
        /// </summary>
        public bool IsSingleton
        {
            get { return (Utility.IsValidDouble(m_t0) && m_t0 == m_t1); }
        }

        /// <summary>
        /// Returns true if T0 &lt; T1.
        /// </summary>
        public bool IsIncreasing
        {
            get { return (Utility.IsValidDouble(m_t0) && m_t0 < m_t1); }
        }

        /// <summary> 
        /// Returns true if T[0] &gt; T[1].
        /// </summary>
        public bool IsDecreasing
        {
            get { return (Utility.IsValidDouble(m_t1) && m_t1 < m_t0); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Computes the hash code for this <see cref="Interval" /> object.
        /// </summary>
        /// <returns>A hash value that might be equal for two different <see cref="Interval" /> values.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_t0.GetHashCode() ^ m_t1.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to the current <see cref="Interval" />,
        /// comparing by value.
        /// </summary>
        /// <param name="obj">The other object to compare with.</param>
        /// <returns>true if obj is an <see cref="Interval" /> and has the same bounds; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Interval && this == (Interval)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Interval" /> is equal to the current <see cref="Interval" />,
        /// comparing by value.
        /// </summary>
        /// <param name="other">The other interval to compare with.</param>
        /// <returns>true if obj is an <see cref="Interval" /> and has the same bounds; false otherwise.</returns>
        public bool Equals(Interval other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares this <see cref="Interval" /> with another interval.
        /// <para>The lower bound has first evaluation priority.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Interval" /> to compare with.</param>
        ///<returns>
        ///<para> 0: if this is identical to other</para>
        ///<para>-1: if this[0] &lt; other[0]</para>
        ///<para>+1: if this[0] &gt; other[0]</para>
        ///<para>-1: if this[0] == other[0] and this[1] &lt; other[1]</para>
        ///<para>+1: if this[0] == other[0] and this[1] &gt; other[1]</para>.</returns>
        public int CompareTo(Interval other)
        {
            if (m_t0 < other.m_t0)
                return -1;
            if (m_t0 > other.m_t0)
                return 1;
            if (m_t1 < other.m_t1)
                return -1;
            if (m_t1 > other.m_t1)
                return 1;
            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Interval)
                return CompareTo((Interval)obj);

            throw new ArgumentException("Input must be of type Interval", "obj");
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Interval" />.
        /// </summary>
        /// <returns>A string with T0,T1.</returns>
        public override string ToString()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return String.Format("{0},{1}", m_t0.ToString(culture), m_t1.ToString(culture));
        }

        /// <summary>
        /// Grows the <see cref="Interval" /> to include the given number.
        /// </summary>
        /// <param name="value">Number to include in this interval.</param>
        public void Grow(double value)
        {
            if (!Utility.IsValidDouble(value)) { return; }

            if (IsDecreasing) { Swap(); }
            if (m_t0 > value) { m_t0 = value; }
            if (m_t1 < value) { m_t1 = value; }
        }

        /// <summary>
        /// Ensures this <see cref="Interval" /> is either singleton or increasing.
        /// </summary>
        public void MakeIncreasing()
        {
            if (IsDecreasing) { Swap(); }
        }

        /// <summary>
        /// Changes interval to [-T1, -T0].
        /// </summary>
        public void Reverse()
        {
            if (IsValid)
            {
                double temp = m_t0;
                m_t0 = -m_t1;
                m_t1 = -temp;
            }
        }

        /// <summary>
        /// Exchanges T0 and T1.
        /// </summary>
        public void Swap()
        {
            double temp = m_t0;
            m_t0 = m_t1;
            m_t1 = temp;
        }

        #region Evaluation
        ///<summary>Converts normalized parameter to interval value, or pair of values.</summary>
        ///<returns>Interval parameter min*(1.0-normalizedParameter) + max*normalizedParameter.</returns>
        ///<seealso>NormalizedParameterAt</seealso>
        public double ParameterAt(double normalizedParameter)
        {
            return (Utility.IsValidDouble(normalizedParameter) ? ((1.0 - normalizedParameter) * m_t0 + normalizedParameter * m_t1) : Utility.UnsetValue);
        }

        ///<summary>Converts normalized parameter to interval value, or pair of values.</summary>
        ///<returns>Interval parameter min*(1.0-normalizedParameter) + max*normalized_paramete.</returns>
        ///<seealso>NormalizedParameterAt</seealso>
        public Interval ParameterIntervalAt(Interval normalizedInterval)
        {
            double t0 = ParameterAt(normalizedInterval.m_t0);
            double t1 = ParameterAt(normalizedInterval.m_t1);
            return new Interval(t0, t1);
        }

        ///<summary>Converts interval value, or pair of values, to normalized parameter.</summary>
        ///<returns>Normalized parameter x so that min*(1.0-x) + max*x = intervalParameter.</returns>
        ///<seealso>ParameterAt</seealso>
        public double NormalizedParameterAt(double intervalParameter)
        {
            double x;
            if (Utility.IsValidDouble(intervalParameter))
            {
                if (m_t0 != m_t1)
                {
                    x = (intervalParameter == m_t1) ? 1.0 : (intervalParameter - m_t0) / (m_t1 - m_t0);
                }
                else
                    x = m_t0;
            }
            else
            {
                x = Utility.UnsetValue;
            }
            return x;
        }

        ///<summary>Converts interval value, or pair of values, to normalized parameter.</summary>
        ///<returns>Normalized parameter x so that min*(1.0-x) + max*x = intervalParameter.</returns>
        ///<seealso>ParameterAt</seealso>
        public Interval NormalizedIntervalAt(Interval intervalParameter)
        {
            double t0 = NormalizedParameterAt(intervalParameter.m_t0);
            double t1 = NormalizedParameterAt(intervalParameter.m_t1);
            return new Interval(t0, t1);
        }

        /// <summary>
        /// Tests a parameter for Interval inclusion.
        /// </summary>
        /// <param name="t">Parameter to test.</param>
        /// <returns>true if t is contained within or is coincident with the limits of this Interval.</returns>
        public bool IncludesParameter(double t)
        {
            return IncludesParameter(t, false);
        }
        /// <summary>
        /// Tests a parameter for Interval inclusion.
        /// </summary>
        /// <param name="t">Parameter to test.</param>
        /// <param name="strict">If true, the parameter must be fully on the inside of the Interval.</param>
        /// <returns>true if t is contained within the limits of this Interval.</returns>
        public bool IncludesParameter(double t, bool strict)
        {
            if (!Utility.IsValidDouble(t)) { return false; }
            if (strict)
            {
                if ((m_t0 <= m_t1) && (m_t0 < t) && (t < m_t1)) { return true; }
                if ((m_t1 <= m_t0) && (m_t1 < t) && (t < m_t0)) { return true; }
            }
            else
            {
                if ((m_t0 <= m_t1) && (m_t0 <= t) && (t <= m_t1)) { return true; }
                if ((m_t1 <= m_t0) && (m_t1 <= t) && (t <= m_t0)) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Tests another interval for Interval inclusion.
        /// </summary>
        /// <param name="interval">Interval to test.</param>
        /// <returns>true if the other interval is contained within or is coincident with the limits of this Interval; otherwise false.</returns>
        public bool IncludesInterval(Interval interval)
        {
            return IncludesInterval(interval, false);
        }
        /// <summary>
        /// Tests another interval for Interval inclusion.
        /// </summary>
        /// <param name="interval">Interval to test.</param>
        /// <param name="strict">If true, the other interval must be fully on the inside of the Interval.</param>
        /// <returns>true if the other interval is contained within the limits of this Interval; otherwise false.</returns>
        public bool IncludesInterval(Interval interval, bool strict)
        {
            return (IncludesParameter(interval.m_t0, strict) && IncludesParameter(interval.m_t1, strict));
        }

        #endregion
        #endregion

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Interval other, double epsilon)
        {
            return Utility.EpsilonEquals(m_t0, other.m_t0, epsilon) &&
                   Utility.EpsilonEquals(m_t1, other.m_t1, epsilon);
        }
    }
}
