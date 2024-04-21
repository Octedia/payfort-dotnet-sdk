﻿using System;
using APS.DotNetSDK.Service;
using System.Threading.Tasks;
using APS.DotNetSDK.Signature;
using APS.DotNetSDK.Exceptions;
using APS.DotNetSDK.Configuration;
using Microsoft.Extensions.Logging;
using APS.DotNetSDK.Commands.Requests;
using APS.DotNetSDK.Commands.Responses;
using Microsoft.Extensions.DependencyInjection;
using APS.DotNetSDK.Commands.Requests.ApplePay;
using APS.DotNetSDK.Commands.Responses.ApplePay;

namespace APS.DotNetSDK.Maintenance
{
    public class MaintenanceOperations : IMaintenanceOperations
    {
        private readonly ILogger<MaintenanceOperations> _logger;
        private readonly ApsEnvironmentConfiguration _configuration;
        private readonly IApiProxy _apiProxy;

        private readonly ISignatureProvider _signatureProvider;
        private readonly ISignatureValidator _signatureValidator;

        private readonly object _lockObject = new object();
        private bool _disposed;

        internal MaintenanceOperations(IApiProxy apiProxy)
        {
            SdkConfiguration.Validate();

            var configuration = new ApsConfiguration(SdkConfiguration.IsTestEnvironment);
            _configuration = configuration.GetEnvironmentConfiguration();

            _signatureProvider = new SignatureProvider();
            _signatureValidator = new SignatureValidator();

            _apiProxy = apiProxy;

            _logger = SdkConfiguration.ServiceProvider.GetService<ILogger<MaintenanceOperations>>();
        }
        /// <summary>
        /// Constructor for Maintenance Operations
        /// </summary>
        /// <exception cref="Exceptions.SdkConfigurationException">Get the exception when sdk configuration is not set</exception>
        public MaintenanceOperations()
            : this(new ApiProxy())
        {
        }

        public async Task<AuthorizeResponseCommand> AuthorizeAsync(AuthorizeRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<AuthorizeRequestCommand, AuthorizeResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType);
        }

