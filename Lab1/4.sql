DROP PROCEDURE IF EXISTS borrowBook;
DELIMITER //
CREATE PROCEDURE borrowBook(IN readerID CHAR(8), IN bookID CHAR(8), OUT result VARCHAR(50)) BEGIN
    IF 
        readerID IN
        (SELECT readerID FROM Borrow
        WHERE book_ID = book_ID AND borrow_Date = CURDATE())
    THEN SET result = 'You have borrowed it today.';
    
    ELSEIF 
        (SELECT status FROM Book WHERE ID = bookID) = 1
    THEN SET result = 'The book has been borrowed.';
    
    ELSEIF 
        (SELECT reserve_Times FROM Book 
        WHERE ID = bookID) <> 0
        AND
        readerID NOT IN 
        (SELECT reader_ID FROM Reserve
        WHERE book_ID = bookID AND reader_ID = readerID)
    THEN SET result = 'You are not in the reservation list.';
    
    ELSEIF
        (SELECT COUNT(book_ID) FROM Borrow
        WHERE return_Date IS NULL AND reader_ID = readerID) >= 3
    THEN SET result = 'You have borrowed too many books.';
    
    ELSE
        SET result = 'OK.';

        DELETE FROM Reserve 
        WHERE reader_ID = readerID AND book_ID = bookID;

        INSERT INTO Borrow VALUES (bookID, readerID, CURDATE(), NULL);

        UPDATE Book 
        SET 
            borrow_Times = borrow_Times + 1,
            status = 1
        WHERE ID = bookID;
    END IF;
END //
DELIMITER ;