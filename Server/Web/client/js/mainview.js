window.requestAnimFrame = (function(){
	return  window.requestAnimationFrame       ||
		window.webkitRequestAnimationFrame ||
		window.mozRequestAnimationFrame    ||
		window.oRequestAnimationFrame      ||
		window.msRequestAnimationFrame     ||
		function( callback ){
			window.setTimeout(callback, 1000 / 60);
		};
})();

var width = window.innerWidth;
var height = window.innerHeight;

var scene = new THREE.Scene();
var camera = new THREE.PerspectiveCamera(50, width/height, 0.1, 1000);
scene.add(camera);

var rend = new THREE.WebGLRenderer( {antialias: true} );
rend.autoClear = false;
rend.setSize(width, height);
document.body.appendChild(rend.domElement);

function stopRender(){
	if (animRequest) {
       window.cancelAnimationFrame(animRequest);
       animRequest = undefined;
       document.body.removeChild(rend.domElement);
    }
}

function startRender(){
	if (!animRequest) {
		document.body.removeChild(rend.domElement);
       update();
    }
}

var geometryObjects = {};
var parseAllGeometry = function(_geoPath, _callback) {
	var propertyName, modelData, modelCount = 0, modelsLoaded = 0;

	for(propertyName in _geoPath){
		if(_geoPath.hasOwnProperty(propertyName)){

			modelCount += 1;
			modelData = _geoPath[propertyName];


			function createMesh(_propertyName){
				var meshCreationHandler = function(_geometry, _materials) {
					_geometry.computeTangents();
					_geometry.matLib = _materials;
					geometryObjects[_propertyName] = _geometry;

					modelsLoaded += 1;

					if(modelsLoaded === modelCount){
						_callback();
					}
				};
				var jsonLoader = new THREE.JSONLoader();
				jsonLoader.load(modelData, meshCreationHandler);
			}
			createMesh(propertyName);
		}
	}
};

var player;
var enemies=[];

var whatToDoWhenAllTheGeometriesAreParsedOkayYupSeriously = function(){


	makeLaser();

	update(); // start loop
};

var threeMakeEnemy(){
	var mesh = new THREE.Mesh(geometryObjects.fishShip, geometryObjects.fishShip.matLib[0]);
	mesh.position.set(50,0,0);
	mesh.lookAt(player.position);
	mesh.rotation.setY(mesh.rotation.y-(90*deg));
	scene.add(mesh);
	//enemies.push(mesh);
	return mesh;
}

var threeMakeShip(){
	var urls = [
		'../models/starfield.png',
		'../models/starfield.png',
		'../models/starfield.png',
		'../models/starfield.png',
		'../models/starfield.png',
		'../models/starfield.png'
    ];

    var cubemap = THREE.ImageUtils.loadTextureCube(urls);
	cubemap.format = THREE.RGBFormat;

	var shader = THREE.ShaderLib["cube"];
	shader.uniforms["tCube"].value = cubemap;

	var material = new THREE.ShaderMaterial( {
        fragmentShader: shader.fragmentShader,
        vertexShader: shader.vertexShader,
        uniforms: shader.uniforms,
        depthWrite: false,
 		side:THREE.BackSide
    });

	var newMesh = new THREE.Mesh(new THREE.CubeGeometry(500, 500, 500, 5, 5, 5),
		material);
	newMesh.flipSided = true;
	scene.add(newMesh);

	newMesh = new THREE.Mesh(geometryObjects.spear, geometryObjects.spear.matLib[0]);
	
	scene.add(newMesh);

	var directionalLightA = new THREE.DirectionalLight(0xffffff,2);
	directionalLightA.position.set( 0, -1, 0 );
	player.add(directionalLightA);

	var directionalLightB = new THREE.DirectionalLight(0xffffff,1.5);
	directionalLightB.position.set( 0, 1, 0 );
	player.add(directionalLightB);

	var directionalLightPointedFromCamera = new THREE.DirectionalLight(0xffffff, 0.5);
	directionalLightPointedFromCamera.position.set( -1, 0, 0 );

	camera.add(directionalLightPointedFromCamera);

	//player = newMesh;

	return newMesh;
}
var threeMakeStarbase(){

	var mesh = new THREE.Mesh(geometryObjects.starbase, geometryObjects.starbase.matLib[0]);
	mesh.position.set(0,0,150);
	mesh.lookAt(player.position);
	mesh.rotation.setY(mesh.rotation.y-(90*deg));
	scene.add(mesh);
	return mesh;
}


