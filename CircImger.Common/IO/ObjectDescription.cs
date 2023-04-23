using System.Numerics;

namespace CircImger.Common.IO
{
    public class ObjectDescription : IFormattableToCsv
    {
        public static string Filename => "obj.csv";

        public Vector2 Position = Vector2.Zero;
        public uint Number = 1;
        public Color Color = Color.White;

        public ObjectDescription() { }
        public ObjectDescription(Vector2 position, uint number, Color color)
        {
            Position = position;
            Number = number; // TODO: 大きい数を入れるとレンダリングが上手くいかないのを直す。
            Color = color;
        }

        public static ObjectDescription? TryParse(string line)
        {
            string[] split = line.Split(',');
            if (split.Length < 3) return null;

            var colSplit = split[0].Split(':');
            if (colSplit.Length != 4) return null;
            byte[] rgba = new byte[4];
            for (int i = 0; i < 4; i++)
                rgba[i] = byte.Parse(colSplit[i]);
            var color = Color.FromRgba(rgba[0], rgba[1], rgba[2], rgba[3]);

            var number = uint.Parse(split[1]);

            var coords = new List<Vector2>();
            for (int i = 2; i < split.Length; i++)
            {
                var coordSplit = split[i].Split(':');
                if (coordSplit.Length != 2) return null;
                int x = int.Parse(coordSplit[0]);
                int y = int.Parse(coordSplit[1]);
                coords.Add(new Vector2(x, y));
            }

            return new ObjectDescription(coords[0], number, color); // TODO: coords(複数)

        }

        public string ToCsv()
        {
            var color = Color.ToPixel<Rgba32>();
            return string.Join(',',
                string.Join(':', color.R, color.G, color.B, color.A),
                Number,
                string.Join(':', Position.X, Position.Y)
            );
        }

        public static string FormatDescription => "R:G:B:A, Number, X:Y";
    }
}
