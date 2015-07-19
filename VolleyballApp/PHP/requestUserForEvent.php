<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

$query = mysql_query("SELECT Event.idEvent, Event.Name eventName, User.Name userName, User.Password, User.idUser, User.Role, Event.StartDate, Event.EndDate, User.Number, User.Position
FROM `volleyball_app_db`.`event`
LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
LEFT JOIN user ON user.idUser = user_to_event.User_idUser
WHERE event.idEvent = 1;");
if($query === FALSE) {
	die("FAILED" . ' ' . mysql_error());
}

while($row = mysql_fetch_array($query)) {
	echo $row['idUser']. '|' . $row['userName'] . '|' . $row['Role'] . '|' . $row['Password']. '|' . $row['Number'] . '|' . $row['Position'] . '|';
}
echo "<endoffile>";
?>