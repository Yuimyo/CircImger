using CircImger.Common.Utils;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CircImger.Common.Drawings
{
    public class TextureDrawer<TPixel>
    where TPixel : unmanaged, IPixel<TPixel>
    {
        public Shape Shape = Square.Default;
        public readonly List<Texture2d<TPixel>> Textures = new();

        public TextureDrawer(Shape shape, params Texture2d<TPixel>[] textures) 
        {
            this.Shape = shape;
            this.Textures.AddRange(textures);
        }

        public void Draw(ref Image<TPixel> canvas) => Draw(ref canvas, Vector2.Zero, Color.White, DrawingMode.Normal);
        public void Draw(ref Image<TPixel> canvas, Vector2 offset) => Draw(ref canvas, offset, Color.White, DrawingMode.Normal);
        public void Draw(ref Image<TPixel> canvas, Color color) => Draw(ref canvas, Vector2.Zero, color, DrawingMode.Normal);
        public void Draw(ref Image<TPixel> canvas, Vector2 offset, Color color, DrawingMode mode)
        {
            foreach (var texture in Textures)
                texture.Draw(ref canvas, Shape.Rect, offset, color, mode);
        }
    }
}
