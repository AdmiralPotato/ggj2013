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

var whatToDoWhenAllTheGeometriesAreParsedOkayYupSeriously = function(){

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
	player = newMesh;
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

	update(); // start loop
};

var makeLaser = function(_source, _target){
	_target = _target || enemies[3];

	var newLaser = new THREE.Mesh(
		geometryObjects.laser,
		geometryObjects.laser.matLib[0]);

	var distance = _source.position.distanceTo(_target.position);
	newLaser.scale.x = distance*0.5;
	newLaser.position = _source.position;

	var newRot = Math.atan2(_target.position.x - _source.position.x, 
		_target.position.z - _source.position.z);

	newLaser.rotation.setY(newRot-(90*deg));

	scene.add(newLaser);
	return newLaser;
};

var updateLaser = function(_laser, _source, _target){
	var distance = _source.position.distanceTo(_target.position);
	_laser.scale.x = distance*0.5;
	_laser.position = _source.position;

	var newRot = Math.atan2(_target.position.x - _source.position.x, 
		_target.position.z - _source.position.z);

	_laser.rotation.setY(newRot-(90*deg));
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
//var updateObjects = [];

var update = function(){
	requestAnimFrame(update);
	var now = Date.now()/1000; // get current time
	var dt = ((now-last) > .2) ? .2 : now-last; // calculate time between frames
	last = now; // save new time
	if(client.paused == false){ // main 'update' loop
		timer += dt;

		//player.rotation.setY(player.rotation.y+(dt*.1));

		/*
		iterate though list of objects and position
		*/

		camera.position = distanceElevationHeading(50, 15 * deg, player.rotation.y);
		camera.lookAt(player.position);

		/*var i = updateObjects.length;
		while(i--){
			updateObjects[i].update();
		}*/
	}
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