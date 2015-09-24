<?php

define('DB_LOCATION', dirname(__DIR__) . '/db/vb_tsvek.db');

function acquireDatabase() {
	return new SQLite3(DB_LOCATION);
}

?>