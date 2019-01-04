using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the two components of a vector in two-dimensional space,
    /// using <see cref="double"/>-precision floating point numbers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 16)]
    [DebuggerDisplay("({m_x}, {m_y})")]
    [Serializable]
    public struct Vector2D : ISerializable, IEquatable<Vector2D>, IComparable<Vector2D>, IComparable, IEpsilonComparable<Vector2D>
    {
        private double m_x;
        private double m_y;

        /// <summary>
        /// Initializes a new instance of the vector based on two, X and Y, components.
        /// </summary>
        /// <param name="x">The X (first) component.</param>
        /// <param name="y">The Y (second) component.</param>
        public Vector2D(double x, double y)
        {
            m_x = x;
            m_y = y;
        }

        private Vector2D(SerializationInfo info, StreamingContext context)
        {
            m_x = info.GetDouble("X");
            m_y = info.GetDouble("Y");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", m_x);
            info.AddValue("Y", m_y);
        }

        /// <summary>
        /// Gets or sets the X (first) component of this vector.
        /// </summary>
        public double X { get { return m_x; } set { m_x = value; } }

        /// <summary>
        /// Gets or sets the Y (second) component of this vector.
        /// </summary>
        public double Y { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// </summary>
        public double Length
        {
            get { return Length2D(m_x, m_y); }
        }

        /// <summary>
        /// 向量的夹角，值域为[0,π]
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public double AngleTo(Vector2D vector)
        {
            if (!(Math.Abs(Length * vector.Length) < Utility.TOL6))
            {
                double result = (this * vector) / (Length * vector.Length);
                double r = result > 1 ? 1 : result;
                double rr = r < -1 ? -1 : r;
                return Math.Acos(rr);
            }
            return 0;
        }

        /// <summary>
        /// 向量source逆时针旋转到终点向量的角度，值域为[0 ,2π)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public double AngleFrom(Vector2D source)
        {
            double angle = AngleTo(source);
            if (Math.Abs(angle) < Utility.TOL6)
                return 0;
            if (Cross(source) > 0)
                return 2 * Math.PI - angle;
            return angle;
        }

        /// <summary>
        /// X轴基向量
        /// </summary>
        public static Vector2D BasisX => new Vector2D(1, 0);
        /// <summary>
        /// Y轴基向量
        /// </summary>
        public static Vector2D BasixY => new Vector2D(0, 1);

        public double LengthSquared
        {
            get { return Length * Length; }
        }
        /// <summary>
        /// 向量叉乘
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Cross(Vector2D v)
        {
            return m_x * v.m_y - m_y * v.m_x;
        }

        #region operators

        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(-v.m_x, -v.m_y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.m_x - v2.m_x, v1.m_y - v2.m_y);
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.m_x + v2.m_x, v1.m_y + v2.m_y);
        }

        public static double operator *(Vector2D v1, Vector2D v2)
        {
            return v1.m_x * v2.m_x + v1.m_y * v2.m_y;
        }

        public static Vector2D operator *(double t, Vector2D v)
        {
            return new Vector2D(t * v.m_x, t * v.m_y);
        }

        public static Vector2D operator *(Vector2D v, double t)
        {
            return new Vector2D(t * v.m_x, t * v.m_y);
        }

        public static Vector2D operator /(Vector2D v, double t)
        {
            return new Vector2D(v.m_x / t, v.m_y / t);
        }


        /// <summary>
        /// Determines whether two vectors have equal values.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if components of the two vectors are pairwise equal; otherwise false.</returns>
        public static bool operator ==(Vector2D a, Vector2D b)
        {
            return a.m_x == b.m_x && a.m_y == b.m_y;
        }

        /// <summary>
        /// Determines whether two vectors have different values.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if any component of the two vectors is pairwise different; otherwise false.</returns>
        public static bool operator !=(Vector2D a, Vector2D b)
        {
            return a.m_x != b.m_x || a.m_y != b.m_y;
        }

        /// <summary>
        /// Determines whether the first specified vector comes before
        /// (has inferior sorting value than) the second vector.
        /// <para>Components have decreasing evaluation priority: first X, then Y.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>true if a.X is smaller than b.X, or a.X == b.X and a.Y is smaller than b.Y; otherwise, false.</returns>
        public static bool operator <(Vector2D a, Vector2D b)
        {
            return (a.X < b.X) || (a.X == b.X && a.Y < b.Y);
        }

        /// <summary>
        /// Determines whether the first specified vector comes before
        /// (has inferior sorting value than) the second vector, or it is equal to it.
        /// <para>Components have decreasing evaluation priority: first X, then Y.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>true if a.X is smaller than b.X, or a.X == b.X and a.Y &lt;= b.Y; otherwise, false.</returns>
        public static bool operator <=(Vector2D a, Vector2D b)
        {
            return (a.X < b.X) || (a.X == b.X && a.Y <= b.Y);
        }

        /// <summary>
        /// Determines whether the first specified vector comes after
        /// (has superior sorting value than) the second vector.
        /// <para>Components have decreasing evaluation priority: first X, then Y.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>true if a.X is larger than b.X, or a.X == b.X and a.Y is larger than b.Y; otherwise, false.</returns>
        public static bool operator >(Vector2D a, Vector2D b)
        {
            return (a.X > b.X) || (a.X == b.X && a.Y > b.Y);
        }

        /// <summary>
        /// Determines whether the first specified vector comes after
        /// (has superior sorting value than) the second vector, or it is equal to it.
        /// <para>Components have decreasing evaluation priority: first X, then Y.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>true if a.X is larger than b.X, or a.X == b.X and a.Y &gt;= b.Y; otherwise, false.</returns>
        public static bool operator >=(Vector2D a, Vector2D b)
        {
            return (a.X > b.X) || (a.X == b.X && a.Y >= b.Y);
        }
        #endregion

        /// <summary>
        /// Determines whether the specified System.Object is a Vector2D and has the same value as the present vector.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is Vector2D and has the same components as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector2D && this == (Vector2D)obj);
        }

        /// <summary>
        /// Determines whether the specified vector has the same value as the present vector.
        /// </summary>
        /// <param name="vector">The specified vector.</param>
        /// <returns>true if vector has the same components as this; otherwise false.</returns>
        public bool Equals(Vector2D vector)
        {
            return this == vector;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Vector2D other, double epsilon)
        {
            return Utility.EpsilonEquals(m_x, other.m_x, epsilon) &&
                   Utility.EpsilonEquals(m_y, other.m_y, epsilon);
        }

        /// <summary>
        /// Compares this <see cref="Vector2D" /> with another <see cref="Vector2D" />.
        /// <para>Components evaluation priority is first X, then Y.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Vector2D" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Vector2D other)
        {
            if (m_x < other.m_x)
                return -1;
            if (m_x > other.m_x)
                return 1;

            if (m_y < other.m_y)
                return -1;
            if (m_y > other.m_y)
                return 1;

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Vector2D)
                return CompareTo((Vector2D)obj);

            throw new ArgumentException("Input must be of type Vector2D", "obj");
        }

        /// <summary>
        /// Provides a hashing value for the present vector.
        /// </summary>
        /// <returns>A non-unique number based on vector components.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_x.GetHashCode() ^ m_y.GetHashCode();
        }

        /// <summary>
        /// Constructs a string representation of the current vector.
        /// </summary>
        /// <returns>A string in the form X,Y.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0},{1}", X, Y);
        }

        /// <summary>
        /// Gets the value of the vector with components 0,0.
        /// </summary>
        public static Vector2D Zero
        {
            get { return new Vector2D(); }
        }

        /// <summary>
        /// Gets the value of the vector with components set as Utility.UnsetValue,Utility.UnsetValue.
        /// </summary>
        public static Vector2D Unset
        {
            get { return new Vector2D(Utility.UnsetValue, Utility.UnsetValue); }
        }

        /// <summary>
        /// Gets a value indicating whether this vector is valid. 
        /// A valid vector must be formed of valid component values for x, y and z.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Utility.IsValidDouble(m_x) &&
                       Utility.IsValidDouble(m_y);
            }
        }

        /// <summary>
        /// Unitizes the vector in place. A unit vector has length 1 unit. 
        /// <para>An invalid or zero length vector cannot be unitized.</para>
        /// </summary>
        /// <returns>true on success or false on failure.</returns>
        public bool Normalize()
        {
            double d = Length;
            if (Utility.IsValidDouble(d))
            {
                if (d > Utility.POS_MIN_DBL)
                {
                    m_x /= d;
                    m_y /= d;
                    return true && IsValid;
                }
                if (d > 0.0)
                {
                    // This code is rarely used and can be slow.
                    // It multiplies by 2^1023 in an attempt to 
                    // normalize the coordinates.
                    // If the renormalization works, then we're
                    // ok.  If the renormalization fails, we
                    // return false.
                    Vector2D tmp;
                    tmp.m_x = m_x * 8.9884656743115795386465259539451e+307;
                    tmp.m_y = m_y * 8.9884656743115795386465259539451e+307;
                    d = tmp.Length;
                    if (Utility.IsValidDouble(d) && d > Utility.POS_MIN_DBL)
                    {
                        m_x = tmp.m_x / d;
                        m_y = tmp.m_y / d;
                        return true && IsValid;
                    }
                }
            }
            m_x = 0.0;
            m_y = 0.0;
            return false;
        }
        private double Length2D(double x, double y)
        {
            double len;
            x = Math.Abs(x);
            y = Math.Abs(y);
            if (y > x)
            {
                len = x; x = y; y = len;
            }
            //     For small denormalized doubles (positive but smaller
            //     than DBL_MIN), some compilers/FPUs set 1.0/fx to +INF.
            //     Without the ON_DBL_MIN test we end up with
            //     microscopic vectors that have infinte length!
            //
            //     This code is absolutely necessary.  It is a critical
            //     part of the bug fix for RR 11217.
            if (x > Utility.POS_MIN_DBL)
            {
                y /= x;
                len = x * Math.Sqrt(1.0 + y * y);
            }
            else if (x > 0.0 && Utility.IsValidDouble(x))
                len = x;
            else
                len = 0.0;
            return len;
        }
    }
}
