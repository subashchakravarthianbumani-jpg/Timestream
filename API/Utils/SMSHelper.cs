using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Model.DomainModel;
using Model.ViewModel;
using OtpNet;
using Serilog;
using Utils.Interface;

namespace Utils
{
    public class SMSHelper : ISMSHelper
    {
        private SMSConfiguration _confg;
        private readonly IConfiguration _configuration;
        public SMSHelper(IOptions<SMSConfiguration> confg, IConfiguration configuration)
        {
            _confg = confg.Value;
            _configuration = configuration;
        }
        public bool SentSMS(List<string> mobileNumbers, string templateCode, IDictionary<string, string> messageReplaces, out string message_out)
        {
            message_out = "";

            if (mobileNumbers.Count > 0 && !string.IsNullOrEmpty(templateCode) && !string.IsNullOrEmpty(_confg.RequestURL) && messageReplaces.Count > 0)
            {
                string Message = _confg.Templates?.Find(x => x.Code == templateCode)?.Template ?? "";
                foreach (KeyValuePair<string, string> replace in messageReplaces)
                {
                    Message = Message.Replace(replace.Key, replace.Value);
                }

                message_out = Message;

                HttpClient _httpClient = new HttpClient();

                string QString = "?";

                QString += "user=" + _confg.UserName;
                QString += "&password=" + _confg.Password;
                QString += "&senderid=" + _confg.senderid;
                QString += "&channel=" + _confg.channel;
                QString += "&DCS=" + _confg.DCS;
                QString += "&flashsms=" + _confg.flashsms;
                QString += "&number=" + string.Join(",", mobileNumbers);
                QString += "&text=" + Message;
                QString += "&route=" + _confg.route;

                HttpResponseMessage message = _httpClient.GetAsync(_confg.RequestURL + QString).Result;

                if (message.StatusCode == System.Net.HttpStatusCode.Forbidden || message.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return false;
                }
                else if (message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
            }

            return false;
        }

        public string GenerateOtp()
        {
            try
            {
                string base32Secret = _configuration.GetSection("LoginTOTPSecretKey").Value.ToString();

                // Parse the secret key
                var secretBytes = Base32Encoding.ToBytes(base32Secret);

                // Create a TOTP generator
                var totp = new Totp(secretBytes, totpSize: 6, mode: OtpHashMode.Sha256);

                // Generate a TOTP code
                string totpCode = totp.ComputeTotp(); // By default, it uses the current time

                return totpCode;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return string.Empty;
            }
        }

        public bool ValidateOtp(string OTP)
        {
            try
            {
                string base32Secret = _configuration.GetSection("LoginTOTPSecretKey").Value.ToString();

                // Parse the secret key
                var secretBytes = Base32Encoding.ToBytes(base32Secret);

                // Create a TOTP generator
                var totp = new Totp(secretBytes, totpSize: 6, mode: OtpHashMode.Sha256);

                // Validate the user-entered OTP
                bool isOTPValid = totp.VerifyTotp(OTP, out long timeStepMatched);

                return isOTPValid;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return false;
            }
        }
    }
}
