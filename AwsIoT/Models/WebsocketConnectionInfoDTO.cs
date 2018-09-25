using System;
namespace AwsIoT.Models
{
    public class WebsocketConnectionInfoDTO
    {
        public string url;

        public WebsocketConnectionInfoDTO(string url)
        {
            this.url = url;
        }
    }
}
