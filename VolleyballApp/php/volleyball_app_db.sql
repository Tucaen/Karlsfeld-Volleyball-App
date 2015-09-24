CREATE TABLE 'event' (
'id' integer PRIMARY KEY NOT NULL,
'name' varchar(255) DEFAULT NULL,
'startDate' datetime DEFAULT NULL,
'endDate' datetime DEFAULT NULL,
'location' varchar(255) DEFAULT NULL
);

INSERT INTO 'event' ('name', 'startDate', 'endDate', 'location') VALUES
('Training', '2015-07-17 18:30:00', '2015-07-17 20:30:00', 'Karlsfeld - Krenmooshalle'),
('Training', '2015-07-22 19:30:00', '2015-07-22 21:30:00', 'Dachau - Berufsschulhalle'),
('Training', '2015-07-24 18:30:00', '2015-07-24 20:30:00', 'Karlsfeld - Krenmooshalle'),
('Training', '2015-07-29 19:30:00', '2015-07-29 21:30:00', 'Dachau - Berufsschulhalle'),
('Training', '2015-07-31 18:30:00', '2015-07-31 20:30:00', 'Karlsfeld - Krenmooshalle'),
('Training', '2015-08-05 19:30:00', '2015-08-05 21:30:00', 'Dachau - Berufsschulhalle');

-- --------------------------------------------------------

CREATE TABLE 'user' (
'id' integer PRIMARY KEY NOT NULL,
'email' varchar(255),
'googlemail' varchar(255),
'password' varchar(255),
'state' varchar(10),
'name' varchar(255),
'role' varchar(64),
'number' integer,
'position' varchar(64)
);

-- --------------------------------------------------------

CREATE TABLE 'attendence' (
'userId' integer NOT NULL,
'eventId' integer NOT NULL,
'state' varchar(64) NOT NULL,
PRIMARY KEY (userId, eventId)
);

INSERT INTO 'attendence' ('userId', 'eventId', 'state') VALUES
(1, 1, 'eingeladen'),
(2, 1, 'eingeladen'),
(3, 1, 'eingeladen'),
(4, 1, 'eingeladen'),
(1, 2, 'eingeladen'),
(3, 2, 'eingeladen'),
(1, 3, 'eingeladen'),
(6, 3, 'eingeladen'),
(5, 1, 'eingeladen'),
(6, 1, 'eingeladen'),
(7, 1, 'eingeladen'),
(5, 2, 'eingeladen'),
(6, 2, 'eingeladen'),
(4, 2, 'eingeladen'),
(7, 2, 'eingeladen'),
(2, 2, 'eingeladen');

-- --------------------------------------------------------

CREATE TABLE 'userRights' (
'userId' integer NOT NULL,
'right' varchar(64) NOT NULL,
PRIMARY KEY (userId, right)
);

CREATE TABLE 'userRightsCollection' (
'name' varchar(64) PRIMARY KEY NOT NULL,
'longname' varchar(255)
);

INSERT INTO 'userRightsCollection' ('name', 'longname') VALUES
('CREATE_EVENT', 'The user can create events and delete his own events.'),
('MODIFY_EVENT', 'The user can create, modify and delete all events.'),
('MODFIY_USERS', 'The user can modify and delete other users'),
('ADMIN', 'The user can do everything.');