using System;
using APS.DotNetSDK.Exceptions;
using APS.DotNetSDK.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace APS.DotNetSDK.Configuration
{
    public static class SdkConfiguration
    {
        private static readonly object LockObject = new object();

        public static bool IsTestEnvironment { get; private set; }

        public static string AccessCode
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public static string MerchantIdentifier
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public static string RequestShaPhrase
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public static bool IsConfigured
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        internal static ServiceProvider ServiceProvider { get; set; }

        public static ServiceProvider Configure(
            string filePath,
            LoggingConfiguration loggingConfiguration,
            ApplePayConfiguration applePayConfiguration = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Configure the SDK
        /// </summary>
        /// 
        /// <param name="loggingConfiguration">The logging configuration</param>
        /// <exception cref="ArgumentNullException">Get the exception when one of the mandatory parameters are null or empty</exception>
        public static ServiceProvider Configure(
            LoggingConfiguration loggingConfiguration,
            Environment isTestEnvironment)
        {
            lock (LockObject)
            {
                IsTestEnvironment = isTestEnvironment == Environment.Test;

                loggingConfiguration.Validate();

                loggingConfiguration.ServiceCollection
                    .AddSerilogLogger(loggingConfiguration.JsonLoggingPathConfig, loggingConfiguration.ApplicationName);

                ServiceProvider = loggingConfiguration.ServiceCollection.BuildServiceProvider();

                return ServiceProvider;
            }
        }

        internal static SdkJsonConfiguration GetSdkConfiguration(string sdkConfigurationId = null)
        {
            var service = ServiceProvider.GetService<IMerchantSdkConfiguraitonService>();

            if (service is SingleMerchantSdkConfiguraitonService singleMerchantSdkConfiguraitonService)
            {
                return singleMerchantSdkConfiguraitonService.GetSdkConfiguration();
            }

            if (service is MultiMerchantSdkConfiguraitonService multiMerchantSdkConfiguraitonService)
            {
                return multiMerchantSdkConfiguraitonService.GetSdkConfiguration(sdkConfigurationId);
            }

            if (service == null)
            {
                throw new SdkConfigurationException();
            }

            throw new NotSupportedException($"{service.GetType()} is not supported");
        }

        internal static void Validate()
        {
            // TODO: Create validation strategy for all configuraitons
        }

        internal static void ValidateApplePayConfiguration()
        {
            // TODO: Create validation strategy for apple pay
        }
    }
}
