var express			= require('express');
var app				= express();
var server			= require('http').createServer(app);
var io 				= require('socket.io').listen(server);
var shortId 		= require('shortid');


app.set('port', process.env.PORT || 3000);

var Users = {};

var Games = {};

var clients	= [];

io.on('connection', function (socket) {//default event for client connect to server

    var currentUser;

    socket.on('USER_CONNECT', function (){
        console.log('Users Connected ');
    });

    socket.on('Signup', function (data){
        Users[data['name']] = data['password'];
        console.log(Users);
    });

    socket.on('Login', function (data){
        var name = data['name'];
        console.log(name);
        var p = Users[name];
        if(p!=undefined){
          socket.emit('LoginSucessful',{status: "True"} );
        }
    });

    socket.on('LOAD_ROOM', function(data){
      console.log(data);
      console.log(Games[data['room']]["participants"]);
      if(Games[data['room']]!=undefined){

        var name = data['name'];
        Games[data['room']]["participants"][name]={"Location": "Top", "AP":5};
        console.log(Games);
        socket.emit('LOAD_ROOM_SUCCESS',{status: "True"} );
      }
    });

    socket.on('CREATE_ROOM', function(data){

      var room_number = data['room'];
      var name = data['name'];
      Games[room_number] = {"participants":  {[name] :{"Location": "Top", "AP":5}} , "Owner": data['name']}//participants need to be changed to a list
      console.log(Games);
      socket.emit('CREATE_ROOM_SUCCESS',{status: "True"} );
    });

    socket.on('gameSetUp', function(data){
      console.log("gameSetUp");
      var room_number = data['room'];
      var level = data['level'];
      var numberOfPlayer = data['numberOfPlayer'];
      Games[room_number]["level"] = level;
      Games[room_number]["numberOfPlayer"] = numberOfPlayer;
      console.log(Games[room_number]);
      socket.emit('gameSetUp_SUCCESS',{status: "True"} );
    });

    socket.on('startGame', function(data){
      console.log("startGame");
      socket.emit('startGame_SUCCESS',Games);
    });

    socket.on('PLAY', function (data){
        currentUser = {
            name:data.name,
            id:shortId.generate(),
            position:data.position
        }

        clients.push(currentUser);
        socket.emit('PLAY',currentUser );
        socket.broadcast.emit('USER_CONNECTED',currentUser);

    });

    socket.on('disconnect', function (){//a default event

        socket.broadcast.emit('USER_DISCONNECTED',currentUser);
        for (var i = 0; i < clients.length; i++) {
            if (clients[i].name === currentUser.name && clients[i].id === currentUser.id) {

                console.log("User "+clients[i].name+" id: "+clients[i].id+" has disconnected");
                clients.splice(i,1);

            };
        };

    });

    socket.on('MOVE', function (data){

        // currentUser.name = data.name;
        // currentUser.id   = data.id;
        currentUser.position = data.position;

        socket.broadcast.emit('MOVE', currentUser);
        console.log(currentUser.name+" Move to "+currentUser.position);


    });

    socket.on('Location', function (data) {
        var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        console.log(Games[room_number]);
        socket.emit('LocationSetUp_SUCCESS',{status: "True"} );
    });


});


server.listen( app.get('port'), function (){
    console.log("------- server is running ------- on port 3000");
} );
