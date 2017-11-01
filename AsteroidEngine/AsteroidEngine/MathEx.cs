using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidEngine
{
    public static class MathEx
    {
        public static float Min(params float[] values)
        {
            return values.Min();
        }

        public static float Max(params float[] values)
        {
            return values.Max();
        }

        public static float Encapsulate(float param, float min, float max)
        {
            return MathEx.Min(MathEx.Max(min, param), max);
        }
    }
}
