using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    /// <summary>
    /// Plane equation definition.
    /// </summary>
    public struct PlaneEquation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double D { get; set; }
        /// <summary>
        /// Plane's coefficients constructor. Equation form: ax + by + cz + d = 0.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="d"></param>
        public PlaneEquation(double x, double y, double z, double d)
        {
            X = x;
            Y = y;
            Z = z;
            D = d;
            var v = new Vector3D(X, Y, Z);
            var length = v.Length;
            if (Math.Abs(1 - length) > Utility.EPSILON)
            {
                if (v.Normalize())
                {
                    X = v.X;
                    Y = v.Y;
                    Z = v.Z;
                    D = D / length;
                }
                else throw new ArgumentException();
            }
        }
        public PlaneEquation(double[] para)
        {
            X = para[0];
            Y = para[1];
            Z = para[2];
            D = para[3];
            var v = new Vector3D(X, Y, Z);
            var length = v.Length;
            if (Math.Abs(1 - length) > Utility.EPSILON)
            {
                if (v.Normalize())
                {
                    X = v.X;
                    Y = v.Y;
                    Z = v.Z;
                    D = D / length;
                }
                else throw new ArgumentException();
            }
        }

        public PlaneEquation(Point3D point, Vector3D normal) : this()
        {
            if (!Create(point, normal))
                throw new ArgumentException();
        }

        public bool Create(Point3D point, Vector3D normal)
        {
            bool rc = false;
            if (point.IsValid && normal.IsValid)
            {
                X = normal.X;
                Y = normal.Y;
                Z = normal.Z;
                Vector3D v = new Vector3D(X, Y, Z);
                rc = (Math.Abs(1.0 - v.Length) > Utility.EPSILON) ? v.Normalize() : true;
                D = -(X * point.X + Y * point.Y + Z * point.Z);
            }
            return rc;
        }

        public double ValueAt(Point3D p)
        {
            return (X * p.X + Y * p.Y + Z * p.Z + D);
        }

        public bool IsValid
        {
            get
            {
                return Utility.IsValidDouble(X) && Utility.IsValidDouble(Y) && Utility.IsValidDouble(Z) && Utility.IsValidDouble(D) && (X != 0.0 || Y != 0.0 || Z != 0.0);
            }

        }
        public Vector3D UnitNormal()
        {
            Vector3D normal = new Vector3D(X, Y, Z);
            if (false == normal.IsUnitVector && false == normal.Normalize())
                normal = Vector3D.Zero;
            return normal;
        }
    }
}
