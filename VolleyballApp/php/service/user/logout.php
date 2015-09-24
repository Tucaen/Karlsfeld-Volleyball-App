<?php

define('ROOT', dirname(dirname(__DIR__)));
require_once(ROOT . '/db.php');
require_once(ROOT . '/util/response_util.php');

/**
* This script is used to logout.
*/

session_start();
$loggedIn = isset($_SESSION['uid']);
session_destroy();
if ($loggedIn === TRUE) {
	sendMessage(OK, 'You logged out successfully.');
} else {
	sendMessage(WARN, 'You weren\'t logged in.');
}

?>