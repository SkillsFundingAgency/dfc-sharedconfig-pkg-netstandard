using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Dfc.SharedConfig.Repositories
{
    [ExcludeFromCodeCoverage]
    internal class FileStorageConfigurationRepository : IConfigurationRepository
    {
        public async Task<string> GetCloudConfigAsync(string serviceName, string keyName)
        {
            var path = GetConfigFilePath(serviceName, keyName);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync().ConfigureAwait(false);
                reader.Close();

                return json;
            }
        }

        public async Task<string> SetCloudConfigAsync(string serviceName, string keyName, string data)
        {
            var path = GetConfigFilePath(serviceName, keyName);

            using (var outputFile = new StreamWriter(path))
            {
                await outputFile.WriteLineAsync(data).ConfigureAwait(false);
            }

            return data;
        }

        private static string GetConfigFilePath(string serviceName, string keyName)
        {
            var appDataFolder = !string.IsNullOrEmpty((string)AppDomain.CurrentDomain.GetData("DataDirectory"))
                ? (string)AppDomain.CurrentDomain.GetData("DataDirectory")
                : Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "App_Data");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            return Path.Combine(appDataFolder, $"{serviceName}_{keyName}.json");
        }
    }
}