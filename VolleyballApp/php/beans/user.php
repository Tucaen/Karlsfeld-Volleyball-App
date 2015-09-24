<?php

require_once('dbbean.php');
require_once('user_right.php');

class User extends DbBean {

	private $id;			// Primary key
	private $email;
	private $googlemail;
	private $password;
	private $state;
	private $name;
	private $role;
	private $number;
	private $position;
	private $rights = [];

	function __construct($db) {
		parent::__construct($db);
	}

	public function getAttributes() {
		return ['id', 'email', 'googlemail', 'password', 'state', 'name', 'role', 'number', 'position'];
	}

	protected function getIdname() {
		return 'id';
	}

	protected function isIdAutoIncrement() {
		return TRUE;
	}

	public function getTablename() {
		return 'user';
	}


	public function setAndEncryptPassword($password) {
		$this->setPassword(self::encryptPassword($password));
	}

	public static function encryptPassword($password) {
		return md5($password);
	}

	public function loadRights() {
		$loader = new UserRight($this->$db);
		$this->setRights($loader->loadWhere('userId=?', [$this->getId()]));
		return $this->getRights();
	}


	// Getter and setter

	function setId($id) {
		$this->id = $id;
	}
	function getId() {
		return $this->id;
	}

	function setEmail($email) {
		$this->email = $email;
	}
	function getEmail() {
		return $this->email;
	}

	function setGooglemail($googlemail) {
		$this->googlemail = $googlemail;
	}

	function getGooglemail() {
		return $this->googlemail;
	}

	function setState($state) {
		$this->state = $state;
	}
	function getState() {
		return $this->state;
	}

	function setName($name) {
		$this->name = $name;
	}
	function getName() {
		return $this->name;
	}

	function setRole($role) {
		$this->role = $role;
	}
	function getRole() {
		return $this->role;
	}

	function setPassword($password) {
		$this->password = $password;
	}
	function getPassword() {
		return $this->password;
	}

	function setNumber($number) {
		$this->number = $number;
	}
	function getNumber() {
		return $this->number;
	}

	function setPosition($position) {
		$this->position = $position;
	}
	function getPosition() {
		return $this->position;
	}

	function setRights($rights) {
		$this->rights = $rights;
	}
	function getRights() {
		return $this->rights;
	}

}

?>