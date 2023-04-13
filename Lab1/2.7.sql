SELECT 
    Reader.ID AS '读者号',
    Reader.name AS '姓名',
    BorrowCnt.cnt AS '借阅图书数'
FROM 
    Reader,
    (SELECT 
        reader_ID AS ID,
        COUNT(book_ID) AS cnt
    FROM Borrow
    WHERE borrow_Date LIKE '2022%'
    GROUP BY reader_ID) AS BorrowCnt
WHERE Reader.ID = BorrowCnt.ID
ORDER BY BorrowCnt.cnt DESC LIMIT 10;