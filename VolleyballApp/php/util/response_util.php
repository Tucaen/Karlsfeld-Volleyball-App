<?php

define('STATE', 'state');
define('OK', 'ok');
define('ERR', 'error');
define('WARN', 'warning');

define('DATA', 'data');
define('MSG', 'message');
define('CODE', 'code');

function createMessage($state, $message = 'n/a', $code = 'n/a') {
	return json_encode([STATE => $state, CODE => $code, MSG => $message]);
}
function sendMessage($state, $message = 'n/a', $code = 'n/a') {
	echo createMessage($state, $message, $code);
}

function createSimpleData($data) {
	return json_encode([STATE => OK, DATA => $data]);
}
function sendSimpleData($data) {
	echo createSimpleData($data);
}

function createData($state, $data, $message = 'n/a', $code = 'n/a') {
	return json_encode([STATE => $state, CODE => $code, MSG => $message, DATA => $data]);
}
function sendData($state, $data, $message = 'n/a', $code = 'n/a') {
	echo createData($state, $data, $message, $code);
}

?>