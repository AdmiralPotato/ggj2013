var n = NPos3d,
	s = new n.Scene({

	});


var shipShape = {
	points: [
		[20, 0, 0],
		[-20, 20, 0],
		[-10, 0, 0],
		[-20, -20, 0],
		[-5, 0, 10]
	],
	lines: [
		[0,1],
		[1,2],
		[2,3],
		[3,0],
		[3,4],
		[2,4],
		[1,4],
		[0,4]
	]
};
var ship = new n.Ob3D({
	shape: shipShape,
	rot: [deg*45,0,0],
	color: '#fc0'
});

var animationController = {
	update: function() {
		ship.rot[2] += deg;
	}
};

s.add(ship);
s.add(animationController);