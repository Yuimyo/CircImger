using CircImger.Common.Drawings;
using CircImger.Common.IO;
using CircImger.Common.Utils;
using System.Numerics;
using static CircImger.Common.IO.UserSetting.TextureSetting;

namespace CircImger.Common
{
    public class ShapeRenderer : IDisposable
    {
        private const int OBJECT_RADIUS = 64;

        private Texture2d<Rgba32> circleTexture = Texture2d<Rgba32>.Empty;
        private Texture2d<Rgba32> overlayTexture = Texture2d<Rgba32>.Empty;
        private Texture2d<Rgba32>[] numTextures = new Texture2d<Rgba32>[10];
        private Shape[] numShapes = new Shape[10];

        private UserSetting setting;
        public ShapeRenderer(UserSetting setting)
        {
            this.setting = setting;
            initializeTextures();
        }

        private void initializeTextures()
        {
            var textureDirectory = setting.Directories.Texture;
            if (!Path.IsPathRooted(textureDirectory))
                textureDirectory = Path.GetFullPath(textureDirectory);

            circleTexture = loadTexture(textureDirectory, setting.Textures.Shape);
            overlayTexture = loadTexture(textureDirectory, setting.Textures.ShapeOverlay);
            for (int i = 0; i < 10; i++)
                numTextures[i] = loadTexture(textureDirectory, setting.Textures.Numbers, i);
            for (int i = 0; i < 10; i++)
                numShapes[i] = new Square(Vector2.Zero, Origin.Center, numTextures[i].ActualSize * sizeScale);
        }

        private Texture2d<Rgba32> loadTexture(string textureDirectory, TextureDetailSetting textureDetail, int? index = null)
        {
            var filename = textureDetail.Filename;
            if (index != null)
                filename = string.Format(filename, index);
            var texturePath = Path.Combine(textureDirectory, filename);

            Texture2d<Rgba32> texture;
            if (File.Exists(texturePath))
                texture = new Texture2d<Rgba32>(texturePath);
            else
                texture = Texture2d<Rgba32>.Empty;

            texture.Size = new SizeF(
                texture.ActualSize.Width * textureDetail.AreaRacio.X,
                texture.ActualSize.Height * textureDetail.AreaRacio.Y
            );

            return texture;
        }

        private readonly float posScale = 1.5f;
        private float sizeScale => ((1 - 0.7f * (setting.ShapeSize - 5) / 5) / 2) / 0.75f;
        private float objectRadius => OBJECT_RADIUS * sizeScale;

        public Image<Rgba32> Render(IEnumerable<ObjectDescription> hitDescriptions)
        {
            var descriptions = new List<ShapeDescription>();
            foreach (var hitDescription in hitDescriptions)
            {
                var color = hitDescription.Color.ToPixel<Rgba32>();
                color = Color.FromRgba(color.R, color.G, color.B, (byte)Math.Round(color.A * setting.ColorOpacity));

                descriptions.Add(new ShapeDescription(
                    new Circle(hitDescription.Position * posScale, objectRadius),
                    hitDescription.Number,
                    color
                ));
            }

            var canvas = prepareCanvas(descriptions.Select(x => x.Shape), out Vector2 canvasOffset);

            descriptions.Reverse();
            foreach (var description in descriptions)
            {
                drawCircle(ref canvas, canvasOffset, description);
            }

            return canvas;
        }

        private void drawCircle(ref Image<Rgba32> canvas, Vector2 canvasOffset, ShapeDescription description)
        {
            circleTexture.Draw(ref canvas, description.Shape.Area, canvasOffset, description.Color);

            var nums = new List<int>();
            foreach (var c in description.Number.ToString())
                nums.Add(Convert.ToInt32($"{c}"));
            float numTotalWidth = 0;
            foreach (var i in nums)
                numTotalWidth += numTextures[i].Size.Width;

            var numOffset = description.Shape.FromOrigin(Origin.Center) - new Vector2(numTotalWidth / 2f * sizeScale, 0);
            foreach (var i in nums)
            {
                var width = numTextures[i].Size.Width * sizeScale;
                numTextures[i].Draw(ref canvas, numShapes[i].Area, canvasOffset + numOffset + new Vector2(width / 2, 0));
                numOffset.X += width;
            }

            overlayTexture.Draw(ref canvas, description.Shape.Area, canvasOffset);
        }

        private Image<Rgba32> prepareCanvas(IEnumerable<Shape> shapes, out Vector2 offset)
        {
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            foreach (var shape in shapes)
            {
                MathHelper.Chmin(ref minX, shape.Area.X);
                MathHelper.Chmax(ref maxX, shape.Area.X + shape.Area.Width);
                MathHelper.Chmin(ref minY, shape.Area.Y);
                MathHelper.Chmax(ref maxY, shape.Area.Y + shape.Area.Height);
            }

            offset = new(setting.Margin - minX, setting.Margin - minY);
            Size resSize = new((int)Math.Round((maxX - minX) + setting.Margin * 2), (int)Math.Round((maxY - minY) + setting.Margin * 2));
            return new Image<Rgba32>(Configuration.Default, resSize.Width, resSize.Height);
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    circleTexture.Dispose();
                    overlayTexture.Dispose();
                    foreach(var numTexture in numTextures)
                        numTexture.Dispose();
                }
                overlayTexture.Dispose();

                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~ShapeRenderer()
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

        public class ShapeDescription
        {
            public Shape Shape;
            public uint Number;
            public Color Color;
            public ShapeDescription(Shape shape, uint number, Color color)
            {
                this.Shape = shape;
                this.Number = number;
                this.Color = color;
            }
        }
    }
}
