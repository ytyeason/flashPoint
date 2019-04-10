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

var poiM = {};

function initialize_tile(room){//family version
    console.log("initializing tile");
    for (x = 0; x < 10; x++){
        for(z = 0; z < 8 ; z++){
            // Family fire setup:
            // if (x == 2 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 3 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 2 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 3 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 4 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 5 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 4 && z == 3) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 6 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 6 && z == 1) room['tileMemo'].push({[[[x],[z]]]: 2});
            // else if (x == 7 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 2});
            // parking spots
            if (x == 7 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 8 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 5 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 4 && z == 7) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 3) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 0 && z == 5) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 0 && z == 6) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 2) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 1) room['tileMemo'].push({[[[x],[z]]]: 3});
            else if (x == 9 && z == 4) room['tileMemo'].push({[[[x],[z]]]: 4});
            else if (x == 9 && z == 3) room['tileMemo'].push({[[[x],[z]]]: 4});
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
    room["hDoorMemo"].push({[[3,1]]:0});
    room["hDoorMemo"].push({[[7,7]]:0});
    room["hDoorMemo"].push({[[4,3]]:0});
    room["hDoorMemo"].push({[[8,5]]:0});
}

function initialize_vDoor(room){
    console.log("initializing vDoor");
    room["vDoorMemo"].push({[[1,4]]:1});
    room["vDoorMemo"].push({[[9,3]]:1});
    room["vDoorMemo"].push({[[3,4]]:1});
    room["vDoorMemo"].push({[[4,6]]:1});
    room["vDoorMemo"].push({[[6,5]]:1});
    room["vDoorMemo"].push({[[6,1]]:1});
    room["vDoorMemo"].push({[[7,3]]:1});
    room["vDoorMemo"].push({[[8,1]]:1});
}

function initialize_r1_hDoor(room){
    console.log("initializing r1 hDoor");
    room["hDoorMemo"].push({[[5,1]]:0});
    room["hDoorMemo"].push({[[6,7]]:0});
    room["hDoorMemo"].push({[[2,4]]:0});
    room["hDoorMemo"].push({[[6,4]]:0});
}

function initialize_r1_vDoor(room){
    console.log("initializing r1 vDoor");
    room["vDoorMemo"].push({[[1,3]]:1});
    room["vDoorMemo"].push({[[9,3]]:1});

    room["vDoorMemo"].push({[[5,2]]:1});
    room["vDoorMemo"].push({[[8,5]]:1});
}

function initialize_r2_hDoor(room){
    console.log("initializing r2 hDoor");
    room["hDoorMemo"].push({[[4,1]]:0});
    room["hDoorMemo"].push({[[4,7]]:0});
    room["hDoorMemo"].push({[[4,4]]:0});
    room["hDoorMemo"].push({[[6,3]]:0});
}

function initialize_r2_vDoor(room){
    console.log("initializing r2 vDoor");
    room["vDoorMemo"].push({[[1,5]]:1});
    room["vDoorMemo"].push({[[9,5]]:1});

    room["vDoorMemo"].push({[[7,2]]:1});
}

function initialize_r3_hDoor(room){
    console.log("initializing r3 hDoor");
    room["hDoorMemo"].push({[[5,1]]:0});
    room["hDoorMemo"].push({[[3,3]]:0});
    room["hDoorMemo"].push({[[6,3]]:0});
    room["hDoorMemo"].push({[[5,4]]:0});
    room["hDoorMemo"].push({[[6,7]]:0});
    room["hDoorMemo"].push({[[3,5]]:0});
}

function initialize_r3_vDoor(room){
    console.log("initializing r3 vDoor");
    room["vDoorMemo"].push({[[1,3]]:1});
    room["vDoorMemo"].push({[[6,4]]:1});

    room["vDoorMemo"].push({[[9,3]]:1});
}

function initialize_r4_hDoor(room){
    console.log("initializing r4 hDoor");
    room["hDoorMemo"].push({[[5,1]]:0});
    room["hDoorMemo"].push({[[6,7]]:0});
    room["hDoorMemo"].push({[[4,4]]:0});
    room["hDoorMemo"].push({[[3,3]]:0});
}

function initialize_r4_vDoor(room){
    console.log("initializing r4 vDoor");
    room["vDoorMemo"].push({[[1,3]]:1});
    room["vDoorMemo"].push({[[9,3]]:1});

    room["vDoorMemo"].push({[[5,5]]:1});
    room["vDoorMemo"].push({[[7,2]]:1});
    room["vDoorMemo"].push({[[8,3]]:1});
}

function initialize_r5_hDoor(room){
    console.log("initializing r5 hDoor");
    room["hDoorMemo"].push({[[5,1]]:0});
    room["hDoorMemo"].push({[[6,7]]:0});
    room["hDoorMemo"].push({[[3,6]]:0});
    room["hDoorMemo"].push({[[3,4]]:0});

    room["hDoorMemo"].push({[[7,4]]:0});
    room["hDoorMemo"].push({[[8,4]]:0});
    room["hDoorMemo"].push({[[7,2]]:0});
}

function initialize_r5_vDoor(room){
    console.log("initializing r5 vDoor");
    room["vDoorMemo"].push({[[1,3]]:1});
    room["vDoorMemo"].push({[[9,3]]:1});

    room["vDoorMemo"].push({[[3,1]]:1});
    room["vDoorMemo"].push({[[6,2]]:1});
}