var laser;
var target;
var makeLaser = function(){
	target = enemies[Math.floor(Math.random()*enemies.length)];

	laser = new THREE.Mesh(
		geometryObjects.laser,
		geometryObjects.laser.matLib[0]);

	var distance = player.position.distanceTo(target.position);
	laser.scale.x = distance*0.5;
	laser.position = player.position;

	var newRot = Math.atan2(target.position.x - player.position.x, 
		target.position.z - player.position.z);

	laser .rotation.setY(newRot-(90*deg));

	scene.add(laser );
	return laser;
};

var updateLaser = function(){
	var distance = player.position.distanceTo(target.position);
	laser.scale.x = distance*0.5;
	laser.position = player.position;

	var newRot = Math.atan2(target.position.x - player.position.x, 
		target.position.z - player.position.z);

	laser.rotation.setY(newRot-(90*deg));
}

parseAllGeometry(
	{
		'laser': "../models/Laser.json",
		'fishShip': "../models/FishShip.json",
		'spear': "../models/Spear.json",
		'starbase': "../models/Starbase.json",
	},
	whatToDoWhenAllTheGeometriesAreParsedOkayYupSeriously
);

var tau = Math.PI*2;
var deg = tau/360;

var last = Date.now()/1000;
var timer = 0.0;

var client = client || {paused:false};
var animRequest;
var lastLaser = 0.0;
var timeToSwitch = 3.0;
var update = function(){
	animRequest = requestAnimationFrame(update);

	var now = Date.now()/1000; // get current time
	var dt = ((now-last) > .2) ? .2 : now-last; // calculate time between frames
	last = now; // save new time
	//if(client.paused == false){ // main 'update' loop
		timer += dt;

		player.rotation.setY(player.rotation.y+(dt*.1));

		particleSystem.rotation.setY(player.rotation.y*.5);

		if(timer > timeToSwitch){
			timeToSwitch = timer+(Math.random()*3);
			var newRnd = Math.floor(Math.random()*enemies.length);
			while(enemies[newRnd] == target){
				newRnd = Math.floor(Math.random()*enemies.length);
			}
			target = enemies[newRnd];
		}
		updateLaser();

		/*
		iterate though list of objects and position
		*/

		camera.position = distanceElevationHeading(50, 15 * deg, player.rotation.y);
		camera.lookAt(player.position);

		/*var i = updateObjects.length;
		while(i--){
			updateObjects[i].update();
		}*/
	//}
	render();
	
};

var render = function(){	
	if(window.innerWidth != width || window.innerHeight != height){
		width = window.innerWidth;
		height = window.innerHeight;
		camera.aspect = width/height;
		camera.updateProjectionMatrix();
		rend.setSize(width, height);
	}
	rend.render(scene, camera);
};

var distanceElevationHeading = function (_distance, _elevation, _heading){
	var radH = _heading + Math.PI;
	var radE = _elevation;
	var a = _distance * Math.cos(radE);
	return new THREE.Vector3(a * Math.cos(radH), _distance * Math.sin(radE), -a * Math.sin(radH));
};

// create the particle variables
var particleCount = 750,
    particles = new THREE.Geometry(),
 	pMaterial = new THREE.ParticleBasicMaterial({
    	color: 0xFFFFFF,
    	size: 10,
		map: THREE.ImageUtils.loadTexture(
      	"particle.png"
    ),
    blending: THREE.AdditiveBlending,
    transparent: true
  });

// now create the individual particles
for(var p = 0; p < particleCount; p++) {

  // create a particle with random
  // position values, -250 -> 250
  var pX = Math.random() * 1000 - 500,
      pY = Math.random() * 1000 - 500,
      pZ = Math.random() * 1000 - 500,
      particle = new THREE.Vertex(
        new THREE.Vector3(pX, pY, pZ)
      );
      // create a velocity vector
	particle.velocity = new THREE.Vector3(
		0,              // x
		-Math.random(), // y: random vel
		0);
	// add it to the geometry
  	particles.vertices.push(particle);
}


// create the particle system
var particleSystem =
  new THREE.ParticleSystem(
    particles,
    pMaterial);
particleSystem.sortParticles = true;
// add it to the scene
scene.add(particleSystem);