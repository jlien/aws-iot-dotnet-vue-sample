// Write your JavaScript code.


var app = new Vue({
      el: '#iotApp',
      data: {
        message: 'Hello IoT!',
        userGuid: "",
        content: "",
        messages: [
            { "userGuid": "Bot", "content": "Enter Messages Here" }
        ],
        postResults: [],
        ajaxRequest: false,
        websocketConnectionUrl: ""
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
                    that.messages.push(data);
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
                    that.websocketConnectionUrl = data.url;
                    $("#nameForm").hide();
                    $("#messageForm").show();
                },
                error: function() {
                    alert("Error fetching websocket connection info");
                }
            });
        },
        configSocket: function() {
            var that = this;

            // Create a client instance
            var client = new Paho.MQTT.Client(location.hostname, Number(location.port), "clientId");

            // set callback handlers
            client.onConnectionLost = onConnectionLost;
            client.onMessageArrived = onMessageArrived;

            // connect the client
            client.connect({onSuccess:onConnect});

            // called when the client connects
            function onConnect() {
              // Once a connection has been made, make a subscription and send a message.
              console.log("onConnect");

                // TODO: Connect to each topic
              client.subscribe("World");
              message = new Paho.MQTT.Message("Hello");
              message.destinationName = "World";
              client.send(message);
            }

            // called when the client loses its connection
            function onConnectionLost(responseObject) {
                // retry
              if (responseObject.errorCode !== 0) {
                console.log("onConnectionLost:"+responseObject.errorMessage);
              }
            }

            // called when a message arrives
            function onMessageArrived(message) {
                console.log("onMessageArrived:"+message.payloadString);
                // handle messages
                that.messages.push(message);
            }    
        }
    }
});
