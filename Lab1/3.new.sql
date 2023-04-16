DROP PROCEDURE IF EXISTS updateReaderID;
DELIMITER //

CREATE PROCEDURE updateReaderID(IN originID CHAR(8), IN newID CHAR(8)
    OUT status VARCHAR(50)) BEGIN
    DECLARE s INT DEFAULT 0;
    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET s = 1;
    START TRANSACTION;
    SET FOREIGN_KEY_CHECKS = 0;
    UPDATE Reader SET ID = newID WHERE ID = originID;
    UPDATE Borrow SET ID = newID WHERE ID = originID;
    UPDATE Reserve SET ID = newID WHERE ID = originID;
    SET FOREIGN_KEY_CHECKS = 1;
    IF s = 0 THEN
        SET status = 'OK';
        COMMIT;
    ELSE 
        SET status = 'Failed.'
        ROLLBACK;
    END IF;
END //

DELIMITER ;