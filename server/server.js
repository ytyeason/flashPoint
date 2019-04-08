var express			= require('express');
var app				= express();
var server			= require('http').createServer(app);
var io 				= require('socket.io').listen(server);
var shortId 		= require('shortid');


app.set('port', process.env.PORT || 3000);

var Users = {};

var Games = {};

var clients	= [];

var Games_state = {};

function initialize_tile(room){//family version
    console.log("initializing tile");
    for (x = 0; x < 10; x++){
        for(z = 0; z < 8 ; z++){
            // Family fire setup:
            if (x == 2 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 3 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 2 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 3 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 4 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 5 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 4 && z == 3) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 6 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 6 && z == 1) room['tileMemo'].push({[[[x],[z]]]: 2});
            else if (x == 7 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 2});
            // parking spots
            else if (x == 7 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 8 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 5 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 4 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 3) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 0 && z == 6) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 1) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 9 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 2 && z == 0) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 1 && z == 0) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 4 && z == 0) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 5 && z == 0) room['tileMemo'].push({[[[x],[z]]]: 4});
            else room['tileMemo'].push({[[[x],[z]]]: 0});		// 2 -> code for Fire

        }
    }
}

function initialize_hDoor(room){
    console.log("initializing hDoor");
    room["hDoorMemo"].push({[[5,1]]:2});
    room["hDoorMemo"].push({[[3,3]]:0});
    room["hDoorMemo"].push({[[6,3]]:0});
    room["hDoorMemo"].push({[[5,4]]:0});
    room["hDoorMemo"].push({[[6,7]]:2});
    room["hDoorMemo"].push({[[3,5]]:0});
}

function initialize_vDoor(room){
    console.log("initializing vDoor");
    room["vDoorMemo"].push({[[1,3]]:3});
    room["vDoorMemo"].push({[[6,4]]:1});
    room["vDoorMemo"].push({[[9,3]]:3});
}

function initialize_hWall(room){
    console.log("initializing hwall");
    room['hWallMemo'].push({[[1,1]]: 0});
    room['hWallMemo'].push({[[2,1]]: 0});
    room['hWallMemo'].push({[[4,1]]: 0});
    room['hWallMemo'].push({[[3,1]]: 0});
    room['hWallMemo'].push({[[6,1]]: 0});
    room['hWallMemo'].push({[[7,1]]: 0});
    room['hWallMemo'].push({[[8,1]]: 0});
    room['hWallMemo'].push({[[1,3]]: 0});
    room['hWallMemo'].push({[[2,3]]: 0});
    room['hWallMemo'].push({[[4,3]]: 0});
    room['hWallMemo'].push({[[5,3]]: 0});
    room['hWallMemo'].push({[[4,4]]: 0});
    room['hWallMemo'].push({[[6,4]]: 0});
    room['hWallMemo'].push({[[7,4]]: 0});
    room['hWallMemo'].push({[[8,4]]: 0});

    room['hWallMemo'].push({[[1,5]]: 0});
    room['hWallMemo'].push({[[2,5]]: 0});
    room['hWallMemo'].push({[[4,6]]: 0});
    room['hWallMemo'].push({[[5,6]]: 0});
    room['hWallMemo'].push({[[1,7]]: 0});
    room['hWallMemo'].push({[[2,7]]: 0});
    room['hWallMemo'].push({[[3,7]]: 0});
    room['hWallMemo'].push({[[4,7]]: 0});
    room['hWallMemo'].push({[[5,7]]: 0});
    room['hWallMemo'].push({[[8,7]]: 0});
}

function initialize_vWall(room){
    console.log("initializing vwall");
    room['vWallMemo'].push({[[1,1]]: 1});
    room['vWallMemo'].push({[[1,2]]: 1});
    room['vWallMemo'].push({[[1,4]]: 1});
    room['vWallMemo'].push({[[1,5]]: 1});
    room['vWallMemo'].push({[[1,6]]: 1});
    room['vWallMemo'].push({[[4,1]]: 1});
    room['vWallMemo'].push({[[4,2]]: 1});
    room['vWallMemo'].push({[[4,4]]: 1});
    room['vWallMemo'].push({[[4,5]]: 1});
    room['vWallMemo'].push({[[4,6]]: 1});
    room['vWallMemo'].push({[[6,5]]: 1});
    room['vWallMemo'].push({[[6,6]]: 1});

    room['vWallMemo'].push({[[7,1]]: 1});
    room['vWallMemo'].push({[[7,2]]: 1});
    room['vWallMemo'].push({[[7,3]]: 1});
    room['vWallMemo'].push({[[9,1]]: 1});
    room['vWallMemo'].push({[[9,2]]: 1});
    room['vWallMemo'].push({[[9,4]]: 1});
    room['vWallMemo'].push({[[9,5]]: 1});
    room['vWallMemo'].push({[[9,6]]: 1});

}

