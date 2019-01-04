using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{

    /// <summary>
    /// Represents the value of a plane, two angles and a radius in
    /// a subcurve of a three-dimensional circle. 
    /// <para>The curve is parameterized by an angle expressed in radians. For an IsValid arc
    /// the total subtended angle AngleRadians() = Domain()(1) - Domain()(0) must satisfy
    /// 0 &lt; AngleRadians() &lt; 2*Pi</para> 
    /// <para>The parameterization of the Arc is inherited from the Circle it is derived from.
    /// In particular</para>
    /// <para>t -> center + cos(t)*radius*xaxis + sin(t)*radius*yaxis</para>
    /// <para>where xaxis and yaxis, (part of Circle.Plane) form an othonormal frame of the plane
    /// containing the circle.</para>
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 152)]
    [Serializable]
    public struct Arc : IEquatable<Arc>, IEpsilonComparable<Arc>
    {
        #region members
        internal Plane m_plane;
        internal double m_radius;
        internal Interval m_angle;
        #endregion

        #region constants
        /// <summary>
        /// Initializes a new instance of an invalid arc.
        /// </summary>
        static internal Arc Invalid
        {
            get { return new Arc(Plane.WorldXY, 0.0, 0.0); }
        }

        /// <summary>
        /// Gets an Arc with Unset components.
        /// </summary>
        static public Arc Unset
        {
            get
            {
                return new Arc(Plane.Unset, Utility.UnsetValue, 0.0);
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of an arc from a base circle and an angle.
        /// </summary>
        /// <param name="circle">Circle to base arc upon.</param>
        /// <param name="angleRadians">Sweep angle of arc (in radians)</param>
        public Arc(Circle circle, double angleRadians)
          : this()
        {
            //UnsafeNativeMethods.ON_Arc_Create1(ref this, ref circle, angleRadians);
        }

        /// <summary>
        /// Initializes a new instance of an arc from a base circle and an interval of angles.
        /// </summary>
        /// <param name="circle">Circle to base arc upon.</param>
        /// <param name="angleIntervalRadians">
        /// Increasing angle interval in radians with angleIntervalRadians.Length() &lt;= 2.0*Math.PI.
        /// </param>
        public Arc(Circle circle, Interval angleIntervalRadians)
          : this()
        {
            //UnsafeNativeMethods.ON_Arc_Create2(ref this, ref circle, angleIntervalRadians);
        }

        /// <summary>
        /// Initializes a new arc from a base plane, a radius value and an angle.
        /// </summary>
        /// <param name="plane">The plane of the arc (arc center will be located at plane origin)</param>
        /// <param name="radius">Radius of arc.</param>
        /// <param name="angleRadians">Sweep angle of arc (in radians)</param>
        public Arc(Plane plane, double radius, double angleRadians)
          : this()
        {
           // UnsafeNativeMethods.ON_Arc_Create3(ref this, ref plane, radius, angleRadians);
        }

        /// <summary>
        /// Initializes a new horizontal arc at the given center point, with a custom radius and angle.
        /// </summary>
        /// <param name="center">Center point of arc.</param>
        /// <param name="radius">Radius of arc.</param>
        /// <param name="angleRadians">Sweep angle of arc (in radians)</param>
        public Arc(Point3D center, double radius, double angleRadians)
          : this()
        {
            //UnsafeNativeMethods.ON_Arc_Create4(ref this, center, radius, angleRadians);
        }

        /// <summary>
        /// Initializes a new aligned arc at the given center point, with a custom radius and angle.
        /// </summary>
        /// <param name="plane">Alignment plane for arc. The arc will be parallel to this plane.</param>
        /// <param name="center">Center point for arc.</param>
        /// <param name="radius">Radius of arc.</param>
        /// <param name="angleRadians">Sweep angle of arc (in radians)</param>
        public Arc(Plane plane, Point3D center, double radius, double angleRadians)
          : this()
        {
           // UnsafeNativeMethods.ON_Arc_Create5(ref this, ref plane, center, radius, angleRadians);
        }

        /// <summary>
        /// Initializes a new arc through three points. If the points are coincident 
        /// or colinear, this will result in an Invalid arc.
        /// </summary>
        /// <param name="startPoint">Start point of arc.</param>
        /// <param name="pointOnInterior">Point on arc interior.</param>
        /// <param name="endPoint">End point of arc.</param>
        public Arc(Point3D startPoint, Point3D pointOnInterior, Point3D endPoint)
          : this()
        {
            //UnsafeNativeMethods.ON_Arc_Create6(ref this, startPoint, pointOnInterior, endPoint);
        }

        /// <summary>
        /// Initializes a new arc from end points and a tangent vector. 
        /// If the tangent is parallel with the endpoints this will result in an Invalid arc.
        /// </summary>
        /// <param name="pointA">Start point of arc.</param>
        /// <param name="tangentA">Tangent at start of arc.</param>
        /// <param name="pointB">End point of arc.</param>
        public Arc(Point3D pointA, Vector3D tangentA, Point3D pointB)
          : this()
        {
            if (!pointA.IsValid || !tangentA.IsValid || !pointB.IsValid)
            { this = Invalid; return; }

            Vector3D vectorAB = pointB - pointA;

            if (!tangentA.Normalize()) { this = Invalid; return; }
            if (!vectorAB.Normalize()) { this = Invalid; return; }

            if (vectorAB.IsParallelTo(tangentA, 1e-6) != 0) { this = Invalid; return; }

            Vector3D vectorBS = vectorAB + tangentA;
            vectorBS.Normalize();

            vectorBS *= (0.5 * (pointA.DistanceTo(pointB))) / (vectorBS * tangentA);

            this = new Arc(pointA, pointA + vectorBS, pointB);
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether or not this arc is valid.
        /// Detail:
        ///	 Radius&gt;0 and 0&lt;AngleRadians()&lt;=2*Math.Pi.
        /// </summary>
        /// <returns>true if the arc is valid.</returns>
        public bool IsValid
        {
            get
            {
                if (!Utility.IsValidDouble(m_radius)) { return false; }
                return m_radius > 0.0;//&& UnsafeNativeMethods.ON_Arc_IsValid(ref this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this arc is a complete circle.
        /// </summary>
        public bool IsCircle
        {
            get
            {
                return (Math.Abs(Math.Abs(Angle) - (2.0 * Math.PI)) <= Utility.ZeroTolerance);
            }
        }

        #region shape properties
        /// <summary>
        /// Gets or sets the plane in which this arc lies.
        /// </summary>
        public Plane Plane
        {
            get { return m_plane; }
            set { m_plane = value; }
        }

        /// <summary>
        /// Gets or sets the radius of this arc.
        /// </summary>
        public double Radius
        {
            get { return m_radius; }
            set { m_radius = value; }
        }

        /// <summary>
        /// Gets or sets the Diameter of this arc.
        /// </summary>
        public double Diameter
        {
            get { return m_radius * 2.0; }
            set { m_radius = 0.5 * value; }
        }

        /// <summary>
        /// Gets or sets the center point for this arc.
        /// </summary>
        public Point3D Center
        {
            get { return m_plane.Origin; }
            set { m_plane.Origin = value; }
        }

        /// <summary>
        /// Gets the circumference of the circle that is coincident with this arc.
        /// </summary>
        public double Circumference
        {
            get { return Math.Abs(2.0 * Math.PI * m_radius); }
        }

        /// <summary>
        /// Gets the length of the arc. (Length = Radius * (subtended angle in radians)).
        /// </summary>
        public double Length
        {
            get { return Math.Abs(Angle * m_radius); }
        }

        /// <summary>
        /// Gets the start point of the arc.
        /// </summary>
        public Point3D StartPoint
        {
            get { return PointAt(m_angle[0]); }
        }

        /// <summary>
        /// Gets the mid-point of the arc.
        /// </summary>
        public Point3D MidPoint
        {
            get { return PointAt(m_angle.Mid); }
        }

        /// <summary>
        /// Gets the end point of the arc.
        /// </summary>
        public Point3D EndPoint
        {
            get { return PointAt(m_angle[1]); }
        }
        #endregion

        #region angle properties
        /// <summary>
        /// Gets or sets the angle domain (in Radians) of this arc.
        /// </summary>
        public Interval AngleDomain
        {
            get { return m_angle; }
            set { m_angle = value; }
        }

        /// <summary>
        /// Gets or sets the start angle (in Radians) for this arc segment.
        /// </summary>
        public double StartAngle
        {
            get { return m_angle.T0; }
            set { m_angle.T0 = value; }
        }

        /// <summary>
        /// Gets or sets the end angle (in Radians) for this arc segment.
        /// </summary>
        public double EndAngle
        {
            get { return m_angle.T1; }
            set { m_angle.T1 = value; }
        }

        /// <summary>
        /// Gets or sets the sweep -or subtended- angle (in Radians) for this arc segment.
        /// </summary>
        public double Angle
        {
            get { return m_angle.Length; }
            set { m_angle.T1 = m_angle.T0 + value; }
        }

        /// <summary>
        /// Gets or sets the start angle (in Radians) for this arc segment.
        /// </summary>
        public double StartAngleDegrees
        {
            get { return Utility.ToDegrees(StartAngle); }
            set { StartAngle = Utility.ToRadians(value); }
        }

        /// <summary>
        /// Gets or sets the end angle (in Radians) for this arc segment.
        /// </summary>
        public double EndAngleDegrees
        {
            get { return Utility.ToDegrees(EndAngle); }
            set { EndAngle = Utility.ToRadians(value); }
        }

        /// <summary>
        /// Gets or sets the sweep -or subtended- angle (in Radians) for this arc segment.
        /// </summary>
        public double AngleDegrees
        {
            get { return Utility.ToDegrees(Angle); }
            set { Angle = Utility.ToRadians(value); }
        }

        /// <summary>
        /// Sets arc's angle domain (in radians) as a subdomain of the circle.
        /// </summary>
        /// <param name="domain">
        /// 0 &lt; domain[1] - domain[0] &lt;= 2.0 * Utility.Pi.
        /// </param>
        /// <returns>true on success, false on failure.</returns>
        public bool Trim(Interval domain)
        {
            bool rc = false;

            if (domain[0] < domain[1]
              && domain[1] - domain[0] <= 2.0 * Math.PI + Utility.ZeroTolerance)
            {
                m_angle = domain;
                if (m_angle.Length > 2.0 * Math.PI) m_angle[1] = m_angle[0] + 2.0 * Math.PI;
                rc = true;
            }

            return rc;
        }
        #endregion
        #endregion

        #region methods

        /// <summary>
        /// Determines whether another object is an arc and has the same value as this arc.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>true if obj is an arc and is exactly equal to this arc; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Arc && Equals((Arc)obj);
        }

        /// <summary>
        /// Determines whether another arc has the same value as this arc.
        /// </summary>
        /// <param name="other">An arc.</param>
        /// <returns>true if obj is equal to this arc; otherwise false.</returns>
        public bool Equals(Arc other)
        {
            return Math.Abs(m_radius - other.m_radius) < Utility.ZeroTolerance && m_angle == other.m_angle && m_plane == other.m_plane;
        }

        /// <summary>
        /// Computes a hash code for the present arc.
        /// </summary>
        /// <returns>A non-unique integer that represents this arc.</returns>
        public override int GetHashCode()
        {
            return m_radius.GetHashCode() ^ m_angle.GetHashCode() ^ m_plane.GetHashCode();
        }

        /// <summary>
        /// Determines whether two arcs have equal values.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        /// <returns>true if all values of the two arcs are exactly equal; otherwise false.</returns>
        public static bool operator ==(Arc a, Arc b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two arcs have different values.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        /// <returns>true if any value of the two arcs differ; otherwise false.</returns>
        public static bool operator !=(Arc a, Arc b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Computes the 3D axis aligned bounding box for this arc.
        /// </summary>
        /// <returns>Bounding box of arc.</returns>
        public BoundingBox BoundingBox()
        {
            //David changed this on april 16th 2010. We should provide tight boundingboxes for atomic types.
            if (m_angle.IsSingleton)
                return new BoundingBox(StartPoint, StartPoint);

            BoundingBox rc = new BoundingBox();
            //UnsafeNativeMethods.ON_Arc_BoundingBox(ref this, ref rc);
            return rc;
        }

        /// <summary>
        /// Gets the point at the given arc parameter.
        /// </summary>
        /// <param name="t">Arc parameter to evaluate.</param>
        /// <returns>The point at the given parameter.</returns>
        public Point3D PointAt(double t)
        {
            return m_plane.PointAt(Math.Cos(t) * m_radius, Math.Sin(t) * m_radius);
        }

        /// <summary>
        /// Gets the tangent at the given parameter.
        /// </summary>
        /// <param name="t">Parameter of tangent to evaluate.</param>
        /// <returns>The tangent at the arc at the given parameter.</returns>
        public Vector3D TangentAt(double t)
        {
            // David : bit of a hack... shouldn't we write a function that operates directly on arcs?
            Circle circ = new Circle(m_plane, m_radius);
            return circ.TangentAt(t);
        }

        /// <summary>
        /// Gets parameter on the arc closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to get close to.</param>
        /// <returns>Parameter (in radians) of the point on the arc that
        /// is closest to the test point. If testPoint is the center
        /// of the arc, then the starting point of the arc is
        /// (arc.Domain()[0]) returned. If no parameter could be found, 
        /// Utility.UnsetValue is returned.</returns>
        public double ClosestParameter(Point3D testPoint)
        {
            double t = 0;
            return t;// UnsafeNativeMethods.ON_Arc_ClosestPointTo(ref this, testPoint, ref t) ? t : Utility.UnsetValue;
        }

        /// <summary>
        /// Computes the point on an arc that is closest to a test point.
        /// </summary>
        /// <param name="testPoint">Point to get close to.</param>
        /// <returns>
        /// The point on the arc that is closest to testPoint. If testPoint is
        /// the center of the arc, then the starting point of the arc is returned.
        /// UnsetPoint on failure.
        /// </returns>
        public Point3D ClosestPoint(Point3D testPoint)
        {
            double t = ClosestParameter(testPoint);
            return Utility.IsValidDouble(t) ? PointAt(t) : Point3D.Unset;
        }


        /// <summary>
        /// Reverses the orientation of the arc. Changes the domain from [a,b]
        /// to [-b,-a].
        /// </summary>
        public void Reverse()
        {
            m_angle.Reverse();
            m_plane.YAxis = -m_plane.YAxis;
            m_plane.ZAxis = -m_plane.ZAxis;
        }

        /// <summary>
        /// Transforms the arc using a Transformation matrix.
        /// </summary>
        /// <param name="xform">Transformations to apply. 
        /// Note that arcs cannot handle non-euclidian transformations.</param>
        /// <returns>true on success, false on failure.</returns>
        public bool Transform(Transform xform)
        {
            return true;// UnsafeNativeMethods.ON_Arc_Transform(ref this, ref xform);
        }
        #endregion

        /// <summary>
        /// Check that all values in other are within epsilon of the values in this
        /// </summary>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool EpsilonEquals(Arc other, double epsilon)
        {
            return Utility.EpsilonEquals(m_radius, other.m_radius, epsilon) &&
                   m_plane.EpsilonEquals(other.m_plane, epsilon) &&
                   m_angle.EpsilonEquals(other.m_angle, epsilon);
        }
    }
}
