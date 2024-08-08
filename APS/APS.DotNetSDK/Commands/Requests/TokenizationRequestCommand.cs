using System;
using System.Text.Json;
using System.Text.Encodings.Web;
using APS.DotNetSDK.Configuration;
using System.Text.Json.Serialization;
using APS.DotNetSDK.Utils;

namespace APS.DotNetSDK.Commands.Requests
{
    public class TokenizationRequestCommand : RequestCommand
    {
        private string _installments;
        private string _rememberMe;
        public TokenizationRequestCommand(string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            AccessCode = sdkConfiguration.AccessCode;
            MerchantIdentifier = sdkConfiguration.MerchantIdentifier;
        }

        private TokenizationRequestCommand() {}

        [JsonPropertyName("service_command")]
        public string Command => "TOKENIZATION";

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
        [JsonPropertyName("card_security_code")]
        public string SecurityCode { get; set; }
        /// <summary>
        /// This is only for custom checkout (mandatory parameter)
        /// </summary>
        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }
        /// <summary>
        /// This is only for custom checkout (optional parameter). Should be "YES" or "NO"
        /// </summary>
        [JsonPropertyName("remember_me")]
        [IgnoreOnSignatureCalculationAttribute(true)]
        public string RememberMe
        {
            get => _rememberMe?.ToUpper();
            set => _rememberMe = value;
        }

        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("installments")]
        public string Installments
        {
            get => _installments?.ToUpper();
            set => _installments = value;
        }
        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("customer_country_code")]
        public string CustomerCountryCode { get; set; }
        
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

            if (!string.IsNullOrEmpty(RememberMe))
            {
                if (!(RememberMe == "YES" || RememberMe == "NO"))
                {
                    throw new ArgumentException("RememberMe should be \"YES\" or \"NO\"", $"RememberMe");
                }
            }

            if (!string.IsNullOrEmpty(Installments))
            {
                if (!(Installments == "STANDALONE"))
                {
                    throw new ArgumentException("Installments should be \"STANDALONE\"", $"Installments");
                }
                ValidateMandatoryPropertiesInstallments();
            }
        }

        public void ValidateMandatoryPropertiesInstallments()
        {
            if (string.IsNullOrWhiteSpace(Amount))
            {
                throw new ArgumentNullException($"Amount", "Amount is mandatory for installments");
            }

            if (string.IsNullOrEmpty(Currency))
            {
                throw new ArgumentNullException($"Currency", "Currency is mandatory for installments");
            }
        }

        internal override string ToAnonymizedJson()
        {
            var anonymized = new TokenizationRequestCommand()
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
                SecurityCode = string.IsNullOrEmpty(SecurityCode) ? this.SecurityCode : "***",
                CardHolderName = string.IsNullOrEmpty(CardHolderName) ? this.CardHolderName : "***",
                RememberMe = this.RememberMe,
                Installments = this.Installments,
                Amount = this.Amount,
                Currency = this.Currency,
                CustomerCountryCode = this.CustomerCountryCode,
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