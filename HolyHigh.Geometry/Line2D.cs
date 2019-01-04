using System;
using System.Runtime.InteropServices;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the value of start and end points in a two-dimensional space line segment, using <see cref="double"/>-precision floating point numbers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 32)]
    [Serializable]
    public struct Line2D : IEquatable<Line2D>, IEpsilonComparable<Line2D>
    {

        /// <summary>
        /// 起点
        /// </summary>
        public Point2D Start { get; set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point2D End { get; set; }

        public Line2D(Point2D start, Point2D end)
        {
            this.Start = start;
            this.End = end;
        }

        public Line2D(double x1, double y1, double x2, double y2)
        {
            Start = new Point2D(x1, y1);
            End = new Point2D(x2, y2);
        }


        public Vector2D Direction
        {
            get
            {
                var v = End - Start;
                v.Normalize();
                return v;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this line is valid. 
        /// Valid lines must have valid start and end points.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Start.IsValid && End.IsValid && Start != End;
            }
        }

        /// <summary>
        /// Gets or sets the length of this line segment. 
        /// Note that a negative length will invert the line segment without 
        /// making the actual length negative. The line Start point will remain fixed 
        /// when a new Length is set.
        /// </summary>
        public double Length
        {
            get { return Start.DistanceTo(End); }
            set
            {
                Vector2D dir = End - Start;
                if (!dir.Normalize())
                    dir = new Vector2D(0, 1);
                End = Start + dir * value;
            }
        }

        /// <summary>
        /// Finds the parameter on the infinite line segment that is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to project onto the line.</param>
        /// <returns>The parameter on the line that is closest to testPoint.</returns>
        public double ClosestParameter(Point2D testPoint)
        {
            Vector2D v = End - Start;
            double lengthSquared = v.LengthSquared;
            Vector2D v1 = testPoint - Start;
            Vector2D v2 = testPoint - End;
            double result = 0.0;
            if (lengthSquared > 0.0)
            {
                if (v1.LengthSquared <= v2.LengthSquared)
                {
                    result = v1 * v / lengthSquared;
                }
                else
                {
                    result = 1.0 + v2 * v / lengthSquared;
                }
            }
            return result;
        }


        /// <summary>
        /// Finds the point on the (in)finite line segment that is closest to a test point.
        /// </summary>
        /// <param name="point">Point to project onto the line.</param>
        /// <param name="limitEndFiniteSegment">If true, the projection is limited to the finite line segment.</param>
        /// <returns>The point on the (in)finite line that is closest to testPoint.</returns>
        public Point2D ClosestPoint(Point2D point, bool limitEndFiniteSegment)
        {
            double t = ClosestParameter(point);
            if (limitEndFiniteSegment)
            {
                t = Math.Max(t, 0.0);
                t = Math.Min(t, 1.0);
            }

            return PointAt(t);
        }
        /// <summary>
        /// Evaluates the line at the specified parameter.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>(1-t)*start + t*end</returns>
        public Point2D PointAt(double t)
        {
            double num = 1.0 - t;
            return new Point2D((Start.X == End.X) ? Start.X : (num * Start.X + t * End.X), (Start.Y == End.Y) ? Start.Y : (num * Start.Y + t * End.Y));
        }

        /// <summary>
        /// 点到直线的投影距离
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double DistanceTo(Point2D point)
        {
            var projectPoint = ClosestPoint(point, false);
            return projectPoint.DistanceTo(point);
        }

        /// <summary>
        /// Intersects two lines
        /// </summary>
        /// <param name="line"></param>
        /// <param name="isSegment">is line segment or not</param>
        /// <param name="epsilon">error</param>
        /// <returns></returns>
        public Point2D? Intersect(Line2D line, bool isSegment = true, double epsilon = Utility.EPSILON)
        {
            epsilon = epsilon < 0 ? Utility.EPSILON : epsilon;
            double rxs = (End - Start).Cross(line.End - line.Start);
            if (Math.Abs(rxs) < epsilon) return null;
            double r = (line.Start - Start).Cross(line.End - line.Start) / rxs;
            var point = PointAt(r);
            var t = ClosestParameter(point);
            var u = line.ClosestParameter(point);
            bool result = t >= -epsilon && t <= 1 + epsilon && u > -epsilon && u <= 1 + epsilon;
            if (isSegment)
            {
                if (result) return point;
                return null;
            }
            return point;

        }

        /// <summary>
        /// 偏移直线
        /// </summary>
        /// <param name="direction">偏移的方向向量</param>
        /// <param name="distance">偏移距离</param>
        /// <returns>返回偏移后的直线</returns>
        public Line2D Offset(Vector2D direction, double distance)
        {
            direction.Normalize();
            var p1 = Start + direction * distance;
            var p2 = End + direction * distance;
            return new Line2D(p1, p2);
        }
        /// <summary>
        /// 偏移直线
        /// </summary>
        /// <param name="vector">偏移的向量</param>
        /// <returns>返回偏移后的直线</returns>
        public Line2D Offset(Vector2D vector)
        {
            return new Line2D(Start + vector, End + vector);
        }

        /// <summary>
        /// 对直线首尾进行延长
        /// </summary>
        /// <param name="startLength">正值延长，负值缩短</param>
        /// <param name="endLength">正值延长，负值缩短</param>
        /// <returns></returns>
        public bool Extend(double startLength, double endLength)
        {
            if (!IsValid) { return false; }
            if (Length == 0.0) { return false; }
            Start -= startLength * Direction;
            End += endLength * Direction;
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Start.ToString(), End.ToString());
        }
        public bool EpsilonEquals(Line2D other, double epsilon)
        {
            return Start.EpsilonEquals(other.Start, epsilon) &&
       End.EpsilonEquals(other.End, epsilon);
        }

        public bool Equals(Line2D other)
        {
            return this == other;
        }
        /// <summary>
        /// Determines whether two lines have the same value.
        /// </summary>
        /// <param name="a">A line.</param>
        /// <param name="b">Another line.</param>
        /// <returns>true if a has the same coordinates as b; otherwise false.</returns>
        public static bool operator ==(Line2D a, Line2D b)
        {
            return a.Start == b.Start && a.End == b.End;
        }

        /// <summary>
        /// Determines whether two lines have different values.
        /// </summary>
        /// <param name="a">A line.</param>
        /// <param name="b">Another line.</param>
        /// <returns>true if a has any coordinate that distinguishes it from b; otherwise false.</returns>
        public static bool operator !=(Line2D a, Line2D b)
        {
            return a.Start != b.Start || a.End != b.End;
        }
        public override bool Equals(object obj)
        {
            return obj is Line2D && this == (Line2D)obj;
        }
        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }
    }
}
