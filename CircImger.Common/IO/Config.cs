using Serilog;

namespace CircImger.Common.IO
{
    public abstract class Config<TConfig> : IDisposable
        where TConfig : class, new()
    {
        private FileSystemWatcher watcher;

        private string directoryName = string.Empty;
        private string fileName = string.Empty;
        protected string fullPath => Path.Combine(directoryName, fileName);

        private TConfig value = new();
        public TConfig Value
        {
            get => value;
            set
            {
                this.value = value;
            }
        }

        public delegate void ConfigReloadingEventHandler(object sender, ConfigReloadingEventArgs<TConfig> e);
        public event ConfigReloadingEventHandler? Reloaded;

        public Config(string filepath)
        {
            this.fileName = Path.GetFileName(filepath);
            var dir = Path.GetDirectoryName(filepath);
            if (dir == null)
                throw new ArgumentException();
            this.directoryName = dir!;

            watcher = new FileSystemWatcher(directoryName);

            watcher.Created += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;
            watcher.Changed += Watcher_Changed;
            watcher.IncludeSubdirectories = false;

            watcher.EnableRaisingEvents = true;
        }

        protected void react()
        {
            if (!File.Exists(fullPath))
            {
                Value = new();
                trySave();
                Log.Information("saved.");
            }
            else
            {
                tryLoad();
                Log.Information("loaded.");
            }

            Reloaded?.Invoke(this, new ConfigReloadingEventArgs<TConfig>(this.Value));
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (fullPath == e.FullPath)
                react();
        }

        protected abstract bool serialize(out Stream result);
        protected abstract bool deserialize(out TConfig? result);

        private bool tryLoad()
        {
            if (!File.Exists(fullPath))
                return false;

            for(int i = 0; i < 30; i++)
            {
                try
                {
                    if (!deserialize(out TConfig? result))
                        return false;
                    value = result!;

                    return true;
                }            
                catch (FormatException e)
                {
                    Log.Warning($"invalid data.");
                    Log.Warning(e.Message);
                    return false;
                }
                catch (IOException)
                {
                    Log.Warning($"failed to load #{i}.");
                    Task.Delay(100);
                }
            }
            return false;
        }

        private bool trySave()
        {
            using (var outStream = File.OpenWrite(fullPath))
            {
                serialize(out Stream result);
                result.CopyTo(outStream);
            }
            return true;
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
                    watcher.EnableRaisingEvents = false;
                    watcher.Created -= Watcher_Changed;
                    watcher.Renamed -= Watcher_Changed;
                    watcher.Changed -= Watcher_Changed;
                    watcher.Dispose();
                    directoryName = string.Empty;
                    fileName = string.Empty;
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~FileWatcher()
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

    public class ConfigReloadingEventArgs<TConfig>
        where TConfig : class, new()
    {
        public TConfig Config;
        public ConfigReloadingEventArgs(TConfig config)
        {
            this.Config = config;
        }
    }
}
