<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["name"])) {$name = $_GET["name"];} else {$name = '';}
if(isset($_GET["startDate"])) {$startDate = $_GET["startDate"];} else {$startDate = '';}
if(isset($_GET["endDate"])) {$endDate = $_GET["endDate"];} else {$endDate = '';}
if(isset($_GET["location"])) {$location = $_GET["location"];} else {$location = '';}

$subquery = mysql_query("SELECT idEvent FROM Event ORDER BY idEvent DESC LIMIT 0, 1;");
$row = mysql_fetch_array($subquery);
$idEvent = $row['idEvent'] + 1;

if($name != '') {
	$query = mysql_query("INSERT INTO Event(Event.idEvent, Event.Name, Event.StartDate, Event.EndDate, Event.Location)
								VALUES($idEvent, '$name', '$startDate', '$endDate', '$location');");

	if($query === FALSE) {
		die("FAILED INSERT - " . ' ' . mysql_error());
	} else {
		echo "Insert succesful.";
	}
} else {
	echo "FAILED INSERT- name can't be null";
}
?>