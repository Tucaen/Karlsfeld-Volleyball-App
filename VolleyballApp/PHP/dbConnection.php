<?php

class ConnectionInfo {

	public function GetConnection() {
		// echo 'Trying to build up a connection'. "<br>";
		//$db = mysqli_connect("127.0.0.1", "root", "", "Volleyball_App_DB");
		$db = mysql_connect("127.0.0.1", "root", "");
		if(!$db) {
			die('Connection failed: ' . mysql_error());
		}
		mysql_select_db("Volleyball_App_DB", $db);
		return $db;
	}
}
?>