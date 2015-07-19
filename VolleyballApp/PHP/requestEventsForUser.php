<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

// $query = 'INSERT INTO `volleyball_app_db`.`user`
// (`idUser`,`Name`,`Role`,`Password`,`Number`,`Position`)
// VALUES
// (5,"Sebastian Mayerhofer","Spieler","",11,"Aussen")';

$query = mysql_query("SELECT Event.idEvent, Event.Name eventName, User.Name userName, Event.StartDate, Event.EndDate, User.Number, User.Position
FROM `volleyball_app_db`.`event`
LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
LEFT JOIN user ON user.idUser = user_to_event.User_idUser
WHERE user.idUser = 1;");
if($query === FALSE) {
	die(mysql_error());
}

while($row = mysql_fetch_array($query)) {
	echo $row['eventName'] . ' ' . $row['userName'] . ' ' . $row['Number'] . ' ' . $row['Position'] ;
	echo "<br>";
}

// $query = mysql_query("SELECT *
// FROM `volleyball_app_db`.`event`
// LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
// LEFT JOIN user ON user.idUser = user_to_event.User_idUser
// WHERE event.idEvent = 1;");
// if($query === FALSE) {
	// die(mysql_error());
// }
// while($row = mysql_fetch_array($query)) {
	// echo $row['Name'] . $row['StartDate'] . $row['EndDate'] ;
	// echo "<br>";
// }
?>