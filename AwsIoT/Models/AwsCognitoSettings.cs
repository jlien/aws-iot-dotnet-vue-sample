using static System.Environment;

namespace AwsIot.Models
{
    public static class AwsCognitoSettings
    {
        public static string AccessKey =>
            GetEnvironmentVariable("AwsCognitoAccessKey");

        public static string SecretKey =>
            GetEnvironmentVariable("AwsCognitoSecretKey");

        public static string IdentityPoolId =>
            GetEnvironmentVariable("AwsCognitoIdentityPoolId");

        public static string IdentityProviderName =>
            GetEnvironmentVariable("AwsCognitoIdentityProviderName");

        public static long TokenDuration =>
            int.Parse(GetEnvironmentVariable("TokenDuration"));
    }
}
