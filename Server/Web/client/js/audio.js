
function showAlert(msg)
{
	alert("in audio.js - msg:"+msg);
}



function SFXChannels(source, channels){
	//No Jims allowed!
	if(this === window){throw('You cannot simply invoke this fuction, as it is meant to be called as a constructor with the `new` keyword!');}
	// simply for shortening and parental access in the member methods
	var t = this;
	t.source = source;
	t.currentSound = 0;
	t.maxChannels = channels;
	t.children = [];
	for(var i = 0; i < t.maxChannels; i++){
		var a = new Audio();
		a.muted=true;
		a.src=t.source;
		a.play(); //forces the audio to start loading from the server!
		a.pause();
		a.muted=false;
		t.children[i] = a;
	}
	t.play = function(delay){
		//console.log(t.currentSound);
		t.children[t.currentSound].currentTime = 0;
		t.children[t.currentSound].play();
		if(t.currentSound < t.maxChannels - 1){
			t.currentSound++;
		}else{
			t.currentSound=0;
		}
	}
	return t;
}

var SFX = {};
function preloadAudioList(sfxList, statusDiv, callback){
	//No Jims allowed!
	if(this === window){throw('You cannot simply invoke this fuction, as it is meant to be called as a constructor with the `new` keyword!');}
	// simply for shortening and parental access in the member methods
	var t = this;
	t.loaded = false;
	t.sounds = sfxList;
	t.children = [];
	for(var i = 0; i < t.sounds.length; i++){
		var src = t.sounds[i].src+'?killcache='+new Date().getTime();
		var audio = new Audio();
		audio.parent = t;
		audio.loaded = false;
		//audio.onload = function(e){this.loaded = true;t.checkProgress(e);}
		//I should probably do something more meaningful onerror.
		audio.onerror = function(e){this.loaded = true;t.checkProgress(e);errorDiv.innerHTML+='<br>error loading: '+this.src;};
		audio.src = src;
		audio.muted = true;
		audio.play(); //forces the audio to start loading from the server!
		//FIREFOX, WHAT THE HELL ARE YOU SMOKING? WHY DOES THE READY STATE GO BACK FROM 4 TO ZERO?!?!?
		//I PAUSE RIGHT AFTER PLAYING TO KEEP THAT STATE AT 4!
		audio.pause();
		audio.muted = false;
		t.children[i] = audio;
	}
	t.display = document.createElement('div');
	t.display.style.backgroundColor = '#333';
	t.display.style.width = '100%';
	t.display.style.height = '16px';
	statusDiv.appendChild(t.display);
	t.progress = document.createElement('div');
	t.progress.style.backgroundColor = '#f00';
	t.progress.style.width = '0%';
	t.progress.style.height = '16px';
	t.progress.style.textAlign = 'center';
	t.display.appendChild(t.progress);
	t.checkProgress = function(){
		var notLoadedCount = t.children.length;
		for(var i = 0; i < t.children.length; i++){
			if(t.children[i].loaded){
				notLoadedCount--;
			}else{
				if(t.children[i].readyState === 4){
					notLoadedCount--;
					t.children[i].loaded = true;
					SFX[t.sounds[i].name] = new SFXChannels(t.children[i].src,t.sounds[i].channels);
				}
			}
		}
		var percent = Math.floor(((t.children.length - notLoadedCount) * 100) / t.children.length);
		t.progress.style.width = percent+'%';
		t.progress.innerHTML = percent+'%';
		if(notLoadedCount === 0){
			clearInterval(t.areWeThereYet);
			t.loaded = true;
			callback();
		}
	}
	t.areWeThereYet = setInterval(t.checkProgress,100,false);
	//console.log(t);
	return t;
}

