DROP PROCEDURE IF EXISTS returnBook;
DELIMITER //
CREATE PROCEDURE returnBook(IN readerID CHAR(8), IN bookID CHAR(8), OUT result VARCHAR(50)) BEGIN
    IF 
        readerID NOT IN 
        (SELECT reader_ID From Borrow 
        WHERE book_ID = bookID AND return_Date IS NULL)
    THEN SET result = 'You have not borrowed it.';
    
    ELSE BEGIN
        DECLARE newStatus INT;
        
        SET result = 'OK.';
        
        UPDATE Borrow 
        SET return_Date = CURDATE() 
        WHERE 
            reader_ID = readerID 
            AND book_ID = bookID
            AND return_Date IS NULL;
        
        IF (SELECT reserve_Times FROM Book WHERE ID = bookID) = 0
        THEN SET newStatus = 0;
        ELSE SET newStatus = 2;
        END IF;
        
        UPDATE Book SET status = newStatus WHERE ID = bookID;
    END;
    END IF;
END //
DELIMITER ;