DROP VIEW IF EXISTS ReaderBorrow;
CREATE VIEW ReaderBorrow AS
    SELECT 
        Reader.ID AS readerID,
        Reader.name AS readerName,
        Book.ID AS bookID,
        Book.name AS bookName,
        borrow_Date AS borrowDate
    FROM Reader, Book, Borrow
    WHERE 
        book_ID = Book.ID
        AND reader_ID = Reader.ID
        AND DATEDIFF(CURDATE(), borrow_Date) <= 365;