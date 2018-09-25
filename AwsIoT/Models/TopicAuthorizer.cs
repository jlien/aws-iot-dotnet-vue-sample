using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
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
        public string UserGuid;

        public TopicAuthorizer(string userGuid)
        {
            UserGuid = userGuid;
        }

        public WebsocketConnectionInfoDTO GenerateProfile()
        {
            // Create a Cognito User with the UserGuid
            var cognitoUser = CreateCognitoUser(UserGuid);

            // Attach an IoT Policy to the User
            //TODO: var policy = AttachIotPolicy();

            // Get Temporary Credentials for the User
            //TODO: var credentials = GetTemporaryCredentials();

            return new WebsocketConnectionInfoDTO("PLACEHOLDER");
        }

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

        private void AttachIotPolicy()
        {
            // TODO:
        }

        private void GetTemporaryCredentials()
        {
            // TODO:
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
                var awsCredentials =
                    new BasicAWSCredentials(IoTAWSAccessKey, IoTAWSSecretKey);
                var region = RegionEndpoint.GetBySystemName(IoTAWSRegion);

                return new AmazonCognitoIdentityClient(awsCredentials, region);
            }
        }
    }
}
