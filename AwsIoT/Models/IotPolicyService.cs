using System.Collections.Generic;
using System.Linq;
using System.IO;
using YamlDotNet.Serialization;
using Newtonsoft.Json;

namespace AwsIoT.Models
{
    public interface IIotPolicyService
    {
        string CreatePolicyJson(string userGuid, string awsEndpoint);
    }

    public class IotPolicyService : IIotPolicyService
    {
        private const string TopicResource = ":topic/";
        private const string TopicFilterResource = ":topicfilter/";
        private const string Allow = "Allow";
        private const string Deny = "Deny";
        private IEnumerable<Policy> policyList;

        public string CreatePolicyJson(string userGuid, string awsEndpoint)
        {
            var subResources = PolicyList.Where(p => p.Subscribe)
                .Select(p => $"{awsEndpoint}{TopicFilterResource}{GenerateTopicString(p, userGuid)}").ToList();

            var recResources = PolicyList.Where(p => p.Receive)
                .Select(p => $"{awsEndpoint}{TopicResource}{GenerateTopicString(p, userGuid)}").ToList();

            var pubResources = PolicyList.Where(p => p.Publish)
                .Select(p => $"{awsEndpoint}{TopicResource}{GenerateTopicString(p, userGuid)}").ToList();

            return string.Format(
                IotConstants.IoTPolicyJsonTemplate,
                AllowOrDeny(subResources),
                GetResourceString(subResources),
                AllowOrDeny(recResources),
                GetResourceString(recResources),
                AllowOrDeny(pubResources),
                GetResourceString(pubResources)
            );
        }

        public Dictionary<string, string> GetWebSocketTopics(string userGuid)
        {
            return PolicyList.ToDictionary(policy => policy.Name, policy => GenerateTopicString(policy, userGuid));
        }

        private string GenerateTopicString(Policy policy, string userGuid)
        {
            return policy.IsUserSpecific ? $"{policy.Topic}/{userGuid}" : $"{policy.Topic}";
        }

        private string AllowOrDeny(IEnumerable<string> actionResources)
        {
            return actionResources.ToList().Count > 0 ? Allow : Deny;
        }

        private string GetResourceString(IEnumerable<string> actionResources)
        {
            return actionResources.ToList().Count > 0 ? JsonConvert.SerializeObject(actionResources) :
                                  JsonConvert.SerializeObject(new List<string> { "*" });
        }


        #region YAML Parsing

        /*
         * To avoid reading and parsing the YAML file multiple times, we are going to load and process the file on the
         * first request and create a template of a list of a helper class to make it easier to work with.
         */
        private IEnumerable<Policy> PolicyList
        {
            get
            {
                // Return if we have already read in the file
                if (policyList != null)
                {
                    return policyList;
                }

                // Read all of the YAML into a string
                var input = File.ReadAllText(
                    Path.Combine("Shared", "WebSockets", "IotPolicies.yml"));

                // Map it to a List of Policy objects and save it
                var deserializer = new Deserializer();
                return policyList = deserializer.Deserialize<IEnumerable<Policy>>(input).ToList();
            }
        }

        private class Policy
        {
            [YamlMember(typeof(string), Alias = "name")]
            public string Name { get; set; }

            [YamlMember(typeof(string), Alias = "topic")]
            public string Topic { get; set; }

            [YamlMember(typeof(bool), Alias = "subscribe")]
            public bool Subscribe { get; set; }

            [YamlMember(typeof(bool), Alias = "receive")]
            public bool Receive { get; set; }

            [YamlMember(typeof(bool), Alias = "publish")]
            public bool Publish { get; set; }

            [YamlMember(typeof(bool), Alias = "user_specific")]
            public bool IsUserSpecific { get; set; }
        }

        #endregion
    }
}
