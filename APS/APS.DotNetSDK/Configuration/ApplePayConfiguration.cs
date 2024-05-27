using System;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using APS.DotNetSDK.Signature;
using APS.DotNetSDK.Exceptions;
using System.Diagnostics;

namespace APS.DotNetSDK.Configuration
{
    public class ApplePayConfiguration
    {
        static private X509Certificate2 CreateSecurityCertificate(
        string appleCertificatePath,
        string appleCertificatePassword
    )
        {
            Debug.Assert((appleCertificatePath == null && appleCertificatePassword == null)
                || (appleCertificatePath != null && appleCertificatePassword != null));

            if (appleCertificatePath == null || appleCertificatePassword == null)
            {
                throw new ArgumentNullException();
            }

            var certificate = new X509Certificate2(appleCertificatePath, appleCertificatePassword);
            return certificate;

        }

        public ApplePayConfiguration(string AccessCode, string MerchantIdentifier,
            string DisplayName, string MerchantUid, string ResponseShaPhrase,
            string RequestShaPhrase, string ShaTypeAsString,
            string DomainName, string SecurityCertificatePath, string SecurityCertificatePassword)
        {
            this.AccessCode = AccessCode;
            this.MerchantIdentifier = MerchantIdentifier;
            this.DisplayName = DisplayName;
            this.MerchantUid = MerchantUid;
            this.ResponseShaPhrase = ResponseShaPhrase;
            this.RequestShaPhrase = RequestShaPhrase;
            this.ShaTypeAsString = ShaTypeAsString;
            this.DomainName = DomainName;
            if (SecurityCertificatePath != null) {
                this.SecurityCertificate = CreateSecurityCertificate(SecurityCertificatePath, SecurityCertificatePassword);
            }

            if (!Enum.TryParse(ShaTypeAsString, out ShaType resultApplePayShaType))
            {
                //details
                throw new SdkConfigurationException("Please provide one of the shaType \"Sha512\" or \"Sha256\". " +
                                                    "Is needed in Apple Pay Configuration. Please check file \"MerchantSdkConfiguration.json\"");
            }

            ShaType = resultApplePayShaType;
        }

        [JsonPropertyName("AccessCode")]
        public string AccessCode { get; set; }

        [JsonPropertyName("MerchantIdentifier")]
        public string MerchantIdentifier { get; set; }

        [JsonPropertyName("DisplayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("MerchantUid")]
        public string MerchantUid { get; set; }

        [JsonPropertyName("ResponseShaPhrase")]
        public string ResponseShaPhrase { get; set; }

        [JsonPropertyName("RequestShaPhrase")]
        public string RequestShaPhrase { get; set; }

        [JsonPropertyName("ShaType")]
        public string ShaTypeAsString { get; set; }

        [JsonPropertyName("DomainName")]
        public string DomainName { get; set; }

        [JsonIgnore]
        public ShaType ShaType { get; set; }

        [JsonPropertyName("SecurityCertificatePath")]
        public string SecurityCertificatePath { get; set; }

        [JsonPropertyName("SecurityCertificatePassword")]
        public string SecurityCertificatePassword { get; set; }

        public X509Certificate2 SecurityCertificate { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(AccessCode))
            {
                throw new ArgumentNullException($"AccessCode", "AccessCode is needed for ApplePay configuration");
            }

            if (string.IsNullOrEmpty(ResponseShaPhrase))
            {
                throw new ArgumentNullException($"ResponseShaPhrase",
                    "ResponseShaPhrase is needed for ApplePay configuration");
            }

            if (string.IsNullOrEmpty(RequestShaPhrase))
            {
                throw new ArgumentNullException($"RequestShaPhrase",
                    "RequestShaPhrase is needed for ApplePay configuration");
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                throw new ArgumentNullException($"DisplayName", "DisplayName is needed for ApplePay configuration");
            }

            if (string.IsNullOrEmpty(MerchantUid))
            {
                throw new ArgumentNullException($"MerchantUid",
                    "MerchantUid is needed for ApplePay configuration");
            }

            if (string.IsNullOrEmpty(DomainName))
            {
                throw new ArgumentNullException($"DomainName",
                    "DomainName is needed for ApplePay configuration");
            }
        }
    }
}