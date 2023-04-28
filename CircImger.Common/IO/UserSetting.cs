using System.Text.Json;
using System.Text.Json.Serialization;

namespace CircImger.Common.IO
{
    public class UserSetting
    {
        [JsonIgnore]
        public static string Filename = "setting.json";

        [JsonPropertyName("directories")]
        public DirectorySetting Directories { get; set; } = new DirectorySetting();
        public class DirectorySetting
        {
            [JsonPropertyName("texture")]
            public string Texture { get; set; } = "..\\Textures";

            [JsonPropertyName("result")]
            public string Result { get; set; } = "..\\Results";
        }

        [JsonPropertyName("result_filename")]
        public string ResultFileName { get; set; } = "result.png";

        [JsonPropertyName("texture_names")]
        public TextureSetting Textures { get; set; } = new TextureSetting();
        public class TextureSetting
        {
            [JsonPropertyName("shape_base")]
            public TextureDetailSetting Shape { get; set; } = new TextureDetailSetting("shape.png", Vector2.One);

            [JsonPropertyName("shape_overlay")]
            public TextureDetailSetting ShapeOverlay { get; set; } = new TextureDetailSetting("shapeoverlay.png", Vector2.One);

            [JsonPropertyName("numbers")]
            public TextureDetailSetting Numbers { get; set; } = new TextureDetailSetting("number{0}.png", Vector2.One);

            public class TextureDetailSetting
            {
                [JsonPropertyName("filename")]
                public string Filename { get; set; } = string.Empty;

                [JsonPropertyName("area_racio")]
                public Vector2 AreaRacio { get; set; } = Vector2.One;

                public TextureDetailSetting(string filename, Vector2 areaRacio)
                {
                    this.Filename = filename;
                    this.AreaRacio = areaRacio;
                }   
            }
        }

        [JsonPropertyName("shape_size")]
        public float ShapeSize { get; set; } = 1;

        [JsonPropertyName("color_opacity")]
        public float ColorOpacity { get; set; } = 0.5f;

        [JsonPropertyName("margin")]
        public int Margin { get; set; } = 16;

        public class Vector2
        {
            [JsonIgnore]
            public static Vector2 Zero = new Vector2(0, 0);

            [JsonIgnore]
            public static Vector2 One = new Vector2(1, 1);

            public Vector2(float x, float y)
            {
                this.X = x;
                this.Y = y;
            }

            [JsonPropertyName("x")]
            public float X { get; set; } = 0;
            [JsonPropertyName("y")]
            public float Y { get; set; } = 0;
        }
    }
}
