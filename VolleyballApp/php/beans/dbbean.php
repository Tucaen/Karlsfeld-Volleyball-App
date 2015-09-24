<?php

require_once(__DIR__ . '/dbexception.php');

class DbBean implements JsonSerializable {

	protected $db;
	protected $inDb;

	function __construct($db) {
		$this->db = $db;
		$this->inDb = FALSE;
	}

	public function getAttributes() {
		return [];
	}

	public function getTablename() {
		return '';
	}

	protected function getIdname() {
		return '';
	}

	protected function isIdAutoIncrement() {
		return FALSE;
	}

	public function load($sql, $vars = []) {
		$stmt = $this->db->prepare($sql);
		for ($i = 0; $i < sizeof($vars); $i++) {
			$stmt->bindParam($i+1, $vars[$i]);
		}
		$res = $stmt->execute();
		if ($res === FALSE) {
			throw new DbException('Database exception occured while loading: [' . $this->db->lastErrorCode() . '] ' . $this->db->lastErrorMsg());
		}

		$mappedRes = $this->mapResultSetToObject($res);
		return $mappedRes;
	}

	protected function getSelect() {
		return 'SELECT * FROM ' . $this->getTablename();
	}

	public function loadAll() {
		return $this->load($this->getSelect());
	}

	public function loadId($ids) {
		$idnames = $this->getIdname();
		if (is_array($idnames) xor is_array($ids)) {
			if (is_array($idnames)) {
				throw new DbException('You must pass an array of IDs, because the primary key contains more than one column.');	
			} else {
				throw new DbException('You must pass an single ID, because the primary key contains just one column.');	
			}
			
		}

		if (is_array($idnames)) {
			if (sizeof($ids) != sizeof($idnames)) {
				throw new DbException('The number of passed IDs doesn\'t match the number of columns contained in the primary key.');
			}
			$sql = '';
			$first = TRUE;
			foreach ($idnames as $n) {
				if ($first === FALSE) {
					$sql = $sql . ' AND ';
				} else {
					$first = FALSE;
				}
				$sql = $sql . $n . '=?';
				return $this->loadWhere($sql, $ids);
			}
		} else {
			return $this->loadWhere($idnames . '=?', [$ids]);
		}
	}

	public function loadWhere($where, $vars = []) {
		$sql = $this->getSelect() . ' WHERE ' . $where;
		return $this->load($sql, $vars);
	}

	public function reload($attrs) {
		$sql = '';
		$vars = [];
		$first = TRUE;
		foreach ($attrs as $a) {
			$tmp = $this->attributeForColumn($a);
			if (!is_null($tmp)) {
				if ($first === FALSE) {
					$sql = $sql . ' AND ';
				} else {
					$first = FALSE;
				}
				$sql .= $tmp . '=?';
				$vars[] = $this->getVar($tmp);
			}
		}
		if (sizeof($vars) < 1) {
			throw new DbException('The passed columns, which are needed to identify the bean don\'t match any column names.');
		}
		$res = $this->loadWhere($sql, $vars);
		if (sizeof($res) > 0) {
			$obj = $res[0];
			$attributes = $this->getAttributes();
			foreach ($attributes as $a) {
				$this->setVar($a, $obj->getVar($a));
			}
		}
	}

	protected function mapResultSetToObject($result) {
		$rClass = new ReflectionClass($this);
		$mapped = [];
		$attrs = $this->getAttributes();
		$colnames = [];
		for ($i = 0; $result->columnName($i) !== FALSE; $i++) {
			$colnames[] = $result->columnName($i);
		}

		while (($rec = $result->fetchArray()) !== FALSE) {
			$instance = $rClass->newInstance($this->db);
			foreach ($colnames as $colname) {
				$colnameLc = strtolower($colname);
				foreach ($attrs as $attr) {
					$attrLc = strtolower($attr);
					if (strcmp($colnameLc, $attrLc) === 0) {
						$instance->setVar($attr, $rec[$colname]);
					}
				}
			}

			$instance->setInDb(TRUE);
			$mapped[] = $instance;
		}

		return $mapped;
	}

	public function isInDb() {
		return $this->inDb;
	}

	public function setInDb($aBool) {
		$this->inDb = $aBool;
	}

	public function save() {
		if ($this->isInDb()) {
			$this->update();
		} else {
			$this->insert();
		}
	}

	public function update() {
		return $this->updateWhere('id=?', [$this->getVar($this->getIdname())]);
	}

	public function updateColumns($columns) {
		return $this->updateBasic($columns, 'id=?', [$this->getVar($this->getIdname())]);
	}

