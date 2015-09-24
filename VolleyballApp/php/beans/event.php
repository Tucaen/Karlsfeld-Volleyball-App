<?php

require_once('dbbean.php');
require_once('attendence.php');

class Event extends DbBean {

	private $id;
	private $name;
	private $location;
	private $startDate;
	private $endDate;
	private $attendences = [];

	function __construct($db) {
		parent::__construct($db);
	}

	function getAttributes() {
		return ['id', 'name', 'location', 'startDate', 'endDate'];
	}

	function getIdname() {
		return 'id';
	}

	function isIdAutoIncrement() {
		return true;
	}

	function getTablename() {
		return 'event';
	}


	// Load the attendences of the users to this event
	function loadAttendences($state = null) {
		$loader = new Attendence($this->$db);
		$this->setAttendences($loader->loadWhere('eventId=?', [$this->getId()]));
		return $this->getAttendences();
	}


	// Getter and setter

	function setId($id) {
		$this->id = $id;
	}
	function getId() {
		return $this->id;
	}

	function setName($name) {
		$this->name = $name;
	}
	function getName() {
		return $this->name;
	}

	function setLocation($location) {
		$this->location = $location;
	}
	function getLocation() {
		return $this->location;
	}

	function setStartDate($startDate) {
		$this->startDate = $startDate;
	}
	function getStartDate() {
		return $this->startDate;
	}

	function setEndDate($endDate) {
		$this->endDate = $endDate;
	}
	function getEndDate() {
		return $this->endDate;
	}

	function setAttendences($atts) {
		$this->attendences = $atts;
	}
	function getAttendences() {
		return $this->attendences;
	}

}

?>