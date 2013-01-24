/************************************************************************************
 
jQuery AJAX Chat
version 1.1

Author: http://www.jquerychat.net/

If you want to buy this script, drop me a mail: support@jquerychat.net

************************************************************************************/


(function($) {
	
	history.navigationMode = 'compatible';	//for Opera to support unload function
	
	$(document).ready(function() {					
		
		var heartBeat = 600;	//chat auto refresh time in miliseconds	
		var windowscount = 0;	
		var playsound = true;
		//GO ONLINE
		$.ajax({
			type: "POST",
			url: 'set_status.php',		
			async: false,					
			data: 'own_id='+$own_id+'&status=online',
			success: function(i){	
				//do nothing	
			}
		});
		
		
		
		//MINIMIZE, MAXIMIZE, CLOSE CHAT WINDOW			
			
			//minimize function
			$('.minimize_chatbox').live('click',function(){
				//remove chat,message area			
				$(this).closest('.chatbox').find('.chat_area,.chat_message,.chat_info').css('height','0px');		
				$(this).closest('.chatbox').css('height','25px');
				
				//replace minimize icon
				$(this).css('display','none');
				$(this).closest('.chatbox').find('.maximize_chatbox').css('display','inline');
				
				return false;
			});
			
			
			//maximize function
			$('.maximize_chatbox').live('click',function(){
				//remove chat,message area			
				$(this).closest('.chatbox').find('.chat_area').css('height','180px');		
				$(this).closest('.chatbox').find('.chat_message').css('height','55px');		
				$(this).closest('.chatbox').find('.chat_info').css('height','20px');		
				$(this).closest('.chatbox').css('height','300px');
				$(this).closest('.chatbox').find('.header_bg_blink').removeClass("header_bg_blink").addClass("header_bg_default");
				
				//replace minimize icon
				$(this).css('display','none');
				$(this).closest('.chatbox').find('.minimize_chatbox').css('display','inline');
				$(this).closest('.chatbox').find('.header .new_message').remove();
				$(this).closest('.chatbox').find('.chat_message textarea').focus();
				return false;
			});
			
			
			//close function
			$('.close_chatbox').live('click',function(){
				$(this).closest('.chatbox').remove();		
				
				//set nu position for all appearing chat window
				$('.chatbox').each(function(){
					var prev_pos = parseInt($(this).css('right'));
					if(prev_pos != 10){
						var nu_pos = prev_pos - 225;
						$(this).css('right',nu_pos+'px');
					}
				});
				
				return false;
			});		
			
					
		
			
		//PLACE CHAT WINDOW
		
			function PopupChat(partner_id,partner_username){
				windowscount ++;
				var wctr = windowscount;
				
				$('body').append('<div class="chatbox cb_default" id="chat_window_'+wctr+'" title="'+partner_id+'">'+
					'<div class="header header_bg_default" title="'+partner_username+'">'+
						'<p>'+partner_username+'</p>'+
						'<a href="#" class="close_chatbox" title="close chat window">X</a>'+
						'<a href="#" class="minimize_chatbox" title="minimize chat window">_</a>'+
						'<a href="#" class="maximize_chatbox" title="maximize chat window">&#8254;</a>'+
					'</div>'+
					'<div class="chat_area" title="'+partner_username+'">'+
					'</div>'+
					'<div class="chat_info"><p></p></div>'+					
					'<div class="chat_message" title="Type your message here">'+
						'<textarea></textarea>'+
					'</div>'+
				'</div>');
				
				//check if already exist any window on page and set nu position
				if(wctr > 0){
					//get last chat window position
					var prev_w = wctr - 1;
					var prev_w_pos = parseInt($('#chat_window_'+prev_w).css('right'));
					var nu_w_pos = prev_w_pos + 225;
					$('#chat_window_'+wctr).css('right',nu_w_pos+'px');
				}
				
				return false;
			
			}

		
		
			//ON USER CLICK POP UP CHAT
		
			$('.chat_user').live('click',function(){
				var substr = $(this).attr('alt').split('|');
				var user_id = substr[0];
				var user_name = substr[1];
				
				//check if a windows is already open with this user first!
				if($('div[title="'+user_id+'"]').length > 0){					
					//alert('You\'re already chatting with him/her!');
				}else{
					PopupChat(user_id,user_name);
				}
					
			});
			
			// set focus in Message area
			$('.chatbox').live('click',function(){
				$textarea = $('.chat_message textarea',this);		
				$textarea.focus();
			});
			
		
			//PRINT A LINE TO CHAT 
			function print_to_chat(window_id,text){
				$('#'+window_id+' .chat_area').append(text);
			}
	
			
			
			//HIGHLIGHT Active chat window
			$('.chat_message textarea').live('focus',function() {
				var chatbox = $(this).closest('.chatbox');
				this_chatbox_headerbg = $('.header',chatbox);
				this_chatbox_headerbg.removeClass("header_bg_blink").addClass("header_bg_default");
				chatbox.removeClass("cb_default").addClass("cb_highlight"); // add highligt to chat window
				chatbox.data('focused',1);		   // enable focus variable
				chatbox.data('havenewmessage',0); // clear new message
			});
			$('.chat_message textarea').live('blur',function() {
				var chatbox = $(this).closest('.chatbox');
				chatbox.removeClass("cb_highlight").addClass("cb_default"); // remove highligt of chat window
				chatbox.data('focused',0);	// disable focus variable
			});
			
			//SEND MESSAGE ON ENTER		
			$('.chat_message textarea').live('keypress', function (e) {
				if (e.keyCode == 13 && !e.shiftKey) {
					e.preventDefault();
					
					//add to MySQL DB with AJAX and PHP
					var to_id = $(this).closest('.chatbox').attr('title');
					var this_chat_window_id = $(this).closest('.chatbox').attr('id');
					var this_textarea = $(this);
					var datastring = 'from_id='+$own_id+'&to_id='+to_id+'&message='+this_textarea.val();
					
					$.ajax({
						type: "POST",
						url: 'send_message.php',						
						data: datastring,
						success: function(i){							
							if(i == 1){
								//if success, reload chat area
								this_textarea.val('');							
							}else{					
								//if error,  print it into chat
								//print_to_chat(this_chat_window_id,'<p><span class="error">Error! Message not sent!</span></p>');
								
								//uncomment to print mysql error to chat
								//print_to_chat(i);								
							}
						}
					});
				}else{
					//user is typing...
					//$is_typing = 1;	//1.2 - Remove this line
				}				
			});
			
			$('.chat_message textarea').live('keyup', function(){
				  //$is_typing = 0; //1.2 - Remove this line
			});
			

			
		//USER GOES OFFLINE ON WINDOW CLOSE/NAVIGATE AWAY
		
			$(window).unload(function (){ 				
				$.ajax({
					type: "POST",
					url: 'set_status.php',		
					async: false,					
					data: 'own_id='+$own_id+'&status=offline',
					success: function(i){	
						//do nothing												
					}
				});						
			});
			
			
			
		//LOOP OF LIFE - checks every ... seconds if there's a new message
		var prev_length = new Array();
		function liveChat(){
			//go through all popped up window and reload messages, mark those messages as received			
			$('.chatbox').each(function(){												
				var is_typing;
				var this_chatbox = $(this);
				var this_chatbox_chat_area = $('.chat_area',this);		
				var this_chatbox_headerbg = $('.header',this);				
				var this_chatbox_header = $('.header p',this);				
				var this_chatbox_max_btn  = $('.header .maximize_chatbox',this);				
				var this_chatbox_id = $(this).attr('title');
				var this_newmessage = $('.header p .new_message',this);				
				//v1.2 -----------------------------------------------------
				
				if (this_chatbox.data('havenewmessage') == 1 && this_chatbox.data('fistload') == 1) { // blinking chat window if not focused and have a new message
					if (this_chatbox.data('blink') == 0) {
						this_chatbox_headerbg.removeClass("header_bg_default").addClass("header_bg_blink");
						this_chatbox.data('blink',1);
					} else {
						this_chatbox.data('blink',0);
						this_chatbox_headerbg.removeClass("header_bg_blink").addClass("header_bg_default");
					}
				}
				
				var this_chatbox_textarea  = $('.chat_message textarea',this);								
				if (this_chatbox_textarea.val() !='') {
					is_typing = 1;
				} else {
					is_typing = 0;
				}
				//v1.2 -----------------------------------------------------								
				$.ajax({
					type: "POST",
					url: 'load_message.php',								
					data: 'own_id='+$own_id+'&partner_id='+this_chatbox_id+'&is_typing='+is_typing,
					success: function(i){	
						//reload messages in chat area
						if(i != 0){						
							//alert(i);
							this_chatbox_chat_area.html(i);														
							
							if(prev_length[this_chatbox_id] != i.length){							
								//scroll to bottom if new message received
								if (i.indexOf('Last message sent at') == -1) {
									if (this_chatbox.data('focused') != 1 && this_chatbox.data('havenewmessage') != 1) {
										if (this_chatbox.data('fistload') == 1) {
											this_chatbox.data('havenewmessage',1);
											if (playsound) {
												$("#player_div").empty();
												$("#player_div").prepend(insertPlayer());
											}	
										}	
									}
								}
								if (this_chatbox.data('fistload') != 1) {
									this_chatbox.data('fistload',1);
								}
								this_chatbox_chat_area.animate({scrollTop: 9999999},200);
								//display new message info next to partner's name, if window is minimized
								if(this_chatbox_max_btn.css('display') != 'none' && this_chatbox_header.find('.new_message').length == 0){									
									this_chatbox_header.append('<span class="new_message"> *</span>');									
								}							
							
							}
							prev_length[this_chatbox_id] = i.length;
						}else{
							//alert(i);
						}
					}
				});	
				
				//check if user is typing
				$.ajax({
					type: "POST",
					url: 'is_typing.php',			
					data: 'own_id='+$own_id+'&partner_id='+$(this).attr('title'),
					success: function(e){
						if(e == 1){
							$('.chatbox[title="'+this_chatbox_id+'"] .chat_info p').text('User is typing...');
						}else{
							$('.chatbox[title="'+this_chatbox_id+'"] .chat_info p').text('');
						}
					}
				});
			});
			
			
			//check if current user has a new message (to_id = current_user_id) which haven't been received and there's no popup with that user
			$.ajax({
				type: "POST",
				url: 'load_popup_message.php',							
				data: 'own_id='+$own_id,
				success: function(o){												
					if(o != 0){
						//there's a new message, so just open up a new chat window and message will be loaded automatically
						var substr = o.split(';;;');		

						//check if a windows is already open with this user first!
						if($('div[title="'+substr[0]+'"]').length == 0){					
							PopupChat(substr[0],substr[1]);
						}												
						
					}
				}
			});
			
			
			
			
			
			//and start the loop again
			t=setTimeout(liveChat,heartBeat);
		}
		
		liveChat();	//start the chat
			
	});
			
})(jQuery);

function insertPlayer(){

	    var playerpath	= 'flash/player.swf';
		var filename	= 'flash/gong.mp3';

		var mp3html = '<object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" ';
		mp3html += 'width="1" height="1" ';
		mp3html += 'codebase="http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab">';
		mp3html += '<param name="movie" value="'+playerpath+'?';
		mp3html += 'showDownload=false&file=' + filename + '&autoStart=true';
		mp3html += '&backColor=ffffff&frontColor=ffffff';
		mp3html += '&repeatPlay=false&songVolume=50" />';
		mp3html += '<param name="wmode" value="transparent" />';
		mp3html += '<embed wmode="transparent" width="1" height="1" ';
		mp3html += 'src="' + playerpath + '?'
		mp3html += 'showDownload=false&file=' + filename + '&autoStart=true';
		mp3html += '&backColor=ffffff&frontColor=ffffff';
		mp3html += '&repeatPlay=false&songVolume=50" ';
		mp3html += 'type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />';
		mp3html += '</object>';
		return mp3html;

}
	
