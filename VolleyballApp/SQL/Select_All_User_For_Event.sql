SELECT *
FROM `volleyball_app_db`.`event`
LEFT JOIN user_to_event ON user_to_event.Event_idEvent = event.idEvent
LEFT JOIN user ON user.idUser = user_to_event.User_idUser
WHERE event.idEvent = 1;