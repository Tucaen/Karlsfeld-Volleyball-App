<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["username"]) && isset($_GET["password"])) {
	$username = $_GET["username"];
	$password = $_GET["password"];
	$subquery = mysql_query("SELECT User.idUser, User.Name, User.Password, User.Role, User.Number, User.Position
							FROM `volleyball_app_db`.`user`
							WHERE user.Username = '$username'
							AND user.Password = '$password';");
	if($subquery === FALSE) {
		echo "FAILED LOGIN - username or password is wrong"; 
	} else {
		$row = mysql_fetch_assoc($subquery);
		if($row == false) {
			echo "FAILED LOGIN - username or password is wrong";
		} else {
			echo $row['idUser']. '|' . $row['Name'] . '|' . $row['Role'] . '|' . $row['Password']. '|' . $row['Number'] . '|' . $row['Position'] . '|' . '' . '|';
			echo "<endoffile>";
		}
	}
} else {
	echo "FAILED TO LOGIN - username and password can not be null";
}