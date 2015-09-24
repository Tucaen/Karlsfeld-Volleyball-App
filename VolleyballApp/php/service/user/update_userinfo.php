<?php

define('ROOT', dirname(dirname(__DIR__)));
require_once(ROOT . '/db.php');
require_once(ROOT . '/beans/user.php');
require_once(ROOT . '/util/response_util.php');
require_once(ROOT . '/beans/dbexception.php');

/**
* This script is used to complete the registration or just update the userdata.
* @param name
* 		The name, which should be displayed for the user.
* 		If the status of the user is 'FILLDATA'. The name is required.
* @param role
* 		The role the user has in the team.
* @param position
* 		Only can set this, if the role is 'Spieler';
* @param number
* 		Only can set this, if the role is 'Spieler';
*
* @return A message with a status: [OK,ERR]
*/

function setValues($user) {
	if (isset($_GET['name'])) {
		$user->setName($_GET['name']);
	}
	if (isset($_GET['role'])) {
		$user->setRole($_GET['role']);
	}
	if (isset($_GET['position']) and $user->getRole() == 'Spieler') {
		$user->setPosition($_GET['position']);
	}
	if (isset($_GET['number']) and $user->getRole() == 'Spieler') {
		$user->setNumber(intval($_GET['number']));
	}
}


session_start();
if (!isset($_SESSION['uid'])) {
	sendMessage(ERR, 'You need to be logged in, to update your profile.');
	return;
}

$db = acquireDatabase();
$loader = new User($db);

try {
	$res = $loader->loadId($_SESSION['uid']);
	if (sizeof($res) <= 0) {
		sendMessage(ERR, 'Couldn\'t find you user ID in the database. Please contact the administrator.');
		return;
	}

	$obj = $res[0];

	if ($obj->getState() == 'FILLDATA') {
		// We need to set at least the name
		if (!isset($_GET['name'])) {
			sendMessage(ERR, 'You need to set at least your name.');
			return;
		}
		$obj->setName($_GET['name']);
		$obj->setState('FINAL');
	}

	setValues($obj);
	if ($obj->save() !== FALSE) {
		$_SESSION['name'] = $obj->getName();
		sendMessage(OK, 'User ' . $obj->getName() . ' updated successfully.');
	} else {
		sendMessage(ERR, 'User ' . $obj->getName() . ' could not be updated due to an database error.');
	}
} catch (DbException $e) {
	sendMessage(ERR, $e->getMessage());
}

$db->close();

?>