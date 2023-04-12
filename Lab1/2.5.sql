SELECT Reader.name AS '读者姓名'
FROM 
    Reader,
    (SELECT reader_ID, COUNT(book_ID) AS cnt
    FROM Borrow GROUP BY reader_ID) AS borrowCnt
WHERE
    Reader.ID = borrowCnt.reader_ID
    AND borrowCnt.cnt > 10;