<?php

define('ROOT', dirname(dirname(__DIR__)));
require_once(ROOT . '/db.php');
require_once(ROOT . '/beans/user.php');
require_once(ROOT . '/util/response_util.php');
require_once(ROOT . '/beans/dbexception.php');

/**
* This script is used to register an account with an email and password.
* If everything is successful, the state of the user is 'FILLDATA'. 
* This is intended to update the user data in the next step. Call <update_userinfo.php> to complete the user setup.
* @param email
* 		The E-mail which will be used to login.
* @param password
* 		The specified password of the user.
*
* @return A message with a status: [OK,ERR]
*/

// Sign up with E-mail
$errMsg = '';
if (!isset($_GET['email'])) {
	$errMsg .= 'email';
}
if (!isset($_GET['password'])) {
	if (strlen($errMsg) > 0) {
		$errMsg .= ', ';
	}
	$errMsg .= 'password';
}
if (strlen($errMsg) > 0) {
	// At least one of the fields is not set, so return an error
	sendMessage(ERR, 'The following required parameters are not set: [' . $errMsg . ']');
	return;
}

$email = $_GET['email'];
$password = $_GET['password'];

// Check mail
if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
	sendMessage(ERR, 'The format of the given email is invalid: [' . $email . ']');
	return;
}

// Check if the mail already exists
$db = acquireDatabase();
$loader = new User($db);
try {
	$res = $loader->loadWhere('email=?', [$email]);
	if (sizeof($res) > 0) {
		$db->close();
		sendMessage(ERR, 'This email is already in use. Did you forget your password?');
		return;
	}

	$user = new User($db);
	$user->setEmail($email);
	$user->setAndEncryptPassword($password);
//	$user->setState('CONF_MAIL');
	$user->setState('FILLDATA'); // Next step is to fill the missing data like name

	$user->save();
	session_start();
	$user->reload(['email']);
	$_SESSION['uid'] = $user->getId();
	$_SESSION['email'] = $user->getEmail();

	sendMessage(OK, 'User ' . $user->getEmail() . ' registered successfully. Please complete your registration.');

} catch (DbException $e) {
	sendMessage(ERR, $e->getMessage());
}

$db->close();

?>