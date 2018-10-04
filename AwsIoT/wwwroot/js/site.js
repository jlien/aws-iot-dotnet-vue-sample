// Write your JavaScript code.

var app = new Vue({
      el: '#iotApp',
      data: {
        message: 'Hello IoT!',
        userGuid: "",
        content: "",
        messages: [
            { "UserGuid": "Bot", "Content": "Enter Messages Here" }
        ],
        postResults: [],
        ajaxRequest: false,
        websocketConnectionUrl: "",
        connectionRetryCount: 0
    },
    methods: {
        sub: function(event) {
            event.preventDefault();

            var messageData = JSON.stringify({
                userGuid: this.userGuid,
                content: this.content
            });

            var that = this;

            $.ajax({
                url: '/Messages',
                type: 'POST',
                data: messageData,
                dataType: 'json',
                contentType: 'application/json',
                success: function(data, other){
                    that.content = "";
                },
                error: function(data){
                    alert(data.error);
                }
            });
        },
        submitName: function(event) {
            event.preventDefault();

            var nameData = JSON.stringify({
                userGuid: this.userGuid
            });

            var that = this;

            $.ajax({
                url: '/WebsocketProviderInfo',
                type: 'POST',
                data: nameData,
                dataType: 'json',
                contentType: 'application/json',
                success: function(data, other) {
                    that.configSocket(data.result);
                    $("#nameForm").hide();
                    $("#messageForm").show();
                },
                error: function() {
                    alert("Error fetching websocket connection info");
                }
            });
        },
        configSocket: function(websocketInfo) {
            var that = this;

            // Create a client instance
            var client = new Paho.MQTT.Client(websocketInfo.url, "clientId");

            // set callback handlers
            client.onConnectionLost = onConnectionLost;
            client.onMessageArrived = onMessageArrived;

            // connect the client
            client.connect({onSuccess:onConnect, keepAliveInterval: 15});

            // called when the client connects
            function onConnect() {
              // Once a connection has been made, make a subscription and send a message.
              console.log("onConnect");

              websocketInfo.topics.forEach(function(topic){
                  client.subscribe(topic);
              });
            }

            // called when the client loses its connection
            function onConnectionLost(responseObject) {
                // retry
                that.connectionRetryCount += 1;

              if (responseObject.errorCode !== 0) {
                console.log("onConnectionLost:"+responseObject.errorMessage);
              }

                console.log("retry counts: " + that.connectionRetryCount);

                if (that.connectionRetryCount < 5) {
                    console.log("trying to reconnect");
                    client.connect({onSuccess: onConnect});
                } else {
                    console.log("too many reconnect attempts");
                }
            }

            // called when a message arrives
            function onMessageArrived(message) {
                console.log("onMessageArrived:"+message.payloadString);

                // handle messages
                var messageObject = JSON.parse(message.payloadString);
                that.messages.push(messageObject);
            }    
        }
    }
});
