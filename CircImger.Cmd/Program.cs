using CircImger.Common;
using CircImger.Common.IO;
using Serilog;
using SixLabors.ImageSharp;
using System.Reflection;

namespace CircImger.Cmd
{
    internal class Program
    {
        public static string RootDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static JsonConfig<UserSetting>? userSetting = null;
        private static CsvConfig<ObjectDescription>? objectSetting = null;
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            using (userSetting = new JsonConfig<UserSetting>(Path.Combine(RootDirectory, UserSetting.Filename)))
            using (objectSetting = new CsvConfig<ObjectDescription>(Path.Combine(RootDirectory, ObjectDescription.Filename), ObjectDescription.TryParse, ObjectDescription.FormatDescription))
            {
                userSetting.Reloaded += UserSetting_Reloaded;
                objectSetting.Reloaded += ObjectSetting_Reloaded;

                Console.ReadLine();

                objectSetting.Reloaded -= ObjectSetting_Reloaded;
                userSetting.Reloaded -= UserSetting_Reloaded;
            }
        }

        private static void UserSetting_Reloaded(object sender, ConfigReloadingEventArgs<UserSetting> e)
        {
            runRenderer();
        }

        private static void ObjectSetting_Reloaded(object sender, ConfigReloadingEventArgs<List<ObjectDescription>> e)
        {
            runRenderer();
        }

        private static void runRenderer()
        {
            if (userSetting == null || objectSetting == null)
            {
                Log.Warning("config(s) are not available.");
                return;
            }
            try
            {
                var renderer = new ShapeRenderer(userSetting.Value);
                using (var result = renderer.Render(objectSetting.Value))
                {
                    var resultDirectory = userSetting.Value.Directories.Result;
                    var resultFilename = userSetting.Value.ResultFileName;
                    if (resultFilename == string.Empty)
                        throw new InvalidDataException();
                    if (!Path.IsPathRooted(resultDirectory))
                        resultDirectory = Path.GetFullPath(resultDirectory);
                    if (!Directory.Exists(resultDirectory))
                        Directory.CreateDirectory(resultDirectory);

                    result.Save(Path.Combine(resultDirectory, resultFilename));
                    Log.Information("Rendered!");
                }
            }
            catch (InvalidDataException)
            {
                Log.Warning("invalid data.");
            }
        }
    }
}