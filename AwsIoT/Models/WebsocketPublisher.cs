using System;
using Amazon.Runtime;
using Amazon.IotData;
using Amazon.IotData.Model;
using System.Text;
using Newtonsoft.Json;

namespace AwsIoT.Models
{
    public class WebsocketPublisher
    {
        public async void Publish(PublishMessageDTO publishMessageDTO)
        {
            var message = JsonConvert.SerializeObject(publishMessageDTO);

            var publishRequest = new PublishRequest
            {
                Topic = PublicTopic,
                Qos = 1,
                Payload = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(message))
            };

            try
            {
                await IotDataClient
                    .PublishAsync(publishRequest)
                    .ConfigureAwait(false); 
            }
            catch (Exception)
            {
                // log the error
                return;
            }

            return;
        }

        private AmazonIotDataClient IotDataClient =>
            new AmazonIotDataClient(AwsIotHostName, AWSCredentials);

        private BasicAWSCredentials AWSCredentials =>
            new BasicAWSCredentials(PublisherAccessKey, PublisherSecretKey);

        private string PublicTopic => "publicTopic";
        private string PublisherAccessKey => "accessKey";
        private string PublisherSecretKey => "secretKey";
        private string AwsIotHostName => "hello";

    }
}
