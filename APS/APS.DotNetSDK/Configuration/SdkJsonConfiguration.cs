using System;
using System.Text.Json.Serialization;
using APS.DotNetSDK.Exceptions;
using APS.DotNetSDK.Signature;

namespace APS.DotNetSDK.Configuration
{
    public class SdkJsonConfiguration
    {
        public SdkJsonConfiguration(string AccessCode, string MerchantIdentifier,
        string ResponseShaPhrase, string RequestShaPhrase, string ShaTypeAsString,
        ApplePayConfiguration ApplePay)
        {
            this.AccessCode = AccessCode;
            this.MerchantIdentifier = MerchantIdentifier;
            this.ResponseShaPhrase = ResponseShaPhrase;
            this.RequestShaPhrase = RequestShaPhrase;
            this.ApplePay = ApplePay;

            if (!Enum.TryParse(ShaTypeAsString, out ShaType resultShaType))
            {
                //details
                throw new SdkConfigurationException("Please provide one of the shaType \"Sha512\" or \"Sha256\". " +
                    "Is needed in Sdk Configuration. Please check file \"MerchantSdkConfiguration.json\"");
            }
            ShaType = resultShaType;
        }

        [JsonPropertyName("AccessCode")]
        public string AccessCode { get; set; }

        [JsonPropertyName("MerchantIdentifier")]
        public string MerchantIdentifier { get; set; }

        [JsonPropertyName("ResponseShaPhrase")]
        public string ResponseShaPhrase { get; set; }

        [JsonPropertyName("RequestShaPhrase")]
        public string RequestShaPhrase { get; set; }

        [JsonPropertyName("ShaType")]
        public string ShaTypeAsString { get; set; }

        [JsonIgnore]
        public ShaType ShaType { get; set; }

        [JsonPropertyName("ApplePay")]
        public ApplePayConfiguration ApplePay { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(AccessCode))
            {
                throw new ArgumentNullException("AccessCode", "AccessCode is needed for SDK configuration");
            }

            if (string.IsNullOrEmpty(MerchantIdentifier))
            {
                throw new ArgumentNullException("MerchantIdentifier", "MerchantIdentifier is needed for SDK configuration");
            }

            if (string.IsNullOrEmpty(RequestShaPhrase))
            {
                throw new ArgumentNullException("RequestShaPhrase", "RequestShaPhrase is needed for SDK configuration");
            }

            if (string.IsNullOrEmpty(ResponseShaPhrase))
            {
                throw new ArgumentNullException("ResponseShaPhrase", "ResponseShaPhrase is needed for SDK configuration");
            }

            ApplePay?.Validate();
        }
    }
}

