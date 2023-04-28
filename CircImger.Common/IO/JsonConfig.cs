using Serilog;
using System.Text;
using System.Text.Json;

namespace CircImger.Common.IO
{
    public class JsonConfig<TConfig> : Config<TConfig>
        where TConfig : class, new()
    {
        public JsonConfig(string filepath) : base(filepath)
        {
            react();
        }

        protected override bool deserialize(out TConfig? result)
        {
            try
            {
                using (var fs = File.Open(fullPath, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs))
                {
                    var config = JsonSerializer.Deserialize<TConfig>(sr.ReadToEnd());
                    if (config != null)
                    {
                        result = config;
                        return true;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }
            }
            catch (JsonException e)
            {
                throw new FormatException(e.Message);
            }
        }

        protected override bool serialize(out Stream result)
        {
            var json = JsonSerializer.Serialize(Value, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
            var bytes = Encoding.UTF8.GetBytes(json);
            result = new MemoryStream(bytes);
            return true;
        }
    }
}
