var n = NPos3d,
	s = new n.Scene({

	}),
	client = {
		entityList: {},
		ui: [],
		entityTypes: {}
	};



client.entityTypes.Ship = function(args){
	var t = this, type = 'Ship';
	if(t.type !== type){throw type + ' constructor must be invoked using the `new` keyword.';}
	args = args || {};
	n.blessWith3DBase(t,args);
	s.add(t);
	return t;
};

client.entityTypes.Ship.prototype = {
	type: 'Ship',
	color: '#0f0',
	shape: wireframes.ship,
	update: function() {
		var t = this;
//		t.rot[2] += deg;
		t.render();
	},
	updateFromData: function(entityData){
		var t = this;
		t.pos[0] = entityData.Position.X;
		t.pos[1] = entityData.Position.Y;
		t.pos[2] = entityData.Position.Z;
		t.rot[2] = entityData.Rotation;
	}
};


var setGameStateFromServer = function(data) {
	var entityDataIndex,
		numEntities = data.Entities.length,
		entityData,
		entity,
		entityIdString;
	console.log('game state update!');
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


//var tempGameStateUpdate = function(){
//	var tempGameStateData = {
//		entityList: [
//			{
//				id: 0,
//				type: 'Ship',
//				position: [0,0,0],
//				rotation: deg * 45
//			}
//		]
//	};
//	setGameStateFromServer(tempGameStateData);
//};

//var tempGameStateInterval = setInterval(tempGameStateUpdate, 250);