        public async Task<PurchaseResponseCommand> PurchaseAsync(PurchaseRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<PurchaseRequestCommand, PurchaseResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType);
        }

        public async Task<ApplePayAuthorizeResponseCommand> AuthorizeAsync(AuthorizeRequestCommand authorizeRequestCommand,
            ApplePayRequestCommand applePayRequestCommand, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            var requestCommand = new ApplePayAuthorizeRequestCommand(authorizeRequestCommand, applePayRequestCommand, sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<ApplePayAuthorizeRequestCommand, ApplePayAuthorizeResponseCommand>(
                requestCommand, sdkConfiguration.ApplePay.RequestShaPhrase, sdkConfiguration.ApplePay.ResponseShaPhrase, sdkConfiguration.ApplePay.ShaType);
        }

        public async Task<ApplePayPurchaseResponseCommand> PurchaseAsync(PurchaseRequestCommand purchaseRequestCommand,
            ApplePayRequestCommand applePayRequestCommand, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            var requestCommand = new ApplePayPurchaseRequestCommand(purchaseRequestCommand, applePayRequestCommand, sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<ApplePayPurchaseRequestCommand, ApplePayPurchaseResponseCommand>(
                requestCommand, sdkConfiguration.ApplePay.RequestShaPhrase, sdkConfiguration.ApplePay.ResponseShaPhrase, sdkConfiguration.ApplePay.ShaType);
        }

        public async Task<CaptureResponseCommand> CaptureAsync(CaptureRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<CaptureRequestCommand, CaptureResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType);
        }

        public async Task<VoidResponseCommand> VoidAsync(VoidRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<VoidRequestCommand, VoidResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType);
        }

        public async Task<RefundResponseCommand> RefundAsync(RefundRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<RefundRequestCommand, RefundResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType);
        }

        public async Task<CheckStatusResponseCommand> CheckStatusAsync(CheckStatusRequestCommand command, string sdkConfigurationId = null)
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            return await SendRequestToPaymentGatewayAsync<CheckStatusRequestCommand, CheckStatusResponseCommand>
                (command, sdkConfiguration.RequestShaPhrase, sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType, true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lockObject)
            {
                if (disposing && _disposed == false)
                {
                    _apiProxy.Dispose();
                    _disposed = true;
                }
            }
        }

        #region private methods
        private async Task<TResponse> SendRequestToPaymentGatewayAsync<TRequest, TResponse>(TRequest command, string requestShaPhrase, string responseShaPhrase, ShaType shaType, bool ignoreLogging = false)
            where TRequest : RequestCommand
            where TResponse : ResponseCommand
        {
            ValidateMandatoryProperties(command, ignoreLogging);

            command.Signature = CalculateSignature(command, ignoreLogging, requestShaPhrase, shaType);

            var responseCommand = await SendRequestAsync<TRequest, TResponse>(command, ignoreLogging);

            ValidateResponseSignature(responseCommand, requestShaPhrase, responseShaPhrase, shaType, ignoreLogging);
            
            return responseCommand;
        }

        private void ValidateMandatoryProperties<TRequest>(TRequest command, bool ignoreLogging) where TRequest : RequestCommand
        {
            if (!ignoreLogging)
            {
                _logger.LogInformation($"Validation of mandatory properties for [MerchantReference:{command.MerchantReference}]");
            }

            command.ValidateMandatoryProperties();

            if (!ignoreLogging)
            {
                _logger.LogInformation($"Validation successfully for [MerchantReference:{command.MerchantReference}]");
            }
        }

        private string CalculateSignature<TRequest>(TRequest command, bool ignoreLogging, string requestShaPhrase, ShaType shaType) where TRequest : RequestCommand
        {
            if (!ignoreLogging)
            {
                _logger.LogDebug($"Starting signature calculation for [MerchantReference:{command.MerchantReference},RequestObject:{@command.ToAnonymizedJson()}]");
            }

            var signature = _signatureProvider.GetSignature(command, requestShaPhrase, shaType);

            if (!ignoreLogging)
            {
                _logger.LogInformation($"Generated signature for [MerchantReference:{command.MerchantReference},Signature:{signature}]");
            }
            return signature;
        }

        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(TRequest command, bool ignoreLogging) where TRequest : RequestCommand
            where TResponse : ResponseCommand
        {
            if (!ignoreLogging)
            {
                _logger.LogInformation($"Sending the request to APS [MerchantReference:{command.MerchantReference}]");
                _logger.LogDebug($"Sending the request to APS [MerchantReference:{command.MerchantReference},Request:{@command.ToAnonymizedJson()}");
            }

            try
            {
                var response = await _apiProxy.PostAsync<TRequest, TResponse>(
                    command,
                    _configuration.MaintenanceOperations.BaseUrl,
                    _configuration.MaintenanceOperations.RequestUri);

                if (!ignoreLogging && response != null)
                {
                    _logger.LogInformation($"Response received from the gateway: [MerchantReference:{response.MerchantReference},FortId: {response.FortId}]");
                    _logger.LogDebug($"Response received from the gateway: [MerchantReference:{response.MerchantReference},Response:{@response.ToAnonymizedJson()}]");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}. [Request:{@command.ToAnonymizedJson()}]");
                throw;
            }
        }

        private void ValidateResponseSignature<TResponse>(TResponse responseCommand, string requestShaPhrase, string responseShaPhrase, ShaType shaType, bool ignoreLogging) where TResponse : ResponseCommand
        {
            if (!ignoreLogging)
            {
                _logger.LogDebug($"Validate signature for response received from APS [MerchantReference:{responseCommand.MerchantReference}]");
            }
            var isResponseSignatureValid = _signatureValidator.ValidateSignature(responseCommand, responseShaPhrase, shaType, responseCommand.Signature);

            if (!ignoreLogging)
            {
                _logger.LogDebug(
                    $"Signature validation is {isResponseSignatureValid} for [MerchantReference:{responseCommand.MerchantReference}]");
            }

            if (!isResponseSignatureValid)
            {
                var actualSignature = _signatureProvider.GetSignature(responseCommand, requestShaPhrase, shaType);

                _logger.LogError($"Signature mismatch when validating the payment gateway response " +
                                 $"[MerchantReference:{responseCommand.MerchantReference}," +
                                 $"Response:{@responseCommand.ToAnonymizedJson()}" +
                                 $"ExpectedSignature:{responseCommand.Signature}," +
                                 $"ActualSignature:{actualSignature}]");

                throw new SignatureException($"Mismatch signature when validating the payment gateway response [" +
                                             $"[MerchantReference:{responseCommand.MerchantReference}," +
                                             $"ExpectedSignature:{responseCommand.Signature}," +
                                             $"ActualSignature:{actualSignature}]");
            }
        }
        #endregion
    }
}