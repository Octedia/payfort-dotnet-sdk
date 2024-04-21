using System.Collections.Generic;

namespace APS.DotNetSDK.Configuration
{
    public interface IMerchantSdkConfiguraitonService {}

    public class SingleMerchantSdkConfiguraitonService : IMerchantSdkConfiguraitonService
    {
        public SingleMerchantSdkConfiguraitonService(SdkJsonConfiguration sdkJsonConfiguration) {
            this.sdkJsonConfiguration = sdkJsonConfiguration;
        }

        private readonly SdkJsonConfiguration sdkJsonConfiguration;

        public SdkJsonConfiguration GetSdkConfiguration() {
            return sdkJsonConfiguration;
        }
    }

    public class MultiMerchantSdkConfiguraitonService : IMerchantSdkConfiguraitonService
    {
        public MultiMerchantSdkConfiguraitonService(MultiSdkJsonConfiguration multiSdkJsonConfiguration) {
            this.multiSdkJsonConfiguration = multiSdkJsonConfiguration;
        }

        private readonly MultiSdkJsonConfiguration multiSdkJsonConfiguration;

        public SdkJsonConfiguration GetSdkConfiguration(string sdkConfigurationId) {
            return multiSdkJsonConfiguration.GetSdkConfiguration(sdkConfigurationId);
        }

        public List<SdkJsonConfiguration> GetAllSdkConfiguration() {
            return multiSdkJsonConfiguration.GetAllSdkConfiguration();
        }
    }
}