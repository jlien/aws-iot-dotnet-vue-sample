# aws-iot-sample
.NET &amp; Vue AWS IoT sample app

## Contrived Example with Authorization on websockets

Everyone loves to chat. But it Sucks to be John. John can only listen to what Jane says in the public chat room.

In this project we set up a public chatroom with AWS IoT and Cognito. Everyone except John can subscribe to all messages. John can only see what Jane says.

## Configuration

There are 2 AWS IAM roles that need to be configured for this demo.

1. WebsocketPublisher: For creating a Message via the HTTP API
2. WeboscketSubscriber: For creating a Websocket user that can connect to the SigV4 Websocket URL

There are several variables in AwsIotSettings that can be overwritten or set with Environment variables. Most of these are the token & secrets for each role, IoT endpoint and region, Cognito Identity Pool, and your custom Cognito identity provider.

## Need Help?

Contact me on twitter: @jameslien
