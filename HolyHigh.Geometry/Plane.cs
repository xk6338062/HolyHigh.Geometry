using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the value of a center point and two axes in a plane in three dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 128)]
    [Serializable]
    public struct Plane : IEquatable<Plane>, IEpsilonComparable<Plane>
    {
        #region members
        private Point3D m_origin;
        private Vector3D m_xaxis;
        private Vector3D m_yaxis;
        private Vector3D m_zaxis;
        public PlaneEquation equation;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the origin point of this plane.
        /// </summary>
        public Point3D Origin
        {
            get { return m_origin; }
            set { m_origin = value; }
        }
        /// <summary>
        /// Gets or sets the X coordinate of the origin of this plane.
        /// </summary>
        public double OriginX
        {
            get { return m_origin.X; }
            set { m_origin.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y coordinate of the origin of this plane.
        /// </summary>
        public double OriginY
        {
            get { return m_origin.Y; }
            set { m_origin.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Z coordinate of the origin of this plane.
        /// </summary>
        public double OriginZ
        {
            get { return m_origin.Z; }
            set { m_origin.Z = value; }
        }
        /// <summary>
        /// Gets or sets the X axis vector of this plane.
        /// </summary>
        public Vector3D XAxis
        {
            get { return m_xaxis; }
            set { m_xaxis = value; }
        }
        /// <summary>
        /// Gets or sets the Y axis vector of this plane.
        /// </summary>
        public Vector3D YAxis
        {
            get { return m_yaxis; }
            set { m_yaxis = value; }
        }
        /// <summary>
        /// Gets or sets the Z axis vector of this plane.
        /// </summary>
        public Vector3D ZAxis
        {
            get { return m_zaxis; }
            set { m_zaxis = value; }
        }
        #endregion

        #region constants
        /// <summary>
        /// plane coincident with the World XY plane.
        /// </summary>
        public static Plane WorldXY
        {
            get
            {
                return new Plane { XAxis = new Vector3D(1, 0, 0), YAxis = new Vector3D(0, 1, 0), ZAxis = new Vector3D(0, 0, 1) };
            }
        }

        /// <summary>
        /// plane coincident with the World YZ plane.
        /// </summary>
        public static Plane WorldYZ
        {
            get
            {
                return new Plane { XAxis = new Vector3D(0, 1, 0), YAxis = new Vector3D(0, 0, 1), ZAxis = new Vector3D(1, 0, 0) };
            }
        }

        /// <summary>
        /// plane coincident with the World ZX plane.
        /// </summary>
        public static Plane WorldZX
        {
            get
            {
                return new Plane { XAxis = new Vector3D(0, 0, 1), YAxis = new Vector3D(1, 0, 0), ZAxis = new Vector3D(0, 1, 0) };
            }
        }

        /// <summary>
        /// Gets a plane that contains Unset origin and axis vectors.
        /// </summary>
        public static Plane Unset
        {
            get
            {
                return new Plane { Origin = Point3D.Unset, XAxis = Vector3D.Unset, YAxis = Vector3D.Unset, ZAxis = Vector3D.Unset };
            }
        }
        #endregion

        #region constructors
        /// <summary>Copy constructor.
        /// <para>This is nothing special and performs the same as assigning to another variable.</para>
        /// </summary>
        /// <param name="other">The source plane value.</param>
        public Plane(Plane other)
        {
            this = other;
        }

        /// <summary>
        /// Constructs a plane from a point and a normal vector.
        /// </summary>
        /// <param name="origin">Origin point of the plane.</param>
        /// <param name="normal">Non-zero normal to the plane.</param>
        /// <seealso>CreateFromNormal</seealso>
        public Plane(Point3D origin, Vector3D normal)
          : this()
        {
            m_origin = origin;
            m_zaxis = normal;
            m_zaxis.Normalize();
            m_xaxis.PerpendicularTo(m_zaxis);
            m_xaxis.Normalize();
            m_yaxis = m_zaxis.Cross(m_xaxis);
            m_yaxis.Normalize();
            if (!UpdateEquation())
            {
                throw new ArgumentException();
            }
        }

        public bool CreateFromEquation(PlaneEquation eqn)
        {
            bool b = false;
            equation = eqn;
            m_zaxis.X = eqn.X;
            m_zaxis.Y = eqn.Y;
            m_zaxis.Z = eqn.Z;
            double d = m_zaxis.Length;
            if (d > 0.0)
            {
                d = 1.0 / d;
                m_zaxis *= d;
                m_origin = (Point3D)(-d * eqn.D * m_zaxis);
                b = true;
            }
            m_xaxis.PerpendicularTo(m_zaxis);
            m_xaxis.Normalize();
            m_yaxis = m_zaxis.Cross(m_xaxis);
            m_yaxis.Normalize();
            return b;
        }
        /// <summary>
        /// Constructs a plane from a point and two vectors in the plane.
        /// </summary>
        /// <param name='origin'>Origin point of the plane.</param>
        /// <param name='xDirection'>
        /// Non-zero vector in the plane that determines the x-axis direction.
        /// </param>
        /// <param name='yDirection'>
        /// Non-zero vector not parallel to x_dir that is used to determine the
        /// yaxis direction. y_dir does not need to be perpendicular to x_dir.
        /// </param>
        public Plane(Point3D origin, Vector3D xDirection, Vector3D yDirection)
          : this()
        {
            var rc = CreateFromFrame(origin, xDirection, yDirection);
            if (!rc) throw new ArgumentException();
        }

        private bool CreateFromFrame(Point3D origin, Vector3D xDirection, Vector3D yDirection)
        {
            m_origin = origin;
            m_xaxis = xDirection;
            m_xaxis.Normalize();
            m_zaxis = m_xaxis.Cross(yDirection);
            var rc = m_zaxis.Normalize();
            if (rc && UpdateEquation())
            {
                m_yaxis = m_zaxis.Cross(m_xaxis);
                return rc;
            }
            return false;
        }
        /// <summary>
        /// Initializes a plane from three non-colinear points.
        /// </summary>
        /// <param name='origin'>Origin point of the plane.</param>
        /// <param name='xPoint'>
        /// Second point in the plane. The x-axis will be parallel to x_point-origin.
        /// </param>
        /// <param name='yPoint'>
        /// Third point on the plane that is not colinear with the first two points.
        /// yaxis*(y_point-origin) will be &gt; 0.
        /// </param>
        public Plane(Point3D origin, Point3D xPoint, Point3D yPoint)
          : this()
        {
            m_origin = origin;
            bool rc = m_zaxis.PerpendicularTo(origin, xPoint, yPoint);
            m_xaxis = xPoint - origin;
            m_xaxis.Normalize();
            m_yaxis = m_zaxis.Cross(m_xaxis);
            m_yaxis.Normalize();
            if (!equation.Create(m_origin, m_zaxis))
            {
                rc = false;
            }
            else throw new ArgumentException();
        }

        /// <summary>
        /// Constructs a plane from an equation
        /// ax+by+cz+d=0.
        /// </summary>
        public Plane(double a, double b, double c, double d)
          : this()
        {
            var eqn = new PlaneEquation(a, b, c, d);
            if (CreateFromEquation(eqn))
            {
                this.m_zaxis.Normalize();
            }
            else throw new ArgumentException();
        }
        #endregion

        #region operators
        /// <summary>
        /// Determines if two planes are equal.
        /// </summary>
        /// <param name="a">A first plane.</param>
        /// <param name="b">A second plane.</param>
        /// <returns>true if the two planes have all equal components; false otherwise.</returns>
        public static bool operator ==(Plane a, Plane b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines if two planes are different.
        /// </summary>
        /// <param name="a">A first plane.</param>
        /// <param name="b">A second plane.</param>
        /// <returns>true if the two planes have any different componet components; false otherwise.</returns>
        public static bool operator !=(Plane a, Plane b)
        {
            return (a.m_origin != b.m_origin) ||
                   (a.m_xaxis != b.m_xaxis) ||
                   (a.m_yaxis != b.m_yaxis) ||
                   (a.m_zaxis != b.m_zaxis);
        }

        /// <summary>
        /// Determines if an object is a plane and has the same components as this plane.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>true if obj is a plane and has the same components as this plane; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return ((obj is Plane) && (this == (Plane)obj));
        }

        /// <summary>
        /// Determines if another plane has the same components as this plane.
        /// </summary>
        /// <param name="plane">A plane.</param>
        /// <returns>true if plane has the same components as this plane; false otherwise.</returns>
        public bool Equals(Plane plane)
        {
            return (m_origin == plane.m_origin) &&
                   (m_xaxis == plane.m_xaxis) &&
                   (m_yaxis == plane.m_yaxis) &&
                   (m_zaxis == plane.m_zaxis);
        }

        /// <summary>
        /// Gets a non-unique hashing code for this entity.
        /// </summary>
        /// <returns>A particular number for a specific instance of plane.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_origin.GetHashCode() ^ m_xaxis.GetHashCode() ^ m_yaxis.GetHashCode() ^ m_zaxis.GetHashCode();
        }

        /// <summary>
        /// Constructs the string representation of this plane.
        /// </summary>
        /// <returns>Text.</returns>
        public override string ToString()
        {
            string rc = String.Format(System.Globalization.CultureInfo.InvariantCulture,
              "Origin={0} XAxis={1}, YAxis={2}, ZAxis={3}",
              Origin, XAxis, YAxis, ZAxis.ToString());
            return rc;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the normal of this plane. This is essentially the ZAxis of the plane.
        /// </summary>
        public Vector3D Normal
        {
            get { return ZAxis; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this is a valid plane. 
        /// A plane is considered to be valid when all fields contain reasonable 
        /// information and the equation jibes with point and zaxis.
        /// </summary>
        public bool IsValid
        {
            get
            {
                PlaneEquation planeEquation = new PlaneEquation(0, 0, 0, 0);
                planeEquation.Create(m_origin, m_zaxis);
                if (!planeEquation.IsValid)
                    return false;
                double x = planeEquation.ValueAt(m_origin);
                if (Math.Abs(x) > Utility.ZeroTolerance)
                {
                    double tol = Math.Abs(m_origin.MaximumCoordinate) + Math.Abs(planeEquation.D);
                    if (tol > 1000.0 && m_origin.IsValid)
                    {
                        // 8 September 2003 Chuck and Dale:
                        //   Fixing discrepancy between optimized and debug behavior.
                        //   In this case, the ON_ZERO_TOLERANCE test worked in release
                        //   and failed in debug. The principal behind this fix is valid
                        //   for release builds too.
                        //   For large point coordinates or planes far from the origin,
                        //   the best we can hope for is to kill the first 15 or so decimal
                        //   places.
                        tol *= (Utility.EPSILON * 10.0);
                        if (Math.Abs(x) > tol)
                            return false;
                    }
                    else
                        return false;
                }

                if (!Vector3D.IsRightHandFrame(m_xaxis, m_yaxis, m_zaxis))
                    return false;
                Vector3D n = planeEquation.UnitNormal();
                x = n * m_zaxis;
                if (Math.Abs(x - 1.0) > Utility.SQRT_EPSILON)
                    return false;
                return true;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Gets the plane equation for this plane in the format of Ax+By+Cz+D=0.
        /// </summary>
        /// <returns>
        /// Array of four values.
        /// </returns>
        public double[] GetPlaneEquation()
        {
            double[] rc = new double[4];
            rc[0] = equation.X;
            rc[1] = equation.Y;
            rc[2] = equation.Z;
            rc[3] = equation.D;
            return rc;
        }

        private bool UpdateEquation()
        {
            return equation.Create(Origin, Normal);
        }

        /// <summary>
        /// Get the value of the plane equation at the point.
        /// </summary>
        /// <param name="p">evaulation point.</param>
        /// <returns>returns pe[0]*p.X + pe[1]*p.Y + pe[2]*p.Z + pe[3] where
        /// pe[0], pe[1], pe[2] and pe[3] are the coeeficients of the plane equation.
        /// 
        /// </returns>
        public double ValueAt(Point3D p)
        {
            var pe = this.GetPlaneEquation();
            return (pe[0] * p.X + pe[1] * p.Y + pe[2] * p.Z + pe[3]);
        }

        /// <summary>
        /// Evaluate a point on the plane.
        /// </summary>
        /// <param name="u">evaulation parameter.</param>
        /// <param name="v">evaulation parameter.</param>
        /// <returns>plane.origin + u*plane.xaxis + v*plane.yaxis.</returns>
        public Point3D PointAt(double u, double v)
        {
            return (Origin + u * XAxis + v * YAxis);
        }

        /// <summary>
        /// Evaluate a point on the plane.
        /// </summary>
        /// <param name="u">evaulation parameter.</param>
        /// <param name="v">evaulation parameter.</param>
        /// <param name="w">evaulation parameter.</param>
        /// <returns>plane.origin + u*plane.xaxis + v*plane.yaxis + z*plane.zaxis.</returns>
        public Point3D PointAt(double u, double v, double w)
        {
            return (Origin + u * XAxis + v * YAxis + w * ZAxis);
        }


        public Line? Intersect(Plane other)
        {
            Line L = new Line();
            Vector3D d = other.m_zaxis.Cross(m_zaxis);
            Point3D p = 0.5 * (m_origin + other.m_origin);
            try
            {
                Plane T = new Plane(p, d);
                var point = Intersect(other, T);
                if (point.HasValue)
                {
                    L.Start = point.Value;
                    L.End = L.Start + d;
                    return L;
                }
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public Point3D? Intersect(Plane p1, Plane p2)
        {
            var r1 = GetPlaneEquation();
            var r2 = p1.GetPlaneEquation();
            var r3 = p2.GetPlaneEquation();
            var a = new double[9];
            a[0] = r1[0]; a[1] = r1[1]; a[2] = r1[2];
            a[3] = r2[0]; a[4] = r2[1]; a[5] = r2[2];
            a[6] = r3[0]; a[7] = r3[1]; a[8] = r3[2];
            var b = new double[3];
            b[0] = -r1[3]; b[1] = -r2[3]; b[2] = -r3[3];
            try
            {
                var x = a.SolveWith(b);
                return new Point3D(x[0], x[1], x[2]);
            }
            catch (ArgumentException)
            {
                return null;
            }

        }

        /// <summary>
        /// Extends this plane through a bounding box. 
        /// </summary>
        /// <param name="box">A box to use as minimal extension boundary.</param>
        /// <param name="s">
        /// If this function returns true, 
        /// the s parameter returns the Interval on the plane along the X direction that will 
        /// encompass the Box.
        /// </param>
        /// <param name="t">
        /// If this function returns true, 
        /// the t parameter returns the Interval on the plane along the Y direction that will 
        /// encompass the Box.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public bool ExtendThroughBox(BoundingBox box, out Interval s, out Interval t)
        {
            s = Interval.Unset;
            t = Interval.Unset;

            if (!IsValid) { return false; }
            if (!box.IsValid) { return false; }

            return ExtendThroughPoints(box.GetCorners(), ref s, ref t);
        }
        internal bool ExtendThroughPoints(IEnumerable<Point3D> pts, ref Interval s, ref Interval t)
        {
            double s0 = double.MaxValue;
            double s1 = double.MinValue;
            double t0 = double.MaxValue;
            double t1 = double.MinValue;
            bool valid = false;

            foreach (Point3D pt in pts)
            {
                double sp, tp;
                if (ClosestParameter(pt, out sp, out tp))
                {
                    valid = true;

                    s0 = Math.Min(s0, sp);
                    s1 = Math.Max(s1, sp);
                    t0 = Math.Min(t0, tp);
                    t1 = Math.Max(t1, tp);
                }
            }

            if (valid)
            {
                s = new Interval(s0, s1);
                t = new Interval(t0, t1);
            }
            return valid;
        }

        #region projections
        /// <summary>
        /// Gets the parameters of the point on the plane closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to get close to.</param>
        /// <param name="s">Parameter along plane X-direction.</param>
        /// <param name="t">Parameter along plane Y-direction.</param>
        /// <returns>
        /// true if a parameter could be found, 
        /// false if the point could not be projected successfully.
        /// </returns>
        /// <example>
        /// <code source='examples\vbnet\ex_addlineardimension2.vb' lang='vbnet'/>
        /// <code source='examples\cs\ex_addlineardimension2.cs' lang='cs'/>
        /// <code source='examples\py\ex_addlineardimension2.py' lang='py'/>
        /// </example>
        public bool ClosestParameter(Point3D testPoint, out double s, out double t)
        {
            Vector3D v = testPoint - Origin;
            s = v * XAxis;
            t = v * YAxis;
            return true;
        }

        /// <summary>
        /// Gets the point on the plane closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to get close to.</param>
        /// <returns>
        /// The point on the plane that is closest to testPoint, 
        /// or Point3D.Unset on failure.
        /// </returns>
        public Point3D ClosestPointTo(Point3D testPoint)
        {
            double s, t;

            // ClosestParameterTo does not currently validate input so won't return
            // false, therefore this function won't actually return an Unset point.
            // The code should probably be left this way so people check return
            // codes in case a fast way to validate input is added. The same problem
            // exists with the C++ sdk. 
            return !ClosestParameter(testPoint, out s, out t) ? Point3D.Unset : PointAt(s, t);
        }

        /// <summary>
        /// Returns the signed distance from testPoint to its projection onto this plane. 
        /// If the point is below the plane, a negative distance is returned.
        /// </summary>
        /// <param name="testPoint">Point to test.</param>
        /// <returns>Signed distance from this plane to testPoint.</returns>
        public double DistanceTo(Point3D testPoint)
        {
            return (testPoint - m_origin) * m_zaxis;
        }

        /// <summary>
        /// Convert a point from World space coordinates into Plane space coordinates.
        /// </summary>
        /// <param name="ptSample">World point to remap.</param>
        /// <param name="ptPlane">Point in plane (s,t,d) coordinates.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <remarks>D stands for distance, not disease.</remarks>
        public bool RemapToPlaneSpace(Point3D ptSample, out Point3D ptPlane)
        {
            double s, t;
            if (!ClosestParameter(ptSample, out s, out t))
            {
                ptPlane = Point3D.Unset;
                return false;
            }

            double d = DistanceTo(ptSample);

            ptPlane = new Point3D(s, t, d);
            return true;
        }


        #endregion

        #region transformations
        /// <summary>
        /// Flip this plane by swapping out the X and Y axes and inverting the Z axis.
        /// </summary>
        public void Flip()
        {
            Vector3D v = m_xaxis;
            m_xaxis = m_yaxis;
            m_yaxis = v;
            m_zaxis = -m_zaxis;
        }

        /// <summary>
        /// Transform the plane with a Transformation matrix.
        /// </summary>
        /// <param name="xform">Transformation to apply to plane.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Transform(Transform xform)
        {
            if (xform == Geometry.Transform.Identity)
            {
                return IsValid;
            }

            Point3D origin_pt = xform * m_origin;

            // use care tranforming vectors to get
            // maximum precision and the right answer
            bool bUseVectorXform = (0.0 == xform.M30
                                     && 0.0 == xform.M31
                                     && 0.0 == xform.M32
                                     && 1.0 == xform.M33
                                   );

            Vector3D xaxis_vec = bUseVectorXform ? (xform * m_xaxis) : ((xform * (m_origin + m_xaxis)) - origin_pt);
            Vector3D yaxis_vec = bUseVectorXform ? (xform * m_yaxis) : ((xform * (m_origin + m_yaxis)) - origin_pt);

            return CreateFromFrame(origin_pt, xaxis_vec, yaxis_vec);
        }

        /// <summary>
        /// Translate (move) the plane along a vector.
        /// </summary>
        /// <param name="delta">Translation (motion) vector.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Translate(Vector3D delta)
        {
            if (!delta.IsValid)
                return false;

            Origin += delta;
            return true;
        }

        /// <summary>
        /// Rotate the plane about its origin point.
        /// </summary>
        /// <param name="sinAngle">Sin(angle).</param>
        /// <param name="cosAngle">Cos(angle).</param>
        /// <param name="axis">Axis of rotation.</param>
        /// <returns>true on success, false on failure.</returns>
        internal bool Rotate(double sinAngle, double cosAngle, Vector3D axis)
        {
            bool rc = true;
            if (axis == ZAxis)
            {
                Vector3D x = cosAngle * XAxis + sinAngle * YAxis;
                Vector3D y = cosAngle * YAxis - sinAngle * XAxis;
                XAxis = x;
                YAxis = y;
                rc = UpdateEquation();
            }
            else
            {
                Point3D origin_pt = Origin;
                rc = Rotate(sinAngle, cosAngle, axis, Origin);
                Origin = origin_pt; // to kill any fuzz
            }
            return rc;
        }

        /// <summary>
        /// Rotate the plane about its origin point.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <param name="axis">Axis of rotation.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angle, Vector3D axis)
        {
            return Rotate(Math.Sin(angle), Math.Cos(angle), axis);
        }

        /// <summary>
        /// Rotate the plane about a custom anchor point.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <param name="axis">Axis of rotation.</param>
        /// <param name="centerOfRotation">Center of rotation.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angle, Vector3D axis, Point3D centerOfRotation)
        {
            return Rotate(Math.Sin(angle), Math.Cos(angle), axis, centerOfRotation);
        }

        public bool Rotate(double sinAngle, double cosAngle, Vector3D axis, Point3D centerOfRotation)
        {
            if (centerOfRotation == Origin)
            {
                Transform rot = Geometry.Transform.Rotation(sinAngle, cosAngle, axis, Point3D.Origin);
                m_xaxis = rot * m_xaxis;
                m_yaxis = rot * m_yaxis;
                if (!(axis == m_zaxis))
                    m_zaxis = rot * m_zaxis;
                return UpdateEquation();
            }
            Transform rot2 = Geometry.Transform.Rotation(sinAngle, cosAngle, axis, centerOfRotation);
            return Transform(rot2);
        }
        #endregion
        #endregion

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Plane other, double epsilon)
        {
            return m_origin.EpsilonEquals(other.m_origin, epsilon) &&
                   m_xaxis.EpsilonEquals(other.m_xaxis, epsilon) &&
                   m_yaxis.EpsilonEquals(other.m_yaxis, epsilon) &&
                   m_zaxis.EpsilonEquals(other.m_zaxis, epsilon);
        }
    }
}
