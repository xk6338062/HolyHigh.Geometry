using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 48)]
    [Serializable]
    public struct BoundingBox
    {
        #region members
        private Point3D m_min;
        private Point3D m_max;
        #endregion

        #region constructors
        /// <summary>
        /// Constructs a new boundingbox from two corner points.
        /// </summary>
        /// <param name="min">Point containing all the minimum coordinates.</param>
        /// <param name="max">Point containing all the maximum coordinates.</param>
        public BoundingBox(Point3D min, Point3D max)
        {
            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Constructs a boundingbox from numeric extremes.
        /// </summary>
        /// <param name="minX">Lower extreme for box X size.</param>
        /// <param name="minY">Lower extreme for box Y size.</param>
        /// <param name="minZ">Lower extreme for box Z size.</param>
        /// <param name="maxX">Upper extreme for box X size.</param>
        /// <param name="maxY">Upper extreme for box Y size.</param>
        /// <param name="maxZ">Upper extreme for box Z size.</param>
        public BoundingBox(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            m_min = new Point3D(minX, minY, minZ);
            m_max = new Point3D(maxX, maxY, maxZ);
        }

        /// <summary>
        /// Constructs a boundingbox from a collection of points.
        /// </summary>
        /// <param name="points">Points to include in the boundingbox.</param>
        public BoundingBox(IEnumerable<Point3D> points)
          : this()
        {
            bool first = true;
            foreach (Point3D pt in points)
            {
                if (first)
                {
                    m_min = pt;
                    m_max = pt;
                    first = false;
                }
                else
                {
                    if (m_min.X > pt.X)
                        m_min.X = pt.X;
                    if (m_min.Y > pt.Y)
                        m_min.Y = pt.Y;
                    if (m_min.Z > pt.Z)
                        m_min.Z = pt.Z;

                    if (m_max.X < pt.X)
                        m_max.X = pt.X;
                    if (m_max.Y < pt.Y)
                        m_max.Y = pt.Y;
                    if (m_max.Z < pt.Z)
                        m_max.Z = pt.Z;
                }
            }
        }

        /// <summary>
        /// Gets an [Empty] boundingbox. An Empty box is an invalid structure that has negative width.
        /// </summary>
        public static BoundingBox Empty
        {
            get
            {
                return new BoundingBox(1, 0, 0, -1, 0, 0);
            }
        }

        /// <summary>
        /// Gets a boundingbox that has Unset coordinates for Min and Max.
        /// </summary>
        public static BoundingBox Unset
        {
            get { return m_unset; }
        }
        static readonly BoundingBox m_unset = new BoundingBox(Point3D.Unset, Point3D.Unset);

        #endregion

        /// <summary>
        /// Constructs the string representation of this aligned boundingbox.
        /// </summary>
        /// <returns>Text.</returns>
        public override string ToString()
        {
            return string.Format("{0} - {1}", m_min.ToString(), m_max.ToString());
        }

        #region properties
        /// <summary>
        /// Gets a value that indicates whether or not this boundingbox is valid. 
        /// Empty boxes are not valid, and neither are boxes with unset points.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (!m_min.IsValid) { return false; }
                if (!m_max.IsValid) { return false; }

                return (m_min.X <= m_max.X && m_min.Y <= m_max.Y && m_min.Z <= m_max.Z);
            }
        }

        /// <summary>
        /// Gets or sets the point in the minimal corner.
        /// </summary>
        public Point3D Min
        {
            get { return m_min; }
            set { m_min = value; }
        }

        /// <summary>
        /// Gets or sets the point in the maximal corner.
        /// </summary>
        public Point3D Max
        {
            get { return m_max; }
            set { m_max = value; }
        }

        /// <summary>
        /// Gets the point in the center of the boundingbox.
        /// </summary>
        public Point3D Center
        {
            get { return 0.5 * (m_max + m_min); }
        }

        /// <summary>
        /// Gets the diagonal vector of this BoundingBox. 
        /// The diagonal connects the Min and Max points. 
        /// </summary>
        public Vector3D Diagonal
        {
            get { return m_max - m_min; }
        }
        #endregion

        #region methods
        /// <summary>
        /// Evaluates the boundingbox with normalized parameters.
        /// <para>The box has idealized side length of 1x1x1.</para>
        /// </summary>
        /// <param name="tx">Normalized (between 0 and 1 is inside the box) parameter along the X direction.</param>
        /// <param name="ty">Normalized (between 0 and 1 is inside the box) parameter along the Y direction.</param>
        /// <param name="tz">Normalized (between 0 and 1 is inside the box) parameter along the Z direction.</param>
        /// <returns>The point at the {tx, ty, tz} parameters.</returns>
        public Point3D PointAt(double tx, double ty, double tz)
        {
            double sx = 1.0 - tx;
            double sy = 1.0 - ty;
            double sz = 1.0 - tz;

            double x = (m_min.X == m_max.X) ? (m_min.X) : (sx * m_min.X + tx * m_max.X);
            double y = (m_min.Y == m_max.Y) ? (m_min.Y) : (sy * m_min.Y + ty * m_max.Y);
            double z = (m_min.Z == m_max.Z) ? (m_min.Z) : (sz * m_min.Z + tz * m_max.Z);

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Finds the closest point on or in the boundingbox.
        /// </summary>
        /// <param name="point">Sample point.</param>
        /// <returns>The point on or in the box that is closest to the sample point.</returns>
        public Point3D ClosestPoint(Point3D point)
        {
            return ClosestPoint(point, true);
        }

        /// <summary>
        /// Finds the closest point on or in the boundingbox.
        /// </summary>
        /// <param name="point">Sample point.</param>
        /// <param name="includeInterior">If false, the point is projected onto the boundary faces only, 
        /// otherwise the interior of the box is also taken into consideration.</param>
        /// <returns>The point on or in the box that is closest to the sample point.</returns>
        public Point3D ClosestPoint(Point3D point, bool includeInterior)
        {
            // Get extremes.
            double x0 = m_min.X;
            double x1 = m_max.X;
            double y0 = m_min.Y;
            double y1 = m_max.Y;
            double z0 = m_min.Z;
            double z1 = m_max.Z;

            // Swap coordinates if they are decreasing.
            if (x0 > x1) { x0 = m_max.X; x1 = m_min.X; }
            if (y0 > y1) { y0 = m_max.Y; y1 = m_min.Y; }
            if (z0 > z1) { z0 = m_max.Z; z1 = m_min.Z; }

            // Project x, y and z onto/into the box.
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            x = Math.Max(x, x0);
            y = Math.Max(y, y0);
            z = Math.Max(z, z0);

            x = Math.Min(x, x1);
            y = Math.Min(y, y1);
            z = Math.Min(z, z1);

            if (includeInterior) { return new Point3D(x, y, z); }
            // If the point was outside the box, we can return the quick solution.
            if (point.X <= x0 || point.X >= x1) { return new Point3D(x, y, z); }
            if (point.Y <= y0 || point.Y >= y1) { return new Point3D(x, y, z); }
            if (point.Z <= z0 || point.Z >= z1) { return new Point3D(x, y, z); }

            // The point appears to be inside the box, we need to project it to all sides.
            Point3D[] C = GetCorners();
            System.Collections.Generic.List<Plane> faces = new System.Collections.Generic.List<Plane>(6);

            if (m_min.X != m_max.X && m_min.Y != m_max.Y)
            {
                // Bottom and Top faces
                faces.Add(new Plane(C[0], C[1], C[3]));
                faces.Add(new Plane(C[4], C[5], C[7]));
            }
            if (m_min.X != m_max.X && m_min.Z != m_max.Z)
            {
                // Front and Back faces
                faces.Add(new Plane(C[0], C[1], C[4]));
                faces.Add(new Plane(C[3], C[2], C[7]));
            }
            if (m_min.Y != m_max.Y && m_min.Z != m_max.Z)
            {
                // Left and Right faces
                faces.Add(new Plane(C[0], C[3], C[4]));
                faces.Add(new Plane(C[1], C[2], C[5]));
            }

            double min_d = double.MaxValue;
            Point3D min_p = new Point3D(x, y, z);
            foreach (Plane face in faces)
            {
                double loc_d = Math.Abs(face.DistanceTo(new Point3D(x, y, z)));
                if (loc_d < min_d)
                {
                    min_d = loc_d;
                    min_p = face.ClosestPointTo(new Point3D(x, y, z));
                }
            }
            return min_p;
        }

        /// <summary>
        /// Finds the furthest point on the Box.
        /// </summary>
        /// <param name="point">Sample point.</param>
        /// <returns>The point on the box that is furthest from the sample point.</returns>
        public Point3D FurthestPoint(Point3D point)
        {
            // Get increasing extremes.
            double x0 = m_min.X;
            double x1 = m_max.X;
            double y0 = m_min.Y;
            double y1 = m_max.Y;
            double z0 = m_min.Z;
            double z1 = m_max.Z;

            // Swap coordinates if they are decreasing.
            if (x0 > x1) { x0 = m_max.X; x1 = m_min.X; }
            if (y0 > y1) { y0 = m_max.Y; y1 = m_min.Y; }
            if (z0 > z1) { z0 = m_max.Z; z1 = m_min.Z; }

            // Find the mid-point.
            double xm = 0.5 * (x0 + x1);
            double ym = 0.5 * (y0 + y1);
            double zm = 0.5 * (z0 + z1);

            // Project x, y and z onto the box.
            double x = x0;
            double y = y0;
            double z = z0;

            if (point.X < xm) { x = x1; }
            if (point.Y < ym) { y = y1; }
            if (point.Z < zm) { z = z1; }

            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Inflates the box with equal amounts in all directions. 
        /// Inflating with negative amounts may result in decreasing boxes. 
        /// <para>Invalid boxes can not be inflated.</para>
        /// </summary>
        /// <param name="amount">Amount (in model units) to inflate this box in all directions.</param>
        public void Inflate(double amount)
        {
            Inflate(amount, amount, amount);
        }

        /// <summary>
        /// Inflate the box with custom amounts in all directions. 
        /// Inflating with negative amounts may result in decreasing boxes. 
        /// <para>InValid boxes can not be inflated.</para>
        /// </summary>
        /// <param name="xAmount">Amount (in model units) to inflate this box in the x direction.</param>
        /// <param name="yAmount">Amount (in model units) to inflate this box in the y direction.</param>
        /// <param name="zAmount">Amount (in model units) to inflate this box in the z direction.</param>
        public void Inflate(double xAmount, double yAmount, double zAmount)
        {
            if (!IsValid) { return; }

            m_min.X -= xAmount;
            m_min.Y -= yAmount;
            m_min.Z -= zAmount;

            m_max.X += xAmount;
            m_max.Y += yAmount;
            m_max.Z += zAmount;
        }

        /// <summary>
        /// Tests a point for boundingbox inclusion. This is the same as calling Contains(point, false)
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <returns>true if the point is on the inside of or coincident with this boundingbox; otherwise false.</returns>
        public bool Contains(Point3D point)
        {
            return Contains(point, false);
        }
        /// <summary>
        /// Tests a point for BoundingBox inclusion.
        /// </summary>
        /// <param name="point">Point to test.</param>
        /// <param name="strict">If true, the point needs to be fully on the inside of the BoundingBox. 
        /// I.e. coincident points will be considered 'outside'.</param>
        /// <returns>
        /// <para>If 'strict' is affirmative, true if the point is inside this boundingbox; false if it is on the surface or outside.</para>
        /// <para>If 'strict' is negative, true if the point is on the surface or on the inside of the boundingbox; otherwise false.</para>
        /// </returns>
        public bool Contains(Point3D point, bool strict)
        {
            if (!point.IsValid) { return false; }

            if (strict)
            {
                if (point.X <= m_min.X) { return false; }
                if (point.X >= m_max.X) { return false; }
                if (point.Y <= m_min.Y) { return false; }
                if (point.Y >= m_max.Y) { return false; }
                if (point.Z <= m_min.Z) { return false; }
                if (point.Z >= m_max.Z) { return false; }
            }
            else
            {
                if (point.X < m_min.X) { return false; }
                if (point.X > m_max.X) { return false; }
                if (point.Y < m_min.Y) { return false; }
                if (point.Y > m_max.Y) { return false; }
                if (point.Z < m_min.Z) { return false; }
                if (point.Z > m_max.Z) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Determines whether this boundingbox contains another boundingbox.
        /// <para>This is the same as calling Contains(box,false).</para>
        /// </summary>
        /// <param name="box">Box to test.</param>
        /// <returns>true if the box is on the inside of this boundingbox, or is coincident with the surface of it.</returns>
        public bool Contains(BoundingBox box)
        {
            return Contains(box, false);
        }

        /// <summary>
        /// Determines whether this boundingbox contains another boundingbox.
        /// <para>The user can choose how to treat boundingboxes with coincidents surfaces.</para>
        /// </summary>
        /// <param name="box">Box to test.</param>
        /// <param name="strict">If true, the box needs to be fully on the inside of the boundingbox. 
        /// I.e. coincident boxes will be considered 'outside'.</param>
        /// <returns>true if the box is (strictly) on the inside of this BoundingBox.</returns>
        public bool Contains(BoundingBox box, bool strict)
        {
            if (!box.IsValid) { return false; }

            if (strict)
            {
                if (box.m_min.X <= m_min.X) { return false; }
                if (box.m_max.X >= m_max.X) { return false; }
                if (box.m_min.Y <= m_min.Y) { return false; }
                if (box.m_max.Y >= m_max.Y) { return false; }
                if (box.m_min.Z <= m_min.Z) { return false; }
                if (box.m_max.Z >= m_max.Z) { return false; }
            }
            else
            {
                if (box.m_min.X < m_min.X) { return false; }
                if (box.m_max.X > m_max.X) { return false; }
                if (box.m_min.Y < m_min.Y) { return false; }
                if (box.m_max.Y > m_max.Y) { return false; }
                if (box.m_min.Z < m_min.Z) { return false; }
                if (box.m_max.Z > m_max.Z) { return false; }
            }

            return true;
        }

        /// <summary>
        /// Ensures that the box is defined in an increasing fashion along X, Y and Z axes.
        /// If the Min or Max points are unset, this function will not change the box.
        /// </summary>
        /// <returns>true if the box was made valid, false if the box could not be made valid.</returns>
        public bool MakeValid()
        {
            if (!m_min.IsValid || !m_max.IsValid)
                return false;

            Point3D A = m_min;
            Point3D B = m_max;
            double minx = Math.Min(A.X, B.X);
            double miny = Math.Min(A.Y, B.Y);
            double minz = Math.Min(A.Z, B.Z);
            double maxx = Math.Max(A.X, B.X);
            double maxy = Math.Max(A.Y, B.Y);
            double maxz = Math.Max(A.Z, B.Z);
            m_min = new Point3D(minx, miny, minz);
            m_max = new Point3D(maxx, maxy, maxz);

            return true;
        }

        /// <summary>
        /// Gets one of the eight corners of the box.
        /// </summary>
        /// <param name="minX">true for the minimum on the X axis; false for the maximum.</param>
        /// <param name="minY">true for the minimum on the Y axis; false for the maximum.</param>
        /// <param name="minZ">true for the minimum on the Z axis; false for the maximum.</param>
        /// <returns>The requested point.</returns>
        public Point3D Corner(bool minX, bool minY, bool minZ)
        {
            double x = minX ? m_min.X : m_max.X;
            double y = minY ? m_min.Y : m_max.Y;
            double z = minZ ? m_min.Z : m_max.Z;
            return new Point3D(x, y, z);
        }

        /// <summary>
        /// Determines whether a bounding box is degenerate (flat) in one or more directions.
        /// </summary>
        /// <param name="tolerance">
        /// Distances &lt;= tolerance will be considered to be zero.  If tolerance
        /// is negative (default), then a scale invarient tolerance is used.
        /// </param>
        /// <returns>
        /// 0 = box is not degenerate
        /// 1 = box is a rectangle (degenerate in one direction).
        /// 2 = box is a line (degenerate in two directions).
        /// 3 = box is a point (degenerate in three directions)
        /// 4 = box is not valid.
        /// </returns>
        public int IsDegenerate(double tolerance)
        {
            Vector3D diag = Diagonal;
            if (tolerance < 0.0)
            {
                // compute scale invarient tolerance
                tolerance = diag.MaximumCoordinate * Utility.SQRT_EPSILON;
            }
            int rc = 0;
            if (diag.X < 0.0)
                return 4;
            if (diag.X <= tolerance)
                rc++;
            if (diag.Y < 0.0)
                return 4;
            if (diag.Y <= tolerance)
                rc++;
            if (diag.Z < 0.0)
                return 4;
            if (diag.Z <= tolerance)
                rc++;
            return rc;
        }

        /// <summary>
        /// Gets an array filled with the 8 corner points of this box.
        /// <para>See remarks for the return order.</para>
        /// </summary>
        /// <returns>An array of 8 corners.</returns>
        /// <remarks>
        /// <para>[0] Min.X, Min.Y, Min.Z</para>
        /// <para>[1] Max.X, Min.Y, Min.Z</para>
        /// <para>[2] Max.X, Max.Y, Min.Z</para>
        /// <para>[3] Min.X, Max.Y, Min.Z</para>
        ///
        /// <para>[4] Min.X, Min.Y, Max.Z</para>
        /// <para>[5] Max.X, Min.Y, Max.Z</para>
        /// <para>[6] Max.X, Max.Y, Max.Z</para>
        /// <para>[7] Min.X, Max.Y, Max.Z</para>
        /// </remarks>
        public Point3D[] GetCorners()
        {
            if (!IsValid)
                return null;

            Point3D[] corners = new Point3D[8];

            // corners need to be output in the same order that RhinoScript users expect
            corners[0] = new Point3D(m_min.X, m_min.Y, m_min.Z);
            corners[1] = new Point3D(m_max.X, m_min.Y, m_min.Z);
            corners[2] = new Point3D(m_max.X, m_max.Y, m_min.Z);
            corners[3] = new Point3D(m_min.X, m_max.Y, m_min.Z);

            corners[4] = new Point3D(m_min.X, m_min.Y, m_max.Z);
            corners[5] = new Point3D(m_max.X, m_min.Y, m_max.Z);
            corners[6] = new Point3D(m_max.X, m_max.Y, m_max.Z);
            corners[7] = new Point3D(m_min.X, m_max.Y, m_max.Z);

            return corners;
        }

        /// <summary>
        /// Gets an array of the 12 edges of this box.
        /// </summary>
        /// <returns>If the boundingbox IsValid, the 12 edges; otherwise, null.</returns>
        public Line[] GetEdges()
        {
            if (!IsValid)
                return null;

            Line[] edges = new Line[12];
            Point3D minPt = Min;
            Point3D maxPt = Max;
            edges[0].Start = minPt;
            edges[0].End = new Point3D(maxPt.X, minPt.Y, minPt.Z);
            edges[1].Start = edges[0].End;
            edges[1].End = new Point3D(maxPt.X, maxPt.Y, minPt.Z);
            edges[2].Start = edges[1].End;
            edges[2].End = new Point3D(minPt.X, maxPt.Y, minPt.Z);
            edges[3].Start = edges[2].End;
            edges[3].End = minPt;

            for (int i = 0; i < 4; i++)
            {
                edges[i + 4] = edges[i];
                edges[i + 4].StartZ = maxPt.Z;
                edges[i + 4].EndZ = maxPt.Z;

                edges[i + 8].Start = edges[i].Start;
                edges[i + 8].End = edges[i + 8].Start;
                edges[i + 8].EndZ = maxPt.Z;
            }
            return edges;
        }

        /// <summary>
        /// Updates this boundingbox to be the smallest axis aligned
        /// boundingbox that contains the transformed result of its 8 original corner
        /// points.
        /// </summary>
        /// <param name="xform">A transform.</param>
        /// <returns>true if this operation is sucessfull; otherwise false.</returns>
        public bool Transform(Transform xform)
        {
            if (!IsValid)
                return false;
            Point3D[] points = GetCorners();
            points = xform.TransformList(points);
            this = new BoundingBox(points);
            return true;
        }

        #region union methods
        /// <summary>
        /// Updates this BoundingBox to represent the union of itself and another box.
        /// </summary>
        /// <param name="other">Box to include in this union.</param>
        /// <remarks>If either this BoundingBox or the other BoundingBox is InValid, 
        /// the Valid BoundingBox will be the only one included in the union.</remarks>
        public void Union(BoundingBox other)
        {
            this = Union(this, other);
        }

        /// <summary>
        /// Updates this BoundingBox to represent the union of itself and a point.
        /// </summary>
        /// <param name="point">Point to include in the union.</param>
        /// <remarks>If this boundingbox is InValid then the union will be 
        /// the BoundingBox containing only the point. If the point is InValid, 
        /// this BoundingBox will remain unchanged.
        /// </remarks>
        public void Union(Point3D point)
        {
            this = Union(this, point);
        }

        /// <summary>
        /// Returns a new BoundingBox that represents the union of boxes a and b.
        /// </summary>
        /// <param name="a">First box to include in union.</param>
        /// <param name="b">Second box to include in union.</param>
        /// <returns>The BoundingBox that contains both a and b.</returns>
        /// <remarks>Invalid boxes are ignored and will not affect the union.</remarks>
        public static BoundingBox Union(BoundingBox a, BoundingBox b)
        {
            if (!a.IsValid) { return b; }
            if (!b.IsValid) { return a; }

            BoundingBox rc = new BoundingBox();

            rc.m_min.X = (a.m_min.X < b.m_min.X) ? a.m_min.X : b.m_min.X;
            rc.m_min.Y = (a.m_min.Y < b.m_min.Y) ? a.m_min.Y : b.m_min.Y;
            rc.m_min.Z = (a.m_min.Z < b.m_min.Z) ? a.m_min.Z : b.m_min.Z;

            rc.m_max.X = (a.m_max.X > b.m_max.X) ? a.m_max.X : b.m_max.X;
            rc.m_max.Y = (a.m_max.Y > b.m_max.Y) ? a.m_max.Y : b.m_max.Y;
            rc.m_max.Z = (a.m_max.Z > b.m_max.Z) ? a.m_max.Z : b.m_max.Z;

            return rc;
        }

        /// <summary>
        /// Computes the intersection of two bounding boxes.
        /// </summary>
        /// <param name="a">A first bounding box.</param>
        /// <param name="b">A second bounding box.</param>
        /// <returns>The intersection bounding box.</returns>
        public static BoundingBox Intersection(BoundingBox a, BoundingBox b)
        {
            BoundingBox rc = Unset;
            if (a.IsValid && b.IsValid)
            {
                Point3D min = new Point3D();
                Point3D max = new Point3D();
                min.X = (a.Min.X >= b.Min.X) ? a.Min.X : b.Min.X;
                min.Y = (a.Min.Y >= b.Min.Y) ? a.Min.Y : b.Min.Y;
                min.Z = (a.Min.Z >= b.Min.Z) ? a.Min.Z : b.Min.Z;
                max.X = (a.Max.X <= b.Max.X) ? a.Max.X : b.Max.X;
                max.Y = (a.Max.Y <= b.Max.Y) ? a.Max.Y : b.Max.Y;
                max.Z = (a.Max.Z <= b.Max.Z) ? a.Max.Z : b.Max.Z;
                rc = new BoundingBox(min, max);
            }
            return rc;
        }

        /// <summary>
        /// Returns a new BoundingBox that represents the union of a bounding box and a point.
        /// </summary>
        /// <param name="box">Box to include in the union.</param>
        /// <param name="point">Point to include in the union.</param>
        /// <returns>The BoundingBox that contains both the box and the point.</returns>
        /// <remarks>Invalid boxes and points are ignored and will not affect the union.</remarks>
        public static BoundingBox Union(BoundingBox box, Point3D point)
        {
            if (!box.IsValid) { return new BoundingBox(point, point); }
            if (!point.IsValid) { return box; }

            BoundingBox rc = new BoundingBox();

            rc.m_min.X = (box.m_min.X < point.X) ? box.m_min.X : point.X;
            rc.m_min.Y = (box.m_min.Y < point.Y) ? box.m_min.Y : point.Y;
            rc.m_min.Z = (box.m_min.Z < point.Z) ? box.m_min.Z : point.Z;

            rc.m_max.X = (box.m_max.X > point.X) ? box.m_max.X : point.X;
            rc.m_max.Y = (box.m_max.Y > point.Y) ? box.m_max.Y : point.Y;
            rc.m_max.Z = (box.m_max.Z > point.Z) ? box.m_max.Z : point.Z;

            return rc;
        }
        #endregion
        #endregion
    }
}
