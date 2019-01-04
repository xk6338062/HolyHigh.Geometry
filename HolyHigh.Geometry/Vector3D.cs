
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the three components of a vector in three-dimensional space,
    /// using <see cref="double"/>-precision floating point numbers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 24)]
    [DebuggerDisplay("({m_x}, {m_y}, {m_z})")]
    [Serializable]
    public struct Vector3D : ISerializable, IEquatable<Vector3D>, IComparable<Vector3D>, IComparable, IEpsilonComparable<Vector3D>
    {
        #region fields
        private double m_x;
        private double m_y;
        private double m_z;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of a vector, using its three components.
        /// </summary>
        /// <param name="x">The X (first) component.</param>
        /// <param name="y">The Y (second) component.</param>
        /// <param name="z">The Z (third) component.</param>
        public Vector3D(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        /// <summary>
        /// Initializes a new instance of a vector, copying the three components from the three coordinates of a point.
        /// </summary>
        /// <param name="point">The point to copy from.</param>
        public Vector3D(Point3D point)
        {
            m_x = point.X;
            m_y = point.Y;
            m_z = point.Z;
        }

        /// <summary>
        /// Initializes a new instance of a vector, copying the three components from a vector.
        /// </summary>
        /// <param name="vector">A double-precision vector.</param>
        public Vector3D(Vector3D vector)
        {
            m_x = vector.X;
            m_y = vector.Y;
            m_z = vector.Z;
        }

        /// <summary>
        /// Gets the value of the vector with components 0,0,0.
        /// </summary>
        public static Vector3D Zero
        {
            get { return new Vector3D(); }
        }

        /// <summary>
        /// Gets the value of the vector with components 1,0,0.
        /// </summary>
        public static Vector3D XAxis
        {
            get { return new Vector3D(1.0, 0.0, 0.0); }
        }

        /// <summary>
        /// Gets the value of the vector with components 0,1,0.
        /// </summary>
        public static Vector3D YAxis
        {
            get { return new Vector3D(0.0, 1.0, 0.0); }
        }

        /// <summary>
        /// Gets the value of the vector with components 0,0,1.
        /// </summary>
        public static Vector3D ZAxis
        {
            get { return new Vector3D(0.0, 0.0, 1.0); }
        }

        /// <summary>
        /// Gets the value of the vector with each component set to Utility.UnsetValue.
        /// </summary>
        public static Vector3D Unset
        {
            get { return new Vector3D(Utility.UnsetValue, Utility.UnsetValue, Utility.UnsetValue); }
        }

        private Vector3D(SerializationInfo info, StreamingContext context)
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
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3D operator *(Vector3D vector, double t)
        {
            return new Vector3D(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3D Multiply(Vector3D vector, double t)
        {
            return new Vector3D(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="t">A number.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3D operator *(double t, Vector3D vector)
        {
            return new Vector3D(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
        /// </summary>
        /// <param name="t">A number.</param>
        /// <param name="vector">A vector.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector3D Multiply(double t, Vector3D vector)
        {
            return new Vector3D(vector.X * t, vector.Y * t, vector.Z * t);
        }

        /// <summary>
        /// Divides a <see cref="Vector3D"/> by a number, having the effect of shrinking it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is componentwise divided by t.</returns>
        public static Vector3D operator /(Vector3D vector, double t)
        {
            return new Vector3D(vector.X / t, vector.Y / t, vector.Z / t);
        }

        /// <summary>
        /// Divides a <see cref="Vector3D"/> by a number, having the effect of shrinking it.
        /// <para>(Provided for languages that do not support operator overloading. You can use the / operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is componentwise divided by t.</returns>
        public static Vector3D Divide(Vector3D vector, double t)
        {
            return new Vector3D(vector.X / t, vector.Y / t, vector.Z / t);
        }

        /// <summary>
        /// Sums up two vectors.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise addition of the two vectors.</returns>
        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>
        /// Sums up two vectors.
        /// <para>(Provided for languages that do not support operator overloading. You can use the + operator otherwise)</para>
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise addition of the two vectors.</returns>
        public static Vector3D Add(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>
        /// Subtracts the second vector from the first one.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise difference of vector1 - vector2.</returns>
        public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        /// <summary>
        /// Subtracts the second vector from the first one.
        /// <para>(Provided for languages that do not support operator overloading. You can use the - operator otherwise)</para>
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise difference of vector1 - vector2.</returns>
        public static Vector3D Subtract(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        /// <summary>
        /// Multiplies two vectors together, returning the dot product (or inner product).
        /// This differs from the cross product.
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>
        /// A value that results from the evaluation of v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z.
        /// <para>This value equals v1.Length * v2.Length * cos(alpha), where alpha is the angle between vectors.</para>
        /// </returns>
        public static double operator *(Vector3D vector1, Vector3D vector2)
        {
            return (vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z);
        }

        /// <summary>
        /// Multiplies two vectors together, returning the dot product (or inner product).
        /// This differs from the cross product.
        /// <para>(Provided for languages that do not support operator overloading. You can use the * operator otherwise)</para>
        /// </summary>
        /// <param name="vector1">A vector.</param>
        /// <param name="vector2">A second vector.</param>
        /// <returns>
        /// A value that results from the evaluation of v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z.
        /// <para>This value equals v1.Length * v2.Length * cos(alpha), where alpha is the angle between vectors.</para>
        /// </returns>
        public static double Multiply(Vector3D vector1, Vector3D vector2)
        {
            return (vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z);
        }

        /// <summary>
        /// Computes the opposite vector.
        /// </summary>
        /// <param name="vector">A vector to negate.</param>
        /// <returns>A new vector where all components were multiplied by -1.</returns>
        public static Vector3D operator -(Vector3D vector)
        {
            return new Vector3D(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// Computes the opposite vector.
        /// <para>(Provided for languages that do not support operator overloading. You can use the - unary operator otherwise)</para>
        /// </summary>
        /// <param name="vector">A vector to negate.</param>
        /// <returns>A new vector where all components were multiplied by -1.</returns>
        public static Vector3D Negate(Vector3D vector)
        {
            return new Vector3D(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// Determines whether two vectors have the same value.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if all coordinates are pairwise equal; false otherwise.</returns>
        public static bool operator ==(Vector3D a, Vector3D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        /// <summary>
        /// Determines whether two vectors have different values.
        /// </summary>
        /// <param name="a">A vector.</param>
        /// <param name="b">Another vector.</param>
        /// <returns>true if any coordinate pair is different; false otherwise.</returns>
        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        /// <summary>
        /// Computes the cross product (or vector product, or exterior product) of two vectors.
        /// <para>This operation is not commutative.</para>
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>A new vector that is perpendicular to both a and b,
        /// <para>has Length == a.Length * b.Length and</para>
        /// <para>with a result that is oriented following the right hand rule.</para>
        /// </returns>
        public static Vector3D CrossProduct(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.Y * b.Z - b.Y * a.Z, a.Z * b.X - b.Z * a.X, a.X * b.Y - b.X * a.Y);
        }

        public Vector3D Cross(Vector3D v)
        {
            return CrossProduct(this, v);
        }

        /// <summary>
        /// Compute the angle between two vectors.
        /// <para>This operation is commutative.</para>
        /// </summary>
        /// <param name="a">First vector for angle.</param>
        /// <param name="b">Second vector for angle.</param>
        /// <returns>If the input is valid, the angle (in radians) between a and b; Utility.UnsetValue otherwise.</returns>
        public static double VectorAngle(Vector3D a, Vector3D b)
        {
            if (!a.Normalize() || !b.Normalize())
                return Utility.UnsetValue;

            //compute dot product
            double dot = a * b;
            // remove any "noise"
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            return Math.Acos(dot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The range is [0,π]</returns>
        public double AngleWith(Vector3D v)
        {
            return VectorAngle(this, v);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns>The range is [0,π/2]</returns>
        public double AngleBetween(Vector3D v)
        {
            var angle = VectorAngle(this, v);
            if (angle > Math.PI / 2)
                angle = Math.PI - angle;
            return angle;
        }
        /// <summary>
        /// Computes the angle on a plane between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <param name="plane">Two-dimensional plane on which to perform the angle measurement.</param>
        /// <returns>On success, the angle (in radians) between a and b as projected onto the plane; Utility.UnsetValue on failure.</returns>
        public static double VectorAngle(Vector3D a, Vector3D b, Plane plane)
        {
            { // Project vectors onto plane.
                Point3D pA = plane.Origin + a;
                Point3D pB = plane.Origin + b;

                pA = plane.ClosestPointTo(pA);
                pB = plane.ClosestPointTo(pB);

                a = pA - plane.Origin;
                b = pB - plane.Origin;
            }

            // Abort on invalid cases.
            if (!a.Normalize()) { return Utility.UnsetValue; }
            if (!b.Normalize()) { return Utility.UnsetValue; }

            double dot = a * b;
            { // Limit dot product to valid range.
                if (dot >= 1.0)
                { dot = 1.0; }
                else if (dot < -1.0)
                { dot = -1.0; }
            }

            double angle = Math.Acos(dot);
            { // Special case (anti)parallel vectors.
                if (Math.Abs(angle) < 1e-64) { return 0.0; }
                if (Math.Abs(angle - Math.PI) < 1e-64) { return Math.PI; }
            }

            Vector3D cross = CrossProduct(a, b);
            if (plane.ZAxis.IsParallelTo(cross) == +1)
                return angle;
            return 2.0 * Math.PI - angle;
        }

        /// <summary>
        /// Determines whether the first specified vector comes before (has inferior sorting value than) the second vector.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z;
        /// otherwise, false.</returns>
        public static bool operator <(Vector3D a, Vector3D b)
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
        /// Determines whether the first specified vector comes before
        /// (has inferior sorting value than) the second vector, or it is equal to it.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is smaller than b.X,
        /// or a.X == b.X and a.Y is smaller than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &lt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator <=(Vector3D a, Vector3D b)
        {
            return a.CompareTo(b) <= 0;
        }

        /// <summary>
        /// Determines whether the first specified vector comes after (has superior sorting value than)
        /// the second vector.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z;
        /// otherwise, false.</returns>
        public static bool operator >(Vector3D a, Vector3D b)
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
        /// Determines whether the first specified vector comes after (has superior sorting value than)
        /// the second vector, or it is equal to it.
        /// <para>Components evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if a.X is larger than b.X,
        /// or a.X == b.X and a.Y is larger than b.Y,
        /// or a.X == b.X and a.Y == b.Y and a.Z &gt;= b.Z;
        /// otherwise, false.</returns>
        public static bool operator >=(Vector3D a, Vector3D b)
        {
            return a.CompareTo(b) >= 0;
        }
                

        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the X (first) component of the vector.
        /// </summary>
        public double X { get { return m_x; } set { m_x = value; } }
        /// <summary>
        /// Gets or sets the Y (second) component of the vector.
        /// </summary>
        public double Y { get { return m_y; } set { m_y = value; } }
        /// <summary>
        /// Gets or sets the Z (third) component of the vector.
        /// </summary>
        public double Z { get { return m_z; } set { m_z = value; } }

        /// <summary>
        /// Gets or sets a vector component at the given index.
        /// </summary>
        /// <param name="index">Index of vector component. Valid values are: 
        /// <para>0 = X-component</para>
        /// <para>1 = Y-component</para>
        /// <para>2 = Z-component</para>
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
                // IronPython works with indexing when we thrown an IndexOutOfRangeException
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
        /// Gets a value indicating whether this vector is valid. 
        /// A valid vector must be formed of valid component values for x, y and z.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Utility.IsValidDouble(m_x) &&
                       Utility.IsValidDouble(m_y) &&
                       Utility.IsValidDouble(m_z);
            }
        }

        /// <summary>
        /// Gets the smallest (both positive and negative) component value in this vector.
        /// </summary>
        public double MinimumCoordinate
        {
            get
            {
                Point3D p = new Point3D(this);
                return p.MinimumCoordinate;
            }
        }

        /// <summary>
        /// Gets the largest (both positive and negative) component value in this vector.
        /// </summary>
        public double MaximumCoordinate
        {
            get
            {
                Point3D p = new Point3D(this);
                return p.MaximumCoordinate;
            }
        }

        /// <summary>
        /// Computes the length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// If this vector is invalid, its length is considered 0.
        /// </summary>
        public double Length
        {
            get { return GetLengthHelper(m_x, m_y, m_z); }
        }

        /// <summary>
        /// Computes the squared length (or magnitude, or size) of this vector.
        /// This is an application of Pythagoras' theorem.
        /// While the Length property checks for input validity,
        /// this property does not. You should check validity in advance,
        /// if this vector can be invalid.
        /// </summary>
        public double SquaredLength
        {
            get { return (m_x * m_x) + (m_y * m_y) + (m_z * m_z); }
        }
        /// <summary>
        /// Gets a value indicating whether or not this is a unit vector. 
        /// A unit vector has length 1.
        /// </summary>
        public bool IsUnitVector
        {
            get
            {
                // checks for invalid values and returns 0.0 if there are any
                double length = GetLengthHelper(m_x, m_y, m_z);
                return Math.Abs(length - 1.0) <= Utility.SQRT_EPSILON;
            }
        }

        /// <summary>
        /// Determines whether a vector is very short.
        /// </summary>
        /// <param name="tolerance">
        /// A nonzero value used as the coordinate zero tolerance.
        /// .</param>
        /// <returns>(Math.Abs(X) &lt;= tiny_tol) AND (Math.Abs(Y) &lt;= tiny_tol) AND (Math.Abs(Z) &lt;= tiny_tol).</returns>
        public bool IsTiny(double tolerance)
        {
            return (Math.Abs(m_x) <= tolerance &&
                Math.Abs(m_y) <= tolerance &&
                Math.Abs(m_z) <= tolerance);
        }

        /// <summary>
        /// Uses Utility.ZeroTolerance for IsTiny calculation.
        /// </summary>
        /// <returns>true if vector is very small, otherwise false.</returns>
        public bool IsTiny()
        {
            return IsTiny(Utility.ZeroTolerance);
        }


        /// <summary>
        /// Gets a value indicating whether the X, Y, and Z values are all equal to 0.0.
        /// </summary>
        public bool IsZero
        {
            get
            {
                return (m_x == 0.0 && m_y == 0.0 && m_z == 0.0);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Determines whether the specified System.Object is a Vector3D and has the same values as the present vector.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is a Vector3D and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3D && this == (Vector3D)obj);
        }

        /// <summary>
        /// Determines whether the specified vector has the same value as the present vector.
        /// </summary>
        /// <param name="vector">The specified vector.</param>
        /// <returns>true if vector has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Vector3D vector)
        {
            return this == vector;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Vector3D other, double epsilon)
        {
            return Utility.EpsilonEquals(m_x, other.X, epsilon) &&
                   Utility.EpsilonEquals(m_y, other.Y, epsilon) &&
                   Utility.EpsilonEquals(m_z, other.Z, epsilon);
        }

        /// <summary>
        /// Compares this <see cref="Vector3D" /> with another <see cref="Vector3D" />.
        /// <para>Component evaluation priority is first X, then Y, then Z.</para>
        /// </summary>
        /// <param name="other">The other <see cref="Vector3D" /> to use in comparison.</param>
        /// <returns>
        /// <para> 0: if this is identical to other</para>
        /// <para>-1: if this.X &lt; other.X</para>
        /// <para>-1: if this.X == other.X and this.Y &lt; other.Y</para>
        /// <para>-1: if this.X == other.X and this.Y == other.Y and this.Z &lt; other.Z</para>
        /// <para>+1: otherwise.</para>
        /// </returns>
        public int CompareTo(Vector3D other)
        {
            if (m_x < other.X)
                return -1;
            if (m_x > other.X)
                return 1;

            if (m_y < other.Y)
                return -1;
            if (m_y > other.Y)
                return 1;

            if (m_z < other.Z)
                return -1;
            if (m_z > other.Z)
                return 1;

            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj is Vector3D)
                return CompareTo((Vector3D)obj);

            throw new ArgumentException("Input must be of type Vector3D", "obj");
        }

        /// <summary>
        /// Computes the hash code for the current vector.
        /// </summary>
        /// <returns>A non-unique number that represents the components of this vector.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_x.GetHashCode() ^ m_y.GetHashCode() ^ m_z.GetHashCode();
        }

        /// <summary>
        /// Returns the string representation of the current vector, in the form X,Y,Z.
        /// </summary>
        /// <returns>A string with the current location of the point.</returns>
        public override string ToString()
        {
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            return String.Format("{0},{1},{2}",
              m_x.ToString(culture), m_y.ToString(culture), m_z.ToString(culture));
        }

        /// <summary>
        /// Unitizes the vector in place. A unit vector has length 1 unit. 
        /// <para>An invalid or zero length vector cannot be unitized.</para>
        /// </summary>
        /// <returns>true on success or false on failure.</returns>
        public bool Normalize()
        {
            double d = Length;
            if (d > Utility.POS_MIN_DBL)
            {
                m_x /= d;
                m_y /= d;
                m_z /= d;
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
                Vector3D tmp;
                tmp.m_x = m_x * 8.9884656743115795386465259539451e+307;
                tmp.m_y = m_y * 8.9884656743115795386465259539451e+307;
                tmp.m_z = m_z * 8.9884656743115795386465259539451e+307;
                d = tmp.Length;
                if (d > Utility.POS_MIN_DBL)
                {
                    m_x = tmp.m_x / d;
                    m_y = tmp.m_y / d;
                    m_z = tmp.m_z / d;
                    return true && IsValid;
                }
            }
            m_x = 0.0;
            m_y = 0.0;
            m_z = 0.0;
            return false;
        }

        /// <summary>
        /// Transforms the vector in place.
        /// <para>The transformation matrix acts on the left of the vector; i.e.,</para>
        /// <para>result = transformation*vector</para>
        /// </summary>
        /// <param name="xform">Transformation matrix to apply.</param>
        public void TransformBy(Transform xform)
        {
            double xx = xform.M00 * m_x + xform.M01 * m_y + xform.M02 * m_z;
            double yy = xform.M10 * m_x + xform.M11 * m_y + xform.M12 * m_z;
            double zz = xform.M20 * m_x + xform.M21 * m_y + xform.M22 * m_z;

            m_x = xx;
            m_y = yy;
            m_z = zz;
        }

        /// <summary>
        /// Rotates this vector around a given axis.
        /// </summary>
        /// <param name="angleRadians">Angle of rotation (in radians).</param>
        /// <param name="rotationAxis">Axis of rotation.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angleRadians, Vector3D rotationAxis)
        {
            if (Utility.IsValidDouble(angleRadians) && rotationAxis.IsValid)
            {
                var rot = Geometry.Transform.Rotation(angleRadians, rotationAxis, Point3D.Origin);
                TransformBy(rot);
                return true;
            }
            return false;
        }


        ///<summary>
        /// Reverses (inverts) this vector in place.
        /// <para>If this vector is Invalid, no changes will occur and false will be returned.</para>
        ///</summary>
        ///<returns>true on success or false if the vector is invalid.</returns>
        public bool Reverse()
        {
            if (!IsValid)
                return false;
            m_x = -m_x;
            m_y = -m_y;
            m_z = -m_z;
            return true;
        }

        internal static bool IsOrthonormalFrame(Vector3D x, Vector3D y, Vector3D z)
        {
            // returns true if X, Y, Z is an orthonormal frame
            if (!IsOrthogonalFrame(x, y, z))
                return false;
            double a = x.Length;
            if (Math.Abs(a - 1.0) > Utility.SQRT_EPSILON)
                return false;
            a = y.Length;
            if (Math.Abs(a - 1.0) > Utility.SQRT_EPSILON)
                return false;
            a = z.Length;
            if (Math.Abs(a - 1.0) > Utility.SQRT_EPSILON)
                return false;

            return true;
        }

        internal static bool IsOrthogonalFrame(Vector3D x, Vector3D y, Vector3D z)
        {
            // returns true if X, Y, Z is an orthogonal frame
            if (!x.IsValid || !y.IsValid || !z.IsValid)
                return false;

            double lx = x.Length;
            double ly = y.Length;
            double lz = z.Length;
            if (lx <= Utility.SQRT_EPSILON)
                return false;
            if (ly <= Utility.SQRT_EPSILON)
                return false;
            if (lz <= Utility.SQRT_EPSILON)
                return false;
            lx = 1.0 / lx;
            ly = 1.0 / ly;
            lz = 1.0 / lz;
            double xy = (x.X * y.X + x.Y * y.Y + x.Z * y.Z) * lx * ly;
            double yz = (y.X * z.X + y.Y * z.Y + y.Z * z.Z) * ly * lz;
            double zx = (z.X * x.X + z.Y * x.Y + z.Z * x.Z) * lz * lx;
            if (Math.Abs(xy) > Utility.SQRT_EPSILON
                 || Math.Abs(yz) > Utility.SQRT_EPSILON
                 || Math.Abs(zx) > Utility.SQRT_EPSILON
               )
            {
                double t = 0.0000152587890625;
                if (Math.Abs(xy) >= t || Math.Abs(yz) >= t || Math.Abs(zx) >= t)
                    return false;

                // do a more careful (and time consuming check)
                // This fixes RR 22219 and 22276
                Vector3D v;
                v = (lx * ly) * x.Cross(y);
                t = Math.Abs((v.X * z.X + v.Y * z.Y + v.Z * z.Z) * lz);
                if (Math.Abs(t - 1.0) > Utility.SQRT_EPSILON)
                    return false;

                v = (ly * lz) * y.Cross(z);
                t = Math.Abs((v.X * x.X + v.Y * x.Y + v.Z * x.Z) * lx);
                if (Math.Abs(t - 1.0) > Utility.SQRT_EPSILON)
                    return false;

                v = (lz * lx) * z.Cross(x);
                t = Math.Abs((v.X * y.X + v.Y * y.Y + v.Z * y.Z) * ly);
                if (Math.Abs(t - 1.0) > Utility.SQRT_EPSILON)
                    return false;
            }
            return true;
        }

        internal static bool IsRightHandFrame(Vector3D x, Vector3D y, Vector3D z)
        {
            // returns true if X, Y, Z is an orthonormal right hand frame
            if (!IsOrthonormalFrame(x, y, z))
                return false;
            double a = x.Cross(y) * z;
            if (a <= Utility.SQRT_EPSILON)
                return false;
            return true;
        }

        /// <summary>
        /// Determines whether this vector is parallel to another vector, within one degree (within Pi / 180). 
        /// </summary>
        /// <param name="other">Vector to use for comparison.</param>
        /// <returns>
        /// Parallel indicator:
        /// <para>+1 = both vectors are parallel</para>
        /// <para> 0 = vectors are not parallel, or at least one of the vectors is zero</para>
        /// <para>-1 = vectors are anti-parallel.</para>
        /// </returns>
        public int IsParallelTo(Vector3D other)
        {
            return IsParallelTo(other, Utility.DefaultAngleTolerance);
        }

        /// <summary>
        /// Determines whether this vector is parallel to another vector, within a provided tolerance. 
        /// </summary>
        /// <param name="other">Vector to use for comparison.</param>
        /// <param name="angleTolerance">Angle tolerance (in radians).</param>
        /// <returns>
        /// Parallel indicator:
        /// <para>+1 = both vectors are parallel.</para>
        /// <para>0 = vectors are not parallel or at least one of the vectors is zero.</para>
        /// <para>-1 = vectors are anti-parallel.</para>
        /// </returns>
        public int IsParallelTo(Vector3D other, double angleTolerance)
        {
            int rc = 0;
            double ll = Length * other.Length;
            if (ll > 0.0)
            {
                double cos_angle = (m_x * other.m_x + m_y * other.m_y + m_z * other.m_z) / ll;
                double cos_tol = Math.Cos(angleTolerance);
                if (cos_angle >= cos_tol)
                    rc = 1;
                else if (cos_angle <= -cos_tol)
                    rc = -1;
            }
            return rc;
        }

        ///<summary>
        /// Test to see whether this vector is perpendicular to within one degree of another one. 
        ///</summary>
        /// <param name="other">Vector to compare to.</param>
        ///<returns>true if both vectors are perpendicular, false if otherwise.</returns>
        public bool IsPerpendicularTo(Vector3D other)
        {
            return IsPerpendicularTo(other, Utility.DefaultAngleTolerance);
        }

        ///<summary>
        /// Determines whether this vector is perpendicular to another vector, within a provided angle tolerance. 
        ///</summary>
        /// <param name="other">Vector to use for comparison.</param>
        /// <param name="angleTolerance">Angle tolerance (in radians).</param>
        ///<returns>true if vectors form Pi-radians (90-degree) angles with each other; otherwise false.</returns>
        public bool IsPerpendicularTo(Vector3D other, double angleTolerance)
        {
            bool rc = false;
            double ll = Length * other.Length;
            if (ll > 0.0)
            {
                if (Math.Abs((m_x * other.X + m_y * other.Y + m_z * other.Z) / ll) <= Math.Sin(angleTolerance))
                    rc = true;
            }
            return rc;
        }

        public bool PerpendicularTo(Point3D p0, Point3D p1, Point3D p2)
        {
            this = Zero;
            Vector3D V0 = p2 - p1;
            Vector3D V1 = p0 - p2;
            Vector3D V2 = p1 - p0;
            Vector3D N0 = V1.Cross(V2);
            if (!N0.Normalize())
                return false;
            Vector3D N1 = V2.Cross(V0);
            if (!N1.Normalize())
                return false;
            Vector3D N2 = V0.Cross(V1);
            if (!N2.Normalize())
                return false;
            double s0 = 1.0 / V0.Length;
            double s1 = 1.0 / V1.Length;
            double s2 = 1.0 / V2.Length;

            // choose normal with smallest total error
            double e0 = s0 * Math.Abs((N0 * V0)) + s1 * Math.Abs((N0 * V1)) + s2 * Math.Abs((N0 * V2));
            double e1 = s0 * Math.Abs((N1 * V0)) + s1 * Math.Abs((N1 * V1)) + s2 * Math.Abs((N1 * V2));
            double e2 = s0 * Math.Abs((N2 * V0)) + s1 * Math.Abs((N2 * V1)) + s2 * Math.Abs((N2 * V2));

            if (e0 <= e1)
            {
                if (e0 <= e2)
                {
                    this = N0;
                }
                else
                {
                    this = N2;
                }
            }
            else if (e1 <= e2)
            {
                this = N1;
            }
            else
            {
                this = N2;
            }

            return true;
        }

        ///<summary>
        /// Sets this vector to be perpendicular to another vector. 
        /// Result is not unitized.
        ///</summary>
        /// <param name="other">Vector to use as guide.</param>
        ///<returns>true on success, false if input vector is zero or invalid.</returns>
        public bool PerpendicularTo(Vector3D other)
        {
            int i, j, k;
            double a, b;
            k = 2;
            if (Math.Abs(other.m_y) > Math.Abs(other.m_x))
            {
                if (Math.Abs(other.m_z) > Math.Abs(other.m_y))
                {
                    // |other.z| > |other.y| > |other.x|
                    i = 2;
                    j = 1;
                    k = 0;
                    a = other.m_z;
                    b = -other.m_y;
                }
                else if (Math.Abs(other.m_z) >= Math.Abs(other.m_x))
                {
                    // |other.y| >= |other.z| >= |other.x|
                    i = 1;
                    j = 2;
                    k = 0;
                    a = other.m_y;
                    b = -other.m_z;
                }
                else
                {
                    // |other.y| > |other.x| > |other.z|
                    i = 1;
                    j = 0;
                    k = 2;
                    a = other.m_y;
                    b = -other.m_x;
                }
            }
            else if (Math.Abs(other.m_z) > Math.Abs(other.m_x))
            {
                // |other.z| > |other.x| >= |other.y|
                i = 2;
                j = 0;
                k = 1;
                a = other.m_z;
                b = -other.m_x;
            }
            else if (Math.Abs(other.m_z) > Math.Abs(other.m_y))
            {
                // |other.x| >= |other.z| > |other.y|
                i = 0;
                j = 2;
                k = 1;
                a = other.m_x;
                b = -other.m_z;
            }
            else
            {
                // |other.x| >= |other.y| >= |other.z|
                i = 0;
                j = 1;
                k = 2;
                a = other.m_x;
                b = -other.m_y;
            }
            this[i] = b;
            this[j] = a;
            this[k] = 0.0;
            return (a != 0.0) ? true : false;
        }
        internal static double GetLengthHelper(double dx, double dy, double dz)
        {
            if (!Utility.IsValidDouble(dx) ||
                !Utility.IsValidDouble(dy) ||
                !Utility.IsValidDouble(dz))
                return 0.0;

            double len;
            double fx = Math.Abs(dx);
            double fy = Math.Abs(dy);
            double fz = Math.Abs(dz);
            if (fy >= fx && fy >= fz)
            {
                len = fx; fx = fy; fy = len;
            }
            else if (fz >= fx && fz >= fy)
            {
                len = fx; fx = fz; fz = len;
            }

            // 15 September 2003 Dale Lear
            //     For small denormalized doubles (positive but smaller
            //     than DBL_MIN), some compilers/FPUs set 1.0/fx to +INF.
            //     Without the ON_DBL_MIN test we end up with
            //     microscopic vectors that have infinite length!
            //
            //     Since this code starts with floats, none of this
            //     should be necessary, but it doesn't hurt anything.
            const double ON_DBL_MIN = 2.2250738585072014e-308;
            if (fx > ON_DBL_MIN)
            {
                len = 1.0 / fx;
                fy *= len;
                fz *= len;
                len = fx * Math.Sqrt(1.0 + fy * fy + fz * fz);
            }
            else if (fx > 0.0 && Utility.IsValidDouble(fx))
                len = fx;
            else
                len = 0.0;
            return len;
        }
        #endregion
    }
}