// var selectRoles=[];

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
        Games[data['room']]["participants"][name]={"Location": "0,0", "AP":4};
        Games[data['room']]["participants_in_order"].push(name);
        console.log(Games);
        socket.emit('LOAD_ROOM_SUCCESS',{status: "True"} );
      }
    });

    socket.on('LOAD_GAME', function(data){
      console.log(data);

      var room_num = data["room"];
      var name = data["name"];
      var room = Games[room_num];
      if(room!=undefined){
          if(room['participants'][name]!= undefined){
              console.log("found participant's saved game!!!")
              var room_state = Games_state[room_num];
              console.log(room_state);
              socket.emit("LOAD_GAME_SUCCESS", {'room':Games, 'state': room_state});
          }else{
            console.log("Didn't found your name!")
            socket.emit("LOAD_GAME_SUCCESS", {'status':false});
          }
      }else{
        console.log("Didn't found your room!")
        socket.emit("LOAD_GAME_SUCCESS", {'status':false});
      }

    });

    socket.on('CREATE_ROOM', function(data){

      var room_number = data['room'];
      var name = data['name'];
      Games[room_number] = {"participants":  {[name] :{"Location": "0,0", "AP":4}} , "Owner": data['name'], "Turn": data['name'], "participants_in_order" : [name]}//participants need to be changed to a list

      Games_state[room_number] = {"hWallMemo":[], "vWallMemo":[], "tileMemo":[], "hDoorMemo":[], "vDoorMemo":[]};

      // var s = [1,2];
      // Games_state[room_number]['hWallMemo'].push({[s]: 0});
      // var s1 = [2,2];
      // Games_state[room_number]['hWallMemo'].push({[s1]: 0});
      //
      // var s = [10,20];
      // Games_state[room_number]['vWallMemo'].push({[s]: 0});
      // var s1 = [20,20];
      // Games_state[room_number]['vWallMemo'].push({[s1]: 0});

      initialize_hWall(Games_state[room_number]);
      initialize_vWall(Games_state[room_number]);
      initialize_tile(Games_state[room_number]);
      initialize_hDoor(Games_state[room_number]);
      initialize_vDoor(Games_state[room_number]);

      console.log(Games);
      console.log(Games_state);
      socket.emit('CREATE_ROOM_SUCCESS',{status: "True"} );
    });

    socket.on('gameSetUp', function(data){
      console.log("gameSetUp");
      var room_number = data['room'];
      var level = data['level'];
      var numberOfPlayer = data['numberOfPlayer'];
      var numberOfHazmat=data['numberOfHazmat'];
      var numberOfHotspot=data['numberOfHotspot'];
      Games[room_number]["level"] = level;
      Games[room_number]["numberOfPlayer"] = numberOfPlayer;
      Games[room_number]["numberOfHazmat"]=numberOfHazmat;
      Games[room_number]["numberOfHotspot"]=numberOfHotspot;
      Games[room_number]["selectedRoles"]=[];
      console.log(Games[room_number]+level);
      socket.emit('gameSetUp_SUCCESS',{"status": "True", "level":level} );
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
        var role=data['role'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]["Role"]=role;
        console.log(Games[room_number]);
        socket.emit('LocationSetUp_SUCCESS',{status: "True"} );

        io.sockets.emit('LocationUpdate_SUCCESS',Games);
    });

    socket.on('UpdateTile',function(data){
        console.log("Updating tile");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var room = data['room'];
        var location = x+','+z;
        console.log(x);
        console.log(z);
        console.log(type);

        console.log("updating tile state");
        var tileMemo = Games_state[room]['tileMemo'];
        tileMemo.forEach(w => {
            if(w[location]!= null){
              console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
              w[[x,z]] = parseInt(type);
            }
        });
        Games_state[room]['tileMemo'] = tileMemo;
        console.log(Games_state[room]['tileMemo']);

        socket.broadcast.emit('TileUpdate_Success', {"x":x, "z":z, "type":type});
    });

    socket.on('UpdateWall',function(data){
        console.log("Updating wall");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var horizontal = data["horizontal"];
        var room = data['room'];
        var location = x+','+z;
        // console.log(x);
        // console.log(z);
        // console.log(type);
        // console.log(horizontal);
        // console.log(room);
        // console.log(Games_state[room]);
        if(horizontal=='1'){//horizontal
            console.log("updating horizontal Games_state wall");
            var hWall_list = Games_state[room]['hWallMemo'];
            hWall_list.forEach(w => {
              if(w[location]!= null){
                  console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
                  w[[x,z]] = parseInt(type);
              }
            });
            Games_state[room]['hWallMemo'] = hWall_list;
            console.log(Games_state[room]['hWallMemo']);
        }else{
            console.log("updating vertical Games_state wall");
            var vWall_list = Games_state[room]['vWallMemo'];
            vWall_list.forEach(w => {
              if(w[location]!= null){
                  console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
                  w[[x,z]] = parseInt(type);
              }
            });
            Games_state[room]['vWallMemo'] = vWall_list;
            console.log(Games_state[room]['vWallMemo']);
        }
        socket.broadcast.emit('WallUpdate_Success', {"x":x, "z":z, "type":type, "horizontal":horizontal});
    });

    socket.on('UpdateDoor',function(data){
        console.log("Updating door");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var toType = data["toType"];
        var room = data['room'];
        var location = x+','+z;
        console.log(x);
        console.log(z);
        console.log(type);
        console.log(toType);

        if(type=='0' || type=="2"){//horizontal
            console.log("updating horizontal Games_state door");
            var hDoor_list = Games_state[room]['hDoorMemo'];
            hDoor_list.forEach(w => {
              if(w[location]!= null){
                  console.log("updating " + x + " " + z + " to type: "+ parseInt(toType));
                  w[[x,z]] = parseInt(toType);
              }
            });
            Games_state[room]['hDoorMemo'] = hDoor_list;
            console.log(Games_state[room]['hDoorMemo']);
        }else{
            console.log("updating vertical Games_state door");
            var vDoor_list = Games_state[room]['vDoorMemo'];
            vDoor_list.forEach(w => {
              if(w[location]!= null){
                  console.log("updating " + x + " " + z + " to type: "+ parseInt(toType));
                  w[[x,z]] = parseInt(toType);
              }
            });
            Games_state[room]['vDoorMemo'] = vDoor_list;
            console.log(Games_state[room]['vDoorMemo']);
        }

        socket.broadcast.emit('DoorUpdate_Success', {"x":x, "z":z, "type":type, "toType":toType});
    });

    socket.on('checkingTurn',function(data){
      var room_number = data['room'];
      var name = data['name'];

      //console.log(Games[room_number]);
      // console.log(Games[room_number]['Turn']);
      var turn_name = Games[room_number]['Turn'];
      if(turn_name.localeCompare(name)==0){
        socket.emit('checkingTurn_Success', {"status": "True"});
      }else{
        socket.emit('checkingTurn_Success', {"status": "False"});
      }

    });

    socket.on('changingTurn', function(data){
        var room_number = data['room'];
        var name = data['name'];

        console.log(Games[room_number]);
        // console.log(Games[room_number]['Turn']);
        var turn_name = Games[room_number]['Turn'];
        if(turn_name.localeCompare(name)==0){//name matches
            var participants_in_order = Games[room_number]["participants_in_order"];
            var index = participants_in_order.indexOf(turn_name);
            if(index == participants_in_order.length-1){
              index = 0;
            }else{
              index = index+1;
            }
            Games[room_number]['Turn'] = participants_in_order[index];
            console.log(Games[room_number]['Turn']);
            socket.emit("changingTurn_Success", {"Turn": Games[room_number]['Turn']});//change isMyTurn to False in frontend
            socket.broadcast.emit("isMyTurnUpdate", {"Turn": Games[room_number]['Turn']});
        }else{
            console.log("name doesn't match in changing turn")
            socket.emit('changingTurn_Success', {"status": "True"});//keep isMyTurn unchanged
        }
    });

    socket.on('sendChat', function(data){
      var name = data['name'];
      var chat = data['chat'];

      socket.broadcast.emit('sendChat_Success', {"name": name, "chat": chat});
    });

    socket.on('sendNotification',function(data){
      var name=data['name'];
      var text=data['text'];
      console.log(name+" "+text);
      io.sockets.emit('sendNotification_Success',{"name":name,"text":text});
    });

    socket.on('RevealPOI',function(data){
        var x = data['x'];
        var z = data['z'];

        console.log(x);
        console.log(z);

        socket.broadcast.emit('revealPOI_Success', {"x":x, "z":z});

    });

    socket.on('TreatV', function(data){
      var x=data['x'];
      var z=data['z'];

      socket.broadcast.emit('TreatV_Success',{'x':x,'z':z});
    });

    socket.on('UpdatePOILocation', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var newx=data['newx'];
      var newz=data['newz'];

      socket.broadcast.emit('UpdatePOILocation_Success',{'origx':origx,'origz':origz,'newx':newx,'newz':newz});
    });

    socket.on('UpdateAmbulanceLocation', function(data){
      // console.log("ambbbbbbbbbbulance");
      var newx=data['newx'];
      var newz=data['newz'];
      socket.broadcast.emit('UpdateAmbulanceLocation_Success',{'newx':newx,'newz':newz});
    });
    socket.on('UpdateEngineLocation', function(data){
      // console.log("engineeee");
      var newx=data['newx'];
      var newz=data['newz'];
      socket.broadcast.emit('UpdateAmbulanceLocation_Success',{'newx':newx,'newz':newz});
    });

    socket.on('UpdateTreatedLocation', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var newx=data['newx'];
      var newz=data['newz'];

      socket.broadcast.emit('UpdateTreatedLocation_Success',{'origx':origx,'origz':origz,'newx':newx,'newz':newz});
    });

    socket.on('SelectRole', function(data){
      var room_number=data['room'];
      var selectRoles=Games[room_number]['selectedRoles'];
      var role=data['role'];
      var result='true';
      console.log(role);
      // console.log(selectRoles);
      if(selectRoles.includes(role)||role==""){
        result='false';
      }else{
        result='true';
        selectRoles.push(role);
      }
      socket.emit('selectRole_SUCCESS',{'result':result,'role':role});

    });

    socket.on('RemoveH',function(data){
      var x = data['x'];
      var z = data['z'];

      console.log(x);
      console.log(z);

      socket.broadcast.emit('RemoveHazmat_Success', {"x":x, "z":z});

    });

    socket.on('UpdateHazmatLocation', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var newx=data['newx'];
      var newz=data['newz'];

      socket.broadcast.emit('UpdateHazmatLocation_Success',{'origx':origx,'origz':origz,'newx':newx,'newz':newz});
    });

    socket.on('StartDrive',function(data){
      var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];
        var drive=data['driving'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]['Driving']=drive;
        console.log(Games[room_number]);
        socket.broadcast.emit('LocationUpdate_SUCCESS',Games );
    });

    socket.on('StartCarryV',function(data){
      var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];
        var carryV=data['carryV'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]['Carrying']=carryV;
        console.log(Games[room_number]);
        socket.broadcast.emit('LocationUpdate_SUCCESS',Games );
    });

    socket.on('AddPOI',function(data){
      var x=data['x'];
      var z=data['z'];
      var type=data['type'];

      socket.broadcast.emit('AddPOI_Success',{'x':x,'z':z,'type':type});
    });

    socket.on('StopDrive',function(data){
      var name=data['name'];
      var room=data['room'];

      Games[room]["participants"][name]['Driving']=0;
      socket.broadcast.emit('LocationUpdate_SUCCESS',Games);

    });

    socket.on('changeRole', function(data){
      var room=data['room'];
      var name=data['name'];
      var role=data['role'];
      var old=data['oldRole'];
      Games[room]["selectedRoles"].splice(Games[room]["selectedRoles"].indexOf(old),1);
      Games[room]["selectedRoles"].push(role);

      Games[room]["participants"][name]['Role']=role;

      console.log("changing Role:");
      console.log(Games[room]);
      socket.broadcast.emit('LocationUpdate_SUCCESS',Games);
    });

    socket.on('savedGame',function(data){
        console.log(data);
        console.log("in saved game");


    });

});


server.listen( app.get('port'), function (){
    console.log("------- server is running ------- on port 3000");
} );
