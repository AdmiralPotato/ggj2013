var n = NPos3d,
	s = new n.Scene({

	});


var shipShape = {
	points: [
		[20, 0, 0],
		[-20, 20, 0],
		[-20, -20, 0]
	],
	lines: [
		[0,1],
		[1,2],
		[2,0]
	]
};
var ship = new n.Ob3D({
	shape: shipShape
});

var animationController = {
	update: function() {
		ship.rot[2] += deg;
	}
};

s.add(ship);
s.add(animationController);