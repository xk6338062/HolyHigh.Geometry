using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents a circle in 3D.
    /// <para>The values used are a radius and an orthonormal frame	of the plane containing the circle,
    /// with origin at the center.</para>
    /// <para>The circle is parameterized by radians from 0 to 2 Pi given by</para>
    /// <para>t -> center + cos(t)*radius*xaxis + sin(t)*radius*yaxis</para>
    /// <para>where center, xaxis and yaxis define the orthonormal frame of the circle plane.</para>
    /// </summary>
    /// <remarks>>An IsValid circle has positive radius and an IsValid plane defining the frame.</remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 136)]
    [Serializable]
    public struct Circle : IEpsilonComparable<Circle>
    {
        #region members
        internal Plane m_plane;
        internal double m_radius;
        #endregion

        #region constants
        /// <summary>
        /// Gets a circle with Unset components.
        /// </summary>
        static public Circle Unset
        {
            get
            {
                return new Circle(Plane.Unset, Utility.UnsetValue);
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a circle with center (0,0,0) in the world XY plane.
        /// </summary>
        /// <param name="radius">Radius of circle, should be a positive number.</param>
        public Circle(double radius) : this(Plane.WorldXY, radius) { }

        /// <summary>
        /// Initializes a circle on a plane with a given radius.
        /// </summary>
        /// <param name="plane">Plane of circle. Plane origin defines the center of the circle.</param>
        /// <param name="radius">Radius of circle (should be a positive value).</param>
        public Circle(Plane plane, double radius)
        {
            m_plane = plane;
            m_radius = radius;
        }

        /// <summary>
        /// Initializes a circle parallel to the world XY plane with given center and radius.
        /// </summary>
        /// <param name="center">Center of circle.</param>
        /// <param name="radius">Radius of circle (should be a positive value).</param>
        public Circle(Point3D center, double radius)
        {
            m_plane = Plane.WorldXY;
            m_plane.Origin = center;
            m_radius = radius;
        }

        /// <summary>
        /// Initializes a circle from an arc.
        /// </summary>
        /// <param name="arc">Arc that defines the plane and radius.</param>
        public Circle(Arc arc)
        {
            m_plane = arc.Plane;
            m_radius = arc.Radius;
        }

        /// <summary>
        /// Initializes a circle through three 3d points. The start/end of the circle is at point1.
        /// </summary>
        public Circle(Point3D point1, Point3D point2, Point3D point3)
          : this()
        {
            if (!Create(point1, point2, point3))
                throw new ArgumentException();
        }

        bool Create(Point3D p, Point3D q, Point3D r)
        {
            Point3D c = Point3D.Origin;
            Vector3D x, y, z;
            z = Vector3D.Zero;
            // get normal
            for (; ; )
            {
                if (!z.PerpendicularTo(p, q, r))
                    break;
                // get center as the intersection of 3 planes
                Plane plane0 = new Plane(p, z);
                Plane plane1 = new Plane(0.5 * (p + q), p - q);
                Plane plane2 = new Plane(0.5 * (r + q), r - q);
                var pp = plane0.Intersect(plane1, plane2);
                if (!pp.HasValue) break;
                c = pp.Value;
                x = p - c;
                m_radius = x.Length;
                if (!(m_radius > 0.0))
                    break;

                if (!x.Normalize())
                    break;

                y = z.Cross(x);
                if (!y.Normalize())
                    break;

                m_plane.Origin = c;
                m_plane.XAxis = x;
                m_plane.YAxis = y;
                m_plane.ZAxis = z;
                return true;
            }

            m_plane = Plane.WorldXY;
            m_radius = 0.0;
            return false;

        }

        /// <summary>
        /// Initializes a circle parallel to a given plane with given center and radius.
        /// </summary>
        /// <param name="plane">Plane for circle.</param>
        /// <param name="center">Center point override.</param>
        /// <param name="radius">Radius of circle (should be a positive value).</param>
        public Circle(Plane plane, Point3D center, double radius)
        {
            m_plane = plane;
            m_radius = radius;
            m_plane.Origin = center;
        }

        /// <summary>
        /// Initializes a circle from two 3d points and a tangent at the first point.
        /// The start/end of the circle is at point "startPoint".
        /// </summary>
        /// <param name="startPoint">Start point of circle.</param>
        /// <param name="tangentAtP">Tangent vector at start.</param>
        /// <param name="pointOnCircle">Point coincident with desired circle.</param>
        /// <remarks>May create an Invalid circle</remarks>
        public Circle(Point3D startPoint, Vector3D tangentAtP, Point3D pointOnCircle)
          : this()
        {
            //if (!UnsafeNativeMethods.ON_Circle_CreatePtVecPt(ref this, startPoint, tangentAtP, pointOnCircle))
            //{
            //    this = new Circle();
            //}
        }
        #endregion

        #region properties
        /// <summary> 
        /// A valid circle has radius larger than 0.0 and a base plane which is must also be valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool rc = Utility.IsValidDouble(m_radius) && m_radius > 0.0 && m_plane.IsValid;
                return rc;
            }
        }

        #region shape properties
        /// <summary>
        /// Gets or sets the radius of this circle. 
        /// Radii should be positive values.
        /// </summary>
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        /// <summary>
        /// Gets or sets the diameter (radius * 2.0) of this circle. 
        /// Diameters should be positive values.
        /// </summary>
        public double Diameter
        {
            get { return m_radius * 2.0; }
            set { m_radius = 0.5 * value; }
        }

        /// <summary>
        /// Gets or sets the plane of the circle.
        /// </summary>
        public Plane Plane
        {
            get { return m_plane; }
            set { m_plane = value; }
        }

        /// <summary>
        /// Gets or sets the center point of this circle.
        /// </summary>
        public Point3D Center
        {
            // David asks : since Point3D is a value type, can't we just return the origin directly?
            get { return m_plane.Origin; }
            set { m_plane.Origin = value; }
        }

        /// <summary>
        /// Gets the normal vector for this circle.
        /// </summary>
        public Vector3D Normal
        {
            get { return m_plane.ZAxis; }
        }

        /// <summary>
        /// Gets or sets the circumference of this circle.
        /// </summary>
        public double Circumference
        {
            get
            {
                return Math.Abs(2.0 * Math.PI * m_radius);
            }
            set
            {
                m_radius = value / (2.0 * Math.PI);
            }
        }

        ///// <summary>
        ///// Gets the circle's 3d axis aligned bounding box.
        ///// </summary>
        ///// <returns>3d bounding box.</returns>
        //public BoundingBox BoundingBox
        //{
        //  get
        //  {
        //    BoundingBox rc = new BoundingBox();
        //    UnsafeNativeMethods.ON_Circle_BoundingBox(ref this, ref rc);
        //    return rc;
        //  }
        //}

        /// <summary>
        /// Gets the circle's 3d axis aligned bounding box.
        /// </summary>
        /// <returns>3d bounding box.</returns>
        public BoundingBox BoundingBox
        {
            // David changed this on april 16th 2010, we need to provide tight boundingboxes for atomic types.
            get
            {
                double rx = m_radius * Length2d(m_plane.ZAxis.Y, m_plane.ZAxis.Z);
                double ry = m_radius * Length2d(m_plane.ZAxis.Z, m_plane.ZAxis.X);
                double rz = m_radius * Length2d(m_plane.ZAxis.X, m_plane.ZAxis.Y);

                double x0 = m_plane.Origin.X - rx;
                double x1 = m_plane.Origin.X + rx;
                double y0 = m_plane.Origin.Y - ry;
                double y1 = m_plane.Origin.Y + ry;
                double z0 = m_plane.Origin.Z - rz;
                double z1 = m_plane.Origin.Z + rz;

                return new BoundingBox(x0, y0, z0, x1, y1, z1);
            }
        }
        private static double Length2d(double x, double y)
        {
            double len;
            x = Math.Abs(x);
            y = Math.Abs(y);
            if (y > x)
            {
                len = x;
                x = y;
                y = len;
            }

            // 15 September 2003 Dale Lear
            //     For small denormalized doubles (positive but smaller
            //     than DBL_MIN), some compilers/FPUs set 1.0/fx to +INF.
            //     Without the ON_DBL_MIN test we end up with
            //     microscopic vectors that have infinite length!
            //
            //     This code is absolutely necessary.  It is a critical
            //     part of the fix for RR 11217.
            if (x > double.Epsilon)
            {
                len = 1.0 / x;
                y *= len;
                len = x * Math.Sqrt(1.0 + y * y);
            }
            else if (x > 0.0 && !double.IsInfinity(x))
            {
                len = x;
            }
            else
            {
                len = 0.0;
            }

            return len;
        }
        #endregion
        #endregion

        #region methods

        #region evaluation methods
        /// <summary>
        /// Evaluates whether or not this circle is co-planar with a given plane.
        /// </summary>
        /// <param name="plane">Plane.</param>
        /// <param name="tolerance">Tolerance to use.</param>
        /// <returns>true if the circle plane is co-planar with the given plane within tolerance.</returns>
        public bool IsInPlane(Plane plane, double tolerance)
        {
            double d;
            int i;
            var equ = new PlaneEquation(plane.GetPlaneEquation());
            for (i = 0; i < 8; i++)
            {
                d = equ.ValueAt(PointAt(0.25 * i * Math.PI));
                if (Math.Abs(d) > tolerance)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Circles use trigonometric parameterization: 
        /// t -> center + cos(t)*radius*xaxis + sin(t)*radius*yaxis.
        /// </summary>
        /// <param name="t">Parameter of point to evaluate.</param>
        /// <returns>The point on the circle at the given parameter.</returns>
        public Point3D PointAt(double t)
        {
            return m_plane.PointAt(Math.Cos(t) * m_radius, Math.Sin(t) * m_radius);
        }

        /// <summary>
        /// Circles use trigonometric parameterization: 
        /// t -> center + cos(t)*radius*xaxis + sin(t)*radius*yaxis.
        /// </summary>
        /// <param name="t">Parameter of tangent to evaluate.</param>
        /// <returns>The tangent at the circle at the given parameter.</returns>
        public Vector3D TangentAt(double t)
        {
            Vector3D tangent = DerivativeAt(1, t);
            tangent.Normalize();
            return tangent;
        }

        /// <summary>
        /// Determines the value of the Nth derivative at a parameter. 
        /// </summary>
        /// <param name="derivative">Which order of derivative is wanted.</param>
        /// <param name="t">Parameter to evaluate derivative. Valid values are 0, 1, 2 and 3.</param>
        /// <returns>The derivative of the circle at the given parameter.</returns>
        public Vector3D DerivativeAt(int derivative, double t)
        {
            double r0 = m_radius;
            double r1 = m_radius;

            switch (Math.Abs(derivative) % 4)
            {
                case 0:
                    r0 *= Math.Cos(t);
                    r1 *= Math.Sin(t);
                    break;
                case 1:
                    r0 *= -Math.Sin(t);
                    r1 *= Math.Cos(t);
                    break;
                case 2:
                    r0 *= -Math.Cos(t);
                    r1 *= -Math.Sin(t);
                    break;
                case 3:
                    r0 *= Math.Sin(t);
                    r1 *= -Math.Cos(t);
                    break;
            }

            return (r0 * m_plane.XAxis + r1 * m_plane.YAxis);
        }

        // David asks : what is this function used for? It sounds awfully nerdy to me.
        ///// <summary>
        ///// Evaluate circle's implicit equation in plane.
        ///// </summary>
        ///// <param name="p">coordinates in plane.</param>
        ///// <returns>-</returns>
        //public double EquationAt(Point2d p)
        //{
        //  double e, x, y;
        //  if (m_radius != 0.0)
        //  {
        //    x = p.X / m_radius;
        //    y = p.Y / m_radius;
        //    e = x * x + y * y - 1.0;
        //  }
        //  else
        //  {
        //    e = 0.0;
        //  }
        //  return e;
        //}

        // David asks : what is this function used for? It sounds awfully nerdy to me.
        ///// <summary>-</summary>
        ///// <param name="p">coordinates in plane.</param>
        ///// <returns>-</returns>
        //public Vector2d GradientAt(Point2d p)
        //{
        //  Vector2d g = new Vector2d();
        //  if (m_radius != 0.0)
        //  {
        //    double rr = 2.0 / (m_radius * m_radius);
        //    g.X = rr * p.X;
        //    g.Y = rr * p.Y;
        //  }
        //  else
        //  {
        //    g.X = g.Y = 0.0;
        //  }
        //  return g;
        //}

        /// <summary>
        /// Gets the parameter on the circle which is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to project onto the circle.</param>
        /// <param name="t">Parameter on circle closes to testPoint.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool ClosestParameter(Point3D testPoint, out double t)
        {
            t = 0;
            bool rc = true;
            if (rc)
            {
                double u, v;
                rc = m_plane.ClosestParameter(testPoint, out u, out v);
                if (u == 0.0 && v == 0.0)
                {
                    t = 0.0;
                }
                else
                {
                    t = Math.Atan2(v, u);
                    if (t < 0.0)
                        t += 2.0 * Math.PI;
                }
            }
            return rc;
        }

        /// <summary>
        /// Gets the point on the circle which is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to project onto the circle.</param>
        /// <returns>
        /// The point on the circle that is closest to testPoint or
        /// Point3D.Unset on failure.
        /// </returns>
        public Point3D ClosestPoint(Point3D testPoint)
        {
            double t;
            if (!ClosestParameter(testPoint, out t))
                return Point3D.Unset;
            return PointAt(t);
        }
        #endregion

        #region transformation methods
        /// <summary>
        /// Transforms this circle using an xform matrix. 
        /// </summary>
        /// <param name="xform">Transformation to apply.</param>
        /// <returns>true on success, false on failure.</returns>
        /// <remarks>
        /// Circles may not be transformed accurately if the xform defines a 
        /// non-euclidian transformation.
        /// </remarks>
        public bool Transform(Transform xform)
        {
            Plane plane0=new Plane(m_plane);
            bool rc = m_plane.Transform(xform);
            if (!rc)
            {
                // restore original
                m_plane = plane0;
            }
            else
            {
                const double ztol = 1.0e-12;
                double a, b, c, d, r1, r2, s;
                // determine scale factor in circle's m_plane
                // In practice, transformation are either rotations,
                // the scale factor is clearly distinct from 1,
                // or the transformation does not map a circle
                // to a circle.  The code below has tolerance checks
                // so that anything that is close to a rotation gets
                // treated does not change the radius.  If it is
                // clearly a uniform scale in the m_plane of the circle
                // the scale factor is calculated without using a
                // determinant.  Sine "2d scales" are common, it doesn't
                // work well use the cubed root of the xform'd determinant.
                Vector3D V = xform * plane0.XAxis;
                a = V * m_plane.XAxis;
                b = V * m_plane.YAxis;
                if (Math.Abs(a) >= Math.Abs(b))
                {
                    r1 = Math.Abs(a);
                    if (r1 > 0.0)
                    {
                        a = (a > 0.0) ? 1.0 : -1.0;
                        b /= r1;
                        if (Math.Abs(b) <= ztol)
                        {
                            b = 0.0;
                            if (Math.Abs(1.0 - r1) <= ztol)
                                r1 = 1.0;
                        }
                    }
                }
                else
                {
                    r1 = Math.Abs(b);
                    b = (b > 0.0) ? 1.0 : -1.0;
                    a /= r1;
                    if (Math.Abs(a) <= ztol)
                    {
                        a = 0.0;
                        if (Math.Abs(1.0 - r1) <= ztol)
                            r1 = 1.0;
                    }
                }
                V = xform * plane0.YAxis;
                c = V * m_plane.XAxis;
                d = V * m_plane.YAxis;
                if (Math.Abs(d) >= Math.Abs(c))
                {
                    r2 = Math.Abs(d);
                    if (r2 > 0.0)
                    {
                        d = (d > 0.0) ? 1.0 : -1.0;
                        c /= r2;
                        if (Math.Abs(c) <= ztol)
                        {
                            c = 0.0;
                            if (Math.Abs(1.0 - r2) <= ztol)
                                r2 = 1.0;
                        }
                    }
                }
                else
                {
                    r2 = Math.Abs(c);
                    c = (c > 0.0) ? 1.0 : -1.0;
                    d /= r2;
                    if (Math.Abs(d) <= ztol)
                    {
                        d = 0.0;
                        if (Math.Abs(1.0 - r2) <= ztol)
                            r2 = 1.0;
                    }
                }
                if (0.0 == b
                     && 0.0 == c
                     && Math.Abs(r1 - r2) <= Utility.SQRT_EPSILON * (r1 + r2)
                   )
                {
                    // transform is a similarity
                    s = (r1 == r2) ? r1 : (0.5 * (r1 + r2)); // = sqrt(r1*r2) but more accurate
                }
                else
                {
                    // non-uniform scaling or skew in circle's m_plane
                    // do something reasonable
                    s = Math.Sqrt(Math.Abs(r1 * r2 * (a * d - b * c)));
                }

                if (s > 0.0)
                {
                    if (Math.Abs(s - 1.0) > Utility.SQRT_EPSILON)
                        m_radius *= s;
                }
            }
            return rc;
        }

        /// <summary>
        /// Rotates the circle around an axis that starts at the base plane origin.
        /// </summary>
        /// <param name="sinAngle">The value returned by Math.Sin(angle) to compose the rotation.</param>
        /// <param name="cosAngle">The value returned by Math.Cos(angle) to compose the rotation.</param>
        /// <param name="axis">A rotation axis.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double sinAngle, double cosAngle, Vector3D axis)
        {
            return m_plane.Rotate(sinAngle, cosAngle, axis);
        }

        /// <summary>
        /// Rotates the circle around an axis that starts at the provided point.
        /// </summary>
        /// <param name="sinAngle">The value returned by Math.Sin(angle) to compose the rotation.</param>
        /// <param name="cosAngle">The value returned by Math.Cos(angle) to compose the rotation.</param>
        /// <param name="axis">A rotation direction.</param>
        /// <param name="point">A rotation base point.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double sinAngle, double cosAngle, Vector3D axis, Point3D point)
        {
            return m_plane.Rotate(sinAngle, cosAngle, axis, point);
        }

        /// <summary>
        /// Rotates the circle through a given angle.
        /// </summary>
        /// <param name="angle">Angle (in radians) of the rotation.</param>
        /// <param name="axis">Rotation axis.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angle, Vector3D axis)
        {
            return m_plane.Rotate(angle, axis);
        }

        /// <summary>
        /// Rotates the circle through a given angle.
        /// </summary>
        /// <param name="angle">Angle (in radians) of the rotation.</param>
        /// <param name="axis">Rotation axis.</param>
        /// <param name="point">Rotation anchor point.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Rotate(double angle, Vector3D axis, Point3D point)
        {
            return m_plane.Rotate(angle, axis, point);
        }

        /// <summary>
        /// Moves the circle.
        /// </summary>
        /// <param name="delta">Translation vector.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Translate(Vector3D delta)
        {
            return m_plane.Translate(delta);
        }
        #endregion

        /// <summary>
        /// Reverse the orientation of the circle. Changes the domain from [a,b]
        /// to [-b,-a].
        /// </summary>
        public void Reverse()
        {
            m_plane.YAxis = -m_plane.YAxis;
            m_plane.ZAxis = -m_plane.ZAxis;
        }

        public bool EpsilonEquals(Circle other, double epsilon)
        {
            throw new NotImplementedException();
        }
    }
}
#endregion