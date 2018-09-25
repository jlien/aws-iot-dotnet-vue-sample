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
            ajaxRequest = true;
            
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
                     that.postResults = data;
                     that.ajaxRequest = false;
                    that.messages.push(data);
                    // $("#messagesList").append("<p>" + data.userGuid + ": " +
                       // data.content + "</p>");
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
        }
    }
});
