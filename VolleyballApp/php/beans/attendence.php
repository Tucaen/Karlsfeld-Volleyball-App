<?php

require_once('dbbean.php');

class Attendence extends DbBean {

	private $userId;
	private $eventId;
	private $state;

	function __construct($db) {
		parent::__construct($db);
	}

	function getAttributes() {
		return ['userId', 'eventId', 'state'];
	}

	function getIdname() {
		return ['userId', 'eventId'];
	}

	function isIdAutoIncrement() {
		return FALSE;
	}

	function getTablename() {
		return 'attendence';
	}


	// Getter and setter

	function setUserId($id) {
		$this->userId = $id;
	}
	function getUserId() {
		return $this->userId;
	}

	function setEventId($id) {
		$this->eventId = $id;
	}
	function getEventId() {
		return $this->eventId;
	}

	function setState($state) {
		$this->state = $state;
	}
	function getState() {
		return $this->state;
	}

}

?>