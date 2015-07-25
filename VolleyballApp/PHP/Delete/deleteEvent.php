<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["idEvent"])) {$idEvent = $_GET["idEvent"];} else {$idEvent = '';}

if($idUser != '') {
	$subquery = mysql_query("SELECT Event.idEvent
							FROM `volleyball_app_db`.`event`
							WHERE event.idEvent = $idEvent;");
	if($subquery === FALSE) {
		echo "FAILED DELETE - User with this idEvent [.$idEvent.] doesn't exist!"; 
	} else {
		$query = mysql_query("DELETE FROM Event
								WHERE idEvent = $idEvent;");

		if($query === FALSE) {
			die("FAILED DELETE - " . ' ' . mysql_error());
		} else {
			echo "Delete succesful.";
		}
	}
} else {
	echo "FAILED DELETE - idUser can't be null";
}
?>