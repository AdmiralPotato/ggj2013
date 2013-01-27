(function() {
	//Make the thrustometer a vertical slider
	//Fires thrustometerValueSelected event
	var createSlider = function($element, options){
		var sliderAdjustValue = function(event){
			//Sets the rangeVal variable.
			var yValueClicked = event.pageY - event.currentTarget.offsetTop;
			var rangeZeroPercentage = 1 - Math.abs(min/(max+Math.abs(min)));
			var uiHeight = $element.height();
			var uiZero = uiHeight * rangeZeroPercentage;
			var threshold = 10;
			var fillColor, fillHeight;
			var rangeVal = max - (yValueClicked/uiHeight)*(max-min)
			if (yValueClicked > uiZero - threshold && yValueClicked < uiZero + threshold)
			{
				fillColor = "#CCC";
				fillHeight = uiHeight * .03;
				fill.css('top', uiZero - fillHeight/2);
				rangeVal = 0;
			}
			else if( yValueClicked > uiZero){
				fillColor = "#F00";
				fillHeight = yValueClicked - uiZero;
				fill.css('bottom', '');
				fill.css('top', uiZero);
			}
			else if(yValueClicked < uiZero){
				console.log("above zero");
				fillColor = "#0F0";
				fillHeight = uiZero - yValueClicked;
				fill.css('top', '');
				fill.css('bottom', uiHeight-uiZero);
			}
			fill.css('background-color', fillColor);
			fill.css('height', fillHeight);

			// Set rangeValue wherever you want.
			// This is the equivalent of a slider's range value.  This is what you send back to the server!!!
			
		};
		var sliderMouseUpHandler = function(event){
			//Remove the sliderAdjustValue event listener
			$element.trigger('thrustometerValueSelected', [$element.attr('data-sliderValue')]);
			$(event.currentTarget).unbind('mousemove', sliderAdjustValue);
		};
		var sliderMouseDownHandler = function(event){
			//Add the sliderAdjustValue event listener
			sliderAdjustValue(event);
			$(event.currentTarget).mousemove(sliderAdjustValue);
		}
		var min = options.min;
		var max = options.max;
		var initial = options.initial;
		var fill = $element.find('.fillColor');
		$element.mousedown(sliderMouseDownHandler);
		$element.mouseup(sliderMouseUpHandler);
		$element.mouseleave(sliderMouseUpHandler);
		
	}
	var thrustometer = $("#thrustometer");
	createSlider(thrustometer,{
		min: -50,
		max: 100,
		initial: 0,
	});
	createSlider($("#thrustometer2"),{
		min: 0,
		max: 100,
		initial: 0,
	});
	createSlider($("#thrustometer3"),{
		min: -100,
		max: 0,
		initial: 0,
	});
})();

