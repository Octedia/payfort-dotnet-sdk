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

namespace APS.DotNetSDK.Web.Installments
{
    public class InstallmentsProvider : IInstallmentsProvider
    {
        private readonly ILogger<InstallmentsProvider> _logger;
        private readonly IApiProxy _apiProxy;

        private readonly ApsEnvironmentConfiguration _configuration;

        private readonly ISignatureProvider _signatureProvider;
        private readonly ISignatureValidator _signatureValidator;

        private readonly object _lockObject = new object();
        private bool _disposed;

        internal InstallmentsProvider(IApiProxy apiProxy)
        {
            SdkConfiguration.Validate();

            var configuration = new ApsConfiguration(SdkConfiguration.IsTestEnvironment);
            _configuration = configuration.GetEnvironmentConfiguration();

            _signatureProvider = new SignatureProvider();
            _signatureValidator = new SignatureValidator();

            _apiProxy = apiProxy;

            _logger = SdkConfiguration.ServiceProvider.GetService<ILogger<InstallmentsProvider>>();
        }

        /// <summary>
        /// Constructor for Installments Provider
        /// </summary>
        /// <exception cref="Exceptions.SdkConfigurationException">Get the exception when sdk configuration is not set</exception>
        public InstallmentsProvider() : this(new ApiProxy())
        {
        }

        public async Task<GetInstallmentsResponseCommand> GetInstallmentsPlansAsync(GetInstallmentsRequestCommand command, string sdkConfigurationId = null)
        {
            command.Signature = CalculateSignature(command, sdkConfigurationId);
            var responseCommand = await SendRequestAsync(command);

            ValidateResponseSignature(responseCommand, sdkConfigurationId);

            return responseCommand;
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

        private async Task<GetInstallmentsResponseCommand> SendRequestAsync(GetInstallmentsRequestCommand command)
        {
            _logger.LogInformation(
                $"Sending the request to APS to get the installments plans [Request:{@command.ToAnonymizedJson()}]");
            _logger.LogDebug(
                $"Sending the request to APS to get the installments plans [Request:{@command.ToAnonymizedJson()}]");

            try
            {
                var response = await _apiProxy.PostAsync<GetInstallmentsRequestCommand, GetInstallmentsResponseCommand>(
                    command,
                    _configuration.Installments.BaseUrl,
                    _configuration.Installments.RequestUri);

                _logger.LogInformation(
                    $"Response received from the gateway: [Response:{@response.ToAnonymizedJson()}]");
                _logger.LogDebug(
                    $"Response received from the gateway: [Response:{@response.ToAnonymizedJson()}]");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}. [Request:{@command.ToAnonymizedJson()}]");
                throw;
            }
        }

        private string CalculateSignature<TRequest>(TRequest command, string sdkConfigurationId = null) where TRequest : GetInstallmentsRequestCommand
        {
            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            _logger.LogDebug($"Starting signature calculation for [RequestObject:{@command.ToAnonymizedJson()}]");

            var signature = _signatureProvider.GetSignature(command, sdkConfiguration.RequestShaPhrase,
                sdkConfiguration.ShaType);

            _logger.LogDebug($"Generated signature for [Signature:{signature}]");

            return signature;
        }

        private void ValidateResponseSignature<TResponse>(TResponse responseCommand, string sdkConfigurationId = null) where TResponse : GetInstallmentsResponseCommand
        {
            _logger.LogDebug($"Validate signature for response received from APS [Response:{@responseCommand.ToAnonymizedJson()}]");

            var sdkConfiguration = SdkConfiguration.GetSdkConfiguration(sdkConfigurationId);

            var isResponseSignatureValid = _signatureValidator.ValidateSignature(responseCommand,
                sdkConfiguration.ResponseShaPhrase, sdkConfiguration.ShaType, responseCommand.Signature);

            _logger.LogDebug(
                $"Signature validation is {isResponseSignatureValid} for [Response:{@responseCommand.ToAnonymizedJson()}]");

            if (!isResponseSignatureValid)
            {
                var actualSignature = _signatureProvider.GetSignature(responseCommand, sdkConfiguration.RequestShaPhrase,
                    sdkConfiguration.ShaType);

                _logger.LogError($"Signature mismatch when validating the payment gateway response " +
                                 $"Response:{@responseCommand.ToAnonymizedJson()}" +
                                 $"ExpectedSignature:{responseCommand.Signature}," +
                                 $"ActualSignature:{actualSignature}]");

                throw new SignatureException($"Mismatch signature when validating the payment gateway response [" +
                                             $"ExpectedSignature:{responseCommand.Signature}," +
                                             $"ActualSignature:{actualSignature}]");
            }
        }

        #endregion
    }
}
