//Use node index.js in the terminal for execute the script.
//Warning: Firefox does not fully support the editor. Please use a chromimum-based web browser such as Chrome, Brave or Edge.
//This script is a basic example of a player's movement. You can load other examples by clicking on "Load example".
const server = 'https://api.artifactsmmo.com';
//Your token is automatically set
const token =
  'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VybmFtZSI6IlNvY2tTdHJlYW0iLCJwYXNzd29yZF9jaGFuZ2VkIjoiIn0.oXkgugB20AMqWw8c7iJovnMVwAPXKvrKTnAdspma04E';
//Put your character name here
const character = 'SockStream';

async function movement( x, y,i) {
  console.log("mouvement, x:" + x + "y:" + y);
  const url = server + '/my/' + character + '/action/move';
  const options = {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
      Authorization: 'Bearer ' + token,
    },
    body: '{"x":'+x+',"y":'+y+'}', //change the position here
  };

  try {
    const response =  await fetch(url, options);
    const status = response.status;
    console.log("status => " + status);
    var cooldown = 0;

        if (response.status === 200) {
          response.json().then((data) => {
              console.log('Your character successfully moved.');
               cooldown = data.data.cooldown.total_seconds;
              console.log(i + "my fucking CD : " + cooldown);
              //setTimeout(performGathering, cooldown * 1000);
        });
    }
    return cooldown;
} catch (error) {
  console.log("error " + error);
}
}

function delay(milliseconds){
  return new Promise(resolve => {
      setTimeout(resolve, milliseconds);
  });
}

async function init()
{
  let result = movement(0,0,1);
  result.then()
  /*console.log("returned cd1 : " + cd);
  console.log("");
  await delay(cd*1000);
  cd = await movement(1,0,2);
  console.log("returned cd2 : " + cd);
  console.log("");
  await delay(cd*1000);
  cd = await movement(11,0,3);
  console.log("returned cd3 : " + cd);
  console.log("");*/
}

init();
