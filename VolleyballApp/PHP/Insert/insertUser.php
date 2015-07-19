<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["name"])) {$name = $_GET["name"];} else {$name = '';}
if(isset($_GET["role"])) {$role = $_GET["role"];} else {$role = '';}
if(isset($_GET["password"])) {$password = $_GET["password"];} else {$password = '';}
if(isset($_GET["number"])) {$number = $_GET["number"];} else {$number = '';}
if(isset($_GET["position"])) {$position = $_GET["position"];} else {$position = '';}

$subquery = mysql_query("SELECT idUser FROM User ORDER BY idUser DESC LIMIT 0, 1;");
$row = mysql_fetch_array($subquery);
$idUser = $row['idUser'] + 1;

if($name != '') {
	$query = mysql_query("INSERT INTO User(User.idUser, User.Name, User.Role, User.Password, User.Number, User.Position)
								VALUES($idUser, '$name', '$role', '$password', $number, '$position');");

	if($query === FALSE) {
		die("FAILED INSERT - " . ' ' . mysql_error());
	} else {
		echo "Insert succesful.";
	}
} else {
	echo "FAILED INSERT- name can't be null";
}
?>