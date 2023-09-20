using Lycoris.Base.Extensions;
using Microsoft.Extensions.Configuration;

namespace Lycoris.Base.ConfigurationManager
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
            this.JsonFilePath = jsonFilePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configure"></param>
        public void AddJsonConfiguration(Func<string> configure)
        {
            this.JsonFilePath = configure();
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
            this.JsonFilePath = configure(Environment.GetEnvironmentVariable(variable) ?? "");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal IConfiguration BuildIConfiguration()
        {
            if (this.JsonFilePath.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(this.JsonFilePath));

            return JsonConfigurationExtensions.AddJsonFile(FileConfigurationExtensions.SetBasePath(new ConfigurationBuilder(), Directory.GetCurrentDirectory()), this.JsonFilePath, true).Build();
        }
    }
}
