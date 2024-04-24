using APS.DotNetSDK.Utils;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace APS.DotNetSDK.Commands
{
    public class Command
    {
        [IgnoreOnSignatureCalculation(true)]
        [JsonPropertyName("signature")]
        public string Signature { get; set; }

        public bool CheckIfApplePayCommand()
        {

            var digitalWallet = GetType().GetProperty("DigitalWallet");
            if (digitalWallet != null)
            {
                var digitalWalletValue = digitalWallet.GetValue(this);
                Debug.Assert(digitalWalletValue is string);
                return digitalWalletValue.Equals("APPLE_PAY");
            }

            return false;

        }
    }
}
