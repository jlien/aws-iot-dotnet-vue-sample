using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Amazon.CognitoIdentity.Model;

namespace AwsIoT.Models
{
    public class WebsocketUrlBuilder
    {
        private const string Protocol = "wss";
        private const string Uri = "/mqtt";
        private const string Method = "GET";
        private const string Service = "iotdevicegateway";
        private const string Algorithm = "AWS4-HMAC-SHA256";

        public string GetWebsocketUrl(Credentials credentials, string timestamp = null)
        {
            var url = $"{Protocol}://{AwsIotSettings.Endpoint}{Uri}?{GetCanonicalQueryString(credentials, timestamp)}";
            return url;
        }

        public string GetCanonicalQueryString(Credentials credentials, string timestamp = null)
        {
            var datetime = DateTime.UtcNow.ToString(Amazon.Util.AWSSDKUtils.ISO8601BasicDateTimeFormat);
            if (!string.IsNullOrEmpty(timestamp))
            {
                if (DateTime.TryParse(timestamp, out var parsedDate))
                {
                    datetime = parsedDate.ToString(Amazon.Util.AWSSDKUtils.ISO8601BasicDateTimeFormat);
                }
            }

            var date = datetime.Substring(0, 8);

            var credentialScope = $"{date}/{"us-east-1"}/{Service}/aws4_request";
            var canonicalQuerystring = $"X-Amz-Algorithm={Algorithm}";
            canonicalQuerystring +=
                $"&X-Amz-Credential={System.Uri.EscapeDataString($"{credentials.AccessKeyId}/{credentialScope}")}";
            canonicalQuerystring += $"&X-Amz-Date={datetime}";
            canonicalQuerystring += "&X-Amz-SignedHeaders=host";
            var canonicalHeaders = $"host:{AwsIotSettings.Endpoint}\n";
            var payloadHash = Sha256("");
            var canonicalRequest = $"{Method}\n{Uri}\n{canonicalQuerystring}\n{canonicalHeaders}\nhost\n{payloadHash}";

            var stringToSign = $"{Algorithm}\n{datetime}\n{credentialScope}\n{Sha256(canonicalRequest)}";
            var signingKey = GetSignatureKey(credentials.SecretKey, date, "us-east-1", Service);
            var signature = ConvertByteArrayToString(HmacSha256(stringToSign, signingKey));
            canonicalQuerystring += $"&X-Amz-Signature={signature}";
            if (!string.IsNullOrEmpty(credentials.SessionToken))
            {
                canonicalQuerystring +=
                    $"&X-Amz-Security-Token={System.Uri.EscapeDataString(credentials.SessionToken)}";
            }

            return canonicalQuerystring;
        }

        #region Encoding Utilities

        private static byte[] HmacSha256(string data, byte[] key)
        {
            var kha = new HMACSHA256(key)
            {
                Key = key
            };

            return kha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private string Sha256(string data)
        {
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to a string   
                return ConvertByteArrayToString(bytes);
            }
        }

        private string ConvertByteArrayToString(IEnumerable<byte> bytes)
        {
            // Convert byte array to a string   
            var builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }

        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            var kSecret = Encoding.UTF8.GetBytes(($"AWS4{key}").ToCharArray());
            var kDate = HmacSha256(dateStamp, kSecret);
            var kRegion = HmacSha256(regionName, kDate);
            var kService = HmacSha256(serviceName, kRegion);
            var kSigning = HmacSha256("aws4_request", kService);

            return kSigning;
        }

        #endregion
    }
}
