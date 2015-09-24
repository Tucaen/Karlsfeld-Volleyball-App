<?php

require_once('dbbean.php');

class UserRight extends DbBean {

	private $userId;
	private $right;

	function __construct($db) {
		parent::__construct($db);
	}

	function getAttributes() {
		return ['userId', 'right'];
	}

	function getIdname() {
		return ['userId', 'right'];
	}

	function isIdAutoIncrement() {
		return FALSE;
	}

	function getTablename() {
		return 'userRights';
	}


	// Getter and setter

	function setUserId($id) {
		$this->userId = $id;
	}
	function getUserId() {
		return $this->userId;
	}

	function setRight($right) {
		$this->right = $right;
	}
	function getRight() {
		return $this->right;
	}

}

?>