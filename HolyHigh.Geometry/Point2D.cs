using System;
using System.Xml;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;

namespace HolyHigh.Geometry
{
    /// <summary>
    ///  Represents the two coordinates of a point in two-dimensional space,
    /// using <see cref="double"/>-precision floating point values.
    /// </summary>
    [DebuggerDisplay("({m_x},{m_y})")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Size = 16, Pack = 8)]
    public struct Point2D : ISerializable, IEquatable<Point2D>, IComparable<Point2D>, IComparable, IEpsilonComparable<Point2D>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Point2D"/> from coordinates.
        /// </summary>
        /// <param name="x">The X (first) coordinate</param>
        /// <param name="y">The Y (second) coordinate</param>
        public Point2D(double x, double y)
        {
            this.m_x = x;
            this.m_y = y;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="Point2D"/> by converting a verctor.
        /// </summary>
        /// <param name="vector">The vector that will be copied</param>
        public Point2D(Vector2D vector)
        {
            m_x = vector.X;
            m_y = vector.Y;
        }
        /// <summary>
        /// Initializes a new instance of <see cref="Point2D"/> by copying another <see cref="Point2D"/>
        /// </summary>
        /// <param name="point">The point that will be copied</param>
        public Point2D(Point2D point)
        {
            m_x = point.X;
            m_y = point.Y;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="Point2D"/> by copying ths first two coordinates of a <see cref="Point3D"/>
        /// </summary>
        /// <param name="point">The point that will be used: the Z (third) coordinate is discarded</param>
        public Point2D(Point3D point)
        {
            m_x = point.X;
            m_y = point.Y;
        }

        private double m_x;
        private double m_y;
        /// <summary>
        /// Gets or sets the X (first) coordinate of the point 
        /// </summary>
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        /// <summary>
        /// Gets or sets the Y (second) coordinate of the point 
        /// </summary>
        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
        /// <summary>
        /// Accesses the coordinates of this point
        /// </summary>
        /// <param name="index">Either 0 or 1</param>
        /// <returns>If index is 0, the X (first) coordinate. If index is 1, the Y (second) coordinate</returns>
        public double this[int index]
        {
            get
            {
                if (index == 0)
                    return m_x;
                if (1 == index)
                    return m_y;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index == 0)
                    m_x = value;
                else
                {
                    if (1 != index)
                        throw new IndexOutOfRangeException();
                    m_y = value;
                }
            }
        }

        ///<summary>
        /// If any coordinate of a point is UnsetValue, then the point is not valid.
        ///</summary>
        public bool IsValid
        {
            get
            {
                if (Utility.IsValidDouble(X))
                    return Utility.IsValidDouble(Y);
                return false;
            }
        }
        /// <summary>
        /// Gets the smallest (both positive and negative) valid coordinate, or RhinoMath.UnsetValue if no coordinate is valid.
        /// </summary>
        public double MinimumCoordinate
        {
            get
            {
                double num;
                if (Utility.IsValidDouble(X))
                {
                    num = Math.Abs(X);
                    if (Utility.IsValidDouble(Y) && Math.Abs(Y) < num)
                        num = Math.Abs(Y);
                }
                else
                    num = !Utility.IsValidDouble(Y) ? -1.23432101234321E+308 : Math.Abs(Y);
                return num;
            }
        }
        /// <summary>
        /// Gets the largest valid coordinate, or RhinoMath.UnsetValue if no coordinate is valid.
        /// </summary>
        public double MaximumCoordinate
        {
            get
            {
                double num;
                if (Utility.IsValidDouble(X))
                {
                    num = Math.Abs(X);
                    if (Utility.IsValidDouble(Y) && Math.Abs(Y) > num)
                        num = Math.Abs(Y);
                }
                else
                    num = !Utility.IsValidDouble(Y) ? -1.23432101234321E+308 : Math.Abs(Y);
                return num;
            }
        }

        public static readonly Point2D Empty = new Point2D();


        /// <summary>
        /// Gets a point at (0,0).
        /// </summary>
        public static Point2D Origin
        {
            get
            {
                return new Point2D(0.0, 0.0);
            }
        }
        /// <summary>
        /// Gets a point at (<see cref="Utility"/>.UnsetValue,<see cref="Utility"/>.UnsetValue).
        /// </summary>
        public static Point2D Unset
        {
            get { return new Point2D(Utility.UnsetValue, Utility.UnsetValue); }
        }

        public static Point2D MinValue
        {
            get
            {
                return new Point2D(double.MinValue, double.MinValue);
            }
        }

        public static Point2D MaxValue
        {
            get
            {
                return new Point2D(double.MaxValue, double.MaxValue);
            }
        }

        public static Point2D operator -(Point2D a, Vector2D b)
        {
            return new Point2D()
            {
                X = a.X - b.X,
                Y = a.Y - b.Y
            };
        }

        public static Point2D operator +(Point2D a, Vector2D b)
        {
            return new Point2D()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }
        public static Vector2D operator -(Point2D a, Point2D b)
        {
            return new Vector2D
            {
                X = a.X - b.X,
                Y = a.Y - b.Y
            };
        }

        public static Point2D operator *(double s, Point2D p)
        {
            return new Point2D
            {
                X = s * p.X,
                Y = s * p.Y
            };
        }

        public static Point2D operator *(Point2D p, double s)
        {
            return s * p;
        }

        public static Point2D operator /(Point2D p, double s)
        {
            return new Point2D
            {
                X = p.X / s,
                Y = p.Y / s
            };
        }

        public static bool operator ==(Point2D left, Point2D right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Point2D left, Point2D right)
        {
            return !Equals(left, right);
        }
        public static bool operator <(Point2D a, Point2D b)
        {
            return (a.X < b.X) || (a.X == b.X && a.Y < b.Y);
        }
        public static bool operator <=(Point2D a, Point2D b)
        {
            return (a.X < b.X) || (a.X == b.X && a.Y <= b.Y);
        }
        public static bool operator >(Point2D a, Point2D b)
        {
            return (a.X > b.X) || (a.X == b.X && a.Y > b.Y);
        }
        public static bool operator >=(Point2D a, Point2D b)
        {
            return (a.X > b.X) || (a.X == b.X && a.Y >= b.Y);
        }
        public bool Equals(Point2D other)
        {
            return (X == other.X && Y == other.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is Point2D && Equals((Point2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() * 397 ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", X, Y);
        }
        public double DistanceTo(Point2D other)
        {
            double d;
            if (IsValid && other.IsValid)
            {
                Vector2D v = other - this;
                d = v.Length;
            }
            else
            {
                d = 0.0;
            }
            return d;
        }

        public double DistanceSquared(Point2D a)
        {
            double x = X - a.X;
            double y = Y - a.Y;
            return x * x + y * y;
        }

        public static Point2D ReadXml(string pointString)
        {
            string[] array = pointString.Split(new char[] {
                ','
            });
            return new Point2D(Convert.ToDouble(array[0]), Convert.ToDouble(array[1]));
        }
        public static Point2D ReadXml(XmlNode node)
        {
            return ReadXml(node.InnerText);
        }

        public int CompareTo(Point2D other)
        {
            if (m_x < other.m_x)
                return -1;
            if (m_x > other.m_x)
                return 1;
            if (m_y < other.m_y)
                return -1;
            return m_y > other.m_y ? 1 : 0;
        }

        public int CompareTo(object obj)
        {
            if (obj is Point2D)
                return CompareTo((Point2D)obj);
            throw new ArgumentException("Input must be of type Point2D", "obj");
        }

        public bool EpsilonEquals(Point2D other, double epsilon)
        {
            if (Utility.EpsilonEquals(X, other.X, epsilon))
                return Utility.EpsilonEquals(Y, other.Y, epsilon);
            return false;
        }
        /// <summary>
        /// 在误差(1e-9)范围内相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsAlmostEqualTo(Point2D other)
        {
            return EpsilonEquals(other, Utility.TOL9);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", m_x);
            info.AddValue("Y", m_y);
        }
    }
}