	public function updateWhere($where, $vars = []) {
		return $this->updateBasic($this->getAttributes(), $where, $vars);
	}

	public function updateBasic($columns, $where, $vars = []) {
		$sql = 'UPDATE ' . $this->getTablename() . ' SET ';
		$firstCol = TRUE;
		$vars2 = [];
		foreach ($columns as $col) {
			$attr = $this->attributeForColumn($col);
			if (empty($attr)) {
				throw new DbException('The passed columns don\'t match the column names.');
			}
			if ($this->isIdAutoIncrement() === FALSE or strcmp($attr, $this->getIdname()) !== 0) {
				if ($firstCol === FALSE) {
					$sql = $sql . ', ';
				} else {
					$firstCol = FALSE;
				}
				$sql = $sql . $col . ' = ?';
				$vars2[] = $this->getVar($attr);
			}
		}
		$sql = $sql . ' WHERE ' . $where;
		$stmt = $this->db->prepare($sql);
		$j = 1;
		for ($i = 0; $i < sizeof($vars2); $i++) {
			$stmt->bindParam($j, $vars2[$i]);
			$j++;
		}
		for ($i = 0; $i < sizeof($vars); $i++) {
			$stmt->bindParam($j, $vars[$i]);
			$j++;
		}

		$res = $stmt->execute();
		if ($res === FALSE) {
			throw new DbException('Database exception occured while updating: [' . $this->db->lastErrorCode() . '] ' . $this->db->lastErrorMsg());
		}
		return $this->db->changes();
	}

	public function insert() {
		return $this->insertBasic($this->getAttributes());
	}

	public function insertBasic($columns) {
		$sql = 'INSERT INTO ' . $this->getTablename() . ' (';
		$values = '';
		$vars = [];
		$firstCol = TRUE;
		foreach ($columns as $col) {
			if ($firstCol === FALSE) {
				$sql = $sql . ', ';
				$values = $values . ', ';
			} else {
				$firstCol = FALSE;
			}
			$sql = $sql . $col;
			$values = $values . '?';
			$attr = $this->attributeForColumn($col);
			if (empty($attr)) {
				throw new DbException('The passed columns don\'t match the column names.');
			}
			$vars[] = $this->getVar($attr);
		}
		$sql = $sql . ') VALUES (' . $values . ')';
		$stmt = $this->db->prepare($sql);
		for ($i = 0; $i < sizeof($vars); $i++) {
			$stmt->bindParam($i+1, $vars[$i]);
		}
		$res = $stmt->execute();
		if ($res === FALSE) {
			throw new DbException('Database exception occured while inserting: [' . $this->db->lastErrorCode() . '] ' . $this->db->lastErrorMsg());
		}
		return $this->db->changes();
	}

	public function delete() {
		return $this->deleteWhere('id=?', [$this->getVar($this->getIdname())]);
	}

	public function deleteWhere($where, $vars = []) {
		$sql = 'DELETE FROM ' . $this->getTablename() . ' WHERE ' . $where;
		$stmt = $this->db->prepare($sql);
		for ($i = 0; $i < sizeof($vars); $i++) {
			$stmt->bindParam($i+1, $vars[$i]);
		}
		$res = $stmt->execute();
		if ($res === FALSE) {
			throw new DbException('Database exception occured while deleting: [' . $this->db->lastErrorCode() . '] ' . $this->db->lastErrorMsg());
		}
		return $this->db->changes();
	}

	private function attributeForColumn($colname) {
		$attrs = $this->getAttributes();
		foreach ($attrs as $attr) {
			$attrLc = strtolower($attr);
			$colnameLc = strtolower($colname);
			if (strcmp($attrLc, $colnameLc) === 0) {
				return $attr;
			}
		}
		return null;
	}

	protected function setVar($varname, $value) {
		$rClass = new ReflectionClass($this);
		$mName = 'set' . ucfirst($varname);
		if ($rClass->hasMethod($mName)) {
			$meth = $rClass->getMethod($mName);
			$meth->invoke($this, $value);
			return TRUE;
		} else {
			return FALSE;
		}
	}

	protected function getVar($varname) {
		$rClass = new ReflectionClass($this);
		$mName = 'get' . ucfirst($varname);
		if ($rClass->hasMethod($mName)) {
			$meth = $rClass->getMethod($mName);
			return $meth->invoke($this);
		} else {
			return null;
		}
	}

	public function jsonSerialize() {
		$attrs = $this->getAttributes();
		$vars = [];
		foreach ($attrs as $var) {
			$vars[$var] = $this->getVar($var);
		}
		return [get_class($this) => $vars];
	}

}

?>