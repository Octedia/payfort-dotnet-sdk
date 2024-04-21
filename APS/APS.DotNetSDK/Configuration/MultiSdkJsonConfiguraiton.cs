using System;
using System.Collections.Generic;
using System.Linq;

namespace APS.DotNetSDK.Configuration {
    public class MultiSdkJsonConfiguration {
        public MultiSdkJsonConfiguration(Dictionary<string, SdkJsonConfiguration> configurations) {
            _configurations = configurations;
        }

        private readonly Dictionary<string, SdkJsonConfiguration> _configurations;

        public SdkJsonConfiguration GetSdkConfiguration(string sdkConfigurationId) {
            if (sdkConfigurationId == null) {
                throw new ArgumentNullException();
            }
            if (!_configurations.ContainsKey(sdkConfigurationId)) {
                throw new ArgumentException();
            }

            return _configurations[sdkConfigurationId];
        }

        public List<SdkJsonConfiguration> GetAllSdkConfiguration() {
            return _configurations.Values.ToList();
        }
    }
}