namespace AwsIoT.Models
{
    public static class IotConstants
    {
        public const string IoTPolicyName = "Policy_{0}";

        public const string IoTPolicyJsonTemplate = @"{{ 
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""{0}"",
                    ""Action"": [""iot:Subscribe""],
                    ""Resource"": {1}
                }},
                {{
                    ""Effect"": ""{2}"",
                    ""Action"": [""iot:Receive""],
                    ""Resource"": {3}
                }},
                {{
                    ""Effect"": ""{4}"",
                    ""Action"": [""iot:Publish""],
                    ""Resource"": {5}
                }},
                {{
                    ""Effect"": ""Allow"",
                    ""Action"": [""iot:Connect""],
                    ""Resource"": [""*""]
                }}
            ]
        }}";
    }
}