function initialize_hWall(room){
    console.log("initializing hwall");
    room['hWallMemo'].push({[[1,1]]: 0});
    room['hWallMemo'].push({[[2,1]]: 0});
    room['hWallMemo'].push({[[4,1]]: 0});
    room['hWallMemo'].push({[[5,1]]: 0});
    room['hWallMemo'].push({[[6,1]]: 0});
    room['hWallMemo'].push({[[7,1]]: 0});
    room['hWallMemo'].push({[[8,1]]: 0});

    room['hWallMemo'].push({[[1,7]]: 0});
    room['hWallMemo'].push({[[2,7]]: 0});
    room['hWallMemo'].push({[[3,7]]: 0});
    room['hWallMemo'].push({[[4,7]]: 0});
    room['hWallMemo'].push({[[5,7]]: 0});
    room['hWallMemo'].push({[[6,7]]: 0});
    room['hWallMemo'].push({[[8,7]]: 0});

    room['hWallMemo'].push({[[3,5]]: 0});
    room['hWallMemo'].push({[[4,5]]: 0});
    room['hWallMemo'].push({[[5,5]]: 0});
    room['hWallMemo'].push({[[6,5]]: 0});
    room['hWallMemo'].push({[[7,5]]: 0});
    room['hWallMemo'].push({[[1,3]]: 0});
    room['hWallMemo'].push({[[2,3]]: 0});
    room['hWallMemo'].push({[[3,3]]: 0});
    room['hWallMemo'].push({[[5,3]]: 0});
    room['hWallMemo'].push({[[6,3]]: 0});
    room['hWallMemo'].push({[[7,3]]: 0});
    room['hWallMemo'].push({[[8,3]]: 0});
}

function initialize_vWall(room){
    console.log("initializing vwall");
    room['vWallMemo'].push({[[1,1]]: 1});
    room['vWallMemo'].push({[[1,2]]: 1});
    room['vWallMemo'].push({[[1,3]]: 1});
    room['vWallMemo'].push({[[1,5]]: 1});
    room['vWallMemo'].push({[[1,6]]: 1});

    room['vWallMemo'].push({[[9,1]]: 1});
    room['vWallMemo'].push({[[9,2]]: 1});
    room['vWallMemo'].push({[[9,4]]: 1});
    room['vWallMemo'].push({[[9,5]]: 1});
    room['vWallMemo'].push({[[9,6]]: 1});

    room['vWallMemo'].push({[[3,3]]: 1});
    room['vWallMemo'].push({[[4,5]]: 1});
    room['vWallMemo'].push({[[6,6]]: 1});
    room['vWallMemo'].push({[[7,4]]: 1});
    room['vWallMemo'].push({[[6,2]]: 1});
    room['vWallMemo'].push({[[8,2]]: 1});

}

