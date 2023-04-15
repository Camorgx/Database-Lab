SELECT 
    Book.ID AS '图书号',
    Book.name AS '书名',
    borrow_Date AS '借期'
FROM Book, Reader, Borrow
WHERE 
    Book.ID = book_ID
    AND Reader.ID = reader_ID
    AND Reader.name = 'Rose';