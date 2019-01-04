using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolyHigh.Geometry
{
    interface IEpsilonComparable<in T>
    {
        bool EpsilonEquals(T other, double epsilon);
    }
}
