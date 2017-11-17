using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Roc.Logging.ES
{
    public class EsLoggerSettings
    {
        private volatile bool disabled;
        private volatile bool enableProfiler;
        private IDisposable reloadCallbackToken;

        public EsLoggerSettings()
        {
        }

        public EsLoggerSettings(IConfiguration configuration)
        {
            this.InitializeFromConfiguration(configuration);
            this.WatchConfigurationChanged(configuration);
        }

        private void InitializeFromConfiguration(IConfiguration configuration)
        {
            this.AppId = configuration["AppId"];
            this.EsUrl = configuration["EsUrl"];
            this.IndexName = configuration["IndexName"];
            this.enableProfiler = bool.TryParse(configuration["EnableProfiler"], out bool tmpEnableProfiler) && tmpEnableProfiler;
            this.disabled = bool.TryParse(configuration["Disabled"], out bool tmpDisabled) && tmpDisabled;
        }

        private void WatchConfigurationChanged(IConfiguration configuration)
        {
            this.reloadCallbackToken = configuration.GetReloadToken().RegisterChangeCallback(onConfigChanged, configuration);
        }

        private void onConfigChanged(object obj)
        {
            this.InitializeFromConfiguration(obj as IConfiguration);
            this.WatchConfigurationChanged(obj as IConfiguration);
        }

        public string EsUrl { get; set; }
        public string IndexName { get; set; }
        public string AppId { get; set; }
        public bool EnableProfiler {get{return this.enableProfiler;} set{this.enableProfiler=value;}}
        public bool Disabled {get{return this.disabled;} set{this.disabled=value;}}
    }
}