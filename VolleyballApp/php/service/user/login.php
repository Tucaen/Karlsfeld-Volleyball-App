<?php

define('ROOT', dirname(dirname(__DIR__)));
require_once(ROOT . '/db.php');
require_once(ROOT . '/beans/user.php');
require_once(ROOT . '/util/response_util.php');
require_once(ROOT . '/beans/dbexception.php');

/**
* This script is used to login.
* @param type
* 		The possible values are: [email, google, facebook]
*		This parameter is optional. Default is 'email'.
* @param email
* 		Needed if type is 'email'.
* @param password
* 		Needed if type is 'email'.
*
* @return A message with a status: [OK,ERR]
*/

function loginFacebook() {
	// TODO do stuff
}

function loginGoogle() {
	// TODO do stuff
}

function loginMail() {
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

	// Check if user exists
	$db = acquireDatabase();
	$loader = new User($db);
	try {
		$res = $loader->loadWhere('email=?', [$email]);
		if (sizeof($res) > 0) {
			$user = $res[0];
			// Check if password is correct
			$validPassword = $user->getPassword();
			$password = User::encryptPassword($password);
			if ($validPassword == $password) {
				// Login successful -> return session id
				session_start();
				$_SESSION['uid'] = $user->getId();
				$_SESSION['email'] = $user->getEmail();
				if ($user->getState() == 'FILL_DATA') {
					sendMessage(WARN, 'Login successful. Please complete your registration.');
				} else {
					$_SESSION['name'] = $user->getName();
					sendMessage(OK, 'Login successful.');
				}
			} else {
				sendMessage(ERR, 'Password invalid.');
			}
		} else {
			// User doesn't exist
			sendMessage(ERR, 'User invalid.');
		}
	} catch (DbException $e) {
		sendMessage(ERR, $e->getMessage());
	}

	$db->close();
}

if (isset($_GET['type'])) {
	switch ($_GET['type']) {
		case 'google':
			// Sign in with Google
			loginGoogle();
			break;
		case 'facebook':
			loginFacebook();
			break;
		case 'email':
			// Sign in with E-Mail
			loginMail();
			break;
		default:
			sendError('The register type (specified with \'type=' . $_GET['type'] . '\') is invalid.');
			break;
	}
} else {
	// Sign in with E-mail
	loginMail();
}

?>