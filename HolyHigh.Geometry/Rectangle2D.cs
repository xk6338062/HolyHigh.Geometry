using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    public class Rectangle2D
    {
        public Rectangle2D(Line2D middleLine2D, double width)
        {

        }

        private Line2D _middleLine2D;
        public Line2D MiddleLine2D
        {
            get { return _middleLine2D; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _middleLine2D = value;
            }
        }
        public double Width { get; set; }
        public IEnumerable<Point2D> GetVertexes()
        {
            if (MiddleLine2D == null) yield break;
            var dir = MiddleLine2D.Direction;
            if (dir == null) yield break;
            var verticalVector = new Vector2D(-dir.Y, dir.X) * Width / 2;
            yield return MiddleLine2D.Start + verticalVector;
            yield return MiddleLine2D.Start - verticalVector;
            yield return MiddleLine2D.End - verticalVector;
            yield return MiddleLine2D.End + verticalVector;
        }

        public IEnumerable<Line2D> GetLines()
        {
            if (MiddleLine2D == null) yield break;
            var vertexes = GetVertexes();
            if (vertexes == null) yield break;
            Point2D? firstVertex = null;
            Point2D? lastVertex = null;
            foreach (var vertex in vertexes)
            {
                if (firstVertex == null) firstVertex = vertex;
                if (lastVertex != null) yield return new Line2D(lastVertex.Value, vertex);
                lastVertex = vertex;
                if (lastVertex != firstVertex) yield return new Line2D(lastVertex.Value, firstVertex.Value);
            }
        }

        public Point2D? Center
        {
            get
            {
                if (MiddleLine2D == null) return null;
                return MiddleLine2D.PointAt(0.5);
            }
        }

        public static Rectangle2D operator +(Rectangle2D rectangel2D,Vector2D offset)
        {
            return new Rectangle2D(rectangel2D.MiddleLine2D.Offset(offset), rectangel2D.Width);
        }

        public bool Contains(Point2D point)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            return point.IsInRegion(GetLines().ToList());
        }
    }
}
