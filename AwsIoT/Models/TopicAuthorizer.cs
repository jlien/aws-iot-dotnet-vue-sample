using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.IoT;
using Amazon.IoT.Model;
using Amazon.Runtime;

namespace AwsIoT.Models
{
    public interface ITopicAuthorizer
    {
        bool IsAuthorized(string topic);
        void GenerateProfile();
    }

    public class TopicAuthorizer
    {
        private string UserGuid;
        private string AwsIotEndpoint;
        private WebsocketUserDTO WebsocketUserDTO;

        public TopicAuthorizer(string userGuid)
        {
            UserGuid = userGuid;
        }

        public WebsocketConnectionInfoDTO GenerateProfile()
        {
            // Create a Cognito User with the UserGuid
            var cognitoUser = CreateCognitoUser(UserGuid);

            // Attach an IoT Policy to the User
            var policy = AttachIotPolicy();

            // Get Temporary Credentials for the User
            var credentials = GetTemporaryCredentials(WebsocketUserDTO);

            return new WebsocketConnectionInfoDTO(
                "PLACEHOLDER",
                TopicsForUser());
        }

        private List<string> TopicsForUser()
        {
            if (UserGuid.Equals("John"))
                return new List<string> { { ChatTopic.JohnTopic } };

            return new List<string> { { ChatTopic.PublicTopic } };
        }

        private async Task<AttachPolicyResponse> AttachIotPolicy()
        {
            var policyJson = IotPolicyService
                .CreatePolicyJson(UserGuid, AwsIotEndpoint);

           return await CreateAndAttachIotPolicyToUserAsync(
                UserGuid,
                WebsocketUserDTO.CognitoIdentityId,
                policyJson
            ).ConfigureAwait(false);
        }

        private async Task<AttachPolicyResponse> CreateAndAttachIotPolicyToUserAsync(string userGuid, string cognitoId, string policyJson)
        {
            var policyName = string.Format(IotConstants.IoTPolicyName, userGuid);

            // Create the policy in IoT
            try
            {
                await IotClient.CreatePolicyAsync(new CreatePolicyRequest
                {
                    PolicyDocument = policyJson,
                    PolicyName = policyName
                }).ConfigureAwait(false);
            }
            catch (ResourceAlreadyExistsException)
            {
                // We don't care if the policy exists
                return null;
            }

            // Attach the policy to the user
            return await IotClient.AttachPolicyAsync(new AttachPolicyRequest
            {
                PolicyName = policyName,
                Target = cognitoId
            }).ConfigureAwait(false);
        }

        private IotPolicyService IotPolicyService => new IotPolicyService();

        protected async Task<WebsocketUserDTO> CreateCognitoUser(string userGuid)
        {

            // Lookup the User GUID from the Context
            string cognitoIdentityId;
            string cognitoIdentityToken;

            // Create the Cognito User
            try
            {
                var openIdTokenResponse = await CreateCognitoUserAsync(userGuid)
                    .ConfigureAwait(false);
                cognitoIdentityId = openIdTokenResponse.IdentityId;
                cognitoIdentityToken = openIdTokenResponse.Token;
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Exception encountered while creating a Cognito User.", e);
            }

            return new WebsocketUserDTO(cognitoIdentityId,
                                        cognitoIdentityToken,
                                        userGuid);
        }

        private async Task<GetOpenIdTokenForDeveloperIdentityResponse> CreateCognitoUserAsync(string userGuid)
        {
            // Create the login request
            var loginDictionary = new Dictionary<string, string> { 
                { IdentityProviderName, userGuid }
            };

            var openIdTokenRequest =
                new GetOpenIdTokenForDeveloperIdentityRequest
                {
                    IdentityPoolId = CognitoIdentityPoolId,
                    Logins = loginDictionary,
                    TokenDuration = TokenDuration
                };

            // Return the login response
            return await CognitoIdentityClient
                .GetOpenIdTokenForDeveloperIdentityAsync(openIdTokenRequest)
                .ConfigureAwait(false);
        }

        private async Task<Credentials> GetTemporaryCredentials(WebsocketUserDTO websocketUserDTO)
        {
            Credentials credentials;

            try
            {
                var loginDictionary = new Dictionary<string, string>
                {
                    { IdentityProviderName, websocketUserDTO.CognitoIdentityToken }
                };

                var getCredentialsForIdentityRequest =
                    new GetCredentialsForIdentityRequest
                    {
                    IdentityId = websocketUserDTO.CognitoIdentityId,
                        Logins = loginDictionary
                    };

                var getCredentialsForIdentityResponse = await CognitoIdentityClient
                    .GetCredentialsForIdentityAsync(getCredentialsForIdentityRequest)
                    .ConfigureAwait(false);

                credentials = getCredentialsForIdentityResponse.Credentials;
            }
            catch (Exception e)
            {
                throw;
            }

            return credentials;
        }

        private string IdentityProviderName => "IdentityProviderName";
        private string CognitoIdentityPoolId => "CognitoIdentityPoolId";
        private long TokenDuration => 3600;
        private string IoTAWSAccessKey => "IoTAWSAccessKey";
        private string IoTAWSSecretKey => "IoTAWSSecretKey";
        private string IoTAWSRegion => "IoTAWSRegion";


        private AmazonCognitoIdentityClient CognitoIdentityClient
        {
            get
            {
                var region = RegionEndpoint.GetBySystemName(IoTAWSRegion);
                return new AmazonCognitoIdentityClient(AWSCredentials, region);
            }
        }

        private BasicAWSCredentials AWSCredentials =>
            new BasicAWSCredentials(IoTAWSAccessKey, IoTAWSSecretKey);


        private AmazonIoTClient IotClient
        {
            get
            {
                var region = RegionEndpoint.GetBySystemName(IoTAWSRegion);
                return new AmazonIoTClient(AWSCredentials, region);
            }
        }
    }
}
