﻿using System;
using Amazon.Runtime;
using Amazon.IotData;
using Amazon.IotData.Model;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace AwsIoT.Models
{
    public class WebsocketPublisher
    {
        public async void Publish(PublishMessageDTO publishMessageDTO)
        {

            List<string> list = TopicsForUser(publishMessageDTO.UserGuid);
            for (int i = 0; i < list.Count; i++)
            {
                string topic = (string)list[i];
                await PublishMessageToTopic(publishMessageDTO, topic);
            }
            return;
        }

        protected async Task PublishMessageToTopic(
            PublishMessageDTO publishMessageDTO,
            string topic)
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            var message = JsonConvert.SerializeObject(publishMessageDTO);
            var publishRequest = new PublishRequest
            {
                Topic = topic,
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
                throw;
            }
        }


        private List<string> TopicsForUser(string userGuid)
        {
            // If Jane publish to public and John
            if (userGuid.Equals("Jane"))
            {
                return new List<string> {
                    ChatTopic.PublicTopic,
                    ChatTopic.JohnTopic
                };
            }

            // otherwise send it to only the public channel
            return new List<string> { ChatTopic.PublicTopic };
        }

        private AmazonIotDataClient IotDataClient =>
            new AmazonIotDataClient(AwsIotHostName, AWSCredentials);

        private BasicAWSCredentials AWSCredentials =>
            new BasicAWSCredentials(PublisherAccessKey, PublisherSecretKey);

        private string PublisherAccessKey => "accessKey";
        private string PublisherSecretKey => "secretKey";
        private string AwsIotHostName => "hello";

    }
}
