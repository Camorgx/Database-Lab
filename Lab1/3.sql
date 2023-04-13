DROP PROCEDURE IF EXISTS updateReaderID;
DELIMITER //
CREATE PROCEDURE updateReaderID(IN originID CHAR(8), IN newID CHAR(8)) BEGIN
    DECLARE state INT DEFAULT 0;
    DECLARE readerName VARCHAR(10);
    DECLARE readerAge INT;
    DECLARE readerAddress VARCHAR(20);
    
    DECLARE borrowedBooks CURSOR FOR
        (SELECT book_ID, borrow_Date, return_Date FROM Borrow
        WHERE reader_ID = originID);
    
    DECLARE reservedBooks CURSOR FOR
        (SELECT book_ID, reserve_Date, take_Date FROM Reserve
        WHERE reader_ID = originID);
    
    DECLARE CONTINUE HANDLER FOR NOT FOUND SET state = 1;

    SELECT name, age, address FROM Reader
    WHERE ID = originID
    INTO readerName, readerAge, readerAddress;

    INSERT INTO Reader VALUES (newID, readerName, readerAge, readerAddress);
    
    OPEN borrowedBooks;
    REPEAT BEGIN
        DECLARE bookID CHAR(8);
        DECLARE borrowDate, returnDate DATE;
        FETCH borrowedBooks INTO bookID, borrowDate, returnDate;
        IF state = 0 THEN
            INSERT INTO Borrow VALUES (bookID, newID, borrowDate, returnDate);
        END IF;
	END;
    UNTIL state = 1
    END REPEAT;
    CLOSE borrowedBooks;

    SET state = 0;

    OPEN reservedBooks;
    REPEAT BEGIN
        DECLARE bookID CHAR(8);
        DECLARE reserveDate, takeDate DATE;
        FETCH reservedBooks INTO bookID, reserveDate, takeDate;
        IF state = 0 THEN
            INSERT INTO Borrow VALUES (bookID, newID, reserveDate, takeDate);
        END IF;
	END;
    UNTIL state = 1
    END REPEAT;
    CLOSE reservedBooks;
    
    DELETE FROM Borrow WHERE reader_ID = originID;
    DELETE FROM Reserve WHERE reader_ID = originID;
    DELETE FROM Reader WHERE ID = originID;
END //
DELIMITER ;