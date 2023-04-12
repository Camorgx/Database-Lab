SELECT author AS '被借阅次数最多的作者'
FROM 
    (SELECT 
        author, 
        SUM(borrow_Times) AS times
    FROM Book GROUP BY author
    ORDER BY times DESC LIMIT 1) AS maxAuthor;