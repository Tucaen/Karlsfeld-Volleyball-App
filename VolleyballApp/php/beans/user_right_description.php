<?php

require_once('dbbean.php');

class UserRightDescription extends DbBean {

	private $name;
	private $longname;

	function __construct($db) {
		parent::__construct($db);
	}

	function getAttributes() {
		return ['name', 'longname'];
	}

	function getIdname() {
		return 'name';
	}

	function isIdAutoIncrement() {
		return FALSE;
	}

	function getTablename() {
		return 'userLongnamesCollection';
	}


	// Getter and setter

	function setName($id) {
		$this->name = $id;
	}
	function getName() {
		return $this->name;
	}

	function setLongname($longname) {
		$this->longname = $longname;
	}
	function getLongname() {
		return $this->longname;
	}

}

?>