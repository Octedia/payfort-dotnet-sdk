using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using APS.DotNetSDK.Configuration;
using APS.DotNetSDK.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace APS.DotNetSDK.Extenions
{
    public static class IServiceCollectionExtensions
    {
        private static MerchantSdkConfiguration ReadMerchantSdkConfiguration(
            string merchantSdkConfigurationPath)
        {
            if (!File.Exists(merchantSdkConfigurationPath))
            {
                throw new FileNotFoundException();
            }

            var jsonContent = FileReader.ReadFromFile(merchantSdkConfigurationPath);
            var merchantSdkConfiguration = JsonSerializer.Deserialize<MerchantSdkConfiguration>(jsonContent);

            return merchantSdkConfiguration;
        }

        public static IServiceCollection AddMerchantSdkConfiguraiton(
            this IServiceCollection services,
            SdkJsonConfiguration sdkJsonConfiguration
        )
        {
            services.AddSingleton<IMerchantSdkConfiguraitonService>(
                new SingleMerchantSdkConfiguraitonService(sdkJsonConfiguration)
            );

            return services;
        }

        public static IServiceCollection AddMerchantSdkConfiguraiton(
            this IServiceCollection services,
            string sdkJsonConfigurationPath
        )
        {
            var merchantConfiguration =
                ReadMerchantSdkConfiguration(sdkJsonConfigurationPath);

            AddMerchantSdkConfiguraiton(services, merchantConfiguration.SdkConfiguration);

            return services;
        }

        public static IServiceCollection AddMerchantSdkConfiguraiton(
            this IServiceCollection services,
            Dictionary<string, SdkJsonConfiguration> sdkJsonConfigurationMap
        )
        {

            services.AddSingleton<IMerchantSdkConfiguraitonService>(
                new MultiMerchantSdkConfiguraitonService(new MultiSdkJsonConfiguration(sdkJsonConfigurationMap))
            );

            return services;
        }

        public static IServiceCollection AddMerchantSdkConfiguraiton(
            this IServiceCollection services,
            Dictionary<string, string> sdkJsonConfigurationPathMap
        )
        {
            var sdkJsonConfigurationMap = new Dictionary<string, SdkJsonConfiguration>();
            foreach (var entry in sdkJsonConfigurationPathMap)
            {
                var merchantConfiguration =
                    ReadMerchantSdkConfiguration(entry.Value);
                sdkJsonConfigurationMap[entry.Key] = merchantConfiguration.SdkConfiguration;
            }

            services.AddSingleton<IMerchantSdkConfiguraitonService>(
                new MultiMerchantSdkConfiguraitonService(new MultiSdkJsonConfiguration(sdkJsonConfigurationMap))
            );

            return services;
        }
    }
}