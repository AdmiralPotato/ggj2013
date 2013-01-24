<?php
error_reporting(0);

//MySQL DB settings
include_once('config.inc.php');



if($_POST['status'] == 'online'){
	$chat_status = 'online';
}else{
	$chat_status = 'offline';
}


mysql_query("UPDATE ".$sql_table_users." SET chat_status='".$chat_status."' WHERE id='".$_POST['own_id']."'");

?>