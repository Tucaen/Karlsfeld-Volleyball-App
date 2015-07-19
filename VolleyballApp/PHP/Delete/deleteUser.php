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
		echo "FAILED DELETE - User with this userId [.$idUser.] doesn't exist!"; 
	} else {
		$query = mysql_query("DELETE FROM User
								WHERE idUser = $idUser;");

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