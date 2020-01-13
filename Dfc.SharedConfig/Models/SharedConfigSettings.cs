namespace Dfc.SharedConfig.Models
{
    public class SharedConfigSettings
    {
        public string ConfigurationStorageConnectionString { get; set; }

        public string InMemoryCacheTimeToLiveTimeSpan { get; set; } = "00:01:00";

        public string CloudStorageTableName { get; set; } = "Configuration";

        public string EnvironmentName { get; set; } = "Production";
    }
}