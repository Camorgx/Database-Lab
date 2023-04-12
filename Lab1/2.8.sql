SELECT 
    readerID AS '读者号',
    COUNT(DISTINCT bookID) AS '借阅的不同图书数'
FROM ReaderBorrow
GROUP BY readerID;