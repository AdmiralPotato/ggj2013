//------START server connection init

//------START SignalR config
//$.connection.hub.logging = true;
if (isUsingRemoteServer) {
    jQuery.support.cors = true;
    $.connection.hub.url = serverPath + 'signalr';
}

var gameHub = $.connection.gameHub;
gameHub.client.handleUpdate = function (update) {
    if (client.loggingEnabled) {
        console.log(JSON.stringify(update));
    }
    setGameStateFromServer(update);
};

if (isUsingRemoteServer) {
    $.connection.hub.start({ jsonp: true, transport: 'longPolling', xdomain: true})
        .done(function () { gameHub.server.setShip(gameId, 0); })
        .fail(function () { alert("Could not Connect!"); });
} else {
    $.connection.hub.start()
        .done(function () { gameHub.server.setShip(gameId, 0); })
        .fail(function () { alert("Could not Connect!"); });
}

$(window).unload(function () {
    gameHub.server.disconnecting(gameId, 0);
});

gameHub.client.addMessage = function (sourceId, sourceName, message) {
    AppendChatMessage(sourceId, sourceName, message);
};


//$.connection.hub.start({ transport: 'longPolling', xdomain: true })
//	.done(function () { gameHub.server.setShip(705804, 0); })
//	.fail(function () { alert("Could not Connect!"); });
//------END SignalR config

//------END server connection init


//------START UI event binding
var sendMessage = function (messageName, messageValue, callback) {
    if (isUsingRemoteServer) {
        $.post(serverPath + currentGame + messageName, messageValue, callback);
    }
    else {
        $.post(messageName, messageValue, callback);
    }
};
var sendMessageClickHandler = function (event) {
    var element = event.currentTarget,
		command = element.id.split('-');
    sendMessage(command[0], command[1]);
};
$(document).on('click touchstart', '.sendMessage', sendMessageClickHandler);

var customEventHandlerMap = {
    shipSelectHandler: function() {
        sendMessage("SelectShip", "defaultShipNumber=" + $("#shipId").val() + "&name=" + $("#shipName").val());
    },
    SelectStationHandler: function(event){
        var station = event.currentTarget.id.split('-')[1];
        sendMessage("SelectStation", "station=" + station, function (data) {
            if (data.length > 0) {
                // fail case
                AppendChatMessage(0, "Computer", data);
            } else {
                // success case
                // set the divs to be hidden except for the selected one.
				$('.stationUI').removeClass('active');
				$('#' + station).addClass('active');
                // keep copy of station value
                client.station = station;
            }
        });
    },
	SetHeadingHandler: function(event) {
		sendMessage('SetHeading', 'amount=' + $("#desiredHeading").val());
	},
	LoadProjectileHandler: function(event){
		var split = event.currentTarget.id.split('-');
		sendMessage(split[0], split[1] + '&type=' + $("#projectile").val());
	},
	LaunchProjectileHandler: function(event){
		var split = event.currentTarget.id.split('-');
		sendMessage(split[0], split[1] + '&targetId=' + $("#desiredTarget").val() + "&type=" + $("#projectile").val());
	},
	FireBeamHandler: function(event){
		var split = event.currentTarget.id.split('-');
		sendMessage(split[0], split[1] + '&targetId=' + $("#desiredTarget").val() + "&type=" + $("#beam").val());
	}
};
var customEventHandler = function(event) {
    var id = event.currentTarget.id.split('-')[0];
    customEventHandlerMap[id + 'Handler'](event);
};
$(document).on('click touchstart', '.customEvent', customEventHandler);

//------END UI event binding