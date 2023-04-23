using System.Numerics;

namespace CircImger.Common.Drawings
{
    public class Circle : Shape
    {
        private Vector2 center = Vector2.Zero;
        private float radius = 0;

        public float Radius
        {
            get { return radius; }
            set
            {
                if (value < 0) throw new InvalidDataException($"{value} is expected to not be less than 0.");
                radius = value;
            }
        }
        public override Vector2 TopLeft => center - new Vector2(radius, radius); // top left
        public override SizeF Size => new SizeF(radius * 2, radius * 2);

        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

    }
}
