var express			= require('express');
var app				= express();
var server			= require('http').createServer(app);
var io 				= require('socket.io').listen(server);
var shortId 		= require('shortid');


app.set('port', process.env.PORT || 3000);

var Users = {};

var Games = {};

var clients	= [];

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

    socket.on('CREATE_ROOM', function(data){

      var room_number = data['room'];
      var name = data['name'];
      Games[room_number] = {"participants":  {[name] :{"Location": "0,0", "AP":4, "Role":0, "Driving":0, "Riding":0}} , "Owner": data['name'], "Turn": data['name'], "participants_in_order" : [name]}//participants need to be changed to a list
      console.log(Games);
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
        console.log(x);
        console.log(z);
        console.log(type);
        socket.broadcast.emit('TileUpdate_Success', {"x":x, "z":z, "type":type});
    });

    socket.on('UpdateWall',function(data){
        console.log("Updating wall");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var horizontal = data["horizontal"];
        console.log(x);
        console.log(z);
        console.log(type);
        console.log(horizontal);
        socket.broadcast.emit('WallUpdate_Success', {"x":x, "z":z, "type":type, "horizontal":horizontal});
    });

    socket.on('UpdateDoor',function(data){
        console.log("Updating door");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var toType = data["toType"];
        console.log(x);
        console.log(z);
        console.log(type);
        console.log(toType);
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
       console.log("ambbbbbbbbbbulance");
      var newx=data['newx'];
      var newz=data['newz'];
      var origx=data['origx'];
      var origz=data['origz'];
      var room=data['room'];
      var names=[];

      var participants=Games[room]["participants"];
      for(var n in participants){
        var riding=participants[n]["Riding"];
        if(riding=="1"){
          participants[n]["Location"]=newx+","+newz;
          names.push(n);
        }
      }
      socket.broadcast.emit('UpdateAmbulanceLocation_Success',{'newx':newx,'newz':newz,"names":names});
    });
      

    socket.on('AskForRide', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var x = parseInt(origx);
      var z = parseInt(origz);
      var room=data['room'];
      var name=data['name'];

      var targetNames=[];
      var ride=[];
      var i=0;
      var participants = Games[room]["participants"];
        // console.log(player[name]);
        for(var n in participants){
          var location = participants[n]["Location"];
          console.log(n + " is asked");
          var arrLocation = location.split(',');
          console.log(arrLocation);
          var intX = parseInt(arrLocation[0]);
          var intZ = parseInt(arrLocation[1]);
          console.log(intX);
          console.log(intZ);
          if((x-3<=intX && intX<=x+3) && (z-3<=intZ && intZ<=z+3)&&n!=name){
            console.log("ask for ride");
            targetNames[i]=n;
            i=i+1;
          }
          // if(participants[n]["Riding"]=="1"){
          //   if(participants[n]["Location"]==newx+ "," + newz){
          //     console.log("move with");
          //     ride[i]=n;
          //   }
            
          // }
        }
        console.log("sending");
       io.sockets.emit('AskForRide_Success',{"targetNames":targetNames});
    });

    socket.on('UpdateEngineLocation', function(data){
      // console.log("engineeee");
      var newx=data['newx'];
      var newz=data['newz'];
      var names=[];

      var participants=Games[room]["participants"];
      for(var n in participants){
        var riding=participants[n]["Riding"];
        if(riding=="2"){
          participants[n]["Location"]=newx+","+newz;
          names.push(n);
        }
      }
      socket.broadcast.emit('UpdateEngineLocation_Success',{'newx':newx,'newz':newz,"names":names});
      // socket.broadcast.emit('UpdateEngineLocation_Success',{'newx':newx,'newz':newz});

    });

    socket.on('StartRide',function(data){
      var name=data['name'];
      var room=data['room'];
      var type=data['type'];

      Games[room]['participants'][name]["Riding"]=type;
      io.sockets.emit('ConfirmRide', "true");
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

});


server.listen( app.get('port'), function (){
    console.log("------- server is running ------- on port 3000");
} );
