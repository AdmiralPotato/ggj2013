var n = NPos3d,
	s = new n.Scene({

	}),
	client = {
		entityList: {},
		ui: [],
		entityTypes: {}
	};
var timeSinceLastFrame = 0;
var timeOfLastFrame = new Date().getTime();
var timeSinceLastUpdate = 0;
var timeOfLastUpdate = timeOfLastFrame;
var deltaSinceUpdate = 0;
var animationController = {
	update: function() {
		var currentTime = new Date().getTime();
		timeSinceLastFrame =  currentTime - timeOfLastFrame;
		timeOfLastFrame = currentTime;
		deltaSinceUpdate = 250 / timeSinceLastUpdate;
	}
};
s.add(animationController);

var deltaInit = function(o){
	o.posLast = [0,0,0];
	o.posNext = [0,0,0];
	o.posDiff = [0,0,0];
	o.lastRot = 0;
	o.rotDiff = 0;
};
var deltaUpdate = function(o, entityData){
	o.posLast[0] = o.posNext[0];
	o.posLast[1] = o.posNext[1];
	o.posLast[2] = o.posNext[2];
	o.posNext[0] = entityData.Position.X;
	o.posNext[1] = entityData.Position.Y;
	o.posNext[2] = entityData.Position.Z;
	o.posDiff[0] = o.posNext[0] - o.posLast[0];
	o.posDiff[1] = o.posNext[1] - o.posLast[1];
	o.posDiff[2] = o.posNext[2] - o.posLast[2];
	o.lastRot = o.rotNext;
	o.rotNext = entityData.Rotation;
	o.rotDiff = o.rotNext - o.lastRot;
};
var deltaInterpolate = function(o){
	o.pos[0] = o.posLast[0] + (o.posNext[0] * deltaSinceUpdate);
	o.pos[1] = o.posLast[1] + (o.posNext[1] * deltaSinceUpdate);
	o.pos[2] = o.posLast[2] + (o.posNext[2] * deltaSinceUpdate);
	o.rot[2] = o.lastRot + (o.posNext[2] * deltaSinceUpdate);
};

client.entityTypes.Ship = function(args){
	var t = this, type = 'Ship';
	if(t.type !== type){throw type + ' constructor must be invoked using the `new` keyword.';}
	args = args || {};
	n.blessWith3DBase(t,args);
	deltaInit(t);
	s.add(t);
	return t;
};

client.entityTypes.Ship.prototype = {
	type: 'Ship',
	color: '#0f0',
	shape: wireframes.simpleShip,
	update: function() {
		var t = this;
		deltaInterpolate(t);
		t.render();
	},
	updateFromData: function(entityData){
		var t = this;
		deltaUpdate(t, entityData);
	}
};


client.entityTypes.Starbase = function(args){
	var t = this, type = 'StarBase';
	if(t.type !== type){throw type + ' constructor must be invoked using the `new` keyword.';}
	args = args || {};
	n.blessWith3DBase(t,args);
	t.scale = [100,100,100];
	deltaInit(t);
	s.add(t);
	return t;
};

client.entityTypes.Starbase.prototype = {
	type: 'StarBase',
	color: '#0f0',
	shape: wireframes.starBase,
	update: function() {
		var t = this;
		deltaInterpolate(t);
		t.render();
	},
	updateFromData: function(entityData){
		var t = this;
		deltaUpdate(t, entityData);
	}
};


var setGameStateFromServer = function(data) {
	var entityDataIndex,
		numEntities = data.Entities.length,
		entityData,
		entity,
		entityIdString,
		currentTime = new Date().getTime();
	timeSinceLastUpdate =  currentTime - timeOfLastUpdate;
	timeOfLastUpdate = currentTime;
	//console.log('game state update!');
	for(entityDataIndex = 0; entityDataIndex < numEntities; entityDataIndex += 1) {
	    entityData = data.Entities[entityDataIndex];
		entityIdString = 'entity-' + entityData.Id;
		entity = client.entityList[entityIdString];
		if(entity === undefined){
			entity = client.entityList[entityIdString] = new client.entityTypes[entityData.Type]();
		}
		entity.updateFromData(entityData);
	}
};