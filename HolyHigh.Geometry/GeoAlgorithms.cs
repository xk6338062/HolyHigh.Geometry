using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    public static class GeoAlgorithms
    {
        public static PolygonLocation PointInPolygon(Point2D p, Point2D[] polygon, double epsilon)
        {
            // number of right & left crossings of edge & ray
            int rightCrossings = 0, leftCrossings = 0;

            // last vertex is starting point for first edge
            int lastIndex = polygon.Length - 1;
            double x1 = polygon[lastIndex].X - p.X, y1 = polygon[lastIndex].Y - p.Y;
            int dy1 = Utility.Compare(y1, 0, epsilon);

            for (int i = 0; i < polygon.Length; i++)
            {
                double x0 = polygon[i].X - p.X, y0 = polygon[i].Y - p.Y;

                int dx0 = Utility.Compare(x0, 0, epsilon);
                int dy0 = Utility.Compare(y0, 0, epsilon);

                // check if q matches current vertex
                if (dx0 == 0 && dy0 == 0)
                    return PolygonLocation.Vertex;

                // check if current edge straddles x-axis
                bool rightStraddle = ((dy0 > 0) != (dy1 > 0));
                bool leftStraddle = ((dy0 < 0) != (dy1 < 0));

                // determine intersection of edge with x-axis
                if (rightStraddle || leftStraddle)
                {
                    double x = (x0 * y1 - x1 * y0) / (y1 - y0);
                    int dx = Utility.Compare(x, 0, epsilon);

                    if (rightStraddle && dx > 0) ++rightCrossings;
                    if (leftStraddle && dx < 0) ++leftCrossings;
                }

                // move starting point for next edge
                x1 = x0; y1 = y0; dy1 = dy0;
            }

            // q is on edge if crossings are of different parity
            if (rightCrossings % 2 != leftCrossings % 2)
                return PolygonLocation.Edge;

            // q is inside for an odd number of crossings, else outside
            return (rightCrossings % 2 == 1 ?
                PolygonLocation.Inside : PolygonLocation.Outside);
        }

        public enum PolygonLocation
        {
            Inside,
            Outside,
            Edge,
            Vertex
        }
    }
}
