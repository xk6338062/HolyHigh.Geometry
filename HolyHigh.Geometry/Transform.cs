using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Represents the values in a 4x4 transform matrix.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 128)]
    [Serializable]
    public struct Transform : IComparable<Transform>, IEquatable<Transform>
    {
        #region members
        private double m_00, m_01, m_02, m_03;
        private double m_10, m_11, m_12, m_13;
        private double m_20, m_21, m_22, m_23;
        private double m_30, m_31, m_32, m_33;
        #endregion

        #region constructors       

        /// <summary>
        /// Initializes a new transform matrix with a specified value along the diagonal.
        /// </summary>
        /// <param name="diagonalValue">Value to assign to all diagonal cells except M33 which is set to 1.0.</param>
        public Transform(double diagonalValue)
          : this()
        {
            m_00 = diagonalValue;
            m_11 = diagonalValue;
            m_22 = diagonalValue;
            m_33 = 1.0;
        }

        /// <summary>
        /// A frame constructor
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Transform(Point3D p, Vector3D x, Vector3D y, Vector3D z)
        {
            m_00 = x[0]; m_01 = y[0]; m_02 = z[0]; m_03 = p[0];
            m_10 = x[1]; m_11 = y[1]; m_12 = z[1]; m_13 = p[1];
            m_20 = x[2]; m_21 = y[2]; m_22 = z[2]; m_23 = p[2];
            m_30 = 0; m_31 = 0; m_32 = 0; m_33 = 1;
        }


        public static Transform Scale(double d0, double d1, double d2)
        {
            var xf = Identity;
            xf.m_00 = d0;
            xf.m_11 = d1;
            xf.m_22 = d2;
            return xf;
        }

        /// <summary>
        /// Gets a new identity transform matrix. An identity matrix defines no transformation.
        /// </summary>
        public static Transform Identity
        {
            get
            {
                Transform xf = new Transform();
                xf.m_00 = 1.0;
                xf.m_11 = 1.0;
                xf.m_22 = 1.0;
                xf.m_33 = 1.0;
                return xf;
            }
        }

        /// <summary>
        /// Gets an XForm filled with Utility.UnsetValue.
        /// </summary>
        public static Transform Unset
        {
            get
            {
                Transform xf = new Transform();
                xf.m_00 = Utility.UnsetValue;
                xf.m_01 = Utility.UnsetValue;
                xf.m_02 = Utility.UnsetValue;
                xf.m_03 = Utility.UnsetValue;
                xf.m_10 = Utility.UnsetValue;
                xf.m_11 = Utility.UnsetValue;
                xf.m_12 = Utility.UnsetValue;
                xf.m_13 = Utility.UnsetValue;
                xf.m_20 = Utility.UnsetValue;
                xf.m_21 = Utility.UnsetValue;
                xf.m_22 = Utility.UnsetValue;
                xf.m_23 = Utility.UnsetValue;
                xf.m_30 = Utility.UnsetValue;
                xf.m_31 = Utility.UnsetValue;
                xf.m_32 = Utility.UnsetValue;
                xf.m_33 = Utility.UnsetValue;
                return xf;
            }
        }


        public Transform(Transform other)
        {
            m_00 = other.m_00; m_01 = other.m_01; m_02 = other.m_02; m_03 = other.m_03;
            m_10 = other.m_10; m_11 = other.m_11; m_12 = other.m_12; m_13 = other.m_13;
            m_20 = other.m_20; m_21 = other.m_21; m_22 = other.m_22; m_23 = other.m_23;
            m_30 = other.m_30; m_31 = other.m_31; m_32 = other.m_32; m_33 = other.m_33;
        }
        #endregion

        #region static constructors
        /// <summary>
        /// Constructs a new translation (move) transformation. 
        /// </summary>
        /// <param name="motion">Translation (motion) vector.</param>
        /// <returns>A transform matrix which moves geometry along the motion vector.</returns>
        public static Transform Translation(Vector3D motion)
        {
            return Translation(motion.X, motion.Y, motion.Z);
        }

        /// <summary>
        /// Constructs a new translation (move) tranformation. 
        /// Right column is (dx, dy, dz, 1.0).
        /// </summary>
        /// <param name="dx">Distance to translate (move) geometry along the world X axis.</param>
        /// <param name="dy">Distance to translate (move) geometry along the world Y axis.</param>
        /// <param name="dz">Distance to translate (move) geometry along the world Z axis.</param>
        /// <returns>A transform matrix which moves geometry with the specified distances.</returns>
        public static Transform Translation(double dx, double dy, double dz)
        {
            Transform xf = Identity;
            xf.m_03 = dx;
            xf.m_13 = dy;
            xf.m_23 = dz;
            return xf;
        }

        /// <summary>
        /// Constructs a new uniform scaling transformation with a specified scaling anchor point.
        /// </summary>
        /// <param name="anchor">Defines the anchor point of the scaling operation.</param>
        /// <param name="scaleFactor">Scaling factor in all directions.</param>
        /// <returns>A transform matrix which scales geometry uniformly around the anchor point.</returns>
        public static Transform Scale(Point3D anchor, double scaleFactor)
        {
            return Scale(anchor, scaleFactor, scaleFactor, scaleFactor);
        }
        private static Transform Scale(Point3D fixPoint, double xScaleFactor, double yScaleFactor, double zScaleFactor)
        {
            var s = Scale(xScaleFactor, yScaleFactor, zScaleFactor);
            if (xScaleFactor == 0 && yScaleFactor == 0 && zScaleFactor == 0)
                return s;
            var delta = fixPoint - Point3D.Origin;
            var t0 = Translation(-delta);
            var t1 = Translation(delta);
            return t1 * s * t0;
        }

        /// <summary>
        /// Constructs a new non-uniform scaling transformation with a specified scaling anchor point.
        /// </summary>
        /// <param name="plane">Defines the center and orientation of the scaling operation.</param>
        /// <param name="xScaleFactor">Scaling factor along the anchor plane X-Axis direction.</param>
        /// <param name="yScaleFactor">Scaling factor along the anchor plane Y-Axis direction.</param>
        /// <param name="zScaleFactor">Scaling factor along the anchor plane Z-Axis direction.</param>
        /// <returns>A transformation matrix which scales geometry non-uniformly.</returns>
        public static Transform Scale(Plane plane, double xScaleFactor, double yScaleFactor, double zScaleFactor)
        {
            return (xScaleFactor == yScaleFactor && xScaleFactor == zScaleFactor) ?
                Scale(plane.Origin, xScaleFactor) : Shear(plane, xScaleFactor * plane.XAxis, yScaleFactor * plane.YAxis, zScaleFactor * plane.ZAxis);
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="sinAngle">Sin of the rotation angle.</param>
        /// <param name="cosAngle">Cos of the rotation angle.</param>
        /// <param name="rotationAxis">Axis direction of rotation.</param>
        /// <param name="rotationCenter">Center point of rotation.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor point.</returns>
        public static Transform Rotation(double sinAngle, double cosAngle, Vector3D rotationAxis, Point3D rotationCenter)
        {
            Transform xf = Identity;
            while (true)
            {
                if (Math.Abs(sinAngle) >= 1.0 - Utility.SQRT_EPSILON && Math.Abs(cosAngle) <= Utility.SQRT_EPSILON)
                {
                    cosAngle = 0.0;
                    sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
                    break;
                }
                if (Math.Abs(cosAngle) >= 1.0 - Utility.SQRT_EPSILON && Math.Abs(sinAngle) <= Utility.SQRT_EPSILON)
                {
                    cosAngle = (cosAngle < 0.0) ? -1.0 : 1.0;
                    sinAngle = 0.0;
                    break;
                }

                if (Math.Abs(cosAngle * cosAngle + sinAngle * sinAngle - 1.0) > Utility.SQRT_EPSILON)
                {
                    var cs = new Vector2D(cosAngle, sinAngle);
                    if (cs.Normalize())
                    {
                        cosAngle = cs.X;
                        sinAngle = cs.Y;
                    }
                    else
                    {
                        cosAngle = 1.0;
                        sinAngle = 0.0;
                        break;
                        throw new ArgumentException("sinAngle and cosAngle are both zero.");
                    }
                }
                if (Math.Abs(cosAngle) > 1.0 - Utility.EPSILON || Math.Abs(sinAngle) < Utility.EPSILON)
                {
                    cosAngle = (cosAngle < 0.0) ? -1.0 : 1.0;
                    sinAngle = 0.0;
                    break;
                }

                if (Math.Abs(sinAngle) > 1.0 - Utility.EPSILON || Math.Abs(cosAngle) < Utility.EPSILON)
                {
                    cosAngle = 0.0;
                    sinAngle = (sinAngle < 0.0) ? -1.0 : 1.0;
                    break;
                }
                break;
            }

            if (sinAngle != 0.0 || cosAngle != 1.0)
            {
                double one_minus_cosAngle = 1.0 - cosAngle;
                Vector3D a = rotationAxis;
                if (Math.Abs(a.SquaredLength - 1.0) > Utility.EPSILON)
                    a.Normalize();

                xf[0, 0] = a.X * a.X * one_minus_cosAngle + cosAngle;
                xf[0, 1] = a.X * a.Y * one_minus_cosAngle - a.Z * sinAngle;
                xf[0, 2] = a.X * a.Z * one_minus_cosAngle + a.Y * sinAngle;

                xf[1, 0] = a.Y * a.X * one_minus_cosAngle + a.Z * sinAngle;
                xf[1, 1] = a.Y * a.Y * one_minus_cosAngle + cosAngle;
                xf[1, 2] = a.Y * a.Z * one_minus_cosAngle - a.X * sinAngle;

                xf[2, 0] = a.Z * a.X * one_minus_cosAngle - a.Y * sinAngle;
                xf[2, 1] = a.Z * a.Y * one_minus_cosAngle + a.X * sinAngle;
                xf[2, 2] = a.Z * a.Z * one_minus_cosAngle + cosAngle;

                if (rotationCenter.X != 0.0 || rotationCenter.Y != 0.0 || rotationCenter.Z != 0.0)
                {
                    xf[0, 3] = -((xf[0, 0] - 1.0) * rotationCenter.X + xf[0, 1] * rotationCenter.Y + xf[0, 2] * rotationCenter.Z);
                    xf[1, 3] = -(xf[1, 0] * rotationCenter.X + (xf[1, 1] - 1.0) * rotationCenter.Y + xf[1, 2] * rotationCenter.Z);
                    xf[2, 3] = -(xf[2, 0] * rotationCenter.X + xf[2, 1] * rotationCenter.Y + (xf[2, 2] - 1.0) * rotationCenter.Z);
                }

                xf[3, 0] = xf[3, 1] = xf[3, 2] = 0.0;
                xf[3, 3] = 1.0;
            }

            return xf;
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle and rotation center.
        /// </summary>
        /// <param name="angleRadians">Angle (in Radians) of the rotation.</param>
        /// <param name="rotationCenter">Center point of rotation. Rotation axis is vertical.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor point.</returns>
        public static Transform Rotation(double angleRadians, Point3D rotationCenter)
        {
            return Rotation(angleRadians, new Vector3D(0, 0, 1), rotationCenter);
        }

        /// <summary>
        /// Constructs a new rotation transformation with specified angle, rotation center and rotation axis.
        /// </summary>
        /// <param name="angleRadians">Angle (in Radians) of the rotation.</param>
        /// <param name="rotationAxis">Axis direction of rotation operation.</param>
        /// <param name="rotationCenter">Center point of rotation. Rotation axis is vertical.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor point.</returns>
        public static Transform Rotation(double angleRadians, Vector3D rotationAxis, Point3D rotationCenter)
        {
            return Rotation(Math.Sin(angleRadians), Math.Cos(angleRadians), rotationAxis, rotationCenter);
        }

        /// <summary>
        /// Constructs a new rotation transformation with start and end directions and rotation center.
        /// </summary>
        /// <param name="startDirection">A start direction.</param>
        /// <param name="endDirection">An end direction.</param>
        /// <param name="rotationCenter">A rotation center.</param>
        /// <returns>A transformation matrix which rotates geometry around an anchor point.</returns>
        public static Transform Rotation(Vector3D startDirection, Vector3D endDirection, Point3D rotationCenter)
        {
            if (Math.Abs(startDirection.Length - 1.0) > Utility.SQRT_EPSILON)
                startDirection.Normalize();
            if (Math.Abs(endDirection.Length - 1.0) > Utility.SQRT_EPSILON)
                endDirection.Normalize();
            double cos_angle = startDirection * endDirection;
            Vector3D axis = Vector3D.CrossProduct(startDirection, endDirection);
            double sin_angle = axis.Length;
            if (0.0 == sin_angle || !axis.Normalize())
            {
                axis.PerpendicularTo(startDirection);
                axis.Normalize();
                sin_angle = 0.0;
                cos_angle = (cos_angle < 0.0) ? -1.0 : 1.0;
            }
            return Rotation(sin_angle, cos_angle, axis, rotationCenter);
        }

        /// <summary>
        /// Constructs a transformation that maps X0 to X1, Y0 to Y1, Z0 to Z1.
        /// </summary>
        /// <param name="x0">First "from" vector.</param>
        /// <param name="y0">Second "from" vector.</param>
        /// <param name="z0">Third "from" vector.</param>
        /// <param name="x1">First "to" vector.</param>
        /// <param name="y1">Second "to" vector.</param>
        /// <param name="z1">Third "to" vector.</param>
        /// <returns>A rotation transformation value.</returns>
        public static Transform Rotation(Vector3D x0, Vector3D y0, Vector3D z0,
          Vector3D x1, Vector3D y1, Vector3D z1)
        {
            // F0 changes x0,y0,z0 to world X,Y,Z
            Transform F0 = new Transform();
            F0[0, 0] = x0.X; F0[0, 1] = x0.Y; F0[0, 2] = x0.Z;
            F0[1, 0] = y0.X; F0[1, 1] = y0.Y; F0[1, 2] = y0.Z;
            F0[2, 0] = z0.X; F0[2, 1] = z0.Y; F0[2, 2] = z0.Z;
            F0[3, 3] = 1.0;

            // F1 changes world X,Y,Z to x1,y1,z1
            Transform F1 = new Transform();
            F1[0, 0] = x1.X; F1[0, 1] = y1.X; F1[0, 2] = z1.X;
            F1[1, 0] = x1.Y; F1[1, 1] = y1.Y; F1[1, 2] = z1.Y;
            F1[2, 0] = x1.Z; F1[2, 1] = y1.Z; F1[2, 2] = z1.Z;
            F1[3, 3] = 1.0;

            return F1 * F0;
        }

        public static Transform Rotation(Point3D p0, Vector3D x0, Vector3D y0, Vector3D z0, Point3D p1,
          Vector3D x1, Vector3D y1, Vector3D z1)
        {
            var transformation = Translation(-p0.X, -p0.Y, -p0.Z);
            var transformation2 = Rotation(x0, y0, z0, x1, y1, z1);
            var transformation3 = Translation(p1.X, p1.Y, p1.Z);
            return transformation3 * transformation2 * transformation;
        }


        /// <summary>
        /// Create mirror transformation matrix
        /// The mirror transform maps a point Q to 
        /// Q - (2*(Q-P)oN)*N, where
        /// P = pointOnMirrorPlane and N = normalToMirrorPlane.
        /// </summary>
        /// <param name="pointOnMirrorPlane">Point on the mirror plane.</param>
        /// <param name="normalToMirrorPlane">Normal vector to the mirror plane.</param>
        /// <returns>A transformation matrix which mirrors geometry in a specified plane.</returns>
        public static Transform Mirror(Point3D pointOnMirrorPlane, Vector3D normalToMirrorPlane)
        {
            Transform xf = Identity;
            var p = pointOnMirrorPlane;
            var n = normalToMirrorPlane;
            n.Normalize();
            Vector3D v = (2.0 * (n.X * p.X + n.Y * p.Y + n.Z * p.Z)) * n;
            xf.m_00 = 1 - 2.0 * n.X * n.X;
            xf.m_01 = -2.0 * n.X * n.Y;
            xf.m_02 = -2.0 * n.X * n.Z;
            xf.m_03 = v.X;

            xf.m_10 = -2.0 * n.Y * n.X;
            xf.m_11 = 1.0 - 2.0 * n.Y * n.Y;
            xf.m_12 = -2.0 * n.Y * n.Z;
            xf.m_13 = v.Y;

            xf.m_20 = -2.0 * n.Z * n.X;
            xf.m_21 = -2.0 * n.Z * n.Y;
            xf.m_22 = 1.0 - 2.0 * n.Z * n.Z;
            xf.m_23 = v.Z;

            xf.m_30 = 0.0;
            xf.m_31 = 0.0;
            xf.m_32 = 0.0;
            xf.m_33 = 1.0;

            return xf;
        }

        /// <summary>
        /// Constructs a new Mirror transformation.
        /// </summary>
        /// <param name="mirrorPlane">Plane that defines the mirror orientation and position.</param>
        /// <returns>A transformation matrix which mirrors geometry in a specified plane.</returns>
        public static Transform Mirror(Plane mirrorPlane)
        {
            return Mirror(mirrorPlane.Origin, mirrorPlane.ZAxis);
        }

        /// <summary>
        /// Computes a change of basis transformation. A basis change is essentially a remapping 
        /// of geometry from one coordinate system to another.
        /// </summary>
        /// <param name="plane0">Coordinate system in which the geometry is currently described.</param>
        /// <param name="plane1">Target coordinate system in which we want the geometry to be described.</param>
        /// <returns>
        /// A transformation matrix which orients geometry from one coordinate system to another on success.
        /// Transform.Unset on failure.
        /// </returns>
        public static Transform ChangeBasis(Plane plane0, Plane plane1)
        {
            return ChangeBasis(plane0.Origin, plane0.XAxis, plane0.YAxis, plane0.ZAxis, plane1.Origin, plane1.XAxis, plane1.YAxis, plane1.ZAxis);
        }

        /// <summary>
        /// Computes a change of basis transformation. A basis change is essentially a remapping 
        /// of geometry from one coordinate system to another.
        /// </summary>
        /// <param name="initialBasisX">can be any 3d basis.</param>
        /// <param name="initialBasisY">can be any 3d basis.</param>
        /// <param name="initialBasisZ">can be any 3d basis.</param>
        /// <param name="finalBasisX">can be any 3d basis.</param>
        /// <param name="finalBasisY">can be any 3d basis.</param>
        /// <param name="finalBasisZ">can be any 3d basis.</param>
        /// <returns>
        /// A transformation matrix which orients geometry from one coordinate system to another on success.
        /// Transform.Unset on failure.
        /// </returns>
        public static Transform ChangeBasis(Vector3D initialBasisX, Vector3D initialBasisY, Vector3D initialBasisZ, Vector3D finalBasisX, Vector3D finalBasisY, Vector3D finalBasisZ)
        {
            Transform rc = Identity;
            bool success = rc.ChangeBasis1(
              initialBasisX, initialBasisY, initialBasisZ, finalBasisX, finalBasisY, finalBasisZ);
            return success ? rc : Unset;
        }
        /// <summary>
        /// 从世界坐标系转到指定坐标系的转换矩阵
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Transform ChangeBasisAndOrigin(Point3D origin, Vector3D x, Vector3D y, Vector3D z)
        {
            var translation = Translation(Point3D.Origin - origin);
            var rotation = ChangeBasis(Vector3D.XAxis, Vector3D.YAxis, Vector3D.ZAxis, x, y, z);
            return translation * rotation;
        }

        /// <summary>
        /// Computes a change of basis transformation. A basis change is essentially a remapping 
        /// of geometry from one coordinate system to another.
        /// </summary>
        /// <param name="initialBasisX">can be any 3d basis.</param>
        /// <param name="initialBasisY">can be any 3d basis.</param>
        /// <param name="initialBasisZ">can be any 3d basis.</param>
        /// <param name="finalBasisX">can be any 3d basis.</param>
        /// <param name="finalBasisY">can be any 3d basis.</param>
        /// <param name="finalBasisZ">can be any 3d basis.</param>
        /// <returns>
        /// A transformation matrix which orients geometry from one coordinate system to another on success.
        /// Transform.Unset on failure.
        /// </returns>
        private bool ChangeBasis1(Vector3D initialBasisX, Vector3D initialBasisY, Vector3D initialBasisZ, Vector3D finalBasisX, Vector3D finalBasisY, Vector3D finalBasisZ)
        {
            this = Scale(0, 0, 0);
            // Q = a0*x0 + b0*y0 + c0*z0 = a1*x1 + b1*y1 + c1*z1
            // then this transform will map the point (a0,b0,c0) to (a1,b1,c1)
            var x0 = initialBasisX;
            var y0 = initialBasisY;
            var z0 = initialBasisZ;
            var x1 = finalBasisX;
            var y1 = finalBasisY;
            var z1 = finalBasisZ;

            double a, b, c, d;
            a = x1 * y1;
            b = x1 * z1;
            c = y1 * z1;
            double[][] r = new double[3][];
            r[0] = new double[6] { x1 * x1, a, b, x1 * x0, x1 * y0, x1 * z0 };
            r[1] = new double[6] { a, y1 * y1, c, y1 * x0, y1 * y0, y1 * z0 };
            r[2] = new double[6] { b, c, z1 * z1, z1 * x0, z1 * y0, z1 * z0 };

            // row reduce r
            int i0 = (r[0][0] >= r[1][1]) ? 0 : 1;
            if (r[2][2] > r[i0][i0])
                i0 = 2;
            int i1 = (i0 + 1) % 3;
            int i2 = (i1 + 1) % 3;
            if (r[i0][i0] == 0.0)
                return false;
            d = 1.0 / r[i0][i0];
            r[i0][0] *= d;
            r[i0][1] *= d;
            r[i0][2] *= d;
            r[i0][3] *= d;
            r[i0][4] *= d;
            r[i0][5] *= d;
            r[i0][i0] = 1.0;
            if (r[i1][i0] != 0.0)
            {
                d = -r[i1][i0];
                r[i1][0] += d * r[i0][0];
                r[i1][1] += d * r[i0][1];
                r[i1][2] += d * r[i0][2];
                r[i1][3] += d * r[i0][3];
                r[i1][4] += d * r[i0][4];
                r[i1][5] += d * r[i0][5];
                r[i1][i0] = 0.0;
            }
            if (r[i2][i0] != 0.0)
            {
                d = -r[i2][i0];
                r[i2][0] += d * r[i0][0];
                r[i2][1] += d * r[i0][1];
                r[i2][2] += d * r[i0][2];
                r[i2][3] += d * r[i0][3];
                r[i2][4] += d * r[i0][4];
                r[i2][5] += d * r[i0][5];
                r[i2][i0] = 0.0;
            }

            if (Math.Abs(r[i1][i1]) < Math.Abs(r[i2][i2]))
            {
                int i = i1; i1 = i2; i2 = i;
            }
            if (r[i1][i1] == 0.0)
                return false;
            d = 1.0 / r[i1][i1];
            r[i1][0] *= d;
            r[i1][1] *= d;
            r[i1][2] *= d;
            r[i1][3] *= d;
            r[i1][4] *= d;
            r[i1][5] *= d;
            r[i1][i1] = 1.0;
            if (r[i0][i1] != 0.0)
            {
                d = -r[i0][i1];
                r[i0][0] += d * r[i1][0];
                r[i0][1] += d * r[i1][1];
                r[i0][2] += d * r[i1][2];
                r[i0][3] += d * r[i1][3];
                r[i0][4] += d * r[i1][4];
                r[i0][5] += d * r[i1][5];
                r[i0][i1] = 0.0;
            }
            if (r[i2][i1] != 0.0)
            {
                d = -r[i2][i1];
                r[i2][0] += d * r[i1][0];
                r[i2][1] += d * r[i1][1];
                r[i2][2] += d * r[i1][2];
                r[i2][3] += d * r[i1][3];
                r[i2][4] += d * r[i1][4];
                r[i2][5] += d * r[i1][5];
                r[i2][i1] = 0.0;
            }

            if (r[i2][i2] == 0.0)
                return false;
            d = 1.0 / r[i2][i2];
            r[i2][0] *= d;
            r[i2][1] *= d;
            r[i2][2] *= d;
            r[i2][3] *= d;
            r[i2][4] *= d;
            r[i2][5] *= d;
            r[i2][i2] = 1.0;
            if (r[i0][i2] != 0.0)
            {
                d = -r[i0][i2];
                r[i0][0] += d * r[i2][0];
                r[i0][1] += d * r[i2][1];
                r[i0][2] += d * r[i2][2];
                r[i0][3] += d * r[i2][3];
                r[i0][4] += d * r[i2][4];
                r[i0][5] += d * r[i2][5];
                r[i0][i2] = 0.0;
            }
            if (r[i1][i2] != 0.0)
            {
                d = -r[i1][i2];
                r[i1][0] += d * r[i2][0];
                r[i1][1] += d * r[i2][1];
                r[i1][2] += d * r[i2][2];
                r[i1][3] += d * r[i2][3];
                r[i1][4] += d * r[i2][4];
                r[i1][5] += d * r[i2][5];
                r[i1][i2] = 0.0;
            }

            m_00 = r[0][3];
            m_01 = r[0][4];
            m_02 = r[0][5];
            m_10 = r[1][3];
            m_11 = r[1][4];
            m_12 = r[1][5];
            m_20 = r[2][3];
            m_21 = r[2][4];
            m_22 = r[2][5];

            return true;
        }


        public static Transform ChangeBasis(
              Point3D p0,  // initial frame center
              Vector3D x0, // initial frame X (X,Y,Z = arbitrary basis)
              Vector3D y0, // initial frame Y
              Vector3D z0, // initial frame Z
              Point3D p1,  // final frame center
              Vector3D x1, // final frame X (X,Y,Z = arbitrary basis)
              Vector3D y1, // final frame Y
              Vector3D z1)  // final frame Z  
        {
            bool rc = false;
            // Q = P0 + a0*X0 + b0*Y0 + c0*Z0 = P1 + a1*X1 + b1*Y1 + c1*Z1
            // then this transform will map the point (a0,b0,c0) to (a1,b1,c1)

            var f0 = new Transform(p0, x0, y0, z0);        // Frame 0

            // T1 translates by -P1
            var t1 = Translation(-p1.X, -p1.Y, -p1.Z);

            Transform cb = new Transform();
            rc = cb.ChangeBasis1(Vector3D.XAxis, Vector3D.YAxis, Vector3D.ZAxis, x1, y1, z1);
            return rc ? cb * t1 * f0 : Unset;
        }

        /// <summary>
        /// Constructs a projection transformation.
        /// </summary>
        /// <param name="plane">Plane onto which everything will be perpendicularly projected.</param>
        /// <returns>A transformation matrix which projects geometry onto a specified plane.</returns>
        public static Transform PlanarProjection(Plane plane)
        {
            Transform rc = Identity;
            int i, j;
            double[] x = { plane.XAxis.X, plane.XAxis.Y, plane.XAxis.Z };
            double[] y = { plane.YAxis.X, plane.YAxis.Y, plane.YAxis.Z };
            double[] p = { plane.Origin.X, plane.Origin.Y, plane.Origin.Z };
            double[] q = new double[3];
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    rc[i, j] = x[i] * x[j] + y[i] * y[j];
                }
                q[i] = rc[i, 0] * p[0] + rc[i, 1] * p[1] + rc[i, 2] * p[2];
            }
            for (i = 0; i < 3; i++)
            {
                rc[3, i] = 0.0;
                rc[i, 3] = p[i] - q[i];
            }
            rc[3, 3] = 1.0;
            return rc;
        }

        /// <summary>
        /// Constructs a Shear transformation.
        /// </summary>
        /// <param name="plane">Base plane for shear.</param>
        /// <param name="x">Shearing vector along plane x-axis.</param>
        /// <param name="y">Shearing vector along plane y-axis.</param>
        /// <param name="z">Shearing vector along plane z-axis.</param>
        /// <returns>A transformation matrix which shear geometry.</returns>
        public static Transform Shear(Plane plane, Vector3D x, Vector3D y, Vector3D z)
        {
            var delta = plane.Origin - Point3D.Origin;
            var t0 = Translation(-delta);
            var t1 = Translation(delta);
            var s0 = Identity;
            var s1 = Identity;
            s0.m_00 = plane.XAxis.X;
            s0.m_01 = plane.XAxis.Y;
            s0.m_02 = plane.XAxis.Z;
            s0.m_10 = plane.YAxis.X;
            s0.m_11 = plane.YAxis.Y;
            s0.m_12 = plane.YAxis.Z;
            s0.m_20 = plane.ZAxis.X;
            s0.m_21 = plane.ZAxis.Y;
            s0.m_22 = plane.ZAxis.Z;

            s1.m_00 = x.X;
            s1.m_10 = x.Y;
            s1.m_20 = x.Z;
            s1.m_01 = y.X;
            s1.m_11 = y.Y;
            s1.m_21 = y.Z;
            s1.m_02 = z.X;
            s1.m_12 = z.Y;
            s1.m_22 = z.Z;
            return t1 * s1 * s0 * t0;
        }




        // TODO: taper.
        #endregion

        #region operators
        /// <summary>
        /// Determines if two transformations are equal in value.
        /// </summary>
        /// <param name="a">A tranform.</param>
        /// <param name="b">Another tranform.</param>
        /// <returns>true if transforms are equal; otherwise false.</returns>
        public static bool operator ==(Transform a, Transform b)
        {
            return a.m_00 == b.m_00 && a.m_01 == b.m_01 && a.m_02 == b.m_02 && a.m_03 == b.m_03 &&
              a.m_10 == b.m_10 && a.m_11 == b.m_11 && a.m_12 == b.m_12 && a.m_13 == b.m_13 &&
              a.m_20 == b.m_20 && a.m_21 == b.m_21 && a.m_22 == b.m_22 && a.m_23 == b.m_23 &&
              a.m_30 == b.m_30 && a.m_31 == b.m_31 && a.m_32 == b.m_32 && a.m_33 == b.m_33;
        }

        /// <summary>
        /// Determines if two transformations are different in value.
        /// </summary>
        /// <param name="a">A tranform.</param>
        /// <param name="b">Another tranform.</param>
        /// <returns>true if transforms are different; otherwise false.</returns>
        public static bool operator !=(Transform a, Transform b)
        {
            return a.m_00 != b.m_00 || a.m_01 != b.m_01 || a.m_02 != b.m_02 || a.m_03 != b.m_03 ||
              a.m_10 != b.m_10 || a.m_11 != b.m_11 || a.m_12 != b.m_12 || a.m_13 != b.m_13 ||
              a.m_20 != b.m_20 || a.m_21 != b.m_21 || a.m_22 != b.m_22 || a.m_23 != b.m_23 ||
              a.m_30 != b.m_30 || a.m_31 != b.m_31 || a.m_32 != b.m_32 || a.m_33 != b.m_33;
        }

        /// <summary>
        /// Multiplies (combines) two transformations.
        /// </summary>
        /// <param name="a">First transformation.</param>
        /// <param name="b">Second transformation.</param>
        /// <returns>A transformation matrix that combines the effect of both input transformations. 
        /// The resulting Transform gives the same result as though you'd first apply A then B.</returns>
        public static Transform operator *(Transform a, Transform b)
        {
            Transform xf = new Transform();
            xf.m_00 = a.m_00 * b.m_00 + a.m_01 * b.m_10 + a.m_02 * b.m_20 + a.m_03 * b.m_30;
            xf.m_01 = a.m_00 * b.m_01 + a.m_01 * b.m_11 + a.m_02 * b.m_21 + a.m_03 * b.m_31;
            xf.m_02 = a.m_00 * b.m_02 + a.m_01 * b.m_12 + a.m_02 * b.m_22 + a.m_03 * b.m_32;
            xf.m_03 = a.m_00 * b.m_03 + a.m_01 * b.m_13 + a.m_02 * b.m_23 + a.m_03 * b.m_33;

            xf.m_10 = a.m_10 * b.m_00 + a.m_11 * b.m_10 + a.m_12 * b.m_20 + a.m_13 * b.m_30;
            xf.m_11 = a.m_10 * b.m_01 + a.m_11 * b.m_11 + a.m_12 * b.m_21 + a.m_13 * b.m_31;
            xf.m_12 = a.m_10 * b.m_02 + a.m_11 * b.m_12 + a.m_12 * b.m_22 + a.m_13 * b.m_32;
            xf.m_13 = a.m_10 * b.m_03 + a.m_11 * b.m_13 + a.m_12 * b.m_23 + a.m_13 * b.m_33;

            xf.m_20 = a.m_20 * b.m_00 + a.m_21 * b.m_10 + a.m_22 * b.m_20 + a.m_23 * b.m_30;
            xf.m_21 = a.m_20 * b.m_01 + a.m_21 * b.m_11 + a.m_22 * b.m_21 + a.m_23 * b.m_31;
            xf.m_22 = a.m_20 * b.m_02 + a.m_21 * b.m_12 + a.m_22 * b.m_22 + a.m_23 * b.m_32;
            xf.m_23 = a.m_20 * b.m_03 + a.m_21 * b.m_13 + a.m_22 * b.m_23 + a.m_23 * b.m_33;

            xf.m_30 = a.m_30 * b.m_00 + a.m_31 * b.m_10 + a.m_32 * b.m_20 + a.m_33 * b.m_30;
            xf.m_31 = a.m_30 * b.m_01 + a.m_31 * b.m_11 + a.m_32 * b.m_21 + a.m_33 * b.m_31;
            xf.m_32 = a.m_30 * b.m_02 + a.m_31 * b.m_12 + a.m_32 * b.m_22 + a.m_33 * b.m_32;
            xf.m_33 = a.m_30 * b.m_03 + a.m_31 * b.m_13 + a.m_32 * b.m_23 + a.m_33 * b.m_33;
            return xf;
        }

        /// <summary>
        /// Multiplies a transformation by a point and gets a new point.
        /// </summary>
        /// <param name="m">A transformation.</param>
        /// <param name="p">A point.</param>
        /// <returns>The tranformed point.</returns>
        public static Point3D operator *(Transform m, Point3D p)
        {
            double x = p.X; // optimizer should put x,y,z in registers
            double y = p.Y;
            double z = p.Z;
            Point3D rc = new Point3D();
            rc.X = m.m_00 * x + m.m_01 * y + m.m_02 * z + m.m_03;
            rc.Y = m.m_10 * x + m.m_11 * y + m.m_12 * z + m.m_13;
            rc.Z = m.m_20 * x + m.m_21 * y + m.m_22 * z + m.m_23;
            double w = m.m_30 * x + m.m_31 * y + m.m_32 * z + m.m_33;
            if (w != 0.0)
            {
                w = 1.0 / w;
                rc.X *= w;
                rc.Y *= w;
                rc.Z *= w;
            }
            return rc;
        }


        /// <summary>
        /// Multiplies a transformation by a vector and gets a new vector.
        /// </summary>
        /// <param name="m">A transformation.</param>
        /// <param name="v">A vector.</param>
        /// <returns>The tranformed vector.</returns>
        public static Vector3D operator *(Transform m, Vector3D v)
        {
            double x = v.X; // optimizer should put x,y,z in registers
            double y = v.Y;
            double z = v.Z;
            Vector3D rc = new Vector3D();
            rc.X = m.m_00 * x + m.m_01 * y + m.m_02 * z;
            rc.Y = m.m_10 * x + m.m_11 * y + m.m_12 * z;
            rc.Z = m.m_20 * x + m.m_21 * y + m.m_22 * z;
            return rc;
        }

        /// <summary>
        /// Multiplies (combines) two transformations.
        /// <para>This is the same as the * operator between two transformations.</para>
        /// </summary>
        /// <param name="a">First transformation.</param>
        /// <param name="b">Second transformation.</param>
        /// <returns>A transformation matrix that combines the effect of both input transformations. 
        /// The resulting Transform gives the same result as though you'd first apply A then B.</returns>
        public static Transform Multiply(Transform a, Transform b)
        {
            return a * b;
        }
        #endregion

        #region properties
        #region accessor properties
        /// <summary>Gets or sets this[0,0].</summary>
        public double M00 { get { return m_00; } set { m_00 = value; } }
        /// <summary>Gets or sets this[0,1].</summary>
        public double M01 { get { return m_01; } set { m_01 = value; } }
        /// <summary>Gets or sets this[0,2].</summary>
        public double M02 { get { return m_02; } set { m_02 = value; } }
        /// <summary>Gets or sets this[0,3].</summary>
        public double M03 { get { return m_03; } set { m_03 = value; } }

        /// <summary>Gets or sets this[1,0].</summary>
        public double M10 { get { return m_10; } set { m_10 = value; } }
        /// <summary>Gets or sets this[1,1].</summary>
        public double M11 { get { return m_11; } set { m_11 = value; } }
        /// <summary>Gets or sets this[1,2].</summary>
        public double M12 { get { return m_12; } set { m_12 = value; } }
        /// <summary>Gets or sets this[1,3].</summary>
        public double M13 { get { return m_13; } set { m_13 = value; } }

        /// <summary>Gets or sets this[2,0].</summary>
        public double M20 { get { return m_20; } set { m_20 = value; } }
        /// <summary>Gets or sets this[2,1].</summary>
        public double M21 { get { return m_21; } set { m_21 = value; } }
        /// <summary>Gets or sets this[2,2].</summary>
        public double M22 { get { return m_22; } set { m_22 = value; } }
        /// <summary>Gets or sets this[2,3].</summary>
        public double M23 { get { return m_23; } set { m_23 = value; } }

        /// <summary>Gets or sets this[3,0].</summary>
        public double M30 { get { return m_30; } set { m_30 = value; } }
        /// <summary>Gets or sets this[3,1].</summary>
        public double M31 { get { return m_31; } set { m_31 = value; } }
        /// <summary>Gets or sets this[3,2].</summary>
        public double M32 { get { return m_32; } set { m_32 = value; } }
        /// <summary>Gets or sets this[3,3].</summary>
        public double M33 { get { return m_33; } set { m_33 = value; } }

        /// <summary>
        /// Gets or sets the matrix value at the given row and column indixes.
        /// </summary>
        /// <param name="row">Index of row to access, must be 0, 1, 2 or 3.</param>
        /// <param name="column">Index of column to access, must be 0, 1, 2 or 3.</param>
        /// <returns>The value at [row, column]</returns>
        /// <value>The new value at [row, column]</value>
        public double this[int row, int column]
        {
            get
            {
                if (row < 0) { throw new IndexOutOfRangeException("Negative row indices are not allowed when accessing a Transform matrix"); }
                if (row > 3) { throw new IndexOutOfRangeException("Row indices higher than 3 are not allowed when accessing a Transform matrix"); }
                if (column < 0) { throw new IndexOutOfRangeException("Negative column indices are not allowed when accessing a Transform matrix"); }
                if (column > 3) { throw new IndexOutOfRangeException("Column indices higher than 3 are not allowed when accessing a Transform matrix"); }

                if (row == 0)
                {
                    if (column == 0) { return m_00; }
                    if (column == 1) { return m_01; }
                    if (column == 2) { return m_02; }
                    if (column == 3) { return m_03; }
                }
                else if (row == 1)
                {
                    if (column == 0) { return m_10; }
                    if (column == 1) { return m_11; }
                    if (column == 2) { return m_12; }
                    if (column == 3) { return m_13; }
                }
                else if (row == 2)
                {
                    if (column == 0) { return m_20; }
                    if (column == 1) { return m_21; }
                    if (column == 2) { return m_22; }
                    if (column == 3) { return m_23; }
                }
                else if (row == 3)
                {
                    if (column == 0) { return m_30; }
                    if (column == 1) { return m_31; }
                    if (column == 2) { return m_32; }
                    if (column == 3) { return m_33; }
                }

                throw new IndexOutOfRangeException("One of the cross beams has gone out askew on the treadle.");
            }
            set
            {
                if (row < 0) { throw new IndexOutOfRangeException("Negative row indices are not allowed when accessing a Transform matrix"); }
                if (row > 3) { throw new IndexOutOfRangeException("Row indices higher than 3 are not allowed when accessing a Transform matrix"); }
                if (column < 0) { throw new IndexOutOfRangeException("Negative column indices are not allowed when accessing a Transform matrix"); }
                if (column > 3) { throw new IndexOutOfRangeException("Column indices higher than 3 are not allowed when accessing a Transform matrix"); }

                if (row == 0)
                {
                    if (column == 0)
                    { m_00 = value; }
                    else if (column == 1)
                    { m_01 = value; }
                    else if (column == 2)
                    { m_02 = value; }
                    else if (column == 3)
                    { m_03 = value; }
                }
                else if (row == 1)
                {
                    if (column == 0)
                    { m_10 = value; }
                    else if (column == 1)
                    { m_11 = value; }
                    else if (column == 2)
                    { m_12 = value; }
                    else if (column == 3)
                    { m_13 = value; }
                }
                else if (row == 2)
                {
                    if (column == 0)
                    { m_20 = value; }
                    else if (column == 1)
                    { m_21 = value; }
                    else if (column == 2)
                    { m_22 = value; }
                    else if (column == 3)
                    { m_23 = value; }
                }
                else if (row == 3)
                {
                    if (column == 0)
                    { m_30 = value; }
                    else if (column == 1)
                    { m_31 = value; }
                    else if (column == 2)
                    { m_32 = value; }
                    else if (column == 3)
                    { m_33 = value; }
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether or not this Transform is a valid matrix. 
        /// A valid transform matrix is not allowed to have any invalid numbers.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool rc = Utility.IsValidDouble(m_00) && Utility.IsValidDouble(m_01) && Utility.IsValidDouble(m_02) && Utility.IsValidDouble(m_03) &&
                          Utility.IsValidDouble(m_10) && Utility.IsValidDouble(m_11) && Utility.IsValidDouble(m_12) && Utility.IsValidDouble(m_13) &&
                          Utility.IsValidDouble(m_20) && Utility.IsValidDouble(m_21) && Utility.IsValidDouble(m_22) && Utility.IsValidDouble(m_23) &&
                          Utility.IsValidDouble(m_30) && Utility.IsValidDouble(m_31) && Utility.IsValidDouble(m_22) && Utility.IsValidDouble(m_33);
                return rc;
            }
        }

        ///// <summary>
        ///// Gets a value indicating whether or not the Transform maintains similarity. 
        ///// The easiest way to think of Similarity is that any circle, when transformed, 
        ///// remains a circle. Whereas a non-similarity Transform deforms circles into ellipses.
        ///// </summary>
        //public TransformSimilarityType SimilarityType
        //{
        //    get
        //    {
        //        int rc = UnsafeNativeMethods.ON_Xform_IsSimilarity(ref this);
        //        return (TransformSimilarityType)rc;
        //    }
        //}

        /// <summary>
        /// The determinant of this 4x4 matrix.
        /// </summary>
        public double Determinant
        {
            get
            {
                return M00 * GetCofactor(M11, M12, M13, M21, M22, M23, M31, M32, M33) -
                       M01 * GetCofactor(M10, M12, M13, M20, M22, M23, M30, M32, M33) +
                       M02 * GetCofactor(M10, M11, M13, M20, M21, M23, M30, M31, M33) -
                       M03 * GetCofactor(M10, M11, M12, M20, M21, M22, M30, M31, M32);
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Computes a new boundingbox that is the smallest axis aligned
        /// boundingbox that contains the transformed result of its 8 original corner
        /// points.
        /// </summary>
        /// <returns>A new bounding box.</returns>
        public BoundingBox TransformBoundingBox(BoundingBox bbox)
        {
            BoundingBox rc = bbox;
            rc.Transform(this);
            return rc;
        }

        /// <summary>
        /// Given a list, an array or any enumerable set of points, computes a new array of tranformed points.
        /// </summary>
        /// <param name="points">A list, an array or any enumerable set of points to be left untouched and copied.</param>
        /// <returns>A new array.</returns>
        public Point3D[] TransformList(IEnumerable<Point3D> points)
        {
            List<Point3D> rc = new List<Point3D>(points);
            for (int i = 0; i < rc.Count; i++)
            {
                Point3D pt = rc[i];
                pt.TransformBy(this);
                rc[i] = pt;
            }
            return rc.ToArray();
        }

        /// <summary>
        /// Determines if another object is a transform and its value equals this transform value.
        /// </summary>
        /// <param name="obj">Another object.</param>
        /// <returns>true if obj is a transform and has the same value as this transform; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is Transform && Equals((Transform)obj);
        }

        /// <summary>
        /// Determines if another transform equals this transform value.
        /// </summary>
        /// <param name="other">Another transform.</param>
        /// <returns>true if other has the same value as this transform; otherwise, false.</returns>
        public bool Equals(Transform other)
        {
            return this == other;
        }

        /// <summary>
        /// Gets a non-unique hashing code for this transform.
        /// </summary>
        /// <returns>A number that can be used to hash this transform in a dictionary.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return m_00.GetHashCode() ^ m_01.GetHashCode() ^ m_02.GetHashCode() ^ m_03.GetHashCode() ^
                   m_10.GetHashCode() ^ m_11.GetHashCode() ^ m_12.GetHashCode() ^ m_13.GetHashCode() ^
                   m_20.GetHashCode() ^ m_21.GetHashCode() ^ m_22.GetHashCode() ^ m_23.GetHashCode() ^
                   m_30.GetHashCode() ^ m_31.GetHashCode() ^ m_32.GetHashCode() ^ m_33.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this transform.
        /// </summary>
        /// <returns>A textual representation.</returns>
        public override string ToString()
        {
            StringBuilder sb = new System.Text.StringBuilder();
            IFormatProvider provider = System.Globalization.CultureInfo.InvariantCulture;
            sb.AppendFormat("R0=({0},{1},{2},{3}),", m_00.ToString(provider), m_01.ToString(provider), m_02.ToString(provider), m_03.ToString(provider));
            sb.AppendFormat(" R1=({0},{1},{2},{3}),", m_10.ToString(provider), m_11.ToString(provider), m_12.ToString(provider), m_13.ToString(provider));
            sb.AppendFormat(" R2=({0},{1},{2},{3}),", m_20.ToString(provider), m_21.ToString(provider), m_22.ToString(provider), m_23.ToString(provider));
            sb.AppendFormat(" R3=({0},{1},{2},{3})", m_30.ToString(provider), m_31.ToString(provider), m_32.ToString(provider), m_33.ToString(provider));
            return sb.ToString();
        }
        /// <summary>
        /// Attempts to get the inverse transform of this transform.
        /// </summary>
        /// <param name="inverseTransform">The inverse transform. This out reference will be assigned during this call.</param>
        /// <returns>
        /// true on success. 
        /// If false is returned and this Transform is Invalid, inserveTransform will be set to this Transform. 
        /// If false is returned and this Transform is Valid, inverseTransform will be set to a pseudo inverse.
        /// </returns>
        public bool TryGetInverse(out Transform inverseTransform)
        {
            inverseTransform = this;
            bool rc = false;
            if (IsValid)
            {
                // get cofactors of minor matrices
                double cofactor0 = GetCofactor(M11, M12, M13, M21, M22, M23, M31, M32, M33);
                double cofactor1 = GetCofactor(M10, M12, M13, M20, M22, M23, M30, M32, M33);
                double cofactor2 = GetCofactor(M10, M11, M13, M20, M21, M23, M30, M31, M33);
                double cofactor3 = GetCofactor(M10, M11, M12, M20, M21, M22, M30, M31, M32);

                if (Math.Abs(Determinant) <= Utility.EPSILON)
                {
                    return false;
                }

                // get rest of cofactors for adj(M)
                double cofactor4 = GetCofactor(M01, M02, M03, M21, M22, M23, M31, M32, M33);
                double cofactor5 = GetCofactor(M00, M02, M03, M20, M22, M23, M30, M32, M33);
                double cofactor6 = GetCofactor(M00, M01, M03, M20, M21, M23, M30, M31, M33);
                double cofactor7 = GetCofactor(M00, M01, M02, M20, M21, M22, M30, M31, M32);

                double cofactor8 = GetCofactor(M01, M02, M03, M11, M12, M13, M31, M32, M33);
                double cofactor9 = GetCofactor(M00, M02, M03, M10, M12, M13, M30, M32, M33);
                double cofactor10 = GetCofactor(M00, M01, M03, M10, M11, M13, M30, M31, M33);
                double cofactor11 = GetCofactor(M00, M01, M02, M10, M11, M12, M30, M31, M32);

                double cofactor12 = GetCofactor(M01, M02, M03, M11, M12, M13, M21, M22, M23);
                double cofactor13 = GetCofactor(M00, M02, M03, M10, M12, M13, M20, M22, M23);
                double cofactor14 = GetCofactor(M00, M01, M03, M10, M11, M13, M20, M21, M23);
                double cofactor15 = GetCofactor(M00, M01, M02, M10, M11, M12, M20, M21, M22);

                // build inverse matrix = adj(M) / det(M)
                // adjugate of M is the transpose of the cofactor matrix of M
                double invDeterminant = 1.0 / Determinant;
                inverseTransform.M00 = invDeterminant * cofactor0;
                inverseTransform.M01 = -invDeterminant * cofactor4;
                inverseTransform.M02 = invDeterminant * cofactor8;
                inverseTransform.M03 = -invDeterminant * cofactor12;

                inverseTransform.M10 = -invDeterminant * cofactor1;
                inverseTransform.M11 = invDeterminant * cofactor5;
                inverseTransform.M12 = -invDeterminant * cofactor9;
                inverseTransform.M13 = invDeterminant * cofactor13;

                inverseTransform.M20 = invDeterminant * cofactor2;
                inverseTransform.M21 = -invDeterminant * cofactor6;
                inverseTransform.M22 = invDeterminant * cofactor10;

                inverseTransform.M30 = -invDeterminant * cofactor3;
                inverseTransform.M31 = invDeterminant * cofactor7;
                inverseTransform.M32 = -invDeterminant * cofactor11;
                inverseTransform.M33 = invDeterminant * cofactor15;
                rc = true;
            }
            return rc;
        }

        /// <summary>
        /// Flip row/column values
        /// </summary>
        /// <returns></returns>
        public Transform Transpose()
        {
            Transform rc = new Transform();
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    rc[r, c] = this[c, r];
                }
            }
            return rc;
        }

        /// <summary>
        /// Return the matrix as a linear array of 16 float values
        /// </summary>
        /// <param name="rowDominant"></param>
        /// <returns></returns>
        public float[] ToFloatArray(bool rowDominant)
        {
            var rc = new float[16];

            if (rowDominant)
            {
                rc[0] = (float)m_00; rc[1] = (float)m_01; rc[2] = (float)m_02; rc[3] = (float)m_03;
                rc[4] = (float)m_10; rc[5] = (float)m_11; rc[6] = (float)m_12; rc[7] = (float)m_13;
                rc[8] = (float)m_20; rc[9] = (float)m_21; rc[10] = (float)m_22; rc[11] = (float)m_23;
                rc[12] = (float)m_30; rc[13] = (float)m_31; rc[14] = (float)m_32; rc[15] = (float)m_33;
            }
            else
            {
                rc[0] = (float)m_00; rc[1] = (float)m_10; rc[2] = (float)m_20; rc[3] = (float)m_30;
                rc[4] = (float)m_01; rc[5] = (float)m_11; rc[6] = (float)m_21; rc[7] = (float)m_31;
                rc[8] = (float)m_02; rc[9] = (float)m_12; rc[10] = (float)m_22; rc[11] = (float)m_32;
                rc[12] = (float)m_03; rc[13] = (float)m_13; rc[14] = (float)m_23; rc[15] = (float)m_33;
            }

            return rc;
        }

        #endregion

        #region Other

        private double GetCofactor(double m0, double m1, double m2,
                           double m3, double m4, double m5,
                           double m6, double m7, double m8) => m0 * (m4 * m8 - m5 * m7) -
                       m1 * (m3 * m8 - m5 * m6) +
                       m2 * (m3 * m7 - m4 * m6);

        /// <summary>
        /// Compares this transform with another transform.
        /// <para>M33 has highest value, then M32, etc..</para>
        /// </summary>
        /// <param name="other">Another transform.</param>
        /// <returns>-1 if this &lt; other; 0 if both are equal; 1 otherwise.</returns>
        public int CompareTo(Transform other)
        {
            for (int i = 3; i >= 0; i--)
            {
                for (int j = 3; j >= 0; j--)
                {
                    if (this[i, j] < other[i, j]) return -1;
                    if (this[i, j] < other[i, j]) return 1;
                }
            }
            return 0;
        }

        #endregion
    }
   
}
