SELECT DISTINCT
    Book.ID AS '图书号',
    Book.name AS '书名'
FROM Book, Borrow
WHERE
    Book.ID = Borrow.book_ID
    AND Borrow.return_Date IS NULL
    AND Book.name LIKE '%MySQL%';