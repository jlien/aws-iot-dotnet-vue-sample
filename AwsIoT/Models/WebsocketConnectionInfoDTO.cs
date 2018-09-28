using System.Collections.Generic;

namespace AwsIoT.Models
{
    public class WebsocketConnectionInfoDTO
    {
        public string url;
        public IEnumerable<string> topics;

        public WebsocketConnectionInfoDTO(string url, List<string> topics)
        {
            this.url = url;
            this.topics = topics;
        }
    }
}
