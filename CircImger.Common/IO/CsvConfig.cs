using Serilog;
using System.Text;

namespace CircImger.Common.IO
{
    public class CsvConfig<TConfig> : Config<List<TConfig>>
        where TConfig : class, IFormattableToCsv, new()
    {
        private Func<string, TConfig?> tryParse;
        private string formatDescription = string.Empty;

        public CsvConfig(string filename, Func<string, TConfig?> tryParse, string formatDescription) : base(filename)
        {
            this.tryParse = tryParse;
            this.formatDescription = formatDescription;

            react();
        }

        protected override bool deserialize(out List<TConfig>? result)
        {
            var config = new List<TConfig>();
            var sb = new StringBuilder();
            sb.AppendLine();
            using (var fs = File.Open(fullPath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == string.Empty || line.StartsWith("//")) continue; // comment

                    var data = tryParse(line);
                    if (data == null) 
                        throw new FormatException("somehow failed to deserialize.");
                    config.Add(data);
                    sb.AppendLine(line);
                }
            }

            Log.Debug(sb.ToString());
            result = config;
            return true;
        }

        protected override bool serialize(out Stream result)
        {
            var sb = new StringBuilder();
            if (formatDescription != string.Empty) 
                sb.AppendLine($"// {formatDescription}");
            foreach(var data in this.Value)
            {
                if (data == null) continue;
                sb.AppendLine(data.ToCsv());
            }
            var csv = sb.ToString();
            Log.Debug(csv);
            var bytes = Encoding.UTF8.GetBytes(csv);
            result = new MemoryStream(bytes);
            return true;
        }
    }
}
