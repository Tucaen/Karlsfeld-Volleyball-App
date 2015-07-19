<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

$idEvent = $_GET["idEvent"];
if(isset($_GET["state"])) {
	$state = $_GET["state"];
} else {
	$state = '';
}

if($state != '') {
	$query = mysql_query("SELECT User.Name, User.Password, User.idUser, User.Role, User.Number, User.Position, user_to_event.Status
	FROM `volleyball_app_db`.`event`
	LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
	LEFT JOIN user ON user.idUser = user_to_event.User_idUser
	WHERE event.idEvent = $idEvent
	AND user_to_event.Status = '$state';");
} else {
	$query = mysql_query("SELECT User.Name, User.Password, User.idUser, User.Role, User.Number, User.Position, user_to_event.Status
	FROM `volleyball_app_db`.`event`
	LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
	LEFT JOIN user ON user.idUser = user_to_event.User_idUser
	WHERE event.idEvent = $idEvent;");
}


if($query === FALSE) {
	die("FAILED" . ' ' . mysql_error());
}

while($row = mysql_fetch_array($query)) {
	echo $row['idUser']. '|' . $row['Name'] . '|' . $row['Role'] . '|' . $row['Password']. '|' . $row['Number'] . '|' . $row['Position'] . '|';
}
echo "<endoffile>";
?>