using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    public static class Extension
    {
        
        /// <summary>
        /// 判断一个点是否在区域内部（不在边上）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsInRegion(this Point2D point,List<Line2D> lines)
        {
            var polygon = lines.Select(x => x.Start).ToArray();
            return GeoAlgorithms.PointInPolygon(point, polygon, Utility.TOL6) == GeoAlgorithms.PolygonLocation.Inside;
        }
    }
}
