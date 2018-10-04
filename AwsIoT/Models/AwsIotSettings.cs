using static System.Environment;

namespace AwsIoT.Models
{
    public static class AwsIotSettings
    {
        public static string SubscriberAccessKey =>
        GetEnvironmentVariable("AwsIotSubscriberAccessKey");

        public static string SubscriberSecretKey =>
        GetEnvironmentVariable("AwsIotSubscriberSecretKey");

        public static string PublisherAccessKey =>
        GetEnvironmentVariable("AwsIotPublisherAccessKey");

        public static string PublisherSecretKey =>
            GetEnvironmentVariable("AwsIotPublisherSecretKey");


        public static string HostName =>
            GetEnvironmentVariable("AwsIotHostName");

        public static string Endpoint =>
            GetEnvironmentVariable("AwsIotEndpoint");

        public static string Region => Amazon.RegionEndpoint.USEast1.ToString();

        public static string IdentityPoolId =>
            GetEnvironmentVariable("AwsCognitoIdentityPoolId");

        public static string CognitoProviderName =>
        GetEnvironmentVariable("AwsCognitoProviderName");

        public static string IdentityProviderName =>
        GetEnvironmentVariable("AwsCognitoIdentityProviderName");

        public static long TokenDuration => 43200;
    }
}
