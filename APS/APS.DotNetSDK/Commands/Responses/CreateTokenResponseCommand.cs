using System.Web;
using System.Text.Json;
using APS.DotNetSDK.Utils;
using APS.DotNetSDK.Exceptions;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace APS.DotNetSDK.Commands.Responses
{
    public class CreateTokenResponseCommand : ResponseCommandWithNotification
    {
        private const string CommandKey = "service_command";

        [JsonPropertyName("service_command")]
        public override string Command => "Create_Token";

        [JsonPropertyName("expiry_date")]
        public string ExpiryDate { get; set; }

        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; }

        [JsonPropertyName("token_name")]
        public string TokenName { get; set; }

        /// <summary>
        /// This is for custom checkout
        /// </summary>
        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }

        [JsonPropertyName("return_url")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// This is only for installments
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency { get; set; }


        public override void BuildNotificationCommand(IDictionary<string, string> dictionaryObject)
        {
            if (!dictionaryObject.Keys.Contains(CommandKey))
            {
                throw new InvalidNotification(
                    "Response does not contain any command. Please contact the SDK provider.");
            }

            var responseCommand = dictionaryObject.CreateObject<CreateTokenResponseCommand>();

            if (responseCommand == null)
            {
                throw new InvalidNotification(
                    $"There was an issue when mapping notification request: [" +
                    $"{dictionaryObject.JoinElements(",")}] to AuthorizeResponseCommand");
            }

            if (dictionaryObject[CommandKey] != Command)
            {
                throw new InvalidNotification(
                    $"Invalid Command received from payment gateway:{dictionaryObject["service_command"]}");
            }

            Signature = responseCommand.Signature;

            AccessCode = responseCommand.AccessCode;
            MerchantIdentifier = responseCommand.MerchantIdentifier;
            Language = responseCommand.Language;
            MerchantReference = responseCommand.MerchantReference;
            ResponseMessage = responseCommand.ResponseMessage;
            ResponseCode = responseCommand.ResponseCode;
            Status = responseCommand.Status;
            ReconciliationReference = responseCommand.ReconciliationReference;
            ProcessorResponseCode = responseCommand.ProcessorResponseCode;

            ExpiryDate = responseCommand.ExpiryDate;
            CardNumber = responseCommand.CardNumber;
            CardHolderName = responseCommand.CardHolderName;
            TokenName = responseCommand.TokenName;
            ReturnUrl = HttpUtility.UrlDecode(responseCommand.ReturnUrl);

            Currency = responseCommand.Currency;
            MerchantExtra = responseCommand.MerchantExtra;
            MerchantExtra1 = responseCommand.MerchantExtra1;
            MerchantExtra2 = responseCommand.MerchantExtra2;
            MerchantExtra3 = responseCommand.MerchantExtra3;
            MerchantExtra4 = responseCommand.MerchantExtra4;
        }

        internal override string ToAnonymizedJson()
        {
            var anonymized = new CreateTokenResponseCommand()
            {
                AccessCode = this.AccessCode,
                MerchantIdentifier = this.MerchantIdentifier,
                Language = this.Language,
                Signature = this.Signature,

                ResponseCode = this.ResponseCode,
                ResponseMessage = this.ResponseMessage,
                Status = this.Status,
                ProcessorResponseCode = this.ProcessorResponseCode,
                ReconciliationReference = this.ReconciliationReference,

                MerchantReference = this.MerchantReference,
                ExpiryDate = string.IsNullOrEmpty(ExpiryDate) ? this.ExpiryDate : "***",
                CardNumber = string.IsNullOrEmpty(CardNumber) ? this.CardNumber : "***",
                TokenName = this.TokenName,
                CardHolderName = string.IsNullOrEmpty(CardHolderName) ? this.CardHolderName : "***",
                ReturnUrl = this.ReturnUrl,
                Currency = this.Currency,
                MerchantExtra = this.MerchantExtra,
                MerchantExtra1 = this.MerchantExtra1,
                MerchantExtra2 = this.MerchantExtra2,
                MerchantExtra3 = this.MerchantExtra3,
                MerchantExtra4 = this.MerchantExtra4
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