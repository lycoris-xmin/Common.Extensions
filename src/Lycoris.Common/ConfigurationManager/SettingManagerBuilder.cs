using Lycoris.Common.Extensions;
using Microsoft.Extensions.Configuration;

namespace Lycoris.Common.ConfigurationManager
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SettingManagerBuilder
    {
        internal string JsonFilePath = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonFilePath"></param>
        public void AddJsonConfiguration(string jsonFilePath)
        {
            JsonFilePath = jsonFilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        public void AddJsonConfiguration(Func<string> configure)
        {
            JsonFilePath = configure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public SettingManagerBuilder AddJsonConfigurationWithEnvironment(Func<string, string> configure) => AddJsonConfigurationWithEnvironment("ASPNETCORE_ENVIRONMENT", configure);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public SettingManagerBuilder AddJsonConfigurationWithEnvironment(string variable, Func<string, string> configure)
        {
            JsonFilePath = configure(Environment.GetEnvironmentVariable(variable) ?? "");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal IConfiguration BuildIConfiguration()
        {
            if (JsonFilePath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(JsonFilePath));

            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(JsonFilePath, true).Build();
        }
    }
}
