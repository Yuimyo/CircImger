using CircImger.Common.Utils;
using System.Numerics;

namespace CircImger.Common.Drawings
{
    public class Square : Shape
    {
        public static Square Default => new Square(Vector2.Zero, Origin.TopLeft, new SizeF(16, 16));

        public Vector2 Position = Vector2.Zero;
        public Origin Origin = Origin.TopLeft;

        public override Vector2 TopLeft => Position - VectorHelper.SizeByOrigin(Size, Origin);

        private SizeF size = SizeF.Empty;
        public override SizeF Size => size;

        public Square(Vector2 position, Origin origin, SizeF size)
        {
            this.Position = position;
            this.Origin = origin;
            this.size = size;
        }
    }
}
