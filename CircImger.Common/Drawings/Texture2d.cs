using Serilog;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Numerics;

namespace CircImger.Common.Drawings
{
    public class Texture2d<TPixel> : IDisposable
    where TPixel : unmanaged, IPixel<TPixel>
    {
        public static Texture2d<TPixel> Empty => new Texture2d<TPixel>();


        private Image? image = null;
        public Size ActualSize
        {
            get
            {
                if (image == null) return SixLabors.ImageSharp.Size.Empty;
                return image.Size;
            }
        }

        private SizeF size = SizeF.Empty;
        public SizeF Size { get => size; set => size = value; }

        private bool isEmpty = false;
        private Texture2d()
        {
            this.isEmpty = true;
        }

        public Texture2d(string path)
        {
            this.image = Image<Rgba32>.Load(path);
            this.size = ActualSize;            
        }

        public void Draw(ref Image<TPixel> canvas, RectangleF rect)
            => Draw(ref canvas, rect, Vector2.Zero, Color.White);
        public void Draw(ref Image<TPixel> canvas, RectangleF rect, Color color)
            => Draw(ref canvas, rect, Vector2.Zero, color);
        public void Draw(ref Image<TPixel> canvas, RectangleF rect, Vector2 offset)
            => Draw(ref canvas, rect, offset, Color.White);
        public void Draw(ref Image<TPixel> canvas, RectangleF rect, Vector2 offset, Color color)
        {
            if (image == null)
            {
                if (!isEmpty)
                    Log.Warning("tried to draw null.");
                return;
            }

            var scaleX = (float)ActualSize.Width / Size.Width;
            var scaleY = (float)ActualSize.Height / Size.Height;
            var minX = rect.X + rect.Width * (1 - scaleX) / 2;
            var maxX = rect.X + rect.Width * (1 + scaleX) / 2;
            var minY = rect.Y + rect.Height * (1 - scaleY) / 2;
            var maxY = rect.Y + rect.Height * (1 + scaleY) / 2;

            var size = new Size((int)Math.Ceiling(maxX - minX), (int)Math.Ceiling(maxY - minY));
            var stamp = image.Clone(x => x
                .Resize(size)
            );
            stamp.Mutate(x => x
                .SetGraphicsOptions(o =>
                {
                    o.ColorBlendingMode = PixelColorBlendingMode.Multiply;
                    o.AlphaCompositionMode = PixelAlphaCompositionMode.SrcAtop;
                })
                .Fill(color)
                .SetGraphicsOptions(o =>
                {
                    o.ColorBlendingMode = PixelColorBlendingMode.Normal;
                    o.AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver;
                })
                .Opacity(color.ToPixel<Rgba32>().A / 255f)
            );

            canvas.Mutate(x => 
                x.DrawImage(stamp, new Point((int)Math.Floor(minX + offset.X), (int)Math.Floor(minY + offset.Y)), 1)
            );
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                    if (image != null)
                    {
                        image.Dispose();
                        image = null;
                        isEmpty = false;
                    }
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~ShapeImage()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public enum DrawingMode
    {
        Normal,
        Multiply,
    }
}
