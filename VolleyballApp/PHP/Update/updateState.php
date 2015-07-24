<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["idUser"])) {$idUser = $_GET["idUser"];} else {$idUser = '';}
if(isset($_GET["idEvent"])) {$idEvent = $_GET["idEvent"];} else {$idEvent = '';}
if(isset($_GET["state"])) {$state = $_GET["state"];} else {$state = '';}

if($idUser != '' && $idEvent != '' && $state != '') {
	$query = mysql_query("UPDATE User_To_Event
						SET Status='$state'
						WHERE User_idUser = $idUser
						AND Event_idEvent = $idEvent;");

	if($query === FALSE) {
		die("FAILED UPDATE - " . ' ' . mysql_error());
	} else {
		echo "Update succesful.";
	}
} else {
	echo "FAILED UPDATE - idUser, idEvent and state can't be null";
}
?>