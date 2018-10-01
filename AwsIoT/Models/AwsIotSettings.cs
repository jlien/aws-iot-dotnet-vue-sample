using static System.Environment;

namespace AwsIoT.Models
{
    public static class AwsIotSettings
    {
        public static string AccessKey =>
            GetEnvironmentVariable("AwsIotAccessKey");

        public static string SecretKey =>
            GetEnvironmentVariable("AwsIotSecretKey");

        public static string HostName =>
            GetEnvironmentVariable("AwsIotHostName");
    }
}
