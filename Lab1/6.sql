DELIMITER //

DROP TRIGGER IF EXISTS onReserveInsert //
CREATE TRIGGER onReserveInsert AFTER INSERT 
ON Reserve FOR EACH ROW BEGIN
    UPDATE Book
    SET status = 2
    WHERE ID = new.book_ID AND status <> 1;
    UPDATE Book 
    SET reserve_Times = reserve_Times + 1
    WHERE ID = new.book_ID;
END //

DROP TRIGGER IF EXISTS onReserveDelete //
CREATE TRIGGER onReserveDelete AFTER DELETE
ON Reserve FOR EACH ROW BEGIN
    UPDATE Book
    SET reserve_Times = reserve_Times - 1
    WHERE ID = old.book_ID;
    UPDATE Book
    SET status = 0
    WHERE
        ID = old.book_ID
        AND status <> 1
        AND reserve_Times <> 0;
END //

DELIMITER ;