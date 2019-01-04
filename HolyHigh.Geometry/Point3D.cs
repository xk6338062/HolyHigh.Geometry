using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the three coordinates of a point in three-dimensional space,
    /// using <see cref="double"/>-precision floating point values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
    [DebuggerDisplay("({m_x}, {m_y}, {m_z})")]
    [Serializable]
    public struct Point3D : ISerializable, IEquatable<Point3D>, IComparable<Point3D>, IComparable, IEpsilonComparable<Point3D>
    {
        #region members
        private double m_x;
        private double m_y;
        private double m_z;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new point by defining the X, Y and Z coordinates.
        /// </summary>
        /// <param name="x">The value of the X (first) coordinate.</param>
        /// <param name="y">The value of the Y (second) coordinate.</param>
        /// <param name="z">The value of the Z (third) coordinate.</param>
        public Point3D(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from the components of a vector.
        /// </summary>
        /// <param name="vector">A vector.</param>
        public Point3D(Vector3D vector)
        {
            m_x = vector.X;
            m_y = vector.Y;
            m_z = vector.Z;
        }

        /// <summary>
        /// Initializes a new point by copying coordinates from another point.
        /// </summary>
        /// <param name="point">A point.</param>
        public Point3D(Point3D point)
        {
            m_x = point.X;
            m_y = point.Y;
            m_z = point.Z;
        }

        /// <summary>
        /// Gets the value of a point at location (0,0,0).
        /// </summary>
        public static Point3D Origin
        {
            get { return new Point3D(0, 0, 0); }
        }

        /// <summary>
        /// Gets the value of a point at location (<see cref="Utility"/>.UnsetValue,<see cref="Utility"/>.UnsetValue,<see cref="Utility"/>.UnsetValue).
        /// </summary>
        public static Point3D Unset
        {
            get { return new Point3D(Utility.UnsetValue, Utility.UnsetValue, Utility.UnsetValue); }
        }

        private Point3D(SerializationInfo info, StreamingContext context)
        {
            m_x = info.GetDouble("X");
            m_y = info.GetDouble("Y");
            m_z = info.GetDouble("Z");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", m_x);
            info.AddValue("Y", m_y);
            info.AddValue("Z", m_z);
        }

        #endregion

        #region operators

        /// <summary>
        /// Multiplies a <see cref="Point3D"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise multiplied by t.</returns>
        public static Point3D operator *(Point3D point, double t)
        {
            return new Point3D(point.m_x * t, point.m_y * t, point.m_z * t);
        }

        /// <summary>
        /// Multiplies a <see cref="Point3D"/> by a number.
        /// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise multiplied by t.</returns>
        public static Point3D Multiply(Point3D point, double t)
        {
            return new Point3D(point.m_x * t, point.m_y * t, point.m_z * t);
        }

        /// <summary>
        /// Multiplies a <see cref="Point3D"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise multiplied by t.</returns>
        public static Point3D operator *(double t, Point3D point)
        {
            return new Point3D(point.m_x * t, point.m_y * t, point.m_z * t);
        }

        /// <summary>
        /// Multiplies a <see cref="Point3D"/> by a number.
        /// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise multiplied by t.</returns>
        public static Point3D Multiply(double t, Point3D point)
        {
            return new Point3D(point.m_x * t, point.m_y * t, point.m_z * t);
        }

        /// <summary>
        /// Divides a <see cref="Point3D"/> by a number.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise divided by t.</returns>
        public static Point3D operator /(Point3D point, double t)
        {
            return new Point3D(point.m_x / t, point.m_y / t, point.m_z / t);
        }

        /// <summary>
        /// Divides a <see cref="Point3D"/> by a number.
        /// <para>(Provided for languages that do not support operator overloading. You can use the / operator otherwise)</para>
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new point that is coordinatewise divided by t.</returns>
        public static Point3D Divide(Point3D point, double t)
        {
            return new Point3D(point.m_x / t, point.m_y / t, point.m_z / t);
        }

        /// <summary>
        /// Sums two <see cref="Point3D"/> instances.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">A point.</param>
        /// <returns>A new point that results from the addition of point1 and point2.</returns>
        public static Point3D operator +(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.m_x + point2.m_x, point1.m_y + point2.m_y, point1.m_z + point2.m_z);
        }

        /// <summary>
        /// Sums two <see cref="Point3D"/> instances.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">A point.</param>
        /// <returns>A new point that results from the addition of point1 and point2.</returns>
        public static Point3D Add(Point3D point1, Point3D point2)
        {
            return new Point3D(point1.m_x + point2.m_x, point1.m_y + point2.m_y, point1.m_z + point2.m_z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3D operator +(Point3D point, Vector3D vector)
        {
            return new Point3D(point.m_x + vector.X, point.m_y + vector.Y, point.m_z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3D Add(Point3D point, Vector3D vector)
        {
            return new Point3D(point.m_x + vector.X, point.m_y + vector.Y, point.m_z + vector.Z);
        }


        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3D operator +(Vector3D vector, Point3D point)
        {
            return new Point3D(point.m_x + vector.X, point.m_y + vector.Y, point.m_z + vector.Z);
        }

        /// <summary>
        /// Sums up a point and a vector, and returns a new point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that results from the addition of point and vector.</returns>
        public static Point3D Add(Vector3D vector, Point3D point)
        {
            return new Point3D(point.m_x + vector.X, point.m_y + vector.Y, point.m_z + vector.Z);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new point that is the difference of point minus vector.</returns>
        public static Point3D operator -(Point3D point, Vector3D vector)
        {
            return new Point3D(point.m_x - vector.X, point.m_y - vector.Y, point.m_z - vector.Z);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the - operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="point">A point.</param>
        /// <returns>A new point that is the difference of point minus vector.</returns>
        public static Point3D Subtract(Point3D point, Vector3D vector)
        {
            return new Point3D(point.m_x - vector.X, point.m_y - vector.Y, point.m_z - vector.Z);
        }

        /// <summary>
        /// Subtracts a point from another point.
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">Another point.</param>
        /// <returns>A new vector that is the difference of point minus vector.</returns>
        public static Vector3D operator -(Point3D point1, Point3D point2)
        {
            return new Vector3D(point1.m_x - point2.m_x, point1.m_y - point2.m_y, point1.m_z - point2.m_z);
        }

        /// <summary>
        /// Subtracts a point from another point.
        /// <para>(Provided for languages that do not support operator overloading. You can use the - operator otherwise)</para>
        /// </summary>
        /// <param name="point1">A point.</param>
        /// <param name="point2">Another point.</param>
        /// <returns>A new vector that is the difference of point minus vector.</returns>
        public static Vector3D Subtract(Point3D point1, Point3D point2)
        {
            return new Vector3D(point1.m_x - point2.m_x, point1.m_y - point2.m_y, point1.m_z - point2.m_z);
        }

        /// <summary>
        /// Computes the additive inverse of all coordinates in the point, and returns the new point.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>A point value that, when summed with the point input, yields the <see cref="Origin"/>.</returns>
        public static Point3D operator -(Point3D point)
        {
            return new Point3D(-point.m_x, -point.m_y, -point.m_z);
        }

        /// <summary>
        /// Determines whether two Point3D have equal values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the coordinates of the two points are exactly equal; otherwise false.</returns>
        public static bool operator ==(Point3D a, Point3D b)
        {
            return (a.m_x == b.m_x && a.m_y == b.m_y && a.m_z == b.m_z);
        }

        /// <summary>
        /// Determines whether two Point3D have different values.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if the two points differ in any coordinate; false otherwise.</returns>
        public static bool operator !=(Point3D a, Point3D b)
        {
            return (a.m_x != b.m_x || a.m_y != b.m_y || a.m_z != b.m_z);
        }

        /// <summary>
        /// Converts a point in a vector, needing casting.
        /// </summary>
        /// <param name="point">A point.</param>
        /// <returns>The resulting vector.</returns>
        public static explicit operator Vector3D(Point3D point)
        {
            return new Vector3D(point);
        }

        /// <summary>
        /// Converts a vector in a point, needing casting.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <returns>The resulting point.</returns>
        public static explicit operator Point3D(Vector3D vector)
        {
            return new Point3D(vector);
        }

        /// <summary>
        /// Determines whether the first specified point comes before (has inferior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
        /// otherwise, false.</returns>
        public static bool operator <(Point3D a, Point3D b)
        {
            if (a.X < b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                    return true;
                if (a.Y == b.Y && a.Z < b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes before
        /// (has inferior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator <=(Point3D a, Point3D b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified point comes after (has superior sorting value than) the second point.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
        /// otherwise, false.</returns>
        public static bool operator >(Point3D a, Point3D b)
        {
            if (a.X > b.X)
                return true;
            if (a.X == b.X)
            {
                if (a.Y > b.Y)
                    return true;
                if (a.Y == b.Y && a.Z > b.Z)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the first specified point comes after
        /// (has superior sorting value than) the second point, or it is equal to it.
        /// <para>Coordinates evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator >=(Point3D a, Point3D b)
        {
            return a.CompareTo(b) >= 0;
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the X (first) coordinate of this point.
        /// </summary>
        public double X { get { return m_x; } set { m_x = value; } }

        /// <summary>
        /// Gets or sets the Y (second) coordinate of this point.
        /// </summary>
        public double Y { get { return m_y; } set { m_y = value; } }

        /// <summary>
        /// Gets or sets the Z (third) coordinate of this point.
        /// </summary>
        public double Z { get { return m_z; } set { m_z = value; } }

        /// <summary>
        /// Gets or sets an indexed coordinate of this point.
        /// </summary>
        /// <param name="index">
        /// The coordinate index. Valid values are:
        /// <para>0 = X coordinate</para>
        /// <para>1 = Y coordinate</para>
        /// <para>2 = Z coordinate</para>
        /// .</param>
        public double this[int index]
        {
            get
            {
                if (0 == index)
                    return m_x;
                if (1 == index)
                    return m_y;
                if (2 == index)
                    return m_z;
                // IronPython works with indexing is we thrown an IndexOutOfRangeException
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (0 == index)
                    m_x = value;
                else if (1 == index)
                    m_y = value;
                else if (2 == index)
                    m_z = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Each coordinate of the point must pass the <see cref="Utility.IsValidDouble"/> test.
        /// </summary>
        public bool IsValid
        {
            get { return Utility.IsValidDouble(m_x) && Utility.IsValidDouble(m_y) && Utility.IsValidDouble(m_z); }
        }

        /// <summary>
        /// Gets the smallest (both positive and negative) coordinate value in this point.
        /// </summary>
        public double MinimumCoordinate
        {
            get
            {
                double c;
                if (Utility.IsValidDouble(m_x))
                {
                    c = System.Math.Abs(m_x);
                    if (Utility.IsValidDouble(m_y) && System.Math.Abs(m_y) < c)
                        c = System.Math.Abs(m_y);
                    if (Utility.IsValidDouble(m_z) && System.Math.Abs(m_z) < c)
                        c = System.Math.Abs(m_z);
                }
                else if (Utility.IsValidDouble(m_y))
                {
                    c = System.Math.Abs(m_y);
                    if (Utility.IsValidDouble(m_z) && System.Math.Abs(m_z) < c)
                        c = System.Math.Abs(m_z);
                }
                else if (Utility.IsValidDouble(m_z))
                {
                    c = System.Math.Abs(m_z);
                }
                else
                    c = Utility.UnsetValue;
                return c;
            }
        }

        /// <summary>
        /// Gets the largest (both positive and negative) valid coordinate in this point,
        /// or Utility.UnsetValue if no coordinate is valid.
        /// </summary>
        public double MaximumCoordinate
        {
            get
            {
                double c;
                if (Utility.IsValidDouble(m_x))
                {
                    c = System.Math.Abs(m_x);
                    if (Utility.IsValidDouble(m_y) && System.Math.Abs(m_y) > c)
                        c = System.Math.Abs(m_y);
                    if (Utility.IsValidDouble(m_z) && System.Math.Abs(m_z) > c)
                        c = System.Math.Abs(m_z);
                }
                else if (Utility.IsValidDouble(m_y))
                {
                    c = System.Math.Abs(m_y);
                    if (Utility.IsValidDouble(m_z) && System.Math.Abs(m_z) > c)
                        c = System.Math.Abs(m_z);
                }
                else if (Utility.IsValidDouble(m_z))
                {
                    c = System.Math.Abs(m_z);
                }
                else
                    c = Utility.UnsetValue;
                return c;
            }
        }


        #endregion

        #region methods
        /// <summary>
        /// Determines whether the specified <see cref="object"/> is a <see cref="Point3D"/> and has the same values as the present point.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is a Point3D and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Point3D && this == (Point3D)obj);
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Point3D other, double epsilon)
        {
            return Utility.EpsilonEquals(m_x, other.m_x, epsilon) &&
                   Utility.EpsilonEquals(m_y, other.m_y, epsilon) &&
                   Utility.EpsilonEquals(m_z, other.m_z, epsilon);
        }

        /// <summary>
        /// Compares this <see cref="Point3D" /> with another <see cref="Point3D" />.
        /// <para>Component evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Point3D" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Point3D other)
        {
            if (m_x < other.m_x)
                return -1;
            if (m_x > other.m_x)
                return 1;

            if (m_y < other.m_y)
                return -1;
            if (m_y > other.m_y)
                return 1;

            if (m_z < other.m_z)
                return -1;
            if (m_z > other.m_z)
                return 1;

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Point3D)
                return CompareTo((Point3D)obj);

            throw new ArgumentException("Input must be of type Point3D", "obj");
        }

        /// <summary>
        /// Determines whether the specified <see cref="Point3D"/> has the same values as the present point.
        /// </summary>
        /// <param name="point">The specified point.</param>
        /// <returns>true if point has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Point3D point)
        {
            return this == point;
        }

        /// <summary>
        /// Computes a hash code for the present point.
        /// </summary>
        /// <returns>A non-unique integer that represents this point.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }

        /// <summary>
        /// Interpolate between two points.
        /// </summary>
        /// <param name="pA">First point.</param>
        /// <param name="pB">Second point.</param>
        /// <param name="t">Interpolation parameter. 
        /// If t=0 then this point is set to pA. 
        /// If t=1 then this point is set to pB. 
        /// Values of t in between 0.0 and 1.0 result in points between pA and pB.</param>
        public void Interpolate(Point3D pA, Point3D pB, double t)
        {
            m_x = pA.m_x + t * (pB.m_x - pA.m_x);
            m_y = pA.m_y + t * (pB.m_y - pA.m_y);
            m_z = pA.m_z + t * (pB.m_z - pA.m_z);
        }

        /// <summary>
        /// Constructs the string representation for the current point.
        /// </summary>
        /// <returns>The point representation in the form X,Y,Z.</returns>
        public override string ToString()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return String.Format("{0},{1},{2}", m_x.ToString(culture), m_y.ToString(culture), m_z.ToString(culture));
        }

        /// <summary>
        /// Computes the distance between two points.
        /// </summary>
        /// <param name="other">Other point for distance measurement.</param>
        /// <returns>The length of the line between this and the other point; or 0 if any of the points is not valid.</returns>
        public double DistanceTo(Point3D other)
        {
            double d;
            if (IsValid && other.IsValid)
            {
                double dx = other.m_x - m_x;
                double dy = other.m_y - m_y;
                double dz = other.m_z - m_z;
                d = Vector3D.GetLengthHelper(dx, dy, dz);
            }
            else
            {
                d = 0.0;
            }
            return d;
        }

        /// <summary>
        /// Transforms the present point in place. The transformation matrix acts on the left of the point. i.e.,
        /// <para>result = transformation*point</para>
        /// </summary>
        /// <param name="xform">Transformation to apply.</param>
        public void TransformBy(Transform xform)
        {
            //David: this method doesn't test for validity. Should it?
            double ww = xform.M30 * m_x + xform.M31 * m_y + xform.M32 * m_z + xform.M33;
            if (ww != 0.0) { ww = 1.0 / ww; }

            double tx = ww * (xform.M00 * m_x + xform.M01 * m_y + xform.M02 * m_z + xform.M03);
            double ty = ww * (xform.M10 * m_x + xform.M11 * m_y + xform.M12 * m_z + xform.M13);
            double tz = ww * (xform.M20 * m_x + xform.M21 * m_y + xform.M22 * m_z + xform.M23);
            m_x = tx;
            m_y = ty;
            m_z = tz;
        }
        #endregion
    }
}
