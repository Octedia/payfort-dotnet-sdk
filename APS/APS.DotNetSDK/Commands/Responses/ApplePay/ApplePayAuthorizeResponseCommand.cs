using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Web;
using APS.DotNetSDK.Exceptions;
using APS.DotNetSDK.Utils;

namespace APS.DotNetSDK.Commands.Responses.ApplePay
{
    public class ApplePayAuthorizeResponseCommand : AuthorizeResponseCommand
    {
        private const string CommandKey = "command";
        
        [JsonPropertyName("apple_data")]
        public string AppleData { get; set; }

        [JsonPropertyName("apple_header")]
        public ApplePayHeader Header { get; set; }

        [JsonPropertyName("apple_paymentMethod")]
        public ApplePayPaymentMethod PaymentMethod { get; set; }

        [JsonPropertyName("apple_signature")]
        public string AppleSignature { get; set; }

        [JsonPropertyName("digital_wallet")]
        public string DigitalWallet { get; set; }

        public override void BuildNotificationCommand(IDictionary<string, string> dictionaryObject)
        {
            if (!dictionaryObject.Keys.Contains(CommandKey))
            {
                throw new InvalidNotification(
                    "Response does not contain any command. Please contact the SDK provider.");
            }

            var responseCommand = dictionaryObject.CreateObject<ApplePayAuthorizeResponseCommand>();

            if (responseCommand == null)
            {
                throw new InvalidNotification(
                    $"There was an issue when mapping notification request: [" +
                    $"{dictionaryObject.JoinElements(",")}] to AuthorizeResponseCommand");
            }

            if (dictionaryObject["command"] != Command)
            {
                throw new InvalidNotification(
                    $"Invalid Command received from payment gateway:{dictionaryObject["command"]}");
            }

            Signature = responseCommand.Signature;

            AccessCode = responseCommand.AccessCode;
            MerchantIdentifier = responseCommand.MerchantIdentifier;
            Language = responseCommand.Language;
            MerchantReference = responseCommand.MerchantReference;
            FortId = responseCommand.FortId;
            ResponseMessage = responseCommand.ResponseMessage;
            ResponseCode = responseCommand.ResponseCode;
            Status = responseCommand.Status;
            AcquirerResponseCode = responseCommand.AcquirerResponseCode;
            AcquirerResponseMessage = responseCommand.AcquirerResponseMessage;
            ReconciliationReference = responseCommand.ReconciliationReference;
            ProcessorResponseCode = responseCommand.ProcessorResponseCode;

            Amount = responseCommand.Amount;
            Currency = responseCommand.Currency;
            CustomerEmail = responseCommand.CustomerEmail;
            TokenName = responseCommand.TokenName;
            PaymentOption = responseCommand.PaymentOption;
            SadadOlp = responseCommand.SadadOlp;
            KnetRefNumber = responseCommand.KnetRefNumber;
            ThirdPartyTransactionNumber = responseCommand.ThirdPartyTransactionNumber;
            Eci = responseCommand.Eci;
            Description = responseCommand.Description;
            CustomerIp = responseCommand.CustomerIp;
            CustomerName = responseCommand.CustomerName;
            MerchantExtra = responseCommand.MerchantExtra;
            MerchantExtra1 = responseCommand.MerchantExtra1;
            MerchantExtra2 = responseCommand.MerchantExtra2;
            MerchantExtra3 = responseCommand.MerchantExtra3;
            MerchantExtra4 = responseCommand.MerchantExtra4;
            MerchantExtra5 = responseCommand.MerchantExtra5;
            AuthorizationCode = responseCommand.AuthorizationCode;
            CardHolderName = responseCommand.CardHolderName;
            ExpiryDate = responseCommand.ExpiryDate;
            CardNumber = responseCommand.CardNumber;
            Secure3dsUrl = responseCommand.Secure3dsUrl;
            RememberMe = responseCommand.RememberMe;
            ReturnUrl = HttpUtility.UrlDecode(responseCommand.ReturnUrl);
            PhoneNumber = responseCommand.PhoneNumber;
            SettlementReference = responseCommand.SettlementReference;
            BillingStateProvince = responseCommand.BillingStateProvince;
            BillingProvinceCode = responseCommand.BillingProvinceCode;
            BillingStreet = responseCommand.BillingStreet;
            BillingStreet2 = responseCommand.BillingStreet2;
            BillingPostCode = responseCommand.BillingPostCode;
            BillingCountry = responseCommand.BillingCountry;
            BillingCompany = responseCommand.BillingCompany;
            BillingCity = responseCommand.BillingCity;
            ShippingStateProvince = responseCommand.ShippingStateProvince;
            ShippingProvinceCode = responseCommand.ShippingProvinceCode;
            ShippingStreet = responseCommand.ShippingStreet;
            ShippingStreet2 = responseCommand.ShippingStreet2;
            ShippingSource = responseCommand.ShippingSource;
            ShippingSameAsBilling = responseCommand.ShippingSameAsBilling;
            ShippingPostCode = responseCommand.ShippingPostCode;
            ShippingCountry = responseCommand.ShippingCountry;
            ShippingCompany = responseCommand.ShippingCompany;
            ShippingCity = responseCommand.ShippingCity;

            AgreementId = responseCommand.AgreementId;
            RecurringMode = responseCommand.RecurringMode;
            RecurringTransactionsCount = responseCommand.RecurringTransactionsCount;

            FraudComment = responseCommand.FraudComment;

            AppleData = responseCommand.AppleData;
            // Will be null
            Header = responseCommand.Header;
            // Will be null
            PaymentMethod = responseCommand.PaymentMethod;
            AppleSignature = responseCommand.AppleSignature;
            DigitalWallet = responseCommand.DigitalWallet;
        }
    }
}
