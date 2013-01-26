(function() {
	//Make the thrustometer a vertical slider
	//Fires thrustometerValueSelected event
	var createSlider = function($element, options){
		var sliderAdjustValue = function(event){
			var relativeYCoordinate = event.pageY - event.currentTarget.offsetTop;
			console.log(relativeYCoordinate);
			var sliderValue = (Math.abs(min) + Math.abs(max) - relativeYCoordinate) + min;
			var percentageToFill = (1 - (relativeYCoordinate / $element.height())*100);
			var fillColor;
			var threshold = 10;
			if(sliderValue < -1*threshold){
				fillColor = "#E01B1B";
				fill.css('top', '66%');
			}
			else if(Math.abs(sliderValue-threshold) < threshold){
				fillColor = "#CCCCCC";
				//Make it "stick" near 0
				fill.css('bottom', 30);
				percentageToFill = 5;
				sliderValue = 0;
			}
			else if(sliderValue > threshold){
				fillColor = "#3BDE12";
				fill.css('bottom', '66%');
			}
			console.log(percentageToFill);
			fill.css('background-color', fillColor);
			fill.height(percentageToFill+'%');
			$element.attr('data-sliderValue', sliderValue);
		};
		var sliderMouseUpHandler = function(event){
			//Remove the sliderAdjustValue event listener
			$element.trigger('thrustometerValueSelected', [$element.attr('data-sliderValue')]);
			$(event.currentTarget).unbind('mousemove', sliderAdjustValue);
		};
		var sliderMouseDownHandler = function(event){
			//Add the sliderAdjustValue event listener
			console.log("mousedown");
			sliderAdjustValue(event);
			$(event.currentTarget).mousemove(sliderAdjustValue);
		}
		var min = options.min || -50;
		var max = options.max || 150;
		var value = options.value || 0;
		var fill = $element.find('.fillColor');
		$element.mousedown(sliderMouseDownHandler);
		$element.mouseup(sliderMouseUpHandler);
		$element.mouseleave(sliderMouseUpHandler);
		
	}
	var thrustometer = $("#thrustometer");
	createSlider(thrustometer,{
		min: -50,
		max: 150,
		value: 0,
	});
})();

