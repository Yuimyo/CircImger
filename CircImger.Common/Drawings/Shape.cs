using System.Numerics;

namespace CircImger.Common.Drawings
{
    public abstract class Shape
    {
        public abstract Vector2 TopLeft { get; }
        public abstract SizeF Size { get; }
        public RectangleF Rect => new RectangleF(TopLeft, Size);

        public Vector2 FromOrigin(Origin origin)
        {
            return TopLeft + new Vector2(Size.Width * ((int)origin % 3) / 2f, Size.Height * ((int)origin / 3) / 2f);
        }
    }
}
