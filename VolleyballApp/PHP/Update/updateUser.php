<?php
require_once(dirname('_FILE_').'/dbConnection.php');

$connection = new ConnectionInfo();
$connection->GetConnection();

if(isset($_GET["idUser"])) {$idUser = $_GET["idUser"];} else {$idUser = '';}

if($idUser != '') {
	$subquery = mysql_query("SELECT User.idUser, User.Name, User.Password, User.Role, User.Number, User.Position
							FROM `volleyball_app_db`.`user`
							WHERE user.idUser = $idUser;");
	if($subquery === FALSE) {
		echo "FAILED UPDATE - User with this userId [.$idUser.] doesn't exist!"; 
	} else {
		while($row = mysql_fetch_array($subquery)) {
			$idUserOriginal = $row['idUser'];
			$nameOriginal = $row['Name'];
			$roleOriginal = $row['Role'];
			$passwordOriginal = $row['Password'];
			$numberOriginal = $row['Number'];
			$positionOriginal = $row['Position'];
		}
	
		if(isset($_GET["name"])) {$name = $_GET["name"]; if($name == ''){$name = $nameOriginal;}} else {$name = $nameOriginal;}
		if(isset($_GET["role"])) {$role = $_GET["role"]; if($role == ''){$role = $roleOriginal;}} else {$role = $roleOriginal;}
		if(isset($_GET["password"])) {$password = $_GET["password"]; if($password == ''){$password = $passwordOriginal;}} else {$password = $passwordOriginal;}
		if(isset($_GET["number"])) {$number = $_GET["number"]; if($number == ''){$number = $numberOriginal;}} else {$number = $numberOriginal;}
		if(isset($_GET["position"])) {$position = $_GET["position"]; if($position == ''){$position = $positionOriginal;}} else {$position = $positionOriginal;}

		$query = mysql_query("UPDATE User
								SET name='$name', role='$role', password='$password', number=$number, position='$position'
								WHERE idUser = $idUser;");

		if($query === FALSE) {
			die("FAILED UPDATE - " . ' ' . mysql_error());
		} else {
			echo "Update succesful.";
		}
	}


} else {
	echo "FAILED UPDATE - idUser can't be null";
}
?>