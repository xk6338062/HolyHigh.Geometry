using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    /// <summary>
    ///  Represents the value of start and end points in a three-dimensional space line segment, using <see cref="double"/>-precision floating point numbers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 48)]
    [Serializable]
    public struct Line : IEquatable<Line>, IEpsilonComparable<Line>
    {
        #region members
        private Point3D start;
        private Point3D end;

        /// <summary>
        /// Start point of line segment.
        /// </summary>
        public Point3D Start
        {
            get { return start; }
            set { start = value; }
        }
        /// <summary>
        /// Gets or sets the X coordinate of the line Start point.
        /// </summary>
        public double StartX
        {
            get { return start.X; }
            set { start.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y coordinate of the line Start point.
        /// </summary>
        public double StartY
        {
            get { return start.Y; }
            set { start.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Z coordinate of the line Start point.
        /// </summary>
        public double StartZ
        {
            get { return start.Z; }
            set { start.Z = value; }
        }

        /// <summary>
        /// End point of line segment.
        /// </summary>
        public Point3D End
        {
            get { return end; }
            set { end = value; }
        }
        /// <summary>
        /// Gets or sets the X coordinate of the line End point.
        /// </summary>
        public double EndX
        {
            get { return end.X; }
            set { end.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y coordinate of the line End point.
        /// </summary>
        public double EndY
        {
            get { return end.Y; }
            set { end.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Z coordinate of the line End point.
        /// </summary>
        public double EndZ
        {
            get { return end.Z; }
            set { end.Z = value; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Constructs a new line segment between two points.
        /// </summary>
        /// <param name="from">Start point of line.</param>
        /// <param name="to">End point of line.</param>
        public Line(Point3D from, Point3D to)
        {
            start = from;
            end = to;
        }

        /// <summary>
        /// Constructs a new line segment from start point and span vector.
        /// </summary>
        /// <param name="start">Start point of line segment.</param>
        /// <param name="span">Direction and length of line segment.</param>
        public Line(Point3D start, Vector3D span)
        {
            this.start = start;
            end = start + span;
        }

        /// <summary>
        /// Constructs a new line segment from start point, direction and length.
        /// </summary>
        /// <param name="start">Start point of line segment.</param>
        /// <param name="direction">Direction of line segment.</param>
        /// <param name="length">Length of line segment.</param>
        public Line(Point3D start, Vector3D direction, double length)
        {
            Vector3D dir = direction;
            if (!dir.Normalize())
                dir = new Vector3D(0, 0, 1);

            this.start = start;
            end = start + dir * length;
        }

        /// <summary>
        /// Constructs a new line segment between two points.
        /// </summary>
        /// <param name="x0">The X coordinate of the first point.</param>
        /// <param name="y0">The Y coordinate of the first point.</param>
        /// <param name="z0">The Z coordinate of the first point.</param>
        /// <param name="x1">The X coordinate of the second point.</param>
        /// <param name="y1">The Y coordinate of the second point.</param>
        /// <param name="z1">The Z coordinate of the second point.</param>
        public Line(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            start = new Point3D(x0, y0, z0);
            end = new Point3D(x1, y1, z1);
        }
        #endregion

        #region constants
        /// <summary>
        /// Gets a line segment which has <see cref="Point3D.Unset"/> end points.
        /// </summary>
        public static Line Unset
        {
            get { return new Line(Point3D.Unset, Point3D.Unset); }
        }
        #endregion

        #region properties
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
                Vector3D dir = End - Start;
                if (!dir.Normalize())
                    dir = new Vector3D(0, 0, 1);
                End = Start + dir * value;
            }
        }

        /// <summary>
        /// Gets the direction of this line segment.The direction vectors are always unit vectors
        /// </summary>
        public Vector3D Direction
        {
            get
            {
                var v = End - Start;
                v.Normalize();
                return v;
            }
        }

        /// <summary>
        /// Gets the line's 3d axis aligned bounding box.
        /// </summary>
        /// <returns>3d bounding box.</returns>
        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox rc = new BoundingBox(Start, End);
                rc.MakeValid();
                return rc;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Determines whether an object is a line that has the same value as this line.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>true if obj is a Line and has the same coordinates as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Line && this == (Line)obj;
        }

        /// <summary>
        /// Determines whether a line has the same value as this line.
        /// </summary>
        /// <param name="other">A line.</param>
        /// <returns>true if other has the same coordinates as this; otherwise false.</returns>
        public bool Equals(Line other)
        {
            return this == other;
        }

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Line other, double epsilon)
        {
            return start.EpsilonEquals(other.start, epsilon) &&
                   end.EpsilonEquals(other.end, epsilon);
        }

        /// <summary>
        /// Computes a hash number that represents this line.
        /// </summary>
        /// <returns>A number that is not unique to the value of this line.</returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        /// <summary>
        /// Contructs the string representation of this line, in the form "Start,End".
        /// </summary>
        /// <returns>A text string.</returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", Start.ToString(), End.ToString());
        }

        /// <summary>
        /// Flip the endpoints of the line segment.
        /// </summary>
        public void Flip()
        {
            Point3D temp = Start;
            Start = End;
            End = temp;
        }

        /// <summary>
        /// Evaluates the line at the specified parameter.
        /// </summary>
        /// <param name="t">Parameter to evaluate line segment at. Line parameters are normalised parameters.</param>
        /// <returns>The point at the specified parameter.</returns>
        public Point3D PointAt(double t)
        {
            double s = 1.0 - t;
            return new Point3D((Start.X == End.X) ? Start.X : s * Start.X + t * End.X,
                               (Start.Y == End.Y) ? Start.Y : s * Start.Y + t * End.Y,
                               (Start.Z == End.Z) ? Start.Z : s * Start.Z + t * End.Z);
        }

        /// <summary>
        /// Finds the parameter on the infinite line segment that is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to project onto the line.</param>
        /// <returns>The parameter on the line that is closest to testPoint.</returns>
        public double ClosestParameter(Point3D testPoint)
        {
            double rc = 0.0;
            if (Direction.SquaredLength > 0)
            {
                if (testPoint.DistanceTo(start) <= testPoint.DistanceTo(end))
                {
                    rc = (testPoint - start) * Direction / Direction.SquaredLength;
                }
                else
                    rc = 1 + (testPoint - end) * Direction / Direction.SquaredLength;
            }
            return rc;
        }

        /// <summary>
        /// Finds the point on the (in)finite line segment that is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to project onto the line.</param>
        /// <param name="limitEndFiniteSegment">If true, the projection is limited to the finite line segment.</param>
        /// <returns>The point on the (in)finite line that is closest to testPoint.</returns>
        public Point3D ClosestPoint(Point3D testPoint, bool limitEndFiniteSegment)
        {
            double t = ClosestParameter(testPoint);

            if (limitEndFiniteSegment)
            {
                t = Math.Max(t, 0.0);
                t = Math.Min(t, 1.0);
            }

            return PointAt(t);
        }

        /// <summary>
        /// Compute the shortest distance between this line segment and a test point.
        /// </summary>
        /// <param name="testPoint">Point for distance computation.</param>
        /// <param name="limitEndFiniteSegment">If true, the distance is limited to the finite line segment.</param>
        /// <returns>The shortest distance between this line segment and testPoint.</returns>
        public double DistanceEnd(Point3D testPoint, bool limitEndFiniteSegment)
        {
            Point3D pp = ClosestPoint(testPoint, limitEndFiniteSegment);
            return pp.DistanceTo(testPoint);
        }
        /// <summary>
        /// Finds the shortest distance between this line as a finite segment
        /// and a test point.
        /// </summary>
        /// <param name="testPoint">A point to test.</param>
        /// <returns>The minimum distance.</returns>
        public double MinimumDistanceEnd(Point3D testPoint)
        {
            Point3D pp = ClosestPoint(testPoint, true);
            return pp.DistanceTo(testPoint);
        }

        /// <summary>
        /// Transform the line using a Transformation matrix.
        /// </summary>
        /// <param name="xform">Transform to apply to this line.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Transform(Transform xform)
        {
            start = xform * start;
            end = xform * end;
            return true;
        }


        //David: all extend methods are untested as of yet.

        /// <summary>
        /// Extend the line by custom distances on both sides.
        /// </summary>
        /// <param name="startLength">
        /// Distance to extend the line at the start point. 
        /// Positive distance result in longer lines.
        /// </param>
        /// <param name="endLength">
        /// Distance to extend the line at the end point. 
        /// Positive distance result in longer lines.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public bool Extend(double startLength, double endLength)
        {
            if (!IsValid) { return false; }
            if (Length == 0.0) { return false; }
            start -= startLength * Direction;
            end += endLength * Direction;
            return true;
        }

        /// <summary>
        /// Ensure the line extends all the way through a box. 
        /// Note, this does not result in the shortest possible line 
        /// that overlaps the box.
        /// </summary>
        /// <param name="box">Box to extend through.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool ExtendThroughBox(BoundingBox box)
        {
            if (!IsValid) { return false; }
            if (!box.IsValid) { return false; }

            return ExtendThroughPointSet(box.GetCorners(), 0.0);
        }
        /// <summary>
        /// Ensure the line extends all the way through a box. 
        /// Note, this does not result in the shortest possible line that overlaps the box.
        /// </summary>
        /// <param name="box">Box to extend through.</param>
        /// <param name="additionalLength">Additional length to append at both sides of the line.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool ExtendThroughBox(BoundingBox box, double additionalLength)
        {
            if (!IsValid) { return false; }
            if (!box.IsValid) { return false; }

            return ExtendThroughPointSet(box.GetCorners(), additionalLength);
        }
        private bool ExtendThroughPointSet(IEnumerable<Point3D> pts, double additionalLength)
        {
            Vector3D unit = Direction;
            if (!unit.IsValid) { return false; }

            double t0 = double.MaxValue;
            double t1 = double.MinValue;

            foreach (Point3D pt in pts)
            {
                double t = ClosestParameter(pt);
                t0 = Math.Min(t0, t);
                t1 = Math.Max(t1, t);
            }

            if (t0 <= t1)
            {
                Point3D A = PointAt(t0) - (additionalLength * unit);
                Point3D B = PointAt(t1) + (additionalLength * unit);
                start = A;
                end = B;
            }
            else
            {
                Point3D A = PointAt(t0) + (additionalLength * unit);
                Point3D B = PointAt(t1) - (additionalLength * unit);
                start = A;
                end = B;
            }

            return true;
        }
        /// <summary>
        /// 返回两条 <see cref="Line"/> 的交点
        /// </summary>
        /// <param name="line"></param>
        /// <param name="isSegment">是否为线段</param>
        /// <param name="epsilon">误差值</param>
        /// <returns></returns>
        public Point3D? Intersect(Line line, bool isSegment = true, double epsilon = Utility.EPSILON)
        {
            epsilon = epsilon <= 0 ? Utility.EPSILON : epsilon;
            var a = 0.0;
            var b = 0.0;
            var isIntersect = LineLine(this, line, out a, out b, epsilon);
            if (!isIntersect) return null;
            var isOnLine = a >= -epsilon && a <= 1+ epsilon && b >= -epsilon && b <= 1+ epsilon;
            if (isSegment && !isOnLine) return null;
            var p1 = PointAt(a);
            var p2 = PointAt(b);
            var dis = p1.DistanceTo(p2);
            if (dis < epsilon) return p1;
            return null;
        }

        private static bool LineLine(Line lineA, Line lineB, out double a, out double b, double epslion = Utility.EPSILON)
        {
            a = b = 0;
            if (lineA.Length < epslion) return false;
            if (lineB.Length < epslion) return false;
            var p13 = lineA.start - lineB.start;
            var p43 = lineB.end - lineB.start;
            var p21 = lineA.end - lineA.start;
            var d1343 = p13 * p43;
            var d4321 = p43 * p21;
            var d1321 = p13 * p21;
            var d4343 = p43 * p43;
            var d2121 = p21 * p21;
            var denom = d2121 * d4343 - d4321 * d4321;
            if (Math.Abs(denom) < epslion) return false;
            double numer = d1343 * d4321 - d1321 * d4343;
            a = numer / denom;
            b = (d1343 + d4321 * a) / d4343;
            return true;
        }
        #endregion

        /// <summary>
        /// Determines whether two lines have the same value.
        /// </summary>
        /// <param name="a">A line.</param>
        /// <param name="b">Another line.</param>
        /// <returns>true if a has the same coordinates as b; otherwise false.</returns>
        public static bool operator ==(Line a, Line b)
        {
            return a.Start == b.Start && a.End == b.End;
        }

        /// <summary>
        /// Determines whether two lines have different values.
        /// </summary>
        /// <param name="a">A line.</param>
        /// <param name="b">Another line.</param>
        /// <returns>true if a has any coordinate that distinguishes it from b; otherwise false.</returns>
        public static bool operator !=(Line a, Line b)
        {
            return a.Start != b.Start || a.End != b.End;
        }

        public static implicit operator Line(Line2D line)
        {
            return new Line(line.Start.X, line.Start.Y, 0, line.End.X, line.End.Y, 0);
        }

        public static implicit operator Vector3D(Line line)
        {
            return line.End - line.Start;
        }
    }
}
