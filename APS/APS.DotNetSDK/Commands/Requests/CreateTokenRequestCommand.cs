using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using APS.DotNetSDK.Configuration;
using System.Text.Json.Serialization;

namespace APS.DotNetSDK.Commands.Requests
{
    public class CreateTokenRequestCommand : RequestCommand
    {
        public CreateTokenRequestCommand(string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            AccessCode = sdkConfiguration.AccessCode;
            MerchantIdentifier = sdkConfiguration.MerchantIdentifier;
        }

        private CreateTokenRequestCommand() { }

        [JsonPropertyName("service_command")]
        public string Command => "CREATE_TOKEN";

        [JsonPropertyName("token_name")]
        public string TokenName { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }
        /// <summary>
        /// This is only for custom checkout (mandatory parameter). E.g. 2105 meaning 21 = day and 05 = month
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; set; }
        /// <summary>
        /// This is only for custom checkout (mandatory parameter)
        /// </summary>
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }
        /// <summary>
        /// This is only for custom checkout (mandatory parameter)
        /// </summary>
        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }
        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("merchant_extra")]
        public string MerchantExtra { get; set; }

        [JsonPropertyName("merchant_extra1")]
        public string MerchantExtra1 { get; set; }

        [JsonPropertyName("merchant_extra2")]
        public string MerchantExtra2 { get; set; }

        [JsonPropertyName("merchant_extra3")]
        public string MerchantExtra3 { get; set; }

        [JsonPropertyName("merchant_extra4")]
        public string MerchantExtra4 { get; set; }

        public override void ValidateMandatoryProperties()
        {
            base.ValidateMandatoryProperties();

            if (string.IsNullOrEmpty(MerchantReference))
            {
                throw new ArgumentNullException($"MerchantReference", "MerchantReference is mandatory");
            }
        }

        internal override string ToAnonymizedJson()
        {
            var anonymized = new CreateTokenRequestCommand()
            {
                AccessCode = this.AccessCode,
                MerchantIdentifier = this.MerchantIdentifier,
                Language = this.Language,
                Signature = this.Signature,

                MerchantReference = this.MerchantReference,
                TokenName = this.TokenName,
                ReturnUrl = this.ReturnUrl,
                ExpiryDate = string.IsNullOrEmpty(ExpiryDate) ? this.ExpiryDate : "***",
                CardNumber = string.IsNullOrEmpty(CardNumber) ? this.CardNumber : "***",
                CardHolderName = string.IsNullOrEmpty(CardHolderName) ? this.CardHolderName : "***",
                Currency = this.Currency,
                MerchantExtra = this.MerchantExtra,
                MerchantExtra1 = this.MerchantExtra1,
                MerchantExtra2 = this.MerchantExtra2,
                MerchantExtra3 = this.MerchantExtra3,
                MerchantExtra4 = this.MerchantExtra4,
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