<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

$idUser = $_GET["idUser"];
if(isset($_GET["state"])) {
	$state = $_GET["state"];
} else {
	$state = '';
}

if($state != '') {
	$query = mysql_query("SELECT Event.idEvent, Event.Name, Event.StartDate, Event.EndDate, Event.Location, user_to_event.Status
	FROM `volleyball_app_db`.`event`
	LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
	LEFT JOIN user ON user.idUser = user_to_event.User_idUser
	WHERE User.idUser = $idUser
	AND user_to_event.Status = '$state';");
} else {
	$query = mysql_query("SELECT Event.idEvent, Event.Name, Event.StartDate, Event.EndDate, Event.Location, user_to_event.Status
	FROM `volleyball_app_db`.`event`
	LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
	LEFT JOIN user ON user.idUser = user_to_event.User_idUser
	WHERE User.idUser = $idUser;");
}

if($query === FALSE) {
	die("FAILED" . ' ' . mysql_error());
}

while($row = mysql_fetch_array($query)) {
	echo $row['idEvent']. '|' . $row['Name'] . '|' . $row['StartDate'] . '|' . $row['EndDate']. '|' . $row['Location'] . '|' . $row['Status'] . '|';
}
echo "<endoffile>";
?>