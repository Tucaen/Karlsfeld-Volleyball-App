<?php

define('ROOT', dirname(dirname(__DIR__)));
require_once(ROOT . '/db.php');
require_once(ROOT . '/util/response_util.php');
require_once(ROOT . '/beans/event.php');
require_once(ROOT . '/beans/dbexception.php');

$id = $_GET['id'];
$db = acquireDatabase();
$loader = new Event($db);

try {
	if (isset($id)) {
		$res = $loader->loadId($id);
	} else {
		$res = $loader->loadAll();
	}
	sendSimpleData($res);
} catch (DbException $e) {
	sendMessage(ERR, $e->getMessage());
}

$db->close();

?>