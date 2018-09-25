using System;
namespace AwsIoT.Models
{
    public class WebsocketUserDTO
    {
        public string CognitoIdentityId;
        public string CognitoIdentityToken;
        public string UserGuid;

        public WebsocketUserDTO(string cognitoIdentityId,
                                string cognitoIdentityToken,
                                string userGuid)
        {
            CognitoIdentityId = cognitoIdentityId;
            CognitoIdentityToken = cognitoIdentityToken;
            UserGuid = userGuid;
        }
    }
}
