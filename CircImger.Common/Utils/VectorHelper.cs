using CircImger.Common.Drawings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CircImger.Common.Utils
{
    public static class VectorHelper
    {
        public static Vector2 SizeByOrigin(SizeF size, Origin origin)
        {
            return new Vector2(size.Width * ((int)origin % 3) / 2f, size.Height * ((int)origin / 3) / 2f);
        }
    }
}
