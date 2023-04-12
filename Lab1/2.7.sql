SELECT 
    Reader.ID AS '读者号',
    Reader.name AS '姓名'
FROM 
    Reader,
    (SELECT 
        reader_ID AS ID,
        COUNT(book_ID) AS cnt
    FROM Borrow
    GROUP BY reader_ID) AS BorrowCnt
WHERE Reader.ID = BorrowCnt.ID
ORDER BY BorrowCnt.cnt DESC LIMIT 10;