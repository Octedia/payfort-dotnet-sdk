﻿using System.Text.Json;
using System.Text.Encodings.Web;
using APS.DotNetSDK.Configuration;
using System.Text.Json.Serialization;

namespace APS.DotNetSDK.Commands.Requests
{
    public class GetInstallmentsRequestCommand : Command
    {
        private string _language;

        public GetInstallmentsRequestCommand(string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            AccessCode = sdkConfiguration.AccessCode;
            MerchantIdentifier = sdkConfiguration.MerchantIdentifier;
        }

        private GetInstallmentsRequestCommand() {}

        [JsonPropertyName("access_code")]
        public string AccessCode { get; protected set; }

        [JsonPropertyName("merchant_identifier")]
        public string MerchantIdentifier { get; protected set; }

        [JsonPropertyName("language")]
        public string Language
        {
            get => _language?.ToLower();
            set => _language = value;
        }

        [JsonPropertyName("query_command")]
        public string Command => "GET_INSTALLMENTS_PLANS";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("issuer_code")]
        public string IssuerCode { get; set; }

        internal string ToAnonymizedJson()
        {
            var anonymized = new GetInstallmentsRequestCommand()
            {
                AccessCode = this.AccessCode,
                MerchantIdentifier = this.MerchantIdentifier,
                Language = this.Language,
                Signature = this.Signature,

                Amount = this.Amount,
                Currency = this.Currency,
                IssuerCode = this.IssuerCode
            };

            var serialized = JsonSerializer.Serialize(anonymized,
                   new JsonSerializerOptions
                   {
                       DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                       Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                   });

            return serialized;
        }
    }
}