function initialize_r1_hWall(room){
  console.log("initializing random 1 hwall");
  room['hWallMemo'].push({[[1,1]]: 0});
  room['hWallMemo'].push({[[2,1]]: 0});
  room['hWallMemo'].push({[[4,1]]: 0});
  room['hWallMemo'].push({[[3,1]]: 0});
  room['hWallMemo'].push({[[6,1]]: 0});
  room['hWallMemo'].push({[[7,1]]: 0});
  room['hWallMemo'].push({[[8,1]]: 0});

  room['hWallMemo'].push({[[1,7]]: 0});
  room['hWallMemo'].push({[[2,7]]: 0});
  room['hWallMemo'].push({[[3,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[5,7]]: 0});
  room['hWallMemo'].push({[[6,7]]: 0});
  room['hWallMemo'].push({[[8,7]]: 0});

  room['hWallMemo'].push({[[1,4]]: 0});
  room['hWallMemo'].push({[[1,3]]: 0});
  room['hWallMemo'].push({[[2,3]]: 0});
  room['hWallMemo'].push({[[3,4]]: 0});
  room['hWallMemo'].push({[[3,3]]: 0});
  room['hWallMemo'].push({[[4,4]]: 0});
  room['hWallMemo'].push({[[4,3]]: 0});
  room['hWallMemo'].push({[[5,4]]: 0});
  room['hWallMemo'].push({[[6,3]]: 0});
  room['hWallMemo'].push({[[7,3]]: 0});
}

function initialize_r1_vWall(room){
  console.log("initializing r1 vwall");
  room['vWallMemo'].push({[[1,1]]: 1});
  room['vWallMemo'].push({[[1,2]]: 1});
  room['vWallMemo'].push({[[1,4]]: 1});
  room['vWallMemo'].push({[[1,5]]: 1});
  room['vWallMemo'].push({[[1,6]]: 1});

  room['vWallMemo'].push({[[9,1]]: 1});
  room['vWallMemo'].push({[[9,2]]: 1});
  room['vWallMemo'].push({[[9,4]]: 1});
  room['vWallMemo'].push({[[9,5]]: 1});
  room['vWallMemo'].push({[[9,6]]: 1});

  room['vWallMemo'].push({[[4,6]]: 1});
  room['vWallMemo'].push({[[5,6]]: 1});
  room['vWallMemo'].push({[[5,5]]: 1});
  room['vWallMemo'].push({[[5,4]]: 1});
  room['vWallMemo'].push({[[5,1]]: 1});
  room['vWallMemo'].push({[[6,2]]: 1});
  room['vWallMemo'].push({[[6,1]]: 1});
  room['vWallMemo'].push({[[7,6]]: 1});
  room['vWallMemo'].push({[[7,5]]: 1});
  room['vWallMemo'].push({[[7,4]]: 1});
  room['vWallMemo'].push({[[8,6]]: 1});
  room['vWallMemo'].push({[[8,4]]: 1});
  room['vWallMemo'].push({[[8,3]]: 1});
}

function initialize_r2_hWall(room){
  console.log("initializing random 2 hwall");
  room['hWallMemo'].push({[[1,1]]: 0});
  room['hWallMemo'].push({[[2,1]]: 0});
  room['hWallMemo'].push({[[4,1]]: 0});
  room['hWallMemo'].push({[[3,1]]: 0});
  room['hWallMemo'].push({[[6,1]]: 0});
  room['hWallMemo'].push({[[7,1]]: 0});
  room['hWallMemo'].push({[[8,1]]: 0});

  room['hWallMemo'].push({[[1,7]]: 0});
  room['hWallMemo'].push({[[2,7]]: 0});
  room['hWallMemo'].push({[[3,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[5,7]]: 0});
  room['hWallMemo'].push({[[6,7]]: 0});
  room['hWallMemo'].push({[[8,7]]: 0});

  room['hWallMemo'].push({[[1,5]]: 0});
  room['hWallMemo'].push({[[2,5]]: 0});
  room['hWallMemo'].push({[[3,5]]: 0});
  room['hWallMemo'].push({[[5,5]]: 0});
  room['hWallMemo'].push({[[7,5]]: 0});
  room['hWallMemo'].push({[[8,5]]: 0});
  room['hWallMemo'].push({[[5,4]]: 0});
  room['hWallMemo'].push({[[7,4]]: 0});
  room['hWallMemo'].push({[[5,3]]: 0});
  room['hWallMemo'].push({[[7,3]]: 0});
  room['hWallMemo'].push({[[6,2]]: 0});
}

function initialize_r2_vWall(room){
  console.log("initializing r2 vwall");
  room['vWallMemo'].push({[[1,1]]: 1});
  room['vWallMemo'].push({[[1,2]]: 1});
  room['vWallMemo'].push({[[1,4]]: 1});
  room['vWallMemo'].push({[[1,5]]: 1});
  room['vWallMemo'].push({[[1,6]]: 1});

  room['vWallMemo'].push({[[9,1]]: 1});
  room['vWallMemo'].push({[[9,2]]: 1});
  room['vWallMemo'].push({[[9,4]]: 1});
  room['vWallMemo'].push({[[9,5]]: 1});
  room['vWallMemo'].push({[[9,6]]: 1});

  room['vWallMemo'].push({[[5,6]]: 1});
  room['vWallMemo'].push({[[5,5]]: 1});
  room['vWallMemo'].push({[[6,4]]: 1});
  room['vWallMemo'].push({[[7,4]]: 1});
  room['vWallMemo'].push({[[4,3]]: 1});
  room['vWallMemo'].push({[[5,3]]: 1});
  room['vWallMemo'].push({[[8,3]]: 1});
  room['vWallMemo'].push({[[4,2]]: 1});
  room['vWallMemo'].push({[[4,1]]: 1});
  room['vWallMemo'].push({[[6,1]]: 1});
  room['vWallMemo'].push({[[8,1]]: 1});
}

function initialize_r3_hWall(room){
  console.log("initializing random 3 hwall");
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
  room['hWallMemo'].push({[[7,7]]: 0});
  room['hWallMemo'].push({[[8,7]]: 0});
}

function initialize_r3_vWall(room){
  console.log("initializing r3 vwall");
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

function initialize_r4_hWall(room){
  console.log("initializing random 4 hwall");
  room['hWallMemo'].push({[[1,1]]: 0});
  room['hWallMemo'].push({[[2,1]]: 0});
  room['hWallMemo'].push({[[4,1]]: 0});
  room['hWallMemo'].push({[[3,1]]: 0});
  room['hWallMemo'].push({[[6,1]]: 0});
  room['hWallMemo'].push({[[7,1]]: 0});
  room['hWallMemo'].push({[[8,1]]: 0});

  room['hWallMemo'].push({[[1,7]]: 0});
  room['hWallMemo'].push({[[2,7]]: 0});
  room['hWallMemo'].push({[[3,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[5,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[7,7]]: 0});
  room['hWallMemo'].push({[[8,7]]: 0});

  room['hWallMemo'].push({[[5,6]]: 0});
  room['hWallMemo'].push({[[7,6]]: 0});
  room['hWallMemo'].push({[[1,5]]: 0});
  room['hWallMemo'].push({[[2,4]]: 0});
  room['hWallMemo'].push({[[3,4]]: 0});
  room['hWallMemo'].push({[[5,4]]: 0});
  room['hWallMemo'].push({[[6,4]]: 0});
  room['hWallMemo'].push({[[7,4]]: 0});
  room['hWallMemo'].push({[[7,3]]: 0});
  room['hWallMemo'].push({[[8,3]]: 0});
  room['hWallMemo'].push({[[4,2]]: 0});
  room['hWallMemo'].push({[[6,2]]: 0});
}

function initialize_r4_vWall(room){
  console.log("initializing r4 vwall");
  room['vWallMemo'].push({[[1,1]]: 1});
  room['vWallMemo'].push({[[1,2]]: 1});
  room['vWallMemo'].push({[[1,4]]: 1});
  room['vWallMemo'].push({[[1,5]]: 1});
  room['vWallMemo'].push({[[1,6]]: 1});

  room['vWallMemo'].push({[[9,1]]: 1});
  room['vWallMemo'].push({[[9,2]]: 1});
  room['vWallMemo'].push({[[9,4]]: 1});
  room['vWallMemo'].push({[[9,5]]: 1});
  room['vWallMemo'].push({[[9,6]]: 1});

  room['vWallMemo'].push({[[2,4]]: 1});
  room['vWallMemo'].push({[[3,3]]: 1});

  room['vWallMemo'].push({[[4,2]]: 1});
  room['vWallMemo'].push({[[5,4]]: 1});
  room['vWallMemo'].push({[[5,1]]: 1});

  room['vWallMemo'].push({[[6,6]]: 1});
  room['vWallMemo'].push({[[6,1]]: 1});

  room['vWallMemo'].push({[[7,6]]: 1});
  room['vWallMemo'].push({[[8,5]]: 1});
  room['vWallMemo'].push({[[8,4]]: 1});
}

function initialize_r5_hWall(room){
  console.log("initializing random 5 hwall");
  room['hWallMemo'].push({[[1,1]]: 0});
  room['hWallMemo'].push({[[2,1]]: 0});
  room['hWallMemo'].push({[[4,1]]: 0});
  room['hWallMemo'].push({[[3,1]]: 0});
  room['hWallMemo'].push({[[6,1]]: 0});
  room['hWallMemo'].push({[[7,1]]: 0});
  room['hWallMemo'].push({[[8,1]]: 0});

  room['hWallMemo'].push({[[1,7]]: 0});
  room['hWallMemo'].push({[[2,7]]: 0});
  room['hWallMemo'].push({[[3,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[5,7]]: 0});
  room['hWallMemo'].push({[[4,7]]: 0});
  room['hWallMemo'].push({[[7,7]]: 0});
  room['hWallMemo'].push({[[8,7]]: 0});

  room['hWallMemo'].push({[[2,6]]: 0});
  room['hWallMemo'].push({[[1,4]]: 0});
  room['hWallMemo'].push({[[2,4]]: 0});
  room['hWallMemo'].push({[[4,4]]: 0});

  room['hWallMemo'].push({[[5,4]]: 0});
  room['hWallMemo'].push({[[6,4]]: 0});
  room['hWallMemo'].push({[[8,2]]: 0});
}

function initialize_r5_vWall(room){
  console.log("initializing r5 vwall");
  room['vWallMemo'].push({[[1,1]]: 1});
  room['vWallMemo'].push({[[1,2]]: 1});
  room['vWallMemo'].push({[[1,4]]: 1});
  room['vWallMemo'].push({[[1,5]]: 1});
  room['vWallMemo'].push({[[1,6]]: 1});

  room['vWallMemo'].push({[[9,1]]: 1});
  room['vWallMemo'].push({[[9,2]]: 1});
  room['vWallMemo'].push({[[9,4]]: 1});
  room['vWallMemo'].push({[[9,5]]: 1});
  room['vWallMemo'].push({[[9,6]]: 1});

  room['vWallMemo'].push({[[2,6]]: 1});
  room['vWallMemo'].push({[[3,3]]: 1});

  room['vWallMemo'].push({[[3,2]]: 1});
  room['vWallMemo'].push({[[5,6]]: 1});
  room['vWallMemo'].push({[[5,5]]: 1});

  room['vWallMemo'].push({[[5,4]]: 1});
  room['vWallMemo'].push({[[6,3]]: 1});

  room['vWallMemo'].push({[[6,1]]: 1});
  room['vWallMemo'].push({[[7,1]]: 1});
  room['vWallMemo'].push({[[8,6]]: 1});
  room['vWallMemo'].push({[[8,4]]: 1});
  room['vWallMemo'].push({[[8,5]]: 1});
}

function addPOI(room,x,z,type){
    console.log("adding in POI");
    room['POIMemo'].push({[[[x],[z]]]: type});
}

function addMovingPOIMemo(room, x, z, type){
    console.log("adding in moving POI");
    room['movingPOIMemo'].push({[[[x],[z]]]: type});
}

function addTreatedPOIMemo(room, x, z, type){
    console.log("adding in moving POI");
    room['treatedPOIMemo'].push({[[[x],[z]]]: type});
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
      var det="true";
      if(Games[data['room']]!=undefined){
        var name = data['name'];
        if(Games[data['room']]["participants"][name]==undefined){
          Games[data['room']]["participants"][name]={"Location": "0,0", "AP":4, "Role":"10","Driving":"0", "Riding":"0","Carrying":"false","Leading":"false"};
          Games[data['room']]["participants_in_order"].push(name);
          det="true";
        }else{
          det="false";
        }
        console.log(Games);
        socket.emit('LOAD_ROOM_SUCCESS',{"status": det, "level":Games[data['room']]["level"]} );
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
              var numberOfHazmat = Games[data['room']]["numberOfHazmat"];
              var numberOfPlayer = Games[data['room']]["numberOfPlayer"]
              var level = Games[data['room']]["level"];
              var numberOfHotspot = Games[data['room']]["numberOfHotspot"];
              var selectedRoles = Games[data['room']]["selectedRoles"];
              var confirmedPosition = Games[data['room']]["confirmedPosition"];
              //var joinedPlayers = Games[data['room']]["joinedPlayers"];

              socket.emit("LOAD_GAME_SUCCESS",
              {'room':Games, 'state': room_state, 'name':name, 'roomNumber':room_num, 'level':level,
              'numberOfPlayer':numberOfPlayer, "numberOfHazmat":numberOfHazmat,
              "numberOfHotspot":numberOfHotspot, "selectedRoles":selectedRoles,
              "confirmedPosition":confirmedPosition});
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
      console.log("craeting room");
      console.log(data);
      var room_number = data['room'];
      var name = data['name'];
      var level = data['level'];
      console.log(level);

      Games[room_number] = {"participants":  {[name] :{"Location": "0,0", "AP":4, "Role":"10", "Driving":"0", "Riding":"0","Carrying":"False","Leading":"False"}} , "Owner": data['name'], "Turn": data['name'], "participants_in_order" : [name]}//participants need to be changed to a list

      Games_state[room_number] = {"hWallMemo":[], "vWallMemo":[], "tileMemo":[], "hDoorMemo":[], "vDoorMemo":[], "POIMemo":[],"movingPOIMemo":[], "treatedPOIMemo":[]};

      // var s = [1,2];
      // Games_state[room_number]['hWallMemo'].push({[s]: 0});
      // var s1 = [2,2];
      // Games_state[room_number]['hWallMemo'].push({[s1]: 0});
      //
      // var s = [10,20];
      // Games_state[room_number]['vWallMemo'].push({[s]: 0});
      // var s1 = [20,20];
      // Games_state[room_number]['vWallMemo'].push({[s1]: 0});


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
      var random = data['random'];
      Games[room_number]["level"] = level;
      Games[room_number]["numberOfPlayer"] = numberOfPlayer;
      Games[room_number]["numberOfHazmat"]=numberOfHazmat;
      Games[room_number]["numberOfHotspot"]=numberOfHotspot;
      Games[room_number]["selectedRoles"]=[];
      Games[room_number]["confirmedPosition"]=[];
      Games[room_number]["joinedPlayers"]=[];
      Games[room_number]["random"]=random;
      console.log(Games[room_number]);

      initialize_tile(Games_state[room_number]);
      if(level!=="Random"){
        initialize_hWall(Games_state[room_number]);
        initialize_vWall(Games_state[room_number]);

        initialize_hDoor(Games_state[room_number]);
        initialize_vDoor(Games_state[room_number]);
      }else{
        if(random==="1"){
          initialize_r1_hWall(Games_state[room_number]);
          initialize_r1_vWall(Games_state[room_number]);

          initialize_r1_hDoor(Games_state[room_number]);
          initialize_r1_vDoor(Games_state[room_number]);
        }else if(random==="2"){
          initialize_r2_hWall(Games_state[room_number]);
          initialize_r2_vWall(Games_state[room_number]);

          initialize_r2_hDoor(Games_state[room_number]);
          initialize_r2_vDoor(Games_state[room_number]);
        }else if(random==="3"){
          initialize_r3_hWall(Games_state[room_number]);
          initialize_r3_vWall(Games_state[room_number]);

          initialize_r3_hDoor(Games_state[room_number]);
          initialize_r3_vDoor(Games_state[room_number]);
        }else if(random==="4"){
          initialize_r4_hWall(Games_state[room_number]);
          initialize_r4_vWall(Games_state[room_number]);

          initialize_r4_hDoor(Games_state[room_number]);
          initialize_r4_vDoor(Games_state[room_number]);
        }else if(random==="5"){
          initialize_r5_hWall(Games_state[room_number]);
          initialize_r5_vWall(Games_state[room_number]);

          initialize_r5_hDoor(Games_state[room_number]);
          initialize_r5_vDoor(Games_state[room_number]);
        }
      }


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
        // console.log(Games[room_number]);
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
        socket.broadcast.emit('TileUpdate_Success', {"x":x, "z":z, "type":type});
        console.log(x);
        console.log(z);
        console.log(type);

        // console.log("updating tile state");
        var tileMemo = Games_state[room]['tileMemo'];
        var found = false;
        tileMemo.forEach(w => {
            if(w[location]!= null){
              // console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
              found = true;
              w[[x,z]] = parseInt(type);
            }
        });
        if(found == false){//if adding a new tile
          tileMemo.push({[[[x],[z]]]: parseInt(type)});
        }
        Games_state[room]['tileMemo'] = tileMemo;
        // console.log(Games_state[room]['tileMemo']);


    });

    socket.on('UpdateWall',function(data){
        console.log("Updating wall");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var horizontal = data["horizontal"];
        var room = data['room'];
        var location = x+','+z;
        var fromExplosion=data["fromExplosion"];
        socket.broadcast.emit('WallUpdate_Success', {"x":x, "z":z, "type":type, "horizontal":horizontal,"fromExplosion":fromExplosion});
        console.log("update wall");
        console.log(x);
        console.log(z);
        console.log(type);
        console.log(horizontal);
        // console.log(room);
        // console.log(Games_state[room]);
        if(horizontal=='1'){//horizontal
            console.log("updating horizontal Games_state wall");
            var hWall_list = Games_state[room]['hWallMemo'];
            hWall_list.forEach(w => {
              if(w[location]!= null){
                  // console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
                  w[[x,z]] = parseInt(type);
              }
            });
            Games_state[room]['hWallMemo'] = hWall_list;
            // console.log(Games_state[room]['hWallMemo']);
        }else{
            // console.log("updating vertical Games_state wall");
            var vWall_list = Games_state[room]['vWallMemo'];
            vWall_list.forEach(w => {
              if(w[location]!= null){
                  // console.log("updating " + x + " " + z + " to type: "+ parseInt(type));
                  w[[x,z]] = parseInt(type);
              }
            });
            Games_state[room]['vWallMemo'] = vWall_list;
            // console.log(Games_state[room]['vWallMemo']);
        }

    });

    socket.on('UpdateDoor',function(data){
        console.log("Updating door");
        var x = data['x'];
        var z = data['z'];
        var type = data['type'];
        var toType = data["toType"];
        var room = data['room'];
        var location = x+','+z;
        var fromExplosion=data['fromExplosion'];
        socket.broadcast.emit('DoorUpdate_Success', {"x":x, "z":z, "type":type, "toType":toType, "fromExplosion":fromExplosion});
        console.log("update door");
        console.log(x);
        console.log(z);
        console.log(type);
        console.log(toType);

        if(type=='0' || type=="2"){//horizontal
            console.log("updating horizontal Games_state door");
            var hDoor_list = Games_state[room]['hDoorMemo'];
            hDoor_list.forEach(w => {
              if(w[location]!= null){
                  // console.log("updating " + x + " " + z + " to type: "+ parseInt(toType));
                  w[[x,z]] = parseInt(toType);
              }
            });
            Games_state[room]['hDoorMemo'] = hDoor_list;
            // console.log(Games_state[room]['hDoorMemo']);
        }else{
            // console.log("updating vertical Games_state door");
            var vDoor_list = Games_state[room]['vDoorMemo'];
            vDoor_list.forEach(w => {
              if(w[location]!= null){
                  // console.log("updating " + x + " " + z + " to type: "+ parseInt(toType));
                  w[[x,z]] = parseInt(toType);
              }
            });
            Games_state[room]['vDoorMemo'] = vDoor_list;
            // console.log(Games_state[room]['vDoorMemo']);
        }


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

    socket.on('checkingOwner',function(data){
      var room_number = data['room'];
      var name = data['name'];

      var owner_name = Games[room_number]['Owner'];
      if(owner_name.localeCompare(name)==0){
        socket.emit('checkOwner_Success', {"owner": "True"});
      }else{
        socket.emit('checkOwner_Success', {"owner": "False"});
      }

    });

    socket.on('changingTurn', function(data){
        var room_number = data['room'];
        var name = data['name'];
        console.log(room_number);
        console.log(Games[room_number]);
        // console.log(Games[room_number]['Turn']);
        var turn_name = Games[room_number]['Turn'];
        console.log("turn: "+turn_name);
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
        var room_number = data['room'];
        var location = x+","+z;

        console.log("revealing poi");
        console.log(x);
        console.log(z);

        //deleting poi from POIMemo if type is 1(false alarm)
        var p = Games_state[room_number]['POIMemo'];
        var i = p.findIndex(x => x[location]==1);
        if (i !== -1) {
          p.splice(i, 1);
          Games_state[room_number]['POIMemo'] = p;
          console.log("deleting false alarm from poiMemo");
          console.log(Games_state[room_number]['POIMemo']);
        }

        socket.broadcast.emit('revealPOI_Success', {"x":x, "z":z});

    });

    socket.on('TreatV', function(data){
        console.log("treat: moving -> treated");
        var x=data['x'];
        var z=data['z'];
        var room_number = data["room"];

        var location = x+','+z;

        //deleting poi from POIMemo
        var p = Games_state[room_number]['movingPOIMemo'];
        var i = p.findIndex(x => x[location]!= null);
        var type = p[i][location];
        if (i !== -1) p.splice(i, 1);
        Games_state[room_number]['movingPOIMemo'] = p;
        console.log("deleting poi with location: " +location+ " and type "+type);

        addTreatedPOIMemo(Games_state[room_number],x,z,type);

      socket.broadcast.emit('TreatV_Success',{'x':x,'z':z});
    });

    socket.on('UpdatePOILocation', function(data){
      console.log("in UpdatePOILocation");
      var origx=data['origx'];
      var origz=data['origz'];
      var newx=data['newx'];
      var newz=data['newz'];
      var room_number = data['room'];

      var location = origx+','+origz;
      console.log(room_number);
      console.log(Games_state[room_number]);

      //updating poi from POIMemo
      var p = Games_state[room_number]['movingPOIMemo'];
      console.log("old poi moving Memo:");
      console.log(p);
      var i = p.findIndex(x => x[location]!= null);

      if (i !== -1) {
          console.log("movingPOI found")
          var type = p[i][location];
          p.splice(i, 1);
          p.push({[[[newx],[newz]]]: type});
          Games_state[room_number]['movingPOIMemo'] = p;
          console.log("new poi moving memo");
          console.log(p);
      }

      socket.broadcast.emit('UpdatePOILocation_Success',{'origx':origx,'origz':origz,'newx':newx,'newz':newz});
    });

    socket.on('UpdateAmbulanceLocation', function(data){
       console.log("ambbbbbbbbbbulance");
      var newx=data['newx'];
      var newz=data['newz'];
      var room=data['room'];
      var driver=data['name'];
      var names=[];

      var participants=Games[room]["participants"];
      for(var n in participants){
        var riding=participants[n]["Riding"];
        if(riding=="1"){
          participants[n]["Location"]=newx+","+newz;
          names.push(n);
        }
      }
      names.push(driver);
      io.sockets.emit('UpdateAmbulanceLocation_Success',{'newx':newx,'newz':newz,"names":names});
    });

    socket.on('UpdateEngineLocation', function(data){
      // console.log("engineeee");
      var newx=data['newx'];
      var newz=data['newz'];
      var names=[];
      var driver=data['name'];
      var room = data['room'];

      var participants=Games[room]["participants"];
      for(var n in participants){
        var riding=participants[n]["Riding"];
        if(riding=="2"){
          participants[n]["Location"]=newx+","+newz;
          names.push(n);
        }
      }
      names.push(driver);
      io.sockets.emit('UpdateEngineLocation_Success',{'newx':newx,'newz':newz,"names":names});
      // socket.broadcast.emit('UpdateEngineLocation_Success',{'newx':newx,'newz':newz});

    });


    socket.on('AskForRide', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var x = parseInt(origx);
      var z = parseInt(origz);
      var room=data['room'];
      var name=data['name'];
      // var type = 0;

      var targetNames=[];
      var ride=[];
      var i=0;
      var participants = Games[room]["participants"];
        // console.log(player[name]);
        for(var n in participants){
          var location = participants[n]["Location"];
          var arrLocation = location.split(',');
          var intX = parseInt(arrLocation[0]);
          var intZ = parseInt(arrLocation[1]);
          console.log("Im at server.js, the intX and intZ are :" + intX + " " + intZ + " " + n + " is asked" + " " + "arrLocation is" + arrLocation);
          if((x-3<=intX && intX<=x+3) && (z-3<=intZ && intZ<=z+3)&&n!=name&&participants[n]["Riding"]!="1"){
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
        // console.log("sending");
        io.sockets.emit('AskForRide_Success',{"targetNames":targetNames, "driver": name, "nRider": i});

    });



    socket.on('StartRide',function(data){
      var name=data['name'];
      var room=data['room'];
      var type=data['type'];

      Games[room]['participants'][name]["Riding"]=type;
      io.sockets.emit('ConfirmRide', {'type': type});
      console.log("im at startride in server.js" + " player" + name + " 's riding type is " + type);
    });

    socket.on('ResetConfirmed', function(data){
      var room = data['room'];
      var participants = Games[room]["participants"];
        // console.log(player[name]);
        for(var n in participants){
          participants[n]["Riding"]=0;
          participants[n]["Driving"]=0;
        }
      io.sockets.emit('RescueTreated_Success', true);
    });

    socket.on('UpdateTreatedLocation', function(data){
      var origx=data['origx'];
      var origz=data['origz'];
      var newx=data['newx'];
      var newz=data['newz'];
      var room_number = data['room'];

      var location = origx+','+origz;
      console.log(room_number);
      console.log(Games_state[room_number]);

      //updating poi from treatedPOIMemo
      var p = Games_state[room_number]['treatedPOIMemo'];
      console.log("old poi moving Memo:");
      console.log(p);
      var i = p.findIndex(x => x[location]!= null);

      if (i !== -1) {
          console.log("treatedPOIMemo found")
          var type = p[i][location];
          p.splice(i, 1);
          p.push({[[[newx],[newz]]]: type});
          Games_state[room_number]['treatedPOIMemo'] = p;
          console.log("new poi treatedPOI memo");
          console.log(p);
      }

      socket.broadcast.emit('UpdateTreatedLocation_Success',{'origx':origx,'origz':origz,'newx':newx,'newz':newz});
    });

    socket.on('SelectRole', function(data){
      var room_number=data['room'];
      var selectRoles=Games[room_number]['selectedRoles'];
      var role=data['role'];
      var result='true';
      var name=data['name'];
      console.log(role);
      // console.log(selectRoles);
      if(selectRoles.includes(role)||role==""){
        result='false';
      }else{
        result='true';
        selectRoles.push(role);
        Games[room_number]["participants"][name]["Role"]=role;
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
        // console.log(Games[room_number]);
        socket.broadcast.emit('LocationUpdate_SUCCESS',Games );
    });

    socket.on('StartCarryV',function(data){//delete from POIMemo and add in movingPOIMemo
        console.log("in StartCarryV: placed -> moving");
        var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];
        var carryV=data['carryV'];
        var x=data['x'];
        var z=data['z'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]['Carrying']=carryV;

        var location = x+','+z;

        //deleting poi from POIMemo
        var p = Games_state[room_number]['POIMemo'];
        var i = p.findIndex(x => x[location]!= null);
        var type = p[i][location];
        if (i !== -1) p.splice(i, 1);
        Games_state[room_number]['POIMemo'] = p;
        console.log("deleting poi with location: " +location+ " and type "+type);

        addMovingPOIMemo(Games_state[room_number],x,z,type);

        console.log(Games[room_number]);
        console.log(Games_state[room_number]['POIMemo']);
        console.log(Games_state[room_number]['movingPOIMemo']);
        socket.broadcast.emit('StartCarryV_Success', {"Games":Games, "x":x, "z":z} );
    });

    socket.on('StartLeadV',function(data){
        console.log("load: treated -> moving")
        var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];
        var carryV=data['carryV'];
        var x=data['x'];
        var z=data['z'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]['Leading']=carryV;

        var location = x+','+z;

        //deleting poi from POIMemo
        var p = Games_state[room_number]['treatedPOIMemo'];
        var i = p.findIndex(x => x[location]!= null);
        var type = p[i][location];
        if (i !== -1) p.splice(i, 1);
        Games_state[room_number]['treatedPOIMemo'] = p;
        console.log("deleting poi with location: " +location+ " and type "+type);

        addMovingPOIMemo(Games_state[room_number],x,z,type);


        // console.log(Games[room_number]);
        socket.broadcast.emit('StartLeadV_Success', {"Games":Games, "x":x, "z":z} );
    });

    socket.on('StartCarryHazmat',function(data){
      var room_number = data['room'];
        var Location = data['Location'];
        var name = data['name'];
        var carryV=data['carryV'];
        var x=data['x'];
        var z=data['z'];

        var participants = Games[room_number]["participants"];
        participants[name]["Location"] = Location;
        participants[name]['Carrying']=carryV;
        // console.log(Games[room_number]);
        socket.broadcast.emit('StartCarryHazmat_Success', {"Games":Games, "x":x, "z":z} );
    });

    socket.on('AddPOI',function(data){
      var x=data['x'];
      var z=data['z'];
      var type=data['type'];
      var room = data['room'];
      // console.log(Games_state[room]['POIMemo']);

      addPOI(Games_state[room],parseInt(x),parseInt(z),parseInt(type));
      console.log(Games_state[room]['POIMemo']);

      socket.broadcast.emit('AddPOI_Success',{'x':x,'z':z,'type':type});
    });

    socket.on('AddHazmat',function(data){
      var x=data['x'];
      var z=data['z'];
      var type=data['type'];

      socket.broadcast.emit('AddHazmat_Success',{'x':x,'z':z,'type':type});
    });

    socket.on('StopDrive',function(data){
      var name=data['name'];
      var room=data['room'];
      var origDrive=Games[room]["participants"][name]['Driving'];
      Games[room]["participants"][name]['Driving']="0";
      var stopride=[];
      for(var n in Games[room]['participants']){
        if(Games[room]['participants'][n]['Riding']==origDrive){
          stopride.push(n);
          Games[room]['participants'][n]['Riding']="0";
        }
      }
      io.sockets.emit('StopRide_Success',{"Games":Games,"ToStop":stopride});
      io.sockets.emit('StopDrive_Success',Games);
    });

    socket.on('StopRide',function(data){
      var name=data['name'];
      var room=data['room'];

      Games[room]["participants"][name]['Riding']="0";
      io.sockets.emit('StopDrive_Success',Games);
    });

    socket.on('StopCarry',function(data){
      var name=data['name'];
      var room=data['room'];
      var x=data['x'];
      var z=data['z'];
      Games[room]["participants"][name]['Carrying']="false";
      io.sockets.emit('StopCarry_Success',{"Games":Games,"x":x,"z":z});
    });

    socket.on('StopCarryH',function(data){
      var name=data['name'];
      var room=data['room'];
      var x=data['x'];
      var z=data['z'];
      Games[room]["participants"][name]['Carrying']="false";
      io.sockets.emit('StopCarryH_Success',{"Games":Games,"x":x,"z":z});
    });

    socket.on('StopLead',function(data){
      var name=data['name'];
      var room=data['room'];
      var x=data['x'];
      var z=data['z'];
      Games[room]["participants"][name]['Leading']="false";
      io.sockets.emit('StopLead_Success',{"Games":Games,"x":x,"z":z});
    })

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
      socket.broadcast.emit('changeRole_Success',Games);
    });

    socket.on('InitializePOI', function(){
      socket.broadcast.emit('InitializePOI_Success');
    });

    socket.on('InitializeHazmat', function(){
      socket.broadcast.emit('InitializeHazmat_Success');
    });

    socket.on('RescueCarried',function(data){
      var x=data['x'];
      var z=data['z'];

      var room = data['room'];
      var location = x+','+z;

      var poi = Games_state[room]['POIMemo'];

      var i = poi.findIndex(x => x[location]!= null);

      if (i !== -1) poi.splice(i, 1);

      Games_state[room]['POIMemo'] = poi;

      socket.broadcast.emit('RescueCarried_Success',{"x":x,"z":z});
    });

    socket.on('RescueTreated',function(data){
      var x=data['x'];
      var z=data['z'];

      var room = data['room'];
      var location = x+','+z;

      var poi = Games_state[room]['treatedPOIMemo'];
      var i = poi.findIndex(x => x[location]!= null);
      if (i !== -1) poi.splice(i, 1);
      Games_state[room]['treatedPOIMemo'] = poi;

      socket.broadcast.emit('RescueTreated_Success',{"x":x,"z":z});
    });

    socket.on('KillPOI',function(data){
      var x=data['x'];
      var z=data['z'];
      var room = data['room'];
      var location = x+','+z;

      var poi = Games_state[room]['POIMemo'];
      var i = poi.findIndex(x => x[location]!= null);
      if (i !== -1) poi.splice(i, 1);
      Games_state[room]['POIMemo'] = poi;

      var tpoi = Games_state[room]['treatedPOIMemo'];
      var i = tpoi.findIndex(x => x[location]!= null);
      if (i !== -1) tpoi.splice(i, 1);
      Games_state[room]['treatedPOIMemo'] = tpoi;

      var mpoi = Games_state[room]['movingPOIMemo'];
      var i = mpoi.findIndex(x => x[location]!= null);
      if (i !== -1) mpoi.splice(i, 1);
      Games_state[room]['movingPOIMemo'] = mpoi;

      socket.broadcast.emit('KillPOI_Success',{"x":x,"z":z});
    });

    socket.on('ConfirmPosition',function(data){
      var x=data['x'];
      var z=data['z'];
      var room=data['room'];
      var name=data['name'];
      Games[room]["participants"][name]["Location"]=x+","+z;
      if(!Games[room]["confirmedPosition"].includes(name)){
        Games[room]["confirmedPosition"].push(name);
      }
      if(Games[room]["confirmedPosition"].length==parseInt(Games[room]["numberOfPlayer"])){
        io.sockets.emit("ConfirmPosition_Success",{"Games":Games,"room":room})
      }
    });

    socket.on('savedGame',function(data){
        console.log("saved game");
        console.log(data);
        poiM = data;

        socket.emit('SaveGame_Success',data);
      });
    socket.on('victory',function(data){
      var room=data['room'];
      Games[room]=undefined;
      io.sockets.emit('victory_Success',{"room":room});
    });

    socket.on('defeat',function(data){
      var room=data['room'];
      Games[room]=undefined;
      io.sockets.emit('defeat_Success',{"room":room});
    });

    socket.on("JoinGame",function(data){
      console.log("in JoinGame");
      var name=data['name'];
      var room=data['room'];
      if(!Games[room]["joinedPlayers"].includes(name)){
        Games[room]["joinedPlayers"].push(name);
      }
      if(Games[room]["joinedPlayers"].length==parseInt(Games[room]["numberOfPlayer"])){
        io.sockets.emit("JoinGame_Success",{"room":room,"owner":Games[room]["Owner"]});
        console.log("emitting JoinGame");
      }
    });

    socket.on("ExplodeHazmat",function(data){
      var x=data['x'];
      var z=data['z'];
      var room=data['room'];

      socket.broadcast.emit("ExplodeHazmat_Success",{"x":x, "z":z, "room":room});
    });

});


server.listen( app.get('port'), function (){
    console.log("------- server is running ------- on port 3000");
});
